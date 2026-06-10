/**
 * 全局应用状态
 */
import { defineStore } from 'pinia'
import { ref } from 'vue'

export const useAppStore = defineStore(
  'app',
  () => {
    const version = ref('1.0.0')
    const isAppReady = ref(false)
    const deviceInfo = ref<UniApp.SystemInfo | null>(null)

    function setAppReady() {
      isAppReady.value = true
    }

    function setDeviceInfo(info: UniApp.SystemInfo) {
      deviceInfo.value = info
    }

    function getStatusBarHeight(): number {
      return deviceInfo.value?.statusBarHeight ?? 0
    }

    function getNavBarHeight(): number {
      const statusBar = getStatusBarHeight()
      return statusBar + 44 // 44 = uni navigation bar height
    }

    return {
      version,
      isAppReady,
      deviceInfo,
      setAppReady,
      setDeviceInfo,
      getStatusBarHeight,
      getNavBarHeight,
    }
  },
  {
    unistorage: false, // 不持久化
  }
)
