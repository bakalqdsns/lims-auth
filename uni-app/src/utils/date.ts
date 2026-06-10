/**
 * 日期工具函数
 * 与 Flutter lib/utils/date_utils.dart 对齐
 */

export type DateFormat = 'Y-M-D' | 'Y-M-D h:m' | 'Y-M-D h:m:s' | 'M-D' | 'h:m' | 'h:m:s' | 'timestamp'

/**
 * 格式化日期
 */
export function formatDate(
  date: string | number | Date | null | undefined,
  fmt: DateFormat = 'Y-M-D'
): string {
  if (!date) return '--'
  const d = new Date(date)
  if (isNaN(d.getTime())) return '--'

  switch (fmt) {
    case 'Y-M-D':
      return `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())}`
    case 'Y-M-D h:m':
      return `${formatDate(date, 'Y-M-D')} ${formatDate(date, 'h:m')}`
    case 'Y-M-D h:m:s':
      return `${formatDate(date, 'Y-M-D')} ${formatDate(date, 'h:m:s')}`
    case 'M-D':
      return `${pad(d.getMonth() + 1)}-${pad(d.getDate())}`
    case 'h:m':
      return `${pad(d.getHours())}:${pad(d.getMinutes())}`
    case 'h:m:s':
      return `${pad(d.getHours())}:${pad(d.getMinutes())}:${pad(d.getSeconds())}`
    case 'timestamp':
      return d.getTime().toString()
    default:
      return d.toISOString()
  }
}

function pad(n: number): string {
  return n < 10 ? '0' + n : String(n)
}

/**
 * 计算相对时间描述 (刚刚、5分钟前、昨天等)
 */
export function relativeTime(date: string | number | Date | null | undefined): string {
  if (!date) return '--'
  const d = new Date(date)
  if (isNaN(d.getTime())) return '--'

  const now = Date.now()
  const diff = now - d.getTime()
  const secs = Math.floor(diff / 1000)
  const mins = Math.floor(secs / 60)
  const hours = Math.floor(mins / 60)
  const days = Math.floor(hours / 24)

  if (secs < 60) return '刚刚'
  if (mins < 60) return `${mins}分钟前`
  if (hours < 24) return `${hours}小时前`
  if (days === 1) return '昨天'
  if (days < 7) return `${days}天前`
  return formatDate(date, 'Y-M-D')
}

/**
 * 获取今天日期字符串 Y-M-D
 */
export function today(): string {
  return formatDate(new Date(), 'Y-M-D')
}

/**
 * 判断两个日期是否为同一天
 */
export function isSameDay(a: string | Date, b: string | Date): boolean {
  const da = new Date(a)
  const db = new Date(b)
  return (
    da.getFullYear() === db.getFullYear() &&
    da.getMonth() === db.getMonth() &&
    da.getDate() === db.getDate()
  )
}

/**
 * 获取指定日期是星期几
 */
export function weekday(date: string | Date): string {
  const d = new Date(date)
  const weekdays = ['周日', '周一', '周二', '周三', '周四', '周五', '周六']
  return weekdays[d.getDay()]
}

/**
 * 日期范围数组 (含两端)
 */
export function dateRange(start: string | Date, end: string | Date): string[] {
  const result: string[] = []
  const s = new Date(start)
  const e = new Date(end)
  while (s <= e) {
    result.push(formatDate(s, 'Y-M-D'))
    s.setDate(s.getDate() + 1)
  }
  return result
}
