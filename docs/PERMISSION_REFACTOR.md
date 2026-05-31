# 权限策略整理

> 创建日期: 2026-04-15
> 更新日期: 2026-05-27

---

## 当前权限策略状态

### 已注册权限 (Program.cs)

| 模块 | 权限 | 状态 |
|------|------|------|
| user | create, read, update, delete, reset_password | ✅ 已使用 |
| role | create, read, update, delete, assign | ✅ 已使用 |
| permission | read, assign | ✅ 已使用 |
| department | create, read, update, delete | ✅ 已使用 |
| lab | create, read, update, delete | ⚠️ 部分未使用 |
| equipment | create, read, update, delete | ⚠️ 部分未使用 |
| course | create, read, update, delete, schedule | ⚠️ 部分未使用 |
| major | create, read, update, delete | ❌ 全部未使用 |
| class | create, read, update, delete | ❌ 全部未使用 |
| period_time | create, read, update, delete | ❌ 全部未使用 |
| calendar | read, update | ⚠️ 部分未使用 |

### 问题分析

1. **MajorsController/ClassesController**: 之前修复时添加了权限注解，但实际方法上没有使用
2. **LabsController/EquipmentsController/CoursesController**: 部分 read 权限未在方法上使用
3. **权限策略混乱**: 有的控制器在类级别用 `[Authorize]`，有的在具体方法上用权限策略
4. **PeriodTimesController**: 完全没有权限注解
5. **CalendarController**: 只有部分方法有权限控制

---

## 已定义的权限代码

### 系统管理权限

| 权限代码 | 说明 | 使用状态 |
|----------|------|----------|
| user:create | 创建用户 | ✅ |
| user:read | 查看用户 | ✅ |
| user:update | 更新用户 | ✅ |
| user:delete | 删除用户 | ✅ |
| user:reset_password | 重置密码 | ✅ |
| role:create | 创建角色 | ✅ |
| role:read | 查看角色 | ✅ |
| role:update | 更新角色 | ✅ |
| role:delete | 删除角色 | ✅ |
| role:assign | 分配角色 | ✅ |
| permission:read | 查看权限 | ✅ |
| permission:assign | 分配权限 | ✅ |
| department:create | 创建部门 | ✅ |
| department:read | 查看部门 | ✅ |
| department:update | 更新部门 | ✅ |
| department:delete | 删除部门 | ✅ |

### 教学管理权限

| 权限代码 | 说明 | 使用状态 |
|----------|------|----------|
| course:create | 创建课程 | ✅ |
| course:read | 查看课程 | ⚠️ |
| course:update | 更新课程 | ✅ |
| course:delete | 删除课程 | ✅ |
| course:schedule | 排课/教学任务 | ✅ |
| major:create | 创建专业 | ❌ |
| major:read | 查看专业 | ❌ |
| major:update | 更新专业 | ❌ |
| major:delete | 删除专业 | ❌ |
| class:create | 创建班级 | ❌ |
| class:read | 查看班级 | ❌ |
| class:update | 更新班级 | ❌ |
| class:delete | 删除班级 | ❌ |
| period_time:create | 创建节次 | ❌ |
| period_time:read | 查看节次 | ❌ |
| period_time:update | 更新节次 | ❌ |
| period_time:delete | 删除节次 | ❌ |
| calendar:read | 查看校历 | ⚠️ |
| calendar:update | 更新校历 | ⚠️ |

### 实验室管理权限

| 权限代码 | 说明 | 使用状态 |
|----------|------|----------|
| lab:create | 创建实验室 | ✅ |
| lab:read | 查看实验室 | ⚠️ |
| lab:update | 更新实验室 | ✅ |
| lab:delete | 删除实验室 | ✅ |
| equipment:create | 创建设备 | ✅ |
| equipment:read | 查看设备 | ⚠️ |
| equipment:update | 更新设备 | ✅ |
| equipment:delete | 删除设备 | ✅ |

---

## 建议的权限体系

### 方案1: 统一使用基于权限的授权 (推荐)

每个控制器的方法都添加具体的权限注解：

```csharp
[ApiController]
[Route("api/v1/[controller]")]
public class ClassesController : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = "Permission:class:read")]
    public async Task<IActionResult> GetList(...)

    [HttpGet("{id}")]
    [Authorize(Policy = "Permission:class:read")]
    public async Task<IActionResult> GetById(...)

    [HttpPost]
    [Authorize(Policy = "Permission:class:create")]
    public async Task<IActionResult> Create(...)

    [HttpPut("{id}")]
    [Authorize(Policy = "Permission:class:update")]
    public async Task<IActionResult> Update(...)

    [HttpDelete("{id}")]
    [Authorize(Policy = "Permission:class:delete")]
    public async Task<IActionResult> Delete(...)
}
```

**优点**:
- 细粒度权限控制
- 清晰明了
- 易于审计

**缺点**:
- 需要在每个方法上添加注解

---

### 方案2: 简化权限，只保留类级别的 [Authorize]

移除所有方法级别的权限策略，只保留类级别的 `[Authorize]`，所有登录用户都可以访问。

```csharp
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]  // 所有登录用户可访问
public class ClassesController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetList(...)

    [HttpPost]
    public async Task<IActionResult> Create(...)
}
```

**优点**:
- 快速实现
- 代码简洁

**缺点**:
- 安全性较低
- 无法区分不同操作权限

---

### 方案3: 角色 + 权限混合 (推荐用于教学场景)

- 查询操作：任何登录用户可访问 `[Authorize]`
- 写操作：需要具体权限 `[Authorize(Policy = "Permission:xxx:xxx")]`

```csharp
[ApiController]
[Route("api/v1/[controller]")]
public class CoursesController : ControllerBase
{
    // 查询操作 - 任何登录用户
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetList(...)

    // 写操作 - 需要具体权限
    [HttpPost]
    [Authorize(Policy = "Permission:course:create")]
    public async Task<IActionResult> Create(...)

    [HttpPut("{id}")]
    [Authorize(Policy = "Permission:course:update")]
    public async Task<IActionResult> Update(...)

    [HttpDelete("{id}")]
    [Authorize(Policy = "Permission:course:delete")]
    public async Task<IActionResult> Delete(...)
}
```

**优点**:
- 平衡安全性和开发效率
- 适合教学场景

---

## 需要修复的控制器

### 高优先级 (P0)

| 控制器 | 问题 | 建议方案 |
|--------|------|----------|
| MajorsController | 完全无权限注解 | 方案1 |
| ClassesController | 完全无权限注解 | 方案1 |
| PeriodTimesController | 完全无权限注解 | 方案1 |
| CalendarController | 部分无权限注解 | 方案1 |

### 中优先级 (P1)

| 控制器 | 问题 | 建议方案 |
|--------|------|----------|
| LabsController | 部分权限未使用 | 方案3 |
| EquipmentsController | 部分权限未使用 | 方案3 |
| CoursesController | 部分权限未使用 | 方案3 |

---

## 角色权限配置参考

### 超级管理员 (super_admin)
拥有所有权限

### 实验室管理员 (lab_admin)
- lab:create, lab:read, lab:update, lab:delete
- equipment:create, equipment:read, equipment:update, equipment:delete
- department:read
- course:read

### 教师 (teacher)
- course:read, course:schedule
- class:read (仅关联班级)
- calendar:read
- period_time:read

### 学生 (student)
- course:read (仅选课课程)
- calendar:read
- period_time:read

### 审计员 (auditor)
- 全部只读权限

---

## 下一步行动

### 立即执行
1. 为 MajorsController 添加权限注解
2. 为 ClassesController 添加权限注解
3. 为 PeriodTimesController 添加权限注解
4. 为 CalendarController 补全权限注解

### 后续优化
1. 统一控制器权限策略风格
2. 为超级管理员角色分配所有权限
3. 添加权限测试用例

---

## 参考资料

- [RBAC_DESIGN.md](./RBAC_DESIGN.md) - 权限系统设计文档
- [KNOWN_ISSUES.md](./KNOWN_ISSUES.md) - 已知问题清单
