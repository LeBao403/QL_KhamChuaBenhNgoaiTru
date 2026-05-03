from fastapi import FastAPI, HTTPException
from database import lay_du_lieu_don_thuoc
from custom_node2vec import CustomNode2Vec

app = FastAPI(title="Hệ Thống Gợi Ý Thuốc AI")

# Tinh chỉnh tham số cho phù hợp với dữ liệu hiện tại
ai_model = CustomNode2Vec(dimensions=32, walk_length=5, num_walks=100, p=0.5, q=2.0, epochs=50, learning_rate=0.02)
ai_model.load_model()

@app.post("/api/train")
def api_train_model():
    """Học lại từ đầu và tạo file model mới"""
    try:
        du_lieu = lay_du_lieu_don_thuoc()
        if not du_lieu or len(du_lieu) == 0:
            return {"message": "Không có đủ dữ liệu từ Database để train."}
        
        ai_model.train(du_lieu)
        return {"message": "Huấn luyện AI thành công!", "so_luong_toa_da_hoc": len(du_lieu)}
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))

@app.get("/api/recommend")
def api_recommend_drug(node_id: str, top_k: int = 40):
    """
    Nhận diện node_id chung (có thể là Mã Thuốc hoặc Mã Bệnh).
    Có thể truyền thêm tham số top_k từ C# xuống nếu muốn động.
    """
    if not ai_model.vectors:
        return {"success": False, "message": "Model chưa được train. Hãy gọi API train trước."}
    
    goi_y = ai_model.get_similar_drugs(node_id, top_k=top_k)
    
    if not goi_y:
        return {"success": False, "message": f"Không tìm thấy dữ liệu liên quan cho {node_id}"}
    
    return {"success": True, "MaGoc": node_id, "GoiY": goi_y}