# GIẢI THÍCH THUẬT TOÁN ĐỀ XUẤT THUỐC BẰNG NODE2VEC (BẢN TỐI ƯU HÓA)

Tài liệu này giải thích chi tiết cấu trúc thuật toán AI được áp dụng trong hệ thống nhằm hỗ trợ bảo vệ khóa luận. Hệ thống áp dụng thuật toán **Node2Vec**, một phương pháp học máy hiện đại thuộc nhóm Graph Representation Learning (Học biểu diễn Đồ thị).

## 1. Nguồn Dữ Liệu
Dữ liệu huấn luyện được truy xuất trực tiếp từ Cơ sở dữ liệu của dự án (Bảng `CT_DON_THUOC`).
- Khác với dữ liệu text thông thường, dữ liệu ở đây là các **Đơn thuốc**. Mỗi đơn thuốc (`MaDonThuoc`) chứa một danh sách các loại thuốc (`MaThuoc`).
- Dữ liệu thô chỉ thể hiện: "Trong đơn thuốc X có kê Thuốc Y và Thuốc Z".
- Hệ thống (thông qua file `database.py`) sẽ lọc và trích xuất toàn bộ các đơn thuốc có **từ 2 loại thuốc trở lên**. Sau đó, các thuốc trong cùng một đơn sẽ được coi là có liên kết với nhau.

*(Lưu ý: Đối với tập dữ liệu thực tế khá mỏng (~13.000 dòng chi tiết, tương đương khoảng 4.000 đơn thuốc), thuật toán đã được thiết kế lại siêu tham số để chống Overfitting: giảm số chiều vector xuống `16`, tăng số vòng lặp `epochs` lên `150` và áp dụng Subsampling tự nhiên thay vì TF-IDF).*

## 2. Xây Dựng Đồ Thị (Graph Construction)
Bước đầu tiên của Node2Vec là biến cơ sở dữ liệu dạng bảng thành cấu trúc Đồ thị (Graph). Đồ thị này là đồ thị vô hướng có trọng số:

*   **Node (Đỉnh đồ thị)**: Mỗi loại thuốc (Mã thuốc) là một Node độc lập.
*   **Edge (Cạnh kết nối)**: Nếu 2 loại thuốc A và B xuất hiện cùng nhau trong ít nhất một đơn thuốc, một cạnh sẽ được tạo ra nối giữa A và B.
*   **Trọng số (Edge Weight)**: Tổng số lần hai loại thuốc A và B xuất hiện cùng nhau trong toàn bộ lịch sử khám bệnh. Tần suất kê đơn chung càng nhiều, trọng số (lực hút) giữa 2 thuốc càng lớn.

## 3. Kiến Trúc Thuật Toán Lõi

Hệ thống Node2Vec tự xây dựng trong file `custom_node2vec.py` bao gồm 2 giai đoạn tính toán cốt lõi tuân thủ tuyệt đối định nghĩa toán học của thuật toán:

### Giai đoạn 1: Trích xuất chuỗi bằng Biased Random Walks (Đi dạo ngẫu nhiên có định hướng)
Để mạng Neural có thể học được Đồ thị, ta phải biến Đồ thị thành các chuỗi tuần tự (giống như các câu văn bản). Hệ thống sử dụng một tác nhân (agent) nhảy ngẫu nhiên từ thuốc này sang thuốc khác trên đồ thị.

Khác với DeepWalk (nhảy mù quáng), Node2Vec thông minh hơn nhờ **2 tham số điều hướng (Biased)** tại hàm `get_alias_edge()`:
-   **Tham số $p$ (Return parameter)**: Điều khiển xác suất tác nhân "quay gót" trở lại loại thuốc vừa đi qua. Tính chất này giúp mô hình nhận diện được các "Phác đồ cục bộ" (Local Community / BFS).
-   **Tham số $q$ (In-out parameter)**: Điều khiển xác suất tác nhân mạnh dạn đi ra xa, khám phá các loại thuốc mới. Tính chất này giúp bắt được cấu trúc tương đồng toàn cục (Global structure / DFS) - các loại thuốc có vai trò giống nhau dù không trực tiếp nằm chung đơn.

Sau bước này, đồ thị phức tạp được trải phẳng thành hàng ngàn chuỗi thuốc (Walks).

### Giai đoạn 2: Học Không gian Vector (Skip-gram Word2Vec)
Hệ thống sử dụng mạng Trí tuệ nhân tạo (Neural Network) 1 lớp ẩn theo cấu trúc Skip-gram. 
-   **Cách học**: Đưa vào một loại thuốc trung tâm, bắt mạng Neural phải dự đoán được các loại thuốc lân cận nó trong chuỗi Random Walk.
-   Các kỹ thuật tối ưu hóa Toán học được nhúng trực tiếp:
    1.  **Subsampling of Frequent Drugs**: Các loại thuốc "quốc dân" xuất hiện quá nhiều (như Paracetamol) sẽ che lấp các thuốc đặc trị hiếm. Hệ thống tự động tính xác suất bỏ qua (drop) các thuốc phổ biến này trong lúc học dựa trên tần suất xuất hiện, giúp bảng trọng số không bị chênh lệch.
    2.  **Negative Sampling chuẩn hóa**: Để huấn luyện mạng neural 1000 node, ta không cần cập nhật sai số cho cả 1000 node mỗi vòng lặp. Hệ thống chỉ cập nhật trọng số cho thuốc mục tiêu (Positive) và **5 loại thuốc ngẫu nhiên khác (Negative Samples)**. Việc bốc mẫu ngẫu nhiên tuân theo phân phối Unigram mũ 0.75 để đảm bảo công bằng cho thuốc hiếm.
    3.  **Learning Rate Decay**: Tốc độ học (Gradient step) giảm dần tuyến tính theo thời gian. Ở các vòng lặp (Epoch) cuối, mô hình bước những bước cực nhỏ để trúng đích chính xác thay vì nhảy vọt (vượt quá cực tiểu).

## 4. Cách Suy Luận & Đề Xuất (Inference)
Kết quả cuối cùng của quá trình trên: Mỗi loại thuốc (Node) ban đầu được biến đổi thành một **Vector toán học 16 chiều** mang đầy đủ ngữ nghĩa y khoa.
- Hai loại thuốc thường được kê chung (Cùng phác đồ) hoặc có chức năng thay thế nhau sẽ có tọa độ rất gần nhau trong không gian 16 chiều này.

**Cách gợi ý (Recommend):**
1. Khi bác sĩ chọn 1 hoặc nhiều thuốc: Hệ thống tính **Trung bình cộng** các vector của các loại thuốc đó tạo thành một `Query Vector`.
2. Hệ thống tính toán độ tương đồng **Cosine (Cosine Similarity)** giữa `Query Vector` và toàn bộ các loại thuốc còn lại trong từ điển. Thuốc nào có khoảng cách Cosine gần bằng 1 nhất sẽ được đưa lên Top 1 gợi ý.
