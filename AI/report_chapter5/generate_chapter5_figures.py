import json
import math
from pathlib import Path

import matplotlib.pyplot as plt
import numpy as np


BASE_DIR = Path(__file__).resolve().parent
AI_DIR = BASE_DIR.parent
MODELS_DIR = AI_DIR / "models"
ASSET_DIR = BASE_DIR / "assets"
ASSET_DIR.mkdir(parents=True, exist_ok=True)


def load_active_model_info():
    registry_path = MODELS_DIR / "registry.json"
    active_path = MODELS_DIR / "active_model.json"
    if not registry_path.exists():
        return None

    registry = json.loads(registry_path.read_text(encoding="utf-8"))
    active_id = None
    if active_path.exists():
        active_id = json.loads(active_path.read_text(encoding="utf-8")).get("model_id")

    models = registry.get("models", [])
    if active_id:
        for item in models:
            if item.get("model_id") == active_id:
                return item
    return models[-1] if models else None


def load_vectors():
    weights_path = MODELS_DIR / "model_weights.json"
    if not weights_path.exists():
        return {}, np.empty((0, 0))
    data = json.loads(weights_path.read_text(encoding="utf-8"))
    labels = list(data.keys())
    matrix = np.array([data[k] for k in labels], dtype=float)
    return labels, matrix


def savefig(name):
    out = ASSET_DIR / name
    plt.tight_layout()
    plt.savefig(out, dpi=220, bbox_inches="tight")
    plt.close()
    print(out)


def draw_pipeline():
    fig, ax = plt.subplots(figsize=(12.5, 3.2))
    ax.axis("off")
    ax.set_xlim(0, 1)
    ax.set_ylim(0, 1)
    steps = [
        ("SQL Server", "CT_DON_THUOC\nDON_THUOC"),
        ("Tiền xử lý", "Gom thuốc theo\nMaDonThuoc"),
        ("Đồ thị thuốc", "Node = thuốc\nEdge = đồng kê đơn"),
        ("Node2Vec", "Biased Random Walk\np = 0.5, q = 2.0"),
        ("Skip-gram", "Negative Sampling\nLearning rate decay"),
        ("API gợi ý", "Cosine similarity\nTop-K thuốc"),
    ]
    x_positions = np.linspace(0.09, 0.91, len(steps))
    for i, (title, body) in enumerate(steps):
        x = x_positions[i]
        rect = plt.Rectangle((x - 0.06, 0.34), 0.12, 0.34, facecolor="#eef6ff", edgecolor="#2f6f9f", linewidth=1.5)
        ax.add_patch(rect)
        ax.text(x, 0.58, title, ha="center", va="center", fontsize=11, fontweight="bold", color="#16324f")
        ax.text(x, 0.43, body, ha="center", va="center", fontsize=9, color="#263238")
        if i < len(steps) - 1:
            ax.annotate("", xy=(x_positions[i + 1] - 0.072, 0.51), xytext=(x + 0.072, 0.51),
                        arrowprops=dict(arrowstyle="->", lw=1.6, color="#5f6f7f"))
    ax.set_title("Quy trình tổng thể của mô-đun AI gợi ý thuốc", fontsize=14, fontweight="bold", pad=8)
    savefig("fig_5_1_pipeline_ai.png")


def draw_graph_construction():
    fig, axes = plt.subplots(1, 2, figsize=(11.5, 4.2))
    for ax in axes:
        ax.axis("off")

    prescriptions = [
        ("DT001", ["TH001", "TH002", "TH003"]),
        ("DT002", ["TH001", "TH002"]),
        ("DT003", ["TH002", "TH004"]),
        ("DT004", ["TH003", "TH004"]),
    ]
    axes[0].set_title("Dữ liệu đơn thuốc sau gom nhóm", fontsize=12, fontweight="bold")
    y = 0.82
    for pid, drugs in prescriptions:
        axes[0].text(0.08, y, pid, fontsize=11, fontweight="bold", color="#174a7c")
        axes[0].text(0.28, y, " - ".join(drugs), fontsize=11)
        y -= 0.17

    axes[1].set_title("Đồ thị đồng kê đơn có trọng số", fontsize=12, fontweight="bold")
    pos = {
        "TH001": np.array([0.25, 0.70]),
        "TH002": np.array([0.58, 0.72]),
        "TH003": np.array([0.25, 0.32]),
        "TH004": np.array([0.63, 0.34]),
    }
    edges = [("TH001", "TH002", 2), ("TH001", "TH003", 1), ("TH002", "TH003", 1), ("TH002", "TH004", 1), ("TH003", "TH004", 1)]
    for u, v, w in edges:
        p1, p2 = pos[u], pos[v]
        axes[1].plot([p1[0], p2[0]], [p1[1], p2[1]], color="#78909c", lw=1.3 + w)
        mid = (p1 + p2) / 2
        axes[1].text(mid[0], mid[1] + 0.035, f"w={w}", fontsize=9, color="#455a64", ha="center")
    for node, p in pos.items():
        circ = plt.Circle(p, 0.07, facecolor="#fff3e0", edgecolor="#ef6c00", linewidth=1.5)
        axes[1].add_patch(circ)
        axes[1].text(p[0], p[1], node, ha="center", va="center", fontsize=10, fontweight="bold")
    savefig("fig_5_2_graph_construction.png")


def draw_graph_definition():
    fig, ax = plt.subplots(figsize=(12.2, 6.4))
    ax.axis("off")
    ax.set_xlim(0, 1)
    ax.set_ylim(0, 1)

    ax.text(0.5, 0.94, "Biểu diễn đồ thị đồng kê đơn thuốc G = (V, E)",
            ha="center", va="center", fontsize=16, fontweight="bold")

    info_box = plt.Rectangle((0.05, 0.62), 0.38, 0.22, facecolor="#eef6ff", edgecolor="#2f6f9f", linewidth=1.4)
    ax.add_patch(info_box)
    ax.text(0.07, 0.79, "G = (V, E)", fontsize=14, fontweight="bold", color="#16324f")
    ax.text(0.07, 0.735, "V = {TH001, TH002, TH003, TH004}", fontsize=10.5, color="#263238")
    ax.text(0.07, 0.685, "E = {(TH001,TH002), (TH001,TH003), ...}", fontsize=10.5, color="#263238")
    ax.text(0.07, 0.635, "w(u,v) = số lần thuốc u và v được kê chung", fontsize=10.5, color="#263238")

    table_box = plt.Rectangle((0.05, 0.18), 0.38, 0.32, facecolor="#fff8e8", edgecolor="#c7861b", linewidth=1.4)
    ax.add_patch(table_box)
    ax.text(0.07, 0.455, "Ví dụ dữ liệu đơn thuốc", fontsize=12, fontweight="bold", color="#6b4300")
    prescriptions = [
        ("DT001", "TH001, TH002, TH003"),
        ("DT002", "TH001, TH002"),
        ("DT003", "TH002, TH004"),
        ("DT004", "TH003, TH004"),
    ]
    y = 0.395
    for pid, drugs in prescriptions:
        ax.text(0.08, y, pid, fontsize=10.5, fontweight="bold", color="#174a7c")
        ax.text(0.19, y, drugs, fontsize=10.5, color="#263238")
        y -= 0.06

    pos = {
        "TH001": np.array([0.60, 0.70]),
        "TH002": np.array([0.82, 0.70]),
        "TH003": np.array([0.60, 0.34]),
        "TH004": np.array([0.82, 0.34]),
    }
    edges = [
        ("TH001", "TH002", 2),
        ("TH001", "TH003", 1),
        ("TH002", "TH003", 1),
        ("TH002", "TH004", 1),
        ("TH003", "TH004", 1),
    ]
    ax.text(0.71, 0.84, "Đồ thị G", ha="center", fontsize=13, fontweight="bold", color="#16324f")
    for u, v, w in edges:
        p1, p2 = pos[u], pos[v]
        ax.plot([p1[0], p2[0]], [p1[1], p2[1]], color="#607d8b", lw=1.4 + w)
        mid = (p1 + p2) / 2
        ax.text(mid[0], mid[1] + 0.025, f"w={w}", fontsize=10, color="#37474f",
                ha="center", bbox=dict(facecolor="white", edgecolor="none", alpha=0.75, pad=1))

    for node, p in pos.items():
        circ = plt.Circle(p, 0.055, facecolor="#e8f5e9", edgecolor="#2e7d32", linewidth=1.5)
        ax.add_patch(circ)
        ax.text(p[0], p[1], node, ha="center", va="center", fontsize=10.5, fontweight="bold")

    ax.annotate("Đỉnh thuộc V\n(mã thuốc duy nhất)", xy=pos["TH001"], xytext=(0.49, 0.82),
                arrowprops=dict(arrowstyle="->", lw=1.2, color="#455a64"),
                fontsize=10, ha="center", color="#263238")
    ax.annotate("Cạnh thuộc E\n(cùng xuất hiện trong đơn)", xy=(0.71, 0.70), xytext=(0.71, 0.56),
                arrowprops=dict(arrowstyle="->", lw=1.2, color="#455a64"),
                fontsize=10, ha="center", color="#263238")
    ax.annotate("Trọng số cạnh w(u,v)\n= số lần kê chung", xy=(0.71, 0.72), xytext=(0.92, 0.82),
                arrowprops=dict(arrowstyle="->", lw=1.2, color="#455a64"),
                fontsize=10, ha="center", color="#263238")

    savefig("fig_5_2b_graph_definition_GVE.png")


def draw_biased_walk():
    p = 0.5
    q = 2.0
    weights = {
        "Quay lại t": 1 / p,
        "Đi gần t": 1.0,
        "Đi xa t": 1 / q,
    }
    total = sum(weights.values())
    probs = {k: v / total for k, v in weights.items()}

    fig, ax = plt.subplots(figsize=(8.5, 4.6))
    bars = ax.bar(probs.keys(), [v * 100 for v in probs.values()], color=["#2f80ed", "#27ae60", "#f2994a"])
    ax.set_ylabel("Xác suất sau chuẩn hóa (%)")
    ax.set_title("Ảnh hưởng của p = 0,5 và q = 2,0 đến hướng random walk", fontsize=13, fontweight="bold")
    ax.set_ylim(0, max(v * 100 for v in probs.values()) + 15)
    ax.grid(axis="y", alpha=0.25)
    for bar in bars:
        h = bar.get_height()
        ax.text(bar.get_x() + bar.get_width() / 2, h + 1.2, f"{h:.1f}%", ha="center", fontsize=10, fontweight="bold")
    ax.text(0.5, -0.22, "Minh họa khi ba loại lựa chọn có cùng trọng số cạnh gốc w(v,x).",
            transform=ax.transAxes, ha="center", fontsize=9, color="#455a64")
    savefig("fig_5_3_pq_transition_bias.png")


def pca_2d(matrix):
    x = matrix - matrix.mean(axis=0, keepdims=True)
    _, _, vt = np.linalg.svd(x, full_matrices=False)
    return x @ vt[:2].T


def draw_embedding_2d():
    labels, matrix = load_vectors()
    if len(labels) == 0:
        fig, ax = plt.subplots(figsize=(9, 5))
        ax.text(0.5, 0.5, "Chưa có model_weights.json để trực quan hóa embedding.", ha="center", va="center")
        ax.axis("off")
        savefig("fig_5_4_embedding_pca_2d.png")
        return

    coords = pca_2d(matrix)
    norms = np.linalg.norm(matrix, axis=1)
    top_idx = np.argsort(norms)[-18:]

    fig, ax = plt.subplots(figsize=(9.5, 6.2))
    ax.scatter(coords[:, 0], coords[:, 1], s=28, alpha=0.65, color="#5b8def", edgecolor="white", linewidth=0.35)
    ax.scatter(coords[top_idx, 0], coords[top_idx, 1], s=48, color="#f2994a", edgecolor="#7a3b00", linewidth=0.5)
    for idx in top_idx:
        ax.text(coords[idx, 0], coords[idx, 1], labels[idx], fontsize=7.5, ha="left", va="bottom")
    ax.set_title("Không gian vector thuốc sau giảm chiều PCA 2D", fontsize=13, fontweight="bold")
    ax.set_xlabel("Thành phần chính 1")
    ax.set_ylabel("Thành phần chính 2")
    ax.grid(alpha=0.2)
    savefig("fig_5_4_embedding_pca_2d.png")


def draw_hit_rates():
    model = load_active_model_info()
    if not model:
        ks = [10, 20, 30]
        rates = [0, 0, 0]
        title = "Hit@K của mô hình"
    else:
        hit_rates = model.get("hit_rates", {})
        ks = sorted(int(k) for k in hit_rates.keys())
        rates = [hit_rates[str(k)]["rate"] for k in ks]
        title = f"Hit@K của model đang dùng: {model.get('model_id', '')}"

    fig, ax = plt.subplots(figsize=(8.5, 4.6))
    bars = ax.bar([f"Hit@{k}" for k in ks], rates, color=["#56cc9d", "#2f80ed", "#9b51e0"])
    ax.set_ylim(0, max(rates + [10]) + 12)
    ax.set_ylabel("Tỷ lệ trúng (%)")
    ax.set_title(title, fontsize=13, fontweight="bold")
    ax.grid(axis="y", alpha=0.25)
    for bar, rate in zip(bars, rates):
        ax.text(bar.get_x() + bar.get_width() / 2, bar.get_height() + 1.0, f"{rate:.2f}%", ha="center", fontsize=10, fontweight="bold")
    if model:
        ax.text(0.5, -0.20, f"Tổng toa: {model.get('total_prescriptions')} | Train: {model.get('train_count')} | Test hợp lệ: {model.get('valid_tests')}",
                transform=ax.transAxes, ha="center", fontsize=9, color="#455a64")
    savefig("fig_5_5_hit_at_k.png")


def draw_api_sequence():
    fig, ax = plt.subplots(figsize=(11, 4.8))
    ax.axis("off")
    lanes = [("Bác sĩ/Web App", 0.15), ("AI Service FastAPI", 0.50), ("SQL Server/Model", 0.84)]
    for title, x in lanes:
        ax.text(x, 0.92, title, ha="center", va="center", fontsize=11, fontweight="bold",
                bbox=dict(boxstyle="round,pad=0.35", facecolor="#eef6ff", edgecolor="#2f6f9f"))
        ax.plot([x, x], [0.12, 0.86], color="#b0bec5", linestyle="--", lw=1)
    messages = [
        (0.15, 0.50, 0.78, "POST /api/train"),
        (0.50, 0.84, 0.66, "Đọc CT_DON_THUOC"),
        (0.84, 0.50, 0.54, "Danh sách toa thuốc"),
        (0.50, 0.84, 0.42, "Lưu model_weights.json"),
        (0.15, 0.50, 0.30, "GET /api/recommend?node_id=..."),
        (0.50, 0.15, 0.18, "Top-K thuốc + độ tương đồng"),
    ]
    for x1, x2, y, text in messages:
        ax.annotate("", xy=(x2, y), xytext=(x1, y), arrowprops=dict(arrowstyle="->", lw=1.4, color="#455a64"))
        ax.text((x1 + x2) / 2, y + 0.025, text, ha="center", fontsize=9, color="#263238")
    ax.set_title("Luồng gọi API huấn luyện và gợi ý thuốc", fontsize=13, fontweight="bold")
    savefig("fig_5_6_api_sequence.png")


def main():
    draw_pipeline()
    draw_graph_construction()
    draw_graph_definition()
    draw_biased_walk()
    draw_embedding_2d()
    draw_hit_rates()
    draw_api_sequence()


if __name__ == "__main__":
    main()
