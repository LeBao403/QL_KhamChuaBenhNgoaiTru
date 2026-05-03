import pyodbc
import os
from dotenv import load_dotenv

# Nạp cấu hình từ file .env vào bộ nhớ hệ thống
load_dotenv()

SERVER = os.getenv('DB_SERVER', 'localhost')
DATABASE = os.getenv('DB_NAME', 'QL_KhamBenhNgoaiTru')
TRUSTED = os.getenv('DB_TRUSTED_CONNECTION', 'yes')
USER = os.getenv('DB_USER', '')
PASS = os.getenv('DB_PASS', '')

def get_db_connection():
    if TRUSTED.lower() == 'yes':
        conn_str = f'DRIVER={{ODBC Driver 17 for SQL Server}};SERVER={SERVER};DATABASE={DATABASE};Trusted_Connection=yes;'
    else:
        conn_str = f'DRIVER={{ODBC Driver 17 for SQL Server}};SERVER={SERVER};DATABASE={DATABASE};UID={USER};PWD={PASS};'
    return pyodbc.connect(conn_str)

def lay_du_lieu_don_thuoc():
    conn = get_db_connection()
    cursor = conn.cursor()
    
    # SỬ DỤNG ĐÚNG TÊN BẢNG VÀ CẤU TRÚC THEO FILE Create_QLKCB.sql
    query = """
        SELECT 
            dt.MaPhieuKhamBenh, 
            ISNULL(cd.MaBenh, 'KHONG_RO') AS MaBenh, 
            ct.MaThuoc 
        FROM CT_DON_THUOC ct
        JOIN DON_THUOC dt ON ct.MaDonThuoc = dt.MaDonThuoc
        LEFT JOIN CHITIET_CHANDOAN cd ON dt.MaPhieuKhamBenh = cd.MaPhieuKhamBenh
    """
    try:
        cursor.execute(query)
        rows = cursor.fetchall()
        
        # Gom nhóm dữ liệu theo từng Phiếu khám bệnh
        don_thuoc_dict = {}
        for row in rows:
            ma_phieu = str(row.MaPhieuKhamBenh).strip()
            ma_benh = str(row.MaBenh).strip()
            ma_thuoc = str(row.MaThuoc).strip()
            
            if ma_phieu not in don_thuoc_dict:
                # Dùng Set để lọc trùng lặp nếu 1 phiếu có nhiều thuốc & nhiều bệnh
                don_thuoc_dict[ma_phieu] = {'benh': set(), 'thuoc': set()}
                
            if ma_benh != 'KHONG_RO':
                don_thuoc_dict[ma_phieu]['benh'].add(ma_benh)
                
            don_thuoc_dict[ma_phieu]['thuoc'].add(ma_thuoc)
            
        # Chuyển đổi dữ liệu thành các chuỗi (walks) cho Node2Vec học
        # Cấu trúc 1 dòng học: [Mã_Bệnh_1, Mã_Bệnh_2, Mã_Thuốc_A, Mã_Thuốc_B...]
        danh_sach_hoc = []
        for ma_phieu, data in don_thuoc_dict.items():
            benh_list = list(data['benh']) if data['benh'] else ['KHONG_RO']
            thuoc_list = list(data['thuoc'])
            
            walk = benh_list + thuoc_list
            if len(walk) > 1: # Chỉ train những toa có từ 2 Node trở lên
                danh_sach_hoc.append(walk)
        
        print(f"Đã trích xuất {len(danh_sach_hoc)} toa thuốc hợp lệ để huấn luyện.")
        conn.close()
        return danh_sach_hoc
        
    except Exception as e:
        print("Lỗi kết nối hoặc truy vấn DB:", e)
        return []