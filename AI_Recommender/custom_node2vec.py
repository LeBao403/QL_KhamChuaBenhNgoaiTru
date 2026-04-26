import numpy as np
import random
import math
from collections import defaultdict

class CustomNode2Vec:
    def __init__(self, dimensions=32, walk_length=5, num_walks=100, p=0.5, q=2.0, window_size=3, epochs=10, learning_rate=0.01):
        self.dimensions = dimensions
        self.walk_length = walk_length
        self.num_walks = num_walks
        self.p = p
        self.q = q
        self.window_size = window_size
        self.epochs = epochs  # ĐÂY RỒI! SỐ VÒNG LẶP BÁC MUỐN ĐÂY!
        self.lr = learning_rate
        
        self.graph = defaultdict(lambda: defaultdict(int))
        self.vectors = {} # Trọng số W1 (Embedding chính)
        self.context_vectors = {} # Trọng số W2 (Embedding ngữ cảnh)
        self.vocab = []

    def build_graph(self, danh_sach_toa_thuoc):
        """Bước 1: Chuyển Toa thuốc thành Mạng lưới (Đồ thị)"""
        for toa in danh_sach_toa_thuoc:
            for i in range(len(toa)):
                for j in range(i + 1, len(toa)):
                    thuoc_A = toa[i]
                    thuoc_B = toa[j]
                    # Cứ kê chung là cộng thêm 1 điểm thân thiết (Trọng số)
                    self.graph[thuoc_A][thuoc_B] += 1
                    self.graph[thuoc_B][thuoc_A] += 1
        
        self.vocab = list(self.graph.keys())
        # Khởi tạo Ma trận Vector ngẫu nhiên (Lớp Ẩn)
        for thuoc in self.vocab:
            self.vectors[thuoc] = np.random.uniform(-0.5, 0.5, self.dimensions)
            self.context_vectors[thuoc] = np.random.uniform(-0.5, 0.5, self.dimensions)

    def generate_random_walks(self):
        """Bước 2: Thuật toán dạo bước (Mô phỏng Node2Vec rút gọn)"""
        walks = []
        nodes = list(self.graph.keys())
        
        for walk_iter in range(self.num_walks):
            random.shuffle(nodes) # Xáo trộn ngẫu nhiên điểm xuất phát
            for node in nodes:
                walk = [node]
                while len(walk) < self.walk_length:
                    current = walk[-1]
                    neighbors = list(self.graph[current].keys())
                    if not neighbors:
                        break
                    
                    # (Code Đồ án: Chỗ này áp dụng Random có trọng số. 
                    # Để code chạy nhanh với đồ thị 200 nút, ta dùng random.choices)
                    weights = list(self.graph[current].values())
                    next_node = random.choices(neighbors, weights=weights, k=1)[0]
                    walk.append(next_node)
                walks.append(walk)
        return walks

    def sigmoid(self, x):
        # Tránh lỗi tràn số (overflow) của hàm mũ
        if x > 10: return 1.0
        elif x < -10: return 0.0
        return 1.0 / (1.0 + math.exp(-x))

    def train(self, danh_sach_toa_thuoc):
        print("1. Đang xây dựng Đồ thị (Graph) từ CSDL...")
        self.build_graph(danh_sach_toa_thuoc)
        print(f"   -> Đồ thị có {len(self.vocab)} loại thuốc (Nút).")

        print("2. Đang đi dạo (Random Walks) để sinh dữ liệu...")
        walks = self.generate_random_walks()
        print(f"   -> Đã tạo ra {len(walks)} chuỗi dữ liệu (Câu văn).")

        print("3. BẮT ĐẦU HUẤN LUYỆN MẠNG NEURAL (Skip-Gram + Negative Sampling)...")
        # Chuẩn bị tập dữ liệu train (Target -> Context)
        training_data = []
        for walk in walks:
            for i, target in enumerate(walk):
                # Lấy các thuốc xung quanh (Cửa sổ trượt)
                start = max(0, i - self.window_size)
                end = min(len(walk), i + self.window_size + 1)
                for j in range(start, end):
                    if i != j:
                        training_data.append((target, walk[j]))

        # VÒNG LẶP EPOCH MA THUẬT MÀ BÁC NÓI TỚI ĐÂY!
        for epoch in range(1, self.epochs + 1):
            total_loss = 0
            random.shuffle(training_data) # Trộn dữ liệu mỗi vòng để máy không học vẹt
            
            for target, context in training_data:
                # --- A. Xử lý mẫu ĐÚNG (Positive) ---
                v_target = self.vectors[target]
                v_context = self.context_vectors[context]
                
                # Tính Sigmoid và Lỗi
                dot_pos = np.dot(v_target, v_context)
                pred_pos = self.sigmoid(dot_pos)
                err_pos = 1.0 - pred_pos  # Mục tiêu là 1 (Kéo lại gần)
                
                # Cập nhật Gradient
                grad_target = self.lr * err_pos * v_context
                grad_context = self.lr * err_pos * v_target
                
                # --- B. Xử lý mẫu SAI (Negative Sampling) ---
                # Bốc bừa 1 loại thuốc không liên quan
                negative = random.choice(self.vocab)
                v_neg = self.context_vectors[negative]
                
                dot_neg = np.dot(v_target, v_neg)
                pred_neg = self.sigmoid(dot_neg)
                err_neg = 0.0 - pred_neg  # Mục tiêu là 0 (Đẩy ra xa)
                
                grad_target += self.lr * err_neg * v_neg
                grad_neg = self.lr * err_neg * v_target
                
                # --- C. Cập nhật Trọng số (Update Weights) ---
                self.vectors[target] += grad_target
                self.context_vectors[context] += grad_context
                self.context_vectors[negative] += grad_neg
                
                # Tính Loss (Binary Cross Entropy) để xem máy có đang khôn lên không
                total_loss -= math.log(pred_pos + 1e-9) + math.log(1.0 - pred_neg + 1e-9)
            
            print(f"   -> Epoch {epoch}/{self.epochs} hoàn tất | Loss (Độ sai sót): {total_loss:.4f}")

        print("HUẤN LUYỆN HOÀN TẤT! Đã ép xong Vector 32 chiều.")
        self.save_model()

    def get_similar_drugs(self, target_drug, top_k=3):
        if target_drug not in self.vectors:
            return []
        
        target_vec = self.vectors[target_drug]
        results = []
        
        for drug, vec in self.vectors.items():
            if drug == target_drug:
                continue
            # Cosine Similarity
            cosine = np.dot(target_vec, vec) / (np.linalg.norm(target_vec) * np.linalg.norm(vec))
            results.append({"MaThuoc": drug, "DoTuongDong": round(float(cosine) * 100, 2)})
            
        results = sorted(results, key=lambda x: x["DoTuongDong"], reverse=True)
        return results[:top_k]
    
    def save_model(self, filepath="models/model_weights.json"):
        """Lưu Ma trận Vector ra file ổ cứng trong thư mục riêng"""
        import json
        import numpy as np
        import os
        
        # TỰ ĐỘNG TẠO THƯ MỤC NẾU CHƯA CÓ
        thumu_chua_file = os.path.dirname(filepath)
        if thumu_chua_file and not os.path.exists(thumu_chua_file):
            os.makedirs(thumu_chua_file)
            print(f"[HỆ THỐNG] Đã tạo thư mục lưu trữ: {thumu_chua_file}")
        
        # Ép kiểu từ mảng NumPy (ndarray) sang danh sách (List)
        vectors_to_save = {drug: vec.tolist() if isinstance(vec, np.ndarray) else vec for drug, vec in self.vectors.items()}
        
        with open(filepath, 'w') as f:
            json.dump(vectors_to_save, f)
        print(f"[HỆ THỐNG] Đã lưu trí nhớ AI ra file: {filepath}")

    def load_model(self, filepath="models/model_weights.json"):
        """Đọc lại Ma trận Vector từ thư mục"""
        import json, os
        import numpy as np
        
        if os.path.exists(filepath):
            with open(filepath, 'r') as f:
                loaded_vectors = json.load(f)
                
            # Ép kiểu ngược lại thành NumPy
            self.vectors = {drug: np.array(vec) for drug, vec in loaded_vectors.items()}
            
            print(f"[HỆ THỐNG] Đã nạp thành công trí nhớ AI từ file: {filepath}")
            return True
        else:
            print(f"[HỆ THỐNG] Không tìm thấy file {filepath}. Vui lòng Train mô hình trước.")
            return False