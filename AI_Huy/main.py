from datetime import datetime
from pathlib import Path
import json
import shutil

from fastapi import FastAPI, HTTPException
from fastapi.staticfiles import StaticFiles
from pydantic import BaseModel
from typing import List

from database import lay_du_lieu_don_thuoc
from custom_node2vec import CustomNode2Vec

app = FastAPI(title="He Thong Goi Y Thuoc AI")

MODELS_DIR = Path("models")
VERSIONS_DIR = MODELS_DIR / "versions"
REGISTRY_FILE = MODELS_DIR / "registry.json"
ACTIVE_FILE = MODELS_DIR / "active_model.json"

MODELS_DIR.mkdir(exist_ok=True)
VERSIONS_DIR.mkdir(exist_ok=True)

app.mount("/models", StaticFiles(directory="models"), name="models")

ai_model = CustomNode2Vec(dimensions=64, walk_length=10, num_walks=30, p=0.5, q=2.0, epochs=25, learning_rate=0.025)


def _now_iso():
    return datetime.now().isoformat(timespec="seconds")


def _read_json(path, default_value):
    if not path.exists():
        return default_value
    try:
        with path.open("r", encoding="utf-8") as f:
            return json.load(f)
    except Exception:
        return default_value


def _write_json(path, value):
    path.parent.mkdir(parents=True, exist_ok=True)
    with path.open("w", encoding="utf-8") as f:
        json.dump(value, f, ensure_ascii=False, indent=2)


def _read_registry():
    return _read_json(REGISTRY_FILE, {"models": []})


def _write_registry(registry):
    _write_json(REGISTRY_FILE, registry)


def _read_active_model_id():
    data = _read_json(ACTIVE_FILE, {})
    return data.get("model_id")


def _get_model_by_id(model_id):
    registry = _read_registry()
    return next((m for m in registry.get("models", []) if m.get("model_id") == model_id), None)


def _activate_model(model_id):
    model_info = _get_model_by_id(model_id)
    if not model_info:
        raise HTTPException(status_code=404, detail="Khong tim thay model.")

    weights_path = MODELS_DIR / model_info["weights_url"].replace("/models/", "")
    if not weights_path.exists():
        raise HTTPException(status_code=404, detail="File trong so model khong ton tai.")

    if not ai_model.load_model(str(weights_path)):
        raise HTTPException(status_code=500, detail="Khong nap duoc model.")

    _write_json(ACTIVE_FILE, {
        "model_id": model_id,
        "activated_at": _now_iso()
    })
    return model_info


def _bootstrap_legacy_model():
    active_id = _read_active_model_id()
    if active_id and _get_model_by_id(active_id):
        try:
            _activate_model(active_id)
            return
        except Exception:
            pass

    legacy_path = MODELS_DIR / "model_weights.json"
    if legacy_path.exists():
        ai_model.load_model(str(legacy_path))


_bootstrap_legacy_model()


class RecommendRequest(BaseModel):
    danh_sach_thuoc: List[str]
    top_k: int = 40


class SelectModelRequest(BaseModel):
    model_id: str


@app.get("/api/status")
def api_status():
    active_id = _read_active_model_id()
    active_model = _get_model_by_id(active_id) if active_id else None
    registry = _read_registry()
    return {
        "success": True,
        "service": "online",
        "active_model_id": active_id,
        "active_model": active_model,
        "model_loaded": bool(ai_model.vectors),
        "model_count": len(registry.get("models", [])),
        "checked_at": _now_iso()
    }


@app.get("/api/models")
def api_models():
    registry = _read_registry()
    active_id = _read_active_model_id()
    models = registry.get("models", [])
    return {
        "success": True,
        "active_model_id": active_id,
        "models": sorted(models, key=lambda m: m.get("created_at", ""), reverse=True)
    }


@app.post("/api/models/select")
def api_select_model(req: SelectModelRequest):
    model_info = _activate_model(req.model_id)
    return {
        "success": True,
        "message": "Da chon model thanh cong.",
        "active_model": model_info
    }


@app.post("/api/train")
def api_train_model(auto_activate: bool = True):
    try:
        du_lieu = lay_du_lieu_don_thuoc()
        if not du_lieu:
            return {
                "success": False,
                "message": "Khong co du du lieu tu Database de train."
            }

        model_id = "model_" + datetime.now().strftime("%Y%m%d_%H%M%S")
        output_dir = VERSIONS_DIR / model_id
        metrics = ai_model.train_and_evaluate(du_lieu, test_ratio=0.2, output_dir=str(output_dir))

        # Keep legacy file names fresh so older integrations still work.
        for file_name in ["model_weights.json", "drug_features.csv", "drug_graph_3d.png", "drug_graph_3d.html"]:
            src = output_dir / file_name
            if src.exists():
                shutil.copy2(src, MODELS_DIR / file_name)

        model_info = {
            "model_id": model_id,
            "created_at": _now_iso(),
            "total_prescriptions": metrics.get("total_prescriptions", len(du_lieu)),
            "train_count": metrics.get("train_count", 0),
            "test_count": metrics.get("test_count", 0),
            "valid_tests": metrics.get("valid_tests", 0),
            "hit_rates": metrics.get("hit_rates", {}),
            "weights_url": f"/models/versions/{model_id}/model_weights.json",
            "graph_url_static": f"/models/versions/{model_id}/drug_graph_3d.png",
            "graph_url_interactive": f"/models/versions/{model_id}/drug_graph_3d.html",
            "csv_url": f"/models/versions/{model_id}/drug_features.csv"
        }

        registry = _read_registry()
        registry.setdefault("models", [])
        registry["models"].append(model_info)
        _write_registry(registry)
        if auto_activate:
            _activate_model(model_id)

        return {
            "success": True,
            "message": "Huan luyen AI thanh cong!",
            **model_info
        }
    except HTTPException:
        raise
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))


@app.get("/api/recommend")
def api_recommend_drug(node_id: str, top_k: int = 40):
    if not ai_model.vectors:
        return {"success": False, "message": "Model chua duoc train. Hay goi API train truoc."}

    goi_y = ai_model.get_similar_drugs_from_list([node_id], top_k=top_k)
    if not goi_y:
        return {"success": True, "MaThuocGoc": node_id, "GoiY": []}

    return {"success": True, "MaThuocGoc": node_id, "GoiY": goi_y}
