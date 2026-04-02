import axios from 'axios'

const API_BASE_URL = '/api/v1'

// 用户管理 API
export const userApi = {
  // 获取用户列表
  getUsers: (params?: { keyword?: string; departmentId?: string; isActive?: boolean; page?: number; pageSize?: number }) =>
    axios.get(`${API_BASE_URL}/users`, { params }),

  // 获取用户详情
  getUser: (id: string) =>
    axios.get(`${API_BASE_URL}/users/${id}`),

  // 创建用户
  createUser: (data: CreateUserRequest) =>
    axios.post(`${API_BASE_URL}/users`, data),

  // 更新用户
  updateUser: (id: string, data: UpdateUserRequest) =>
    axios.put(`${API_BASE_URL}/users/${id}`, data),

  // 删除用户
  deleteUser: (id: string) =>
    axios.delete(`${API_BASE_URL}/users/${id}`),

  // 更新用户状态
  updateUserStatus: (id: string, isActive: boolean) =>
    axios.patch(`${API_BASE_URL}/users/${id}/status`, isActive, {
      headers: { 'Content-Type': 'application/json' }
    }),

  // 分配角色
  updateUserRoles: (id: string, roleIds: string[]) =>
    axios.put(`${API_BASE_URL}/users/${id}/roles`, { roleIds }),

  // 重置密码
  resetPassword: (id: string, newPassword: string) =>
    axios.put(`${API_BASE_URL}/users/${id}/password`, { newPassword }),

  // 修改密码
  changePassword: (oldPassword: string, newPassword: string) =>
    axios.post(`${API_BASE_URL}/users/change-password`, { oldPassword, newPassword }),

  // 获取当前用户权限
  getMyPermissions: () =>
    axios.get(`${API_BASE_URL}/users/permissions/my`)
}

// 角色管理 API
export const roleApi = {
  // 获取角色列表
  getRoles: (params?: { keyword?: string; isActive?: boolean; page?: number; pageSize?: number }) =>
    axios.get(`${API_BASE_URL}/roles`, { params }),

  // 获取所有角色(下拉选择)
  getAllRoles: () =>
    axios.get(`${API_BASE_URL}/roles/all`),

  // 获取角色详情
  getRole: (id: string) =>
    axios.get(`${API_BASE_URL}/roles/${id}`),

  // 创建角色
  createRole: (data: CreateRoleRequest) =>
    axios.post(`${API_BASE_URL}/roles`, data),

  // 更新角色
  updateRole: (id: string, data: UpdateRoleRequest) =>
    axios.put(`${API_BASE_URL}/roles/${id}`, data),

  // 删除角色
  deleteRole: (id: string) =>
    axios.delete(`${API_BASE_URL}/roles/${id}`),

  // 分配权限
  updateRolePermissions: (id: string, permissionIds: string[]) =>
    axios.put(`${API_BASE_URL}/roles/${id}/permissions`, { permissionIds }),

  // 获取角色下的用户
  getRoleUsers: (id: string) =>
    axios.get(`${API_BASE_URL}/roles/${id}/users`)
}

// 权限管理 API
export const permissionApi = {
  // 获取所有权限
  getAllPermissions: () =>
    axios.get(`${API_BASE_URL}/permissions`),

  // 按模块分组获取权限
  getPermissionsByModule: () =>
    axios.get(`${API_BASE_URL}/permissions/by-module`),

  // 获取权限模块列表
  getPermissionModules: () =>
    axios.get(`${API_BASE_URL}/permissions/modules`)
}

// 部门管理 API
export const departmentApi = {
  // 获取部门树
  getDepartmentTree: () =>
    axios.get(`${API_BASE_URL}/departments`),

  // 获取所有部门(下拉选择)
  getAllDepartments: () =>
    axios.get(`${API_BASE_URL}/departments/all`),

  // 获取部门详情
  getDepartment: (id: string) =>
    axios.get(`${API_BASE_URL}/departments/${id}`),

  // 创建部门
  createDepartment: (data: CreateDepartmentRequest) =>
    axios.post(`${API_BASE_URL}/departments`, data),

  // 更新部门
  updateDepartment: (id: string, data: UpdateDepartmentRequest) =>
    axios.put(`${API_BASE_URL}/departments/${id}`, data),

  // 删除部门
  deleteDepartment: (id: string) =>
    axios.delete(`${API_BASE_URL}/departments/${id}`)
}

// 类型定义
export interface CreateUserRequest {
  username: string
  password: string
  email?: string
  phone?: string
  fullName?: string
  departmentId?: string
  roleIds: string[]
  isActive?: boolean
}

export interface UpdateUserRequest {
  email?: string
  phone?: string
  fullName?: string
  departmentId?: string
  isActive?: boolean
}

export interface CreateRoleRequest {
  code: string
  name: string
  description?: string
}

export interface UpdateRoleRequest {
  name?: string
  description?: string
  isActive?: boolean
}

export interface CreateDepartmentRequest {
  code: string
  name: string
  parentId?: string
  managerId?: string
  description?: string
}

export interface UpdateDepartmentRequest {
  name?: string
  managerId?: string
  description?: string
  isActive?: boolean
}

export interface UserListItem {
  id: string
  username: string
  email?: string
  phone?: string
  fullName?: string
  isActive: boolean
  createdAt: string
  lastLoginAt?: string
  roles: RoleBrief[]
  department?: DepartmentBrief
}

export interface RoleBrief {
  id: string
  code: string
  name: string
}

export interface DepartmentBrief {
  id: string
  code: string
  name: string
}

export interface RoleDto {
  id: string
  code: string
  name: string
  description?: string
  isSystem: boolean
  isActive: boolean
  createdAt: string
  userCount: number
}

export interface RoleDetailDto extends RoleDto {
  permissions: PermissionBrief[]
}

export interface PermissionBrief {
  id: string
  code: string
  name: string
}

export interface PermissionDto {
  id: string
  code: string
  name: string
  module: string
  description?: string
}

export interface PermissionModuleDto {
  module: string
  moduleName: string
  permissions: PermissionDto[]
}

export interface DepartmentDto {
  id: string
  code: string
  name: string
  parentId?: string
  parentName?: string
  managerId?: string
  managerName?: string
  description?: string
  isActive: boolean
  createdAt: string
  children: DepartmentDto[]
}

export interface PagedResponse<T> {
  items: T[]
  total: number
  page: number
  pageSize: number
  totalPages: number
}
