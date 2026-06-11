/**
 * 统计状态管理
 */
import { defineStore } from 'pinia'
import { ref } from 'vue'
import { getDashboardStats, getLabUsageStats, getReservationStats, getWeeklySummary } from '@/api/statistics'
import type { DashboardStats, LabUsageStat, ReservationStat, WeeklySummary } from '@/types/teaching'

export const useStatisticsStore = defineStore(
  'statistics',
  () => {
    const dashboard = ref<DashboardStats | null>(null)
    const labUsage = ref<LabUsageStat[]>([])
    const reservationTrend = ref<ReservationStat[]>([])
    const weeklySummary = ref<WeeklySummary[]>([])
    const isLoading = ref(false)

    async function loadDashboard() {
      isLoading.value = true
      try {
        dashboard.value = await getDashboardStats()
      } catch {
        // ignore
      } finally {
        isLoading.value = false
      }
    }

    async function loadLabUsage() {
      try {
        labUsage.value = await getLabUsageStats()
      } catch {
        // ignore
      }
    }

    async function loadReservationTrend(startDate?: string, endDate?: string) {
      try {
        reservationTrend.value = await getReservationStats(startDate, endDate)
      } catch {
        // ignore
      }
    }

    async function loadWeeklySummary() {
      try {
        weeklySummary.value = await getWeeklySummary()
      } catch {
        // ignore
      }
    }

    async function loadAll() {
      isLoading.value = true
      await Promise.all([loadDashboard(), loadLabUsage(), loadWeeklySummary()])
      isLoading.value = false
    }

    return {
      dashboard,
      labUsage,
      reservationTrend,
      weeklySummary,
      isLoading,
      loadDashboard,
      loadLabUsage,
      loadReservationTrend,
      loadWeeklySummary,
      loadAll,
    }
  },
  {
    unistorage: false,
  }
)
