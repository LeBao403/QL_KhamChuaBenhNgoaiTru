# 💊 TÀI LIỆU KỸ THUẬT: AI SERVICE GỢI Ý THUỐC (NODE2VEC)

**Dự án:** Hệ thống Quản lý Khám chữa bệnh Ngoại trú
**Mô-đun:** Hệ thống Gợi ý Kê đơn tích hợp AI (Microservice)
**Ngôn ngữ/Công nghệ:** Python, FastAPI, NumPy, SQL Server

---

## 📂 1. CHÚ THÍCH CẤU TRÚC THƯ MỤC VÀ FILE

Dự án được cấu trúc theo chuẩn Microservices, tách biệt rõ ràng giữa Database, Core Logic (Thuật toán) và API Layer.

* `requirements.txt`: Chứa danh sách các thư viện phụ thuộc (FastAPI, NumPy, pyodbc...).
* `database.py`: Lớp Data Access. Chịu trách nhiệm kết nối trực tiếp vào SQL Server, truy vấn bảng `CT_DON_THUOC`, lọc các toa thuốc hợp lệ (>= 2 loại thuốc) và gom nhóm dữ liệu.
* `custom_node2vec.py`: **Trái tim của hệ thống.** Chứa toàn bộ logic toán học của Node2Vec được hiện thực hóa từ đầu (from scratch) bằng NumPy, không phụ thuộc vào thư viện Black-box.
* `main.py`: Lớp API. Khởi tạo server FastAPI, hứng các Request từ ứng dụng .NET MVC và điều phối luồng xử lý.

---

## 🚀 2. HƯỚNG DẪN CÀI ĐẶT VÀ KHỞI CHẠY

**Bước 1: Cài đặt môi trường**
Đảm bảo máy đã cài đặt Python 3.8+. Mở Terminal tại thư mục dự án và chạy lệnh:
```bash
pip install -r requirements.txt

Bước 2: Cấu hình Database
Mở file database.py, sửa các biến SERVER và DATABASE khớp với chuỗi kết nối SQL Server nội bộ.

Bước 3: Khởi chạy API Server
uvicorn main:app --reload

Server sẽ chạy tại http://127.0.0.1:8000.
Truy cập http://127.0.0.1:8000/docs để mở giao diện Swagger UI kiểm thử API.

3. PHÂN TÍCH THUẬT TOÁN VÀ ÁP DỤNG TRONG CODE
Thuật toán Node2Vec trong file custom_node2vec.py được chia thành 4 giai đoạn cốt lõi. Dưới đây là cách lý thuyết được dịch ra mã nguồn:

Giai đoạn 1: Khởi tạo Đồ thị từ Dữ liệu (Graph Construction)
Lý thuyết: Chuyển đổi lịch sử kê đơn thành một mạng lưới. Các loại thuốc là Nút (Node). Kê chung 1 toa là Cạnh (Edge). Kê chung càng nhiều, Trọng số (Weight) càng lớn.
Trong Code: Hàm train() nhận danh_sach_toa_thuoc từ database.py. Sử dụng Dictionary để đếm tần suất cặp thuốc, từ đó xây dựng Ma trận Kề (Adjacency List) có trọng số.

Giai đoạn 2: Tính Xác Suất & Alias Method (Tối ưu O(1))
Lý thuyết: Thuật toán đi dạo không chọn đường ngẫu nhiên (Uniform) mà bị "thiên vị" bởi 2 tham số:
    $p = 0.5$ (Return parameter): Ưu tiên quay lại nút cũ, giúp khám phá sâu các cụm thuốc chữa cùng một nhóm bệnh (Homophily).
    $q = 2.0$ (In-out parameter): Hạn chế đi xa (DFS), tránh việc gợi ý thuốc lạc sang chuyên khoa khác.
Trong Code: Code tính toán các xác suất chuyển trạng thái (Transition Probabilities). Sau đó, áp dụng Alias Method (tạo mảng J và q) để máy tính có thể "bốc thăm" bước đi tiếp theo với độ phức tạp $O(1)$, giúp hệ thống chạy mượt mà ngay cả khi danh mục thuốc tăng lên hàng ngàn loại.

Giai đoạn 3: Đi dạo thiên vị (Biased Random Walk)
Lý thuyết: Máy tính nhảy từ thuốc này sang thuốc khác để tạo ra các "câu văn" (danh sách thuốc).
Trong Code: Một vòng lặp for quét qua từng Mã thuốc. Từ mỗi Mã thuốc, gọi hàm Random Walk bước đi walk_length (ví dụ 5 bước), lặp lại num_walks (100 lần). Kết quả sinh ra một tập dữ liệu khổng lồ các chuỗi thuốc.


Giai đoạn 4: Học Biểu Diễn (Skip-Gram & Negative Sampling)
Lý thuyết: Dùng Cửa sổ trượt (Sliding Window) cắt chuỗi thuốc thành các cặp (Target, Context). Ép Mạng Neural kéo các vector của thuốc hay đi cùng nhau lại gần, và đẩy các thuốc không liên quan ra xa.
Trong Code: * Khởi tạo self.vectors bằng ma trận ngẫu nhiên (kích thước 32 chiều).
    Dùng Negative Sampling bốc ngẫu nhiên các loại thuốc không có trong Cửa sổ trượt.
    Sử dụng tích vô hướng (Dot product), đẩy qua hàm kích hoạt Sigmoid.
    Tính đạo hàm (Gradient Descent) để cập nhật liên tục các số thực trong ma trận self.vectors.


4. HỆ THỐNG GỢI Ý (INFERENCE)
Sau khi gọi API /api/train, mảng self.vectors sẽ chứa Vector của toàn bộ danh mục thuốc.
Khi bác sĩ gọi API /api/recommend?mathuoc=TH001:
    Hệ thống trích xuất Vector của TH001
    Sử dụng công thức toán học Cosine Similarity (Tích vô hướng chia cho Tích độ dài) tính góc giữa TH001 và các thuốc còn lại.
    Cosine càng gần 1 (góc càng hẹp), độ tương đồng càng cao. Thuật toán sắp xếp và trả về Top 3 loại thuốc có % tương đồng cao nhất để UI hiển thị gợi ý cho bác sĩ.