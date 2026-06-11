/**
 * 数字格式化工具
 * 与 Flutter lib/utils/num_utils.dart 对齐
 */

/**
 * 安全转换为整数
 */
export function safeInt(value: unknown, fallback = 0): number {
  if (value === null || value === undefined) return fallback
  const n = Number(value)
  return isNaN(n) ? fallback : Math.floor(n)
}

/**
 * 安全转换为浮点数
 */
export function safeFloat(value: unknown, fallback = 0.0): number {
  if (value === null || value === undefined) return fallback
  const n = Number(value)
  return isNaN(n) ? fallback : n
}

/**
 * 格式化数字 (千分位)
 */
export function formatNumber(n: number | string | null | undefined): string {
  if (n === null || n === undefined) return '0'
  const num = typeof n === 'string' ? parseFloat(n) : n
  if (isNaN(num)) return '0'
  return num.toLocaleString('zh-CN')
}

/**
 * 保留小数位
 */
export function toFixed(num: number | string | null | undefined, decimals = 2): string {
  if (num === null || num === undefined) return '0'
  const n = typeof num === 'string' ? parseFloat(num) : num
  if (isNaN(n)) return '0'
  return n.toFixed(decimals)
}

/**
 * 百分比格式化
 */
export function formatPercent(value: number | string | null | undefined, decimals = 1): string {
  if (value === null || value === undefined) return '0%'
  const n = typeof value === 'string' ? parseFloat(value) : value
  if (isNaN(n)) return '0%'
  return (n * 100).toFixed(decimals) + '%'
}

/**
 * 字节大小格式化 (B, KB, MB, GB)
 */
export function formatBytes(bytes: number | string | null | undefined): string {
  if (bytes === null || bytes === undefined || bytes === 0) return '0 B'
  const n = typeof bytes === 'string' ? parseFloat(bytes) : bytes
  if (isNaN(n)) return '0 B'
  const units = ['B', 'KB', 'MB', 'GB', 'TB']
  let i = 0
  let size = n
  while (size >= 1024 && i < units.length - 1) {
    size /= 1024
    i++
  }
  return `${size.toFixed(1)} ${units[i]}`
}

/**
 * 计算百分比
 */
export function calcPercent(value: number, total: number, decimals = 0): string {
  if (total === 0) return '0%'
  return ((value / total) * 100).toFixed(decimals) + '%'
}
