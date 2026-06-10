# LimsAuth API Documentation

**Base URL:** `http://localhost:5047`  
**API Version:** v1  
**Swagger UI:** http://localhost:5047/swagger  
**OpenAPI Spec:** http://localhost:5047/swagger/v1/swagger.json

---

## Table of Contents

1. [认证 (Auth)](#1-认证-auth)
2. [用户 (Users)](#2-用户-users)
3. [角色 (Roles)](#3-角色-roles)
4. [权限 (Permissions)](#4-权限-permissions)
5. [部门 (Departments)](#5-部门-departments)
6. [校区 (Campuses)](#6-校区-campuses)
7. [建筑 (Buildings)](#7-建筑-buildings)
8. [实验室 (Labs)](#8-实验室-labs)
9. [设备 (Equipments)](#9-设备-equipments)
10. [借用记录 (BorrowRecords)](#10-借用记录-borrowrecords)
11. [学期 (Semesters)](#11-学期-semesters)
12. [课程 (Courses)](#12-课程-courses)
13. [专业 (Majors)](#13-专业-majors)
14. [班级 (Classes)](#14-班级-classes)
15. [授课任务 (TeachingTasks)](#15-授课任务-teachingtasks)
16. [授课申请 (TeachingApplications)](#16-授课申请-teachingapplications)
17. [节次时间 (PeriodTimes)](#17-节次时间-periodtimes)
18. [校历 (Calendar)](#18-校历-calendar)
19. [排课 (Schedules)](#19-排课-schedules)
20. [预约 (Reservations)](#20-预约-reservations)
21. [使用登记 (UsageRegistrations)](#21-使用登记-usageregistrations)
22. [统计 (Statistics)](#22-统计-statistics)
23. [实验管理 (Experiments)](#23-实验管理-experiments)
24. [导出 (Export)](#24-导出-export)
25. [数据模型 (Schemas)](#25-数据模型-schemas)

---

## 1. 认证 (Auth)

### 登录
```
POST /api/v1/Auth/login
```
**Request:**
```json
{
  "username": "string",
  "password": "string"
}
```

### 获取当前用户信息
```
GET /api/v1/Auth/me
```

### 更新个人资料
```
PUT /api/v1/Auth/profile
```
**Request:**
```json
{
  "fullName": "string",
  "email": "string",
  "phone": "string"
}
```

### 刷新 Token
```
POST /api/v1/Auth/refresh
```

### 健康检查
```
GET /api/v1/Auth/health
```

---

## 2. 用户 (Users)

### 获取用户列表
```
GET /api/v1/Users
```

### 创建用户
```
POST /api/v1/Users
```
**Request:**
```json
{
  "username": "string",
  "password": "string",
  "email": "string",
  "phone": "string",
  "fullName": "string",
  "departmentId": "string",
  "roleIds": ["string"],
  "isActive": true
}
```

### 获取用户详情
```
GET /api/v1/Users/{id}
```

### 更新用户
```
PUT /api/v1/Users/{id}
```
**Request:**
```json
{
  "email": "string",
  "phone": "string",
  "fullName": "string",
  "departmentId": "string",
  "isActive": true
}
```

### 删除用户
```
DELETE /api/v1/Users/{id}
```

### 重置密码
```
PUT /api/v1/Users/{id}/password
```

### 分配用户角色
```
PUT /api/v1/Users/{id}/roles
```

### 修改用户状态
```
PATCH /api/v1/Users/{id}/status
```

### 修改密码（当前用户）
```
POST /api/v1/Users/change-password
```
**Request:**
```json
{
  "oldPassword": "string",
  "newPassword": "string"
}
```

### 获取当前用户权限
```
GET /api/v1/Users/permissions/my
```

---

## 3. 角色 (Roles)

### 获取角色列表
```
GET /api/v1/Roles
```

### 获取所有角色
```
GET /api/v1/Roles/all
```

### 创建角色
```
POST /api/v1/Roles
```
**Request:**
```json
{
  "code": "string",
  "name": "string",
  "description": "string"
}
```

### 获取角色详情
```
GET /api/v1/Roles/{id}
```

### 更新角色
```
PUT /api/v1/Roles/{id}
```
**Request:**
```json
{
  "name": "string",
  "description": "string",
  "isActive": true
}
```

### 删除角色
```
DELETE /api/v1/Roles/{id}
```

### 分配角色权限
```
PUT /api/v1/Roles/{id}/permissions
```
**Request:**
```json
{
  "permissionIds": ["string"]
}
```

### 获取角色下的用户
```
GET /api/v1/Roles/{id}/users
```

---

## 4. 权限 (Permissions)

### 获取权限列表
```
GET /api/v1/Permissions
```

### 按模块获取权限
```
GET /api/v1/Permissions/by-module
```

### 获取所有模块
```
GET /api/v1/Permissions/modules
```

---

## 5. 部门 (Departments)

### 获取部门列表
```
GET /api/v1/Departments
```

### 获取所有部门
```
GET /api/v1/Departments/all
```

### 创建部门
```
POST /api/v1/Departments
```
**Request:**
```json
{
  "code": "string",
  "name": "string",
  "parentId": "string",
  "managerId": "string",
  "description": "string"
}
```

### 获取部门详情
```
GET /api/v1/Departments/{id}
```

### 更新部门
```
PUT /api/v1/Departments/{id}
```
**Request:**
```json
{
  "name": "string",
  "managerId": "string",
  "description": "string",
  "isActive": true
}
```

### 删除部门
```
DELETE /api/v1/Departments/{id}
```

---

## 6. 校区 (Campuses)

### 获取校区列表
```
GET /api/v1/campuses
```

### 创建校区
```
POST /api/v1/campuses
```
**Request:**
```json
{
  "code": "string",
  "name": "string",
  "address": "string",
  "area": 0,
  "campusType": "string",
  "contactPhone": "string",
  "managerId": "string",
  "description": "string"
}
```

### 获取校区详情
```
GET /api/v1/campuses/{id}
```

### 更新校区
```
PUT /api/v1/campuses/{id}
```

### 删除校区
```
DELETE /api/v1/campuses/{id}
```

### 修改校区状态
```
PATCH /api/v1/campuses/{id}/status
```

---

## 7. 建筑 (Buildings)

### 获取建筑列表
```
GET /api/v1/buildings
```

### 按校区获取建筑
```
GET /api/v1/buildings/by-campus/{campusId}
```

### 创建建筑
```
POST /api/v1/buildings
```
**Request:**
```json
{
  "code": "string",
  "name": "string",
  "campusId": "string",
  "address": "string",
  "floorCount": 0,
  "buildingArea": 0,
  "buildingType": "string",
  "builtYear": 0,
  "managerId": "string",
  "description": "string"
}
```

### 获取建筑详情
```
GET /api/v1/buildings/{id}
```

### 更新建筑
```
PUT /api/v1/buildings/{id}
```

### 删除建筑
```
DELETE /api/v1/buildings/{id}
```

### 修改建筑状态
```
PATCH /api/v1/buildings/{id}/status
```

---

## 8. 实验室 (Labs)

### 获取实验室列表
```
GET /api/v1/labs
```

### 创建实验室
```
POST /api/v1/labs
```
**Request:**
```json
{
  "code": "string",
  "name": "string",
  "departmentId": "string",
  "buildingId": "string",
  "floor": 0,
  "roomNumber": "string",
  "location": "string",
  "capacity": 0,
  "labType": "string",
  "safetyLevel": "string",
  "managerId": "string",
  "description": "string"
}
```

### 获取实验室详情
```
GET /api/v1/labs/{id}
```

### 更新实验室
```
PUT /api/v1/labs/{id}
```

### 删除实验室
```
DELETE /api/v1/labs/{id}
```

### 修改实验室状态
```
PATCH /api/v1/labs/{id}/status
```

---

## 9. 设备 (Equipments)

### 获取设备列表
```
GET /api/v1/equipments
```

### 创建设备
```
POST /api/v1/equipments
```
**Request:**
```json
{
  "code": "string",
  "name": "string",
  "model": "string",
  "manufacturer": "string",
  "serialNumber": "string",
  "labId": "string",
  "category": "string",
  "unit": "string",
  "status": "string",
  "purchaseDate": "string",
  "warrantyMonths": 0,
  "price": 0,
  "location": "string",
  "imageUrl": "string",
  "instructions": "string",
  "requiresBooking": true,
  "maxBookingHours": 0,
  "totalQuantity": 0,
  "availableQuantity": 0,
  "brand": "string",
  "supplier": "string",
  "description": "string"
}
```

### 获取设备详情
```
GET /api/v1/equipments/{id}
```

### 更新设备
```
PUT /api/v1/equipments/{id}
```

### 删除设备
```
DELETE /api/v1/equipments/{id}
```

### 修改设备状态
```
PATCH /api/v1/equipments/{id}/status
PATCH /api/v1/equipments/{id}/equipment-status
```

### 获取设备统计
```
GET /api/v1/equipments/statistics
```

### 导入设备
```
POST /api/v1/equipments/import
```

### 下载导入模板
```
GET /api/v1/equipments/import-template
```

### 导出设备
```
GET /api/v1/equipments/export
```

---

## 10. 借用记录 (BorrowRecords)

### 获取借用记录列表
```
GET /api/v1/borrow-records?status=string&keyword=string
```

### 创建借用记录
```
POST /api/v1/borrow-records
```
**Request:**
```json
{
  "equipmentId": "string",
  "borrowDate": "string",
  "expectedReturnDate": "string",
  "purpose": "string",
  "phone": "string",
  "usageLocation": "string",
  "remarks": "string"
}
```

### 获取我的借用记录
```
GET /api/v1/borrow-records/my?status=string
```

### 获取待审批借用记录
```
GET /api/v1/borrow-records/pending
```

### 获取即将到期记录
```
GET /api/v1/borrow-records/expiring
```

### 获取已逾期记录
```
GET /api/v1/borrow-records/overdue
```

### 获取借用流程
```
GET /api/v1/borrow-records/flow
```

### 按编号获取记录
```
GET /api/v1/borrow-records/no/{recordNo}
```

### 获取记录详情
```
GET /api/v1/borrow-records/{id}
```

### 删除借用记录
```
DELETE /api/v1/borrow-records/{id}
```

### 确认借用
```
POST /api/v1/borrow-records/{id}/confirm-borrow
```

### 提交归还
```
POST /api/v1/borrow-records/{id}/submit-return
```

### 确认归还
```
POST /api/v1/borrow-records/{id}/confirm-return
```

### 申请续借
```
POST /api/v1/borrow-records/{id}/renew
```
**Request:**
```json
{
  "newReturnDate": "string"
}
```

### 审批续借
```
POST /api/v1/borrow-records/{id}/approve-renew
```

### 主管审批
```
POST /api/v1/borrow-records/{id}/supervisor-approve
```

### 管理员审批
```
POST /api/v1/borrow-records/{id}/admin-approve
```

### 归还审批
```
POST /api/v1/borrow-records/{id}/approve-return
```

---

## 11. 学期 (Semesters)

### 获取学期列表
```
GET /api/v1/semesters
```

### 获取当前学期
```
GET /api/v1/semesters/current
```

### 获取当前学期今日信息
```
GET /api/v1/semesters/current/today
```

### 检查学期重叠
```
GET /api/v1/semesters/check-overlap
```

### 获取可导入的授课任务
```
GET /api/v1/semesters/importable-tasks
```

### 创建学期
```
POST /api/v1/semesters
```
**Request:**
```json
{
  "name": "string",
  "code": "string",
  "academicYear": "string",
  "semesterType": "First|Second|Summer|Winter",
  "startDate": "string",
  "endDate": "string",
  "teachingStartDate": "string",
  "teachingEndDate": "string",
  "totalWeeks": 0,
  "teachingWeeks": 0,
  "courseSelectionStart": "string",
  "courseSelectionEnd": "string",
  "courseSelectionEndWithdraw": "string",
  "schedulingStart": "string",
  "schedulingEnd": "string",
  "schedulePublishTime": "string",
  "examWeekStart": "string",
  "examWeekEnd": "string",
  "gradeEntryStart": "string",
  "gradeEntryEnd": "string",
  "gradePublishTime": "string",
  "registrationStart": "string",
  "registrationEnd": "string",
  "tuitionPaymentStart": "string",
  "tuitionPaymentEnd": "string",
  "description": "string"
}
```

### 获取学期详情
```
GET /api/v1/semesters/{id}
```

### 更新学期
```
PUT /api/v1/semesters/{id}
```

### 删除学期
```
DELETE /api/v1/semesters/{id}
```

### 设置为当前学期
```
POST /api/v1/semesters/{id}/set-current
```

### 更新学期状态
```
POST /api/v1/semesters/{id}/status
```

### 获取学期校历
```
GET /api/v1/semesters/{id}/calendar
```

### 生成校历
```
POST /api/v1/semesters/{id}/generate-calendar
```
**Request:**
```json
{
  "holidays": [{ "date": "string", "name": "string", "type": "string", "isWorkday": false, "description": "string" }],
  "specialEvents": [{ "date": "string", "eventType": "string", "eventName": "string", "priority": "string", "description": "string", "color": "string" }]
}
```

### 获取学期操作日志
```
GET /api/v1/semesters/{id}/logs
```

### 获取学期周信息
```
GET /api/v1/semesters/{id}/week-info
```

### 归档学期
```
POST /api/v1/semesters/{id}/archive
```

### 自动切换学期
```
POST /api/v1/semesters/auto-transition
```

### 验证学期
```
POST /api/v1/semesters/validate
```

### 从学期复制
```
POST /api/v1/semesters/copy-from-semester
```
**Request:**
```json
{
  "sourceSemesterId": "string",
  "name": "string",
  "code": "string",
  "academicYear": "string",
  "startDate": "string",
  "endDate": "string",
  "copyOptions": {
    "copyCourses": true,
    "copyClasses": true,
    "copyTeachingTasks": true,
    "copyPeriodTimes": true
  }
}
```

### 从模板复制
```
POST /api/v1/semesters/copy-from-template
```
**Request:**
```json
{
  "templateId": "string",
  "name": "string",
  "code": "string",
  "academicYear": "string",
  "startDate": "string",
  "endDate": "string"
}
```

### 创建沙箱
```
POST /api/v1/semesters/{id}/sandbox
```
**Request:**
```json
{
  "name": "string"
}
```

### 应用沙箱
```
POST /api/v1/semesters/sandbox/{sandboxId}/apply
```

### 删除沙箱
```
DELETE /api/v1/semesters/sandbox/{sandboxId}
```

---

## 12. 课程 (Courses)

### 获取课程列表
```
GET /api/v1/courses
```

### 创建课程
```
POST /api/v1/courses
```
**Request:**
```json
{
  "code": "string",
  "name": "string",
  "englishName": "string",
  "courseType": "string",
  "credits": 0,
  "totalHours": 0,
  "theoryHours": 0,
  "practiceHours": 0,
  "experimentHours": 0,
  "onlineHours": 0,
  "semesterType": 0,
  "departmentId": "string",
  "managerId": "string",
  "description": "string"
}
```

### 获取课程详情
```
GET /api/v1/courses/{id}
```

### 更新课程
```
PUT /api/v1/courses/{id}
```

### 删除课程
```
DELETE /api/v1/courses/{id}
```

### 修改课程状态
```
PATCH /api/v1/courses/{id}/status
```

---

## 13. 专业 (Majors)

### 获取专业列表
```
GET /api/v1/majors
```

### 获取所有专业
```
GET /api/v1/majors/all
```

### 创建专业
```
POST /api/v1/majors
```
**Request:**
```json
{
  "code": "string",
  "name": "string",
  "englishName": "string",
  "departmentId": "string",
  "duration": 0,
  "degreeType": "string",
  "description": "string"
}
```

### 获取专业详情
```
GET /api/v1/majors/{id}
```

### 更新专业
```
PUT /api/v1/majors/{id}
```

### 删除专业
```
DELETE /api/v1/majors/{id}
```

### 修改专业状态
```
PATCH /api/v1/majors/{id}/status
```

---

## 14. 班级 (Classes)

### 获取班级列表
```
GET /api/v1/classes
```

### 创建班级
```
POST /api/v1/classes
```
**Request:**
```json
{
  "code": "string",
  "name": "string",
  "grade": "string",
  "majorId": "string",
  "departmentId": "string",
  "headTeacherId": "string",
  "adminStudentId": "string",
  "description": "string"
}
```

### 获取班级详情
```
GET /api/v1/classes/{id}
```

### 更新班级
```
PUT /api/v1/classes/{id}
```

### 删除班级
```
DELETE /api/v1/classes/{id}
```

### 修改班级状态
```
PATCH /api/v1/classes/{id}/status
```

### 获取班级学生列表
```
GET /api/v1/classes/{id}/students
```

### 添加学生到班级
```
POST /api/v1/classes/{id}/students
```
**Request:**
```json
{
  "studentIds": ["string"]
}
```

### 从班级移除学生
```
DELETE /api/v1/classes/{id}/students/{studentId}
```

---

## 15. 授课任务 (TeachingTasks)

### 获取授课任务列表
```
GET /api/v1/teaching-tasks
```

### 创建授课任务
```
POST /api/v1/teaching-tasks
```
**Request:**
```json
{
  "semesterId": "string",
  "courseId": "string",
  "classId": "string",
  "taskType": "string",
  "description": "string",
  "teacherIds": ["string"]
}
```

### 获取授课任务详情
```
GET /api/v1/teaching-tasks/{id}
```

### 更新授课任务
```
PUT /api/v1/teaching-tasks/{id}
```

### 删除授课任务
```
DELETE /api/v1/teaching-tasks/{id}
```

### 修改授课任务状态
```
PATCH /api/v1/teaching-tasks/{id}/status
```

### 添加授课教师
```
POST /api/v1/teaching-tasks/{id}/teachers
```
**Request:**
```json
{
  "teacherId": "string",
  "isMainTeacher": true
}
```

### 移除授课教师
```
DELETE /api/v1/teaching-tasks/{id}/teachers/{teacherId}
```

---

## 16. 授课申请 (TeachingApplications)

### 获取授课申请列表
```
GET /api/v1/teaching-applications
```

### 创建授课申请
```
POST /api/v1/teaching-applications
```
**Request:**
```json
{
  "semesterId": "string",          // [必填]
  "teachingTaskId": "string",       // [必填]
  "courseName": "string",
  "majorId": "string",
  "majorName": "string",
  "classId": "string",
  "className": "string",
  "weekNumbers": [0],
  "dayOfWeek": 0,
  "periodNumbers": [0],
  "expectedLabId": "string",
  "remark": "string"
}
```

### 获取我的授课申请
```
GET /api/v1/teaching-applications/my
```

### 获取待审批申请
```
GET /api/v1/teaching-applications/pending
```

### 获取申请详情
```
GET /api/v1/teaching-applications/{id}
```

### 审批授课申请
```
PUT /api/v1/teaching-applications/{id}/approve
```
**Request:**
```json
{
  "comment": "string",
  "approverName": "string"
}
```

### 驳回授课申请
```
PUT /api/v1/teaching-applications/{id}/reject
```

### 取消授课申请
```
PUT /api/v1/teaching-applications/{id}/cancel
```

---

## 17. 节次时间 (PeriodTimes)

### 获取节次时间列表
```
GET /api/v1/period-times
```

### 创建节次时间
```
POST /api/v1/period-times
```
**Request:**
```json
{
  "periodNumber": 0,
  "name": "string",
  "startTime": "string",
  "endTime": "string",
  "description": "string"
}
```

### 获取节次时间详情
```
GET /api/v1/period-times/{id}
```

### 更新节次时间
```
PUT /api/v1/period-times/{id}
```

### 删除节次时间
```
DELETE /api/v1/period-times/{id}
```

### 修改节次时间状态
```
PATCH /api/v1/period-times/{id}/status
```

---

## 18. 校历 (Calendar)

### 获取校历列表
```
GET /api/v1/calendar
```

### 获取校历详情
```
GET /api/v1/calendar/{id}
```

### 更新校历事件
```
PUT /api/v1/calendar/{id}
```

### 获取今日校历
```
GET /api/v1/calendar/today
```

### 获取本周信息
```
GET /api/v1/calendar/week-info
```

### 按日期获取校历
```
GET /api/v1/calendar/date/{date}
```

### 按事件类型获取校历
```
GET /api/v1/calendar/by-event-type
```

### 获取校历事件类型
```
GET /api/v1/calendar/event-types
```

### 获取假期列表
```
GET /api/v1/calendar/holidays
```

### 添加假期
```
POST /api/v1/calendar/holidays
```
**Request:**
```json
{
  "date": "string",
  "name": "string",
  "type": "string",
  "isWorkday": false,
  "description": "string"
}
```

### 调整工作日
```
POST /api/v1/calendar/adjust-workday
```
**Request:**
```json
{
  "date": "string",
  "isWorkday": true,
  "reason": "string"
}
```

### 检查校历权限
```
GET /api/v1/calendar/check-permission
```

---

## 19. 排课 (Schedules)

### 获取排课列表
```
GET /api/v1/schedules
```

### 创建排课
```
POST /api/v1/schedules
```
**Request:**
```json
{
  "semesterId": "string",          // [必填]
  "labId": "string",
  "weekNumber": 0,
  "startWeek": 0,
  "endWeek": 0,
  "dayOfWeek": 0,
  "periodNumber": 0,
  "source": "string",
  "reservationId": "string",
  "teachingApplicationId": "string",
  "experimentTaskId": "string",
  "teachingTaskId": "string",
  "courseName": "string",
  "projectName": "string",
  "courseId": "string",
  "teacherId": "string",
  "teacherName": "string",
  "classId": "string",
  "className": "string",
  "majorId": "string",
  "majorName": "string",
  "studentCount": 0,
  "remark": "string",
  "forceSchedule": false
}
```

### 获取排课详情
```
GET /api/v1/schedules/{id}
```

### 更新排课
```
PUT /api/v1/schedules/{id}
```

### 删除排课
```
DELETE /api/v1/schedules/{id}
```

### 获取课表视图
```
GET /api/v1/schedules/table-view
```

### 检查排课冲突
```
POST /api/v1/schedules/check-conflicts
```

### 按班级获取排课
```
GET /api/v1/schedules/by-class/{classId}
```

### 按实验室获取排课
```
GET /api/v1/schedules/by-lab/{labId}
```

### 按教师获取排课
```
GET /api/v1/schedules/by-teacher/{teacherId}
```

### 获取可用实验室
```
GET /api/v1/schedules/available-labs
```

### 从授课任务导入排课
```
POST /api/v1/schedules/import-from-tasks
```
**Request:**
```json
{
  "taskIds": ["string"]
}
```

---

## 20. 预约 (Reservations)

### 获取预约列表
```
GET /api/v1/reservations
```

### 创建预约
```
POST /api/v1/reservations
```
**Request:**
```json
{
  "semesterId": "string",              // [必填]
  "labId": "string",                   // [必填]
  "useDate": "string",
  "dayOfWeek": 0,
  "periodNumbers": [0],
  "weekNumber": 0,
  "expectedDurationHours": 0,
  "projectName": "string",             // [必填]
  "projectCategory": "string",
  "remark": "string",
  "projectLeaderId": "string",
  "projectLeaderName": "string",
  "projectLeaderPhone": "string",
  "memberGrade": "string",
  "memberClassId": "string",
  "memberClassName": "string",
  "memberCount": 0
}
```

### 获取预约详情
```
GET /api/v1/reservations/{id}
```

### 审批预约
```
PUT /api/v1/reservations/{id}/approve
```

### 取消预约
```
PUT /api/v1/reservations/{id}/cancel
```

### 驳回预约
```
PUT /api/v1/reservations/{id}/reject
```

### 获取待审批预约
```
GET /api/v1/reservations/pending
```

---

## 21. 使用登记 (UsageRegistrations)

### 获取使用登记列表
```
GET /api/v1/usage-registrations
```

### 创建使用登记
```
POST /api/v1/usage-registrations
```
**Request:**
```json
{
  "semesterId": "string",              // [必填]
  "labId": "string",
  "labName": "string",
  "useDate": "string",
  "weekNumber": 0,
  "dayOfWeek": 0,
  "periodNumber": 0,
  "source": "string",
  "scheduleEntryId": "string",
  "reservationId": "string",
  "teachingApplicationId": "string",
  "experimentTaskId": "string",
  "teachingTaskId": "string",
  "courseName": "string",
  "projectName": "string",
  "experimentItemName": "string",
  "experimentItemType": "string",
  "plannedHours": 0,
  "actualHours": 0,
  "className": "string",
  "expectedStudentCount": 0,
  "actualStudentCount": 0,
  "attendanceRecord": "string",
  "teachingCondition": "string",
  "equipmentCondition": "string"
}
```

### 获取使用登记详情
```
GET /api/v1/usage-registrations/{id}
```

### 催办使用登记
```
PUT /api/v1/usage-registrations/{id}/remind
```

### 获取待登记
```
GET /api/v1/usage-registrations/pending
```

### 获取已逾期登记
```
GET /api/v1/usage-registrations/overdue
```

### 获取完成率统计
```
GET /api/v1/usage-registrations/statistics/completion
```

---

## 22. 统计 (Statistics)

### 获取仪表盘数据
```
GET /api/v1/statistics/dashboard
```

### 按班级统计
```
GET /api/v1/statistics/by-class
```

### 按课程统计
```
GET /api/v1/statistics/by-course
```

### 按年级统计
```
GET /api/v1/statistics/by-grade
```

### 按专业统计
```
GET /api/v1/statistics/by-major
```

### 获取完成率
```
GET /api/v1/statistics/completion-rate
```

### 获取实验室使用统计
```
GET /api/v1/statistics/lab-usage
```

### 获取预约统计
```
GET /api/v1/statistics/reservation
```

### 获取每周摘要
```
GET /api/v1/statistics/weekly-summary
```

### 导出统计数据
```
GET /api/v1/statistics/export
```

---

## 23. 实验管理 (Experiments)

### 教学任务
```
GET  /api/Experiments/tasks
POST /api/Experiments/tasks
GET  /api/Experiments/tasks/{id}
PUT  /api/Experiments/tasks/{id}
DELETE /api/Experiments/tasks/{id}
```

### 实验项目
```
GET  /api/Experiments/items
POST /api/Experiments/items
GET  /api/Experiments/items/{id}
PUT  /api/Experiments/items/{id}
DELETE /api/Experiments/items/{id}
GET  /api/Experiments/items/by-task/{taskId}
```

### 排课计划
```
GET  /api/Experiments/schedules
POST /api/Experiments/schedules
GET  /api/Experiments/schedules/{id}
PUT  /api/Experiments/schedules/{id}
DELETE /api/Experiments/schedules/{id}
```

### 质量评估
```
GET  /api/Experiments/quality
POST /api/Experiments/quality
GET  /api/Experiments/quality/{id}
PUT  /api/Experiments/quality/{id}
DELETE /api/Experiments/quality/{id}
```

### 培养计划
```
GET  /api/Experiments/training-plans
POST /api/Experiments/training-plans
GET  /api/Experiments/training-plans/{id}
PUT  /api/Experiments/training-plans/{id}
DELETE /api/Experiments/training-plans/{id}
POST /api/Experiments/training-plans/{id}/approve
```

---

## 24. 导出 (Export)

### 导出实验排课计划
```
GET /api/export/experiment/schedule-plan
```

### 导出实验任务列表
```
GET /api/export/experiment/task-list
```

### 获取导出模板列表
```
GET /api/export/templates
```

---

## 25. 数据模型 (Schemas)

### 通用枚举

**SemesterType (学期类型)**
- `First` — 第一学期
- `Second` — 第二学期
- `Summer` — 小学期
- `Winter` — 冬季学期

**SemesterStatus (学期状态)**
- `Draft` — 草稿
- `Active` — 进行中
- `Completed` — 已结束
- `Archived` — 已归档

**CalendarEventType (校历事件类型)**
- `Holiday` — 假期
- `Exam` — 考试
- `Holiday` — 节假日
- `Teaching` — 教学
- `Registration` — 注册
- 自定义事件类型

**CalendarEventPriority (校历事件优先级)**
- `High` — 高
- `Normal` — 普通
- `Low` — 低

**ScheduleSource (排课来源)**
- `Manual` — 手动
- `Reservation` — 预约
- `TeachingApplication` — 授课申请
- `Import` — 导入

---

### 核心实体

#### User (用户)
| 字段 | 类型 | 说明 |
|------|------|------|
| id | string | 用户ID |
| username | string | 用户名 |
| email | string | 邮箱 |
| phone | string | 电话 |
| fullName | string | 全名 |
| departmentId | string | 部门ID |
| isActive | boolean | 是否启用 |
| employeeId | string | 员工号 |
| studentId | string | 学号 |
| createdAt | string | 创建时间 |
| department | Department | 所属部门 |
| userRoles | UserRole[] | 用户角色 |

#### Role (角色)
| 字段 | 类型 | 说明 |
|------|------|------|
| id | string | 角色ID |
| code | string | 角色代码 |
| name | string | 角色名称 |
| description | string | 描述 |
| isSystem | boolean | 是否系统角色 |
| isActive | boolean | 是否启用 |
| rolePermissions | RolePermission[] | 角色权限 |

#### Semester (学期)
| 字段 | 类型 | 说明 |
|------|------|------|
| id | string | 学期ID |
| name | string | 学期名称 |
| code | string | 学期代码 |
| academicYear | string | 学年 |
| semesterType | SemesterType | 学期类型 |
| startDate | string | 开始日期 |
| endDate | string | 结束日期 |
| teachingStartDate | string | 教学开始日期 |
| teachingEndDate | string | 教学结束日期 |
| totalWeeks | int | 总周数 |
| teachingWeeks | int | 教学周数 |
| status | SemesterStatus | 状态 |
| isCurrent | boolean | 是否当前学期 |
| isSandbox | boolean | 是否沙箱 |
| isTemplate | boolean | 是否模板 |

#### Course (课程)
| 字段 | 类型 | 说明 |
|------|------|------|
| id | string | 课程ID |
| code | string | 课程代码 |
| name | string | 课程名称 |
| credits | number | 学分 |
| totalHours | int | 总学时 |
| theoryHours | int | 理论学时 |
| practiceHours | int | 实践学时 |
| experimentHours | int | 实验学时 |
| departmentId | string | 所属院系 |

#### Lab (实验室)
| 字段 | 类型 | 说明 |
|------|------|------|
| id | string | 实验室ID |
| code | string | 实验室代码 |
| name | string | 实验室名称 |
| buildingId | string | 所属建筑 |
| floor | int | 楼层 |
| roomNumber | string | 房间号 |
| capacity | int | 容量 |
| labType | string | 实验室类型 |
| safetyLevel | string | 安全等级 |
| isActive | boolean | 是否启用 |

#### Equipment (设备)
| 字段 | 类型 | 说明 |
|------|------|------|
| id | string | 设备ID |
| code | string | 设备编号 |
| name | string | 设备名称 |
| model | string | 型号 |
| manufacturer | string | 制造商 |
| serialNumber | string | 序列号 |
| labId | string | 所属实验室 |
| category | string | 类别 |
| status | string | 状态 |
| totalQuantity | int | 总数量 |
| availableQuantity | int | 可用数量 |
| price | number | 价格 |
| warrantyMonths | int | 保修月数 |

#### ReservationDto (预约)
| 字段 | 类型 | 说明 |
|------|------|------|
| id | string | 预约ID |
| semesterId | string | 学期ID |
| labId | string | 实验室ID |
| labName | string | 实验室名称 |
| useDate | string | 使用日期 |
| weekNumber | int | 周次 |
| dayOfWeek | int | 星期 |
| periodNumbers | int[] | 节次 |
| projectName | string | 项目名称 |
| applicantId | string | 申请人ID |
| applicantName | string | 申请人姓名 |
| status | string | 状态 |

#### UsageRegistrationDto (使用登记)
| 字段 | 类型 | 说明 |
|------|------|------|
| id | string | 登记ID |
| semesterId | string | 学期ID |
| labId | string | 实验室ID |
| labName | string | 实验室名称 |
| useDate | string | 使用日期 |
| weekNumber | int | 周次 |
| dayOfWeek | int | 星期 |
| periodNumber | int | 节次 |
| courseName | string | 课程名称 |
| projectName | string | 项目名称 |
| expectedStudentCount | int | 预期学生数 |
| actualStudentCount | int | 实际学生数 |
| attendanceRecord | string | 出勤记录 |
| status | string | 状态 |

#### ConflictCheckResult (冲突检测结果)
| 字段 | 类型 | 说明 |
|------|------|------|
| hasHardConflict | boolean | 是否有硬冲突 |
| hasSoftConflict | boolean | 是否有软冲突 |
| hardConflicts | ConflictItem[] | 硬冲突列表 |
| softConflicts | ConflictItem[] | 软冲突列表 |
| canForceSchedule | boolean | 能否强制排课 |

#### ConflictItem (冲突项)
| 字段 | 类型 | 说明 |
|------|------|------|
| id | string | 冲突ID |
| type | string | 冲突类型 |
| message | string | 冲突信息 |
| labName | string | 实验室名称 |
| weekNumber | int | 周次 |
| dayOfWeek | int | 星期 |
| periodNumber | int | 节次 |

#### CompletionRate (完成率)
| 字段 | 类型 | 说明 |
|------|------|------|
| total | int | 总数 |
| completed | int | 已完成 |
| pending | int | 待处理 |
| overdue | int | 已逾期 |
| rate | number | 完成率 |

---

## 认证方式

本系统使用 **JWT Bearer Token** 认证。

登录成功后，服务端返回 JWT Token。在后续请求的 Header 中携带：

```
Authorization: Bearer <your_jwt_token>
```

**JWT 配置信息：**
- Issuer: `LimsAuth`
- Audience: `LimsClient`
- 过期时间: 60 分钟（默认）

---
