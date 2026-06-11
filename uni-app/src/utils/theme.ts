/**
 * 主题配置
 * 与 Flutter lib/utils/theme/app_theme.dart 对齐
 * 统一颜色: primary #667eea, primaryDark #764ba2
 */

export const theme = {
  // 主题色
  primary: '#667eea',
  primaryDark: '#764ba2',
  primaryLight: '#8490ff',

  // 功能色
  success: '#67c23a',
  warning: '#e6a23c',
  error: '#f56c6c',
  info: '#909399',

  // 背景色
  bgPage: '#f5f7fa',
  bgCard: '#ffffff',
  bgHover: '#f1f1f1',

  // 文字色
  textPrimary: '#303133',
  textRegular: '#606266',
  textSecondary: '#909399',
  textPlaceholder: '#c0c4cc',

  // 边框
  border: '#dcdfe6',
  borderLight: '#e4e7ed',

  // 状态颜色
  status: {
    pending: '#e6a23c',    // 待处理
    approved: '#67c23a',   // 已通过
    rejected: '#f56c6c',   // 已拒绝
    processing: '#409eff', // 处理中
    completed: '#909399',  // 已完成
    cancelled: '#c0c4cc',  // 已取消
  },

  // 渐变色 (用于登录页等)
  gradient: {
    start: '#667eea',
    end: '#764ba2',
  },

  // 阴影
  shadow: {
    sm: '0 2rpx 8rpx rgba(0,0,0,0.06)',
    base: '0 4rpx 16rpx rgba(0,0,0,0.08)',
    lg: '0 8rpx 32rpx rgba(0,0,0,0.12)',
  },

  // 圆角
  radius: {
    sm: '8rpx',
    base: '16rpx',
    lg: '24rpx',
    full: '9999rpx',
  },
}

export type Theme = typeof theme
