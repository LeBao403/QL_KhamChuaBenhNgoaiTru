from pathlib import Path
import random

random.seed(42)

base = Path(__file__).parent
out = base / 'training_seed_append.sql'

# =========================
# 1) DANH MỤC BỆNH MỞ RỘNG
# =========================
benh_nhom = [
    ('Hô hấp', [
        ('Cảm cúm', 'sốt, ho, sổ mũi, đau đầu', 'Nhiễm siêu vi đường hô hấp trên, điều trị triệu chứng', 3),
        ('Viêm họng cấp', 'đau họng, sốt nhẹ, nuốt đau, ho khan', 'Viêm niêm mạc họng cấp do virus hoặc vi khuẩn', 3),
        ('Viêm amidan', 'đau họng, sốt, khó nuốt, hôi miệng', 'Viêm cấp hoặc mạn tính của amidan khẩu cái', 3),
        ('Viêm phế quản cấp', 'ho đờm, khó thở nhẹ, sốt, đau ngực', 'Viêm đường thở dưới cấp tính', 3),
        ('Viêm mũi xoang', 'nghẹt mũi, chảy mũi, đau đầu, sốt', 'Viêm niêm mạc mũi xoang', 3),
        ('Viêm mũi dị ứng', 'hắt hơi, ngứa mũi, chảy mũi, nghẹt mũi', 'Phản ứng dị ứng ở niêm mạc mũi', 3),
        ('Hen phế quản', 'khò khè, khó thở, tức ngực, ho', 'Bệnh viêm mạn tính đường thở gây co thắt phế quản', 3),
        ('Sốt siêu vi', 'sốt, mệt, đau đầu, đau nhức người', 'Bệnh do nhiễm virus', 3),
        ('COVID-19', 'sốt, ho, đau họng, mất mùi vị', 'Bệnh nhiễm virus SARS-CoV-2', 3),
        ('Ho kéo dài', 'ho khan, ho đờm, mệt, đau ngực', 'Triệu chứng ho kéo dài cần đánh giá nguyên nhân', 2),
        ('Viêm thanh quản', 'khàn tiếng, ho, đau họng', 'Viêm thanh quản', 3),
        ('Viêm mũi cấp', 'nghẹt mũi, chảy mũi, hắt hơi', 'Viêm niêm mạc mũi cấp', 3),
        ('Viêm thanh khí phế quản', 'ho ông ổng, khàn tiếng, khó thở', 'Viêm đường hô hấp trên và dưới', 3),
        ('Co thắt phế quản', 'khò khè, tức ngực, khó thở', 'Co thắt đường thở', 3),
        ('Viêm phổi cộng đồng', 'sốt, ho, khó thở, đau ngực', 'Nhiễm trùng nhu mô phổi mắc phải cộng đồng', 3),
    ]),
    ('Tiêu hóa', [
        ('Viêm dạ dày', 'đau thượng vị, buồn nôn, ợ nóng, đầy bụng', 'Viêm niêm mạc dạ dày', 3),
        ('Trào ngược dạ dày thực quản', 'ợ nóng, ợ chua, đau ngực, khó chịu', 'Dịch vị trào ngược lên thực quản', 3),
        ('Rối loạn tiêu hóa', 'đau bụng, đầy hơi, tiêu chảy, buồn nôn', 'Rối loạn chức năng tiêu hóa thường gặp', 3),
        ('Tiêu chảy cấp', 'tiêu chảy, đau bụng, mất nước, mệt', 'Đi ngoài phân lỏng nhiều lần trong thời gian ngắn', 3),
        ('Táo bón', 'đi ngoài khó, đau bụng, bụng chướng', 'Giảm tần suất đi tiêu và phân khô cứng', 3),
        ('Bệnh trĩ', 'đau rát hậu môn, chảy máu khi đi ngoài', 'Giãn tĩnh mạch hậu môn trực tràng', 3),
        ('Đau bụng', 'đau bụng, buồn nôn, chướng bụng', 'Triệu chứng đau vùng bụng do nhiều nguyên nhân', 3),
        ('Viêm gan', 'mệt, vàng da, chán ăn, đau hạ sườn phải', 'Viêm tổn thương nhu mô gan', 3),
        ('Gan nhiễm mỡ', 'mệt, nặng tức hạ sườn phải', 'Tích tụ mỡ trong gan', 3),
        ('Đầy hơi', 'bụng chướng, ợ hơi, khó chịu, chán ăn', 'Triệu chứng rối loạn tiêu hóa thường gặp', 2),
        ('Viêm đại tràng', 'đau bụng, tiêu chảy, đầy hơi, mót rặn', 'Viêm niêm mạc đại tràng', 3),
        ('Hội chứng ruột kích thích', 'đau bụng, tiêu chảy, táo bón, đầy hơi', 'Rối loạn chức năng ruột mạn tính', 3),
        ('Viêm loét dạ dày tá tràng', 'đau thượng vị, ợ nóng, buồn nôn', 'Tổn thương loét niêm mạc dạ dày tá tràng', 3),
        ('Nôn ói', 'buồn nôn, nôn, mệt, chóng mặt', 'Triệu chứng tiêu hóa thường gặp', 2),
        ('Chán ăn', 'ăn kém, sụt cân, mệt, đầy bụng', 'Triệu chứng toàn thân/tiêu hóa', 2),
    ]),
    ('Nội tiết - chuyển hóa', [
        ('Tăng huyết áp', 'đau đầu, hoa mắt, chóng mặt, mệt', 'Bệnh tim mạch mạn tính', 3),
        ('Rối loạn lipid máu', 'mỡ máu cao, thừa cân, ít triệu chứng', 'Rối loạn chuyển hóa chất béo trong máu', 3),
        ('Đái tháo đường type 2', 'khát nước, tiểu nhiều, mệt, sút cân', 'Rối loạn chuyển hóa glucose kéo dài', 3),
        ('Tiền đái tháo đường', 'hơi khát nước, mệt, đường huyết tăng nhẹ', 'Trạng thái trước đái tháo đường', 2),
        ('Tăng acid uric máu', 'đau khớp, sưng khớp, đau ngón chân cái', 'Rối loạn chuyển hóa acid uric', 3),
        ('Béo phì', 'tăng cân, thừa mỡ, khó thở khi vận động', 'Thừa cân/béo phì', 3),
        ('Thừa cân', 'tăng cân, BMI cao, mệt khi vận động', 'Cân nặng cao hơn mức khuyến nghị', 3),
        ('Thiếu máu', 'mệt, hoa mắt, chóng mặt, da xanh', 'Giảm hemoglobin hoặc hồng cầu', 3),
        ('Rối loạn chuyển hóa', 'mệt, tăng cân, đường huyết bất thường', 'Nhóm rối loạn chuyển hóa chung', 3),
        ('Hạ đường huyết', 'run tay, vã mồ hôi, đói, mệt', 'Đường huyết giảm thấp hơn bình thường', 2),
        ('Rối loạn điện giải', 'mệt, co cơ, khát nước, chóng mặt', 'Mất cân bằng điện giải', 3),
        ('Tăng đường huyết', 'khát nước, tiểu nhiều, mệt', 'Đường huyết tăng cao hơn bình thường', 3),
        ('Rối loạn tuyến giáp', 'mệt, sụt cân, hồi hộp, run tay', 'Rối loạn chức năng tuyến giáp', 3),
        ('Rối loạn mỡ máu', 'ít triệu chứng, mỡ máu cao', 'Rối loạn lipid máu', 3),
        ('Rối loạn chuyển hóa acid uric', 'đau khớp, sưng khớp, mệt', 'Rối loạn chuyển hóa acid uric', 3),
    ]),
    ('Cơ xương khớp', [
        ('Đau cơ xương khớp', 'đau cơ, đau khớp, cứng khớp, đau khi vận động', 'Nhóm bệnh lý gây đau và hạn chế vận động', 3),
        ('Đau lưng dưới', 'đau thắt lưng, đau lan mông, cứng lưng', 'Đau vùng thắt lưng', 3),
        ('Viêm khớp', 'đau khớp, sưng khớp, cứng khớp, hạn chế vận động', 'Tình trạng viêm một hoặc nhiều khớp', 3),
        ('Thoái hóa khớp gối', 'đau gối, cứng khớp, khó đi lại', 'Thoái hóa sụn và cấu trúc khớp gối', 3),
        ('Chấn thương phần mềm', 'sưng đau, bầm tím, đau khi vận động', 'Tổn thương cơ, gân, dây chằng', 3),
        ('Trật khớp', 'đau dữ dội, biến dạng khớp, hạn chế vận động', 'Khớp lệch khỏi vị trí bình thường', 3),
        ('Đau vai gáy', 'đau cổ vai gáy, cứng cổ, tê tay', 'Đau vùng cổ vai gáy do nhiều nguyên nhân', 3),
        ('Đau cổ', 'đau cổ, cứng cổ, hạn chế xoay cổ', 'Đau vùng cổ thường gặp', 3),
        ('Viêm gân', 'đau tại gân, sưng nhẹ, đau khi vận động', 'Viêm gân do quá tải hoặc chấn thương', 3),
        ('Loãng xương', 'đau lưng, giảm chiều cao, dễ gãy xương', 'Giảm mật độ xương', 3),
        ('Bong gân', 'sưng đau, hạn chế vận động, bầm tím', 'Tổn thương dây chằng do chấn thương', 3),
        ('Đau khớp gối', 'đau gối, cứng khớp, đi lại khó', 'Triệu chứng/ bệnh lý khớp gối', 3),
        ('Đau vai', 'đau vai, hạn chế nâng tay, cứng khớp', 'Đau vùng vai', 3),
        ('Đau lưng trên', 'đau lưng, căng cơ, mỏi vai', 'Đau vùng lưng trên', 3),
        ('Viêm bao hoạt dịch', 'đau khớp, sưng nóng, hạn chế vận động', 'Viêm túi hoạt dịch quanh khớp', 3),
    ]),
    ('Da liễu', [
        ('Dị ứng', 'ngứa, nổi mề đay, hắt hơi, chảy nước mắt', 'Phản ứng quá mẫn của cơ thể với dị nguyên', 3),
        ('Viêm da dị ứng', 'ngứa da, mẩn đỏ, khô da, rát da', 'Bệnh viêm da mạn tính liên quan cơ địa dị ứng', 3),
        ('Chàm da', 'ngứa, da khô, mẩn đỏ, bong vảy', 'Tổn thương da mạn tính gây ngứa và viêm', 3),
        ('Nhiễm trùng da', 'đỏ da, sưng, đau, mủ, sốt', 'Nhiễm khuẩn tại da và mô dưới da', 3),
        ('Mẩn ngứa', 'ngứa, nổi ban, đỏ da', 'Triệu chứng da thường gặp do dị ứng hoặc kích ứng', 2),
        ('Mề đay', 'nổi mảng phù, ngứa, đỏ da', 'Biểu hiện dị ứng da cấp tính', 3),
        ('Nấm da', 'ngứa, bong vảy, tổn thương vòng', 'Nhiễm nấm ở da', 3),
        ('Nấm kẽ', 'ngứa, hăm kẽ, đỏ da', 'Nhiễm nấm tại vùng kẽ cơ thể', 3),
        ('Viêm da tiếp xúc', 'đỏ da, rát, ngứa, phồng rộp', 'Phản ứng viêm da do tiếp xúc dị nguyên', 3),
        ('Rôm sảy', 'ngứa, nổi mẩn nhỏ, rát da', 'Tắc tuyến mồ hôi gây mẩn ngứa', 2),
        ('Mụn trứng cá', 'mụn viêm, mụn đầu đen, da dầu', 'Bệnh da liễu thường gặp', 3),
        ('Khô da', 'da khô, nứt nẻ, ngứa', 'Tình trạng da thiếu ẩm', 2),
        ('Nấm móng', 'móng dày, đổi màu, giòn móng', 'Nhiễm nấm móng', 3),
        ('Viêm nang lông', 'mụn mủ nhỏ, ngứa, đỏ da', 'Viêm nang lông do vi khuẩn hoặc nấm', 3),
        ('Vảy nến', 'mảng đỏ, vảy trắng, ngứa', 'Bệnh da mạn tính', 3),
    ]),
    ('Tiết niệu', [
        ('Nhiễm khuẩn tiết niệu', 'tiểu buốt, tiểu rắt, đau bụng dưới, sốt', 'Nhiễm trùng hệ tiết niệu', 3),
        ('Viêm bàng quang', 'tiểu buốt, tiểu rắt, đau hạ vị, sốt nhẹ', 'Viêm nhiễm bàng quang', 3),
        ('Viêm thận - bể thận', 'sốt cao, đau hông lưng, tiểu buốt', 'Nhiễm trùng đường tiết niệu lên thận', 3),
        ('Tiểu buốt', 'tiểu đau, tiểu rắt, khó chịu khi đi tiểu', 'Triệu chứng đường tiết niệu', 2),
        ('Tiểu rắt', 'đi tiểu nhiều lần, lượng ít, khó chịu', 'Triệu chứng đường tiết niệu', 2),
        ('Tiểu đêm', 'đi tiểu nhiều về đêm, mất ngủ, mệt', 'Triệu chứng thường gặp ở người lớn tuổi', 2),
        ('Sỏi tiết niệu', 'đau quặn thận, tiểu buốt, tiểu máu', 'Sỏi đường tiết niệu', 3),
        ('Suy thận mạn', 'mệt, phù, tiểu ít, chán ăn', 'Suy giảm chức năng thận kéo dài', 3),
        ('Đái máu', 'nước tiểu đỏ, đau bụng dưới, tiểu buốt', 'Triệu chứng bất thường đường tiết niệu', 2),
        ('Phù', 'sưng chân, mặt, tăng cân nhanh', 'Giữ nước do nhiều nguyên nhân', 2),
        ('Tiểu khó', 'tiểu khó, dòng tiểu yếu, đau rát', 'Rối loạn tiểu tiện', 2),
        ('Viêm niệu đạo', 'tiểu buốt, tiểu rắt, ngứa niệu đạo', 'Viêm đường niệu đạo', 3),
        ('Tăng tiểu đêm', 'đi tiểu đêm nhiều lần, mệt', 'Triệu chứng tiết niệu', 2),
        ('Suy thận cấp', 'tiểu ít, phù, mệt, buồn nôn', 'Suy giảm chức năng thận cấp tính', 3),
        ('Đau hông lưng', 'đau hông lưng, sốt, tiểu buốt', 'Triệu chứng có thể liên quan tiết niệu', 2),
    ]),
    ('Tai mắt mũi họng', [
        ('Viêm kết mạc', 'đỏ mắt, ngứa mắt, chảy ghèn', 'Viêm kết mạc mắt', 3),
        ('Viêm tai giữa', 'đau tai, sốt, ù tai, nghe kém', 'Viêm tai giữa', 3),
        ('Viêm lợi', 'đỏ lợi, sưng lợi, chảy máu lợi', 'Viêm mô nướu', 3),
        ('Đau răng', 'đau răng, ê buốt, sưng nướu', 'Đau liên quan răng và mô quanh răng', 3),
        ('Sâu răng', 'đau răng, ê buốt, lỗ sâu răng', 'Tổn thương cấu trúc răng do vi khuẩn', 3),
        ('Áp xe răng', 'đau răng, sưng nướu, mủ, sốt', 'Nhiễm trùng quanh răng', 3),
        ('Ù tai', 'ù tai, nghe kém, chóng mặt', 'Triệu chứng tai mũi họng', 2),
        ('Khàn tiếng', 'nói khó, giọng khàn, đau họng', 'Triệu chứng thanh quản', 2),
        ('Chảy máu chân răng', 'chảy máu lợi, đau nướu', 'Biểu hiện bệnh lý răng lợi', 2),
        ('Viêm xoang mạn', 'nghẹt mũi, đau đầu, chảy mũi', 'Viêm xoang kéo dài', 3),
        ('Viêm mũi họng', 'sổ mũi, đau họng, ho', 'Viêm vùng hầu họng', 3),
        ('Viêm thanh quản cấp', 'khàn tiếng, ho, đau họng', 'Viêm thanh quản cấp', 3),
        ('Viêm tai ngoài', 'đau tai, ngứa tai, chảy dịch', 'Viêm ống tai ngoài', 3),
        ('Viêm nha chu', 'chảy máu lợi, hôi miệng, lung lay răng', 'Bệnh nha chu', 3),
        ('Khô mắt', 'cộm mắt, khô mắt, mỏi mắt', 'Triệu chứng mắt', 2),
    ]),
    ('Thần kinh - tâm lý', [
        ('Đau đầu căng thẳng', 'đau đầu, căng cổ, mệt, khó tập trung', 'Đau đầu do căng thẳng', 3),
        ('Đau nửa đầu', 'đau đầu một bên, buồn nôn, sợ ánh sáng', 'Dạng đau đầu theo cơn', 3),
        ('Rối loạn lo âu', 'lo lắng, hồi hộp, mất ngủ, căng thẳng', 'Rối loạn lo âu', 3),
        ('Trầm cảm nhẹ', 'buồn bã, mệt mỏi, mất hứng thú', 'Rối loạn khí sắc nhẹ', 3),
        ('Rối loạn giấc ngủ', 'khó ngủ, ngủ không sâu, thức giấc sớm', 'Nhóm rối loạn giấc ngủ', 3),
        ('Căng thẳng', 'mệt, lo âu, mất ngủ, đau đầu', 'Phản ứng stress kéo dài', 2),
        ('Chóng mặt', 'choáng váng, buồn nôn, mất thăng bằng', 'Triệu chứng thần kinh thường gặp', 2),
        ('Run tay', 'run tay, hồi hộp, vã mồ hôi', 'Triệu chứng thần kinh hoặc chuyển hóa', 2),
        ('Rối loạn tập trung', 'khó tập trung, hay quên, mệt', 'Triệu chứng chức năng thần kinh', 2),
        ('Mệt mỏi', 'mệt, uể oải, giảm tập trung', 'Triệu chứng toàn thân', 2),
        ('Mất ngủ mạn', 'khó ngủ kéo dài, thức giấc sớm', 'Rối loạn giấc ngủ mạn tính', 3),
        ('Stress', 'căng thẳng, mệt, lo âu', 'Phản ứng stress', 2),
        ('Rối loạn cảm xúc', 'buồn bã, lo lắng, mất ngủ', 'Rối loạn khí sắc', 3),
        ('Đau đầu migraine', 'đau đầu một bên, buồn nôn, sợ ánh sáng', 'Migraine', 3),
        ('Hội chứng mệt mỏi', 'mệt kéo dài, giảm tập trung', 'Tình trạng mệt mỏi kéo dài', 2),
    ]),
]

benh_list = []
for _, items in benh_nhom:
    benh_list.extend(items)

# Tạo biến thể để đủ 500 bệnh
variants = ['cấp', 'mạn', 'tái phát', 'nhẹ', 'vừa', 'nặng', 'không đặc hiệu', 'chưa rõ nguyên nhân']
while len(benh_list) < 500:
    base_name, base_sym, base_desc, stage = random.choice(benh_list)
    suffix = random.choice(variants)
    new_name = f'{base_name} {suffix}'
    if any(x[0] == new_name for x in benh_list):
        continue
    benh_list.append((new_name, base_sym, f'{base_desc} - Biến thể {suffix}.', stage))

# =========================
# 2) DANH MỤC THUỐC/HOẠT CHẤT
# =========================
thuoc_base = [
    ('Giảm đau hạ sốt', 'Paracetamol', '500mg', 'Viên', 'Uống', 12000, 1),
    ('Kháng sinh', 'Amoxicillin', '500mg', 'Viên', 'Uống', 45000, 1),
    ('Kháng sinh', 'Cefixime', '200mg', 'Viên', 'Uống', 78000, 1),
    ('Kháng sinh', 'Azithromycin', '500mg', 'Viên', 'Uống', 65000, 1),
    ('Kháng histamin', 'Cetirizine', '10mg', 'Viên', 'Uống', 25000, 1),
    ('Kháng histamin', 'Loratadine', '10mg', 'Viên', 'Uống', 22000, 1),
    ('Dạ dày - tiêu hóa', 'Omeprazole', '20mg', 'Viên', 'Uống', 30000, 1),
    ('Dạ dày - tiêu hóa', 'Pantoprazole', '40mg', 'Viên', 'Uống', 42000, 1),
    ('Nội tiết - chuyển hóa', 'Metformin', '500mg', 'Viên', 'Uống', 38000, 1),
    ('Nội tiết - chuyển hóa', 'Gliclazide', '80mg', 'Viên', 'Uống', 50000, 1),
    ('Tim mạch', 'Amlodipine', '5mg', 'Viên', 'Uống', 27000, 1),
    ('Tim mạch', 'Losartan', '50mg', 'Viên', 'Uống', 33000, 1),
    ('Cơ xương khớp', 'Ibuprofen', '400mg', 'Viên', 'Uống', 26000, 1),
    ('Cơ xương khớp', 'Diclofenac', '50mg', 'Viên', 'Uống', 29000, 1),
    ('Hô hấp', 'Salbutamol', '2mg', 'Viên', 'Uống', 24000, 1),
    ('Hô hấp', 'Bromhexine', '8mg', 'Viên', 'Uống', 21000, 1),
    ('Hô hấp', 'Acetylcysteine', '200mg', 'Gói', 'Uống', 35000, 1),
    ('Bù nước - điện giải', 'ORS', '1 gói', 'Gói', 'Uống', 18000, 1),
    ('Thần kinh - tâm lý', 'Melatonin', '3mg', 'Viên', 'Uống', 58000, 0),
    ('Da liễu', 'Clotrimazole', '1%', 'Tuýp', 'Bôi ngoài da', 31000, 0),
    ('Hô hấp', 'Dextromethorphan', '15mg', 'Viên', 'Uống', 28000, 1),
    ('Dạ dày - tiêu hóa', 'Domperidone', '10mg', 'Viên', 'Uống', 26000, 1),
    ('Tim mạch', 'Captopril', '25mg', 'Viên', 'Uống', 23000, 1),
    ('Nội tiết - chuyển hóa', 'Insulin', '100IU/ml', 'Ống', 'Tiêm', 125000, 1),
    ('Kháng sinh', 'Levofloxacin', '500mg', 'Viên', 'Uống', 92000, 1),
    ('Kháng sinh', 'Ciprofloxacin', '500mg', 'Viên', 'Uống', 64000, 1),
    ('Kháng histamin', 'Fexofenadine', '120mg', 'Viên', 'Uống', 39000, 1),
    ('Dạ dày - tiêu hóa', 'Esomeprazole', '20mg', 'Viên', 'Uống', 55000, 1),
    ('Hô hấp', 'Budesonide', '200mcg', 'Ống', 'Khí dung', 86000, 1),
    ('Cơ xương khớp', 'Naproxen', '250mg', 'Viên', 'Uống', 34000, 1),
]

unit_pool = ['Viên', 'Gói', 'Chai', 'Lọ', 'Tuýp', 'Ống']
route_pool = ['Uống', 'Bôi ngoài da', 'Ngậm', 'Khí dung', 'Xịt mũi', 'Tiêm']
expanded_drugs = []
for i in range(300):
    grp, act, strength, unit, route, price, bhyt = thuoc_base[i % len(thuoc_base)]
    v = i // len(thuoc_base)
    drug_name = f'{act} {strength}' if v == 0 else f'{act} {strength} {v + 1}'
    expanded_drugs.append({
        'drug_code': f'T{i+1:03d}',
        'drug_name': drug_name,
        'drug_group': grp,
        'route': route if v == 0 else random.choice(route_pool),
        'drug_family': act,
        'unit': unit if v == 0 else random.choice(unit_pool),
        'price': price + v * 1500,
        'bhyt': bhyt,
    })

GROUP_MAP = {
    'Hô hấp': ['Hô hấp', 'Giảm đau hạ sốt', 'Kháng sinh', 'Kháng histamin', 'Bù nước - điện giải'],
    'Tiêu hóa': ['Dạ dày - tiêu hóa', 'Bù nước - điện giải', 'Giảm đau hạ sốt'],
    'Nội tiết - chuyển hóa': ['Nội tiết - chuyển hóa', 'Tim mạch'],
    'Cơ xương khớp': ['Cơ xương khớp', 'Giảm đau hạ sốt'],
    'Da liễu': ['Kháng histamin', 'Dạ dày - tiêu hóa'],
    'Tiết niệu': ['Kháng sinh', 'Bù nước - điện giải'],
    'Tai mắt mũi họng': ['Kháng sinh', 'Kháng histamin', 'Giảm đau hạ sốt'],
    'Thần kinh - tâm lý': ['Thần kinh - tâm lý', 'Giảm đau hạ sốt'],
}

# =========================
# 3) CHIẾN LƯỢC TẠO DỮ LIỆU TRAIN
# =========================
sexes = ['Nam', 'Nữ']
age_groups = [(0, 12, 'TreEm'), (13, 17, 'ThieuNien'), (18, 39, 'NguoiLonTre'), (40, 59, 'TrungNien'), (60, 90, 'NguoiCaoTuoi')]
allergy_flags = ['None', 'Penicillin', 'NSAID', 'Sulfa']
comorbids = ['None', 'Tăng huyết áp', 'Tiểu đường', 'Suy gan', 'Suy thận', 'Viêm loét dạ dày']

def pick_age_group(age):
    for mn, mx, label in age_groups:
        if mn <= age <= mx:
            return label
    return 'NguoiLonTre'

rows = []
for i in range(500_000):
    disease_name, symptoms, desc, stage = random.choice(benh_list)
    disease_group = next((g for g, items in benh_nhom if any(x[0] == disease_name for x in items)), 'Hô hấp')
    age = random.randint(1, 90)
    sex = random.choice(sexes)
    age_group = pick_age_group(age)
    allergy = random.choices(allergy_flags, weights=[0.75, 0.12, 0.08, 0.05], k=1)[0]
    comorbid = random.choices(comorbids, weights=[0.55, 0.15, 0.15, 0.05, 0.05, 0.05], k=1)[0]

    candidate_groups = GROUP_MAP.get(disease_group, ['Giảm đau hạ sốt'])
    candidate_drugs = [x for x in expanded_drugs if x['drug_group'] in candidate_groups]
    if not candidate_drugs:
        candidate_drugs = expanded_drugs

    if random.random() < 0.73:
        drug = random.choice(candidate_drugs)
    else:
        drug = random.choice(expanded_drugs)

    contraindication = 0
    if disease_group == 'Tiêu hóa' and drug['drug_family'] in ['Ibuprofen', 'Diclofenac', 'Naproxen']:
        contraindication = 1
    if allergy == 'Penicillin' and drug['drug_family'] == 'Amoxicillin':
        contraindication = 1
    if comorbid == 'Suy gan' and drug['drug_family'] in ['Paracetamol', 'Ibuprofen', 'Diclofenac', 'Naproxen']:
        contraindication = 1
    if comorbid == 'Tiểu đường' and drug['drug_family'] in ['Gliclazide'] and age < 18:
        contraindication = 1

    label = 1 if (drug['drug_group'] in candidate_groups and contraindication == 0) else 0

    # nhiễu có kiểm soát
    if label == 1 and random.random() < 0.10:
        label = 0
    elif label == 0 and random.random() < 0.02 and contraindication == 0:
        label = 1

    interaction_risk = 1 if (comorbid == 'Suy gan' and drug['drug_family'] in ['Paracetamol', 'Ibuprofen', 'Diclofenac', 'Naproxen']) else 0
    in_stock = 1 if random.random() > 0.03 else 0

    rows.append({
        'record_id': i + 1,
        'age': age,
        'sex': sex,
        'age_group': age_group,
        'disease': disease_group,
        'disease_name': disease_name,
        'symptoms': symptoms,
        'comorbidity': comorbid,
        'allergy': allergy,
        'drug_code': drug['drug_code'],
        'drug_name': drug['drug_name'],
        'drug_group': drug['drug_group'],
        'route': drug['route'],
        'drug_family': drug['drug_family'],
        'in_stock': in_stock,
        'contraindication': contraindication,
        'interaction_risk': interaction_risk,
        'label': label,
    })

# =========================
# 4) GHI FILE SQL
# =========================
lines = []
lines.append('-- Auto-generated training seed append')
lines.append('DELETE FROM DANHMUC_BENH;')
lines.append('DELETE FROM DANHMUC_THUOC;')
lines.append('DELETE FROM DANHMUC_HOATCHAT;')
lines.append('DELETE FROM THUOC;')
lines.append('DELETE FROM THANHPHAN_THUOC;')
lines.append('GO\n')

lines.append('INSERT INTO DANHMUC_BENH (MaBenh, TenBenh, TrieuChung, MoTa, SoGiaiDoan) VALUES')
for idx, (name, sym, desc, stage) in enumerate(benh_list[:500], start=1):
    comma = ',' if idx < 500 else ';'
    lines.append(f"('BENH{idx:03d}', N'{name}', N'{sym}', N'{desc}', {stage}){comma}")
lines.append('GO\n')

# map nhóm thuốc / hoạt chất
cat_map = {}
hc_names = []
for d in expanded_drugs:
    cat_map[d['drug_group']] = d['drug_group']
    if d['drug_family'] not in hc_names:
        hc_names.append(d['drug_family'])

lines.append('INSERT INTO DANHMUC_THUOC (MaDanhMuc, TenDanhMuc, MoTa) VALUES')
cat_codes = {}
for i, grp in enumerate(sorted(cat_map.keys()), start=1):
    code = f'DM{i:03d}'
    cat_codes[grp] = code
    comma = ',' if i < len(cat_map) else ';'
    lines.append(f"('{code}', N'{grp}', N'Nhóm thuốc {grp}'){comma}")
lines.append('GO\n')

lines.append('INSERT INTO DANHMUC_HOATCHAT (MaHoatChat, TenHoatChat, MoTa) VALUES')
hc_codes = {}
for i, act in enumerate(hc_names, start=1):
    code = f'HC{i:03d}'
    hc_codes[act] = code
    comma = ',' if i < len(hc_names) else ';'
    lines.append(f"('{code}', N'{act}', N'Hoạt chất {act}'){comma}")
lines.append('GO\n')

lines.append('INSERT INTO THUOC (MaThuoc, TenThuoc, QuyCach, DonViCoBan, MaLoaiThuoc, DuongDung, GiaBan, CoBHYT, MaNSX, TrangThai) VALUES')
for i, d in enumerate(expanded_drugs, start=1):
    comma = ',' if i < len(expanded_drugs) else ';'
    lines.append(f"('{d['drug_code']}', N'{d['drug_name']}', N'Hộp 10 vỉ x 10 viên', N'{d['unit']}', '{cat_codes[d['drug_group']]}', N'{d['route']}', {d['price']}, {d['bhyt']}, NULL, 1){comma}")
lines.append('GO\n')

lines.append('INSERT INTO THANHPHAN_THUOC (MaThanhPhan, MaThuoc, MaHoatChat, HamLuong) VALUES')
for i, d in enumerate(expanded_drugs, start=1):
    comma = ',' if i < len(expanded_drugs) else ';'
    lines.append(f"('TP{i:03d}', '{d['drug_code']}', '{hc_codes[d['drug_family']]}', N'{d['drug_name'].split()[-1] if d['drug_name'].split()[-1].endswith(('mg','g','mcg')) else '1 đơn vị'}'){comma}")
lines.append('GO\n')

out.write_text('\n'.join(lines), encoding='utf-8')
print(f'Wrote {out}')
