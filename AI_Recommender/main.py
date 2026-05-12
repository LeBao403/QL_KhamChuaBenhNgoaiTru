from fastapi import FastAPI, HTTPException
from fastapi.staticfiles import StaticFiles
from database import lay_du_lieu_don_thuoc
from custom_node2vec import CustomNode2Vec
import os
from pydantic import BaseModel
from typing import List

app = FastAPI(title="Hệ Thống Gợi Ý Thuốc AI")

# TỰ ĐỘNG TẠO THƯ MỤC MODELS NẾU CHƯA CÓ TRƯỚC KHI MOUNT
if not os.path.exists("models"):
    os.makedirs("models")

# Mở cổng truy cập file tĩnh để C# có thể lấy ảnh đồ thị và file CSV
app.mount("/models", StaticFiles(directory="models"), name="models")

# Tinh chỉnh tham số cho phù hợp với dữ liệu hiện tại
ai_model = CustomNode2Vec(dimensions=32, walk_length=5, num_walks=100, p=0.5, q=2.0, epochs=50, learning_rate=0.02)
ai_model.load_model()

@app.post("/api/train")
def api_train_model():
    """Học lại từ đầu, lưu model và tạo đồ thị phân cụm"""
    try:
        du_lieu = lay_du_lieu_don_thuoc()
        if not du_lieu or len(du_lieu) == 0:
            return {"message": "Không có đủ dữ liệu từ Database để train."}
        
        # CHÚ Ý: Đổi sang hàm train_and_evaluate (80% train, 20% test)
        ai_model.train_and_evaluate(du_lieu, test_ratio=0.2)
        
        return {
            "message": "Huấn luyện AI thành công!", 
            "so_luong_toa_da_hoc": len(du_lieu),
            "graph_url_static": "/models/drug_graph_3d.png",  
            "graph_url_interactive": "/models/drug_graph_3d.html", 
            "csv_url": "/models/drug_features.csv" 
        }
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))

# Định nghĩa cấu trúc Dữ liệu C# sẽ gửi sang
class RecommendRequest(BaseModel):
    danh_sach_thuoc: List[str]  # Mảng chứa các mã thuốc (vd: ["TH001", "TH005"])
    top_k: int = 40  # Số lượng gợi ý muốn nhận về (mặc định là 40)

@app.post("/api/recommend")
def api_recommend_drug(request: RecommendRequest):
    """
    Gợi ý thuốc dựa trên mảng các loại thuốc đang có trong Đơn.
    """
    if not ai_model.vectors:
        return {"success": False, "message": "Model chưa được train. Hãy gọi API train trước."}
    
    if not request.danh_sach_thuoc or len(request.danh_sach_thuoc) == 0:
        return {"success": False, "message": "Danh sách thuốc đầu vào trống."}
    
    # Gọi hàm xử lý mảng ta vừa viết ở bước 2
    goi_y = ai_model.get_similar_drugs_from_list(request.danh_sach_thuoc, top_k=request.top_k)
    
    if not goi_y:
        return {"success": False, "message": "Không tìm thấy gợi ý phù hợp cho tổ hợp thuốc này."}
    
    return {
        "success": True, 
        "ToaHienTai": request.danh_sach_thuoc, 
        "GoiY": goi_y
    }