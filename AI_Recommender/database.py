import pyodbc
import os
from dotenv import load_dotenv

# Nạp cấu hình từ file .env vào bộ nhớ hệ thống
load_dotenv()

# Lấy các giá trị cấu hình từ môi trường, có kèm giá trị mặc định nếu không tìm thấy
SERVER = os.getenv('DB_SERVER', 'localhost')
DATABASE = os.getenv('DB_NAME', 'QL_KhamBenhNgoaiTru')
TRUSTED = os.getenv('DB_TRUSTED_CONNECTION', 'yes')
USER = os.getenv('DB_USER', '')
PASS = os.getenv('DB_PASS', '')

def get_db_connection():
    """
    Thiết lập kết nối tới SQL Server dựa trên cấu hình từ .env
    """
    if TRUSTED.lower() == 'yes':
        # Kết nối bằng quyền Windows (Thường dùng cho máy cá nhân)
        conn_str = (
            f'DRIVER={{ODBC Driver 17 for SQL Server}};'
            f'SERVER={SERVER};'
            f'DATABASE={DATABASE};'
            f'Trusted_Connection=yes;'
        )
    else:
        # Kết nối bằng tài khoản SQL Server (sa)
        conn_str = (
            f'DRIVER={{ODBC Driver 17 for SQL Server}};'
            f'SERVER={SERVER};'
            f'DATABASE={DATABASE};'
            f'UID={USER};'
            f'PWD={PASS};'
        )
    
    return pyodbc.connect(conn_str)

def lay_du_lieu_don_thuoc():
    """
    Trích xuất dữ liệu toa thuốc, thực hiện lọc và báo cáo thống kê
    """
    conn = get_db_connection()
    cursor = conn.cursor()
    
    # 1. Thống kê tổng số dòng để kiểm tra dữ liệu gốc
    cursor.execute("SELECT COUNT(*) FROM CT_DON_THUOC")
    total_rows_db = cursor.fetchone()[0]
    
    # 2. Lấy dữ liệu các đơn thuốc có từ 2 loại thuốc trở lên (Điều kiện bắt buộc để train Node2Vec)
    query = """
        SELECT MaDonThuoc, MaThuoc 
        FROM CT_DON_THUOC 
        WHERE MaDonThuoc IN (
            SELECT MaDonThuoc FROM CT_DON_THUOC GROUP BY MaDonThuoc HAVING COUNT(MaThuoc) > 1
        )
    """
    cursor.execute(query)
    rows = cursor.fetchall()
    
    # 3. Gom nhóm dữ liệu theo mã đơn và xóa khoảng trắng thừa (Tránh lỗi CHAR/VARCHAR)
    don_thuoc_dict = {}
    for row in rows:
        ma_don = row.MaDonThuoc.strip()
        ma_thuoc = row.MaThuoc.strip()
        
        if ma_don not in don_thuoc_dict:
            don_thuoc_dict[ma_don] = []
        don_thuoc_dict[ma_don].append(ma_thuoc)
    
    # 4. In báo cáo thống kê ra Terminal để đối soát
    so_dong_lay_duoc = len(rows)
    so_don_hop_le = len(don_thuoc_dict)
    
    print("-" * 50)
    print("BÁO CÁO TRÍCH XUẤT DỮ LIỆU AI:")
    print(f"- Tổng số dòng trong DB: {total_rows_db}")
    print(f"- Số dòng thỏa điều kiện (Toa > 1 thuốc): {so_dong_lay_duoc}")
    print(f"- Tổng số Toa thuốc hợp lệ để huấn luyện: {so_don_hop_le}")
    print(f"- Tỷ lệ dữ liệu hữu dụng: {(so_dong_lay_duoc/total_rows_db)*100:.2f}%")
    print("-" * 50)
    
    conn.close()
    return list(don_thuoc_dict.values())