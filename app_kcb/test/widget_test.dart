import 'package:flutter_test/flutter_test.dart';
import 'package:app_kcb/main.dart';

void main() {
  testWidgets('MedicHub app smoke test', (WidgetTester tester) async {
    await tester.pumpWidget(const MedicHubApp());
    expect(find.byType(MedicHubApp), findsOneWidget);
  });
}
