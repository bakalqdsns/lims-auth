# 教学管理系统设计文档

## 系统概述

教学管理模块是 LIMS 高校实验室管理系统的重要组成部分，提供完整的教学基础数据管理功能，包括学期管理、课程管理、专业管理、班级管理、教学任务管理和节次时间管理。

---

## 数据库实体设计

### 1. 学期 (Semester)
| 字段 | 类型 | 说明 |
|------|------|------|
| Id | Guid | 主键 |
| Name | string | 学期名称(如: 2024-2025学年第一学期) |
| StartDate | DateTime | 学期开始日期 |
| EndDate | DateTime | 学期结束日期 |
| IsCurrent | bool | 是否当前学期 |
| IsActive | bool | 是否启用 |
| CreatedAt | DateTime | 创建时间 |

### 2. 校历/日历 (AcademicCalendar)
| 字段 | 类型 | 说明 |
|------|------|------|
| Id | Guid | 主键 |
| SemesterId | Guid | 学期ID |
| Date | DateTime | 日期 |
| WeekNumber | int | 第几周 |
| DayOfWeek | int | 星期几(0=周日,1=周一) |
| IsHoliday | bool | 是否节假日 |
| HolidayName | string | 节假日名称 |
| Description | string | 描述 |

### 3. 课程 (Course)
| 字段 | 类型 | 说明 |
|------|------|------|
| Id | Guid | 主键 |
| Code | string | 课程代码 |
| Name | string | 课程中文名称 |
| EnglishName | string | 课程英文名称 |
| CourseType | string | 修读性质(必修/选修/限选) |
| Credits | decimal | 学分 |
| TotalHours | int | 总学时 |
| TheoryHours | int | 讲授学时 |
| PracticeHours | int | 实践实训学时 |
| ExperimentHours | int | 实验学时 |
| OnlineHours | int | 网络教学学时 |
| SemesterType | int | 设课学期(1=上学期,2=下学期,3=全学年) |
| DepartmentId | Guid | 开课部门ID |
| Description | string | 课程描述 |
| IsActive | bool | 是否启用 |
| CreatedAt | DateTime | 创建时间 |

### 4. 专业 (Major)
| 字段 | 类型 | 说明 |
|------|------|------|
| Id | Guid | 主键 |
| Code | string | 专业代码 |
| Name | string | 专业名称 |
| EnglishName | string | 英文名称 |
| DepartmentId | Guid | 所属部门ID |
| Duration | int | 学制(年) |
| DegreeType | string | 学位类型(Bachelor/Master/Doctor/Associate) |
| Description | string | 描述 |
| IsActive | bool | 是否启用 |
| CreatedAt | DateTime | 创建时间 |

### 5. 班级 (Class)
| 字段 | 类型 | 说明 |
|------|------|------|
| Id | Guid | 主键 |
| Code | string | 班级代码 |
| Name | string | 班级名称 |
| Grade | string | 年级(如: 2024) |
| MajorId | Guid | 专业ID |
| DepartmentId | Guid | 所属部门ID |
| HeadTeacherId | Guid | 班主任ID(用户ID) |
| AdminStudentId | Guid | 班级管理员学生ID |
| StudentCount | int | 学生人数 |
| Description | string | 描述 |
| IsActive | bool | 是否启用 |
| CreatedAt | DateTime | 创建时间 |

### 6. 班级学生关联 (ClassStudent)
| 字段 | 类型 | 说明 |
|------|------|------|
| ClassId | Guid | 班级ID |
| StudentId | Guid | 学生ID(用户ID) |
| JoinedAt | DateTime | 加入时间 |

### 7. 教学任务 (TeachingTask)
| 字段 | 类型 | 说明 |
|------|------|------|
| Id | Guid | 主键 |
| SemesterId | Guid | 学期ID |
| CourseId | Guid | 课程ID |
| ClassId | Guid | 班级ID |
| TaskType | string | 任务类型(主讲/辅导/实验) |
| Description | string | 描述 |
| IsActive | bool | 是否启用 |
| CreatedAt | DateTime | 创建时间 |

### 8. 教学任务教师关联 (TeachingTaskTeacher)
| 字段 | 类型 | 说明 |
|------|------|------|
| TeachingTaskId | Guid | 教学任务ID |
| TeacherId | Guid | 教师ID(用户ID) |
| IsMainTeacher | bool | 是否主讲教师 |
| AssignedAt | DateTime | 分配时间 |

### 9. 节次时间 (PeriodTime)
| 字段 | 类型 | 说明 |
|------|------|------|
| Id | Guid | 主键 |
| PeriodNumber | int | 节次编号(如: 1,2,3...) |
| Name | string | 节次名称(如: 第1-2节) |
| StartTime | TimeSpan | 开始时间 |
| EndTime | TimeSpan | 结束时间 |
| Description | string | 描述 |
| IsActive | bool | 是否启用 |
| CreatedAt | DateTime | 创建时间 |

---

## API 端点实现

### 学期管理 API
| 方法 | 端点 | 说明 | 权限 |
|------|------|------|------|
| GET | /api/v1/semesters | 获取学期列表 | 任意登录用户 |
| GET | /api/v1/semesters/{id} | 获取学期详情 | 任意登录用户 |
| POST | /api/v1/semesters | 创建学期 | Permission |
| PUT | /api/v1/semesters/{id} | 更新学期 | Permission |
| DELETE | /api/v1/semesters/{id} | 删除学期 | Permission |
| POST | /api/v1/semesters/{id}/set-current | 设为当前学期 | Permission |
| GET | /api/v1/semesters/current | 获取当前学期 | 任意登录用户 |
| POST | /api/v1/semesters/{id}/generate-calendar | 生成校历 | Permission |

### 校历管理 API
| 方法 | 端点 | 说明 | 权限 |
|------|------|------|------|
| GET | /api/v1/calendar?semesterId={id} | 获取学期校历 | 任意登录用户 |
| POST | /api/v1/calendar/generate | 生成校历 | Permission |
| PUT | /api/v1/calendar/{id} | 更新日历项 | Permission |
| GET | /api/v1/calendar/today | 获取今天校历 | 任意登录用户 |
| GET | /api/v1/calendar/date/{date} | 获取指定日期校历 | 任意登录用户 |
| GET | /api/v1/calendar/week-info | 获取周次信息 | 任意登录用户 |

### 课程管理 API
| 方法 | 端点 | 说明 | 权限 |
|------|------|------|------|
| GET | /api/v1/courses | 获取课程列表 | 任意登录用户 |
| GET | /api/v1/courses/{id} | 获取课程详情 | 任意登录用户 |
| POST | /api/v1/courses | 创建课程 | course:create |
| PUT | /api/v1/courses/{id} | 更新课程 | course:update |
| DELETE | /api/v1/courses/{id} | 删除课程 | course:delete |
| PATCH | /api/v1/courses/{id}/status | 切换状态 | course:update |

### 专业管理 API
| 方法 | 端点 | 说明 | 权限 |
|------|------|------|------|
| GET | /api/v1/majors | 获取专业列表 | 任意登录用户 |
| GET | /api/v1/majors/all | 获取所有专业 | 任意登录用户 |
| GET | /api/v1/majors/{id} | 获取专业详情 | 任意登录用户 |
| POST | /api/v1/majors | 创建专业 | Permission |
| PUT | /api/v1/majors/{id} | 更新专业 | Permission |
| DELETE | /api/v1/majors/{id} | 删除专业 | Permission |
| PATCH | /api/v1/majors/{id}/status | 切换状态 | Permission |

### 班级管理 API
| 方法 | 端点 | 说明 | 权限 |
|------|------|------|------|
| GET | /api/v1/classes | 获取班级列表 | 任意登录用户 |
| GET | /api/v1/classes/{id} | 获取班级详情 | 任意登录用户 |
| POST | /api/v1/classes | 创建班级 | Permission |
| PUT | /api/v1/classes/{id} | 更新班级 | Permission |
| DELETE | /api/v1/classes/{id} | 删除班级 | Permission |
| PATCH | /api/v1/classes/{id}/status | 切换状态 | Permission |
| GET | /api/v1/classes/{id}/students | 获取班级学生 | 任意登录用户 |
| POST | /api/v1/classes/{id}/students | 添加学生到班级 | Permission |
| DELETE | /api/v1/classes/{id}/students/{studentId} | 从班级移除学生 | Permission |

### 教学任务管理 API
| 方法 | 端点 | 说明 | 权限 |
|------|------|------|------|
| GET | /api/v1/teaching-tasks | 获取教学任务列表 | 任意登录用户 |
| GET | /api/v1/teaching-tasks/{id} | 获取教学任务详情 | 任意登录用户 |
| POST | /api/v1/teaching-tasks | 创建教学任务 | course:schedule |
| PUT | /api/v1/teaching-tasks/{id} | 更新教学任务 | course:schedule |
| DELETE | /api/v1/teaching-tasks/{id} | 删除教学任务 | course:schedule |
| PATCH | /api/v1/teaching-tasks/{id}/status | 切换状态 | course:schedule |
| POST | /api/v1/teaching-tasks/{id}/teachers | 添加教师 | course:schedule |
| DELETE | /api/v1/teaching-tasks/{id}/teachers/{teacherId} | 移除教师 | course:schedule |

### 节次时间管理 API
| 方法 | 端点 | 说明 | 权限 |
|------|------|------|------|
| GET | /api/v1/period-times | 获取节次列表 | 任意登录用户 |
| GET | /api/v1/period-times/{id} | 获取节次详情 | 任意登录用户 |
| POST | /api/v1/period-times | 创建节次 | Permission |
| PUT | /api/v1/period-times/{id} | 更新节次 | Permission |
| DELETE | /api/v1/period-times/{id} | 删除节次 | Permission |
| PATCH | /api/v1/period-times/{id}/status | 切换状态 | Permission |

---

## 前端页面实现

### 页面列表

| 页面 | 路径 | 功能说明 |
|------|------|----------|
| 学期管理 | /teaching/semesters | 学期CRUD、当前学期设置、校历生成 |
| 课程管理 | /teaching/courses | 课程CRUD、学时学分管理 |
| 专业管理 | /teaching/majors | 专业CRUD、学制学位管理 |
| 班级管理 | /teaching/classes | 班级CRUD、学生管理、班主任设置 |
| 教学任务 | /teaching/tasks | 教学任务CRUD、教师分配 |
| 节次时间 | /teaching/periods | 节次时间CRUD |

### 组件列表

| 组件 | 路径 | 功能说明 |
|------|------|----------|
| SemesterFormDialog | components/SemesterFormDialog.vue | 学期表单对话框 |
| CalendarViewDialog | components/CalendarViewDialog.vue | 校历查看对话框 |
| CourseFormDialog | components/CourseFormDialog.vue | 课程表单对话框 |
| MajorFormDialog | components/MajorFormDialog.vue | 专业表单对话框 |
| ClassFormDialog | components/ClassFormDialog.vue | 班级表单对话框 |
| ClassStudentsDialog | components/ClassStudentsDialog.vue | 班级学生管理对话框 |
| TeachingTaskFormDialog | components/TeachingTaskFormDialog.vue | 教学任务表单对话框 |
| PeriodTimeFormDialog | components/PeriodTimeFormDialog.vue | 节次时间表单对话框 |

---

## 后端服务实现

### 服务列表

| 服务 | 文件 | 功能说明 |
|------|------|----------|
| ISemesterService | Services/SemesterService.cs | 学期管理业务逻辑 |
| ICourseService | Services/CourseService.cs | 课程管理业务逻辑 |
| IMajorService | Services/MajorService.cs | 专业管理业务逻辑 |
| IClassService | Services/ClassService.cs | 班级管理业务逻辑 |
| ITeachingTaskService | Services/TeachingTaskService.cs | 教学任务业务逻辑 |
| IPeriodTimeService | Services/PeriodTimeService.cs | 节次时间管理业务逻辑 |
| IAcademicCalendarService | Services/AcademicCalendarService.cs | 校历管理业务逻辑 |

### 控制器列表

| 控制器 | 文件 | 功能说明 |
|--------|------|----------|
| SemestersController | Controllers/SemestersController.cs | 学期API端点 |
| CoursesController | Controllers/CoursesController.cs | 课程API端点 |
| MajorsController | Controllers/MajorsController.cs | 专业API端点 |
| ClassesController | Controllers/ClassesController.cs | 班级API端点 |
| TeachingTasksController | Controllers/TeachingTasksController.cs | 教学任务API端点 |
| PeriodTimesController | Controllers/PeriodTimesController.cs | 节次时间API端点 |
| CalendarController | Controllers/CalendarController.cs | 校历API端点 |

---

## 权限配置

### 教学管理相关权限

| 权限代码 | 说明 |
|----------|------|
| course:create | 创建课程 |
| course:read | 查看课程 |
| course:update | 更新课程 |
| course:delete | 删除课程 |
| course:schedule | 排课/教学任务管理 |

---

## 技术栈

### 后端
- **框架**: .NET 8.0 + ASP.NET Core Web API
- **数据库**: SQLite
- **ORM**: Entity Framework Core
- **认证**: JWT Token
- **权限**: 基于 RBAC 的自定义授权策略

### 前端
- **框架**: Vue 3 + TypeScript
- **UI库**: Element Plus
- **构建工具**: Vite
- **状态管理**: Pinia
- **HTTP客户端**: Axios

---

## 运行说明

### 后端启动
```bash
cd backend/LimsAuth.Api
dotnet run --urls "http://0.0.0.0:5047"
```

### 前端启动
```bash
cd frontend
npm run dev
```

### 访问地址
- 前端: http://localhost:5173
- 后端API: http://localhost:5047
- Swagger文档: http://localhost:5047/swagger

### 测试账号
- 管理员: admin / admin123
- 教师: teacher / teacher123
- 学生: student / student123

---

## 开发记录

### 2026-04-02 教学管理模块开发完成

**完成内容：**
1. 数据库实体设计 (9个实体)
2. 后端API实现 (7个控制器 + 7个服务)
3. 前端页面实现 (6个页面 + 8个组件)
4. 权限配置集成

**修复的问题：**
1. 前端API方法名与后端不匹配 - 统一为 `getList`, `create`, `update`, `delete`, `toggleStatus`
2. 后端返回数据缺少关联字段 - MajorService/ClassService返回DTO包含关联数据
3. Major实体缺少字段 - 添加 `EnglishName`, `Duration`, `DegreeType`
4. 前端导入路径问题 - 使用 `@/api/teaching` 别名

**文件清单：**
- 后端实体: Semester.cs, AcademicCalendar.cs, Course.cs, Major.cs, Class.cs, ClassStudent.cs, TeachingTask.cs, TeachingTaskTeacher.cs, PeriodTime.cs
- 后端服务: SemesterService.cs, CourseService.cs, MajorService.cs, ClassService.cs, TeachingTaskService.cs, PeriodTimeService.cs, AcademicCalendarService.cs
- 后端控制器: SemestersController.cs, CoursesController.cs, MajorsController.cs, ClassesController.cs, TeachingTasksController.cs, PeriodTimesController.cs, CalendarController.cs
- 前端页面: SemestersView.vue, CoursesView.vue, MajorsView.vue, ClassesView.vue, TeachingTasksView.vue, PeriodTimesView.vue
- 前端组件: SemesterFormDialog.vue, CalendarViewDialog.vue, CourseFormDialog.vue, MajorFormDialog.vue, ClassFormDialog.vue, ClassStudentsDialog.vue, TeachingTaskFormDialog.vue, PeriodTimeFormDialog.vue
