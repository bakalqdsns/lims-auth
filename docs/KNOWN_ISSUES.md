# LIMS 系统已知缺陷与风险清单

> 创建日期: 2026-04-02
> 严重程度: 🔴 高 | 🟡 中 | 🟢 低

---

## 🔴 严重缺陷 (需立即修复)

### 1. 权限控制漏洞
**问题描述**: 专业/班级/节次时间等控制器缺少权限注解，任何登录用户都能执行增删改操作

**受影响文件**:
- `MajorsController.cs` - 无权限检查
- `ClassesController.cs` - 无权限检查  
- `PeriodTimesController.cs` - 无权限检查
- `CalendarController.cs` - 无权限检查

**风险**: 普通学生/教师账号可随意修改系统基础数据

**修复建议**:
```csharp
[Authorize(Policy = "Permission:department:update")]  // 或其他合适权限
```

---

### 2. 并发数据冲突
**问题描述**: 读取-修改-保存非原子操作，并发时数据会错乱

**代码位置**:
```csharp
// ClassService.cs
var classEntity = await _dbContext.Classes.FindAsync(id);
classEntity.StudentCount += newStudentIds.Count;  // 并发时计数错误
await _dbContext.SaveChangesAsync();
```

**风险**: 多人同时添加学生到同一班级时，学生数统计不准确

**修复建议**: 使用数据库原子操作或乐观锁
```csharp
await _dbContext.Classes
    .Where(c => c.Id == classId)
    .ExecuteUpdateAsync(setters => 
        setters.SetProperty(c => c.StudentCount, c => c.StudentCount + count));
```

---

### 3. 事务缺失导致数据不一致
**问题描述**: 多表操作没有事务保护，中间失败会导致脏数据

**代码位置**:
```csharp
// TeachingTaskService.cs
_dbContext.TeachingTasks.Add(task);
await _dbContext.SaveChangesAsync();  // 如果这里成功...

// 教师关联在另一个 SaveChanges，如果这里失败，任务存在但教师未分配
foreach (var teacherId in request.TeacherIds) {
    _dbContext.TeachingTaskTeachers.Add(...);
}
await _dbContext.SaveChangesAsync();
```

**风险**: 教学任务创建成功但教师未分配，或班级创建成功但学生未加入

**修复建议**:
```csharp
using var transaction = await _dbContext.Database.BeginTransactionAsync();
try {
    // 所有操作
    await _dbContext.SaveChangesAsync();
    await transaction.CommitAsync();
} catch {
    await transaction.RollbackAsync();
    throw;
}
```

---

## 🟡 中等缺陷 (建议修复)

### 4. 外键约束缺失
**问题描述**: 删除数据时未检查是否被引用，会导致外键冲突或孤儿数据

**受影响操作**:
- 删除学期 - 未检查是否有关联教学任务
- 删除课程 - 未检查是否有关联教学任务
- 删除专业 - 未检查是否有关联班级
- 删除班级 - 未检查是否有关联教学任务

**风险**: 
- 数据库外键约束异常
- 产生孤儿数据（如教学任务指向不存在的课程）

**修复建议**:
```csharp
public async Task<bool> DeleteAsync(Guid id) {
    // 检查引用
    var hasTasks = await _dbContext.TeachingTasks.AnyAsync(t => t.CourseId == id);
    if (hasTasks) throw new InvalidOperationException("该课程已被教学任务引用，无法删除");
    
    // 执行删除
}
```

---

### 5. N+1 查询性能问题
**问题描述**: 列表查询时循环查询关联数据

**代码位置**:
```csharp
// MajorService.cs
var majors = await query.ToListAsync();
var departments = await _dbContext.Departments  // 每次查询都执行
    .Where(d => departmentIds.Contains(d.Id))
    .ToDictionaryAsync(d => d.Id, d => d.Name);
```

**风险**: 数据量大时查询性能急剧下降

**修复建议**: 使用 `Include` 或投影查询一次性获取

---

### 6. 无分页的大列表
**问题描述**: 班级学生列表没有分页

**代码位置**:
```csharp
// ClassService.cs
return await _dbContext.Users
    .Where(u => studentIds.Contains(u.Id))
    .ToListAsync();  // 可能返回数千条记录
```

**风险**: 班级人多时（如公共课200+人）前端卡死

**修复建议**: 添加分页参数

---

### 7. 缺少操作审计日志
**问题描述**: 没有记录谁在什么时间做了什么操作

**风险**: 
- 无法追溯问题操作
- 无法审计数据变更历史
- 安全事故无法溯源

**修复建议**: 添加审计日志表和中间件

---

## 🟢 轻微问题 (可延后)

### 8. 前端缺少错误处理
**问题描述**: API 调用没有统一错误处理

**表现**:
- 401/403 错误不会自动跳转登录
- 网络错误没有重试机制
- 表单提交没有防重复

---

### 9. 敏感操作无二次确认
**问题描述**: 删除操作没有密码确认或二次验证

**风险**: 误操作导致数据丢失

---

### 10. 缺少数据备份机制
**问题描述**: SQLite 数据库没有自动备份

**风险**: 硬件故障时数据丢失

---

## 修复优先级建议

| 优先级 | 缺陷 | 预计工时 |
|--------|------|----------|
| P0 | 权限控制漏洞 | 2h |
| P0 | 事务缺失 | 4h |
| P1 | 并发冲突 | 3h |
| P1 | 外键约束检查 | 3h |
| P2 | N+1查询优化 | 2h |
| P2 | 列表分页 | 2h |
| P3 | 审计日志 | 8h |
| P3 | 前端错误处理 | 4h |

---

## 代码审查记录

- 审查日期: 2026-04-02
- 审查范围: 教学管理模块全部后端代码
- 发现问题: 10项
- 严重问题: 3项
