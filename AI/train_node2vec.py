from __future__ import annotations

import csv
from collections import defaultdict
from pathlib import Path

import networkx as nx
import numpy as np
from node2vec import Node2Vec

BASE = Path(__file__).resolve().parent
ART = BASE / 'artifacts'
EDGE_FILE = ART / 'edges.csv'
MODEL_FILE = ART / 'node2vec.model'
EMB_FILE = ART / 'embeddings.csv'


def load_graph() -> nx.Graph:
    g = nx.Graph()
    with EDGE_FILE.open(encoding='utf-8') as f:
        r = csv.DictReader(f)
        for row in r:
            s, d, rel = row['src'], row['dst'], row['relation']
            weight = 3.0 if rel == 'recommended' else 2.0 if rel in {'has_symptom', 'has_family', 'in_group'} else 1.0
            g.add_edge(s, d, weight=weight)
    return g


def main():
    g = load_graph()
    print(f'Graph nodes={g.number_of_nodes()} edges={g.number_of_edges()}')

    node2vec = Node2Vec(
        g,
        dimensions=64,
        walk_length=25,
        num_walks=20,
        workers=2,
        weight_key='weight',
        p=1,
        q=1,
        seed=42,
    )
    model = node2vec.fit(window=8, min_count=1, batch_words=128)
    model.save(str(MODEL_FILE))

    with EMB_FILE.open('w', newline='', encoding='utf-8') as f:
        w = csv.writer(f)
        w.writerow(['node_id'] + [f'e{i}' for i in range(64)])
        for node in model.wv.index_to_key:
            vec = model.wv[node]
            w.writerow([node] + [float(x) for x in vec])

    print(f'Saved model to {MODEL_FILE}')
    print(f'Saved embeddings to {EMB_FILE}')


if __name__ == '__main__':
    main()
