import { createSSRApp } from 'vue'
import { createPinia } from 'pinia'
import { createUnistorage } from 'pinia-plugin-unistorage'
import App from './App.vue'
import { setupRouteGuard } from '@/utils/guard'
import { useAppStore } from '@/stores/app'

export function createApp() {
  const app = createSSRApp(App)

  // Pinia
  const pinia = createPinia()
  pinia.use(createUnistorage())
  app.use(pinia)

  // 路由守卫
  setupRouteGuard()

  // 获取设备信息
  const appStore = useAppStore()
  uni.getSystemInfo({
    success: (info) => {
      appStore.setDeviceInfo(info as unknown as UniApp.SystemInfo)
    },
  })

  return {
    app,
  }
}
