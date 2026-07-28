/**
 * 设备状态管理
 */
import { defineStore } from 'pinia'
import { ref } from 'vue'
import { getEquipments, getEquipmentById, getEquipmentStatistics } from '@/api/equipment'
import type { Equipment, EquipmentQuery, EquipmentStatistics } from '@/types/equipment'

export const useEquipmentStore = defineStore(
  'equipment',
  () => {
    const equipments = ref<Equipment[]>([])
    const total = ref(0)
    const currentEquipment = ref<Equipment | null>(null)
    const statistics = ref<EquipmentStatistics | null>(null)
    const isLoading = ref(false)

    async function loadEquipments(query?: EquipmentQuery) {
      isLoading.value = true
      try {
        const resp = await getEquipments(query)
        if (Array.isArray(resp)) {
          // 后端直接返回数组
          equipments.value = resp
          total.value = resp.length
        } else {
          // 标准分页格式
          equipments.value = resp?.items ?? []
          total.value = resp?.total ?? 0
        }
      } catch {
        // ignore
      } finally {
        isLoading.value = false
      }
    }

    async function loadEquipmentById(id: string) {
      try {
        currentEquipment.value = await getEquipmentById(id)
      } catch {
        // ignore
      }
    }

    async function loadStatistics() {
      try {
        statistics.value = await getEquipmentStatistics()
      } catch {
        // ignore
      }
    }

    return {
      equipments,
      total,
      currentEquipment,
      statistics,
      isLoading,
      loadEquipments,
      loadEquipmentById,
      loadStatistics,
    }
  },
  {
    unistorage: false,
  }
)
