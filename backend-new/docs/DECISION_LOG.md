# 决策记录

> 本文档记录 backend-new 后端重构过程中的所有关键技术决策。

---

## 1. 项目整体决策

### 1.1 数据库精简设计
- **决策**: 将原有分散在多个模块的数据库合并为一套统一系统
- **理由**: 需求文档覆盖了基础教学、排课预约、设备管理、耗材管理四大模块,统一数据库更易于管理和维护
- **日期**: 2026-06-11

### 1.2 技术选型
- **框架**: .NET 8.0 ASP.NET Core Web API
- **数据库**: SQLite + Entity Framework Core 8.0
- **认证**: JWT Bearer Token
- **文档**: Swagger/OpenAPI
- **理由**: 与原项目技术栈保持一致,降低学习成本,SQLite适合快速开发和小型部署

---

## 2. 数据库设计决策

### 2.1 主键类型: GUID
- **决策**: 所有表使用 GUID 作为主键
- **理由**: 
  - 分布式环境下更友好
  - 避免ID猜测攻击
  - 合并数据时不会冲突
- **替代方案**: 自增INT (被否决:不适合分布式)

### 2.2 审计字段
- **决策**: 每张业务表统一包含 CreatedAt/CreatedBy/UpdatedAt/UpdatedBy
- **理由**: 便于追踪数据变更历史,符合企业级应用规范

### 2.3 软删除
- **决策**: 使用 IsDeleted 位标记实现软删除
- **理由**: 保护数据安全,支持误删恢复
- **替代方案**: 物理删除 (被否决:数据不可恢复)

### 2.4 表名命名规范
- **决策**: 使用 模块前缀_表名 格式 (如 Sys_User, Edu_Course, Ven_Building)
- **前缀映射**:
  - Sys_ = 系统基础
  - Edu_ = 基础教学
  - Ven_ = 场地管理
  - Lab_ = 排课预约
  - Dev_ = 设备管理
  - Mat_ = 耗材管理
- **理由**: 清晰区分业务模块,便于理解和维护

### 2.5 字段名映射
- **决策**: 数据库列名使用 PascalCase (如 UserName, PasswordHash)
- **理由**: 符合 .NET 命名惯例,通过 EF Core Fluent API 显式映射到数据库蛇形命名

### 2.6 库存设计 (耗材)
- **决策**: 耗材表同时存储 CurrentStock(总库存)、AvailableStock(可用库存)、LockedStock(锁定库存)
- **理由**: 满足审批流程中的库存预占需求,支持出入库原子操作

---

## 3. API 设计决策

### 3.1 响应格式
- **决策**: 统一使用 ApiResponse<T> 包装所有响应
- **格式**:
```json
{
  "code": 200,
  "message": "success",
  "data": { ... }
}
```
- **理由**: 前后端通信格式统一,便于统一错误处理和日志记录

### 3.2 分页格式
- **决策**: 使用 PagedResponse<T> 统一分页格式
- **理由**: 所有列表接口保持一致的分页体验

### 3.3 权限注解
- **决策**: 使用 ASP.NET Core Authorization Policy + 自定义 PermissionHandler
- **Policy格式**: `Permission:{module}:{action}`
- **理由**: 
  - 细粒度权限控制
  - 便于动态扩展权限点
  - 与 SeedData 中的权限码一致

### 3.4 权限模式
- **决策**: 查询操作需要 [Authorize], 写操作需要具体 Permission 策略
- **理由**: 平衡安全性和开发效率

---

## 4. 架构决策

### 4.1 分层架构
- **决策**: Controller -> Service -> DbContext -> Database
- **理由**: 清晰的职责分离,便于测试和维护

### 4.2 Service 组织方式
- **决策**: 按业务模块合并 Service 到单个文件
- **理由**: 减少文件数量,便于快速定位
- **例如**: ClassTeachingTaskService.cs 包含 IClassService + ClassService + ITeachingTaskService + TeachingTaskService

### 4.3 DTO 组织方式
- **决策**: 按业务模块合并 DTO 到单个文件
- **理由**: 减少文件数量,DTO 通常一起使用
- **例如**: TeachingDtos.cs 包含 Semester/Course/Major/Class/TeachingTask 相关 DTO

### 4.4 Entity Configuration
- **决策**: 使用 IEntityTypeConfiguration 实现 Fluent API 配置
- **理由**: 分离配置代码,保持实体类纯净
- **每个模块一个配置文件**: SystemConfiguration.cs, TeachingConfiguration.cs 等

---

## 5. 种子数据决策

### 5.1 初始角色
- **决策**: 创建 3 个初始角色 (super_admin, teacher, student)
- **理由**: 满足基本测试和管理需求

### 5.2 初始用户
- **决策**: 创建 3 个初始用户 (admin/teacher/student)
- **密码**: admin123 / teacher123 / student123
- **理由**: 便于快速测试和演示

### 5.3 初始权限
- **决策**: 预置 65 个权限点覆盖 19 个模块
- **理由**: 完整权限体系,支持细粒度控制

---

## 6. 待补充决策项

- [ ] 是否引入 Redis 缓存用户权限?
- [ ] 是否添加操作审计日志表?
- [ ] 是否支持多租户?
- [ ] 是否添加消息通知功能?
- [ ] 是否引入数据备份机制?
