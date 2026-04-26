import 'package:flutter/material.dart';

import '../../core/models/discovery_models.dart';
import '../../core/services/discovery_service.dart';
import '../../core/theme/app_theme.dart';
import '../../shared/widgets/common_widgets.dart';

class DoctorsScreen extends StatefulWidget {
  const DoctorsScreen({super.key, this.initialSpecialty});

  final String? initialSpecialty;

  @override
  State<DoctorsScreen> createState() => _DoctorsScreenState();
}

class _DoctorsScreenState extends State<DoctorsScreen> {
  final DiscoveryService _service = DiscoveryService();
  final TextEditingController _searchController = TextEditingController();

  DoctorDirectoryModel? _directory;
  bool _isLoading = true;
  String? _error;
  String _selectedSpecialty = '';

  @override
  void initState() {
    super.initState();
    _selectedSpecialty = widget.initialSpecialty ?? '';
    _loadDoctors();
  }

  @override
  void dispose() {
    _searchController.dispose();
    super.dispose();
  }

  Future<void> _loadDoctors() async {
    setState(() {
      _isLoading = true;
      _error = null;
    });

    final result = await _service.getDoctors(
      searchQuery: _searchController.text.trim(),
      specialty: _selectedSpecialty,
    );

    if (!mounted) return;
    setState(() {
      _isLoading = false;
      if (result.success) {
        _directory = result.data;
      } else {
        _error = result.message;
      }
    });
  }

  @override
  Widget build(BuildContext context) {
    final specialties = <String>[
      '',
      ...?_directory?.specialties,
    ];

    return Scaffold(
      backgroundColor: AppTheme.bgLight,
      resizeToAvoidBottomInset: false,
      appBar: AppBar(
        title: const Text('Tìm bác sĩ'),
      ),
      body: RefreshIndicator(
        onRefresh: _loadDoctors,
        child: ListView(
          physics: const AlwaysScrollableScrollPhysics(),
          padding: const EdgeInsets.fromLTRB(16, 12, 16, 24),
          children: [
            _buildHero(),
            const SizedBox(height: 16),
            _buildSearchBox(specialties),
            const SizedBox(height: 16),
            if (_isLoading)
              const Padding(
                padding: EdgeInsets.only(top: 60),
                child: Center(child: CircularProgressIndicator()),
              )
            else if (_error != null)
              _buildError()
            else
              ..._buildDoctorList(),
          ],
        ),
      ),
    );
  }

  Widget _buildHero() {
    return Container(
      padding: const EdgeInsets.all(20),
      decoration: BoxDecoration(
        gradient: AppTheme.heroGradient,
        borderRadius: BorderRadius.circular(24),
      ),
      child: const Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            'Đội ngũ bác sĩ',
            style: TextStyle(
              color: Colors.white,
              fontSize: 24,
              fontWeight: FontWeight.w800,
            ),
          ),
          SizedBox(height: 10),
          Text(
            'Tìm theo tên hoặc chuyên khoa để chọn hướng khám phù hợp trước khi đặt lịch khám.',
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

  Widget _buildSearchBox(List<String> specialties) {
    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(20),
        border: Border.all(color: AppTheme.borderLight),
      ),
      child: Column(
        children: [
          TextField(
            controller: _searchController,
            textInputAction: TextInputAction.search,
            onSubmitted: (_) => _loadDoctors(),
            decoration: InputDecoration(
              hintText: 'Nhập tên bác sĩ...',
              prefixIcon: const Icon(Icons.search_rounded),
              suffixIcon: IconButton(
                onPressed: _loadDoctors,
                icon: const Icon(Icons.arrow_forward_rounded),
              ),
            ),
          ),
          const SizedBox(height: 12),
          DropdownButtonFormField<String>(
            key: ValueKey(_selectedSpecialty),
            initialValue: specialties.contains(_selectedSpecialty)
                ? _selectedSpecialty
                : '',
            decoration: const InputDecoration(
              labelText: 'Chuyên khoa',
              prefixIcon: Icon(Icons.medical_services_outlined),
            ),
            items: specialties
                .map(
                  (item) => DropdownMenuItem<String>(
                    value: item,
                    child: Text(item.isEmpty ? 'Tất cả chuyên khoa' : item),
                  ),
                )
                .toList(),
            onChanged: (value) {
              setState(() => _selectedSpecialty = value ?? '');
              _loadDoctors();
            },
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
          const Icon(
            Icons.error_outline_rounded,
            color: AppTheme.danger,
            size: 42,
          ),
          const SizedBox(height: 12),
          Text(
            _error ?? 'Không tải được danh sách bác sĩ.',
            textAlign: TextAlign.center,
          ),
          const SizedBox(height: 12),
          ElevatedButton(
            onPressed: _loadDoctors,
            child: const Text('Thử lại'),
          ),
        ],
      ),
    );
  }

  List<Widget> _buildDoctorList() {
    final doctors = _directory?.doctors ?? const <DoctorModel>[];
    if (doctors.isEmpty) {
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
              Icon(
                Icons.person_search_rounded,
                size: 44,
                color: AppTheme.textMuted,
              ),
              SizedBox(height: 12),
              Text(
                'Không tìm thấy bác sĩ phù hợp.',
                style: TextStyle(fontWeight: FontWeight.w600),
              ),
            ],
          ),
        ),
      ];
    }

    return [
      Text(
        'Tìm thấy ${_directory?.totalItems ?? doctors.length} bác sĩ',
        style: const TextStyle(
          fontSize: 15,
          fontWeight: FontWeight.w700,
          color: AppTheme.textDark,
        ),
      ),
      const SizedBox(height: 12),
      ...doctors.map(_buildDoctorCard),
    ];
  }

  Widget _buildDoctorCard(DoctorModel doctor) {
    return Container(
      margin: const EdgeInsets.only(bottom: 12),
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(20),
        border: Border.all(color: AppTheme.borderLight),
        boxShadow: const [
          BoxShadow(
            color: Color(0x0F0F172A),
            blurRadius: 18,
            offset: Offset(0, 8),
          ),
        ],
      ),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          DoctorAvatar(
            name: doctor.hoTen,
            imagePath: doctor.hinhAnh,
            radius: 30,
          ),
          const SizedBox(width: 14),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  doctor.hoTen,
                  style: const TextStyle(
                    fontSize: 16,
                    fontWeight: FontWeight.w700,
                    color: AppTheme.textDark,
                  ),
                ),
                const SizedBox(height: 4),
                Text(
                  doctor.chucDanh,
                  style: const TextStyle(
                    color: AppTheme.textMuted,
                    fontSize: 13,
                  ),
                ),
                const SizedBox(height: 8),
                Container(
                  padding:
                      const EdgeInsets.symmetric(horizontal: 10, vertical: 6),
                  decoration: BoxDecoration(
                    color: AppTheme.secondary.withOpacity(0.12),
                    borderRadius: BorderRadius.circular(999),
                  ),
                  child: Text(
                    doctor.chuyenKhoa,
                    style: const TextStyle(
                      color: AppTheme.secondary,
                      fontWeight: FontWeight.w700,
                      fontSize: 12,
                    ),
                  ),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}
