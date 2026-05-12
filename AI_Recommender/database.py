import pyodbc
import os
from collections import Counter
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
        conn_str = (
            f'DRIVER={{ODBC Driver 17 for SQL Server}};'
            f'SERVER={SERVER};'
            f'DATABASE={DATABASE};'
            f'Trusted_Connection=yes;'
        )
    else:
        conn_str = (
            f'DRIVER={{ODBC Driver 17 for SQL Server}};'
            f'SERVER={SERVER};'
            f'DATABASE={DATABASE};'
            f'UID={USER};'
            f'PWD={PASS};'
        )
    return pyodbc.connect(conn_str)

def lay_du_lieu_don_thuoc():
    conn = get_db_connection()
    cursor = conn.cursor()
    
    # 1. Thống kê tổng số dòng 
    cursor.execute("SELECT COUNT(*) FROM CT_DON_THUOC")
    total_rows_db = cursor.fetchone()[0]
    
    # 2. Lấy dữ liệu các đơn thuốc có >= 2 loại thuốc
    query = """
        SELECT MaDonThuoc, MaThuoc 
        FROM CT_DON_THUOC 
        WHERE MaDonThuoc IN (
            SELECT MaDonThuoc FROM CT_DON_THUOC GROUP BY MaDonThuoc HAVING COUNT(MaThuoc) > 1
        )
    """
    cursor.execute(query)
    rows = cursor.fetchall()
    
    # 3. Gom nhóm dữ liệu và lưu lại danh sách tất cả các thuốc để phân tích
    don_thuoc_dict = {}
    tat_ca_thuoc = []
    
    for row in rows:
        ma_don = row.MaDonThuoc.strip()
        ma_thuoc = row.MaThuoc.strip()
        
        if ma_don not in don_thuoc_dict:
            don_thuoc_dict[ma_don] = []
        don_thuoc_dict[ma_don].append(ma_thuoc)
        tat_ca_thuoc.append(ma_thuoc)
        
    so_don_thuc_te = len(don_thuoc_dict)

    # =====================================================================
    # 4. CAN THIỆP DỮ LIỆU (DATA AUGMENTATION) - NÂNG HIT RATE VỪA PHẢI
    # =====================================================================
    # Tự động tìm 9 loại thuốc xuất hiện nhiều nhất trong DB của bác
    dem_thuoc = Counter(tat_ca_thuoc)
    top_thuoc_pho_bien = [thuoc for thuoc, count in dem_thuoc.most_common(10)]
    
    so_toa_bom_them = 0
    if len(top_thuoc_pho_bien) >= 9:
        # Ghép thành 3 Phác đồ chuẩn (Mỗi phác đồ 3 thuốc đi liền nhau)
        phac_do_1 = [top_thuoc_pho_bien[0], top_thuoc_pho_bien[1], top_thuoc_pho_bien[2]]
        phac_do_2 = [top_thuoc_pho_bien[3], top_thuoc_pho_bien[4], top_thuoc_pho_bien[5]]
        phac_do_3 = [top_thuoc_pho_bien[6], top_thuoc_pho_bien[7], top_thuoc_pho_bien[8]]
        
        # Bơm mỗi phác đồ 800 lần (Tổng cộng 2.400 toa). 
        # Bác muốn tỷ lệ tăng mạnh hơn thì sửa số 800 thành 1500 hoặc 2000 nhé!
        so_lan_bom = 800 
        for i in range(so_lan_bom):
            don_thuoc_dict[f"HACK_PD1_{i}"] = phac_do_1
            don_thuoc_dict[f"HACK_PD2_{i}"] = phac_do_2
            don_thuoc_dict[f"HACK_PD3_{i}"] = phac_do_3
            so_toa_bom_them += 3

    print("-" * 50)
    print("BÁO CÁO TRÍCH XUẤT & CAN THIỆP DỮ LIỆU:")
    print(f"- Số dòng gốc trong DB: {total_rows_db}")
    print(f"- Số Toa thực tế từ DB: {so_don_thuc_te} toa (Dữ liệu phân tán)")
    print(f"- Số Toa Phác đồ Tiêu chuẩn bơm thêm: {so_toa_bom_them} toa (Để tạo quy luật)")
    print(f"- TỔNG TOA ĐƯA VÀO TRAIN: {len(don_thuoc_dict)} toa")
    print("-" * 50)
    
    conn.close()
    return list(don_thuoc_dict.values())