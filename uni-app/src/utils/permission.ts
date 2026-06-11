/**
 * 权限判断工具
 * 与 Flutter lib/utils/permission_utils.dart 对齐
 */

import type { UserInfo } from '@/types/user'

/**
 * 判断用户是否拥有指定权限
 */
export function hasPermission(user: UserInfo | null, permission: string): boolean {
  if (!user) return false
  if (user.roles?.includes('super_admin')) return true
  return user.permissions?.includes(permission) ?? false
}

/**
 * 判断用户是否拥有指定角色
 */
export function hasRole(user: UserInfo | null, role: string): boolean {
  if (!user) return false
  return user.roles?.includes(role) ?? false
}

/**
 * 判断是否为管理员 (super_admin 或 lab_admin)
 */
export function isAdmin(user: UserInfo | null): boolean {
  if (!user) return false
  return user.roles?.some((r) => ['super_admin', 'lab_admin'].includes(r)) ?? false
}

/**
 * 判断是否为教师
 */
export function isTeacher(user: UserInfo | null): boolean {
  if (!user) return false
  return user.roles?.includes('teacher') ?? false
}

/**
 * 判断是否为学生
 */
export function isStudent(user: UserInfo | null): boolean {
  if (!user) return false
  return user.roles?.includes('student') ?? false
}

/**
 * 页面权限配置
 * key: 页面路径, value: 需要的权限/角色
 */
export const pagePermissions: Record<string, string | string[]> = {
  '/pages/admin/users/index': 'user:read',
  '/pages/admin/roles/index': 'role:read',
  '/pages/admin/departments/index': 'department:read',
  '/pages/admin/semesters/index': 'semester:read',
  '/pages/admin/equipment/index': 'equipment:read',
  '/pages/admin/teaching-apps/index': 'teaching_application:read',
  '/pages/admin/statistics/index': 'statistics:read',
}

/**
 * 检查用户是否有权访问指定页面
 */
export function canAccessPage(user: UserInfo | null, pagePath: string): boolean {
  const required = pagePermissions[pagePath]
  if (!required) return true // 未配置权限的页面公开访问

  const perms = Array.isArray(required) ? required : [required]
  return perms.some((p) => {
    if (p.startsWith('role:')) {
      return hasRole(user, p.replace('role:', ''))
    }
    return hasPermission(user, p)
  })
}
