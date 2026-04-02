# LIMS-Auth 用户管理与权限系统

## 功能概述

系统已升级，新增完整的用户管理和基于 RBAC 的权限控制系统。

## 核心功能

### 1. 用户管理
- 用户列表(分页、筛选)
- 新增/编辑用户
- 启用/禁用用户
- 重置密码
- 分配角色
- 修改密码

### 2. 角色管理
- 角色列表(分页、筛选)
- 新增/编辑角色
- 删除角色(系统内置角色不可删除)
- 分配权限

### 3. 权限管理
- 权限列表(按模块分组)
- 9大模块共38个权限点

### 4. 部门管理
- 部门树形结构
- 新增/编辑部门
- 设置负责人
- 层级管理

## 权限模块

| 模块 | 权限点 |
|------|--------|
| user | create, read, update, delete, reset_password |
| role | create, read, update, delete, assign |
| permission | read, assign |
| department | create, read, update, delete |
| equipment | create, read, update, delete, borrow |
| lab | create, read, update, delete |
| course | create, read, update, delete, schedule |
| report | create, read, approve |
| system | config, log |

## 预定义角色

| 角色 | 权限范围 |
|------|----------|
| super_admin | 所有权限 |
| lab_admin | 设备、实验室、部门管理 |
| teacher | 课程、报告、设备查看/借用 |
| student | 课程查看、报告、设备借用 |
| auditor | 所有查看权限、系统日志 |

## 测试账号

| 角色 | 用户名 | 密码 |
|------|--------|------|
| 超级管理员 | admin | admin123 |
| 教师 | teacher | teacher123 |
| 学生 | student | student123 |

## API 端点

### 认证
- POST /api/v1/auth/login
- GET /api/v1/auth/me
- POST /api/v1/auth/refresh

### 用户管理
- GET /api/v1/users
- POST /api/v1/users
- GET /api/v1/users/{id}
- PUT /api/v1/users/{id}
- DELETE /api/v1/users/{id}
- PATCH /api/v1/users/{id}/status
- PUT /api/v1/users/{id}/roles
- PUT /api/v1/users/{id}/password
- POST /api/v1/users/change-password

### 角色管理
- GET /api/v1/roles
- GET /api/v1/roles/all
- POST /api/v1/roles
- GET /api/v1/roles/{id}
- PUT /api/v1/roles/{id}
- DELETE /api/v1/roles/{id}
- PUT /api/v1/roles/{id}/permissions

### 权限管理
- GET /api/v1/permissions
- GET /api/v1/permissions/by-module

### 部门管理
- GET /api/v1/departments
- GET /api/v1/departments/all
- POST /api/v1/departments
- PUT /api/v1/departments/{id}
- DELETE /api/v1/departments/{id}

## 前端页面

- /home - 首页
- /system/users - 用户管理
- /system/roles - 角色管理
- /system/departments - 部门管理

## 权限指令

```vue
<!-- 权限指令 -->
<button v-permission="'user:create'">新增用户</button>
<button v-permission="['user:update', 'user:delete']">操作</button>

<!-- 角色指令 -->
<div v-role="'super_admin'">管理员可见</div>
```

## 运行步骤

### 后端
```bash
cd lims-auth/backend/LimsAuth.Api
dotnet ef database update
dotnet run
```

### 前端
```bash
cd lims-auth/frontend
npm install
npm run dev
```

## 数据库迁移

首次运行会自动创建数据库和种子数据。

如需手动迁移：
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```
