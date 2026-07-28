/**
 * API 统一导出
 * 与后端 LimsAuth.Api 控制器一一对应
 */

// 鉴权 & 用户
export * from './auth'
export * from './user'
export * from './role'
export * from './permission'
export * from './department'

// 资源
export * from './campus'
export * from './building'
export * from './lab'
export * from './equipment'
export * from './consumable'

// 组织/教学基础数据
export * from './course'
export * from './major'
export * from './class'
export * from './teaching-task'
export * from './teaching-app'
export * from './semester'
export * from './calendar'

// 业务
export * from './reservation'
export * from './borrow-record'
export * from './schedule'
export * from './usage-reg'
export * from './experiment'

// 统计 & 导出
export * from './statistics'
export * from './export'
