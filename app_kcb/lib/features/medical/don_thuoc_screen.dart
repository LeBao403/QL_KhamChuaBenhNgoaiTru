import 'package:flutter/material.dart';
import 'package:intl/intl.dart';
import '../../core/models/user_model.dart';
import '../../core/services/medical_service.dart';
import '../../core/theme/app_theme.dart';

class DonThuocScreen extends StatefulWidget {
  const DonThuocScreen({super.key});

  @override
  State<DonThuocScreen> createState() => _DonThuocScreenState();
}

class _DonThuocScreenState extends State<DonThuocScreen> {
  final _service = MedicalService();
  List<DonThuocModel> _list = [];
  bool _isLoading = true;
  String? _error;

  @override
  void initState() {
    super.initState();
    _load();
  }

  Future<void> _load() async {
    setState(() {
      _isLoading = true;
      _error = null;
    });
    final result = await _service.getDonThuoc();
    if (!mounted) return;
    setState(() {
      _isLoading = false;
      if (result.success) {
        _list = result.data ?? [];
      } else {
        _error = result.message;
      }
    });
  }

  String _fmtDate(DateTime d) =>
      DateFormat('dd/MM/yyyy', 'vi_VN').format(d);

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppTheme.bgLight,
      appBar: AppBar(
        title: const Text('Đơn thuốc'),
        backgroundColor: Colors.white,
        foregroundColor: AppTheme.textDark,
        elevation: 0,
        scrolledUnderElevation: 1,
        shadowColor: const Color(0x18000000),
      ),
      body: _isLoading
          ? const Center(child: CircularProgressIndicator())
          : _error != null
              ? _buildError()
              : _list.isEmpty
                  ? _buildEmpty()
                  : RefreshIndicator(
                      onRefresh: _load,
                      child: ListView.separated(
                        padding: const EdgeInsets.all(16),
                        itemCount: _list.length,
                        separatorBuilder: (_, __) =>
                            const SizedBox(height: 12),
                        itemBuilder: (_, i) => _buildCard(_list[i]),
                      ),
                    ),
    );
  }

  Widget _buildCard(DonThuocModel item) {
    return InkWell(
      onTap: () => _showChiTiet(item),
      borderRadius: BorderRadius.circular(16),
      child: Container(
        decoration: BoxDecoration(
          color: Colors.white,
          borderRadius: BorderRadius.circular(16),
          border: Border.all(color: AppTheme.borderLight),
          boxShadow: [
            BoxShadow(
              color: Colors.black.withOpacity(0.04),
              blurRadius: 8,
              offset: const Offset(0, 2),
            ),
          ],
        ),
        child: Padding(
          padding: const EdgeInsets.all(16),
          child: Row(
            children: [
              Container(
                width: 48,
                height: 48,
                decoration: BoxDecoration(
                  color: const Color(0xFFEFF6FF),
                  borderRadius: BorderRadius.circular(12),
                ),
                child: const Icon(Icons.medication_rounded,
                    color: AppTheme.primary, size: 24),
              ),
              const SizedBox(width: 14),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      'Đơn thuốc ngày ${_fmtDate(item.ngayKe)}',
                      style: const TextStyle(
                        fontWeight: FontWeight.w700,
                        fontSize: 14,
                        color: AppTheme.textDark,
                      ),
                    ),
                    const SizedBox(height: 4),
                    if (item.tenBacSi.isNotEmpty)
                      Text(
                        'Kê bởi BS. ${item.tenBacSi}',
                        style: const TextStyle(
                            color: AppTheme.textMuted, fontSize: 12),
                      ),
                    if (item.loiDanBS.isNotEmpty)
                      Padding(
                        padding: const EdgeInsets.only(top: 4),
                        child: Text(
                          item.loiDanBS,
                          maxLines: 2,
                          overflow: TextOverflow.ellipsis,
                          style: const TextStyle(
                              color: AppTheme.textBody,
                              fontSize: 12,
                              height: 1.4),
                        ),
                      ),
                    const SizedBox(height: 6),
                    Container(
                      padding: const EdgeInsets.symmetric(
                          horizontal: 8, vertical: 3),
                      decoration: BoxDecoration(
                        color: item.statusColor.withOpacity(0.1),
                        borderRadius: BorderRadius.circular(50),
                        border: Border.all(
                            color: item.statusColor.withOpacity(0.3)),
                      ),
                      child: Text(
                        item.trangThai,
                        style: TextStyle(
                            color: item.statusColor,
                            fontSize: 11,
                            fontWeight: FontWeight.w600),
                      ),
                    ),
                  ],
                ),
              ),
              const Icon(Icons.chevron_right_rounded,
                  color: AppTheme.textLight),
            ],
          ),
        ),
      ),
    );
  }

  Future<void> _showChiTiet(DonThuocModel don) async {
    showModalBottomSheet(
      context: context,
      isScrollControlled: true,
      backgroundColor: Colors.transparent,
      builder: (_) => _ChiTietDonThuocSheet(
        don: don,
        service: _service,
      ),
    );
  }

  Widget _buildEmpty() {
    return Center(
      child: Column(
        mainAxisAlignment: MainAxisAlignment.center,
        children: [
          Container(
            width: 80,
            height: 80,
            decoration: BoxDecoration(
              color: AppTheme.primary.withOpacity(0.08),
              shape: BoxShape.circle,
            ),
            child: const Icon(Icons.medication_rounded,
                size: 40, color: AppTheme.primary),
          ),
          const SizedBox(height: 16),
          const Text(
            'Chưa có đơn thuốc nào',
            style: TextStyle(
                color: AppTheme.textDark,
                fontSize: 16,
                fontWeight: FontWeight.w600),
          ),
          const SizedBox(height: 8),
          const Text(
            'Các đơn thuốc từ lần khám sẽ hiển thị ở đây',
            style: TextStyle(color: AppTheme.textMuted, fontSize: 13),
          ),
        ],
      ),
    );
  }

  Widget _buildError() {
    return Center(
      child: Padding(
        padding: const EdgeInsets.all(32),
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            const Icon(Icons.error_outline_rounded,
                size: 56, color: AppTheme.textLight),
            const SizedBox(height: 16),
            Text(_error!,
                textAlign: TextAlign.center,
                style: const TextStyle(color: AppTheme.textMuted)),
            const SizedBox(height: 16),
            ElevatedButton.icon(
              onPressed: _load,
              icon: const Icon(Icons.refresh_rounded),
              label: const Text('Thử lại'),
            ),
          ],
        ),
      ),
    );
  }
}

// ─── Bottom sheet chi tiết đơn thuốc ──────────────────────────────────────────
class _ChiTietDonThuocSheet extends StatefulWidget {
  final DonThuocModel don;
  final MedicalService service;

  const _ChiTietDonThuocSheet(
      {required this.don, required this.service});

  @override
  State<_ChiTietDonThuocSheet> createState() =>
      _ChiTietDonThuocSheetState();
}

class _ChiTietDonThuocSheetState extends State<_ChiTietDonThuocSheet> {
  List<ChiTietThuocModel> _chiTiet = [];
  bool _isLoading = true;

  @override
  void initState() {
    super.initState();
    _load();
  }

  Future<void> _load() async {
    final result =
        await widget.service.getChiTietDonThuoc(widget.don.maDonThuoc);
    if (mounted) {
      setState(() {
        _isLoading = false;
        if (result.success) _chiTiet = result.data ?? [];
      });
    }
  }

  String _fmtMoney(double v) =>
      NumberFormat.currency(locale: 'vi_VN', symbol: 'VNĐ').format(v);

  @override
  Widget build(BuildContext context) {
    return DraggableScrollableSheet(
      initialChildSize: 0.7,
      maxChildSize: 0.93,
      minChildSize: 0.4,
      builder: (_, ctrl) => Container(
        decoration: const BoxDecoration(
          color: Colors.white,
          borderRadius: BorderRadius.vertical(top: Radius.circular(24)),
        ),
        child: Column(
          children: [
            // Handle bar
            Container(
              margin: const EdgeInsets.only(top: 12, bottom: 8),
              width: 40,
              height: 4,
              decoration: BoxDecoration(
                color: AppTheme.borderLight,
                borderRadius: BorderRadius.circular(2),
              ),
            ),
            // Header
            Padding(
              padding: const EdgeInsets.fromLTRB(20, 8, 20, 12),
              child: Row(
                children: [
                  const Icon(Icons.medication_rounded,
                      color: AppTheme.primary, size: 22),
                  const SizedBox(width: 10),
                  Expanded(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        const Text(
                          'Chi tiết đơn thuốc',
                          style: TextStyle(
                            fontWeight: FontWeight.w700,
                            fontSize: 16,
                            color: AppTheme.textDark,
                          ),
                        ),
                        if (widget.don.loiDanBS.isNotEmpty)
                          Text(
                            widget.don.loiDanBS,
                            style: const TextStyle(
                                color: AppTheme.textMuted, fontSize: 12),
                          ),
                      ],
                    ),
                  ),
                ],
              ),
            ),
            const Divider(height: 0),
            // Content
            Expanded(
              child: _isLoading
                  ? const Center(child: CircularProgressIndicator())
                  : _chiTiet.isEmpty
                      ? const Center(
                          child: Text('Không có dữ liệu chi tiết'))
                      : ListView.separated(
                          controller: ctrl,
                          padding: const EdgeInsets.all(16),
                          itemCount: _chiTiet.length,
                          separatorBuilder: (_, __) =>
                              const SizedBox(height: 10),
                          itemBuilder: (_, i) =>
                              _buildThuocItem(_chiTiet[i], i + 1),
                        ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildThuocItem(ChiTietThuocModel ct, int stt) {
    return Container(
      padding: const EdgeInsets.all(14),
      decoration: BoxDecoration(
        color: AppTheme.bgLight,
        borderRadius: BorderRadius.circular(12),
        border: Border.all(color: AppTheme.borderLight),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            children: [
              Container(
                width: 24,
                height: 24,
                decoration: const BoxDecoration(
                  color: AppTheme.primary,
                  shape: BoxShape.circle,
                ),
                child: Center(
                  child: Text(
                    '$stt',
                    style: const TextStyle(
                        color: Colors.white,
                        fontSize: 12,
                        fontWeight: FontWeight.w700),
                  ),
                ),
              ),
              const SizedBox(width: 10),
              Expanded(
                child: Text(
                  ct.tenThuoc,
                  style: const TextStyle(
                    fontWeight: FontWeight.w700,
                    fontSize: 14,
                    color: AppTheme.textDark,
                  ),
                ),
              ),
            ],
          ),
          const SizedBox(height: 10),
          // Cách dùng chip
          Container(
            padding:
                const EdgeInsets.symmetric(horizontal: 10, vertical: 5),
            decoration: BoxDecoration(
              color: AppTheme.primary.withOpacity(0.08),
              borderRadius: BorderRadius.circular(8),
            ),
            child: Row(
              mainAxisSize: MainAxisSize.min,
              children: [
                const Icon(Icons.schedule_outlined,
                    size: 14, color: AppTheme.primary),
                const SizedBox(width: 6),
                Text(
                  ct.cachUong,
                  style: const TextStyle(
                      color: AppTheme.primary,
                      fontSize: 12,
                      fontWeight: FontWeight.w600),
                ),
              ],
            ),
          ),
          const SizedBox(height: 8),
          Row(
            children: [
              _miniTag('${ct.soNgayDung} ngày', Icons.calendar_today_outlined),
              const SizedBox(width: 8),
              _miniTag('${ct.soLuong} ${ct.donViCoBan.isNotEmpty ? ct.donViCoBan : ct.donViTinh}',
                  Icons.inventory_2_outlined),
              if (ct.donGia > 0) ...[
                const SizedBox(width: 8),
                _miniTag(_fmtMoney(ct.donGia * ct.soLuong),
                    Icons.payments_outlined),
              ],
            ],
          ),
          if (ct.ghiChu.isNotEmpty) ...[
            const SizedBox(height: 8),
            Text(
              '💬 ${ct.ghiChu}',
              style: const TextStyle(
                  color: AppTheme.textMuted,
                  fontSize: 12,
                  fontStyle: FontStyle.italic),
            ),
          ],
        ],
      ),
    );
  }

  Widget _miniTag(String label, IconData icon) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(6),
        border: Border.all(color: AppTheme.borderLight),
      ),
      child: Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          Icon(icon, size: 12, color: AppTheme.textMuted),
          const SizedBox(width: 4),
          Text(label,
              style: const TextStyle(
                  color: AppTheme.textBody,
                  fontSize: 11,
                  fontWeight: FontWeight.w500)),
        ],
      ),
    );
  }
}
