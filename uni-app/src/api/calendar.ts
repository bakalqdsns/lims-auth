/**
 * 校历 API
 * 对应后端 CalendarController (api/v1/calendar)
 *  GET    /api/v1/calendar?semesterId={id}
 *  GET    /api/v1/calendar/today
 *  GET    /api/v1/calendar/date/{date}
 *  GET    /api/v1/calendar/week-info
 *  PUT    /api/v1/calendar/{id}
 *  GET    /api/v1/calendar/event-types
 *  GET    /api/v1/calendar/check-permission
 *  GET    /api/v1/calendar/holidays
 *  POST   /api/v1/calendar/holidays
 *  POST   /api/v1/calendar/adjust-workday
 *  GET    /api/v1/calendar/by-event-type
 */
import { get, post, put } from '@/utils/request'
import type { ApiResponse } from '@/types/api'
import type {
  AcademicCalendar,
  CalendarEventType,
  UpdateCalendarRequest,
  HolidayDto,
  AddHolidayRequest,
  AdjustWorkdayRequest,
  BusinessPermission,
} from '@/types/calendar'

/** 学期日历 */
export function getCalendarBySemester(semesterId: string) {
  return get<ApiResponse<AcademicCalendar[]>>('/calendar', { semesterId })
}

/** 今日 */
export function getCalendarToday() {
  return get<ApiResponse<AcademicCalendar>>('/calendar/today')
}

/** 指定日期 */
export function getCalendarByDate(date: string) {
  return get<ApiResponse<AcademicCalendar>>(`/calendar/date/${date}`)
}

/** 周次信息 */
export function getCalendarWeekInfo(semesterId: string, weekNumber: number) {
  return get<ApiResponse<{
    semesterId: string
    weekNumber: number
    startDate: string
    endDate: string
    days: AcademicCalendar[]
  }>>('/calendar/week-info', { semesterId, weekNumber })
}

/** 更新日历项 */
export function updateCalendar(id: string, data: UpdateCalendarRequest) {
  return put<ApiResponse>(`/calendar/${id}`, data)
}

/** 事件类型字典 */
export function getCalendarEventTypes() {
  return get<ApiResponse<{ code: string; name: string; color: string }[]>>('/calendar/event-types')
}

/** 业务时间窗校验 */
export function checkBusinessPermission(semesterId: string, businessType: string) {
  return get<ApiResponse<BusinessPermission>>('/calendar/check-permission', { semesterId, businessType })
}

/** 节假日列表 */
export function getHolidays(semesterId: string) {
  return get<ApiResponse<HolidayDto[]>>('/calendar/holidays', { semesterId })
}

/** 新增节假日 */
export function addHoliday(data: AddHolidayRequest) {
  return post<ApiResponse>('/calendar/holidays', data)
}

/** 调休 */
export function adjustWorkday(data: AdjustWorkdayRequest) {
  return post<ApiResponse>('/calendar/adjust-workday', data)
}

/** 按事件类型筛选 */
export function getCalendarByEventType(semesterId: string, eventType: CalendarEventType) {
  return get<ApiResponse<AcademicCalendar[]>>('/calendar/by-event-type', { semesterId, eventType })
}
