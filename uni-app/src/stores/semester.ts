/**
 * 学期状态管理
 */
import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { getCurrentSemester, getSemesterCalendar, getWeekInfo } from '@/api/semester'
import type { Semester, CalendarItem } from '@/types/semester'

export const useSemesterStore = defineStore(
  'semester',
  () => {
    const currentSemester = ref<Semester | null>(null)
    const currentWeek = ref(1)
    const totalWeeks = ref(16)
    const semesterStartDate = ref('')
    const calendarItems = ref<CalendarItem[]>([])
    const isLoading = ref(false)

    const hasSemester = computed(() => !!currentSemester.value)

    async function loadCurrentSemester() {
      isLoading.value = true
      try {
        const semester = await getCurrentSemester()
        currentSemester.value = semester
        semesterStartDate.value = semester.startDate
        totalWeeks.value = semester.weekCount || 16
        currentWeek.value = semester.currentWeek || 1
      } catch {
        // ignore
      } finally {
        isLoading.value = false
      }
    }

    async function loadCalendar(semesterId: string) {
      try {
        calendarItems.value = await getSemesterCalendar(semesterId)
      } catch {
        // ignore
      }
    }

    async function loadWeekInfo(semesterId: string) {
      try {
        const info = await getWeekInfo(semesterId)
        currentWeek.value = info.currentWeek
        totalWeeks.value = info.totalWeeks
        semesterStartDate.value = info.startDate
      } catch {
        // ignore
      }
    }

    function getDayOfWeek(dateStr: string): number {
      const d = new Date(dateStr)
      let day = d.getDay()
      return day === 0 ? 7 : day // 周一=1, 周日=7
    }

    return {
      currentSemester,
      currentWeek,
      totalWeeks,
      semesterStartDate,
      calendarItems,
      isLoading,
      hasSemester,
      loadCurrentSemester,
      loadCalendar,
      loadWeekInfo,
      getDayOfWeek,
    }
  },
  {
    unistorage: false, // 不持久化
  }
)
