# GIỚI THIỆU THUẬT TOÁN NODE2VEC VÀ PHÂN TÍCH MÔ-ĐUN GỢI Ý THUỐC TRONG ĐỀ TÀI

Tài liệu này trình bày cơ sở lý thuyết của thuật toán Node2Vec, cách thuật toán hoạt động trên đồ thị, ý nghĩa các tham số, quy trình huấn luyện vector nhúng và cách sử dụng kết quả nhúng cho bài toán gợi ý. Phần cuối phân tích cụ thể thuật toán đang được cài đặt trong mô-đun AI của đề tài quản lý khám chữa bệnh ngoại trú.

## 1. Tổng quan về học biểu diễn trên đồ thị

Trong nhiều bài toán thực tế, dữ liệu không chỉ tồn tại dưới dạng bảng độc lập mà còn có quan hệ giữa các đối tượng. Đồ thị là mô hình tự nhiên để biểu diễn dạng dữ liệu này. Một đồ thị G = (V, E) gồm tập đỉnh V và tập cạnh E; mỗi đỉnh biểu diễn một thực thể, mỗi cạnh biểu diễn quan hệ giữa hai thực thể. Trong hệ thống gợi ý thuốc, đỉnh có thể là thuốc hoặc bệnh, còn cạnh thể hiện việc hai thực thể cùng xuất hiện trong một đơn thuốc hoặc một lượt khám.

Học biểu diễn trên đồ thị, còn gọi là graph representation learning hoặc graph embedding, là quá trình biến mỗi đỉnh thành một vector số thực có số chiều thấp hơn nhiều so với kích thước đồ thị ban đầu. Mục tiêu là các đỉnh có quan hệ gần nhau trong đồ thị sẽ có vector gần nhau trong không gian nhúng. Khi đã có vector, hệ thống có thể áp dụng các phép toán như tính độ tương đồng cosine, phân cụm, phân loại hoặc dự đoán liên kết.

Node2Vec thuộc nhóm phương pháp học biểu diễn không giám sát dựa trên random walk. Ý tưởng cốt lõi là chuyển cấu trúc đồ thị thành các chuỗi đỉnh tương tự như câu trong xử lý ngôn ngữ tự nhiên, sau đó dùng mô hình Skip-gram để học vector cho từng đỉnh.

## 2. Định nghĩa thuật toán Node2Vec

Node2Vec là thuật toán học biểu diễn vector cho các đỉnh trong đồ thị, được đề xuất bởi Aditya Grover và Jure Leskovec tại KDD 2016. Thuật toán học một ánh xạ f: V -> R^d, trong đó mỗi đỉnh v thuộc V được biểu diễn bởi một vector d chiều. Vector này cần bảo toàn được thông tin lân cận của đỉnh trong đồ thị, nghĩa là các đỉnh có ngữ cảnh đồ thị tương tự nhau sẽ có vector gần nhau.

Điểm khác biệt quan trọng của Node2Vec so với DeepWalk là cơ chế random walk có điều hướng. DeepWalk sinh các bước đi ngẫu nhiên gần như đồng nhất theo hàng xóm của đỉnh hiện tại, còn Node2Vec bổ sung hai tham số p và q để điều khiển xu hướng đi bộ trên đồ thị. Nhờ đó, thuật toán có thể linh hoạt cân bằng giữa việc khai thác các đỉnh gần nhau về cộng đồng và các đỉnh có vai trò cấu trúc tương tự.

| Khái niệm | Ý nghĩa trong Node2Vec | Liên hệ với bài toán gợi ý thuốc |
|---|---|---|
| Node/đỉnh | Thực thể được học vector biểu diễn | Mã thuốc, hoặc mở rộng thêm mã bệnh |
| Edge/cạnh | Quan hệ giữa hai node | Hai thuốc cùng xuất hiện trong một đơn; bệnh và thuốc cùng xuất hiện trong lượt khám |
| Weight/trọng số | Mức độ mạnh yếu của quan hệ | Số lần hai thuốc được kê chung |
| Random walk | Chuỗi đỉnh được sinh bằng cách đi trên đồ thị | Chuỗi thuốc mô phỏng thói quen kê đơn |
| Embedding | Vector số thực biểu diễn node | Vector thuốc dùng để tính thuốc tương tự |
| Context/ngữ cảnh | Các node xuất hiện gần nhau trong random walk | Các thuốc thường đi kèm trong cùng phác đồ hoặc cùng nhóm bệnh |

## 3. Tư tưởng chính của Node2Vec

Node2Vec dựa trên một phép tương tự: nếu Word2Vec học nghĩa của từ từ các câu trong văn bản, thì Node2Vec học ý nghĩa của đỉnh từ các chuỗi đỉnh được sinh ra trên đồ thị. Trong văn bản, những từ xuất hiện trong cùng ngữ cảnh thường có quan hệ ngữ nghĩa; trong đồ thị, những đỉnh xuất hiện gần nhau trong các random walk thường có quan hệ cấu trúc hoặc quan hệ cộng đồng.

Quy trình tổng quát gồm các bước sau:

- Biểu diễn dữ liệu dưới dạng đồ thị có trọng số.
- Sinh nhiều random walk từ từng node trong đồ thị.
- Coi mỗi random walk như một câu, mỗi node như một từ.
- Dùng Skip-gram học vector sao cho node trung tâm dự đoán tốt các node ngữ cảnh.
- Dùng vector đã học cho các tác vụ như gợi ý, phân cụm, phân loại hoặc dự đoán liên kết.

## 4. Cơ chế random walk có điều hướng

Thành phần quan trọng nhất của Node2Vec là random walk bậc hai. Khi đang đứng ở node v và trước đó đi từ node t đến v, thuật toán không chỉ nhìn hàng xóm x của v mà còn xét quan hệ giữa x và node trước đó t. Điều này làm bước đi tiếp theo phụ thuộc vào cạnh vừa đi qua, nhờ đó thuật toán điều khiển được xu hướng quay lại, đi gần hoặc đi xa.

Xác suất chuyển từ v sang x tỉ lệ với trọng số cạnh w_vx nhân với hệ số điều hướng alpha_pq(t, x):

pi_vx = alpha_pq(t, x) * w_vx

Trong đó alpha_pq(t, x) được xác định theo khoảng cách ngắn nhất giữa node t và node x.

| Trường hợp | Giá trị alpha | Ý nghĩa |
|---|---|---|
| x = t | 1 / p | Quay lại node vừa đi qua. p nhỏ làm thuật toán dễ quay lại hơn; p lớn làm thuật toán ít quay lại hơn. |
| x kề với t | 1 | Đi trong vùng lân cận gần, giữ walk quanh cộng đồng hiện tại. |
| x không kề với t | 1 / q | Đi ra xa khỏi vùng hiện tại. q nhỏ khuyến khích khám phá xa; q lớn hạn chế đi xa. |

Hai tham số p và q quyết định phong cách khám phá đồ thị. Nếu muốn học các node cùng cộng đồng, thuật toán thường chọn cách đi gần giống BFS. Nếu muốn phát hiện các node có vai trò cấu trúc tương tự dù ở xa nhau, thuật toán có thể đi theo hướng giống DFS hơn.

| Thiết lập tham số | Hành vi trực giác | Khi nào hữu ích |
|---|---|---|
| p nhỏ | Dễ quay lại node trước | Tăng cường khai thác vùng cục bộ, tránh walk đi quá xa |
| p lớn | Ít quay lại | Khuyến khích khám phá các node mới |
| q nhỏ | Dễ đi ra xa | Tìm các node có vai trò tương tự ở vùng khác của đồ thị |
| q lớn | Ưu tiên đi quanh vùng gần | Tìm các node cùng cộng đồng hoặc cùng nhóm liên hệ |

## 5. Quan hệ giữa Node2Vec, DeepWalk và Word2Vec

DeepWalk là một tiền đề quan trọng của Node2Vec. DeepWalk dùng random walk để tạo chuỗi đỉnh, sau đó áp dụng Skip-gram giống Word2Vec. Node2Vec kế thừa ý tưởng này nhưng thay random walk đồng nhất bằng random walk có điều hướng thông qua p và q. Nhờ vậy, Node2Vec linh hoạt hơn trong việc bảo toàn hai kiểu thông tin: homophily và structural equivalence.

Homophily là hiện tượng các node kết nối gần nhau hoặc cùng cộng đồng thường giống nhau. Trong bài toán thuốc, các thuốc thường được kê cùng nhau cho một nhóm bệnh có thể tạo thành cụm homophily. Structural equivalence là hiện tượng các node có vai trò tương tự trong đồ thị dù không nhất thiết ở gần nhau. Ví dụ, hai thuốc ở hai nhóm đơn khác nhau nhưng đều đóng vai trò thuốc hỗ trợ trong phác đồ có thể có cấu trúc liên kết tương tự.

| Thuật toán | Cách sinh dữ liệu | Điểm mạnh | Hạn chế |
|---|---|---|---|
| Word2Vec | Câu văn tự nhiên | Học quan hệ ngữ nghĩa giữa từ | Không xử lý trực tiếp dữ liệu đồ thị |
| DeepWalk | Random walk trên đồ thị | Đơn giản, hiệu quả, dễ cài đặt | Ít kiểm soát kiểu khám phá đồ thị |
| Node2Vec | Biased random walk với p, q | Linh hoạt giữa BFS và DFS, phù hợp nhiều loại đồ thị | Cần chọn tham số và cài đặt xác suất chuyển phức tạp hơn |

## 6. Mô hình Skip-gram và Negative Sampling

Sau khi có các random walk, Node2Vec coi mỗi chuỗi walk như một câu. Với một node trung tâm u, các node nằm trong cửa sổ trượt xung quanh u được xem là context. Mục tiêu học là vector của u phải dự đoán tốt các node context xuất hiện gần nó. Đây là cùng tư tưởng với Skip-gram trong Word2Vec.

Nếu tính xác suất trên toàn bộ tập node ở mỗi bước học thì chi phí rất lớn. Negative Sampling giải quyết bằng cách học trên một số mẫu âm. Với một cặp dương (target, context) xuất hiện thật trong random walk, mô hình kéo vector của chúng lại gần nhau. Đồng thời, mô hình chọn một hoặc vài node ngẫu nhiên làm mẫu âm và đẩy vector của target ra xa các node âm đó.

Trong cài đặt đơn giản, điểm liên quan giữa hai node được tính bằng tích vô hướng của vector target và vector context, sau đó đưa qua hàm sigmoid. Với mẫu dương, nhãn mong muốn là 1; với mẫu âm, nhãn mong muốn là 0. Quá trình gradient descent cập nhật vector qua nhiều epoch để giảm loss.

## 7. Độ tương đồng cosine và gợi ý sau huấn luyện

Sau khi huấn luyện, mỗi node có một vector embedding. Để gợi ý các node liên quan, hệ thống lấy vector của node đầu vào và tính độ tương đồng với các vector còn lại. Độ tương đồng cosine được dùng phổ biến vì nó đo góc giữa hai vector, ít bị ảnh hưởng bởi độ lớn tuyệt đối của vector.

Công thức cosine similarity giữa hai vector a và b là:

cos(a, b) = (a . b) / (||a|| * ||b||)

Giá trị càng gần 1 thì hai vector càng cùng hướng, nghĩa là hai node càng giống nhau theo ngữ cảnh đã học. Trong bài toán gợi ý thuốc, nếu thuốc A và thuốc B thường xuất hiện trong những ngữ cảnh kê đơn tương tự, vector của chúng có xu hướng gần nhau và B có thể được đưa vào danh sách gợi ý khi bác sĩ chọn A.

## 8. Ưu điểm và hạn chế của Node2Vec

| Khía cạnh | Ưu điểm | Hạn chế/cần lưu ý |
|---|---|---|
| Dữ liệu đầu vào | Tận dụng được quan hệ tự nhiên trong dữ liệu đồ thị | Chất lượng phụ thuộc mạnh vào dữ liệu lịch sử |
| Huấn luyện | Không cần nhãn thủ công cho từng node | Cần chọn tham số walk_length, num_walks, p, q, window_size, dimensions |
| Khả năng mở rộng | Có thể dùng cho nhiều tác vụ sau khi có embedding | Đồ thị lớn cần tối ưu sampling và bộ nhớ |
| Diễn giải | Có thể giải thích bằng quan hệ đồng xuất hiện và độ gần vector | Vector nhúng không tự cung cấp lý do lâm sàng đầy đủ |
| Ứng dụng y tế | Hữu ích như công cụ tham khảo dựa trên mẫu kê đơn | Không thay thế bác sĩ; chưa kiểm tra dị ứng, chống chỉ định, tương tác thuốc nếu không bổ sung dữ liệu |

## 9. Ứng dụng Node2Vec vào bài toán gợi ý thuốc

Trong đề tài, dữ liệu kê đơn có thể được chuyển thành đồ thị đồng xuất hiện. Mỗi thuốc là một node. Nếu hai thuốc cùng xuất hiện trong một đơn thuốc, hệ thống tạo một cạnh giữa hai thuốc. Trọng số cạnh tăng theo số lần cặp thuốc đó được kê chung. Cách mô hình hóa này giả định rằng các thuốc thường xuất hiện cùng nhau trong lịch sử có thể có liên hệ về phác đồ, nhóm bệnh, mục đích điều trị hoặc thói quen kê đơn.

Khi mở rộng thêm mã bệnh, đồ thị trở thành đồ thị hỗn hợp bệnh - thuốc. Khi một bệnh và một thuốc cùng xuất hiện trong một lượt khám, hệ thống có thể tạo quan hệ giữa bệnh và thuốc. Cách này giúp truy vấn gợi ý không chỉ bắt đầu từ thuốc đang kê mà còn có thể bắt đầu từ mã bệnh/chẩn đoán.

Quy trình áp dụng trong hệ thống:

- Đầu vào: lịch sử đơn thuốc, chi tiết đơn thuốc, chẩn đoán hoặc mã bệnh nếu có.
- Xây đồ thị: node là thuốc/bệnh; edge là quan hệ đồng xuất hiện; weight là tần suất đồng xuất hiện.
- Huấn luyện: sinh random walk, học embedding bằng Skip-gram và Negative Sampling.
- Suy luận: lấy node đầu vào, tính cosine similarity, trả về top_k thuốc gần nhất.
- Vai trò trong hệ thống: hỗ trợ bác sĩ tham khảo khi kê đơn, không tự động quyết định đơn thuốc.

## 10. Phân tích thuật toán đang dùng trong project

Trong workspace hiện tại có hai thư mục AI đáng chú ý: `AI_Recommender_Quoc` và `AI_Recommender`. Cả hai đều triển khai lớp `CustomNode2Vec` bằng NumPy và cung cấp API bằng FastAPI. Web App gọi service tại địa chỉ `http://127.0.0.1:8000/api/recommend` từ `BacSiController` khi bác sĩ dùng chức năng gợi ý thuốc.

Bản `AI_Recommender_Quoc` là phiên bản cơ bản. File `database.py` lấy dữ liệu từ `CT_DON_THUOC`, `DON_THUOC` và `CHITIET_CHANDOAN`, gom theo `MaPhieuKhamBenh`. Mỗi mẫu học có thể gồm mã bệnh và danh sách thuốc. File `custom_node2vec.py` xây đồ thị đồng xuất hiện, sinh random walk có trọng số, huấn luyện Skip-gram với Negative Sampling và lưu vector vào `models/model_weights.json`.

Bản `AI_Recommender` là phiên bản mở rộng hơn. `database.py` lấy các đơn có từ hai thuốc trở lên từ `CT_DON_THUOC`, gom theo `MaDonThuoc`, sau đó có bước data augmentation: tự tìm các thuốc xuất hiện phổ biến và bơm thêm các phác đồ mẫu lặp nhiều lần để tạo quy luật rõ hơn. `custom_node2vec.py` bổ sung chia train/test, đánh giá Hit@10, Hit@20, Hit@30, áp dụng trọng số dạng TF-IDF để giảm ảnh hưởng của thuốc quá phổ biến, xuất vector ra CSV và trực quan hóa embedding bằng PCA 3D.

| Thành phần trong code | Vai trò | Nhận xét kỹ thuật |
|---|---|---|
| database.py | Trích xuất và gom dữ liệu kê đơn | Bản Quoc có bệnh + thuốc; bản AI_Recommender hiện tập trung vào thuốc và có data augmentation |
| build_graph | Tạo đồ thị đồng xuất hiện có trọng số | Cạnh giữa hai thuốc tăng trọng số khi xuất hiện chung trong đơn |
| generate_random_walks | Sinh chuỗi node để huấn luyện | Chọn hàng xóm bằng `random.choices` theo trọng số cạnh |
| train/train_and_evaluate | Huấn luyện embedding | Dùng Skip-gram, sigmoid, negative sampling, gradient descent |
| get_similar_drugs/get_similar_drugs_from_list | Suy luận gợi ý | Tính cosine similarity và trả về top_k thuốc gần nhất |
| main.py | API Layer | Cung cấp `/api/train` và `/api/recommend` cho Web App |

## 11. Điểm giống và khác giữa thuật toán gốc với code hiện tại

Về mặt ý tưởng, code hiện tại bám đúng pipeline lớn của Node2Vec: xây đồ thị, sinh random walk, học embedding bằng Skip-gram/Negative Sampling, sau đó gợi ý bằng cosine similarity. Đây là phần cốt lõi làm cho hệ thống có khả năng học quan hệ đồng xuất hiện giữa các thuốc thay vì chỉ lọc theo luật cố định.

Tuy nhiên, khi đối chiếu với Node2Vec gốc, có một điểm cần ghi nhận rõ: các tham số p và q được khai báo trong constructor nhưng hàm `generate_random_walks` hiện tại chưa sử dụng p và q để tính xác suất chuyển bậc hai `alpha_pq(t, x)`. Bước đi tiếp theo được chọn chủ yếu theo trọng số cạnh từ node hiện tại sang các hàng xóm. Vì vậy, implementation hiện tại nên được mô tả chính xác là mô hình gợi ý dựa trên tư tưởng Node2Vec, hoặc phiên bản rút gọn gần với weighted DeepWalk kết hợp Skip-gram/Negative Sampling.

Nếu báo cáo gọi là Node2Vec tự cài đặt, nên bổ sung giải thích rằng nhóm đã cài đặt phiên bản rút gọn, trong đó random walk có trọng số theo tần suất đồng xuất hiện nhưng chưa triển khai đầy đủ cơ chế biased second-order random walk của thuật toán gốc.

| Tiêu chí | Node2Vec gốc | Code hiện tại |
|---|---|---|
| Đồ thị có trọng số | Hỗ trợ | Có: trọng số là số lần đồng xuất hiện |
| Random walk | Biased second-order random walk | Random walk có trọng số cạnh, chưa dùng trạng thái node trước đó |
| Tham số p, q | Điều khiển quay lại/đi xa | Có khai báo nhưng chưa tham gia tính xác suất chuyển |
| Alias sampling | Thường dùng để tối ưu sampling | Chưa triển khai alias sampling; dùng `random.choices` |
| Skip-gram | Có | Có tự cài đặt bằng NumPy |
| Negative Sampling | Có | Có, mỗi cặp dương chọn một mẫu âm ngẫu nhiên |
| Đánh giá | Tùy downstream task | Bản `AI_Recommender` có Hit@10/20/30 trên tập test |

## 12. Đánh giá cách áp dụng trong đề tài

Cách áp dụng thuật toán trong đề tài phù hợp với mục tiêu xây dựng mô-đun gợi ý tham khảo cho bác sĩ. Dữ liệu đơn thuốc tự nhiên tạo thành mạng lưới đồng xuất hiện; embedding giúp hệ thống tìm các thuốc có ngữ cảnh kê đơn tương tự; API FastAPI giúp tách AI Service khỏi Web App ASP.NET MVC, thuận tiện huấn luyện và gọi gợi ý độc lập.

Điểm mạnh lớn nhất là nhóm không chỉ gọi một thư viện có sẵn mà đã tự cài đặt các thành phần học embedding bằng NumPy. Điều này giúp báo cáo có giá trị kỹ thuật: nhóm hiểu cách xây đồ thị, sinh dữ liệu học, cập nhật vector và tính độ tương đồng. Việc bổ sung đánh giá Hit@K trong bản `AI_Recommender` cũng giúp mô-đun có chỉ số kiểm tra ban đầu thay vì chỉ quan sát kết quả thủ công.

Dù vậy, có ba hạn chế cần nêu trung thực. Thứ nhất, random walk chưa dùng p và q đúng như Node2Vec gốc, nên khả năng điều khiển BFS/DFS chưa thật sự có hiệu lực. Thứ hai, data augmentation bằng cách bơm thêm phác đồ phổ biến có thể làm Hit@K tăng nhưng cũng dễ tạo thiên lệch, khiến mô hình ưu tiên các thuốc phổ biến hoặc phác đồ nhân tạo. Thứ ba, mô hình mới học từ quan hệ đồng xuất hiện, chưa kiểm tra an toàn lâm sàng như dị ứng, chống chỉ định, tương tác thuốc, tuổi, cân nặng, bệnh nền hoặc hướng dẫn điều trị chuẩn.

## 13. Đề xuất diễn đạt trong báo cáo

Để phần báo cáo vừa đúng kỹ thuật vừa bảo vệ được trước hội đồng, có thể diễn đạt mô-đun AI như sau:

Đề tài xây dựng mô-đun gợi ý thuốc dựa trên tư tưởng của Node2Vec. Dữ liệu lịch sử kê đơn được chuyển thành đồ thị đồng xuất hiện có trọng số, trong đó mỗi thuốc là một node và cạnh thể hiện quan hệ xuất hiện chung trong đơn thuốc. Từ đồ thị này, hệ thống sinh các chuỗi random walk có trọng số, sau đó tự cài đặt mô hình Skip-gram với Negative Sampling bằng NumPy để học vector nhúng cho từng thuốc. Khi bác sĩ chọn một bệnh hoặc một thuốc, hệ thống tính độ tương đồng cosine giữa vector đầu vào và các vector thuốc còn lại để trả về danh sách thuốc gợi ý.

Nếu muốn khẳng định là Node2Vec đầy đủ, cần nâng cấp `generate_random_walks` để dùng node trước đó, tính `alpha_pq(t, x)`, áp dụng p và q vào xác suất chuyển, đồng thời có thể bổ sung alias sampling để tối ưu tốc độ. Khi đó, phần mô tả thuật toán gốc và phần code sẽ khớp hoàn toàn hơn.

## 14. Kết luận

Node2Vec là một thuật toán học biểu diễn đồ thị hiệu quả, kết hợp random walk có điều hướng với Skip-gram để chuyển mỗi node thành một vector số thực. Trong bài toán gợi ý thuốc, thuật toán phù hợp vì lịch sử kê đơn có thể được mô hình hóa thành đồ thị đồng xuất hiện giữa thuốc, hoặc mở rộng thành đồ thị bệnh - thuốc. Sau huấn luyện, độ tương đồng giữa các vector giúp hệ thống tìm các thuốc có ngữ cảnh kê đơn gần nhau.

Mô-đun AI trong đề tài đã hiện thực được pipeline quan trọng của phương pháp này: trích xuất dữ liệu kê đơn, xây đồ thị, sinh random walk, học embedding, lưu mô hình và cung cấp API gợi ý cho Web App. Phần đang chạy có giá trị như một phiên bản rút gọn theo tư tưởng Node2Vec. Để hoàn thiện về mặt học thuật, hướng cải tiến quan trọng nhất là triển khai đầy đủ biased second-order random walk sử dụng p và q, đồng thời bổ sung các ràng buộc an toàn thuốc trước khi xem xét ứng dụng trong môi trường y tế thực tế.

## Tài liệu tham khảo

[1] A. Grover and J. Leskovec, “node2vec: Scalable Feature Learning for Networks”, KDD 2016. arXiv: https://arxiv.org/abs/1607.00653. PDF: https://cs.stanford.edu/~jure/pubs/node2vec-kdd16.pdf

[2] B. Perozzi, R. Al-Rfou and S. Skiena, “DeepWalk: Online Learning of Social Representations”, KDD 2014. arXiv: https://arxiv.org/abs/1403.6652

[3] T. Mikolov, I. Sutskever, K. Chen, G. Corrado and J. Dean, “Distributed Representations of Words and Phrases and their Compositionality”, NeurIPS 2013. https://papers.nips.cc/paper/5021-distributed-representations-of-words-and-phrases-and-their-compositionality

[4] SNAP Stanford, node2vec project page and reference implementation. https://snap.stanford.edu/node2vec/
