import 'vue-router'

declare module 'vue-router' {
  interface RouteMeta {
    public?: boolean
    requiresAuth?: boolean
    roles?: ('super_admin' | 'admin' | 'lab_admin' | 'teacher' | 'student')[]
    permission?: string | string[]
    hide?: boolean
  }
}
