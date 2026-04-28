/** DASHBOARD MANAGER - TRÌNH ĐIỀU KHIỂN TRUNG TÂM */

//$(document).ready(function () {
//    loadDashboard('tong-quan');

//    $('#dashboardType').change(function () {
//        loadDashboard($(this).val());
//    });
//});

$(document).ready(function () {
    // Đọc URL xem đang ở Tab nào (VD: .../Dashboard#doanh-thu)
    let hash = window.location.hash.replace('#', '');
    let activeTab = hash ? hash : 'tong-quan'; // Mặc định là tổng quan

    // Gán dropdown và load đúng tab
    $('#dashboardType').val(activeTab);
    loadDashboard(activeTab);

    // Bắt sự kiện khi đổi Dropdown
    $('#dashboardType').change(function () {
        let type = $(this).val();
        window.location.hash = type; // Lưu lại lịch sử lên URL
        loadDashboard(type);
    });
});

function loadDashboard(type) {
    let urlAction = '';
    if (type === 'tong-quan') urlAction = '/Admin/Dashboard/LoadDashboardTongQuan';
    else if (type === 'doanh-thu') urlAction = '/Admin/Dashboard/LoadDashboardDoanhThu';
    else if (type === 'benh-nhan') urlAction = '/Admin/Dashboard/LoadDashboardBenhNhan';
    else if (type === 'kho-duoc') urlAction = '/Admin/Dashboard/LoadDashboardKhoDuoc';
    else {
        $('#dashboard-content').html(`
            <div class="text-center py-5 mt-5">
                <i class="bi bi-tools text-muted mb-3" style="font-size: 4rem;"></i>
                <h4 class="text-muted fw-bold">Tính năng đang phát triển</h4>
            </div>
        `);
        return;
    }

    $('#dashboard-loader').show();
    $('#dashboard-content').hide();

    $.ajax({
        url: urlAction, type: 'GET',
        success: function (res) {
            $('#dashboard-loader').hide();
            $('#dashboard-content').html(res).fadeIn(300);
        },
        error: function () {
            $('#dashboard-loader').hide();
            $('#dashboard-content').html('<div class="alert alert-danger">Lỗi kết nối máy chủ!</div>').show();
        }
    });
}

const formatVND = num => new Intl.NumberFormat('vi-VN').format(num);

// =========================================================================
// PHẦN BIỂU ĐỒ DOANH THU (TRẢ LẠI MÀU XANH DƯƠNG VÀ FIX PIE CHART)
// =========================================================================
let cXuHuong, cPTTT, cNguonThu, cTopDV;

function renderDoanhThuCharts(xuHuong, pttt, nguonThu, topDV) {
    // 1. Line Chart Xu Hướng (MÀU XANH DƯƠNG MỀM MẠI)
    let ctx = document.getElementById('chartXuHuong');
    if (!ctx) return;

    let gradient = ctx.getContext('2d').createLinearGradient(0, 0, 0, 400);
    gradient.addColorStop(0, 'rgba(59, 130, 246, 0.4)'); // Xanh dương mờ
    gradient.addColorStop(1, 'rgba(59, 130, 246, 0.0)');

    if (cXuHuong) cXuHuong.destroy();
    cXuHuong = new Chart(ctx, {
        type: 'line',
        data: {
            labels: xuHuong.map(x => x.Ngay),
            datasets: [{
                label: 'Thực thu', data: xuHuong.map(x => x.SoTien),
                borderColor: '#3b82f6', backgroundColor: gradient, // Cấu hình màu xanh
                borderWidth: 3,
                tension: 0.4, // Làm cong
                fill: true,
                pointRadius: 0, pointHoverRadius: 6, pointBackgroundColor: '#fff', pointBorderColor: '#3b82f6', pointBorderWidth: 2
            }]
        },
        options: {
            responsive: true, maintainAspectRatio: false, plugins: { legend: { display: false } },
            interaction: { mode: 'index', intersect: false },
            scales: {
                x: { grid: { display: false }, ticks: { color: '#64748b', font: { weight: 'bold' } } },
                y: {
                    beginAtZero: true, grid: { borderDash: [5, 5], color: '#e2e8f0' }, border: { display: false },
                    ticks: { color: '#64748b', callback: value => (value >= 1000000 ? (value / 1000000).toFixed(0) + 'M' : (value / 1000).toFixed(0) + 'K') }
                }
            }
        }
    });

    // 2. Chart PTTT 
    if (cPTTT) cPTTT.destroy();
    cPTTT = new Chart(document.getElementById('chartPTTT'), {
        type: 'doughnut',
        data: { labels: pttt.map(x => x.Ten), datasets: [{ data: pttt.map(x => x.SoTien), backgroundColor: ['#10b981', '#f59e0b', '#3b82f6', '#ef4444'], borderWidth: 0 }] },
        options: { responsive: true, maintainAspectRatio: false, cutout: '75%' }
    });

    // 3. Chart Nguồn thu (ÉP HIỂN THỊ CÁC KHOẢN SIÊU NHỎ)
    if (cNguonThu) cNguonThu.destroy();

    // Bước 1: Tính toán ép tỷ lệ ảo để vẽ
    let rawDataNguonThu = nguonThu.map(x => x.SoTien);
    let totalNguonThu = rawDataNguonThu.reduce((a, b) => a + b, 0);

    // Đặt mức tối thiểu là 1% tổng doanh thu để mắt người nhìn thấy được
    let minVisualValue = totalNguonThu * 0.01;

    // Mảng dữ liệu ảo: Nếu tiền > 0 mà bé hơn 1% -> Ép lên 1% để vẽ
    let visualDataNguonThu = rawDataNguonThu.map(v => (v > 0 && v < minVisualValue) ? minVisualValue : v);

    cNguonThu = new Chart(document.getElementById('chartNguonThu'), {
        type: 'pie',
        data: {
            labels: nguonThu.map(x => x.Ten),
            datasets: [{
                data: visualDataNguonThu,  // Dùng data ẢO để vẽ hình
                realData: rawDataNguonThu, // Giữ data THẬT ở backup để hiện chữ
                backgroundColor: [
                    '#3b82f6', '#ec4899', '#10b981', '#f59e0b',
                    '#8b5cf6', '#06b6d4', '#f43f5e', '#64748b'
                ],
                borderWidth: 1.5,
                borderColor: '#ffffff' // Viền trắng để tách các miếng nhỏ cho rõ
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                tooltip: {
                    callbacks: {
                        label: function (context) {
                            // Khi hover chuột, lấy data THẬT ra để hiển thị
                            let realValue = context.dataset.realData[context.dataIndex];
                            return ' ' + context.label + ': ' + formatVND(realValue) + ' đ';
                        }
                    }
                }
            }
        }
    });

    // 4. Chart Top Dịch vụ
    if (cTopDV) cTopDV.destroy();
    cTopDV = new Chart(document.getElementById('chartTopDV'), {
        type: 'bar',
        data: { labels: topDV.map(x => x.TenDV), datasets: [{ data: topDV.map(x => x.SoTien), backgroundColor: '#f43f5e', borderRadius: 5 }] },
        options: { indexAxis: 'y', responsive: true, maintainAspectRatio: false, plugins: { legend: { display: false } }, scales: { x: { grid: { display: false } }, y: { grid: { display: false } } } }
    });
}

let fpTuNgay, fpDenNgay;
function initDoanhThuFilter() {
    let commonConfig = { dateFormat: "Y-m-d", locale: "vn", altInput: true, altFormat: "d/m/Y", monthSelectorType: "dropdown", onChange: () => executeDoanhThuFilter() };
    fpTuNgay = flatpickr("#dtTuNgay", { ...commonConfig, defaultDate: $('#hdnTuNgay').val() });
    fpDenNgay = flatpickr("#dtDenNgay", { ...commonConfig, defaultDate: $('#hdnDenNgay').val() });
    $('#cbGroupBy').change(() => executeDoanhThuFilter());
}

function executeDoanhThuFilter() {
    let tuNgay = $('#dtTuNgay').val(); let denNgay = $('#dtDenNgay').val(); let groupBy = $('#cbGroupBy').val();
    if (!tuNgay || !denNgay || (new Date(tuNgay) > new Date(denNgay))) return;

    $('#dashboard-content').css('opacity', '0.5');
    $.ajax({
        url: '/Admin/Dashboard/FilterDoanhThu', type: 'POST', data: { tuNgay: tuNgay, denNgay: denNgay, groupBy: groupBy },
        success: function (res) {
            if (res.success) {
                let d = res.data;
                $('#kpiTongGoc').html(formatVND(d.TongGoc) + ' <small>đ</small>');
                $('#kpiThucThu').html(formatVND(d.ThucThu) + ' <small>đ</small>');
                $('#kpiBHYT').html(formatVND(d.BHYT) + ' <small>đ</small>');
                $('#kpiTrungBinh').html(formatVND(d.TrungBinhLuot) + ' <small>đ</small>');
                $('#kpiSoLuot').text('Dựa trên ' + d.SoLuotThanhToan + ' lượt');
                renderDoanhThuCharts(d.BieuDoXuHuong, d.BieuDoPhuongThuc, d.BieuDoNguonThu, d.TopDichVu);
            }
        },
        complete: () => $('#dashboard-content').css('opacity', '1')
    });
}


// =========================================================================
// PHẦN 2: CODE DÀNH CHO DASHBOARD BỆNH NHÂN
// =========================================================================
let bnChartLuuLuong, bnChartGioiTinh, bnChartTuyenKham, bnChartDoTuoi, bnChartBenhLy;

function renderBenhNhanCharts(luuLuong, gioiTinh, tuyenKham, doTuoi, benhLy) {
    let ctxLuuLuong = document.getElementById('chartLuuLuong');
    if (!ctxLuuLuong) return;

    let gradient = ctxLuuLuong.getContext('2d').createLinearGradient(0, 0, 0, 300);
    gradient.addColorStop(0, 'rgba(14, 165, 233, 0.5)'); // Cyan mờ
    gradient.addColorStop(1, 'rgba(14, 165, 233, 0.0)');

    if (bnChartLuuLuong) bnChartLuuLuong.destroy();
    bnChartLuuLuong = new Chart(ctxLuuLuong, {
        type: 'line',
        data: {
            labels: luuLuong.map(x => x.Nhan),
            datasets: [{
                label: 'Lượt khám', data: luuLuong.map(x => x.GiaTri),
                borderColor: '#0ea5e9', backgroundColor: gradient,
                borderWidth: 3, tension: 0.4, fill: true,
                pointRadius: 0, pointHoverRadius: 6, pointBackgroundColor: '#fff', pointBorderColor: '#0ea5e9'
            }]
        },
        options: { responsive: true, maintainAspectRatio: false, plugins: { legend: { display: false } }, interaction: { mode: 'index', intersect: false }, scales: { y: { beginAtZero: true, grid: { borderDash: [5, 5] } }, x: { grid: { display: false } } } }
    });

    if (bnChartGioiTinh) bnChartGioiTinh.destroy();
    bnChartGioiTinh = new Chart(document.getElementById('chartGioiTinh'), {
        type: 'pie',
        data: { labels: gioiTinh.map(x => x.Nhan), datasets: [{ data: gioiTinh.map(x => x.GiaTri), backgroundColor: ['#3b82f6', '#ec4899', '#cbd5e1'], borderWidth: 0 }] },
        options: { responsive: true, maintainAspectRatio: false }
    });

    if (bnChartTuyenKham) bnChartTuyenKham.destroy();
    bnChartTuyenKham = new Chart(document.getElementById('chartTuyenKham'), {
        type: 'doughnut',
        data: { labels: tuyenKham.map(x => x.Nhan), datasets: [{ data: tuyenKham.map(x => x.GiaTri), backgroundColor: ['#10b981', '#f43f5e'], borderWidth: 0 }] },
        options: { responsive: true, maintainAspectRatio: false, cutout: '70%' }
    });

    if (bnChartDoTuoi) bnChartDoTuoi.destroy();
    bnChartDoTuoi = new Chart(document.getElementById('chartDoTuoi'), {
        type: 'bar',
        data: { labels: doTuoi.map(x => x.Nhan), datasets: [{ label: 'Số lượng', data: doTuoi.map(x => x.GiaTri), backgroundColor: '#8b5cf6', borderRadius: 6 }] },
        options: { responsive: true, maintainAspectRatio: false, plugins: { legend: { display: false } }, scales: { x: { grid: { display: false } }, y: { beginAtZero: true } } }
    });

    if (bnChartBenhLy) bnChartBenhLy.destroy();
    bnChartBenhLy = new Chart(document.getElementById('chartBenhLy'), {
        type: 'bar',
        data: { labels: benhLy.map(x => x.Nhan), datasets: [{ label: 'Số ca mắc', data: benhLy.map(x => x.GiaTri), backgroundColor: '#f59e0b', borderRadius: 4 }] },
        options: { indexAxis: 'y', responsive: true, maintainAspectRatio: false, plugins: { legend: { display: false } }, scales: { x: { grid: { display: false } }, y: { grid: { display: false } } } }
    });
}

let fpBnTuNgay, fpBnDenNgay;
function initBenhNhanFilter() {
    let cfg = { dateFormat: "Y-m-d", locale: "vn", altInput: true, altFormat: "d/m/Y", monthSelectorType: "dropdown", onChange: () => executeBenhNhanFilter() };
    fpBnTuNgay = flatpickr("#bnTuNgay", { ...cfg, defaultDate: $('#bnHdnTuNgay').val() });
    fpBnDenNgay = flatpickr("#bnDenNgay", { ...cfg, defaultDate: $('#bnHdnDenNgay').val() });
    $('#cbKhoaKham').change(() => executeBenhNhanFilter());
}

function executeBenhNhanFilter() {
    let tuNgay = $('#bnTuNgay').val(); let denNgay = $('#bnDenNgay').val(); let maKhoa = $('#cbKhoaKham').val();
    if (!tuNgay || !denNgay || (new Date(tuNgay) > new Date(denNgay))) return;

    $('#dashboard-content').css('opacity', '0.5');
    $.ajax({
        url: '/Admin/Dashboard/FilterBenhNhan', type: 'POST', data: { tuNgay: tuNgay, denNgay: denNgay, maKhoa: maKhoa },
        success: function (res) {
            if (res.success) {
                let d = res.data;
                $('#kpiTongKham').html(formatVND(d.TongLuotKham) + ' <small>lượt</small>');
                $('#kpiBHYT').text(d.TyLeBHYT + '%');
                $('#kpiOnline').html(formatVND(d.SoDangKyOnline) + ' <small>người</small>');
                $('#kpiOffline').html(formatVND(d.SoDangKyOffline) + ' <small>người</small>');
                renderBenhNhanCharts(d.LuuLuongTheoGio, d.PhieuGioiTinh, d.PhieuTuyenKham, d.PhanKhucTuoi, d.TopBenhLy);
            }
        },
        complete: () => $('#dashboard-content').css('opacity', '1')
    });
}


// =========================================================================
// PHẦN 3: CODE DÀNH CHO DASHBOARD KHO DƯỢC
// =========================================================================
let kdChartNhapKho, kdChartDanhMuc, kdChartTopThuoc;

// Hàm 1: Vẽ biểu đồ
function renderKhoDuocCharts(nhapKho, danhMuc, topThuoc) {
    // 1. Bar Chart: Chi phí nhập
    if (kdChartNhapKho) kdChartNhapKho.destroy();
    kdChartNhapKho = new Chart(document.getElementById('chartNhapKho'), {
        type: 'bar',
        data: {
            labels: nhapKho.map(x => x.Nhan),
            datasets: [{
                label: 'Chi phí nhập', data: nhapKho.map(x => x.GiaTri),
                backgroundColor: '#3b82f6', borderRadius: 6
            }]
        },
        options: {
            responsive: true, maintainAspectRatio: false, plugins: { legend: { display: false } },
            scales: { x: { grid: { display: false } }, y: { beginAtZero: true, grid: { borderDash: [5, 5] }, ticks: { callback: v => (v >= 1000000 ? (v / 1000000).toFixed(0) + 'M' : v) } } }
        }
    });

    // 2. Doughnut: Cơ cấu Danh mục
    if (kdChartDanhMuc) kdChartDanhMuc.destroy();
    kdChartDanhMuc = new Chart(document.getElementById('chartDanhMuc'), {
        type: 'doughnut',
        data: { labels: danhMuc.map(x => x.Nhan), datasets: [{ data: danhMuc.map(x => x.GiaTri), backgroundColor: ['#8b5cf6', '#ec4899', '#10b981', '#f59e0b', '#64748b'], borderWidth: 0 }] },
        options: { responsive: true, maintainAspectRatio: false, cutout: '70%' }
    });

    // 3. Horizontal Bar: Top Thuốc xuất
    if (kdChartTopThuoc) kdChartTopThuoc.destroy();
    kdChartTopThuoc = new Chart(document.getElementById('chartTopThuoc'), {
        type: 'bar',
        data: { labels: topThuoc.map(x => x.Nhan), datasets: [{ label: 'Đã xuất', data: topThuoc.map(x => x.GiaTri), backgroundColor: '#ef4444', borderRadius: 4 }] },
        options: { indexAxis: 'y', responsive: true, maintainAspectRatio: false, plugins: { legend: { display: false } }, scales: { x: { grid: { display: false } }, y: { grid: { display: false } } } }
    });
}

// Hàm 2: Khởi tạo Bộ lọc
let fpKdTuNgay, fpKdDenNgay;
function initKhoDuocFilter() {
    let cfg = { dateFormat: "Y-m-d", locale: "vn", altInput: true, altFormat: "d/m/Y", monthSelectorType: "dropdown", onChange: () => executeKhoDuocFilter() };
    fpKdTuNgay = flatpickr("#kdTuNgay", { ...cfg, defaultDate: $('#kdHdnTuNgay').val() });
    fpKdDenNgay = flatpickr("#kdDenNgay", { ...cfg, defaultDate: $('#kdHdnDenNgay').val() });
    $('#cbKhoDuoc').change(() => executeKhoDuocFilter());
}

// Hàm 3: Chạy Live Search lấy Data mới
function executeKhoDuocFilter() {
    let tuNgay = $('#kdTuNgay').val(); let denNgay = $('#kdDenNgay').val(); let maKho = $('#cbKhoDuoc').val();
    if (!tuNgay || !denNgay || (new Date(tuNgay) > new Date(denNgay))) return;

    $('#dashboard-content').css('opacity', '0.5');
    $.ajax({
        url: '/Admin/Dashboard/FilterKhoDuoc', type: 'POST', data: { tuNgay: tuNgay, denNgay: denNgay, maKho: maKho },
        success: function (res) {
            if (res.success) {
                let d = res.data;
                // Cập nhật số liệu
                $('#kpiTonKho').html(formatVND(d.TongGiaTriTon) + ' <small>đ</small>');
                $('#kpiNhapKho').html(formatVND(d.GiaTriNhapThang) + ' <small>đ</small>');
                $('#kpiHetHan').html(formatVND(d.ThuocSapHetHan) + ' <small>lô thuốc</small>');
                $('#kpiHetHang').html(formatVND(d.ThuocSapHetHang) + ' <small>loại thuốc</small>');

                // Cập nhật Bảng (Table)
                let tbody = $('#tbCanhBao tbody'); tbody.empty();
                if (d.DanhSachCanhBao.length === 0) {
                    tbody.append('<tr><td colspan="4" class="text-center text-muted py-3">Kho dược an toàn, không có thuốc cận date!</td></tr>');
                } else {
                    d.DanhSachCanhBao.forEach(item => {
                        tbody.append(`<tr><td class="fw-semibold text-dark">${item.TenThuoc}</td><td><span class="badge bg-light text-secondary border">${item.MaLo}</span></td><td class="text-danger fw-bold">${item.HanSuDung}</td><td>${item.SoLuongTon}</td></tr>`);
                    });
                }

                renderKhoDuocCharts(d.BieuDoNhapKho, d.TyTrongDanhMuc, d.TopThuocXuat);
            }
        },
        complete: () => $('#dashboard-content').css('opacity', '1')
    });
}

// =========================================================================
// PHẦN 5: DASHBOARD TỔNG QUAN CŨ (FIX NÚT 7 NGÀY / 12 THÁNG)
// =========================================================================
let revenueChart = null;

function initChart(labels, data, label) {
    let ctxEl = document.getElementById('revenueChart');
    if (!ctxEl) return; // Bảo vệ an toàn nếu đang ở tab khác

    const ctx = ctxEl.getContext('2d');
    if (revenueChart) { revenueChart.destroy(); }

    revenueChart = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: labels,
            datasets: [{
                label: label,
                data: data,
                backgroundColor: 'rgba(59, 130, 246, 0.7)',
                borderColor: 'rgba(59, 130, 246, 1)',
                borderWidth: 1,
                borderRadius: 8,
                barThickness: label === 'Doanh thu 7 ngày' ? 30 : 20
            }]
        },
        options: {
            responsive: true, maintainAspectRatio: false,
            plugins: {
                legend: { display: false },
                tooltip: { callbacks: { label: context => formatVND(context.parsed.y) + ' đ' } }
            },
            scales: {
                y: {
                    beginAtZero: true, grid: { color: '#f1f5f9' },
                    ticks: { callback: v => (v >= 1000000 ? (v / 1000000).toFixed(1) + 'M' : (v >= 1000 ? (v / 1000).toFixed(0) + 'K' : v)) }
                },
                x: { grid: { display: false } }
            }
        }
    });
}

// Bắt sự kiện khi người dùng nhấn nút 7 ngày hoặc 12 tháng
function switchChart(type, btnElement) {
    // 1. Gỡ class 'active' khỏi tất cả các nút
    document.querySelectorAll('.nav-chart-tabs .nav-link').forEach(el => el.classList.remove('active'));

    // 2. Gắn class 'active' cho nút vừa bấm
    if (btnElement) { btnElement.classList.add('active'); }

    // 3. Đọc dữ liệu từ biến toàn cục bên View HTML và vẽ lại
    // Lưu ý: Các biến chartLabels7, chartData7... đã được gắn ở dưới cùng file _DashboardTongQuan.cshtml
    if (type === 'week' && typeof chartLabels7 !== 'undefined') {
        initChart(chartLabels7, chartData7, 'Doanh thu 7 ngày');
    } else if (type === 'month' && typeof chartLabels12 !== 'undefined') {
        initChart(chartLabels12, chartData12, 'Doanh thu 12 tháng');
    }
}