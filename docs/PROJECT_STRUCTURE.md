# LIMS 项目结构文档

> 更新日期: 2026-05-27

---

## 项目概览

LIMS (Laboratory Information Management System) 是一个高校实验室管理系统，提供用户权限管理、教学管理和实验室管理功能。

### 技术栈

| 层级 | 技术 |
|------|------|
| 后端框架 | .NET 8.0 ASP.NET Core Web API |
| 数据库 | SQLite + Entity Framework Core 8.0 |
| 前端框架 | Vue 3 + TypeScript + Vite 5 |
| UI 组件 | Element Plus 2.5 |
| 状态管理 | Pinia |
| 路由 | Vue Router 4 |
| 认证方式 | JWT Bearer Token |
| API 文档 | Swagger/OpenAPI |

---

## 项目根目录

```
lims-auth/
├── backend/                    # .NET 8.0 后端
│   └── LimsAuth.Api/          # 主项目目录
├── frontend/                   # Vue 3 前端
│   └── src/                   # 源代码目录
└── docs/                      # 项目文档
```

---

## 后端结构 (backend/LimsAuth.Api/)

### 目录结构

```
backend/LimsAuth.Api/
├── LimsAuth.Api.csproj         # 项目文件
├── Program.cs                  # 应用入口 + 服务配置
├── appsettings.json            # 配置文件
├── lims.db                     # SQLite 数据库文件
│
├── Authorization/              # 权限认证模块
│   ├── PermissionHandler.cs    # 权限验证处理器
│   ├── PermissionRequirement.cs # 权限需求定义
│   ├── PermissionPolicy.cs      # 权限策略常量
│   └── PermissionAuthorizationExtensions.cs  # 授权扩展
│
├── Controllers/                # API 控制器 (14个)
│   ├── AuthController.cs       # 认证控制器
│   ├── UsersController.cs       # 用户管理
│   ├── RolesController.cs       # 角色管理
│   ├── PermissionsController.cs # 权限管理
│   ├── DepartmentsController.cs # 部门管理
│   ├── SemestersController.cs   # 学期管理
│   ├── CalendarController.cs    # 校历管理
│   ├── CoursesController.cs     # 课程管理
│   ├── MajorsController.cs      # 专业管理
│   ├── ClassesController.cs     # 班级管理
│   ├── PeriodTimesController.cs # 节次时间管理
│   ├── TeachingTasksController.cs  # 教学任务管理
│   ├── LabsController.cs        # 实验室管理
│   └── EquipmentsController.cs   # 设备管理
│
├── Data/                       # 数据访问层
│   └── AppDbContext.cs         # EF Core 上下文 + 种子数据
│
├── Models/                     # 数据模型
│   ├── User.cs                 # 用户实体
│   ├── Role.cs                 # 角色实体
│   ├── Permission.cs           # 权限实体
│   ├── UserRole.cs             # 用户角色关联
│   ├── RolePermission.cs       # 角色权限关联
│   ├── Department.cs           # 部门实体
│   ├── Semester.cs             # 学期实体
│   ├── AcademicCalendar.cs     # 校历实体
│   ├── Course.cs               # 课程实体
│   ├── Major.cs                # 专业实体
│   ├── Class.cs                # 班级实体
│   ├── ClassStudent.cs         # 班级学生关联
│   ├── TeachingTask.cs         # 教学任务实体
│   ├── TeachingTaskTeacher.cs  # 教学任务教师关联
│   ├── PeriodTime.cs           # 节次时间实体
│   ├── Lab.cs                  # 实验室实体
│   ├── Equipment.cs            # 设备实体
│   └── DTOs/
│       └── AuthDtos.cs         # 所有请求/响应 DTO
│
└── Services/                   # 业务逻辑层 (15个)
    ├── AuthService.cs          # 认证服务
    ├── JwtService.cs           # JWT 服务
    ├── UserService.cs          # 用户服务
    ├── RoleService.cs          # 角色服务
    ├── PermissionService.cs     # 权限服务
    ├── DepartmentService.cs     # 部门服务
    ├── SemesterService.cs       # 学期服务
    ├── AcademicCalendarService.cs  # 校历服务
    ├── CourseService.cs         # 课程服务
    ├── MajorService.cs          # 专业服务
    ├── ClassService.cs          # 班级服务
    ├── TeachingTaskService.cs   # 教学任务服务
    ├── PeriodTimeService.cs     # 节次时间服务
    ├── LabService.cs            # 实验室服务
    └── EquipmentService.cs      # 设备服务
```

---

## 数据库实体 (18个表)

### 系统管理模块
| 表名 | 说明 |
|------|------|
| users | 用户表 |
| roles | 角色表 |
| permissions | 权限表 |
| user_roles | 用户角色关联表 |
| role_permissions | 角色权限关联表 |
| departments | 部门表 |

### 教学管理模块
| 表名 | 说明 |
|------|------|
| semesters | 学期表 |
| academic_calendars | 校历表 |
| courses | 课程表 |
| majors | 专业表 |
| classes | 班级表 |
| class_students | 班级学生关联表 |
| teaching_tasks | 教学任务表 |
| teaching_task_teachers | 教学任务教师关联表 |
| period_times | 节次时间表 |

### 实验室管理模块
| 表名 | 说明 |
|------|------|
| labs | 实验室表 |
| equipments | 设备表 |

---

## 前端结构 (frontend/)

### 目录结构

```
frontend/
├── package.json                # 依赖配置
├── vite.config.ts              # Vite 配置
├── tsconfig.json               # TypeScript 配置
├── index.html                  # HTML 入口
└── src/
    ├── main.ts                 # 入口文件
    ├── App.vue                 # 根组件
    ├── api/                    # API 接口层
    │   ├── lab.ts              # 实验室 API
    │   ├── system.ts           # 系统管理 API
    │   └── teaching.ts         # 教学管理 API
    ├── stores/                 # 状态管理 (Pinia)
    │   └── auth.ts             # 认证状态
    ├── router/                 # 路由配置
    │   └── index.ts            # 路由 + 守卫
    ├── directives/             # 指令
    │   └── permission.ts       # 权限指令
    ├── utils/                  # 工具函数
    │   └── request.ts          # Axios 封装
    └── views/                  # 页面视图
        ├── LoginView.vue        # 登录页
        ├── HomeView.vue         # 主页布局 (含侧边栏导航)
        ├── system/              # 系统管理模块
        │   ├── UsersView.vue    # 用户管理页
        │   ├── RolesView.vue    # 角色管理页
        │   ├── PermissionsView.vue  # 权限查看页
        │   ├── DepartmentsView.vue # 部门管理页
        │   └── components/      # 弹窗组件
        │       ├── UserFormDialog.vue
        │       ├── UserRolesDialog.vue
        │       ├── RoleFormDialog.vue
        │       ├── RolePermissionsDialog.vue
        │       └── DepartmentFormDialog.vue
        ├── teaching/           # 教学管理模块
        │   ├── SemestersView.vue    # 学期管理
        │   ├── CoursesView.vue      # 课程管理
        │   ├── MajorsView.vue       # 专业管理
        │   ├── ClassesView.vue      # 班级管理
        │   ├── TeachingTasksView.vue  # 教学任务
        │   ├── PeriodTimesView.vue  # 节次时间
        │   └── components/          # 弹窗组件
        │       ├── SemesterFormDialog.vue
        │       ├── CalendarViewDialog.vue
        │       ├── CourseFormDialog.vue
        │       ├── MajorFormDialog.vue
        │       ├── ClassFormDialog.vue
        │       ├── ClassStudentsDialog.vue
        │       ├── TeachingTaskFormDialog.vue
        │       └── PeriodTimeFormDialog.vue
        └── lab/                # 实验室管理模块
            ├── LabsView.vue         # 实验室管理
            ├── EquipmentsView.vue   # 设备管理
            └── components/          # 弹窗组件
```

---

## 权限系统

### 权限编码规范

格式: `{module}:{action}`

| 模块 | 权限代码 | 说明 |
|------|----------|------|
| user | user:create, user:read, user:update, user:delete, user:reset_password | 用户管理 |
| role | role:create, role:read, role:update, role:delete, role:assign | 角色管理 |
| permission | permission:read, permission:assign | 权限管理 |
| department | department:create, department:read, department:update, department:delete | 部门管理 |
| course | course:create, course:read, course:update, course:delete, course:schedule | 课程管理 |
| major | major:create, major:read, major:update, major:delete | 专业管理 |
| class | class:create, class:read, class:update, class:delete | 班级管理 |
| period_time | period_time:create, period_time:read, period_time:update, period_time:delete | 节次管理 |
| lab | lab:create, lab:read, lab:update, lab:delete | 实验室管理 |
| equipment | equipment:create, equipment:read, equipment:update, equipment:delete | 设备管理 |
| calendar | calendar:read, calendar:update | 校历管理 |

### 预定义角色

| 角色编码 | 角色名称 | 说明 |
|----------|---------|------|
| super_admin | 超级管理员 | 拥有所有权限 |
| lab_admin | 实验室管理员 | 管理实验室、设备 |
| teacher | 教师 | 课程管理、学生管理 |
| student | 学生 | 预约设备、提交报告 |
| auditor | 审计员 | 查看日志、报表 |

---

## 启动命令

### 后端启动

```bash
cd backend/LimsAuth.Api
dotnet run --urls "http://0.0.0.0:5047"
```

### 前端启动

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

## 相关文档

| 文档 | 路径 | 说明 |
|------|------|------|
| 项目主文档 | [README.md](../README.md) | 项目总览 |
| 权限系统设计 | [RBAC_DESIGN.md](./RBAC_DESIGN.md) | RBAC 设计详情 |
| 教学系统设计 | [TEACHING_SYSTEM_DESIGN.md](./TEACHING_SYSTEM_DESIGN.md) | 教学模块设计 |
| 教学模块参考 | [TEACHING_README.md](./TEACHING_README.md) | 教学模块快速参考 |
| 权限策略整理 | [PERMISSION_REFACTOR.md](./PERMISSION_REFACTOR.md) | 权限配置建议 |
| 已知问题 | [KNOWN_ISSUES.md](./KNOWN_ISSUES.md) | 缺陷与风险清单 |
