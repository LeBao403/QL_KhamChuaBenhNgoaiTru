from __future__ import annotations

import csv
import re
from dataclasses import dataclass
from pathlib import Path
from typing import Dict, List, Tuple

BASE = Path(__file__).resolve().parents[1]
SQL_PATH = BASE / 'Database' / 'training_seed_append.sql'
OUT_DIR = BASE / 'AI' / 'artifacts'
OUT_DIR.mkdir(parents=True, exist_ok=True)


@dataclass
class Disease:
    code: str
    name: str
    symptoms: str
    desc: str


@dataclass
class Drug:
    code: str
    name: str
    group: str
    route: str
    family: str


DISEASE_GROUP_RULES = {
    'Hô hấp': ['Hô hấp', 'Giảm đau hạ sốt', 'Kháng sinh', 'Kháng histamin', 'Bù nước - điện giải'],
    'Tiêu hóa': ['Dạ dày - tiêu hóa', 'Bù nước - điện giải', 'Giảm đau hạ sốt'],
    'Nội tiết - chuyển hóa': ['Nội tiết - chuyển hóa', 'Tim mạch'],
    'Cơ xương khớp': ['Cơ xương khớp', 'Giảm đau hạ sốt'],
    'Da liễu': ['Kháng histamin', 'Dạ dày - tiêu hóa'],
    'Tiết niệu': ['Kháng sinh', 'Bù nước - điện giải'],
    'Tai mắt mũi họng': ['Kháng sinh', 'Kháng histamin', 'Giảm đau hạ sốt'],
    'Thần kinh - tâm lý': ['Thần kinh - tâm lý', 'Giảm đau hạ sốt'],
}


def parse_values_block(sql: str, table: str) -> List[Tuple[str, ...]]:
    pat = rf"INSERT INTO {table} .*?VALUES\s*(.*?);\s*GO"
    m = re.search(pat, sql, flags=re.S | re.I)
    if not m:
        return []
    block = m.group(1)
    rows = re.findall(r"\((.*?)\)(?:,|$)", block, flags=re.S)
    out = []
    for row in rows:
        parts = re.findall(r"N?'(?:''|[^'])*'|NULL|\d+\.\d+|\d+", row)
        cleaned = []
        for p in parts:
            if p == 'NULL':
                cleaned.append('')
            elif p.startswith("N'") or p.startswith("'"):
                cleaned.append(p[2:-1].replace("''", "'") if p.startswith("N'") else p[1:-1].replace("''", "'"))
            else:
                cleaned.append(p)
        out.append(tuple(cleaned))
    return out


def load_seed() -> tuple[list[Disease], list[Drug], Dict[str, str], Dict[str, str]]:
    sql = SQL_PATH.read_text(encoding='utf-8', errors='ignore')
    diseases = [Disease(*r[:4]) for r in parse_values_block(sql, 'DANHMUC_BENH')]
    drug_groups = {code: name for code, name, *_ in parse_values_block(sql, 'DANHMUC_THUOC')}
    drugs = []
    for row in parse_values_block(sql, 'THUOC'):
        code = row[0]
        name = row[1]
        group_code = row[4]
        route = row[5]
        drugs.append(Drug(code, name, group_code, route, name))
    family_by_code = {code: family for _, code, family, *_ in parse_values_block(sql, 'THANHPHAN_THUOC')}
    return diseases, drugs, drug_groups, family_by_code


def infer_category(disease_name: str, symptoms: str, desc: str) -> str:
    text = f'{disease_name} {symptoms} {desc}'.lower()
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


def build_edges(diseases: list[Disease], drugs: list[Drug]) -> list[tuple[str, str, str]]:
    edges: list[tuple[str, str, str]] = []
    drug_by_group: Dict[str, list[Drug]] = {}
    for d in drugs:
        drug_by_group.setdefault(d.group, []).append(d)

    for disease in diseases:
        dnode = f'disease::{disease.code}'
        edges.append((dnode, f'text::{disease.name}', 'has_name'))
        for sym in re.split(r'[;,|]', disease.symptoms):
            sym = sym.strip()
            if sym:
                edges.append((dnode, f'symptom::{sym}', 'has_symptom'))

        cat = infer_category(disease.name, disease.symptoms, disease.desc)
        for grp in DISEASE_GROUP_RULES.get(cat, ['Giảm đau hạ sốt']):
            for drug in drug_by_group.get(grp, [])[:5]:
                edges.append((dnode, f'drug::{drug.code}', 'recommended'))
                edges.append((f'drug::{drug.code}', f'group::{drug.group}', 'in_group'))
                edges.append((f'drug::{drug.code}', f'family::{drug.family}', 'has_family'))

    # co-occurrence edges for drugs within same group/family
    for grp, items in drug_by_group.items():
        for i in range(len(items)):
            for j in range(i + 1, min(i + 4, len(items))):
                edges.append((f'drug::{items[i].code}', f'drug::{items[j].code}', 'co_drug'))

    return edges


def write_outputs(diseases: list[Disease], drugs: list[Drug], edges: list[tuple[str, str, str]]):
    with (OUT_DIR / 'nodes_diseases.csv').open('w', newline='', encoding='utf-8') as f:
        w = csv.writer(f)
        w.writerow(['node_id', 'label', 'type'])
        for d in diseases:
            w.writerow([f'disease::{d.code}', d.name, 'disease'])
            w.writerow([f'text::{d.name}', d.name, 'text'])
            for sym in re.split(r'[;,|]', d.symptoms):
                sym = sym.strip()
                if sym:
                    w.writerow([f'symptom::{sym}', sym, 'symptom'])
    with (OUT_DIR / 'nodes_drugs.csv').open('w', newline='', encoding='utf-8') as f:
        w = csv.writer(f)
        w.writerow(['node_id', 'label', 'type'])
        for d in drugs:
            w.writerow([f'drug::{d.code}', d.name, 'drug'])
            w.writerow([f'group::{d.group}', d.group, 'group'])
            w.writerow([f'family::{d.family}', d.family, 'family'])
    with (OUT_DIR / 'edges.csv').open('w', newline='', encoding='utf-8') as f:
        w = csv.writer(f)
        w.writerow(['src', 'dst', 'relation'])
        w.writerows(edges)


def main():
    diseases, drugs, _drug_groups, _family_by_code = load_seed()
    edges = build_edges(diseases, drugs)
    write_outputs(diseases, drugs, edges)
    print(f'Generated {len(diseases)} diseases, {len(drugs)} drugs, {len(edges)} edges')


if __name__ == '__main__':
    main()
