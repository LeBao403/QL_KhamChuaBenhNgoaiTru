import 'package:flutter/material.dart';

import '../../core/theme/app_theme.dart';
import '../booking/booking_screen.dart';

class GuideScreen extends StatefulWidget {
  const GuideScreen({super.key});

  @override
  State<GuideScreen> createState() => _GuideScreenState();
}

class _GuideScreenState extends State<GuideScreen> {
  final TextEditingController _searchController = TextEditingController();

  final List<_GuideStep> _steps = const [
    _GuideStep(
        '1. Đặt lịch',
        'Đặt lịch trước qua ứng dụng để chủ động chọn ngày và khung giờ.',
        Icons.calendar_month_rounded),
    _GuideStep(
        '2. Tiếp nhận',
        'Mang CCCD hoặc BHYT để xác nhận thông tin tại quầy tiếp đón.',
        Icons.badge_rounded),
    _GuideStep(
        '3. Khám lâm sàng',
        'Bác sĩ chuyên khoa thăm khám, đánh giá triệu chứng và chỉ định cần thiết.',
        Icons.health_and_safety_rounded),
    _GuideStep(
        '4. Cận lâm sàng',
        'Thực hiện xét nghiệm, siêu âm hoặc chẩn đoán hình ảnh nếu có chỉ định.',
        Icons.monitor_heart_rounded),
    _GuideStep(
        '5. Nhận kết quả',
        'Thanh toán, nhận thuốc và lời dặn của bác sĩ trước khi ra về.',
        Icons.medication_rounded),
  ];

  final List<_GuidePrep> _preps = const [
    _GuidePrep(
      'Xét nghiệm máu',
      'Nhịn ăn từ 8 đến 12 tiếng trước khi lấy máu. Chỉ nên uống nước lọc.',
      Icons.bloodtype_rounded,
      AppTheme.danger,
    ),
    _GuidePrep(
      'Siêu âm',
      'Siêu âm ổ bụng thường cần nhịn ăn hoặc nhịn tiểu theo hướng dẫn của nhân viên y tế.',
      Icons.graphic_eq_rounded,
      AppTheme.info,
    ),
    _GuidePrep(
      'Nội soi',
      'Nhịn ăn tối thiểu 6 tiếng. Nếu nội soi gây mê, nên có người thân đi cùng.',
      Icons.vaccines_rounded,
      Color(0xFFB45309),
    ),
  ];

  final List<_FaqItem> _faqs = const [
    _FaqItem('Tôi có cần đặt lịch trước khi đến khám không?',
        'Bạn nên đặt lịch trước để chọn được khung giờ phù hợp và giảm thời gian chờ tại bệnh viện.'),
    _FaqItem('Đi khám cần mang theo giấy tờ gì?',
        'Bạn nên mang CCCD, BHYT còn hiệu lực, các kết quả khám cũ và toa thuốc đang sử dụng nếu có.'),
    _FaqItem('BHYT có áp dụng khi đặt lịch trên app không?',
        'Có. Phí đặt lịch giữ chỗ là khoản riêng, còn quyền lợi BHYT vẫn được áp dụng trong quá trình khám theo quy định.'),
    _FaqItem('Tôi có thể hủy lịch đã đặt không?',
        'Bạn có thể hủy lịch khi lịch vẫn ở trạng thái chờ xử lý hoặc chưa hoàn tất quy trình tiếp nhận.'),
    _FaqItem('Nếu không biết nên khám chuyên khoa nào thì sao?',
        'Bạn có thể xem mục Chuyên khoa, gọi tổng đài tư vấn hoặc chọn khám tổng quát để được định hướng phù hợp.'),
  ];

  final List<_GuideArticle> _articles = const [
    _GuideArticle(
      'Chuẩn bị trước khi đi khám để tiết kiệm thời gian',
      'Ưu tiên đi đúng giờ, mang đủ giấy tờ và ghi sẵn triệu chứng cần trao đổi với bác sĩ.',
      'Cẩm nang người bệnh',
    ),
    _GuideArticle(
      'Những câu hỏi nên hỏi bác sĩ trong buổi khám',
      'Hãy hỏi rõ về chẩn đoán, hướng điều trị, thời gian tái khám và các dấu hiệu cần theo dõi.',
      'Hướng dẫn khám bệnh',
    ),
    _GuideArticle(
      'Cách theo dõi sức khỏe sau khi dùng thuốc',
      'Đọc kỹ hướng dẫn dùng thuốc, ghi nhận tác dụng phụ bất thường và tái khám theo hẹn.',
      'Sau khám',
    ),
  ];

  List<_FaqItem> get _filteredFaqs {
    final query = _searchController.text.trim().toLowerCase();
    if (query.isEmpty) return _faqs;
    return _faqs.where((item) {
      return item.question.toLowerCase().contains(query) ||
          item.answer.toLowerCase().contains(query);
    }).toList();
  }

  @override
  void dispose() {
    _searchController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppTheme.bgLight,
      resizeToAvoidBottomInset: false,
      appBar: AppBar(title: const Text('Hướng dẫn')),
      body: ListView(
        padding: const EdgeInsets.fromLTRB(16, 12, 16, 24),
        children: [
          _buildHero(),
          const SizedBox(height: 20),
          _buildSectionTitle(
              'Quy trình khám bệnh', 'Các bước trên app và tại bệnh viện'),
          const SizedBox(height: 12),
          ..._steps.map(_buildStepCard),
          const SizedBox(height: 24),
          _buildSectionTitle(
              'Lưu ý trước khi khám', 'Chuẩn bị đúng để kết quả chính xác hơn'),
          const SizedBox(height: 12),
          ..._preps.map(_buildPrepCard),
          const SizedBox(height: 24),
          _buildSectionTitle(
              'Câu hỏi thường gặp', 'Tra cứu nhanh những thắc mắc phổ biến'),
          const SizedBox(height: 12),
          TextField(
            controller: _searchController,
            onChanged: (_) => setState(() {}),
            decoration: const InputDecoration(
              hintText: 'Tìm câu hỏi về BHYT, đặt lịch, nhịn ăn...',
              prefixIcon: Icon(Icons.search_rounded),
            ),
          ),
          const SizedBox(height: 12),
          ..._filteredFaqs.map(_buildFaqItem),
          const SizedBox(height: 24),
          _buildSectionTitle(
              'Cẩm nang sức khỏe', 'Thông tin nên đọc trước và sau khi khám'),
          const SizedBox(height: 12),
          ..._articles.map(_buildArticleCard),
          const SizedBox(height: 24),
          Container(
            padding: const EdgeInsets.all(20),
            decoration: BoxDecoration(
              gradient: AppTheme.heroGradient,
              borderRadius: BorderRadius.circular(24),
            ),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                const Text(
                  'Bạn cần hỗ trợ đặt lịch?',
                  style: TextStyle(
                    color: Colors.white,
                    fontSize: 20,
                    fontWeight: FontWeight.w800,
                  ),
                ),
                const SizedBox(height: 8),
                const Text(
                  'Đặt lịch ngay trên ứng dụng để chủ động thời gian và thanh toán QR ngân hàng nhanh chóng.',
                  style: TextStyle(color: Colors.white, height: 1.5),
                ),
                const SizedBox(height: 16),
                ElevatedButton.icon(
                  onPressed: () {
                    Navigator.push(
                      context,
                      MaterialPageRoute(builder: (_) => const BookingScreen()),
                    );
                  },
                  style: ElevatedButton.styleFrom(
                    backgroundColor: Colors.white,
                    foregroundColor: AppTheme.primary,
                  ),
                  icon: const Icon(Icons.calendar_month_rounded),
                  label: const Text('Đặt lịch khám'),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildHero() {
    return Container(
      padding: const EdgeInsets.all(20),
      decoration: BoxDecoration(
        gradient: const LinearGradient(
          colors: [AppTheme.primaryDeep, AppTheme.primary],
          begin: Alignment.topLeft,
          end: Alignment.bottomRight,
        ),
        borderRadius: BorderRadius.circular(24),
      ),
      child: const Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            'Hướng dẫn & cẩm nang y khoa',
            style: TextStyle(
              color: Colors.white,
              fontSize: 24,
              fontWeight: FontWeight.w800,
            ),
          ),
          SizedBox(height: 10),
          Text(
            'Mọi thông tin cần biết trước, trong và sau khi khám đều được gom lại ở đây để bạn dễ tra cứu trên điện thoại.',
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

  Widget _buildSectionTitle(String title, String subtitle) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          title,
          style: const TextStyle(
            fontSize: 18,
            fontWeight: FontWeight.w800,
            color: AppTheme.textDark,
          ),
        ),
        const SizedBox(height: 4),
        Text(
          subtitle,
          style: const TextStyle(color: AppTheme.textMuted),
        ),
      ],
    );
  }

  Widget _buildStepCard(_GuideStep step) {
    return Container(
      margin: const EdgeInsets.only(bottom: 12),
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(20),
        border: Border.all(color: AppTheme.borderLight),
      ),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Container(
            width: 48,
            height: 48,
            decoration: BoxDecoration(
              color: AppTheme.primary.withOpacity(0.1),
              borderRadius: BorderRadius.circular(14),
            ),
            child: Icon(step.icon, color: AppTheme.primary),
          ),
          const SizedBox(width: 14),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  step.title,
                  style: const TextStyle(
                    fontWeight: FontWeight.w700,
                    color: AppTheme.textDark,
                  ),
                ),
                const SizedBox(height: 6),
                Text(step.description),
              ],
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildPrepCard(_GuidePrep prep) {
    return Container(
      margin: const EdgeInsets.only(bottom: 12),
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(20),
        border: Border(
          left: BorderSide(color: prep.color, width: 4),
          top: const BorderSide(color: AppTheme.borderLight),
          right: const BorderSide(color: AppTheme.borderLight),
          bottom: const BorderSide(color: AppTheme.borderLight),
        ),
      ),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Icon(prep.icon, color: prep.color),
          const SizedBox(width: 12),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  prep.title,
                  style: const TextStyle(
                    fontWeight: FontWeight.w700,
                    color: AppTheme.textDark,
                  ),
                ),
                const SizedBox(height: 6),
                Text(prep.description),
              ],
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildFaqItem(_FaqItem faq) {
    return Container(
      margin: const EdgeInsets.only(bottom: 12),
      child: ExpansionTile(
        collapsedBackgroundColor: Colors.white,
        backgroundColor: Colors.white,
        collapsedShape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(18),
          side: const BorderSide(color: AppTheme.borderLight),
        ),
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(18),
          side: const BorderSide(color: AppTheme.borderLight),
        ),
        title: Text(
          faq.question,
          style: const TextStyle(
            fontWeight: FontWeight.w700,
            color: AppTheme.textDark,
          ),
        ),
        childrenPadding: const EdgeInsets.fromLTRB(16, 0, 16, 16),
        children: [
          Text(
            faq.answer,
            style: const TextStyle(height: 1.6),
          ),
        ],
      ),
    );
  }

  Widget _buildArticleCard(_GuideArticle article) {
    return Container(
      margin: const EdgeInsets.only(bottom: 12),
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(20),
        border: Border.all(color: AppTheme.borderLight),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Container(
            padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 6),
            decoration: BoxDecoration(
              color: AppTheme.info.withOpacity(0.12),
              borderRadius: BorderRadius.circular(999),
            ),
            child: Text(
              article.category,
              style: const TextStyle(
                color: AppTheme.primary,
                fontWeight: FontWeight.w700,
                fontSize: 12,
              ),
            ),
          ),
          const SizedBox(height: 10),
          Text(
            article.title,
            style: const TextStyle(
              fontSize: 16,
              fontWeight: FontWeight.w700,
              color: AppTheme.textDark,
            ),
          ),
          const SizedBox(height: 6),
          Text(article.summary),
        ],
      ),
    );
  }
}

class _GuideStep {
  const _GuideStep(this.title, this.description, this.icon);

  final String title;
  final String description;
  final IconData icon;
}

class _GuidePrep {
  const _GuidePrep(this.title, this.description, this.icon, this.color);

  final String title;
  final String description;
  final IconData icon;
  final Color color;
}

class _FaqItem {
  const _FaqItem(this.question, this.answer);

  final String question;
  final String answer;
}

class _GuideArticle {
  const _GuideArticle(this.title, this.summary, this.category);

  final String title;
  final String summary;
  final String category;
}
