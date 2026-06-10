import 'package:flutter/material.dart';

class AppTheme {
  // 品牌色系 - 紫色渐变
  static const Color primary = Color(0xFF667eea);
  static const Color primaryDark = Color(0xFF764ba2);
  static const Color primaryLight = Color(0xFF8B9AEF);

  static const Color surface = Colors.white;
  static const Color background = Color(0xFFF5F7FA);
  static const Color cardBackground = Colors.white;

  // 状态颜色
  static const Color success = Color(0xFF10B981);
  static const Color warning = Color(0xFFF59E0B);
  static const Color error = Color(0xFFEF4444);
  static const Color info = Color(0xFF3B82F6);

  // 文字颜色
  static const Color textPrimary = Color(0xFF1F2937);
  static const Color textSecondary = Color(0xFF6B7280);
  static const Color textHint = Color(0xFF9CA3AF);

  static ThemeData get light => ThemeData(
        useMaterial3: true,
        colorScheme: ColorScheme.fromSeed(
          seedColor: primary,
          primary: primary,
          secondary: primaryDark,
          surface: surface,
          error: error,
        ),
        scaffoldBackgroundColor: background,
        appBarTheme: const AppBarTheme(
          backgroundColor: primary,
          foregroundColor: Colors.white,
          elevation: 0,
          centerTitle: true,
        ),
        cardTheme: CardThemeData(
          color: cardBackground,
          elevation: 1,
          shape: RoundedRectangleBorder(
            borderRadius: BorderRadius.circular(12),
          ),
        ),
        inputDecorationTheme: InputDecorationTheme(
          filled: true,
          fillColor: Colors.white,
          contentPadding: const EdgeInsets.symmetric(horizontal: 16, vertical: 14),
          border: OutlineInputBorder(
            borderRadius: BorderRadius.circular(10),
            borderSide: const BorderSide(color: Color(0xFFE5E7EB)),
          ),
          enabledBorder: OutlineInputBorder(
            borderRadius: BorderRadius.circular(10),
            borderSide: const BorderSide(color: Color(0xFFE5E7EB)),
          ),
          focusedBorder: OutlineInputBorder(
            borderRadius: BorderRadius.circular(10),
            borderSide: const BorderSide(color: primary, width: 2),
          ),
          errorBorder: OutlineInputBorder(
            borderRadius: BorderRadius.circular(10),
            borderSide: const BorderSide(color: error),
          ),
          hintStyle: const TextStyle(color: textHint),
        ),
        elevatedButtonTheme: ElevatedButtonThemeData(
          style: ElevatedButton.styleFrom(
            backgroundColor: primary,
            foregroundColor: Colors.white,
            padding: const EdgeInsets.symmetric(horizontal: 24, vertical: 14),
            shape: RoundedRectangleBorder(
              borderRadius: BorderRadius.circular(10),
            ),
            elevation: 0,
          ),
        ),
        outlinedButtonTheme: OutlinedButtonThemeData(
          style: OutlinedButton.styleFrom(
            foregroundColor: primary,
            padding: const EdgeInsets.symmetric(horizontal: 24, vertical: 14),
            shape: RoundedRectangleBorder(
              borderRadius: BorderRadius.circular(10),
            ),
            side: const BorderSide(color: primary),
          ),
        ),
        textButtonTheme: TextButtonThemeData(
          style: TextButton.styleFrom(
            foregroundColor: primary,
          ),
        ),
        floatingActionButtonTheme: const FloatingActionButtonThemeData(
          backgroundColor: primary,
          foregroundColor: Colors.white,
          elevation: 2,
        ),
        bottomNavigationBarTheme: const BottomNavigationBarThemeData(
          backgroundColor: Colors.white,
          selectedItemColor: primary,
          unselectedItemColor: textSecondary,
          type: BottomNavigationBarType.fixed,
          elevation: 8,
        ),
        chipTheme: ChipThemeData(
          backgroundColor: primary.withOpacity(0.1),
          labelStyle: const TextStyle(color: primary),
          shape: RoundedRectangleBorder(
            borderRadius: BorderRadius.circular(8),
          ),
        ),
        dividerTheme: const DividerThemeData(
          color: Color(0xFFE5E7EB),
          thickness: 1,
        ),
        snackBarTheme: SnackBarThemeData(
          behavior: SnackBarBehavior.floating,
          shape: RoundedRectangleBorder(
            borderRadius: BorderRadius.circular(8),
          ),
        ),
      );
}

/// 状态颜色映射
class StatusColors {
  static Color getColor(String? status) {
    switch (status?.toLowerCase()) {
      case 'active':
      case 'approved':
      case 'completed':
      case 'returned':
      case 'available':
        return AppTheme.success;
      case 'pending':
      case 'draft':
      case 'processing':
        return AppTheme.warning;
      case 'rejected':
      case 'cancelled':
      case 'failed':
      case 'overdue':
        return AppTheme.error;
      case 'inactive':
      case 'disabled':
        return AppTheme.textSecondary;
      default:
        return AppTheme.info;
    }
  }
}

/// 角色颜色
class RoleColors {
  static Color getColor(String? role) {
    switch (role?.toLowerCase()) {
      case 'super_admin':
      case 'admin':
      case 'lab_admin':
        return AppTheme.error;
      case 'teacher':
        return AppTheme.success;
      case 'student':
        return AppTheme.info;
      default:
        return AppTheme.textSecondary;
    }
  }
}
