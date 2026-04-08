import 'package:flutter/material.dart';

import '../../core/models/discovery_models.dart';
import '../../core/services/discovery_service.dart';
import '../../core/theme/app_theme.dart';
import '../booking/booking_screen.dart';
import '../doctors/doctors_screen.dart';

class SpecialtiesScreen extends StatefulWidget {
  const SpecialtiesScreen({super.key});

  @override
  State<SpecialtiesScreen> createState() => _SpecialtiesScreenState();
}

class _SpecialtiesScreenState extends State<SpecialtiesScreen> {
  final DiscoveryService _service = DiscoveryService();
  final TextEditingController _searchController = TextEditingController();

  List<SpecialtyModel> _specialties = const [];
  bool _isLoading = true;
  String? _error;

  @override
  void initState() {
    super.initState();
    _loadSpecialties();
  }

  @override
  void dispose() {
    _searchController.dispose();
    super.dispose();
  }

  Future<void> _loadSpecialties() async {
    setState(() {
      _isLoading = true;
      _error = null;
    });

    final result = await _service.getSpecialties();
    if (!mounted) return;

    setState(() {
      _isLoading = false;
      if (result.success) {
        _specialties = result.data ?? const [];
      } else {
        _error = result.message;
      }
    });
  }

  List<SpecialtyModel> get _filteredSpecialties {
    final query = _searchController.text.trim().toLowerCase();
    if (query.isEmpty) return _specialties;
    return _specialties.where((item) {
      return item.tenKhoa.toLowerCase().contains(query) ||
          item.moTa.toLowerCase().contains(query);
    }).toList();
  }

  IconData _iconForSpecialty(String name) {
    final text = name.toLowerCase();
    if (text.contains('nhi')) return Icons.child_care_rounded;
    if (text.contains('sản')) return Icons.pregnant_woman_rounded;
    if (text.contains('răng')) return Icons.sentiment_very_satisfied_rounded;
    if (text.contains('tai mũi họng')) return Icons.hearing_rounded;
    if (text.contains('mắt')) return Icons.visibility_rounded;
    if (text.contains('ngoại')) return Icons.healing_rounded;
    return Icons.local_hospital_rounded;
  }

  Color _colorForIndex(int index) {
    const colors = [
      AppTheme.primary,
      AppTheme.secondary,
      Color(0xFFB45309),
      Color(0xFF7C3AED),
      Color(0xFF0F766E),
      AppTheme.danger,
    ];
    return colors[index % colors.length];
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppTheme.bgLight,
      resizeToAvoidBottomInset: false,
      appBar: AppBar(title: const Text('Chuyên khoa')),
      body: RefreshIndicator(
        onRefresh: _loadSpecialties,
        child: ListView(
          physics: const AlwaysScrollableScrollPhysics(),
          padding: const EdgeInsets.fromLTRB(16, 12, 16, 24),
          children: [
            _buildHero(),
            const SizedBox(height: 16),
            TextField(
              controller: _searchController,
              onChanged: (_) => setState(() {}),
              decoration: const InputDecoration(
                hintText: 'Tìm chuyên khoa, ví dụ: Nội, Nhi, Răng Hàm Mặt...',
                prefixIcon: Icon(Icons.search_rounded),
              ),
            ),
            const SizedBox(height: 16),
            if (_isLoading)
              const Padding(
                padding: EdgeInsets.only(top: 60),
                child: Center(child: CircularProgressIndicator()),
              )
            else if (_error != null)
              _buildError()
            else
              ..._buildSpecialtyList(),
          ],
        ),
      ),
    );
  }

  Widget _buildHero() {
    return Container(
      padding: const EdgeInsets.all(20),
      decoration: BoxDecoration(
        gradient: AppTheme.greenGradient,
        borderRadius: BorderRadius.circular(24),
      ),
      child: const Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            'Khám phá chuyên khoa',
            style: TextStyle(
              color: Colors.white,
              fontSize: 24,
              fontWeight: FontWeight.w800,
            ),
          ),
          SizedBox(height: 10),
          Text(
            'Tra cứu thông tin chuyên khoa, xem bác sĩ liên quan và chuyển nhanh sang đặt lịch khám.',
            style: TextStyle(
              color: Colors.white,
              fontSize: 14,
              height: 1.5,
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildError() {
    return Container(
      padding: const EdgeInsets.all(20),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(20),
        border: Border.all(color: AppTheme.borderLight),
      ),
      child: Column(
        children: [
          const Icon(Icons.error_outline_rounded,
              color: AppTheme.danger, size: 42),
          const SizedBox(height: 12),
          Text(_error ?? 'Không tải được danh sách chuyên khoa.'),
          const SizedBox(height: 12),
          ElevatedButton(
            onPressed: _loadSpecialties,
            child: const Text('Thử lại'),
          ),
        ],
      ),
    );
  }

  List<Widget> _buildSpecialtyList() {
    final items = _filteredSpecialties;
    if (items.isEmpty) {
      return [
        Container(
          padding: const EdgeInsets.all(24),
          decoration: BoxDecoration(
            color: Colors.white,
            borderRadius: BorderRadius.circular(20),
            border: Border.all(color: AppTheme.borderLight),
          ),
          child: const Column(
            children: [
              Icon(Icons.medical_services_outlined,
                  size: 44, color: AppTheme.textMuted),
              SizedBox(height: 12),
              Text(
                'Không tìm thấy chuyên khoa phù hợp.',
                style: TextStyle(fontWeight: FontWeight.w600),
              ),
            ],
          ),
        ),
      ];
    }

    return items.asMap().entries.map((entry) {
      final index = entry.key;
      final specialty = entry.value;
      final color = _colorForIndex(index);

      return Container(
        margin: const EdgeInsets.only(bottom: 12),
        padding: const EdgeInsets.all(18),
        decoration: BoxDecoration(
          color: Colors.white,
          borderRadius: BorderRadius.circular(22),
          border: Border.all(color: AppTheme.borderLight),
          boxShadow: const [
            BoxShadow(
              color: Color(0x0F0F172A),
              blurRadius: 18,
              offset: Offset(0, 8),
            ),
          ],
        ),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Container(
              width: 54,
              height: 54,
              decoration: BoxDecoration(
                color: color.withOpacity(0.12),
                borderRadius: BorderRadius.circular(16),
              ),
              child: Icon(_iconForSpecialty(specialty.tenKhoa), color: color),
            ),
            const SizedBox(height: 14),
            Text(
              specialty.tenKhoa,
              style: const TextStyle(
                fontSize: 18,
                fontWeight: FontWeight.w700,
                color: AppTheme.textDark,
              ),
            ),
            const SizedBox(height: 8),
            Text(
              specialty.moTa.isEmpty
                  ? 'Chuyên khoa được đầu tư đồng bộ để hỗ trợ chẩn đoán và điều trị.'
                  : specialty.moTa,
              style: const TextStyle(
                color: AppTheme.textBody,
                height: 1.5,
              ),
            ),
            const SizedBox(height: 16),
            Row(
              children: [
                Expanded(
                  child: OutlinedButton.icon(
                    onPressed: () {
                      Navigator.push(
                        context,
                        MaterialPageRoute(
                          builder: (_) => DoctorsScreen(
                            initialSpecialty: specialty.tenKhoa,
                          ),
                        ),
                      );
                    },
                    icon: const Icon(Icons.people_alt_outlined, size: 18),
                    label: const Text('Xem bác sĩ'),
                  ),
                ),
                const SizedBox(width: 10),
                Expanded(
                  child: ElevatedButton.icon(
                    onPressed: () {
                      Navigator.push(
                        context,
                        MaterialPageRoute(
                          builder: (_) => const BookingScreen(),
                        ),
                      );
                    },
                    icon: const Icon(Icons.calendar_month_rounded, size: 18),
                    label: const Text('Đặt khám'),
                  ),
                ),
              ],
            ),
          ],
        ),
      );
    }).toList();
  }
}
