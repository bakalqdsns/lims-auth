# LIMS 高校实验室管理系统

> 实验室信息管理系统 (Laboratory Information Management System) - 教学与实验室管理一体化平台

---

## 项目概述

LIMS 是一个基于 .NET 8.0 + Vue 3 的高校实验室管理系统，提供完整的用户权限管理、教学管理和实验室管理功能。

---

## 目录结构

```
lims-auth/
├── backend/                  # 原后端 (.NET 旧版本)
├── backend-new/               # 新后端 (.NET 8.0 重构版本)
│   ├── lims-auth.sln
│   ├── src/LimsAuth.Api/    # .NET 8.0 Web API 项目
│   │   ├── Controllers/      # API 控制器 (23个)
│   │   ├── Services/        # 业务逻辑层 (11个文件)
│   │   ├── Models/          # 数据模型
│   │   │   ├── Entities/    # 数据库实体 (26个)
│   │   │   └── DTOs/       # 请求/响应 DTO
│   │   ├── Data/            # 数据库上下文 + 配置
│   │   └── Authorization/    # 权限认证
│   └── docs/                # 详细文档
│       ├── DATABASE_DESIGN.md # 25张表全部字段平铺
│       ├── API_SPEC.md       # 完整API接口定义
│       ├── PROJECT_STRUCTURE.md # 项目结构说明
│       ├── DECISION_LOG.md   # 技术决策记录
│       ├── DEV_LOG.md        # 开发日志
│       └── TODO.md           # 待办任务
├── frontend/                 # Vue 3 前端
├── docs/                     # 原项目文档
└── README.md                 # 本文档
```

---

## 技术栈

### 后端 (.NET 8.0)
- ASP.NET Core Web API
- SQLite + Entity Framework Core 8.0
- JWT Bearer Token 认证
- Swagger API 文档
- BCrypt 密码加密

### 前端
- Vue 3 + TypeScript
- Vite 5 构建工具
- Element Plus 2.5 UI 组件库
- Pinia 状态管理
- Vue Router 4 路由

---

## 四大功能模块

| 模块 | 子模块 | 说明 |
|------|--------|------|
| 系统管理 | 用户/角色/权限/机构/部门 | 完整的RBAC权限体系 |
| 基础教学 | 学期/课程/专业/班级/教学任务 | 教学基础数据管理 |
| 排课预约 | 排课/实验项目/预约申请/使用登记 | 实验室使用全流程管理 |
| 资源管理 | 设备资产/借还 + 耗材/出入库 | 设备耗材全生命周期管理 |

---

## 数据库设计 (backend-new)

**总计 25 张数据库表，全部字段平铺展示在 [docs/DATABASE_DESIGN.md](backend-new/docs/DATABASE_DESIGN.md)**

| 分类 | 表数 | 说明 |
|------|------|------|
| 系统基础 | 7张 | 用户/角色/权限/机构/部门 + 关联表 |
| 基础教学 | 7张 | 学期/课程/专业/班级/教学任务 + 关联表 |
| 场地管理 | 2张 | 楼宇/实验室 |
| 排课预约 | 4张 | 排课/实验项目/预约申请/使用登记 |
| 设备管理 | 2张 | 设备资产/借还申请 |
| 耗材管理 | 4张 | 耗材/入库/出库/库存日志 |

---

## API 接口 (backend-new)

**完整接口定义在 [docs/API_SPEC.md](backend-new/docs/API_SPEC.md)**

| # | 控制器 | 端点数 | 说明 |
|---|--------|--------|------|
| 1 | AuthController | 4 | 登录/登出/修改密码/资料更新 |
| 2 | UsersController | 9 | 用户CRUD/角色分配/重置密码 |
| 3 | RolesController | 6 | 角色CRUD/权限分配 |
| 4 | PermissionsController | 2 | 权限列表/按模块分组 |
| 5 | InstitutionsController | 5 | 机构CRUD/树形结构 |
| 6 | DepartmentsController | 5 | 部门CRUD/树形结构 |
| 7 | SemestersController | 7 | 学期CRUD/设为当前 |
| 8 | CoursesController | 6 | 课程CRUD/状态切换 |
| 9 | MajorsController | 6 | 专业CRUD/状态切换 |
| 10 | ClassesController | 8 | 班级CRUD/学生分配 |
| 11 | TeachingTasksController | 6 | 教学任务CRUD/教师分配 |
| 12 | BuildingsController | 5 | 楼宇CRUD |
| 13 | RoomsController | 5 | 实验室CRUD |
| 14 | SchedulesController | 6 | 排课CRUD/仪表盘 |
| 15 | ExperimentItemsController | 5 | 实验项目CRUD |
| 16 | BookingAppliesController | 5 | 预约申请CRUD/审批 |
| 17 | UsageRegistersController | 3 | 使用登记CRUD |
| 18 | AssetsController | 6 | 设备资产CRUD/统计 |
| 19 | LoanAppliesController | 6 | 借还申请CRUD/审批/归还/续借 |
| 20 | ConsumablesController | 6 | 耗材CRUD/统计 |
| 21 | ConsumableInRecordsController | 3 | 耗材入库CRUD/审核 |
| 22 | ConsumableOutRecordsController | 3 | 耗材出库CRUD/审核 |
| 23 | ConsumableStockLogsController | 2 | 库存日志/手动调整 |

**总计约 120+ 个 API 端点**

---

## 权限系统

### 权限编码规范
格式: `Permission:{module}:{action}`

### 预定义角色

| 角色编码 | 角色名称 | 说明 |
|----------|---------|------|
| super_admin | 超级管理员 | 拥有所有权限 |
| teacher | 教师 | 课程管理、学生管理 |
| student | 学生 | 查看课程、提交预约 |

### 预置权限点 (65个)
覆盖 user/role/permission/institution/department/semester/course/major/class/teachingtask/building/room/schedule/booking/usage/experimentitem/asset/loan/consumable 等19个模块

---

## 测试账号

| 账号 | 密码 | 角色 |
|------|------|------|
| admin | admin123 | 超级管理员 |
| teacher | teacher123 | 教师 |
| student | student123 | 学生 |

---

## 快速开始

### 启动新后端 (backend-new)

```bash
cd backend-new/src/LimsAuth.Api
dotnet run --urls "http://0.0.0.0:5047"
```

### 访问地址

| 服务 | 地址 |
|------|------|
| 后端 API | http://localhost:5047 |
| Swagger 文档 | http://localhost:5047/swagger |

---

## 文档导航

详细文档请查看 `backend-new/docs/` 目录:

| 文档 | 说明 |
|------|------|
| [DATABASE_DESIGN.md](backend-new/docs/DATABASE_DESIGN.md) | 25张数据库表全部字段平铺展示 |
| [API_SPEC.md](backend-new/docs/API_SPEC.md) | 完整API接口定义 |
| [PROJECT_STRUCTURE.md](backend-new/docs/PROJECT_STRUCTURE.md) | 项目结构详细说明 |
| [DECISION_LOG.md](backend-new/docs/DECISION_LOG.md) | 技术决策记录 |
| [DEV_LOG.md](backend-new/docs/DEV_LOG.md) | 开发进度日志 |
| [TODO.md](backend-new/docs/TODO.md) | 待办任务清单 |

---

## 许可证

MIT License

---

## 最后更新

2026-06-11
