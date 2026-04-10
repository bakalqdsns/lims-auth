# 权限策略整理

## 当前权限策略状态

### 已注册权限（Program.cs）

| 模块 | 权限 | 状态 |
|------|------|------|
| user | create, read, update, delete, reset_password | ✅ 已使用 |
| role | create, read, update, delete, assign | ✅ 已使用 |
| permission | read, assign | ✅ 已使用 |
| department | create, read, update, delete | ✅ 已使用 |
| lab | create, read, update, delete | ⚠️ read未使用 |
| equipment | create, read, update, delete | ⚠️ read未使用 |
| course | create, read, update, delete, schedule | ⚠️ read未使用 |
| calendar | read, update | ✅ 已使用 |
| period_time | create, read, update, delete | ✅ 已使用 |
| major | create, read, update, delete | ⚠️ 全部未使用 |
| class | create, read, update, delete | ⚠️ 全部未使用 |

### 问题分析

1. **MajorsController/ClassesController**: 之前修复时添加了权限注解，但实际方法上没有使用
2. **LabsController/EquipmentsController/CoursesController**: 部分 read 权限未在方法上使用
3. **权限策略混乱**: 有的控制器在类级别用 `[Authorize]`，有的在具体方法上用权限策略

## 建议的权限体系

### 方案1: 统一使用基于权限的授权（推荐）

每个控制器的方法都添加具体的权限注解：

```csharp
[HttpGet]
[Authorize(Policy = "Permission:class:read")]
public async Task<IActionResult> GetList(...)

[HttpPost]
[Authorize(Policy = "Permission:class:create")]
public async Task<IActionResult> Create(...)

[HttpPut("{id}")]
[Authorize(Policy = "Permission:class:update")]
public async Task<IActionResult> Update(...)

[HttpDelete("{id}")]
[Authorize(Policy = "Permission:class:delete")]
public async Task<IActionResult> Delete(...)
```

### 方案2: 简化权限，只保留类级别的 [Authorize]

移除所有方法级别的权限策略，只保留类级别的 `[Authorize]`，所有登录用户都可以访问。

### 方案3: 角色 + 权限混合

- 查询操作：任何登录用户可访问 `[Authorize]`
- 写操作：需要具体权限 `[Authorize(Policy = "Permission:xxx:xxx")]`

## 需要修复的控制器

1. **ClassesController** - 添加方法级别的权限注解
2. **MajorsController** - 添加方法级别的权限注解
3. **LabsController** - 检查并统一权限配置
4. **EquipmentsController** - 检查并统一权限配置
5. **CoursesController** - 检查并统一权限配置

## 下一步行动

请选择一种方案，我将统一修复所有控制器的权限配置：

- **方案1**: 完整的基于权限的授权（最细粒度控制）
- **方案2**: 简化权限（快速实现，安全性较低）
- **方案3**: 混合模式（查询开放，写操作受控）