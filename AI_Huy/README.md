# HỆ THỐNG GỢI Ý THUỐC BẰNG AI (NODE2VEC TỐI ƯU)

Dự án này là phiên bản được tối ưu hóa đặc biệt của thuật toán **Node2Vec** dành riêng cho bài toán gợi ý thuốc trong y tế, xử lý trực tiếp trên tập dữ liệu nhỏ (~13.000 dòng `CT_DON_THUOC`). 

Bản cài đặt này KHÔNG sử dụng thư viện Gensim có sẵn mà tự xây dựng bằng `Numpy` nhằm phục vụ cho mục đích bảo vệ khóa luận (giải thích chi tiết lõi thuật toán). Đồng thời khắc phục các lỗi toán học của phiên bản cũ (thêm cơ chế `p, q` Biased Walk, Negative Sampling chuẩn và LR Decay).

## 1. Yêu cầu hệ thống (Requirements)
Cài đặt môi trường bằng lệnh sau:
```bash
pip install -r requirements.txt
```

Các thư viện chính bao gồm:
- `fastapi`, `uvicorn`: Xây dựng Backend API.
- `pyodbc`: Kết nối cơ sở dữ liệu SQL Server.
- `numpy`: Toán học ma trận (Lõi mạng neural).
- `plotly`, `matplotlib`, `pandas`: Vẽ biểu đồ không gian 3D của thuốc.

## 2. Kết nối Cơ Sở Dữ Liệu
Hệ thống sử dụng bảng `CT_DON_THUOC` để trích xuất các đơn thuốc. Bạn cần cấu hình chuỗi kết nối (connection string) tới SQL Server. 
Mở file `database.py` hoặc `.env` để kiểm tra các thông số:
- Tên Server: `DB_SERVER`
- Tên Database: `DB_NAME` (Mặc định `QL_KhamBenhNgoaiTru`)

## 3. Khởi động hệ thống
Di chuyển vào thư mục `AI_Huy` và khởi chạy server FastAPI:
```bash
uvicorn main:app --reload
```
API sẽ hoạt động tại địa chỉ: `http://localhost:8000`

## 4. Các API Chính (Endpoints)

### A. Huấn luyện Mô hình (Train)
**`POST /api/train`**
- API này sẽ kéo dữ liệu từ Database về, xây dựng đồ thị thuốc, đi dạo ngẫu nhiên (Biased Random Walks), và tiến hành huấn luyện mạng neural theo chuẩn phân phối Word2Vec.
- Mô hình chạy xong sẽ tự động lưu lại trí nhớ (weights) vào thư mục `models/versions/` đồng thời tự động xuất 2 ảnh biểu diễn không gian 3D của các loại thuốc (`.png` và `.html` có thể xoay tương tác).

### B. Gợi ý thuốc (Recommend)
**`GET /api/recommend`**
- **Tham số truyền vào**: `node_id` (Mã thuốc bạn đang chọn), `top_k` (Số lượng thuốc gợi ý muốn lấy).
- **Phản hồi**: Trả về danh sách các loại thuốc thường đi kèm (hoặc giống hệt tính chất) với thuốc bạn đang chọn, dựa trên khoảng cách Cosine trong không gian 16 chiều.

### C. Quản lý Models
- **`GET /api/status`**: Xem trạng thái và thông tin của model đang hoạt động.
- **`GET /api/models`**: Lịch sử tất cả các lần huấn luyện.
- **`POST /api/models/select`**: Kích hoạt lại mô hình của một ngày cụ thể (Rollback).

## 5. Tài liệu Khóa luận
Để hiểu sâu hơn về công thức toán học và cấu trúc của thuật toán Node2Vec được code tay trong file `custom_node2vec.py`, vui lòng đọc file [GIAITHICH.md](GIAITHICH.md) đính kèm.