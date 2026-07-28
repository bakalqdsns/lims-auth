# LIMS 高校实验室管理系统

> 实验室信息管理系统 (Laboratory Information Management System) - 教学与实验室管理一体化平台

---

## 项目概述

LIMS 是一个基于 .NET 8.0 + Vue 3 的高校实验室管理系统，提供完整的用户权限管理、教学管理和实验室管理功能。

### 核心功能

| 模块 | 功能说明 |
|------|----------|
| 用户权限 | 用户管理、角色管理、RBAC 权限控制 |
| 教学管理 | 学期管理、课程管理、专业管理、班级管理、教学任务管理 |
| 校历管理 | 学期校历生成、节假日管理、周次信息 |
| 实验室管理 | 实验室管理、设备管理 (规划中) |

### 技术栈

**后端**
- .NET 8.0 ASP.NET Core Web API
- SQLite + Entity Framework Core 8.0
- JWT Bearer Token 认证
- Swagger API 文档

**前端**
- Vue 3 + TypeScript
- Vite 5 构建工具
- Element Plus 2.5 UI 组件库
- Pinia 状态管理
- Vue Router 4 路由

---

## 快速开始

### 环境要求

- .NET 8.0 SDK
- Node.js 18+
- npm 或 pnpm

### 启动后端

```bash
cd backend/LimsAuth.Api
dotnet run --urls "http://0.0.0.0:5047"
```

### 启动前端

```bash
cd frontend
npm install
npm run dev
```

### 访问地址

| 服务 | 地址 |
|------|------|
| 前端 | http://localhost:5173 |
| 后端 API | http://localhost:5047 |
| Swagger 文档 | http://localhost:5047/swagger |

---

## 测试账号

| 账号 | 密码 | 角色 |
|------|------|------|
| admin | admin123 | 超级管理员 |
| teacher | teacher123 | 教师 |
| student | student123 | 学生 |

---

## 项目结构

```
lims-auth/
├── backend/
│   └── LimsAuth.Api/           # .NET 8.0 后端项目
│       ├── Controllers/        # API 控制器 (14个)
│       ├── Services/           # 业务逻辑层 (15个)
│       ├── Models/             # 数据模型 + DTOs
│       ├── Data/               # 数据库上下文
│       └── Authorization/       # 权限认证
├── frontend/
│   └── src/
│       ├── api/               # API 接口
│       ├── views/             # 页面视图
│       ├── stores/            # 状态管理
│       ├── router/            # 路由配置
│       └── directives/        # 指令
└── docs/                      # 项目文档
```

### 后端控制器 (14个)

| 控制器 | 功能 |
|--------|------|
| AuthController | 认证 |
| UsersController | 用户管理 |
| RolesController | 角色管理 |
| PermissionsController | 权限管理 |
| DepartmentsController | 部门管理 |
| SemestersController | 学期管理 |
| CalendarController | 校历管理 |
| CoursesController | 课程管理 |
| MajorsController | 专业管理 |
| ClassesController | 班级管理 |
| PeriodTimesController | 节次时间管理 |
| TeachingTasksController | 教学任务管理 |
| LabsController | 实验室管理 |
| EquipmentsController | 设备管理 |

---

## 数据库实体 (18个表)

### 系统管理模块
| 表名 | 说明 |
|------|------|
| users | 用户表 |
| roles | 角色表 |
| permissions | 权限表 |
| user_roles | 用户角色关联 |
| role_permissions | 角色权限关联 |
| departments | 部门表 |

### 教学管理模块
| 表名 | 说明 |
|------|------|
| semesters | 学期表 |
| academic_calendars | 校历表 |
| courses | 课程表 |
| majors | 专业表 |
| classes | 班级表 |
| class_students | 班级学生关联 |
| teaching_tasks | 教学任务表 |
| teaching_task_teachers | 教学任务教师关联 |
| period_times | 节次时间表 |

### 实验室管理模块
| 表名 | 说明 |
|------|------|
| labs | 实验室表 |
| equipments | 设备表 |

---

## 权限系统

### 权限编码规范
格式: `{module}:{action}`

| 模块 | 权限代码 |
|------|----------|
| user | user:create, user:read, user:update, user:delete, user:reset_password |
| role | role:create, role:read, role:update, role:delete, role:assign |
| permission | permission:read, permission:assign |
| department | department:create, department:read, department:update, department:delete |
| course | course:create, course:read, course:update, course:delete, course:schedule |
| major | major:create, major:read, major:update, major:delete |
| class | class:create, class:read, class:update, class:delete |
| period_time | period_time:create, period_time:read, period_time:update, period_time:delete |
| lab | lab:create, lab:read, lab:update, lab:delete |
| equipment | equipment:create, equipment:read, equipment:update, equipment:delete |
| calendar | calendar:read, calendar:update |

### 预定义角色

| 角色编码 | 角色名称 | 说明 |
|----------|---------|------|
| super_admin | 超级管理员 | 拥有所有权限 |
| lab_admin | 实验室管理员 | 管理实验室、设备 |
| teacher | 教师 | 课程管理、学生管理 |
| student | 学生 | 预约设备、提交报告 |
| auditor | 审计员 | 查看日志、报表 |

---

## API 接口概览

### 认证相关
```
POST /api/v1/auth/login     # 用户登录
POST /api/v1/auth/logout    # 用户登出
GET  /api/v1/auth/me        # 获取当前用户信息
```

### 系统管理
```
# 用户管理
GET    /api/v1/users           # 用户列表
POST   /api/v1/users           # 创建用户
GET    /api/v1/users/{id}      # 用户详情
PUT    /api/v1/users/{id}      # 更新用户
DELETE /api/v1/users/{id}      # 删除用户
PATCH  /api/v1/users/{id}/status  # 启用/禁用

# 角色管理
GET    /api/v1/roles           # 角色列表
POST   /api/v1/roles           # 创建角色
PUT    /api/v1/roles/{id}      # 更新角色
DELETE /api/v1/roles/{id}      # 删除角色

# 部门管理
GET    /api/v1/departments     # 部门列表
POST   /api/v1/departments     # 创建部门
PUT    /api/v1/departments/{id}  # 更新部门
DELETE /api/v1/departments/{id}  # 删除部门
```

### 教学管理
```
# 学期管理
GET    /api/v1/semesters              # 学期列表
POST   /api/v1/semesters              # 创建学期
GET    /api/v1/semesters/current      # 获取当前学期
POST   /api/v1/semesters/{id}/set-current  # 设为当前学期
POST   /api/v1/semesters/{id}/generate-calendar  # 生成校历

# 课程/专业/班级
GET    /api/v1/courses       # 课程列表
GET    /api/v1/majors        # 专业列表
GET    /api/v1/classes       # 班级列表
GET    /api/v1/classes/{id}/students  # 班级学生

# 教学任务
GET    /api/v1/teaching-tasks            # 教学任务列表
POST   /api/v1/teaching-tasks            # 创建教学任务
POST   /api/v1/teaching-tasks/{id}/teachers  # 分配教师

# 校历管理
GET    /api/v1/calendar              # 获取校历
GET    /api/v1/calendar/today        # 今天校历
GET    /api/v1/calendar/week-info    # 周次信息

# 节次时间
GET    /api/v1/period-times    # 节次列表
```

---

## 文档导航

详细文档请查看 `docs/` 目录：

| 文档 | 说明 |
|------|------|
| [docs/README.md](./docs/README.md) | 文档首页 |
| [docs/PROJECT_STRUCTURE.md](./docs/PROJECT_STRUCTURE.md) | 项目结构详细说明 |
| [docs/RBAC_DESIGN.md](./docs/RBAC_DESIGN.md) | 权限系统设计文档 |
| [docs/TEACHING_SYSTEM_DESIGN.md](./docs/TEACHING_SYSTEM_DESIGN.md) | 教学管理系统设计 |
| [docs/TEACHING_README.md](./docs/TEACHING_README.md) | 教学模块快速参考 |
| [docs/PERMISSION_REFACTOR.md](./docs/PERMISSION_REFACTOR.md) | 权限策略整理与建议 |
| [docs/KNOWN_ISSUES.md](./docs/KNOWN_ISSUES.md) | 已知缺陷与风险清单 |

---
