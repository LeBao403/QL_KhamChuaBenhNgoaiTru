import 'package:flutter/material.dart';
import '../../core/theme/app_theme.dart';

class ClinicBrandLogo extends StatelessWidget {
  static const String assetPath = 'assets/images/clinic_logo.png';

  final double size;
  final double imagePadding;
  final double borderRadius;
  final Color? backgroundColor;
  final Color borderColor;
  final List<BoxShadow>? boxShadow;

  const ClinicBrandLogo({
    super.key,
    this.size = 72,
    this.imagePadding = 8,
    this.borderRadius = 999,
    this.backgroundColor,
    this.borderColor = AppTheme.primary,
    this.boxShadow,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      width: size,
      height: size,
      padding: EdgeInsets.all(imagePadding),
      decoration: BoxDecoration(
        color: backgroundColor ?? Colors.white,
        borderRadius: BorderRadius.circular(borderRadius),
        border: Border.all(color: borderColor, width: 2),
        boxShadow: boxShadow ??
            [
              BoxShadow(
                color: AppTheme.primary.withOpacity(0.18),
                blurRadius: 14,
                offset: const Offset(0, 6),
              ),
            ],
      ),
      child: ClipRRect(
        borderRadius: BorderRadius.circular(borderRadius),
        child: Image.asset(
          assetPath,
          fit: BoxFit.contain,
          errorBuilder: (context, error, stackTrace) {
            return const Icon(
              Icons.local_hospital_rounded,
              color: AppTheme.primary,
            );
          },
        ),
      ),
    );
  }
}

class AppGradientBackground extends StatelessWidget {
  final Gradient gradient;
  final Widget child;
  const AppGradientBackground(
      {super.key, required this.gradient, required this.child});

  @override
  Widget build(BuildContext context) {
    return Container(
      decoration: BoxDecoration(gradient: gradient),
      child: child,
    );
  }
}

class SectionTitle extends StatelessWidget {
  final String subtitle;
  final String title;
  final CrossAxisAlignment alignment;
  const SectionTitle({
    super.key,
    required this.subtitle,
    required this.title,
    this.alignment = CrossAxisAlignment.center,
  });

  @override
  Widget build(BuildContext context) {
    return Column(
      crossAxisAlignment: alignment,
      children: [
        Text(
          subtitle,
          style: const TextStyle(
            color: AppTheme.primary,
            fontSize: 13,
            fontWeight: FontWeight.w600,
            letterSpacing: 1.2,
          ),
        ),
        const SizedBox(height: 6),
        Text(
          title,
          style: const TextStyle(
            color: AppTheme.textDark,
            fontSize: 22,
            fontWeight: FontWeight.w700,
          ),
          textAlign:
              alignment == CrossAxisAlignment.center ? TextAlign.center : TextAlign.start,
        ),
        if (alignment == CrossAxisAlignment.center) ...[
          const SizedBox(height: 10),
          Container(
            width: 48,
            height: 3,
            decoration: BoxDecoration(
              gradient: AppTheme.heroGradient,
              borderRadius: BorderRadius.circular(2),
            ),
          ),
        ],
      ],
    );
  }
}

class AppCard extends StatelessWidget {
  final Widget child;
  final EdgeInsetsGeometry? padding;
  final VoidCallback? onTap;
  final Color? color;
  const AppCard(
      {super.key, required this.child, this.padding, this.onTap, this.color});

  @override
  Widget build(BuildContext context) {
    return Material(
      color: color ?? AppTheme.surfaceCard,
      borderRadius: BorderRadius.circular(16),
      clipBehavior: Clip.antiAlias,
      child: InkWell(
        onTap: onTap,
        borderRadius: BorderRadius.circular(16),
        child: Container(
          decoration: BoxDecoration(
            border: Border.all(color: AppTheme.borderLight),
            borderRadius: BorderRadius.circular(16),
          ),
          padding: padding ?? const EdgeInsets.all(16),
          child: child,
        ),
      ),
    );
  }
}

class GradientButton extends StatelessWidget {
  final String label;
  final IconData? icon;
  final VoidCallback? onPressed;
  final Gradient gradient;
  final bool isLoading;
  const GradientButton({
    super.key,
    required this.label,
    this.icon,
    this.onPressed,
    this.gradient = AppTheme.heroGradient,
    this.isLoading = false,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      decoration: BoxDecoration(
        gradient: onPressed != null ? gradient : null,
        color: onPressed == null ? AppTheme.borderMedium : null,
        borderRadius: BorderRadius.circular(12),
        boxShadow: onPressed != null
            ? [
                BoxShadow(
                  color: AppTheme.primary.withOpacity(0.3),
                  blurRadius: 12,
                  offset: const Offset(0, 4),
                )
              ]
            : null,
      ),
      child: Material(
        color: Colors.transparent,
        borderRadius: BorderRadius.circular(12),
        child: InkWell(
          onTap: isLoading ? null : onPressed,
          borderRadius: BorderRadius.circular(12),
          child: Padding(
            padding:
                const EdgeInsets.symmetric(horizontal: 24, vertical: 14),
            child: isLoading
                ? const SizedBox(
                    height: 20,
                    width: 20,
                    child: CircularProgressIndicator(
                        color: Colors.white, strokeWidth: 2))
                : Row(
                    mainAxisSize: MainAxisSize.min,
                    mainAxisAlignment: MainAxisAlignment.center,
                    children: [
                      if (icon != null) ...[
                        Icon(icon, color: Colors.white, size: 18),
                        const SizedBox(width: 8),
                      ],
                      Text(
                        label,
                        style: const TextStyle(
                          color: Colors.white,
                          fontSize: 15,
                          fontWeight: FontWeight.w600,
                        ),
                      ),
                    ],
                  ),
          ),
        ),
      ),
    );
  }
}

class InfoBadge extends StatelessWidget {
  final String label;
  final Color color;
  final IconData? icon;
  const InfoBadge(
      {super.key, required this.label, required this.color, this.icon});

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 4),
      decoration: BoxDecoration(
        color: color.withOpacity(0.12),
        borderRadius: BorderRadius.circular(50),
        border: Border.all(color: color.withOpacity(0.3)),
      ),
      child: Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          if (icon != null) ...[
            Icon(icon, color: color, size: 12),
            const SizedBox(width: 4),
          ],
          Text(
            label,
            style: TextStyle(
                color: color, fontSize: 12, fontWeight: FontWeight.w600),
          ),
        ],
      ),
    );
  }
}

class TimeSlotChip extends StatelessWidget {
  final String time;
  final bool isSelected;
  final bool isDisabled;
  final String status;
  final VoidCallback? onTap;
  const TimeSlotChip({
    super.key,
    required this.time,
    required this.isSelected,
    required this.isDisabled,
    required this.status,
    this.onTap,
  });

  @override
  Widget build(BuildContext context) {
    Color borderColor = isSelected
        ? AppTheme.primary
        : isDisabled
            ? AppTheme.borderLight
            : AppTheme.borderMedium;
    Color bg = isSelected
        ? const Color(0xFFEFF6FF)
        : isDisabled
            ? const Color(0xFFF1F5F9)
            : Colors.white;

    return GestureDetector(
      onTap: isDisabled ? null : onTap,
      child: AnimatedContainer(
        duration: const Duration(milliseconds: 200),
        padding:
            const EdgeInsets.symmetric(horizontal: 12, vertical: 10),
        decoration: BoxDecoration(
          color: bg,
          border: Border.all(
            color: borderColor,
            width: isSelected ? 2 : 1,
          ),
          borderRadius: BorderRadius.circular(10),
          boxShadow: isSelected
              ? [
                  BoxShadow(
                    color: AppTheme.primary.withOpacity(0.15),
                    blurRadius: 8,
                    offset: const Offset(0, 2),
                  )
                ]
              : null,
        ),
        child: Column(
          children: [
            Text(
              time,
              style: TextStyle(
                fontSize: 13,
                fontWeight: FontWeight.w700,
                color: isDisabled
                    ? AppTheme.textLight
                    : isSelected
                        ? AppTheme.primary
                        : AppTheme.textDark,
              ),
            ),
            const SizedBox(height: 4),
            Container(
              padding: const EdgeInsets.symmetric(horizontal: 6, vertical: 2),
              decoration: BoxDecoration(
                color: isDisabled
                    ? AppTheme.textLight.withOpacity(0.15)
                    : status == 'Khả dụng'
                        ? AppTheme.secondary.withOpacity(0.12)
                        : AppTheme.danger.withOpacity(0.12),
                borderRadius: BorderRadius.circular(4),
              ),
              child: Text(
                status,
                style: TextStyle(
                  fontSize: 10,
                  fontWeight: FontWeight.w600,
                  color: isDisabled
                      ? AppTheme.textLight
                      : status == 'Khả dụng'
                          ? AppTheme.secondary
                          : AppTheme.danger,
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }
}
