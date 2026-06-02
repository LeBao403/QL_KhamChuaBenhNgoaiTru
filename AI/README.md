# AI gợi ý thuốc

Dịch vụ AI dùng FastAPI để huấn luyện và phục vụ mô hình gợi ý thuốc cho hệ thống khám chữa bệnh ngoại trú. Lõi thuật toán là Node2Vec tự cài đặt bằng NumPy trong `custom_node2vec.py`, dùng dữ liệu đơn thuốc từ SQL Server để học quan hệ các thuốc thường được kê cùng nhau.

## Thành phần chính

- `main.py`: FastAPI app, endpoint huấn luyện, gợi ý và quản lý model.
- `database.py`: kết nối SQL Server và đọc dữ liệu phục vụ huấn luyện.
- `custom_node2vec.py`: cài đặt đồ thị thuốc, biased random walk, negative sampling và embedding.
- `models/`: model đang hoạt động, registry và các phiên bản model đã train.
- `report_chapter5/`: script và hình minh họa phục vụ báo cáo/chương mô hình AI.
- `GIAITHICH.md`, `SO_SANH.md`: tài liệu giải thích thuật toán và so sánh.

## Cài đặt

```powershell
cd AI
python -m venv .venv
.\.venv\Scripts\Activate.ps1
pip install -r requirements.txt
```

## Cấu hình database

Kiểm tra thông tin kết nối trong `database.py` và đảm bảo SQL Server đã có database được tạo từ thư mục `../Database`.

Dữ liệu huấn luyện lấy từ các bảng đơn thuốc/chi tiết đơn thuốc của hệ thống. Nếu đổi tên server, database hoặc tài khoản SQL Server, cập nhật lại cấu hình trước khi gọi API train.

## Chạy API

```powershell
cd AI
uvicorn main:app --reload
```

Mặc định API chạy tại:

```text
http://localhost:8000
```

## Endpoint chính

- `GET /api/status`: xem trạng thái model hiện tại.
- `GET /api/models`: xem danh sách các model đã train.
- `POST /api/models/select`: chọn lại một model đã train.
- `POST /api/train`: train model mới từ dữ liệu SQL Server.
- `GET /api/recommend?node_id=<MA_THUOC>&top_k=40`: lấy danh sách thuốc gợi ý.

## Ghi chú vận hành

- Sau khi train, model được lưu trong `models/versions/<model_id>/`.
- `models/active_model.json` xác định model đang được dùng để gợi ý.
- Các file `.png`, `.html`, `.csv` trong model version là artifact trực quan hóa và phân tích embedding.
- Web ASP.NET gọi dịch vụ này để quản lý huấn luyện và hiển thị gợi ý trong module AI.
