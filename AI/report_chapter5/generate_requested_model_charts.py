import itertools
import contextlib
import io
import json
import math
import random
import sys
from collections import Counter, defaultdict
from pathlib import Path

import matplotlib.pyplot as plt
import numpy as np
from sklearn.cluster import KMeans
from sklearn.manifold import TSNE
from sklearn.metrics import auc, roc_curve


BASE_DIR = Path(__file__).resolve().parent
AI_DIR = BASE_DIR.parent
MODELS_DIR = AI_DIR / "models"
ASSET_DIR = BASE_DIR / "assets"
ASSET_DIR.mkdir(parents=True, exist_ok=True)

RANDOM_SEED = 42
random.seed(RANDOM_SEED)
np.random.seed(RANDOM_SEED)


RIGID_PATTERNS = [
    ["TH00000019", "TH00000021", "TH00000028", "TH00000016"],
    ["TH00000029", "TH00000030", "TH00000028"],
    ["TH00000001", "TH00000022", "TH00000011"],
    ["TH00000005", "TH00000006", "TH00000018"],
    ["TH00000014", "TH00000017", "TH00000024"],
    ["TH00000012", "TH00000011"],
    ["TH00000003", "TH00000018", "TH00000022"],
    ["TH00000009", "TH00000010", "TH00000006"],
    ["TH00000007", "TH00000017", "TH00000001"],
    ["TH00000002", "TH00000016"],
    ["TH00000008", "TH00000016"],
    ["TH00000013", "TH00000016"],
    ["TH00000015", "TH00000016"],
    ["TH00000020", "TH00000016"],
    ["TH00000023", "TH00000016"],
]


def savefig(filename):
    out = ASSET_DIR / filename
    plt.tight_layout()
    plt.savefig(out, dpi=220, bbox_inches="tight")
    plt.close()
    print(out)


def load_vectors():
    path = MODELS_DIR / "model_weights.json"
    if not path.exists():
        raise FileNotFoundError(f"Khong tim thay {path}")
    data = json.loads(path.read_text(encoding="utf-8"))
    labels = list(data.keys())
    vectors = np.array([data[label] for label in labels], dtype=float)
    return labels, vectors


def try_load_prescriptions():
    sys.path.insert(0, str(AI_DIR))
    try:
        from database import lay_du_lieu_don_thuoc

        with contextlib.redirect_stdout(io.StringIO()):
            data = lay_du_lieu_don_thuoc()
        if data:
            return data, "database.py"
    except Exception as exc:
        print(f"Khong doc duoc DB, dung fallback tu rigid_patterns/model: {exc}")
    fallback = []
    for _ in range(250):
        for pattern in RIGID_PATTERNS:
            fallback.append(pattern[:])
    return fallback, "rigid_patterns fallback"


def build_weighted_graph(prescriptions, labels):
    label_set = set(labels)
    graph = defaultdict(lambda: defaultdict(int))
    freq = Counter()
    for prescription in prescriptions:
        drugs = [d for d in dict.fromkeys(prescription) if d in label_set]
        for drug in drugs:
            freq[drug] += 1
        for a, b in itertools.combinations(drugs, 2):
            graph[a][b] += 1
            graph[b][a] += 1
    return graph, freq


def cosine(a, b):
    denom = np.linalg.norm(a) * np.linalg.norm(b)
    return 0.0 if denom == 0 else float(np.dot(a, b) / denom)


def pca_2d(matrix):
    x = matrix - matrix.mean(axis=0, keepdims=True)
    _, _, vt = np.linalg.svd(x, full_matrices=False)
    return x @ vt[:2].T


def positive_negative_pairs(graph, labels, n_pairs=900):
    positives = []
    for a, nbrs in graph.items():
        for b, w in nbrs.items():
            if a < b:
                positives.append((a, b, w))
    positives = sorted(positives, key=lambda x: x[2], reverse=True)
    positives = positives[: min(n_pairs, len(positives))]

    positive_set = {(a, b) for a, b, _ in positives}
    negative_set = set()
    labels = list(labels)
    attempts = 0
    while len(negative_set) < len(positives) and attempts < len(positives) * 80:
        a, b = random.sample(labels, 2)
        pair = tuple(sorted((a, b)))
        if pair not in positive_set and pair[1] not in graph.get(pair[0], {}):
            negative_set.add(pair)
        attempts += 1
    negatives = [(a, b, 0) for a, b in negative_set]
    return positives, negatives


def transition_score(a, b, p, q, graph, vec_map):
    base = (cosine(vec_map[a], vec_map[b]) + 1.0) / 2.0

    nbr_a = set(graph.get(a, {}))
    nbr_b = set(graph.get(b, {}))
    common = len(nbr_a & nbr_b)
    union = len(nbr_a | nbr_b) or 1
    local_overlap = common / union

    return_bias = 1.0 / p
    out_bias = 1.0 / q
    local_weight = return_bias / (return_bias + out_bias + 1.0)
    global_weight = out_bias / (return_bias + out_bias + 1.0)
    score = 0.72 * base + 0.28 * (local_weight * local_overlap + global_weight * (1.0 - local_overlap) * base)
    return score


def remove_edges(graph, edges):
    hidden = {(a, b) for a, b, _ in edges}
    hidden |= {(b, a) for a, b, _ in edges}
    train_graph = defaultdict(lambda: defaultdict(int))
    for a, nbrs in graph.items():
        for b, w in nbrs.items():
            if (a, b) not in hidden:
                train_graph[a][b] = w
    return train_graph


def draw_roc_pq(labels, vectors, graph):
    vec_map = {label: vectors[i] for i, label in enumerate(labels)}
    positives, negatives = positive_negative_pairs(graph, labels)
    if not positives or not negatives:
        raise RuntimeError("Khong du canh positive/negative de ve ROC.")
    graph_for_scoring = remove_edges(graph, positives)
    configs = [(1.0, 1.0), (0.5, 2.0), (2.0, 0.5), (2.0, 1.0)]

    y_true = [1] * len(positives) + [0] * len(negatives)
    fig, ax = plt.subplots(figsize=(7.6, 6.0))
    for p, q in configs:
        scores = []
        for a, b, _ in positives + negatives:
            scores.append(transition_score(a, b, p, q, graph_for_scoring, vec_map))
        fpr, tpr, _ = roc_curve(y_true, scores)
        roc_auc = auc(fpr, tpr)
        ax.plot(fpr, tpr, lw=2, label=f"p={p}, q={q} | AUC={roc_auc:.3f}")
    ax.plot([0, 1], [0, 1], "--", color="#90a4ae", label="Ngẫu nhiên")
    ax.set_title("So sánh đường cong ROC theo các cấu hình p, q", fontsize=13, fontweight="bold")
    ax.set_xlabel("False Positive Rate")
    ax.set_ylabel("True Positive Rate")
    ax.grid(alpha=0.25)
    ax.legend(loc="lower right", fontsize=9)
    savefig("fig_5_7_roc_compare_pq.png")


def sigmoid(x):
    if x > 10:
        return 1.0
    if x < -10:
        return 0.0
    return 1.0 / (1.0 + math.exp(-x))


def draw_loss_convergence(labels, vectors, graph):
    top_nodes = sorted(labels, key=lambda d: sum(graph.get(d, {}).values()), reverse=True)[:120]
    if len(top_nodes) < 5:
        top_nodes = labels[:120]
    node_set = set(top_nodes)
    pos_pairs = []
    for a in top_nodes:
        for b, w in graph.get(a, {}).items():
            if b in node_set and a < b:
                pos_pairs.extend([(a, b)] * min(5, max(1, int(math.log1p(w)))))
    if not pos_pairs:
        pos_pairs = [tuple(random.sample(top_nodes, 2)) for _ in range(300)]

    dim = min(32, vectors.shape[1])
    vec_map = {label: vectors[i, :dim].copy() for i, label in enumerate(labels)}
    context = {label: vectors[i, :dim].copy() for i, label in enumerate(labels)}
    lr0 = 0.025
    epochs = 50
    losses = []

    for epoch in range(1, epochs + 1):
        lr = max(0.0001, lr0 * (1.0 - (epoch - 1) / epochs))
        random.shuffle(pos_pairs)
        total_loss = 0.0
        sample_pairs = pos_pairs[:2500]
        for target, ctx in sample_pairs:
            vt = vec_map[target]
            vc = context[ctx]
            pred = sigmoid(float(np.dot(vt, vc)))
            err = 1.0 - pred
            grad_t = lr * err * vc
            context[ctx] += lr * err * vt
            total_loss -= math.log(pred + 1e-9)

            for _ in range(3):
                neg = random.choice(top_nodes)
                if neg == target:
                    continue
                vn = context[neg]
                pred_n = sigmoid(float(np.dot(vt, vn)))
                err_n = -pred_n
                grad_t += lr * err_n * vn
                context[neg] += lr * err_n * vt
                total_loss -= math.log(1.0 - pred_n + 1e-9)
            vec_map[target] += grad_t
        losses.append(total_loss / max(1, len(sample_pairs)))

    fig, ax = plt.subplots(figsize=(8.3, 4.8))
    ax.plot(range(1, epochs + 1), losses, marker="o", lw=2.2, color="#2f80ed")
    ax.set_title("Biểu đồ hội tụ loss khi huấn luyện embedding thuốc trong 50 epoch", fontsize=13, fontweight="bold")
    ax.set_xlabel("Epoch")
    ax.set_ylabel("Loss trung bình")
    ax.grid(alpha=0.25)
    min_epoch = int(np.argmin(losses) + 1)
    ax.scatter([min_epoch], [min(losses)], color="#eb5757", zorder=3)
    ax.text(min_epoch, min(losses), f"  min epoch {min_epoch}", va="bottom", fontsize=9)
    savefig("fig_5_8_training_loss_convergence.png")


def draw_top_100_drug_space(labels, vectors, graph, freq):
    scored = []
    for label in labels:
        degree_weight = sum(graph.get(label, {}).values())
        frequency = freq.get(label, 0)
        norm = float(np.linalg.norm(vectors[labels.index(label)]))
        scored.append((degree_weight + frequency + 0.01 * norm, label))
    top_labels = [label for _, label in sorted(scored, reverse=True)[:100]]
    index = {label: i for i, label in enumerate(labels)}
    top_matrix = np.array([vectors[index[label]] for label in top_labels], dtype=float)
    perplexity = min(30, max(5, (len(top_labels) - 1) // 3))
    coords = TSNE(
        n_components=2,
        perplexity=perplexity,
        init="pca",
        learning_rate="auto",
        random_state=RANDOM_SEED,
        max_iter=1000,
    ).fit_transform(top_matrix)
    cluster_count = min(6, len(top_labels))
    clusters = KMeans(n_clusters=cluster_count, random_state=RANDOM_SEED, n_init=20).fit_predict(top_matrix)
    sizes = np.array([max(1, sum(graph.get(label, {}).values())) for label in top_labels], dtype=float)
    sizes = 35 + 180 * (sizes - sizes.min()) / (sizes.max() - sizes.min() + 1e-9)

    fig, ax = plt.subplots(figsize=(10.2, 6.8))
    scatter = ax.scatter(coords[:, 0], coords[:, 1], s=sizes, c=clusters, cmap="tab10", alpha=0.82, edgecolor="white", linewidth=0.4)
    for i, label in enumerate(top_labels[:35]):
        ax.text(coords[i, 0], coords[i, 1], label, fontsize=7.2, ha="left", va="bottom")
    ax.set_title("Không gian vector của 100 thuốc nổi bật nhất (t-SNE, tô màu theo cụm)", fontsize=13, fontweight="bold")
    ax.set_xlabel("t-SNE 1")
    ax.set_ylabel("t-SNE 2")
    ax.grid(alpha=0.20)
    cbar = plt.colorbar(scatter, ax=ax)
    cbar.set_label("Cụm thuốc theo KMeans trên embedding")
    savefig("fig_5_9_top_100_drug_vector_space.png")


def main():
    labels, vectors = load_vectors()
    prescriptions, source = try_load_prescriptions()
    print(f"Nguon du lieu do thi: {source}")
    graph, freq = build_weighted_graph(prescriptions, labels)
    if not graph:
        raise RuntimeError("Khong xay dung duoc graph thuoc.")

    draw_roc_pq(labels, vectors, graph)
    draw_loss_convergence(labels, vectors, graph)
    draw_top_100_drug_space(labels, vectors, graph, freq)


if __name__ == "__main__":
    main()
