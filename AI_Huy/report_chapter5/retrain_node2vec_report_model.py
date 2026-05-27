import json
import shutil
import sys
from datetime import datetime
from pathlib import Path


BASE_DIR = Path(__file__).resolve().parent
AI_DIR = BASE_DIR.parent
MODELS_DIR = AI_DIR / "models"
VERSIONS_DIR = MODELS_DIR / "versions"

sys.path.insert(0, str(AI_DIR))

from custom_node2vec import CustomNode2Vec
from database import lay_du_lieu_don_thuoc


def write_json(path, value):
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(json.dumps(value, ensure_ascii=False, indent=2), encoding="utf-8")


def read_json(path, default):
    if not path.exists():
        return default
    try:
        return json.loads(path.read_text(encoding="utf-8"))
    except Exception:
        return default


def main():
    model_id = "model_report_" + datetime.now().strftime("%Y%m%d_%H%M%S")
    output_dir = VERSIONS_DIR / model_id
    output_dir.mkdir(parents=True, exist_ok=True)

    print("Dang doc du lieu don thuoc...")
    prescriptions = lay_du_lieu_don_thuoc()
    if not prescriptions:
        raise RuntimeError("Khong co du lieu de train.")

    model = CustomNode2Vec(
        dimensions=64,
        walk_length=10,
        num_walks=80,
        p=0.5,
        q=2.0,
        window_size=5,
        epochs=50,
        learning_rate=0.025,
    )

    metrics = model.train_and_evaluate(
        prescriptions,
        test_ratio=0.2,
        output_dir=str(output_dir),
    )

    for filename in ["model_weights.json", "drug_features.csv", "drug_graph_3d.png", "drug_graph_3d.html"]:
        src = output_dir / filename
        if src.exists():
            shutil.copy2(src, MODELS_DIR / filename)

    model_info = {
        "model_id": model_id,
        "created_at": datetime.now().isoformat(timespec="seconds"),
        "total_prescriptions": metrics.get("total_prescriptions", len(prescriptions)),
        "train_count": metrics.get("train_count", 0),
        "test_count": metrics.get("test_count", 0),
        "valid_tests": metrics.get("valid_tests", 0),
        "hit_rates": metrics.get("hit_rates", {}),
        "params": {
            "dimensions": 64,
            "walk_length": 10,
            "num_walks": 80,
            "p": 0.5,
            "q": 2.0,
            "window_size": 5,
            "epochs": 50,
            "learning_rate": 0.025,
        },
        "weights_url": f"/models/versions/{model_id}/model_weights.json",
        "graph_url_static": f"/models/versions/{model_id}/drug_graph_3d.png",
        "graph_url_interactive": f"/models/versions/{model_id}/drug_graph_3d.html",
        "csv_url": f"/models/versions/{model_id}/drug_features.csv",
    }

    registry_path = MODELS_DIR / "registry.json"
    registry = read_json(registry_path, {"models": []})
    registry.setdefault("models", []).append(model_info)
    write_json(registry_path, registry)
    write_json(MODELS_DIR / "active_model.json", {
        "model_id": model_id,
        "activated_at": datetime.now().isoformat(timespec="seconds"),
    })

    print("Da train xong model moi:")
    print(output_dir)
    print("Da copy model moi sang AI_Huy/models de cac script bieu do doc dung embedding moi.")


if __name__ == "__main__":
    main()
