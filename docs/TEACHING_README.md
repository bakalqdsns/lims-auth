# LIMS 教学管理模块 - 快速参考

## 功能模块

| 模块 | 页面路径 | 功能 |
|------|----------|------|
| 学期管理 | /teaching/semesters | 学期CRUD、当前学期设置、校历生成 |
| 课程管理 | /teaching/courses | 课程CRUD、学时学分管理 |
| 专业管理 | /teaching/majors | 专业CRUD、学制学位管理 |
| 班级管理 | /teaching/classes | 班级CRUD、学生管理 |
| 教学任务 | /teaching/tasks | 教学任务CRUD、教师分配 |
| 节次时间 | /teaching/periods | 上课节次时间配置 |

## 技术栈

- **后端**: .NET 8.0 + SQLite + EF Core + JWT
- **前端**: Vue 3 + TypeScript + Element Plus + Vite

## 启动命令

```bash
# 后端 (端口5047)
cd backend/LimsAuth.Api
dotnet run --urls "http://0.0.0.0:5047"

# 前端 (端口5173)
cd frontend
npm run dev
```

## 测试账号

- 管理员: `admin` / `admin123`
- 教师: `teacher` / `teacher123`
- 学生: `student` / `student123`

## 核心文件位置

```
backend/LimsAuth.Api/
├── Controllers/          # API控制器
│   ├── SemestersController.cs
│   ├── CoursesController.cs
│   ├── MajorsController.cs
│   ├── ClassesController.cs
│   ├── TeachingTasksController.cs
│   ├── PeriodTimesController.cs
│   └── CalendarController.cs
├── Services/             # 业务逻辑
│   ├── SemesterService.cs
│   ├── CourseService.cs
│   ├── MajorService.cs
│   ├── ClassService.cs
│   ├── TeachingTaskService.cs
│   ├── PeriodTimeService.cs
│   └── AcademicCalendarService.cs
└── Models/               # 实体模型
    ├── Semester.cs
    ├── AcademicCalendar.cs
    ├── Course.cs
    ├── Major.cs
    ├── Class.cs
    ├── ClassStudent.cs
    ├── TeachingTask.cs
    ├── TeachingTaskTeacher.cs
    └── PeriodTime.cs

frontend/src/
├── api/teaching.ts       # API客户端
└── views/teaching/       # 页面
    ├── SemestersView.vue
    ├── CoursesView.vue
    ├── MajorsView.vue
    ├── ClassesView.vue
    ├── TeachingTasksView.vue
    ├── PeriodTimesView.vue
    └── components/       # 组件
        ├── SemesterFormDialog.vue
        ├── CalendarViewDialog.vue
        ├── CourseFormDialog.vue
        ├── MajorFormDialog.vue
        ├── ClassFormDialog.vue
        ├── ClassStudentsDialog.vue
        ├── TeachingTaskFormDialog.vue
        └── PeriodTimeFormDialog.vue
```

## 权限代码

- `course:create` - 创建课程
- `course:read` - 查看课程
- `course:update` - 更新课程
- `course:delete` - 删除课程
- `course:schedule` - 排课/教学任务管理

## 数据库实体关系

```
Semester (学期) 1:N AcademicCalendar (校历)
Semester (学期) 1:N TeachingTask (教学任务)
Course (课程) 1:N TeachingTask (教学任务)
Class (班级) 1:N TeachingTask (教学任务)
Class (班级) N:M User (学生) via ClassStudent
TeachingTask (教学任务) N:M User (教师) via TeachingTaskTeacher
Major (专业) 1:N Class (班级)
Department (部门) 1:N Major/Course/Class
```

## 开发日期

2026-04-02 完成开发

## 详细文档

见 `docs/TEACHING_SYSTEM_DESIGN.md`
