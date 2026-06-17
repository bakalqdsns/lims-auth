# LIMS 高校实验室管理系统 - 项目文档

> **最后更新:** 2026-06-11
>
> 本文档为项目主索引，描述整体架构和各子项目结构。

---

## 项目目录

```
lims-auth/
├── backend-new/                     # .NET 8.0 后端（重构版本）
│   ├── lims-auth.sln
│   ├── README.md                          # 项目总览 + 快速开始
│   ├── test-client/                       # 最简 API 测试前端（单文件 HTML）
│   │   └── index.html                    # 直接在浏览器打开即可测试所有 API
│   ├── src/
│   │   └── LimsAuth.Api/          # 主项目
│   └── docs/                       # 后端文档
│       ├── DATABASE_DESIGN.md      # 数据库设计
│       ├── API_SPEC.md             # API 接口规范
│       ├── PROJECT_STRUCTURE.md    # 项目结构
│       ├── DECISION_LOG.md         # 技术决策记录
│       ├── DEV_LOG.md              # 开发日志
│       └── TODO.md                 # 待办任务
├── frontend/                       # Vue 3 前端
├── backend/                        # .NET 8.0 后端（旧版，参考用）
└── docs/                          # 项目总文档（旧版）
```

---

## backend-new 技术栈

- **框架**: .NET 8.0 ASP.NET Core Web API
- **数据库**: SQLite + Entity Framework Core 8.0（含 IEntityTypeConfiguration fluent API）
- **认证**: JWT Bearer Token
- **权限**: 自定义 RBAC（PermissionPolicy + 策略授权）
- **API 文档**: Swagger (Swashbuckle)
- **密码加密**: BCrypt.Net-Next

---

## backend-new 目录结构

```
backend-new/src/LimsAuth.Api/
├── LimsAuth.Api.csproj             # 项目文件
├── Program.cs                      # 应用入口 + 服务注册
├── appsettings.json                # 配置文件
├── Authorization/
│   └── PermissionPolicies.cs       # 权限策略定义
├── Controllers/                    # API 控制器 (14个)
│   ├── AuthController.cs           # 认证（登录/改密/资料）
│   ├── UsersController.cs          # 用户管理
│   ├── RolesController.cs          # 角色管理
│   ├── PermissionsController.cs    # 权限管理
│   ├── InstitutionsController.cs    # 机构管理
│   ├── DepartmentsController.cs    # 部门管理
│   ├── SemestersController.cs      # 学期管理
│   ├── CoursesController.cs        # 课程管理
│   ├── MajorsController.cs         # 专业管理
│   ├── ClassesController.cs        # 班级管理
│   ├── TeachingTasksController.cs   # 教学任务管理
│   ├── BuildingsRoomsController.cs  # 楼宇与场地管理
│   ├── ScheduleControllers.cs      # 排课/预约/使用登记
│   ├── ConsumableController.cs     # 耗材管理（入库/出库/统计）
│   └── AssetLoanController.cs       # 设备借还管理
├── Data/
│   ├── AppDbContext.cs            # EF Core DbContext
│   ├── SeedData.cs                # 种子数据初始化
│   └── Configuration/            # IEntityTypeConfiguration 配置类
│       ├── SystemConfiguration.cs      # 系统表配置
│       ├── TeachingConfiguration.cs    # 教学表配置
│       ├── VenConfiguration.cs        # 场地表配置
│       ├── ScheduleConfiguration.cs   # 排课表配置
│       ├── DeviceConfiguration.cs     # 设备表配置
│       └── ConsumableConfiguration.cs  # 耗材表配置
├── Models/
│   ├── ApiCommon.cs               # ApiResponse<T> / PagedResponse<T>
│   ├── QueryModels.cs             # 分页查询基类
│   ├── Entities/                 # 实体类（6个文件，25张表）
│   │   ├── SystemEntities.cs     # Sys_User/Role/Permission/Institution/Department
│   │   ├── TeachingEntities.cs     # Edu_Semester/Course/Major/Class/TeachingTask
│   │   ├── VenEntities.cs        # Ven_Building / Lab_Room
│   │   ├── ScheduleEntities.cs   # Lab_Schedule/ExperimentItem/BookingApply/UsageRegister
│   │   ├── DeviceEntities.cs      # Dev_Asset / Dev_LoanApply
│   │   └── ConsumableEntities.cs # Mat_Consumable/InboundOrder/OutboundOrder/StockLog
│   └── DTOs/                     # 数据传输对象
│       ├── SystemDtos.cs          # 用户/角色/权限/机构/部门 DTO
│       ├── TeachingDtos.cs        # 学期/课程/专业/班级/教学任务 DTO
│       ├── VenDtos.cs             # 楼宇/场地 DTO
│       ├── ScheduleDtos.cs        # 排课/预约/使用登记 DTO
│       ├── DeviceDtos.cs          # 设备资产/借还 DTO
│       └── ConsumableDtos.cs      # 耗材/入库/出库/统计 DTO
└── Services/                      # 业务逻辑层
    ├── AuthService.cs             # 认证服务
    ├── UserService.cs             # 用户服务
    ├── RoleService.cs             # 角色服务
    ├── PermissionService.cs       # 权限服务
    ├── InstitutionService.cs      # 机构服务
    ├── DepartmentService.cs       # 部门服务
    ├── SemesterCourseMajorService.cs  # 学期/课程/专业服务
    ├── ClassTeachingTaskService.cs    # 班级/教学任务服务
    ├── BuildingRoomService.cs    # 楼宇/场地服务
    ├── ScheduleService.cs        # 排课/预约/使用登记服务
    ├── AssetLoanService.cs       # 设备借还服务
    ├── ConsumableService.cs      # 耗材管理服务
    └── InboundService.cs / OutboundService.cs / StockLogService.cs
```

---

## 数据库实体 (25张表)

| 模块 | 表名 | 说明 |
|------|------|------|
| 系统基础 | Sys_User, Sys_Role, Sys_Permission, Sys_UserRole, Sys_RolePermission, Sys_Institution, Sys_Department | 用户、角色、权限、机构、部门 |
| 基础教学 | Edu_Semester, Edu_Course, Edu_Major, Edu_Class, Edu_ClassStudent, Edu_TeachingTask, Edu_TeachingTaskTeacher | 学期、课程、专业、班级、教学任务 |
| 场地管理 | Ven_Building, Ven_Room | 楼宇、实验室场地 |
| 排课预约 | Lab_Schedule, Lab_ExperimentItem, Lab_BookingApply, Lab_UsageRegister | 排课记录、实验项目、预约申请、使用登记 |
| 设备管理 | Dev_Asset, Dev_LoanApply | 设备资产台账、设备借还申请 |
| 耗材管理 | Mat_Consumable, Mat_InboundOrder, Mat_OutboundOrder, Mat_StockLog | 耗材基础、入库单、出库单、库存变动日志 |

详细字段定义见 [DATABASE_DESIGN.md](backend-new/docs/DATABASE_DESIGN.md)。

---

## 快速开始

### 前置要求

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)（建议最新稳定版）
- Node.js 18+（仅前端需要）

### 1. 克隆并进入目录

```bash
cd e:\lims-auth\backend-new
```

### 2. 还原依赖

```bash
dotnet restore
```

### 3. 启动后端（自动建库并初始化种子数据）

```bash
cd src/LimsAuth.Api
dotnet run
```

启动时自动执行：
1. `Database.EnsureCreated()` — 若 `lims.db` 不存在则创建 SQLite 数据库
2. `SeedData.SeedAsync()` — 初始化 3 个角色、3 个用户、65 个权限点等种子数据
3. `PermissionService.RegisterPermissionPoliciesAsync()` — 向 ASP.NET Core Authorization 注册所有权限策略

### 4. 访问服务

| 服务 | 地址 |
|------|------|
| 后端 API | http://localhost:5047 |
| Swagger 文档 | http://localhost:5047/swagger |

### 5. 首次登录

使用测试账号登录（获取 JWT Token）：

```
POST http://localhost:5047/api/v1/auth/login
Content-Type: application/json

{
  "username": "admin",
  "password": "admin123"
}
```

响应示例：

```json
{
  "code": 200,
  "message": "success",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "tokenType": "Bearer",
    "expiresIn": 43200,
    "user": {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "username": "admin",
      "realName": "管理员",
      "email": "admin@lims.edu.cn"
    }
  }
}
```

在 Swagger 页面右上角点击 **Authorize** 按钮，输入 `Bearer {token}`，即可测试需要授权的接口。

### 6. 数据库文件

数据库文件位于 `backend-new/src/LimsAuth.Api/lims.db`（SQLite）。如需重置数据库，删除该文件后重新运行 `dotnet run` 即可。

### 7. 配置文件

主要配置项位于 `src/LimsAuth.Api/appsettings.json`：

| 配置项 | 默认值 | 说明 |
|--------|--------|------|
| `ConnectionStrings.DefaultConnection` | `Data Source=lims.db` | SQLite 数据库路径（相对于项目根目录） |
| `Jwt.SecretKey` | `LimsAuth_SuperSecretKey_2026_MinLength32Chars!` | JWT 签名密钥（生产环境请更换） |
| `Jwt.Issuer` | `LimsAuth.Api` | JWT 签发者 |
| `Jwt.Audience` | `LimsAuth.Client` | JWT 接收方 |
| `Jwt.ExpirationHours` | `12` | Token 有效期（小时） |
| `Urls` | `http://0.0.0.0:5047` | 服务监听地址 |

### 8. API 测试客户端

项目包含一个最简的 API 测试前端，位于 `backend-new/test-client/index.html`，无需任何构建工具，直接在浏览器中打开即可使用。

功能：
- JWT 登录/登出（Token 自动持久化到 localStorage）
- 12 个快速测试按钮（权限/角色/学期/机构/部门/课程/专业/班级/楼宇/场地/耗材/设备）
- 自定义请求（支持 GET/POST/PUT/DELETE，自定义路径和 JSON Body）
- 彩色语法高亮响应展示（成功绿色/失败红色）
- 请求历史记录（最近 20 条）
- 支持切换后端地址

使用方式：直接在浏览器中打开 `backend-new/test-client/index.html` 即可。

---

## 测试账号

| 账号 | 密码 | 角色 |
|------|------|------|
| admin | admin123 | 超级管理员 |
| teacher | teacher123 | 教师 |
| student | student123 | 学生 |

---

## API 接口概览

所有 API 遵循统一响应格式：

```json
{
  "code": 200,
  "message": "success",
  "data": { ... }
}
```

分页响应：

```json
{
  "code": 200,
  "message": "success",
  "data": {
    "items": [...],
    "total": 100,
    "page": 1,
    "pageSize": 20,
    "totalPages": 5
  }
}
```

详细接口规范见 [API_SPEC.md](backend-new/docs/API_SPEC.md)。

---

## 权限编码规范

格式: `{module}:{action}`

| 模块 | 权限示例 |
|------|---------|
| user | user:create, user:read, user:update, user:delete |
| role | role:create, role:read, role:update, role:delete, role:assign |
| permission | permission:read |
| institution | institution:create, institution:read, institution:update, institution:delete |
| department | department:create, department:read, department:update, department:delete |
| semester | semester:create, semester:read, semester:update, semester:delete, semester:set_current |
| course | course:create, course:read, course:update, course:delete |
| major | major:create, major:read, major:update, major:delete |
| class | class:create, class:read, class:update, class:delete |
| teachingtask | teachingtask:create, teachingtask:read, teachingtask:update, teachingtask:delete |
| building | building:create, building:read, building:update, building:delete |
| room | room:create, room:read, room:update, room:delete |
| schedule | schedule:create, schedule:read, schedule:update, schedule:delete |
| booking | booking:create, booking:read, booking:update, booking:delete, booking:audit |
| usage | usage:create, usage:read, usage:update, usage:delete |
| asset | asset:create, asset:read, asset:update, asset:delete |
| loan | loan:create, loan:read, loan:update, loan:audit, loan:return |
| consumable | consumable:create, consumable:read, consumable:update, consumable:delete |
| experimentitem | experimentitem:create, experimentitem:read, experimentitem:update, experimentitem:delete |

---

## 后端文档索引

| 文档 | 说明 |
|------|------|
| [DATABASE_DESIGN.md](backend-new/docs/DATABASE_DESIGN.md) | 数据库设计（25张表，字段定义，索引，ER图） |
| [API_SPEC.md](backend-new/docs/API_SPEC.md) | API 接口规范（请求/响应格式，认证，权限） |
| [PROJECT_STRUCTURE.md](backend-new/docs/PROJECT_STRUCTURE.md) | 项目结构说明 |
| [DECISION_LOG.md](backend-new/docs/DECISION_LOG.md) | 技术决策记录 |
| [DEV_LOG.md](backend-new/docs/DEV_LOG.md) | 开发日志 |
| [TODO.md](backend-new/docs/TODO.md) | 待办任务（含编译修复记录） |

---

## 前端结构 (frontend/)

### 技术栈
- **框架**: Vue 3 + TypeScript
- **构建工具**: Vite 5
- **UI 库**: Element Plus 2.5
- **状态管理**: Pinia
- **路由**: Vue Router 4
- **HTTP**: Axios

### 启动命令

```bash
cd frontend
npm run dev
# 前端: http://localhost:5173
```
