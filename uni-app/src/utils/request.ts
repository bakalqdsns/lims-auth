/**
 * uni.request 封装
 * - 自动注入 Authorization: Bearer {token}
 * - 401 自动清除存储并跳转登录页
 * - 统一错误处理
 * - 超时: 30s
 */

import { storage } from './storage'

// API 配置
// 开发时通过 Vite proxy 代理到后端 (见 vite.config.ts)
// H5/MP 等平台各自通过 uni.request 直连
// 微信小程序开发环境: 使用局域网 IP（如 http://192.168.x.x:5047）
// 生产环境: 使用域名（如 https://api.example.com）
// 开发时需将开发者电脑的局域网 IP 加入微信开发者工具的"不校验合法域名"设置
const BASE_URL = 'http://172.16.155.113:5047'
const API_PREFIX = '/api/v1'
const TIMEOUT = 30000

// 无需认证的路径
const NO_AUTH_PATHS = ['/auth/login', '/auth/health']

export interface RequestOptions {
  url: string
  method?: 'GET' | 'POST' | 'PUT' | 'DELETE' | 'PATCH'
  data?: Record<string, unknown> | unknown[] | string | null
  params?: Record<string, string | number | boolean>
  header?: Record<string, string>
  loading?: boolean
  loadingText?: string
  showError?: boolean
}

type RequestData = Record<string, unknown> | unknown[] | string | null

/**
 * 将 params 附加到 URL 上
 */
function buildUrl(url: string, params?: Record<string, string | number | boolean>): string {
  if (!params) return url
  const qs = Object.entries(params)
    .filter(([, v]) => v !== undefined && v !== null && v !== '')
    .map(([k, v]) => `${encodeURIComponent(k)}=${encodeURIComponent(String(v))}`)
    .join('&')
  if (!qs) return url
  return url.includes('?') ? `${url}&${qs}` : `${url}?${qs}`
}

/**
 * 统一请求方法
 */
export function request<T = unknown>(options: RequestOptions): Promise<T> {
  const {
    url,
    method = 'GET',
    data,
    params,
    header = {},
    loading = true,
    loadingText = '加载中...',
    showError = true,
  } = options

  if (loading) {
    uni.showLoading({ title: loadingText, mask: true })
  }

  const token = storage.getToken()
  const needAuth = !NO_AUTH_PATHS.some((p) => url.toLowerCase().includes(p.toLowerCase()))
  if (needAuth && token) {
    header['Authorization'] = `Bearer ${token}`
  }
  header['Content-Type'] = header['Content-Type'] ?? 'application/json'
  header['Accept'] = 'application/json'

  const fullUrl = `${BASE_URL}${API_PREFIX}${buildUrl(url.startsWith('/') ? url : '/' + url, params)}`

  return new Promise((resolve, reject) => {
    uni.request({
      url: fullUrl,
      method: method as UniApp.RequestOptions['method'],
      data: data as UniApp.RequestOptions['data'],
      header: header as UniApp.RequestOptions['header'],
      timeout: TIMEOUT,
      success: (res) => {
        if (loading) uni.hideLoading()

        const statusCode = res.statusCode as number
        const responseData = res.data as Record<string, unknown>

        if (statusCode === 401) {
          storage.removeToken()
          storage.removeUser()
          uni.showToast({ title: '登录已过期，请重新登录', icon: 'none' })
          uni.reLaunch({ url: '/pages/login/index' })
          reject({ code: 401, message: '登录已过期' })
          return
        }

        if (statusCode >= 200 && statusCode < 300) {
          if (responseData.code === 200 || responseData.code === undefined) {
            resolve((responseData.data ?? responseData) as T)
          } else {
            if (showError && responseData.message) {
              uni.showToast({ title: responseData.message as string, icon: 'none' })
            }
            reject({ code: responseData.code as number, message: (responseData.message as string) || '请求失败' })
          }
          return
        }

        let errMsg = `请求失败 (${statusCode})`
        if (responseData?.message) {
          errMsg = responseData.message as string
        }
        if (showError) {
          uni.showToast({ title: errMsg, icon: 'none' })
        }
        reject({ code: statusCode, message: errMsg })
      },
      fail: (err) => {
        if (loading) uni.hideLoading()

        let msg = '网络请求失败'
        if (err.errMsg?.includes('timeout')) {
          msg = '请求超时，请检查网络'
        } else if (err.errMsg?.includes('abort')) {
          msg = '请求已取消'
        }

        if (showError) {
          uni.showToast({ title: msg, icon: 'none' })
        }
        reject({ code: -1, message: msg })
      },
    })
  })
}

/**
 * GET 请求
 */
export function get<T = unknown>(
  url: string,
  params?: Record<string, string | number | boolean>,
  options?: Partial<Omit<RequestOptions, 'url' | 'method' | 'params'>>
): Promise<T> {
  return request<T>({ url, method: 'GET', params, ...options })
}

/**
 * POST 请求
 */
export function post<T = unknown>(
  url: string,
  data?: unknown,
  options?: Partial<Omit<RequestOptions, 'url' | 'method' | 'data'>>
): Promise<T> {
  return request<T>({ url, method: 'POST', data: data as RequestData, ...options })
}

/**
 * PUT 请求
 */
export function put<T = unknown>(
  url: string,
  data?: unknown,
  options?: Partial<Omit<RequestOptions, 'url' | 'method' | 'data'>>
): Promise<T> {
  return request<T>({ url, method: 'PUT', data: data as RequestData, ...options })
}

/**
 * PATCH 请求
 */
export function patch<T = unknown>(
  url: string,
  data?: unknown,
  options?: Partial<Omit<RequestOptions, 'url' | 'method' | 'data'>>
): Promise<T> {
  return request<T>({ url, method: 'PATCH', data: data as RequestData, ...options })
}

/**
 * DELETE 请求
 */
export function del<T = unknown>(
  url: string,
  data?: unknown,
  options?: Partial<Omit<RequestOptions, 'url' | 'method' | 'data'>>
): Promise<T> {
  return request<T>({ url, method: 'DELETE', data: data as RequestData, ...options })
}

/**
 * 上传文件
 */
export function uploadFile(
  url: string,
  filePath: string,
  name = 'file',
  formData?: Record<string, string>
): Promise<{ url: string; fileName: string }> {
  const token = storage.getToken()
  const header: Record<string, string> = {}
  if (token) {
    header['Authorization'] = `Bearer ${token}`
  }

  return new Promise((resolve, reject) => {
    uni.showLoading({ title: '上传中...', mask: true })
    uni.uploadFile({
      url: `${BASE_URL}${API_PREFIX}${url}`,
      filePath,
      name,
      formData,
      header,
      success: (res) => {
        uni.hideLoading()
        if (res.statusCode === 200) {
          const data = JSON.parse(res.data as string)
          if (data.code === 200) {
            resolve(data.data)
          } else {
            uni.showToast({ title: data.message || '上传失败', icon: 'none' })
            reject(data)
          }
        } else {
          uni.showToast({ title: '上传失败', icon: 'none' })
          reject({ code: res.statusCode, message: '上传失败' })
        }
      },
      fail: (err) => {
        uni.hideLoading()
        uni.showToast({ title: '上传失败', icon: 'none' })
        reject(err)
      },
    })
  })
}
