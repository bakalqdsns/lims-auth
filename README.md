# LIMS 项目文档


---

## 📁 项目根目录

```
lims-auth/
├── backend/                    # .NET 8.0 后端
├── frontend/                   # Vue 3 前端
└── docs/                       # 项目文档
    ├── KNOWN_ISSUES.md         # 已知缺陷清单
    ├── RBAC_DESIGN.md          # 权限系统设计
    ├── TEACHING_README.md      # 教学模块快速参考
    ├── TEACHING_SYSTEM_DESIGN.md  # 教学系统设计
    └── PROJECT_STRUCTURE.md    # 本文件
```

---

## 🔧 后端结构 (backend/LimsAuth.Api/)

### 技术栈
- **框架**: .NET 8.0 ASP.NET Core Web API
- **数据库**: SQLite + Entity Framework Core 8.0
- **认证**: JWT Bearer Token
- **权限**: 自定义 RBAC
- **API 文档**: Swagger

### 目录结构

```
backend/LimsAuth.Api/
├── LimsAuth.Api.csproj         # 项目文件
├── Program.cs                  # 应用入口 + 服务配置
├── appsettings.json            # 配置文件
├── lims.db                     # SQLite 数据库
├── Authorization/              # 权限认证
│   ├── PermissionHandler.cs    # 权限验证处理器
│   ├── PermissionRequirement.cs
│   ├── PermissionPolicy.cs
│   └── PermissionAuthorizationExtensions.cs
├── Controllers/                # API 控制器 (13个)
│   ├── AuthController.cs
│   ├── UsersController.cs
│   ├── RolesController.cs
│   ├── PermissionsController.cs
│   ├── DepartmentsController.cs
│   ├── SemestersController.cs
│   ├── CalendarController.cs
│   ├── CoursesController.cs
│   ├── MajorsController.cs
│   ├── ClassesController.cs
│   ├── PeriodTimesController.cs
│   ├── TeachingTasksController.cs
│   ├── LabsController.cs
│   └── EquipmentsController.cs
├── Data/
│   └── AppDbContext.cs         # EF Core 上下文 + 种子数据
├── Models/                     # 实体模型
│   ├── User.cs, Role.cs, Permission.cs
│   ├── Department.cs
│   ├── Semester.cs, AcademicCalendar.cs
│   ├── Course.cs, Major.cs, Class.cs, ClassStudent.cs
│   ├── TeachingTask.cs, TeachingTaskTeacher.cs
│   ├── PeriodTime.cs
│   ├── Lab.cs, Equipment.cs
│   └── DTOs/
│       └── AuthDtos.cs         # 请求/响应 DTO
└── Services/                   # 业务逻辑层
    ├── AuthService.cs
    ├── JwtService.cs
    ├── UserService.cs
    ├── RoleService.cs
    ├── PermissionService.cs
    ├── DepartmentService.cs
    ├── SemesterService.cs
    ├── AcademicCalendarService.cs
    ├── CourseService.cs
    ├── MajorService.cs
    ├── ClassService.cs
    ├── TeachingTaskService.cs
    ├── PeriodTimeService.cs
    ├── LabService.cs
    └── EquipmentService.cs
```

### 数据库实体 (18个表)

| 模块 | 实体 |
|------|------|
| 系统管理 | users, roles, permissions, user_roles, role_permissions, departments |
| 教学管理 | semesters, academic_calendars, courses, majors, classes, class_students, teaching_tasks, teaching_task_teachers, period_times |
| 实验室管理 | labs, equipments |

### 启动命令

```bash
cd backend/LimsAuth.Api
dotnet run --urls "http://0.0.0.0:5047"
```

- API: http://localhost:5047
- Swagger: http://localhost:5047/swagger

---

## 🎨 前端结构 (frontend/)

### 技术栈
- **框架**: Vue 3 + TypeScript
- **构建工具**: Vite 5
- **UI 库**: Element Plus 2.5
- **状态管理**: Pinia
- **路由**: Vue Router 4
- **HTTP**: Axios

### 目录结构

```
frontend/
├── package.json                # 依赖配置
├── vite.config.ts              # Vite 配置
├── tsconfig.json               # TypeScript 配置
└── src/
    ├── main.ts                 # 入口文件
    ├── App.vue                 # 根组件
    ├── api/                    # API 接口层
    │   ├── lab.ts              # 实验室 API
    │   ├── system.ts           # 系统管理 API
    │   └── teaching.ts         # 教学管理 API
    ├── stores/
    │   └── auth.ts             # 认证状态 (Pinia)
    ├── router/
    │   └── index.ts            # 路由配置 + 守卫
    ├── directives/
    │   └── permission.ts       # 权限指令
    └── views/                  # 页面视图
        ├── LoginView.vue
        ├── HomeView.vue        # 主页布局 (含侧边栏)
        ├── system/             # 系统管理模块
        │   ├── UsersView.vue
        │   ├── RolesView.vue
        │   ├── DepartmentsView.vue
        │   └── components/     # 弹窗组件
        ├── teaching/           # 教学管理模块
        │   ├── SemestersView.vue
        │   ├── CoursesView.vue
        │   ├── MajorsView.vue
        │   ├── ClassesView.vue
        │   ├── TeachingTasksView.vue
        │   ├── PeriodTimesView.vue
        │   └── components/     # 8个表单弹窗
        └── lab/                # 实验室管理模块
            ├── LabsView.vue
            ├── EquipmentsView.vue
            └── components/
```

### 启动命令

```bash
cd frontend
npm run dev
```

- 前端: http://localhost:5173

---

## 🔐 权限系统

### 权限编码规范
格式: `{module}:{action}`

| 模块 | 权限示例 |
|------|---------|
| user | user:create, user:read, user:update, user:delete |
| role | role:create, role:read, role:update, role:delete, role:assign |
| department | department:create, department:read, department:update, department:delete |
| course | course:create, course:read, course:update, course:delete, course:schedule |
| lab | lab:create, lab:read, lab:update, lab:delete |
| equipment | equipment:create, equipment:read, equipment:update, equipment:delete, equipment:borrow |

### 预定义角色

| 角色编码 | 角色名称 | 说明 |
|----------|---------|------|
| super_admin | 超级管理员 | 拥有所有权限 |
| lab_admin | 实验室管理员 | 管理实验室、设备 |
| teacher | 教师 | 课程管理、学生管理 |
| student | 学生 | 预约设备、提交报告 |
| auditor | 审计员 | 查看日志、报表 |

---

## 🧪 测试账号

| 账号 | 密码 | 角色 |
|------|------|------|
| admin | admin123 | 超级管理员 |
| teacher | teacher123 | 教师 |
| student | student123 | 学生 |

---

## ⚠️ 已知问题

详见 `KNOWN_ISSUES.md`，主要包括：

1. **权限控制漏洞** - Majors/Classes/PeriodTimes/Calendar 控制器缺少权限注解
2. **并发数据冲突** - 学生计数非原子操作
3. **事务缺失** - 多表操作无事务保护
4. **外键约束缺失** - 删除数据未检查引用
5. **N+1 查询性能问题**
6. **无分页的大列表**

---

## 📚 相关文档

- `TEACHING_README.md` - 教学模块快速参考
- `TEACHING_SYSTEM_DESIGN.md` - 教学系统设计
- `RBAC_DESIGN.md` - 权限系统设计
- `KNOWN_ISSUES.md` - 已知缺陷清单
