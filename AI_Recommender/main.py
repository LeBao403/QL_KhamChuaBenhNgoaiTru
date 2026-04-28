from fastapi import FastAPI, HTTPException
from database import lay_du_lieu_don_thuoc
from custom_node2vec import CustomNode2Vec

app = FastAPI(title="Hệ Thống Gợi Ý Thuốc AI")

# Biến toàn cục lưu model AI
ai_model = CustomNode2Vec(dimensions=32, walk_length=5, num_walks=100, p=0.5, q=2.0, epochs=50, learning_rate=0.02)

ai_model.load_model()

@app.post("/api/train")
def api_train_model():
    """C# sẽ gọi API này khi bác bấm nút 'Cập nhật AI' trên Web"""
    try:
        # 1. Lấy dữ liệu từ DB
        du_lieu = lay_du_lieu_don_thuoc()
        if not du_lieu:
            return {"message": "Không có đủ dữ liệu toa thuốc để train."}
        
        # 2. Bắt đầu train
        ai_model.train(du_lieu)
        return {"message": "Huấn luyện AI thành công!", "so_luong_toa_da_hoc": len(du_lieu)}
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))

@app.get("/api/recommend")
def api_recommend_drug(mathuoc: str):
    """C# sẽ gọi API này lúc bác sĩ đang gõ đơn thuốc để lấy gợi ý"""
    if not ai_model.vectors:
        return {"message": "Model chưa được train. Hãy gọi API train trước."}
    
    goi_y = ai_model.get_similar_drugs(mathuoc, top_k=3)
    
    if not goi_y:
        return {"message": f"Không tìm thấy dữ liệu cho thuốc {mathuoc}"}
    
    return {"MaThuocGoc": mathuoc, "GoiY": goi_y}