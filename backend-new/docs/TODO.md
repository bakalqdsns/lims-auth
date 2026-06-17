# 待办任务

> 本文档记录 backend-new 后端重构的待完成工作。

---

## 一、功能待办

### 高优先级

- [x] 编译验证,修复所有编译错误和警告
  - 完成时间: 2026-06-11
  - 修复内容:
    - 实体文件(6个)添加 `using Microsoft.EntityFrameworkCore;` 和 `using LimsAuth.Api.Data.Configuration;`
    - `ApiCommon.cs` 添加非泛型 `ApiResponse` 类
    - `AuthService.cs` 添加 `using LimsAuth.Api.Models.Entities;`
    - `SeedData.cs` 添加 EF Core using
    - `ConsumableDtos.cs` 的 `InboundDto` 添加 `AuditRemark` 属性
    - `PermissionService.cs` 移除不存在的 `IAuthorizationBuilder` 引用
    - `TeachingDtos.cs` 的 `SemesterDto.CreatedBy` 类型修正为 `Guid?`
    - `ScheduleService.cs` `RegisterUserId` 添加 `?? Guid.Empty`
    - `ConsumableService.cs` `GroupBy+ToDictionary` 改用 foreach 循环绕过编译器歧义
  - 剩余: 12个 CS8601 null赋值警告(非阻塞)
- [ ] 补充 Program.cs 中 PermissionPolicies 的完整注册 (AddPermissionAuthorization 调用)
- [ ] 补充 SeedData 中的初始数据(课程、专业、班级的更多示例数据)
- [ ] 补充 Room.RoomType 枚举值定义

### 中优先级

- [ ] 添加 HealthCheck 健康检查接口
- [ ] 添加全局异常处理中间件
- [ ] 添加请求日志中间件
- [ ] 补充 IAuthorizationHandler 的完整实现(PermissionHandler 需要注入 DbContext)
- [ ] 添加请求验证 (DataAnnotations + FluentValidation)
- [ ] 补充 Pagination 的默认排序

### 低优先级

- [ ] 添加 Redis 缓存支持(用户权限缓存)
- [ ] 添加操作审计日志表和中间件
- [ ] 添加消息通知功能 (站内信/邮件)
- [ ] 添加数据备份机制
- [ ] 补充 Excel 导入导出功能 (EPPlus)
- [ ] 添加 WebSocket 实时通知

---

## 二、Bug修复待办

### 已识别问题

- [ ] PermissionHandler 需要注入 AppDbContext 来验证权限
- [ ] LoanApplyService.CreateAsync 中参数名引用错误 (request.AssetId vs request.AssetID)
- [ ] SeedData 中 IsCurrent 类型为 int 而非 bool
- [ ] 部分 Service 方法参数名与 DTO 属性名不匹配 (需统一检查)

### 需要验证

- [ ] JWT Token 过期后刷新机制
- [ ] 密码修改后 Token 是否应该失效
- [ ] 角色删除前的依赖检查(是否有关联用户)
- [ ] 部门/机构删除前的依赖检查

---

## 三、优化待办

### 性能优化

- [ ] 列表查询添加 AsNoTracking()
- [ ] 补充必要的数据库索引
- [ ] 大列表查询添加服务器端分页

### 安全优化

- [ ] 密码强度验证
- [ ] 登录失败次数限制
- [ ] SQL注入防护(参数化查询 EF Core已默认处理)
- [ ] XSS防护(输入校验+输出编码)

### 代码质量

- [ ] 补充所有 Controller 和 Service 的 XML 注释
- [ ] 添加单元测试项目
- [ ] 添加集成测试
- [ ] 配置 StyleCop/Analyser 规则

---

## 四、文档待办

- [ ] 补充 docs/README.md 主索引文档
- [ ] 补充 docs/TODO.md 本文件(待办任务)
- [ ] 补充 README.md 中的快速开始指南(编译、运行命令)
  - 完成时间: 2026-06-11
  - 补充内容: 前置要求、还原依赖、自动建库流程、Swagger 认证步骤、配置文件说明
- [ ] 添加 API 变更日志 (CHANGELOG.md)
- [ ] 添加 CONTRIBUTING.md (贡献指南)
- [ ] 添加 DEPLOY.md (部署指南)
