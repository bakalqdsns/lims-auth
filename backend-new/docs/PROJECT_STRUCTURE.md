# LIMS 高校实验室管理系统 - 项目结构

> 本文档描述 backend-new 目录下的 .NET 8.0 后端项目结构。

---

## 项目概览

```
backend-new/
├── lims-auth.sln                        # Visual Studio 解决方案文件
├── README.md                            # 项目主文档
└── src/
    └── LimsAuth.Api/                   # 主 API 项目
        ├── LimsAuth.Api.csproj          # 项目文件
        ├── Program.cs                   # 应用入口
        ├── appsettings.json             # 配置文件
        ├── appsettings.Development.json  # 开发环境配置
        │
        ├── Controllers/                # API 控制器 (15个文件, 23个控制器)
        │   ├── AuthController.cs       # 认证控制器
        │   ├── UsersController.cs      # 用户管理
        │   ├── RolesController.cs      # 角色管理
        │   ├── PermissionsController.cs # 权限管理
        │   ├── InstitutionsController.cs # 机构管理
        │   ├── DepartmentsController.cs # 部门管理
        │   ├── SemestersController.cs  # 学期管理
        │   ├── CoursesController.cs    # 课程管理
        │   ├── MajorsController.cs    # 专业管理
        │   ├── ClassesController.cs    # 班级管理
        │   ├── TeachingTasksController.cs # 教学任务
        │   ├── BuildingsRoomsController.cs # 楼宇 + 实验室
        │   ├── ScheduleControllers.cs  # 排课 + 实验项目 + 预约 + 使用登记
        │   ├── AssetLoanController.cs # 设备资产 + 借还
        │   └── ConsumableController.cs  # 耗材 + 入库 + 出库 + 库存日志
        │
        ├── Services/                   # 业务逻辑层 (11个文件)
        │   ├── AuthService.cs         # 认证服务
        │   ├── UserService.cs         # 用户服务
        │   ├── RoleService.cs         # 角色服务
        │   ├── PermissionService.cs    # 权限服务
        │   ├── InstitutionDepartmentService.cs # 机构 + 部门
        │   ├── SemesterCourseMajorService.cs # 学期 + 课程 + 专业
        │   ├── ClassTeachingTaskService.cs # 班级 + 教学任务
        │   ├── BuildingRoomService.cs # 楼宇 + 实验室
        │   ├── ScheduleService.cs    # 排课 + 预约 + 使用登记
        │   ├── AssetLoanService.cs   # 设备 + 借还
        │   └── ConsumableService.cs   # 耗材 + 出入库 + 库存日志
        │
        ├── Models/                    # 数据模型
        │   ├── ApiCommon.cs           # 通用响应、分页请求
        │   ├── QueryModels.cs         # 查询参数模型
        │   ├── Entities/              # 数据库实体 (6个文件)
        │   │   ├── SystemEntities.cs  # 用户/角色/权限/机构/部门
        │   │   ├── TeachingEntities.cs # 学期/课程/专业/班级/教学任务
        │   │   ├── VenEntities.cs    # 楼宇/实验室
        │   │   ├── ScheduleEntities.cs # 排课/预约/使用登记
        │   │   ├── DeviceEntities.cs # 设备/借还
        │   │   └── ConsumableEntities.cs # 耗材/出入库/库存日志
        │   └── DTOs/                # 数据传输对象 (6个文件)
        │       ├── SystemDtos.cs    # 系统管理DTO
        │       ├── TeachingDtos.cs   # 教学管理DTO
        │       ├── VenDtos.cs       # 场地管理DTO
        │       ├── ScheduleDtos.cs  # 排课预约DTO
        │       ├── DeviceDtos.cs    # 设备管理DTO
        │       └── ConsumableDtos.cs # 耗材管理DTO
        │
        ├── Data/                      # 数据访问层
        │   ├── AppDbContext.cs      # EF Core 数据库上下文
        │   └── Configuration/        # Fluent API 配置 (6个文件)
        │       ├── SystemConfiguration.cs
        │       ├── TeachingConfiguration.cs
        │       ├── VenConfiguration.cs
        │       ├── ScheduleConfiguration.cs
        │       ├── DeviceConfiguration.cs
        │       └── ConsumableConfiguration.cs
        │   └── SeedData.cs          # 种子数据
        │
        ├── Authorization/             # 权限认证
        │   └── PermissionPolicies.cs # 权限策略定义 + 处理器
        │
        └── docs/                    # 项目文档
            ├── DATABASE_DESIGN.md     # 数据库设计 (25张表全部字段)
            ├── API_SPEC.md           # API 接口详细规范
            ├── PROJECT_STRUCTURE.md   # 项目结构 (本文件)
            ├── DECISION_LOG.md       # 技术决策记录
            ├── DEV_LOG.md            # 开发日志
            └── TODO.md               # 待办任务
```

---

## 层级职责

### Controllers (API 层)
- 接收 HTTP 请求
- 参数验证
- 调用 Service 层
- 返回统一格式的响应
- 添加 `[Authorize]` 权限注解

### Services (业务逻辑层)
- 封装业务逻辑
- 数据校验
- 事务管理
- 调用 DbContext 进行数据访问

### Models/Entities (实体层)
- 数据库表的代码映射
- 包含数据验证注解
- 与 EF Core Configuration 配合定义表结构

### Models/DTOs (数据传输对象层)
- API 请求/响应数据结构
- 与前端数据契约
- 包含验证注解

### Data (数据访问层)
- AppDbContext: 数据库上下文,管理数据库连接和实体集
- Configuration: EF Core Fluent API 配置
- SeedData: 初始数据

### Authorization (权限认证层)
- 定义权限策略名称
- 实现权限验证处理器
- 注册授权服务

---

## 命名规范

| 类型 | 规范 | 示例 |
|------|------|------|
| Controller | {Entity}Controller.cs | UsersController.cs |
| Service接口 | I{Entity}Service | IUserService |
| Service实现 | {Entity}Service | UserService |
| Entity | {Prefix}_{EntityName} | Sys_User |
| DTO请求 | Create{Entity}Request / Update{Entity}Request | CreateUserRequest |
| DTO响应 | {Entity}Dto / {Entity}ListDto | UserDto |
| 查询DTO | {Entity}QueryRequest | UserQueryRequest |
| 权限策略 | Permission:{module}:{action} | Permission:user:create |

---

## 权限策略清单

| 模块 | 权限策略 |
|------|----------|
| user | Permission:user:create, Permission:user:read, Permission:user:update, Permission:user:delete |
| role | Permission:role:create, Permission:role:read, Permission:role:update, Permission:role:delete |
| permission | Permission:permission:read, Permission:permission:assign |
| institution | Permission:institution:create, Permission:institution:read, Permission:institution:update, Permission:institution:delete |
| department | Permission:department:create, Permission:department:read, Permission:department:update, Permission:department:delete |
| semester | Permission:semester:create, Permission:semester:read, Permission:semester:update, Permission:semester:delete |
| course | Permission:course:create, Permission:course:read, Permission:course:update, Permission:course:delete |
| major | Permission:major:create, Permission:major:read, Permission:major:update, Permission:major:delete |
| class | Permission:class:create, Permission:class:read, Permission:class:update, Permission:class:delete |
| teachingtask | Permission:teachingtask:create, Permission:teachingtask:read, Permission:teachingtask:update, Permission:teachingtask:delete |
| building | Permission:building:create, Permission:building:read, Permission:building:update, Permission:building:delete |
| room | Permission:room:create, Permission:room:read, Permission:room:update, Permission:room:delete |
| schedule | Permission:schedule:create, Permission:schedule:read, Permission:schedule:update, Permission:schedule:delete |
| booking | Permission:booking:create, Permission:booking:read, Permission:booking:approve, Permission:booking:delete |
| usage | Permission:usage:read, Permission:usage:create |
| experimentitem | Permission:experimentitem:create, Permission:experimentitem:read, Permission:experimentitem:update, Permission:experimentitem:delete |
| asset | Permission:asset:create, Permission:asset:read, Permission:asset:update, Permission:asset:delete |
| loan | Permission:loan:create, Permission:loan:read, Permission:loan:approve |
| consumable | Permission:consumable:create, Permission:consumable:read, Permission:consumable:update, Permission:consumable:delete, Permission:consumable:approve |
