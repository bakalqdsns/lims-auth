/**
 * 实验室状态管理
 */
import { defineStore } from 'pinia'
import { ref } from 'vue'
import { getLabs, getLabById } from '@/api/lab'
import { getCampuses, getBuildings } from '@/api/campus'
import type { Lab, LabQuery } from '@/types/campus'
import type { Campus, Building } from '@/types/campus'

export const useLabStore = defineStore(
  'lab',
  () => {
    const labs = ref<Lab[]>([])
    const total = ref(0)
    const currentLab = ref<Lab | null>(null)
    const campuses = ref<Campus[]>([])
    const buildings = ref<Building[]>([])
    const isLoading = ref(false)

    async function loadLabs(query?: LabQuery) {
      isLoading.value = true
      try {
        const resp = await getLabs(query)
        labs.value = resp?.items ?? []
        total.value = resp.total
      } catch {
        // ignore
      } finally {
        isLoading.value = false
      }
    }

    async function loadLabById(id: string) {
      try {
        currentLab.value = await getLabById(id)
      } catch {
        // ignore
      }
    }

    async function loadCampuses() {
      try {
        campuses.value = await getCampuses()
      } catch {
        // ignore
      }
    }

    async function loadBuildings(campusId?: string) {
      try {
        buildings.value = await getBuildings(campusId)
      } catch {
        // ignore
      }
    }

    function getCampusName(id: string): string {
      return campuses.value.find((c) => c.id.toString() === id)?.name ?? ''
    }

    function getBuildingName(id: string): string {
      return buildings.value.find((b) => b.id.toString() === id)?.name ?? ''
    }

    return {
      labs,
      total,
      currentLab,
      campuses,
      buildings,
      isLoading,
      loadLabs,
      loadLabById,
      loadCampuses,
      loadBuildings,
      getCampusName,
      getBuildingName,
    }
  },
  {
    unistorage: false,
  }
)
