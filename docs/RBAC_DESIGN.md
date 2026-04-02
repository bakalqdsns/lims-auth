# LIMS 用户管理与权限系统设计文档

## 1. 系统概述

### 1.1 设计目标
- 完整的用户生命周期管理（增删改查、启用/禁用）
- 基于RBAC的细粒度权限控制
- 支持多角色、多权限组合
- 前后端分离的权限验证机制

### 1.2 核心概念

```
用户(User) ──N:M── 角色(Role) ──N:M── 权限(Permission)
```

- **用户(User)**: 系统使用者，可拥有多个角色
- **角色(Role)**: 权限的集合，如"实验室管理员"、"普通教师"
- **权限(Permission)**: 最小权限单元，如"user:create", "equipment:read"

## 2. 数据库设计

### 2.1 实体关系

```
users (用户表)
├── id (PK)
├── username (唯一)
├── password_hash
├── email
├── phone
├── full_name
├── avatar_url
├── department_id (FK)
├── is_active
├── created_at
├── updated_at
└── last_login_at

roles (角色表)
├── id (PK)
├── code (唯一, 如: admin, lab_manager)
├── name (显示名称)
├── description
├── is_system (系统内置角色,不可删除)
├── is_active
└── created_at

permissions (权限表)
├── id (PK)
├── code (唯一, 如: user:create)
├── name (显示名称)
├── module (所属模块: user, equipment, lab等)
├── description
└── created_at

user_roles (用户角色关联表)
├── user_id (FK)
├── role_id (FK)
└── assigned_at

role_permissions (角色权限关联表)
├── role_id (FK)
├── permission_id (FK)
└── assigned_at

departments (部门/实验室表)
├── id (PK)
├── code
├── name
├── parent_id (自关联,支持层级)
├── manager_id (FK,负责人)
├── description
├── is_active
└── created_at
```

### 2.2 权限编码规范

格式: `{module}:{action}`

| 模块 | 权限示例 | 说明 |
|------|---------|------|
| user | user:create, user:read, user:update, user:delete | 用户管理 |
| role | role:create, role:read, role:update, role:delete, role:assign | 角色管理 |
| permission | permission:read, permission:assign | 权限管理 |
| equipment | equipment:create, equipment:read, equipment:update, equipment:delete, equipment:borrow | 设备管理 |
| lab | lab:create, lab:read, lab:update, lab:delete | 实验室管理 |
| course | course:create, course:read, course:update, course:delete, course:schedule | 课程管理 |
| report | report:create, report:read, report:approve | 报告管理 |
| system | system:config, system:log | 系统管理 |

### 2.3 预定义角色

| 角色编码 | 角色名称 | 说明 |
|----------|---------|------|
| super_admin | 超级管理员 | 拥有所有权限 |
| lab_admin | 实验室管理员 | 管理实验室、设备、预约 |
| teacher | 教师 | 课程管理、学生管理 |
| student | 学生 | 预约设备、提交报告 |
| auditor | 审计员 | 查看日志、报表 |

## 3. API 设计

### 3.1 用户管理 API

```
GET    /api/v1/users              # 获取用户列表(分页、筛选)
POST   /api/v1/users              # 创建用户
GET    /api/v1/users/{id}         # 获取用户详情
PUT    /api/v1/users/{id}         # 更新用户信息
DELETE /api/v1/users/{id}         # 删除用户
PATCH  /api/v1/users/{id}/status  # 启用/禁用用户
PUT    /api/v1/users/{id}/roles    # 分配角色
PUT    /api/v1/users/{id}/password # 重置密码
GET    /api/v1/users/{id}/permissions # 获取用户所有权限(平铺)
```

### 3.2 角色管理 API

```
GET    /api/v1/roles              # 获取角色列表
POST   /api/v1/roles              # 创建角色
GET    /api/v1/roles/{id}         # 获取角色详情
PUT    /api/v1/roles/{id}         # 更新角色
DELETE /api/v1/roles/{id}         # 删除角色
PUT    /api/v1/roles/{id}/permissions # 分配权限
GET    /api/v1/roles/{id}/users   # 获取角色下的用户
```

### 3.3 权限管理 API

```
GET    /api/v1/permissions        # 获取所有权限(按模块分组)
GET    /api/v1/permissions/modules # 获取权限模块列表
GET    /api/v1/permissions/my     # 获取当前用户权限
```

### 3.4 部门管理 API

```
GET    /api/v1/departments        # 获取部门树
POST   /api/v1/departments        # 创建部门
GET    /api/v1/departments/{id}   # 获取部门详情
PUT    /api/v1/departments/{id}   # 更新部门
DELETE /api/v1/departments/{id}   # 删除部门
```

## 4. 前端设计

### 4.1 页面结构

```
/system
├── /users              # 用户管理页
│   ├── 用户列表
│   ├── 新增/编辑用户弹窗
│   └── 角色分配弹窗
├── /roles              # 角色管理页
│   ├── 角色列表
│   ├── 新增/编辑角色弹窗
│   └── 权限分配弹窗
├── /permissions        # 权限查看页(只读)
└── /departments        # 部门管理页
    └── 部门树形管理
```

### 4.2 权限指令

```vue
<!-- 权限指令 -->
<button v-permission="'user:create'">新增用户</button>
<button v-permission="['user:update', 'user:delete']">操作</button>

<!-- 角色指令 -->
<div v-role="'admin'">管理员可见</div>

<!-- 路由守卫 -->
{
  path: '/system/users',
  meta: { permissions: ['user:read'] }
}
```

## 5. 安全设计

### 5.1 权限验证流程

1. **登录**: 验证用户名密码 → 生成JWT → 返回Token+用户基本信息
2. **请求**: 携带Token → 解析用户ID → 查询用户权限 → 验证权限 → 执行
3. **缓存**: 用户权限缓存到Redis，TTL=30分钟

### 5.2 权限注解

```csharp
[Authorize]                          // 仅需登录
[Authorize(Roles = "admin")]         // 需特定角色
[RequirePermission("user:create")]   // 需特定权限
[RequirePermission("user:update", Policy = PermissionPolicy.Any)] // 任一权限
[RequirePermission("user:update", Policy = PermissionPolicy.All)] // 所有权限
```

## 6. 实现步骤

### Phase 1: 后端基础
1. 创建数据库实体和迁移
2. 实现 Repository 层
3. 实现 Service 层
4. 实现 Controller API
5. 添加权限验证中间件/过滤器

### Phase 2: 前端基础
1. 创建 API 服务
2. 实现用户管理页面
3. 实现角色管理页面
4. 实现权限管理页面
5. 添加权限指令和路由守卫

### Phase 3: 集成优化
1. 权限缓存优化
2. 前端权限指令完善
3. 操作日志记录
4. 数据权限(行级权限)
