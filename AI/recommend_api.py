from __future__ import annotations

from pathlib import Path
from typing import List

from fastapi import FastAPI
from pydantic import BaseModel
import csv
import math
import re

BASE = Path(__file__).resolve().parent
ART = BASE / 'artifacts'
EMB_FILE = ART / 'embeddings.csv'

app = FastAPI(title='QLKCB Drug Recommendation API')


def load_embeddings() -> dict[str, list[float]]:
    emb = {}
    if not EMB_FILE.exists():
        return emb
    with EMB_FILE.open(encoding='utf-8') as f:
        r = csv.DictReader(f)
        for row in r:
            emb[row['node_id']] = [float(row[f'e{i}']) for i in range(64)]
    return emb


EMB = load_embeddings()


AGE_GROUP_RULES = {
    'TreEm': 'TreEm',
    'ThieuNien': 'ThieuNien',
    'NguoiLonTre': 'NguoiLonTre',
    'TrungNien': 'TrungNien',
    'NguoiCaoTuoi': 'NguoiCaoTuoi',
}


class Candidate(BaseModel):
    drug_code: str
    drug_name: str
    drug_group: str | None = None
    route: str | None = None
    drug_family: str | None = None
    in_stock: int = 1
    contraindication: int = 0
    interaction_risk: int = 0
    is_bhyt: int = 0


class RecommendRequest(BaseModel):
    age: int = 30
    sex: str = 'Nam'
    age_group: str = 'NguoiLonTre'
    disease: str = ''
    symptoms: str = ''
    comorbidity: str = 'None'
    allergy: str = 'None'
    has_bhyt: bool = False
    candidates: List[Candidate]


def infer_category(text: str) -> str:
    text = text.lower()
    if any(k in text for k in ['ho', 'sốt', 'mũi', 'họng', 'phế quản', 'phổi', 'xoang']):
        return 'Hô hấp'
    if any(k in text for k in ['dạ dày', 'ruột', 'tiêu chảy', 'táo bón', 'bụng', 'gan']):
        return 'Tiêu hóa'
    if any(k in text for k in ['đái tháo đường', 'huyết áp', 'mỡ máu', 'uric', 'chuyển hóa']):
        return 'Nội tiết - chuyển hóa'
    if any(k in text for k in ['khớp', 'xương', 'gối', 'gân', 'vai', 'lưng']):
        return 'Cơ xương khớp'
    if any(k in text for k in ['da', 'ngứa', 'mẩn', 'nấm', 'mụn', 'dị ứng']):
        return 'Da liễu'
    if any(k in text for k in ['tiểu', 'thận', 'bàng quang', 'niệu']):
        return 'Tiết niệu'
    if any(k in text for k in ['tai', 'mắt', 'mũi', 'họng', 'răng', 'xoang']):
        return 'Tai mắt mũi họng'
    if any(k in text for k in ['đầu', 'lo âu', 'stress', 'mất ngủ', 'tâm lý', 'run']):
        return 'Thần kinh - tâm lý'
    return 'Hô hấp'


RULES = {
    'Hô hấp': ['Hô hấp', 'Giảm đau hạ sốt', 'Kháng sinh', 'Kháng histamin', 'Bù nước - điện giải'],
    'Tiêu hóa': ['Dạ dày - tiêu hóa', 'Bù nước - điện giải', 'Giảm đau hạ sốt'],
    'Nội tiết - chuyển hóa': ['Nội tiết - chuyển hóa', 'Tim mạch'],
    'Cơ xương khớp': ['Cơ xương khớp', 'Giảm đau hạ sốt'],
    'Da liễu': ['Kháng histamin', 'Dạ dày - tiêu hóa'],
    'Tiết niệu': ['Kháng sinh', 'Bù nước - điện giải'],
    'Tai mắt mũi họng': ['Kháng sinh', 'Kháng histamin', 'Giảm đau hạ sốt'],
    'Thần kinh - tâm lý': ['Thần kinh - tâm lý', 'Giảm đau hạ sốt'],
}


def safe_sigmoid(x: float) -> float:
    return 1 / (1 + math.exp(-x))


def embed_similarity(query_text: str, drug_text: str) -> float:
    if not EMB:
        return 0.0

    def avg_vec(text: str) -> list[float] | None:
        toks = [t for t in re.split(r"[^\wÀ-ỹ]+", text.lower()) if t]
        vecs = [EMB[t] for t in toks if t in EMB]
        if not vecs:
            return None
        dim = len(vecs[0])
        return [sum(v[i] for v in vecs) / len(vecs) for i in range(dim)]

    qv = avg_vec(query_text)
    dv = avg_vec(drug_text)
    if not qv or not dv:
        return 0.0

    dot = sum(a * b for a, b in zip(qv, dv))
    nq = math.sqrt(sum(a * a for a in qv))
    nd = math.sqrt(sum(a * a for a in dv))
    if nq == 0 or nd == 0:
        return 0.0
    return dot / (nq * nd)


def score_candidate(req: RecommendRequest, c: Candidate) -> float:
    """Score mềm, ưu tiên theo nhóm bệnh, text match và tín hiệu an toàn."""
    disease_cat = infer_category(f'{req.disease} {req.symptoms}')
    allowed = RULES.get(disease_cat, [])

    query_text = f"{req.disease or ''} {req.symptoms or ''} {req.comorbidity or ''} {req.allergy or ''}".lower()
    query_tokens = [t for t in re.split(r"[^\wÀ-ỹ]+", query_text) if len(t) >= 2]

    drug_text = f"{c.drug_name or ''} {c.drug_family or ''} {c.drug_group or ''} {c.route or ''}".lower()
    token_hits = sum(1 for t in set(query_tokens) if t in drug_text)
    sim = embed_similarity(query_text, drug_text)

    score = 0.18
    if c.drug_group and c.drug_group in allowed:
        score += 0.22

    score += min(0.28, token_hits * 0.06)
    score += max(0.0, min(0.18, (sim + 1) / 2 * 0.18))

    if c.in_stock:
        score += 0.05
    else:
        score -= 0.10

    if req.has_bhyt and c.is_bhyt:
        score += 0.35 # Prioritize BHYT items when patient has BHYT

    if c.contraindication:
        score -= 0.45
    if c.interaction_risk:
        score -= 0.18

    # Tránh trả toàn 0 dù dữ liệu candidate nghèo
    score = max(0.01, min(0.99, score))
    return score


@app.post('/recommend')
def recommend(req: RecommendRequest):
    disease_cat = infer_category(f'{req.disease} {req.symptoms}')
    items = []
    for c in req.candidates:
        score = score_candidate(req, c)
        reasons = [f'Nhóm bệnh: {disease_cat}']
        if c.drug_group and c.drug_group in RULES.get(disease_cat, []):
            reasons.append(f'Phù hợp nhóm thuốc {c.drug_group}')
        if c.in_stock:
            reasons.append('Còn trong kho')
        if req.has_bhyt and c.is_bhyt:
            reasons.append('Thuốc được hưởng BHYT')
        if c.contraindication:
            reasons.append('Có tín hiệu chống chỉ định')
        if c.interaction_risk:
            reasons.append('Có tín hiệu tương tác')

        items.append({
            'drug_code': c.drug_code,
            'drug_name': c.drug_name,
            'drug_group': c.drug_group,
            'route': c.route,
            'score': round(score, 4),
            'reason': '; '.join(reasons),
        })

    items.sort(key=lambda x: x['score'], reverse=True)
    return {'success': True, 'items': items[:20], 'meta': {'disease_category': disease_cat, 'candidate_count': len(req.candidates)}}


@app.post('/recommend-form')
def recommend_form(age: int = 30, sex: str = 'Nam', age_group: str = 'NguoiLonTre', disease: str = '', symptoms: str = '', comorbidity: str = 'None', allergy: str = 'None'):
    # Fallback cho client nào gửi form-urlencoded thay vì JSON.
    req = RecommendRequest(
        age=age,
        sex=sex,
        age_group=age_group,
        disease=disease,
        symptoms=symptoms,
        comorbidity=comorbidity,
        allergy=allergy,
        candidates=[],
    )
    return {'success': True, 'items': [], 'meta': {'disease_category': infer_category(f'{disease} {symptoms}')}}
