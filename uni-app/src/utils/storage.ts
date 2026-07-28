/**
 * Storage 读写封装
 * 提供类型安全的本地存储访问
 */

const PREFIX = 'lims_'

export const storage = {
  set<T>(key: string, value: T): void {
    try {
      uni.setStorageSync(`${PREFIX}${key}`, JSON.stringify(value))
    } catch (e) {
      console.error('[Storage] set failed:', key, e)
    }
  },

  get<T>(key: string, defaultValue: T | null = null): T | null {
    try {
      const raw = uni.getStorageSync(`${PREFIX}${key}`)
      if (raw === '') return defaultValue
      return JSON.parse(raw as string) as T ?? defaultValue
    } catch (e) {
      console.error('[Storage] get failed:', key, e)
      return defaultValue
    }
  },

  remove(key: string): void {
    try {
      uni.removeStorageSync(`${PREFIX}${key}`)
    } catch (e) {
      console.error('[Storage] remove failed:', key, e)
    }
  },

  clear(): void {
    try {
      uni.clearStorageSync()
    } catch (e) {
      console.error('[Storage] clear failed:', e)
    }
  },

  // --- 常用 key ---
  getToken(): string | null {
    return storage.get<string>('token')
  },

  setToken(token: string): void {
    storage.set('token', token)
  },

  removeToken(): void {
    storage.remove('token')
  },

  getUser<T>(): T | null {
    return storage.get<T>('user')
  },

  setUser<T>(user: T): void {
    storage.set('user', user)
  },

  removeUser(): void {
    storage.remove('user')
  },
}
