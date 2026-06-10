import 'package:flutter/material.dart';
import 'package:shimmer/shimmer.dart';
import '../utils/theme/app_theme.dart';

class StatusBadge extends StatelessWidget {
  final String? status;
  final double fontSize;

  const StatusBadge({super.key, this.status, this.fontSize = 12});

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 3),
      decoration: BoxDecoration(
        color: StatusColors.getColor(status).withOpacity(0.12),
        borderRadius: BorderRadius.circular(6),
      ),
      child: Text(
        _formatStatus(status),
        style: TextStyle(
          color: StatusColors.getColor(status),
          fontSize: fontSize,
          fontWeight: FontWeight.w500,
        ),
      ),
    );
  }

  String _formatStatus(String? status) {
    if (status == null || status.isEmpty) return '-';
    return status
        .replaceAllMapped(RegExp(r'([A-Z])'), (m) => ' ${m.group(1)}')
        .trim()
        .split(' ')
        .map((w) => w.isNotEmpty ? '${w[0].toUpperCase()}${w.substring(1).toLowerCase()}' : '')
        .join(' ');
  }
}
