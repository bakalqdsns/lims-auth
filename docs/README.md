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
│       ├── Controllers/        # API 控制器
│       ├── Services/           # 业务逻辑层
│       ├── Models/             # 数据模型
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

---

## 文档导航

| 文档 | 说明 |
|------|------|
| [PROJECT_STRUCTURE.md](./PROJECT_STRUCTURE.md) | 项目结构详细说明 |
| [RBAC_DESIGN.md](./RBAC_DESIGN.md) | 权限系统设计文档 |
| [TEACHING_SYSTEM_DESIGN.md](./TEACHING_SYSTEM_DESIGN.md) | 教学管理系统设计 |
| [TEACHING_README.md](./TEACHING_README.md) | 教学模块快速参考 |
| [PERMISSION_REFACTOR.md](./PERMISSION_REFACTOR.md) | 权限策略整理与建议 |
| [KNOWN_ISSUES.md](./KNOWN_ISSUES.md) | 已知缺陷与风险清单 |

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
GET    /api/v1/users        # 用户列表
POST   /api/v1/users        # 创建用户
GET    /api/v1/users/{id}   # 用户详情
PUT    /api/v1/users/{id}    # 更新用户
DELETE /api/v1/users/{id}    # 删除用户

# 角色管理
GET    /api/v1/roles        # 角色列表
POST   /api/v1/roles        # 创建角色
PUT    /api/v1/roles/{id}   # 更新角色
DELETE /api/v1/roles/{id}   # 删除角色

# 部门管理
GET    /api/v1/departments  # 部门列表
POST   /api/v1/departments  # 创建部门
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

# 课程管理
GET    /api/v1/courses       # 课程列表
POST   /api/v1/courses        # 创建课程
PUT    /api/v1/courses/{id}   # 更新课程
DELETE /api/v1/courses/{id}   # 删除课程

# 专业管理
GET    /api/v1/majors        # 专业列表
POST   /api/v1/majors         # 创建专业
PUT    /api/v1/majors/{id}    # 更新专业
DELETE /api/v1/majors/{id}    # 删除专业

# 班级管理
GET    /api/v1/classes                    # 班级列表
POST   /api/v1/classes                    # 创建班级
GET    /api/v1/classes/{id}/students      # 班级学生
POST   /api/v1/classes/{id}/students      # 添加学生

# 教学任务
GET    /api/v1/teaching-tasks            # 教学任务列表
POST   /api/v1/teaching-tasks            # 创建教学任务
POST   /api/v1/teaching-tasks/{id}/teachers  # 分配教师

# 节次时间
GET    /api/v1/period-times    # 节次列表
POST   /api/v1/period-times    # 创建节次
PUT    /api/v1/period-times/{id}  # 更新节次
DELETE /api/v1/period-times/{id}  # 删除节次

# 校历管理
GET    /api/v1/calendar              # 获取校历
GET    /api/v1/calendar/today        # 今天校历
GET    /api/v1/calendar/week-info    # 周次信息
```

---

## 开发指南

### 添加新控制器

1. 在 `Controllers/` 创建控制器类
2. 继承 `ControllerBase`
3. 注入相关 Service
4. 添加 `[Authorize]` 或权限注解

```csharp
[ApiController]
[Route("api/v1/[controller]")]
[Authorize(Policy = "Permission:xxx:read")]
public class XxxController : ControllerBase
{
    private readonly IXxxService _xxxService;

    public XxxController(IXxxService xxxService)
    {
        _xxxService = xxxService;
    }

    [HttpGet]
    public async Task<IActionResult> GetList() { ... }
}
```

### 添加新权限

在 `Program.cs` 中注册权限策略：

```csharp
services.AddAuthorization(options =>
{
    options.AddPolicy("Permission:xxx:create", policy =>
        policy.Requirements.Add(new PermissionRequirement("xxx:create")));
    // ...
});
```

### 前端 API 调用

```typescript
// api/teaching.ts
import request from '@/utils/request'

export const getCourses = () => {
  return request.get('/courses')
}
```

---

## 许可证

MIT License

---

## 最后更新

2026-05-27
