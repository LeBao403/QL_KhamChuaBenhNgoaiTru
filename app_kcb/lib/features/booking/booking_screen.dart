import 'dart:async';
import 'package:flutter/material.dart';
import 'package:intl/intl.dart';
import '../../core/models/user_model.dart';
import '../../core/services/booking_service.dart';
import '../../core/theme/app_theme.dart';
import '../../shared/widgets/common_widgets.dart';

class BookingScreen extends StatefulWidget {
  const BookingScreen({super.key});

  @override
  State<BookingScreen> createState() => _BookingScreenState();
}

class _BookingScreenState extends State<BookingScreen>
    with SingleTickerProviderStateMixin {
  late TabController _tabCtrl;
  final _bookingService = BookingService();

  // ── State ─────────────────────────────────────────────────────────────────
  int _step = 0; // 0=date, 1=time, 2=service, 3=confirm
  bool _bookingDone = false;

  DateTime? _selectedDate;
  TimeSlotModel? _selectedSlot;
  ServiceModel? _selectedService;
  final _lyDoCtrl = TextEditingController();

  List<TimeSlotModel> _slots = [];
  bool _loadingSlots = false;
  String? _slotError;

  List<ServiceModel> _services = [];
  bool _loadingServices = false;

  String _phuongThucTT = 'QR';
  bool _isSubmitting = false;
  String? _submitError;

  // My appointments
  List<LichKhamModel> _appointments = [];
  bool _loadingAppts = false;
  String? _apptError;

  // Booking result (set after successful datLichKham)
  String? _maPhieuDK;
  String? _maHD;
  String _tenQuay = 'Quầy Tiếp Tân';
  Timer? _pollTimer;

  // ── Lifecycle ──────────────────────────────────────────────────────────────
  @override
  void initState() {
    super.initState();
    _tabCtrl = TabController(length: 2, vsync: this);
    _tabCtrl.addListener(() {
      if (_tabCtrl.index == 1 && _appointments.isEmpty) {
        _loadAppointments();
      }
    });
    _loadServiceList();
  }

  @override
  void dispose() {
    _tabCtrl.dispose();
    _lyDoCtrl.dispose();
    _pollTimer?.cancel();
    super.dispose();
  }

  // ── Data loaders ───────────────────────────────────────────────────────────
  Future<void> _loadServiceList() async {
    setState(() => _loadingServices = true);
    final result = await _bookingService.getDanhSachDichVu();
    if (mounted) {
      setState(() {
        _loadingServices = false;
        if (result.success) _services = result.data ?? [];
      });
    }
  }

  Future<void> _loadTimeSlots() async {
    if (_selectedDate == null) return;
    setState(() {
      _loadingSlots = true;
      _selectedSlot = null;
      _slots = [];
      _slotError = null;
    });
    final result = await _bookingService.getKhungGio(_selectedDate!);
    if (mounted) {
      setState(() {
        _loadingSlots = false;
        if (result.success) {
          _slots = result.data ?? [];
        } else {
          _slotError = result.message;
        }
      });
    }
  }

  Future<void> _loadAppointments() async {
    setState(() {
      _loadingAppts = true;
      _apptError = null;
    });
    final result = await _bookingService.getLichKham();
    if (mounted) {
      setState(() {
        _loadingAppts = false;
        if (result.success) {
          _appointments = result.data ?? [];
        } else {
          _apptError = result.message;
        }
      });
    }
  }

  // ── Submit booking ─────────────────────────────────────────────────────────
  Future<void> _submitBooking() async {
    if (_selectedDate == null ||
        _selectedSlot == null ||
        _selectedService == null ||
        _lyDoCtrl.text.trim().isEmpty) return;

    setState(() {
      _isSubmitting = true;
      _submitError = null;
    });

    final result = await _bookingService.datLichKham(
      ngayKham: _selectedDate!,
      maKhungGio: _selectedSlot!.maKhungGio,
      maDV: _selectedService!.maDV,
      lyDo: _lyDoCtrl.text.trim(),
    );

    if (!mounted) return;
    setState(() => _isSubmitting = false);

    if (result.success) {
      _maPhieuDK = result.data!['maPhieuDK'];
      _maHD = result.data!['maHD'];
      _tenQuay = result.data!['tenQuay'] ?? 'Quầy Tiếp Tân';

      if (_phuongThucTT == 'QR') {
        _showQRPaymentDialog();
      } else {
        _showCardPaymentDialog();
      }
    } else {
      setState(() => _submitError = result.message);
    }
  }

  // ── QR Payment flow ────────────────────────────────────────────────────────
  void _showQRPaymentDialog() {
    showDialog(
      context: context,
      barrierDismissible: false,
      builder: (_) => _QRPaymentDialog(
        maHD: _maHD!,
        maPhieuDK: _maPhieuDK!,
        tenQuay: _tenQuay,
        bookingService: _bookingService,
        onPaid: () {
          Navigator.pop(context);
          setState(() => _bookingDone = true);
          _loadAppointments();
        },
        onCancel: () {
          _bookingService.huyDatLichKhiKhongThanhToan(_maPhieuDK!);
          Navigator.pop(context);
        },
      ),
    );
  }

  // ── Card Payment mock ──────────────────────────────────────────────────────
  void _showCardPaymentDialog() {
    showDialog(
      context: context,
      builder: (_) => AlertDialog(
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(20)),
        title: const Row(
          children: [
            Icon(Icons.credit_card_rounded, color: AppTheme.primary),
            SizedBox(width: 8),
            Text('Thanh toán thẻ', style: TextStyle(fontSize: 18)),
          ],
        ),
        content: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            Container(
              padding: const EdgeInsets.all(16),
              decoration: BoxDecoration(
                gradient: AppTheme.heroGradient,
                borderRadius: BorderRadius.circular(16),
              ),
              child: Column(
                children: [
                  const Icon(Icons.credit_card_rounded,
                      color: Colors.white, size: 48),
                  const SizedBox(height: 12),
                  const Text('Phí đặt lịch',
                      style: TextStyle(color: Colors.white70, fontSize: 13)),
                  const Text('100.000 VNĐ',
                      style: TextStyle(
                          color: Colors.white,
                          fontSize: 24,
                          fontWeight: FontWeight.w800)),
                ],
              ),
            ),
            const SizedBox(height: 16),
            const Text(
              'Xác nhận thanh toán phí đặt lịch 100.000 VNĐ qua thẻ ATM/Visa?',
              textAlign: TextAlign.center,
              style: TextStyle(color: AppTheme.textBody, height: 1.5),
            ),
          ],
        ),
        actions: [
          TextButton(
            onPressed: () {
              _bookingService.huyDatLichKhiKhongThanhToan(_maPhieuDK!);
              Navigator.pop(context);
            },
            child: const Text('Hủy', style: TextStyle(color: AppTheme.danger)),
          ),
          ElevatedButton(
            onPressed: () async {
              Navigator.pop(context);
              final r = await _bookingService.xacNhanThanhToanThe(
                  _maHD!, _maPhieuDK!);
              if (r.success) {
                setState(() => _bookingDone = true);
                _loadAppointments();
              }
            },
            child: const Text('Xác nhận thanh toán'),
          ),
        ],
      ),
    );
  }

  void _resetBooking() {
    setState(() {
      _step = 0;
      _bookingDone = false;
      _selectedDate = null;
      _selectedSlot = null;
      _selectedService = null;
      _lyDoCtrl.clear();
      _phuongThucTT = 'QR';
      _submitError = null;
      _maPhieuDK = null;
      _maHD = null;
      _slots = [];
    });
  }

  // ── Helpers ────────────────────────────────────────────────────────────────
  String _fmtCurrency(double amount) =>
      NumberFormat.currency(locale: 'vi_VN', symbol: 'VNĐ').format(amount);

  String _fmtDate(DateTime date) =>
      DateFormat('EEEE, dd/MM/yyyy', 'vi_VN').format(date);

  // ── BUILD ──────────────────────────────────────────────────────────────────
  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppTheme.bgLight,
      appBar: AppBar(
        title: const Text('Đặt lịch khám'),
        bottom: TabBar(
          controller: _tabCtrl,
          indicatorColor: AppTheme.primary,
          labelColor: AppTheme.primary,
          unselectedLabelColor: AppTheme.textMuted,
          labelStyle:
              const TextStyle(fontSize: 14, fontWeight: FontWeight.w600),
          tabs: const [
            Tab(text: 'Đặt lịch mới'),
            Tab(text: 'Lịch của tôi'),
          ],
        ),
      ),
      body: TabBarView(
        controller: _tabCtrl,
        physics: const NeverScrollableScrollPhysics(),
        children: [
          _bookingDone ? _buildSuccessView() : _buildNewBookingTab(),
          _buildMyAppointmentsTab(),
        ],
      ),
    );
  }

  // ───────── TAB 1: NEW BOOKING ────────────────────────────────────────────
  Widget _buildNewBookingTab() {
    return SingleChildScrollView(
      padding: const EdgeInsets.all(16),
      child: Column(
        children: [
          _buildInfoBanner(),
          const SizedBox(height: 20),
          _buildStepIndicator(),
          const SizedBox(height: 24),
          AnimatedSwitcher(
            duration: const Duration(milliseconds: 250),
            transitionBuilder: (child, anim) =>
                FadeTransition(opacity: anim, child: child),
            child: KeyedSubtree(
              key: ValueKey(_step),
              child: _buildStep(),
            ),
          ),
          if (_submitError != null) ...[
            const SizedBox(height: 16),
            Container(
              padding: const EdgeInsets.all(14),
              decoration: BoxDecoration(
                color: AppTheme.danger.withOpacity(0.08),
                borderRadius: BorderRadius.circular(12),
                border: Border.all(color: AppTheme.danger.withOpacity(0.3)),
              ),
              child: Row(
                children: [
                  const Icon(Icons.error_outline_rounded,
                      color: AppTheme.danger, size: 18),
                  const SizedBox(width: 8),
                  Expanded(
                    child: Text(_submitError!,
                        style: const TextStyle(
                            color: AppTheme.danger, fontSize: 13)),
                  ),
                ],
              ),
            ),
          ],
        ],
      ),
    );
  }

  Widget _buildInfoBanner() {
    return Container(
      padding: const EdgeInsets.all(14),
      decoration: BoxDecoration(
        color: const Color(0xFFF0FDF4),
        border: Border.all(color: const Color(0xFF86EFAC)),
        borderRadius: BorderRadius.circular(12),
      ),
      child: Row(
        children: [
          const Icon(Icons.info_outline_rounded,
              color: AppTheme.secondary, size: 20),
          const SizedBox(width: 12),
          Expanded(
            child: RichText(
              text: const TextSpan(
                style: TextStyle(
                    color: Color(0xFF166534), fontSize: 13, height: 1.4),
                children: [
                  TextSpan(text: 'Phí đặt lịch '),
                  TextSpan(
                      text: '100.000 VNĐ',
                      style: TextStyle(fontWeight: FontWeight.w700)),
                  TextSpan(
                      text:
                          ' để giữ chỗ. Phí này độc lập với viện phí khi khám.'),
                ],
              ),
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildStepIndicator() {
    final steps = ['Chọn ngày', 'Khung giờ', 'Dịch vụ', 'Xác nhận'];
    return Row(
      children: List.generate(steps.length, (i) {
        final isActive = i == _step;
        final isDone = i < _step;
        return Expanded(
          child: Row(
            children: [
              Expanded(
                child: Column(
                  children: [
                    AnimatedContainer(
                      duration: const Duration(milliseconds: 250),
                      width: 32,
                      height: 32,
                      decoration: BoxDecoration(
                        gradient:
                            isActive || isDone ? AppTheme.heroGradient : null,
                        color: isActive || isDone ? null : AppTheme.borderLight,
                        shape: BoxShape.circle,
                        boxShadow: isActive
                            ? [
                                BoxShadow(
                                  color: AppTheme.primary.withOpacity(0.4),
                                  blurRadius: 8,
                                )
                              ]
                            : null,
                      ),
                      child: Center(
                        child: isDone
                            ? const Icon(Icons.check_rounded,
                                color: Colors.white, size: 16)
                            : Text(
                                '${i + 1}',
                                style: TextStyle(
                                  color: isActive
                                      ? Colors.white
                                      : AppTheme.textMuted,
                                  fontWeight: FontWeight.w700,
                                  fontSize: 13,
                                ),
                              ),
                      ),
                    ),
                    const SizedBox(height: 4),
                    Text(steps[i],
                        style: TextStyle(
                          fontSize: 10,
                          fontWeight:
                              isActive ? FontWeight.w600 : FontWeight.w400,
                          color:
                              isActive ? AppTheme.primary : AppTheme.textMuted,
                        )),
                  ],
                ),
              ),
              if (i < steps.length - 1)
                Expanded(
                  child: Container(
                    height: 2,
                    margin: const EdgeInsets.only(bottom: 20),
                    decoration: BoxDecoration(
                      color:
                          i < _step ? AppTheme.primary : AppTheme.borderLight,
                      borderRadius: BorderRadius.circular(1),
                    ),
                  ),
                ),
            ],
          ),
        );
      }),
    );
  }

  Widget _buildStep() {
    switch (_step) {
      case 0:
        return _buildStepDate();
      case 1:
        return _buildStepTime();
      case 2:
        return _buildStepService();
      case 3:
        return _buildStepConfirm();
      default:
        return const SizedBox.shrink();
    }
  }

  // ── Step 0: Date ──────────────────────────────────────────────────────────
  Widget _buildStepDate() {
    final now = DateTime.now();
    final dates = List.generate(14, (i) => now.add(Duration(days: i + 1)));
    const dayNames = ['T2', 'T3', 'T4', 'T5', 'T6', 'T7', 'CN'];

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        const Text('Chọn ngày khám',
            style: TextStyle(
                fontSize: 18,
                fontWeight: FontWeight.w700,
                color: AppTheme.textDark)),
        const SizedBox(height: 4),
        const Text('Bệnh viện làm việc Thứ 2 đến Thứ 6',
            style: TextStyle(color: AppTheme.textMuted, fontSize: 13)),
        const SizedBox(height: 20),
        GridView.builder(
          shrinkWrap: true,
          physics: const NeverScrollableScrollPhysics(),
          gridDelegate: const SliverGridDelegateWithFixedCrossAxisCount(
            crossAxisCount: 4,
            crossAxisSpacing: 8,
            mainAxisSpacing: 8,
            childAspectRatio: 0.82,
          ),
          itemCount: dates.length,
          itemBuilder: (_, i) {
            final date = dates[i];
            final isWeekend = date.weekday >= 6;
            final isSelected = _selectedDate != null &&
                _selectedDate!.day == date.day &&
                _selectedDate!.month == date.month;

            return GestureDetector(
              onTap:
                  isWeekend ? null : () => setState(() => _selectedDate = date),
              child: AnimatedContainer(
                duration: const Duration(milliseconds: 200),
                decoration: BoxDecoration(
                  gradient: isSelected ? AppTheme.heroGradient : null,
                  color: isWeekend
                      ? const Color(0xFFF1F5F9)
                      : isSelected
                          ? null
                          : Colors.white,
                  borderRadius: BorderRadius.circular(12),
                  border: Border.all(
                    color: isSelected ? AppTheme.primary : AppTheme.borderLight,
                    width: isSelected ? 2 : 1,
                  ),
                  boxShadow: isSelected
                      ? [
                          BoxShadow(
                            color: AppTheme.primary.withOpacity(0.3),
                            blurRadius: 10,
                            offset: const Offset(0, 3),
                          )
                        ]
                      : null,
                ),
                child: Column(
                  mainAxisAlignment: MainAxisAlignment.center,
                  children: [
                    Text(
                      dayNames[date.weekday - 1],
                      style: TextStyle(
                          fontSize: 11,
                          color: isWeekend
                              ? AppTheme.textLight
                              : isSelected
                                  ? Colors.white70
                                  : AppTheme.textMuted),
                    ),
                    const SizedBox(height: 4),
                    Text(
                      '${date.day}',
                      style: TextStyle(
                          fontSize: 20,
                          fontWeight: FontWeight.w800,
                          color: isWeekend
                              ? AppTheme.textLight
                              : isSelected
                                  ? Colors.white
                                  : AppTheme.textDark),
                    ),
                    Text(
                      'Th${date.month}',
                      style: TextStyle(
                          fontSize: 10,
                          color: isWeekend
                              ? AppTheme.textLight
                              : isSelected
                                  ? Colors.white70
                                  : AppTheme.textMuted),
                    ),
                    if (isWeekend)
                      const Text('Nghỉ',
                          style: TextStyle(
                              fontSize: 9, color: AppTheme.textLight)),
                  ],
                ),
              ),
            );
          },
        ),
        const SizedBox(height: 24),
        SizedBox(
          width: double.infinity,
          child: GradientButton(
            label: 'Xem khung giờ',
            icon: Icons.access_time_rounded,
            onPressed: _selectedDate != null
                ? () {
                    setState(() => _step = 1);
                    _loadTimeSlots();
                  }
                : null,
          ),
        ),
      ],
    );
  }

  // ── Step 1: Time ──────────────────────────────────────────────────────────
  Widget _buildStepTime() {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        _backRow('Chọn khung giờ', 0),
        const SizedBox(height: 4),
        if (_selectedDate != null) _dateChip(_selectedDate!),
        const SizedBox(height: 20),
        if (_loadingSlots)
          const Center(child: CircularProgressIndicator())
        else if (_slotError != null)
          _emptyState(Icons.error_outline_rounded, _slotError!)
        else if (_slots.isEmpty)
          _emptyState(Icons.access_time_rounded,
              'Không có khung giờ nào khả dụng cho ngày này.')
        else
          GridView.builder(
            shrinkWrap: true,
            physics: const NeverScrollableScrollPhysics(),
            gridDelegate: const SliverGridDelegateWithFixedCrossAxisCount(
              crossAxisCount: 2,
              crossAxisSpacing: 10,
              mainAxisSpacing: 10,
              childAspectRatio: 2.4,
            ),
            itemCount: _slots.length,
            itemBuilder: (_, i) {
              final slot = _slots[i];
              final disabled = slot.isDisabledForDate(_selectedDate!);
              return TimeSlotChip(
                time: slot.tenKhungGio,
                isSelected: _selectedSlot?.maKhungGio == slot.maKhungGio,
                isDisabled: disabled,
                status: slot.statusForDate(_selectedDate!),
                onTap: () => setState(() => _selectedSlot = slot),
              );
            },
          ),
        const SizedBox(height: 24),
        SizedBox(
          width: double.infinity,
          child: GradientButton(
            label: 'Chọn dịch vụ',
            icon: Icons.medical_services_rounded,
            onPressed:
                _selectedSlot != null ? () => setState(() => _step = 2) : null,
          ),
        ),
      ],
    );
  }

  // ── Step 2: Service ───────────────────────────────────────────────────────
  Widget _buildStepService() {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        _backRow('Chọn dịch vụ khám', 1),
        const SizedBox(height: 20),

        if (_loadingServices)
          const Center(child: CircularProgressIndicator())
        else if (_services.isEmpty)
          _emptyState(Icons.medical_services_rounded,
              'Không tải được danh sách dịch vụ.')
        else
          ListView.separated(
            shrinkWrap: true,
            physics: const NeverScrollableScrollPhysics(),
            itemCount: _services.length,
            separatorBuilder: (_, __) => const SizedBox(height: 8),
            itemBuilder: (_, i) {
              final sv = _services[i];
              final isSelected = _selectedService?.maDV == sv.maDV;
              return GestureDetector(
                onTap: () => setState(() => _selectedService = sv),
                child: AnimatedContainer(
                  duration: const Duration(milliseconds: 200),
                  padding: const EdgeInsets.all(14),
                  decoration: BoxDecoration(
                    color: isSelected ? const Color(0xFFEFF6FF) : Colors.white,
                    borderRadius: BorderRadius.circular(12),
                    border: Border.all(
                      color:
                          isSelected ? AppTheme.primary : AppTheme.borderLight,
                      width: isSelected ? 2 : 1,
                    ),
                  ),
                  child: Row(
                    children: [
                      Container(
                        width: 40,
                        height: 40,
                        decoration: BoxDecoration(
                          color: isSelected
                              ? AppTheme.primary.withOpacity(0.1)
                              : AppTheme.bgLight,
                          borderRadius: BorderRadius.circular(8),
                        ),
                        child: Icon(Icons.medical_services_rounded,
                            color: isSelected
                                ? AppTheme.primary
                                : AppTheme.textMuted,
                            size: 20),
                      ),
                      const SizedBox(width: 12),
                      Expanded(
                        child: Column(
                          crossAxisAlignment: CrossAxisAlignment.start,
                          children: [
                            Text(sv.tenDV,
                                style: TextStyle(
                                  fontSize: 14,
                                  fontWeight: FontWeight.w600,
                                  color: isSelected
                                      ? AppTheme.primary
                                      : AppTheme.textDark,
                                )),
                            if (sv.giaDichVu > 0)
                              Text(_fmtCurrency(sv.giaDichVu),
                                  style: const TextStyle(
                                      color: AppTheme.secondary,
                                      fontSize: 13,
                                      fontWeight: FontWeight.w600)),
                          ],
                        ),
                      ),
                      if (isSelected)
                        Container(
                          width: 20,
                          height: 20,
                          decoration: const BoxDecoration(
                              color: AppTheme.primary, shape: BoxShape.circle),
                          child: const Icon(Icons.check_rounded,
                              color: Colors.white, size: 14),
                        ),
                    ],
                  ),
                ),
              );
            },
          ),
        const SizedBox(height: 20),

        // Lý do khám
        TextField(
          controller: _lyDoCtrl,
          maxLines: 3,
          onChanged: (_) => setState(() {}),
          decoration: const InputDecoration(
            labelText: 'Triệu chứng / Lý do đến khám *',
            hintText: 'Mô tả ngắn gọn tình trạng sức khỏe...',
            alignLabelWithHint: true,
          ),
        ),
        const SizedBox(height: 24),
        SizedBox(
          width: double.infinity,
          child: GradientButton(
            label: 'Xác nhận',
            icon: Icons.check_circle_outline_rounded,
            onPressed:
                (_selectedService != null && _lyDoCtrl.text.trim().isNotEmpty)
                    ? () => setState(() => _step = 3)
                    : null,
          ),
        ),
      ],
    );
  }

  // ── Step 3: Confirm ───────────────────────────────────────────────────────
  Widget _buildStepConfirm() {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        _backRow('Xác nhận đặt lịch', 2),
        const SizedBox(height: 20),

        // Summary card
        Container(
          padding: const EdgeInsets.all(20),
          decoration: BoxDecoration(
            color: Colors.white,
            borderRadius: BorderRadius.circular(16),
            border: Border.all(color: AppTheme.borderLight),
          ),
          child: Column(
            children: [
              _summaryRow(Icons.calendar_today_rounded, 'Ngày khám',
                  _selectedDate != null ? _fmtDate(_selectedDate!) : ''),
              const Divider(height: 20),
              _summaryRow(Icons.access_time_rounded, 'Khung giờ',
                  _selectedSlot?.tenKhungGio ?? ''),
              const Divider(height: 20),
              _summaryRow(Icons.medical_services_rounded, 'Dịch vụ',
                  _selectedService?.tenDV ?? ''),
              const Divider(height: 20),
              _summaryRow(
                  Icons.notes_rounded, 'Lý do khám', _lyDoCtrl.text.trim()),
              const Divider(height: 20),
              _summaryRow(Icons.payments_rounded, 'Phí đặt lịch', '100.000 VNĐ',
                  valueColor: AppTheme.danger),
            ],
          ),
        ),
        const SizedBox(height: 20),

        const Text('Phương thức thanh toán phí đặt lịch',
            style: TextStyle(
                fontSize: 15,
                fontWeight: FontWeight.w700,
                color: AppTheme.textDark)),
        const SizedBox(height: 12),
        Row(
          children: [
            Expanded(
              child: _paymentCard('QR', Icons.qr_code_rounded,
                  'Ứng dụng ngân hàng', 'Quét VietQR'),
            ),
            const SizedBox(width: 10),
            Expanded(
              child: _paymentCard('THE', Icons.credit_card_rounded,
                  'Thẻ ATM/Visa', 'Cổng thanh toán'),
            ),
          ],
        ),
        const SizedBox(height: 16),

        // Security note
        Container(
          padding: const EdgeInsets.all(12),
          decoration: BoxDecoration(
            color: const Color(0xFFFEFCE8),
            borderRadius: BorderRadius.circular(10),
            border: Border.all(color: const Color(0xFFFEF08A)),
          ),
          child: const Row(
            children: [
              Icon(Icons.shield_outlined, color: AppTheme.accent, size: 18),
              SizedBox(width: 8),
              Expanded(
                child: Text(
                  'Kết nối thanh toán được mã hóa an toàn qua PayOS.',
                  style: TextStyle(
                      color: Color(0xFF854D0E), fontSize: 12, height: 1.4),
                ),
              ),
            ],
          ),
        ),
        const SizedBox(height: 24),

        SizedBox(
          width: double.infinity,
          child: GradientButton(
            label: 'Tiến hành thanh toán',
            icon: Icons.lock_rounded,
            isLoading: _isSubmitting,
            onPressed: _isSubmitting ? null : _submitBooking,
          ),
        ),
      ],
    );
  }

  Widget _summaryRow(IconData icon, String label, String value,
      {Color? valueColor}) {
    return Row(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Container(
          width: 36,
          height: 36,
          decoration: BoxDecoration(
            color: AppTheme.primary.withOpacity(0.08),
            borderRadius: BorderRadius.circular(8),
          ),
          child: Icon(icon, color: AppTheme.primary, size: 18),
        ),
        const SizedBox(width: 12),
        Expanded(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(label,
                  style:
                      const TextStyle(color: AppTheme.textMuted, fontSize: 12)),
              Text(value,
                  style: TextStyle(
                      color: valueColor ?? AppTheme.textDark,
                      fontSize: 14,
                      fontWeight: FontWeight.w600)),
            ],
          ),
        ),
      ],
    );
  }

  Widget _paymentCard(String value, IconData icon, String label, String desc) {
    final isSelected = _phuongThucTT == value;
    return GestureDetector(
      onTap: () => setState(() => _phuongThucTT = value),
      child: AnimatedContainer(
        duration: const Duration(milliseconds: 200),
        padding: const EdgeInsets.all(12),
        decoration: BoxDecoration(
          color: isSelected ? const Color(0xFFEFF6FF) : Colors.white,
          borderRadius: BorderRadius.circular(12),
          border: Border.all(
            color: isSelected ? AppTheme.primary : AppTheme.borderLight,
            width: isSelected ? 2 : 1,
          ),
        ),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                Icon(icon,
                    color: isSelected ? AppTheme.primary : AppTheme.textMuted,
                    size: 24),
                const Spacer(),
                if (isSelected)
                  const Icon(Icons.check_circle_rounded,
                      color: AppTheme.primary, size: 18),
              ],
            ),
            const SizedBox(height: 8),
            Text(label,
                style: TextStyle(
                    fontSize: 13,
                    fontWeight: FontWeight.w700,
                    color: isSelected ? AppTheme.primary : AppTheme.textDark)),
            Text(desc,
                style:
                    const TextStyle(color: AppTheme.textMuted, fontSize: 11)),
          ],
        ),
      ),
    );
  }

  // ── Helpers UI ────────────────────────────────────────────────────────────
  Widget _backRow(String title, int backStep) {
    return Row(
      children: [
        GestureDetector(
          onTap: () => setState(() => _step = backStep),
          child: const Icon(Icons.arrow_back_ios_new_rounded,
              size: 18, color: AppTheme.primary),
        ),
        const SizedBox(width: 8),
        Text(title,
            style: const TextStyle(
                fontSize: 18,
                fontWeight: FontWeight.w700,
                color: AppTheme.textDark)),
      ],
    );
  }

  Widget _dateChip(DateTime date) {
    return Row(
      children: [
        const Icon(Icons.calendar_today_rounded,
            size: 14, color: AppTheme.primary),
        const SizedBox(width: 6),
        Text(_fmtDate(date),
            style: const TextStyle(
                color: AppTheme.primary,
                fontSize: 13,
                fontWeight: FontWeight.w600)),
      ],
    );
  }

  Widget _emptyState(IconData icon, String msg) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 32),
      child: Center(
        child: Column(
          children: [
            Icon(icon, size: 48, color: AppTheme.textLight),
            const SizedBox(height: 12),
            Text(msg,
                textAlign: TextAlign.center,
                style:
                    const TextStyle(color: AppTheme.textMuted, fontSize: 14)),
          ],
        ),
      ),
    );
  }

  // ── Success view ──────────────────────────────────────────────────────────
  Widget _buildSuccessView() {
    return Center(
      child: Padding(
        padding: const EdgeInsets.all(32),
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Container(
              width: 96,
              height: 96,
              decoration: BoxDecoration(
                gradient: AppTheme.greenGradient,
                shape: BoxShape.circle,
                boxShadow: [
                  BoxShadow(
                    color: AppTheme.secondary.withOpacity(0.3),
                    blurRadius: 24,
                    offset: const Offset(0, 8),
                  ),
                ],
              ),
              child: const Icon(Icons.check_rounded,
                  color: Colors.white, size: 52),
            ),
            const SizedBox(height: 24),
            const Text('Đặt lịch thành công!',
                style: TextStyle(
                    fontSize: 24,
                    fontWeight: FontWeight.w800,
                    color: AppTheme.textDark)),
            const SizedBox(height: 10),
            Text(
              'Lịch hẹn tại $_tenQuay đã được ghi nhận. Vui lòng đến đúng phòng khám để làm thủ tục.',
              textAlign: TextAlign.center,
              style: const TextStyle(
                  color: AppTheme.textMuted, fontSize: 14, height: 1.6),
            ),
            const SizedBox(height: 28),
            Container(
              padding: const EdgeInsets.all(16),
              decoration: BoxDecoration(
                color: const Color(0xFFF0FDF4),
                borderRadius: BorderRadius.circular(14),
                border: Border.all(color: const Color(0xFF86EFAC)),
              ),
              child: Column(
                children: [
                  _successItem(Icons.calendar_today_rounded, 'Ngày khám',
                      _selectedDate != null ? _fmtDate(_selectedDate!) : ''),
                  const SizedBox(height: 8),
                  _successItem(Icons.access_time_rounded, 'Khung giờ',
                      _selectedSlot?.tenKhungGio ?? ''),
                  const SizedBox(height: 8),
                  _successItem(Icons.medical_services_rounded, 'Dịch vụ',
                      _selectedService?.tenDV ?? ''),
                ],
              ),
            ),
            const SizedBox(height: 28),
            Row(
              children: [
                Expanded(
                  child: OutlinedButton.icon(
                    onPressed: _resetBooking,
                    icon: const Icon(Icons.add_rounded),
                    label: const Text('Đặt lịch mới'),
                  ),
                ),
                const SizedBox(width: 12),
                Expanded(
                  child: ElevatedButton.icon(
                    onPressed: () => _tabCtrl.animateTo(1),
                    icon: const Icon(Icons.calendar_month_rounded),
                    label: const Text('Xem lịch'),
                  ),
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }

  Widget _successItem(IconData icon, String label, String value) {
    return Row(
      children: [
        Icon(icon, color: AppTheme.secondary, size: 16),
        const SizedBox(width: 10),
        Text('$label: ',
            style: const TextStyle(color: AppTheme.textMuted, fontSize: 13)),
        Expanded(
          child: Text(value,
              style: const TextStyle(
                  color: AppTheme.secondary,
                  fontSize: 13,
                  fontWeight: FontWeight.w600)),
        ),
      ],
    );
  }

  // ───────── TAB 2: MY APPOINTMENTS ────────────────────────────────────────
  Widget _buildMyAppointmentsTab() {
    if (_loadingAppts) {
      return const Center(child: CircularProgressIndicator());
    }
    if (_apptError != null) {
      return Center(
        child: Padding(
          padding: const EdgeInsets.all(32),
          child: Column(
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              const Icon(Icons.error_outline_rounded,
                  size: 48, color: AppTheme.textLight),
              const SizedBox(height: 12),
              Text(_apptError!,
                  textAlign: TextAlign.center,
                  style: const TextStyle(color: AppTheme.textMuted)),
              const SizedBox(height: 16),
              OutlinedButton.icon(
                onPressed: _loadAppointments,
                icon: const Icon(Icons.refresh_rounded),
                label: const Text('Thử lại'),
              ),
            ],
          ),
        ),
      );
    }

    if (_appointments.isEmpty) {
      return Center(
        child: Padding(
          padding: const EdgeInsets.all(32),
          child: Column(
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              const Icon(Icons.calendar_month_outlined,
                  size: 64, color: AppTheme.textLight),
              const SizedBox(height: 16),
              const Text('Chưa có lịch khám nào',
                  style: TextStyle(
                      fontSize: 18,
                      fontWeight: FontWeight.w700,
                      color: AppTheme.textDark)),
              const SizedBox(height: 8),
              const Text('Nhấn "Đặt lịch mới" để đặt lịch khám',
                  style: TextStyle(color: AppTheme.textMuted)),
              const SizedBox(height: 20),
              ElevatedButton.icon(
                onPressed: () => _tabCtrl.animateTo(0),
                icon: const Icon(Icons.add_rounded),
                label: const Text('Đặt lịch ngay'),
              ),
            ],
          ),
        ),
      );
    }

    return RefreshIndicator(
      onRefresh: _loadAppointments,
      child: ListView.separated(
        padding: const EdgeInsets.all(16),
        itemCount: _appointments.length,
        separatorBuilder: (_, __) => const SizedBox(height: 12),
        itemBuilder: (_, i) => _buildAppointmentCard(_appointments[i]),
      ),
    );
  }

  Widget _buildAppointmentCard(LichKhamModel appt) {
    final statusColor = appt.statusColor;
    return Container(
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(16),
        border: Border.all(color: AppTheme.borderLight),
        boxShadow: [
          BoxShadow(
              color: Colors.black.withOpacity(0.04),
              blurRadius: 10,
              offset: const Offset(0, 2))
        ],
      ),
      child: Column(
        children: [
          // Header
          Container(
            padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 10),
            decoration: BoxDecoration(
              color: statusColor.withOpacity(0.06),
              borderRadius:
                  const BorderRadius.vertical(top: Radius.circular(16)),
            ),
            child: Row(
              children: [
                Icon(Icons.receipt_long_rounded, size: 16, color: statusColor),
                const SizedBox(width: 6),
                Text('Phiếu #${appt.maPhieuDK}',
                    style: TextStyle(
                        color: statusColor,
                        fontSize: 13,
                        fontWeight: FontWeight.w700)),
                const Spacer(),
                Container(
                  padding:
                      const EdgeInsets.symmetric(horizontal: 10, vertical: 4),
                  decoration: BoxDecoration(
                    color: statusColor.withOpacity(0.12),
                    borderRadius: BorderRadius.circular(50),
                  ),
                  child: Text(appt.trangThai,
                      style: TextStyle(
                          color: statusColor,
                          fontSize: 12,
                          fontWeight: FontWeight.w600)),
                ),
              ],
            ),
          ),
          Padding(
            padding: const EdgeInsets.all(16),
            child: Column(
              children: [
                if (appt.lyDoDenKham.isNotEmpty)
                  _apptRow(Icons.medical_services_rounded, appt.lyDoDenKham),
                if (appt.ngayDangKy != null) ...[
                  const SizedBox(height: 8),
                  _apptRow(
                      Icons.calendar_today_rounded,
                      DateFormat('dd/MM/yyyy', 'vi_VN')
                          .format(appt.ngayDangKy!)),
                ],
                if (appt.tenPhong.isNotEmpty) ...[
                  const SizedBox(height: 8),
                  _apptRow(Icons.location_on_rounded, appt.tenPhong),
                ],
                if (appt.tenBacSi.isNotEmpty) ...[
                  const SizedBox(height: 8),
                  _apptRow(Icons.person_rounded, 'BS. ${appt.tenBacSi}'),
                ],
              ],
            ),
          ),
          if (appt.trangThai == 'Chờ xử lý')
            Padding(
              padding: const EdgeInsets.fromLTRB(16, 0, 16, 16),
              child: SizedBox(
                width: double.infinity,
                child: OutlinedButton.icon(
                  onPressed: () => _showCancelDialog(appt.maPhieuDK),
                  icon: const Icon(Icons.cancel_outlined, size: 16),
                  label: const Text('Hủy lịch'),
                  style: OutlinedButton.styleFrom(
                    foregroundColor: AppTheme.danger,
                    side: const BorderSide(color: AppTheme.danger),
                  ),
                ),
              ),
            ),
        ],
      ),
    );
  }

  Widget _apptRow(IconData icon, String value) {
    return Row(
      children: [
        Icon(icon, size: 16, color: AppTheme.textMuted),
        const SizedBox(width: 10),
        Expanded(
          child: Text(value,
              style: const TextStyle(
                  color: AppTheme.textBody,
                  fontSize: 13,
                  fontWeight: FontWeight.w500)),
        ),
      ],
    );
  }

  void _showCancelDialog(String maPhieuDK) {
    showDialog(
      context: context,
      builder: (_) => AlertDialog(
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
        title: const Text('Xác nhận hủy lịch',
            style: TextStyle(fontSize: 18, fontWeight: FontWeight.w700)),
        content: Text(
            'Bạn có chắc muốn hủy lịch khám #$maPhieuDK không? Chỉ lịch ở trạng thái "Chờ xử lý" mới được hủy.'),
        actions: [
          TextButton(
              onPressed: () => Navigator.pop(context),
              child: const Text('Không')),
          ElevatedButton(
            onPressed: () async {
              Navigator.pop(context);
              final r = await _bookingService.huyLich(maPhieuDK);
              if (mounted) {
                ScaffoldMessenger.of(context).showSnackBar(SnackBar(
                  content: Text(r.success
                      ? 'Đã hủy lịch thành công!'
                      : r.message ?? 'Hủy thất bại!'),
                  backgroundColor:
                      r.success ? AppTheme.secondary : AppTheme.danger,
                  behavior: SnackBarBehavior.floating,
                  shape: RoundedRectangleBorder(
                      borderRadius: BorderRadius.circular(10)),
                ));
                if (r.success) _loadAppointments();
              }
            },
            style: ElevatedButton.styleFrom(backgroundColor: AppTheme.danger),
            child: const Text('Xác nhận hủy'),
          ),
        ],
      ),
    );
  }
}

// ─── QR Payment Dialog ────────────────────────────────────────────────────────
class _QRPaymentDialog extends StatefulWidget {
  final String maHD;
  final String maPhieuDK;
  final String tenQuay;
  final BookingService bookingService;
  final VoidCallback onPaid;
  final VoidCallback onCancel;

  const _QRPaymentDialog({
    required this.maHD,
    required this.maPhieuDK,
    required this.tenQuay,
    required this.bookingService,
    required this.onPaid,
    required this.onCancel,
  });

  @override
  State<_QRPaymentDialog> createState() => _QRPaymentDialogState();
}

class _QRPaymentDialogState extends State<_QRPaymentDialog> {
  bool _loadingQR = true;
  String? _qrString;
  String? _qrError;
  int? _orderCode;
  Timer? _pollTimer;
  int _remainSeconds = 300; // 5 phút

  @override
  void initState() {
    super.initState();
    _createQR();
  }

  Future<void> _createQR() async {
    final result =
        await widget.bookingService.taoMaQR(widget.maHD, widget.maPhieuDK);
    if (mounted) {
      setState(() {
        _loadingQR = false;
        if (result.success) {
          _qrString = result.data!['qrString'];
          _orderCode = result.data!['orderCode'];
          _startPolling();
        } else {
          _qrError = result.message;
        }
      });
    }
  }

  void _startPolling() {
    _pollTimer = Timer.periodic(const Duration(seconds: 3), (t) async {
      if (!mounted) {
        t.cancel();
        return;
      }
      final paid = await widget.bookingService
          .kiemTraThanhToan(_orderCode!, widget.maPhieuDK, widget.maHD);
      if (paid) {
        t.cancel();
        widget.onPaid();
      }
      setState(() {
        _remainSeconds -= 3;
        if (_remainSeconds <= 0) {
          t.cancel();
          widget.onCancel();
        }
      });
    });
  }

  @override
  void dispose() {
    _pollTimer?.cancel();
    super.dispose();
  }

  String get _remainStr {
    final m = _remainSeconds ~/ 60;
    final s = _remainSeconds % 60;
    return '${m.toString().padLeft(2, '0')}:${s.toString().padLeft(2, '0')}';
  }

  @override
  Widget build(BuildContext context) {
    return AlertDialog(
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(20)),
      titlePadding: EdgeInsets.zero,
      title: Container(
        padding: const EdgeInsets.all(20),
        decoration: const BoxDecoration(
          gradient: AppTheme.heroGradient,
          borderRadius: BorderRadius.vertical(top: Radius.circular(20)),
        ),
        child: Column(
          children: [
            const Row(
              children: [
                Icon(Icons.qr_code_rounded, color: Colors.white, size: 24),
                SizedBox(width: 10),
                Text('Thanh toán VietQR',
                    style: TextStyle(
                        color: Colors.white,
                        fontSize: 18,
                        fontWeight: FontWeight.w700)),
              ],
            ),
            const SizedBox(height: 8),
            Text('Quầy: ${widget.tenQuay}',
                style: const TextStyle(color: Colors.white70, fontSize: 13)),
          ],
        ),
      ),
      content: SizedBox(
        width: double.maxFinite,
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            if (_loadingQR)
              const Padding(
                padding: EdgeInsets.symmetric(vertical: 40),
                child: CircularProgressIndicator(),
              )
            else if (_qrError != null)
              Padding(
                padding: const EdgeInsets.symmetric(vertical: 20),
                child: Column(
                  children: [
                    const Icon(Icons.error_outline_rounded,
                        color: AppTheme.danger, size: 48),
                    const SizedBox(height: 10),
                    Text(_qrError!,
                        textAlign: TextAlign.center,
                        style: const TextStyle(color: AppTheme.danger)),
                  ],
                ),
              )
            else ...[
              // QR Code (dùng text thay thế vì không có qr_flutter)
              Container(
                width: 250,
                height: 250,
                padding: const EdgeInsets.all(12),
                decoration: BoxDecoration(
                  color: Colors.white,
                  border: Border.all(color: AppTheme.borderLight, width: 2),
                  borderRadius: BorderRadius.circular(12),
                ),
                child: Image.network(
                  'https://api.qrserver.com/v1/create-qr-code/?size=250x250&data=${Uri.encodeComponent(_qrString ?? '')}',
                  fit: BoxFit.contain,
                  errorBuilder: (_, __, ___) => const Center(
                    child: Column(
                      mainAxisAlignment: MainAxisAlignment.center,
                      children: [
                        Icon(Icons.qr_code_2_rounded,
                            size: 80, color: AppTheme.textDark),
                        SizedBox(height: 8),
                        Text('Quét bằng app ngân hàng',
                            style: TextStyle(
                                fontSize: 12, color: AppTheme.textMuted),
                            textAlign: TextAlign.center),
                      ],
                    ),
                  ),
                ),
              ),
              const SizedBox(height: 12),
              const Text(
                'Mở app ngân hàng và quét mã VietQR để thanh toán phí đặt lịch.',
                textAlign: TextAlign.center,
                style: TextStyle(
                  color: AppTheme.textMuted,
                  fontSize: 13,
                  height: 1.4,
                ),
              ),
              const SizedBox(height: 16),
              Container(
                padding:
                    const EdgeInsets.symmetric(horizontal: 16, vertical: 10),
                decoration: BoxDecoration(
                  color: AppTheme.bgLight,
                  borderRadius: BorderRadius.circular(8),
                ),
                child: Column(
                  children: [
                    const Text('Số tiền',
                        style:
                            TextStyle(color: AppTheme.textMuted, fontSize: 12)),
                    const Text('100.000 VNĐ',
                        style: TextStyle(
                            color: AppTheme.danger,
                            fontSize: 22,
                            fontWeight: FontWeight.w800)),
                    const SizedBox(height: 8),
                    Row(
                      mainAxisAlignment: MainAxisAlignment.center,
                      children: [
                        const Icon(Icons.timer_rounded,
                            size: 14, color: AppTheme.textMuted),
                        const SizedBox(width: 4),
                        Text('Hết hạn sau: $_remainStr',
                            style: const TextStyle(
                                color: AppTheme.textMuted, fontSize: 12)),
                      ],
                    ),
                  ],
                ),
              ),
              const SizedBox(height: 12),
              const Text(
                'Đang kiểm tra thanh toán tự động...',
                style: TextStyle(
                    color: AppTheme.primary,
                    fontSize: 12,
                    fontStyle: FontStyle.italic),
              ),
            ],
          ],
        ),
      ),
      actions: [
        TextButton(
          onPressed: () {
            _pollTimer?.cancel();
            widget.onCancel();
          },
          child: const Text('Hủy thanh toán',
              style: TextStyle(color: AppTheme.danger)),
        ),
      ],
    );
  }
}
