/**
 * Pinia Store 统一导出
 */
import { createPinia } from 'pinia'
import { createUnistorage } from 'pinia-plugin-unistorage'

export { useAuthStore } from './auth'
export { useAppStore } from './app'
export { useSemesterStore } from './semester'
export { useLabStore } from './lab'
export { useEquipmentStore } from './equipment'
export { useStatisticsStore } from './statistics'

export function setupPinia() {
  const pinia = createPinia()
  pinia.use(createUnistorage())
  return pinia
}
