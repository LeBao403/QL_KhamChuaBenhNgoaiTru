import numpy as np
import random
import math
import os
import json
from collections import defaultdict

class CustomNode2Vec:
    def __init__(self, 
                 dimensions=128,       
                 walk_length=10,      
                 num_walks=150,       
                 p=1.5,               
                 q=0.5,               
                 window_size=3,       
                 epochs=50,           
                 learning_rate=0.01): 
        
        self.dimensions = dimensions
        self.walk_length = walk_length
        self.num_walks = num_walks
        self.p = p
        self.q = q
        self.window_size = window_size
        self.epochs = epochs
        self.lr = learning_rate
        
        self.graph = defaultdict(lambda: defaultdict(int))
        self.vectors = {} 
        self.context_vectors = {} 
        self.vocab = []
        self.drug_frequencies = defaultdict(int) # MỚI: Đếm tần suất thuốc (TF)

    def build_graph(self, danh_sach_toa_thuoc):
        self.graph.clear()
        self.drug_frequencies.clear()
        
        for toa in danh_sach_toa_thuoc:
            for thuoc in toa:
                self.drug_frequencies[thuoc] += 1 # Đếm số lần xuất hiện
                
            for i in range(len(toa)):
                for j in range(i + 1, len(toa)):
                    thuoc_A, thuoc_B = toa[i], toa[j]
                    self.graph[thuoc_A][thuoc_B] += 1
                    self.graph[thuoc_B][thuoc_A] += 1
        
        self.vocab = list(self.graph.keys())
        for thuoc in self.vocab:
            self.vectors[thuoc] = np.random.uniform(-0.5, 0.5, self.dimensions)
            self.context_vectors[thuoc] = np.random.uniform(-0.5, 0.5, self.dimensions)

    def generate_random_walks(self):
        walks = []
        nodes = list(self.graph.keys())
        for _ in range(self.num_walks):
            random.shuffle(nodes)
            for node in nodes:
                walk = [node]
                while len(walk) < self.walk_length:
                    current = walk[-1]
                    neighbors = list(self.graph[current].keys())
                    if not neighbors: break
                    weights = list(self.graph[current].values())
                    next_node = random.choices(neighbors, weights=weights, k=1)[0]
                    walk.append(next_node)
                walks.append(walk)
        return walks

    def sigmoid(self, x):
        if x > 10: return 1.0
        elif x < -10: return 0.0
        return 1.0 / (1.0 + math.exp(-x))

    def get_tfidf_weight(self, drug, total_prescriptions):
        """MỚI: Kỹ thuật TF-IDF giảm trọng số các thuốc quá phổ biến, tăng trọng số thuốc hiếm"""
        tf = self.drug_frequencies[drug]
        if tf == 0: return 1.0
        # Càng xuất hiện nhiều, trọng số (weight) càng giảm
        return math.log(total_prescriptions / tf) 

    def train_and_evaluate(self, danh_sach_toa_thuoc, test_ratio=0.2, output_dir="models"):
        print(f"\n--- BẮT ĐẦU QUY TRÌNH HUẤN LUYỆN & ĐÁNH GIÁ THỰC TẾ ---")
        
        random.shuffle(danh_sach_toa_thuoc)
        split_idx = int(len(danh_sach_toa_thuoc) * (1 - test_ratio))
        train_data = danh_sach_toa_thuoc[:split_idx]
        test_data = danh_sach_toa_thuoc[split_idx:]
        total_train_prescriptions = len(train_data)
        
        print(f"1. Tổng số toa: {len(danh_sach_toa_thuoc)} | Train: {len(train_data)} (80%) | Test: {len(test_data)} (20%)")

        print("2. Đang xây dựng Đồ thị từ tập Train...")
        self.build_graph(train_data)
        
        print("3. Đang đi dạo (Random Walks)...")
        walks = self.generate_random_walks()

        print("4. BẮT ĐẦU HUẤN LUYỆN MẠNG NEURAL...")
        training_samples = []
        for walk in walks:
            for i, target in enumerate(walk):
                start = max(0, i - self.window_size)
                end = min(len(walk), i + self.window_size + 1)
                for j in range(start, end):
                    if i != j:
                        training_samples.append((target, walk[j]))

        for epoch in range(1, self.epochs + 1):
            total_loss = 0
            random.shuffle(training_samples)
            
            # CẬP NHẬT: In chi tiết bên trong từng vòng
            print(f"   -> Đang chạy Epoch {epoch}/{self.epochs}...", end="\r")
            
            for target, context in training_samples:
                v_target = self.vectors[target]
                v_context = self.context_vectors[context]
                
                # Áp dụng TF-IDF Weight để học sâu hơn
                weight = self.get_tfidf_weight(target, total_train_prescriptions)
                
                # Positive
                dot_pos = np.dot(v_target, v_context)
                pred_pos = self.sigmoid(dot_pos)
                err_pos = (1.0 - pred_pos) * weight # Nâng cấp: Lỗi * Trọng số
                
                grad_target = self.lr * err_pos * v_context
                grad_context = self.lr * err_pos * v_target
                
                # Negative
                negative = random.choice(self.vocab)
                v_neg = self.context_vectors[negative]
                dot_neg = np.dot(v_target, v_neg)
                pred_neg = self.sigmoid(dot_neg)
                err_neg = (0.0 - pred_neg) * weight
                
                grad_target += self.lr * err_neg * v_neg
                grad_neg = self.lr * err_neg * v_target
                
                self.vectors[target] += grad_target
                self.context_vectors[context] += grad_context
                self.context_vectors[negative] += grad_neg
                
                total_loss -= (math.log(pred_pos + 1e-9) + math.log(1.0 - pred_neg + 1e-9))
            
            # In chi tiết MỌI vòng lặp
            print(f"   -> Epoch {epoch:02d}/{self.epochs} | Loss: {total_loss:12.4f} | Tốc độ học (LR): {self.lr:.4f}")

        print("\n5. ĐÁNH GIÁ CHẤT LƯỢNG MÔ HÌNH TRÊN TẬP TEST (20% Dữ liệu lạ)...")
        metrics = self.evaluate_multiple_hit_rates(test_data)

        if not os.path.exists(output_dir):
            os.makedirs(output_dir)

        self.save_model(os.path.join(output_dir, "model_weights.json"))
        self.export_features_to_csv(os.path.join(output_dir, "drug_features.csv"))
        
        # MỚI: XUẤT CẢ 2 ĐỊNH DẠNG HÌNH ẢNH CÙNG LÚC
        self.visualize_embeddings_static(os.path.join(output_dir, "drug_graph_3d.png")) 
        self.visualize_embeddings_interactive(os.path.join(output_dir, "drug_graph_3d.html"))

        metrics["total_prescriptions"] = len(danh_sach_toa_thuoc)
        metrics["train_count"] = len(train_data)
        metrics["test_count"] = len(test_data)
        return metrics

    def evaluate_multiple_hit_rates(self, test_data):
        """Hàm đánh giá in chi tiết số lượng trúng/trượt"""
        hits = {10: 0, 20: 0, 30: 0}
        valid_tests = 0
        
        for toa in test_data:
            valid_drugs_in_toa = [d for d in toa if d in self.vocab]
            unique_drugs = list(set(valid_drugs_in_toa))
            
            if len(unique_drugs) < 2:
                continue
                
            hidden_drug = random.choice(unique_drugs)
            query_drugs = [d for d in unique_drugs if d != hidden_drug]
            
            if not query_drugs:
                continue
                
            query_vec = np.mean([self.vectors[d] for d in query_drugs], axis=0)
            
            results = []
            for drug, vec in self.vectors.items():
                if drug in query_drugs: continue
                
                norm_query = np.linalg.norm(query_vec)
                norm_vec = np.linalg.norm(vec)
                cosine = 0.0 if (norm_query == 0 or norm_vec == 0) else np.dot(query_vec, vec) / (norm_query * norm_vec)
                results.append((drug, float(cosine)))
                
            top_predictions = [r[0] for r in sorted(results, key=lambda x: x[1], reverse=True)]
            
            for k in hits.keys():
                if hidden_drug in top_predictions[:k]:
                    hits[k] += 1
            valid_tests += 1

        print("-" * 50)
        print(f"BÁO CÁO KẾT QUẢ DỰ ĐOÁN (Tổng số toa đã test: {valid_tests} toa):")
        metrics = {"valid_tests": valid_tests, "hit_rates": {}}
        for k in sorted(hits.keys()):
            rate = (hits[k] / valid_tests) * 100 if valid_tests > 0 else 0
            # MỚI: In rõ số lượng trúng / tổng số
            print(f" - Top {k:2d}: Trúng {hits[k]:3d} / {valid_tests} toa \t| Tỷ lệ (Hit@{k}): {rate:5.2f}%")
            metrics["hit_rates"][str(k)] = {
                "hits": hits[k],
                "total": valid_tests,
                "rate": round(rate, 2)
            }
        print("-" * 50)
        return metrics

    def export_features_to_csv(self, filename="models/drug_features.csv"):
        import pandas as pd
        data = []
        for drug, vec in self.vectors.items():
            row = {'MaThuoc': drug}
            for i, val in enumerate(vec):
                row[f'Dim_{i+1}'] = float(val)
            data.append(row)
        pd.DataFrame(data).to_csv(filename, index=False, encoding='utf-8')
        print(f"[HỆ THỐNG] Đã xuất {len(self.vocab)} đặc trưng thuốc ra file: {filename}")

    def visualize_embeddings_static(self, filename="models/drug_graph_3d.png"):
        """Vẽ ảnh tĩnh PNG 3D"""
        try:
            import matplotlib
            matplotlib.use('Agg')
            import matplotlib.pyplot as plt
            from sklearn.decomposition import PCA

            words = list(self.vectors.keys())
            vectors = np.array(list(self.vectors.values()))

            pca = PCA(n_components=3)
            vectors_3d = pca.fit_transform(vectors)

            fig = plt.figure(figsize=(10, 8))
            ax = fig.add_subplot(111, projection='3d')
            ax.scatter(vectors_3d[:, 0], vectors_3d[:, 1], vectors_3d[:, 2], c='red', edgecolors='k', alpha=0.6)

            ax.set_title('Bản đồ phân cụm Thuốc (Node2Vec PCA 3D - Tĩnh)')
            plt.savefig(filename)
            plt.close()
            print(f"[HỆ THỐNG] Đã xuất ảnh TĨNH 3D ra file: {filename}")
        except Exception as e:
            print(f"Lỗi khi vẽ ảnh tĩnh 3D: {e}")

    def visualize_embeddings_interactive(self, filename="models/drug_graph_3d.html"):
        """Vẽ file HTML 3D tương tác bằng Plotly"""
        try:
            import plotly.express as px
            import pandas as pd
            from sklearn.decomposition import PCA

            words = list(self.vectors.keys())
            vectors = np.array(list(self.vectors.values()))

            pca = PCA(n_components=3)
            vectors_3d = pca.fit_transform(vectors)

            df = pd.DataFrame({
                'X': vectors_3d[:, 0],
                'Y': vectors_3d[:, 1],
                'Z': vectors_3d[:, 2],
                'TenThuoc': words
            })

            fig = px.scatter_3d(df, x='X', y='Y', z='Z', 
                                hover_name='TenThuoc',
                                opacity=0.7,
                                title='Bản đồ phân cụm Thuốc (Interactive 3D Node2Vec)')
            
            fig.update_traces(marker=dict(size=5, color='green', line=dict(width=1, color='darkgreen')))
            fig.update_layout(margin=dict(l=0, r=0, b=0, t=40))

            fig.write_html(filename)
            print(f"[HỆ THỐNG] Đã xuất ảnh ĐỘNG 3D tương tác ra file Web: {filename}")
        except Exception as e:
            print(f"Lỗi khi vẽ ảnh động 3D: {e}")

    def get_similar_drugs_from_list(self, current_drugs, top_k=30):
        if not self.vectors: return []
        
        valid_drugs = [drug for drug in current_drugs if drug in self.vectors]
        if not valid_drugs: return []
            
        query_vec = np.mean([self.vectors[drug] for drug in valid_drugs], axis=0)
        
        results = []
        for drug, vec in self.vectors.items():
            if drug in current_drugs: continue
            norm_query = np.linalg.norm(query_vec)
            norm_vec = np.linalg.norm(vec)
            cosine = 0.0 if (norm_query == 0 or norm_vec == 0) else np.dot(query_vec, vec) / (norm_query * norm_vec)
            results.append({"MaThuoc": drug, "DoTuongDong": round(float(cosine) * 100, 2)})
            
        return sorted(results, key=lambda x: x["DoTuongDong"], reverse=True)[:top_k]

    def save_model(self, filepath="models/model_weights.json"):
        thumu_chua_file = os.path.dirname(filepath)
        if thumu_chua_file and not os.path.exists(thumu_chua_file): os.makedirs(thumu_chua_file)
        vectors_to_save = {drug: vec.tolist() if isinstance(vec, np.ndarray) else vec for drug, vec in self.vectors.items()}
        with open(filepath, 'w') as f: json.dump(vectors_to_save, f)
        print(f"[HỆ THỐNG] Đã lưu trí nhớ AI ra file: {filepath}")

    def load_model(self, filepath="models/model_weights.json"):
        if os.path.exists(filepath):
            with open(filepath, 'r') as f: loaded_vectors = json.load(f)
            self.vectors = {drug: np.array(vec) for drug, vec in loaded_vectors.items()}
            self.vocab = list(self.vectors.keys())
            return True
        return False
