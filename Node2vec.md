# Node2vec - Hướng dẫn train model gợi ý thuốc

Tài liệu này mô tả cách tạo dữ liệu graph, train `node2vec`, và dùng kết quả để gợi ý thuốc trong màn hình bác sĩ.

## 1. Mục tiêu

Model không dự đoán chẩn đoán mới, mà làm bài toán **recommendation thuốc**:

- Input: triệu chứng + kết luận bệnh + danh sách thuốc ứng viên
- Output: danh sách thuốc được gợi ý theo điểm phù hợp (`score`)

Kết quả được dùng để:

- đưa thuốc gợi ý lên đầu dropdown
- gắn dấu `★`
- highlight thuốc phù hợp
- hỗ trợ bác sĩ chọn nhanh hơn

---

## 2. Dữ liệu đầu vào

Hiện tại dự án dùng file seed đã tạo từ database:

- `Database/training_seed_append.sql`

Các bảng chính được dùng để sinh graph:

- `DANHMUC_BENH`
- `DANHMUC_THUOC`
- `DANHMUC_HOATCHAT`
- `THUOC`
- `THANHPHAN_THUOC`

### Ý nghĩa

- `DANHMUC_BENH`: danh mục bệnh, triệu chứng, mô tả
- `THUOC`: danh sách thuốc
- `DANHMUC_THUOC`: nhóm thuốc
- `DANHMUC_HOATCHAT`: danh mục hoạt chất
- `THANHPHAN_THUOC`: mapping thuốc ↔ hoạt chất

---

## 3. Cấu trúc graph

### Node

Graph hiện tại có các loại node:

- `disease::<MaBenh>`
- `symptom::<...>`
- `text::<TenBenh>`
- `drug::<MaThuoc>`
- `group::<TenNhomThuoc>`
- `family::<TenHoatChat>`

### Edge

Các quan hệ chính:

- `has_name`: bệnh ↔ tên bệnh
- `has_symptom`: bệnh ↔ triệu chứng
- `recommended`: bệnh ↔ thuốc
- `in_group`: thuốc ↔ nhóm thuốc
- `has_family`: thuốc ↔ hoạt chất
- `co_drug`: thuốc ↔ thuốc cùng nhóm/cùng cụm

---

## 4. Cách sinh graph dataset

Script:

- `AI/generate_graph_dataset.py`

Chức năng:

1. Đọc file `Database/training_seed_append.sql`
2. Parse dữ liệu bệnh và thuốc
3. Tạo node và edge
4. Ghi ra file trung gian trong `AI/artifacts/`

### File output

- `AI/artifacts/nodes_diseases.csv`
- `AI/artifacts/nodes_drugs.csv`
- `AI/artifacts/edges.csv`

---

## 5. Cách train `node2vec`

Script:

- `AI/train_node2vec.py`

### Mô hình

Model dùng thư viện `node2vec` để học embedding từ graph.

### Tham số chính

- `dimensions=64`
- `walk_length=25`
- `num_walks=20`
- `window=8`
- `min_count=1`

### Output

- `AI/artifacts/node2vec.model`
- `AI/artifacts/embeddings.csv`

---

## 6. Luồng train chuẩn

### Bước 1. Cài dependencies

```powershell
cd "D:\KhoaLuanKiSu\QL_KhamChuaBenhNgoaiTru\AI"
python -m pip install --upgrade pip
pip install -r requirements.txt
```

### Bước 2. Sinh graph dataset

```powershell
cd "D:\KhoaLuanKiSu\QL_KhamChuaBenhNgoaiTru\AI"
python generate_graph_dataset.py
```

### Bước 3. Train `node2vec`

```powershell
python train_node2vec.py
```

### Bước 4. Chạy API gợi ý thuốc

```powershell
uvicorn recommend_api:app --host 127.0.0.1 --port 8000
```

---

## 7. Luồng dùng trong màn hình bác sĩ

Khi bác sĩ bấm nút `Gợi ý thuốc`:

1. Frontend lấy:
   - `Triệu chứng`
   - `Kết luận`
   - tuổi
   - giới tính
2. Backend lấy danh sách thuốc từ database
3. Backend gửi request sang FastAPI
4. FastAPI chấm điểm thuốc ứng viên
5. Trả danh sách thuốc đã sắp xếp theo score
6. Frontend:
   - đưa thuốc gợi ý lên đầu dropdown
   - gắn dấu `★`
   - giữ nhãn BHYT
   - highlight thuốc gợi ý

---

## 8. Format request gửi sang API

Ví dụ payload:

```json
{
  "age": 30,
  "sex": "Nam",
  "age_group": "NguoiLonTre",
  "disease": "Cảm cúm",
  "symptoms": "sốt, ho, sổ mũi, đau đầu",
  "comorbidity": "None",
  "allergy": "None",
  "candidates": [
    {
      "drug_code": "T001",
      "drug_name": "Paracetamol 500mg",
      "drug_group": "DM001",
      "route": "Uống",
      "drug_family": "Paracetamol 500mg",
      "in_stock": 1,
      "contraindication": 0,
      "interaction_risk": 0
    }
  ]
}
```

---

## 9. Kết quả trả về mong muốn

```json
{
  "success": true,
  "items": [
    {
      "drug_code": "T001",
      "drug_name": "Paracetamol 500mg",
      "score": 0.96,
      "reason": "Gợi ý theo nhóm bệnh Hô hấp"
    }
  ]
}
```

---

## 10. Cách hiển thị trên UI

Frontend sẽ:

- đưa các thuốc có `score` cao lên đầu
- hiển thị `★`
- giữ note BHYT / Không BHYT
- render lại `Select2`

Ví dụ hiển thị:

- `★ [BHYT] Paracetamol 500mg (96.0%)`
- `★ [Không BHYT] Cetirizine 10mg (84.0%)`

---

## 11. Khuyến nghị khi retrain

### Khi nào nên train lại

- khi có thêm dữ liệu lịch sử kê đơn thật
- khi thêm danh mục thuốc mới
- khi thay đổi mapping bệnh–thuốc
- khi cần cải thiện độ chính xác

### Nên làm thêm sau này

- log gợi ý thuốc bác sĩ đã xem
- log thuốc được bác sĩ chọn
- log thuốc bị bỏ qua
- thêm bảng lịch sử train model

---

## 12. Lưu ý

Model hiện tại là **bootstrap model**, tức là:

- train từ dữ liệu seed và rule suy diễn
- chưa học từ lịch sử kê đơn thật
- phù hợp cho MVP và demo

Muốn chất lượng tốt hơn, cần:

- dữ liệu kê đơn thực tế
- log phản hồi bác sĩ
- retrain định kỳ

---

## 13. Tóm tắt nhanh

Quy trình:

1. Tạo seed từ database
2. Sinh graph dataset
3. Train `node2vec`
4. Chạy FastAPI
5. Tích hợp vào `GoiYThuoc`
6. Highlight thuốc gợi ý trong dropdown

---

## 14. Lệnh chạy nhanh

```powershell
cd "D:\KhoaLuanKiSu\QL_KhamChuaBenhNgoaiTru\AI"
python generate_graph_dataset.py
python train_node2vec.py
uvicorn recommend_api:app --host 127.0.0.1 --port 8000
```

Giai đoạn 1
Dùng dữ liệu của bạn để tạo graph:

bệnh
triệu chứng
thuốc
hoạt chất
nhóm thuốc
và dùng rule thủ công để sinh cạnh bệnh -> thuốc.

Giai đoạn 2
Khi hệ thống chạy thật:

bác sĩ chọn thuốc
lưu log
tạo lịch sử kê đơn
retrain node2vec
