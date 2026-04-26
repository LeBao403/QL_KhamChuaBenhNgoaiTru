### File: `GIAI_THICH_THUAT_TOAN.md`
*(Dành cho Hội đồng/Báo cáo - Giải thích 4 giai đoạn 1, 2, 3, 4 bác hỏi)*

```markdown
# 🧠 CƠ SỞ LÝ THUYẾT VÀ GIẢI THUẬT NODE2VEC TỰ ĐỊNH NGHĨA

Tài liệu này giải thích chi tiết 4 giai đoạn xử lý trong file `custom_node2vec.py` để đạt được khả năng gợi ý thuốc thông minh.

---

## 1️⃣ Giai đoạn 1: Xây dựng Đồ thị (Graph Construction)
- **Mục tiêu:** Biến lịch sử kê đơn thành một mạng lưới liên kết.
- **Cách làm:** Mỗi loại thuốc là một **Nút (Node)**. Nếu hai thuốc cùng xuất hiện trong một toa thuốc, một **Cạnh (Edge)** được tạo ra.
- **Trọng số:** Càng nhiều bác sĩ kê chung cặp thuốc đó, trọng số cạnh càng lớn, thể hiện mối quan hệ càng khăng khít.

## 2️⃣ Giai đoạn 2: Tính toán Xác suất & Alias Method
- **Mục tiêu:** Tối ưu hóa việc chọn đường đi trên đồ thị.
- **Lý thuyết:** Node2Vec không đi ngẫu nhiên đơn thuần. Nó sử dụng tham số $p$ và $q$ để điều hướng:
    - $p=0.5$: Ưu tiên quay lại nút vừa đi (BFS) để tìm các thuốc cùng nhóm.
    - $q=2.0$: Hạn chế đi ra quá xa (DFS) để tránh gợi ý lạc đề.
- **Kỹ thuật:** Cài đặt **Alias Method** để máy tính "bốc thăm" bước đi tiếp theo với tốc độ cực nhanh $O(1)$.

## 3️⃣ Giai đoạn 3: Sinh dữ liệu bằng Random Walk (Đi dạo)
- **Mục tiêu:** Tạo ra hàng ngàn "chuỗi thuốc" để máy tính học tập.
- **Cách làm:** Từ mỗi loại thuốc, máy tính thực hiện 100 chuyến đi dạo (Walks), mỗi chuyến dài 5 bước. 
- **Kết quả:** Sinh ra một tập dữ liệu văn bản chứa các thói quen kê đơn, phục vụ làm đầu vào cho mạng Neural.

## 4️⃣ Giai đoạn 4: Huấn luyện Mạng Neural (Skip-gram)
- **Mục tiêu:** Nén các mối quan hệ phức tạp thành các Vector số thực (32 chiều).
- **Kỹ thuật:** - **Skip-gram:** Dự đoán các thuốc xung quanh dựa trên 1 thuốc mục tiêu.
    - **Negative Sampling:** Kỹ thuật bốc mẫu âm để đẩy các thuốc không liên quan ra xa nhau trong không gian Vector.
    - **Gradient Descent:** Cập nhật trọng số qua từng vòng lặp (Epoch) để giảm thiểu sai số (Loss).

---
**=> Kết quả cuối cùng:** Dùng **Cosine Similarity** để tính góc giữa các Vector. Thuốc nào có góc hẹp nhất với thuốc đang chọn sẽ được đưa lên đầu danh sách gợi ý.