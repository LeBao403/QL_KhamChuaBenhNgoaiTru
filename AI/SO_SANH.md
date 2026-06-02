# BẢNG SO SÁNH: AI_RECOMMENDER (GỐC) vs AI_HUY (TỐI ƯU)

Tài liệu này tổng hợp các khác biệt cốt lõi về mặt Toán học, Học máy (Machine Learning) và Siêu tham số (Hyperparameters) giữa phiên bản gốc và phiên bản đã được tối ưu hóa. Bảng so sánh này rất hữu ích để đưa vào slide bảo vệ hoặc chương "Đánh giá mô hình" trong quyển Khóa luận.

---

## 1. Bản chất Thuật toán Đồ thị (Graph Algorithm)

| Tiêu chí | `AI_Recommender` (Gốc) | `AI_Huy` (Tối ưu) |
| :--- | :--- | :--- |
| **Bản chất thực sự** | **DeepWalk** | **Chuẩn Node2Vec (Stanford)** |
| **Random Walks** | Nhảy ngẫu nhiên thuần túy dựa trên trọng số cạnh (Edge weight). Bỏ qua hoàn toàn các tham số điều hướng. | Là **Biased Random Walks**. Tính toán lại xác suất bước nhảy dựa trên node trước đó. |
| **Tham số `p` (Return)** | Có khai báo nhưng code không sử dụng. | Khai thác triệt để, giúp mô hình bắt được cấu trúc Cụm cục bộ (Local / BFS). |
| **Tham số `q` (In-out)** | Có khai báo nhưng code không sử dụng. | Khai thác triệt để, giúp mô hình bắt được các loại thuốc có cùng đặc tính chức năng dù ở xa (Global / DFS). |

---

## 2. Kỹ thuật Lấy Mẫu & Học Máy (Sampling & Training)

| Tiêu chí | `AI_Recommender` (Gốc) | `AI_Huy` (Tối ưu) |
| :--- | :--- | :--- |
| **Xử lý Thuốc phổ biến** | Nhân trực tiếp trọng số TF-IDF vào sai số (`err_pos * weight`). **Lỗi:** Gây bùng nổ Gradient (Gradient Explosion) với thuốc hiếm. | Áp dụng **Subsampling**. Tự động bỏ qua các thuốc xuất hiện quá nhiều theo xác suất $1 - \sqrt{\frac{t}{f(w)}}$, giúp cân bằng phân phối. |
| **Mẫu Âm (Negative Samples)**| Chỉ lấy **1 mẫu âm** cho mỗi bước học. | Lấy **5 mẫu âm** cho mỗi bước học. Bắt buộc để mô hình học cách phân biệt (Discriminative learning). |
| **Phân phối Mẫu Âm** | Lấy ngẫu nhiên chia đều (`random.choice(vocab)`). **Lỗi:** Không phản ánh thực tế. | Lấy theo **Phân phối Tần suất (Unigram distribution ^ 0.75)**. Các thuốc hiếm được đối xử công bằng hơn. |

---

## 3. Tối ưu Thuật toán Tối ưu hóa (Optimization)

| Tiêu chí | `AI_Recommender` (Gốc) | `AI_Huy` (Tối ưu) |
| :--- | :--- | :--- |
| **Tốc độ học (Learning Rate)** | Giữ nguyên cố định `0.02` từ đầu đến cuối vòng lặp. | Áp dụng **Learning Rate Decay**. Tốc độ học giảm dần tuyến tính theo thời gian. |
| **Quá trình Hội tụ** | Ở các epoch cuối, tốc độ học vẫn lớn khiến vector nhảy qua lại quanh điểm tối ưu (Dao động nhiễu). | Vector hội tụ mượt mà và chính xác vào điểm đáy tối ưu (Global Minima) nhờ những bước nhảy cực nhỏ lúc cuối. |

---

## 4. Ép xung Siêu Tham Số (Dành riêng cho dữ liệu nhỏ 13k dòng)

Với tập dữ liệu mỏng (~13.000 bản ghi chi tiết đơn thuốc), việc để thông số mạng Neural quá lớn sẽ gây ra hiện tượng **Học vẹt (Overfitting)** và **Loãng dữ liệu (Sparsity)**.

| Tham số | `AI_Recommender` (Gốc) | `AI_Huy` (Tối ưu) | Giải thích sự thay đổi |
| :--- | :--- | :--- | :--- |
| **`dimensions`** | `32` | `16` | Thu nhỏ không gian vector. Ép mô hình nén các đặc trưng cốt lõi của thuốc, tránh bị loãng. |
| **`walk_length`** | `5` | `15` | Chuỗi 5 bước là quá ngắn. Tăng lên 15 giúp mô hình kết nối các loại thuốc ở đầu và cuối một phác đồ dài. |
| **`epochs`** | `50` | `150` | Tập dữ liệu càng nhỏ thì càng phải học nhiều vòng lặp để mô hình ghi nhớ kỹ các quy luật ngầm. |

---
**Kết luận:** Thư mục `AI_Huy` không chỉ thay đổi các con số cài đặt, mà đã **cấu trúc lại toàn bộ nhân lõi của mạng Neural** để biến nó trở thành một mô hình Học sâu (Deep Learning) chuẩn học thuật.
