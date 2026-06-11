# 开发日志

> 本文档记录 backend-new 后端重构的开发进度和每次变更内容。

---

## 开发进度

### 2026-06-11 - 项目骨架搭建完成

**完成内容**:
1. 创建 .NET 8.0 Web API 项目结构
2. 配置 Program.cs (JWT认证、Swagger、服务注册)
3. 创建 appsettings.json 配置
4. 创建 solution 文件

**文件清单**:
- backend-new/lims-auth.sln
- backend-new/src/LimsAuth.Api/LimsAuth.Api.csproj
- backend-new/src/LimsAuth.Api/Program.cs
- backend-new/src/LimsAuth.Api/appsettings.json
- backend-new/src/LimsAuth.Api/appsettings.Development.json

---

### 2026-06-11 - 数据模型层完成

**完成内容**:
1. 创建全部 26 个 Entity 实体类
2. 创建 6 个 Entity Configuration 文件 (Fluent API)
3. 创建 AppDbContext 数据库上下文
4. 创建 SeedData 种子数据

**实体清单**:
- SystemEntities.cs: SysUser, SysRole, SysPermission, SysUserRole, SysRolePermission, SysInstitution, SysDepartment (7个)
- TeachingEntities.cs: EduSemester, EduCourse, EduMajor, EduClass, EduClassStudent, EduTeachingTask, EduTeachingTaskTeacher (7个)
- VenEntities.cs: VenBuilding, LabRoom (2个)
- ScheduleEntities.cs: LabSchedule, LabExperimentItem, LabBookingApply, LabUsageRegister (4个)
- DeviceEntities.cs: DevAsset, DevLoanApply (2个)
- ConsumableEntities.cs: MatConsumable, MatInboundOrder, MatOutboundOrder, MatStockLog (4个)

**配置文件清单**:
- Data/Configuration/SystemConfiguration.cs
- Data/Configuration/TeachingConfiguration.cs
- Data/Configuration/VenConfiguration.cs
- Data/Configuration/ScheduleConfiguration.cs
- Data/Configuration/DeviceConfiguration.cs
- Data/Configuration/ConsumableConfiguration.cs

---

### 2026-06-11 - DTO层完成

**完成内容**:
1. 创建通用响应 DTO (ApiCommon.cs)
2. 创建查询模型 (QueryModels.cs)
3. 创建系统管理 DTO (SystemDtos.cs)
4. 创建教学管理 DTO (TeachingDtos.cs)
5. 创建场地管理 DTO (VenDtos.cs)
6. 创建排课预约 DTO (ScheduleDtos.cs)
7. 创建设备管理 DTO (DeviceDtos.cs)
8. 创建耗材管理 DTO (ConsumableDtos.cs)

**DTO文件清单**:
- Models/ApiCommon.cs: ApiResponse, PagedResponse, PagedQuery, ToggleStatusRequest
- Models/QueryModels.cs: PagedQueryRequest, UserQueryRequest, RoleQueryRequest
- Models/DTOs/SystemDtos.cs: 认证/用户/角色/权限/机构/部门 DTO
- Models/DTOs/TeachingDtos.cs: 学期/课程/专业/班级/教学任务 DTO
- Models/DTOs/VenDtos.cs: 楼宇/实验室 DTO
- Models/DTOs/ScheduleDtos.cs: 排课/预约/使用登记/统计 DTO
- Models/DTOs/DeviceDtos.cs: 设备资产/借还 DTO
- Models/DTOs/ConsumableDtos.cs: 耗材/出入库/库存日志 DTO

---

### 2026-06-11 - Service层完成

**完成内容**:
1. 创建 AuthService (JWT生成、登录、密码修改)
2. 创建 UserService (用户CRUD、角色分配)
3. 创建 RoleService (角色CRUD、权限分配)
4. 创建 PermissionService (权限管理)
5. 创建 InstitutionService/DepartmentService (机构部门树)
6. 创建 SemesterService (学期管理)
7. 创建 CourseService (课程CRUD)
8. 创建 MajorService (专业CRUD)
9. 创建 ClassService/TeachingTaskService (班级和教学任务)
10. 创建 BuildingService/RoomService (楼宇和实验室)
11. 创建 ScheduleService/ExperimentItemService/BookingApplyService/UsageRegisterService
12. 创建 AssetService/LoanApplyService
13. 创建 ConsumableService/InboundService/OutboundService/StockLogService

**Service文件清单**:
- Services/AuthService.cs
- Services/UserService.cs
- Services/RoleService.cs
- Services/PermissionService.cs
- Services/InstitutionDepartmentService.cs
- Services/SemesterCourseMajorService.cs
- Services/ClassTeachingTaskService.cs
- Services/BuildingRoomService.cs
- Services/ScheduleService.cs
- Services/AssetLoanService.cs
- Services/ConsumableService.cs

---

### 2026-06-11 - Controller层完成

**完成内容**:
1. 创建 AuthController (认证相关)
2. 创建 UsersController/RolesController/PermissionsController
3. 创建 InstitutionsController/DepartmentsController
4. 创建 SemestersController/CoursesController/MajorsController
5. 创建 ClassesController/TeachingTasksController
6. 创建 BuildingsController/RoomsController
7. 创建 SchedulesController/ExperimentItemsController/BookingAppliesController/UsageRegistersController
8. 创建 AssetsController/LoanAppliesController
9. 创建 ConsumablesController/ConsumableInRecordsController/ConsumableOutRecordsController/ConsumableStockLogsController

**Controller文件清单** (11个文件, 23个控制器):
- Controllers/AuthController.cs
- Controllers/UsersController.cs
- Controllers/RolesController.cs
- Controllers/PermissionsController.cs
- Controllers/InstitutionsController.cs
- Controllers/DepartmentsController.cs
- Controllers/SemestersController.cs
- Controllers/CoursesController.cs
- Controllers/MajorsController.cs
- Controllers/ClassesController.cs
- Controllers/TeachingTasksController.cs
- Controllers/BuildingsRoomsController.cs
- Controllers/ScheduleControllers.cs
- Controllers/AssetLoanController.cs
- Controllers/ConsumableController.cs

---

### 2026-06-11 - Authorization模块完成

**完成内容**:
1. 创建 PermissionPolicies.cs (权限策略注册)
2. 创建 PermissionRequirement (权限需求)
3. 创建 PermissionHandler (权限验证处理器)

**文件清单**:
- Authorization/PermissionPolicies.cs

---

### 2026-06-11 - 文档完成

**完成内容**:
1. 创建 DATABASE_DESIGN.md (25张表全部平铺字段)
2. 创建 API_SPEC.md (完整API接口定义)
3. 创建 DECISION_LOG.md (技术决策记录)
4. 创建 DEV_LOG.md (开发日志)
5. 创建 PROJECT_STRUCTURE.md (项目结构)
6. 创建 TODO.md (待办任务)
7. 创建根目录 README.md (项目总览)

**文档清单**:
- docs/DATABASE_DESIGN.md
- docs/API_SPEC.md
- docs/DECISION_LOG.md
- docs/DEV_LOG.md
- docs/PROJECT_STRUCTURE.md
- docs/TODO.md
- README.md (根目录)

---

## 待完成工作

- [ ] 编译验证,修复所有编译错误
- [ ] 补充 Program.cs 中的权限策略注册
- [ ] 完善单元测试
- [ ] 补充 README.md 中的快速开始指南
- [ ] 添加 HealthCheck 接口
- [ ] 添加全局异常处理中间件
- [ ] 补充 SeedData 中的初始数据(如初始楼栋、实验室等)
