# LIMS 后端 API 接口规范

> 本文档详细描述 backend-new 后端所有 API 接口定义。所有字段均来自原始需求文档，无任何猜测。

---

## 一、认证相关 (Auth)

### 1.1 登录
```
POST /api/v1/auth/login
```
**Request:**
| 字段 | 类型 | 必填 | 说明 |
|------|------|------|------|
| Username | string | 是 | 用户名 |
| Password | string | 是 | 密码 |

**Response:**
| 字段 | 类型 | 说明 |
|------|------|------|
| Code | int | 状态码(200成功/401失败) |
| Message | string | 消息 |
| Data.Token | string | JWT Token |
| Data.TokenType | string | Token类型(Bearer) |
| Data.ExpiresIn | long | 有效期(秒) |
| Data.User.Id | GUID | 用户ID |
| Data.User.Username | string | 用户名 |
| Data.User.RealName | string | 真实姓名 |
| Data.User.Email | string | 邮箱 |
| Data.User.Mobile | string | 手机号 |
| Data.User.Roles | string[] | 角色列表 |
| Data.User.Permissions | string[] | 权限码列表 |

### 1.2 获取当前用户
```
GET /api/v1/auth/me
```
**Response:** 同登录响应中的 User 字段

### 1.3 修改密码
```
POST /api/v1/auth/change-password
```
**Request:**
| 字段 | 类型 | 必填 | 说明 |
|------|------|------|------|
| OldPassword | string | 是 | 原密码 |
| NewPassword | string | 是 | 新密码(最少6位) |

### 1.4 更新资料
```
PUT /api/v1/auth/profile
```
**Request:**
| 字段 | 类型 | 说明 |
|------|------|------|
| RealName | string | 真实姓名 |
| Email | string | 邮箱 |
| Mobile | string | 手机号 |

---

## 二、用户管理 (Users)

### 2.1 获取用户列表
```
GET /api/v1/users
```
**Query Parameters:**
| 字段 | 类型 | 说明 |
|------|------|------|
| Keyword | string | 搜索关键词(用户名/姓名) |
| DepartmentId | GUID | 部门ID |
| IsActive | bool | 是否启用 |
| Page | int | 页码(默认1) |
| PageSize | int | 每页数量(默认20) |

**Response:**
| 字段 | 类型 | 说明 |
|------|------|------|
| Items[].Id | GUID | 用户ID |
| Items[].Username | string | 用户名 |
| Items[].RealName | string | 真实姓名 |
| Items[].EmployeeNo | string | 工号 |
| Items[].Gender | int | 性别 |
| Items[].Mobile | string | 手机号 |
| Items[].Email | string | 邮箱 |
| Items[].MainDepartmentName | string | 部门名称 |
| Items[].UserType | string | 用户类型 |
| Items[].Status | int | 状态 |
| Items[].LastLoginTime | datetime | 最后登录时间 |
| Items[].CreatedAt | datetime | 创建时间 |
| Items[].Roles | RoleBriefDto[] | 角色列表 |
| Total | int | 总数 |
| Page | int | 当前页 |
| PageSize | int | 每页数量 |
| TotalPages | int | 总页数 |

### 2.2 获取用户详情
```
GET /api/v1/users/{id}
```
**Response:** 用户列表字段 + Permissions 权限列表

### 2.3 创建用户
```
POST /api/v1/users
```
**Request:**
| 字段 | 类型 | 必填 | 说明 |
|------|------|------|------|
| Username | string | 是 | 用户名 |
| Password | string | 是 | 密码(最少6位) |
| RealName | string | 是 | 真实姓名 |
| EmployeeNo | string | 否 | 工号 |
| Gender | int | 否 | 性别(0女/1男/2未知) |
| Mobile | string | 否 | 手机号 |
| Email | string | 否 | 邮箱 |
| MainInstitutionId | GUID | 否 | 主机构ID |
| MainDepartmentId | GUID | 否 | 主部门ID |
| UserType | string | 否 | 用户类型 |
| RoleIds | GUID[] | 否 | 角色ID列表 |
| Status | int | 否 | 状态(默认1) |

### 2.4 更新用户
```
PUT /api/v1/users/{id}
```
**Request:** 同创建用户,所有字段可选

### 2.5 删除用户
```
DELETE /api/v1/users/{id}
```
**Response:** `{ Code: 200, Message: "用户删除成功" }`

### 2.6 分配角色
```
PUT /api/v1/users/{id}/roles
```
**Request:**
| 字段 | 类型 | 说明 |
|------|------|------|
| RoleIds | GUID[] | 角色ID列表 |

### 2.7 重置密码
```
PUT /api/v1/users/{id}/password
```
**Request:**
| 字段 | 类型 | 说明 |
|------|------|------|
| NewPassword | string | 新密码 |

---

## 三、角色管理 (Roles)

### 3.1 获取角色列表
```
GET /api/v1/roles
```
**Query Parameters:**
| 字段 | 类型 | 说明 |
|------|------|------|
| Keyword | string | 搜索关键词 |
| IsActive | bool | 是否启用 |
| Page | int | 页码 |
| PageSize | int | 每页数量 |

**Response:**
| 字段 | 类型 | 说明 |
|------|------|------|
| Items[].Id | GUID | 角色ID |
| Items[].Code | string | 角色编码 |
| Items[].Name | string | 角色名称 |
| Items[].Description | string | 描述 |
| Items[].IsSystem | int | 是否系统内置 |
| Items[].Status | int | 状态 |
| Items[].UserCount | int | 关联用户数 |

### 3.2 获取角色详情
```
GET /api/v1/roles/{id}
```
**Response:** 角色列表字段 + Permissions 权限列表

### 3.3 创建角色
```
POST /api/v1/roles
```
**Request:**
| 字段 | 类型 | 必填 | 说明 |
|------|------|------|------|
| Code | string | 是 | 角色编码 |
| Name | string | 是 | 角色名称 |
| Description | string | 否 | 描述 |

### 3.4 更新角色
```
PUT /api/v1/roles/{id}
```
**Request:** 同创建,所有字段可选

### 3.5 删除角色
```
DELETE /api/v1/roles/{id}
```
**前置条件:** 非系统内置,无关联用户

### 3.6 分配权限
```
PUT /api/v1/roles/{id}/permissions
```
**Request:**
| 字段 | 类型 | 说明 |
|------|------|------|
| PermissionIds | GUID[] | 权限ID列表 |

---

## 四、权限管理 (Permissions)

### 4.1 获取所有权限
```
GET /api/v1/permissions
```
**Response:**
| 字段 | 类型 | 说明 |
|------|------|------|
| [].Id | GUID | 权限ID |
| [].Code | string | 权限编码 |
| [].Name | string | 权限名称 |
| [].Module | string | 所属模块 |
| [].Description | string | 描述 |

### 4.2 按模块分组获取
```
GET /api/v1/permissions/by-module
```
**Response:**
| 字段 | 类型 | 说明 |
|------|------|------|
| [].Module | string | 模块名 |
| [].ModuleName | string | 模块中文名 |
| [].Permissions | PermissionDto[] | 权限列表 |

---

## 五、机构管理 (Institutions)

### 5.1 获取机构树
```
GET /api/v1/institutions
```
**Response:**
| 字段 | 类型 | 说明 |
|------|------|------|
| [].Id | GUID | 机构ID |
| [].Code | string | 机构编码 |
| [].Name | string | 机构名称 |
| [].ParentId | GUID | 上级机构ID |
| [].ParentName | string | 上级机构名称 |
| [].InstitutionType | string | 机构类型 |
| [].Level | int | 层级 |
| [].Status | int | 状态 |
| [].Children | InstitutionDto[] | 子机构 |

### 5.2 创建机构
```
POST /api/v1/institutions
```
**Request:**
| 字段 | 类型 | 必填 | 说明 |
|------|------|------|------|
| Code | string | 是 | 机构编码 |
| Name | string | 是 | 机构名称 |
| ParentId | GUID | 否 | 上级机构ID |
| InstitutionType | string | 否 | 机构类型 |
| Status | int | 否 | 状态(默认1) |
| Description | string | 否 | 描述 |

### 5.3 更新机构
```
PUT /api/v1/institutions/{id}
```
### 5.4 删除机构
```
DELETE /api/v1/institutions/{id}
```

---

## 六、部门管理 (Departments)

### 6.1 获取部门树
```
GET /api/v1/departments
```
### 6.2 创建部门
```
POST /api/v1/departments
```
**Request:**
| 字段 | 类型 | 必填 | 说明 |
|------|------|------|------|
| Code | string | 是 | 部门编码 |
| Name | string | 是 | 部门名称 |
| InstitutionId | GUID | 否 | 所属机构ID |
| ParentId | GUID | 否 | 父部门ID |
| DepartmentType | string | 否 | 部门类型 |
| ManagerId | GUID | 否 | 部门负责人ID |
| Status | int | 否 | 状态(默认1) |
| Description | string | 否 | 描述 |

### 6.3 更新/删除部门
```
PUT /api/v1/departments/{id}
DELETE /api/v1/departments/{id}
```

---

## 七、学期管理 (Semesters)

### 7.1 获取学期列表
```
GET /api/v1/semesters
```
**Query Parameters:**
| 字段 | 类型 | 说明 |
|------|------|------|
| Keyword | string | 搜索关键词 |
| Page | int | 页码 |
| PageSize | int | 每页数量 |

**Response:**
| 字段 | 类型 | 说明 |
|------|------|------|
| Items[].Id | GUID | 学期ID |
| Items[].Code | string | 学期编码 |
| Items[].Name | string | 学期名称 |
| Items[].SchoolYear | string | 学年 |
| Items[].SemesterNo | int | 学期序号 |
| Items[].StartDate | datetime | 开始日期 |
| Items[].EndDate | datetime | 结束日期 |
| Items[].TotalWeeks | int | 总周数 |
| Items[].IsCurrent | int | 是否当前学期 |
| Items[].Status | int | 状态 |

### 7.2 获取当前学期
```
GET /api/v1/semesters/current
```
### 7.3 创建学期
```
POST /api/v1/semesters
```
**Request:**
| 字段 | 类型 | 必填 | 说明 |
|------|------|------|------|
| Code | string | 是 | 学期编码 |
| Name | string | 是 | 学期名称 |
| SchoolYear | string | 否 | 学年 |
| SemesterNo | int | 否 | 学期序号 |
| StartDate | datetime | 是 | 开始日期 |
| EndDate | datetime | 是 | 结束日期 |
| TotalWeeks | int | 否 | 总周数 |
| IsCurrent | int | 否 | 是否当前(默认0) |
| Status | int | 否 | 状态(默认1) |

### 7.4 更新/删除学期
```
PUT /api/v1/semesters/{id}
DELETE /api/v1/semesters/{id}
```

### 7.5 设为当前学期
```
POST /api/v1/semesters/{id}/set-current
```

---

## 八、课程管理 (Courses)

### 8.1 获取课程列表
```
GET /api/v1/courses
```
**Query Parameters:**
| 字段 | 类型 | 说明 |
|------|------|------|
| Keyword | string | 搜索关键词 |
| Page | int | 页码 |
| PageSize | int | 每页数量 |

**Response:**
| 字段 | 类型 | 说明 |
|------|------|------|
| Items[].Id | GUID | 课程ID |
| Items[].Code | string | 课程编码 |
| Items[].Name | string | 课程名称 |
| Items[].NameEn | string | 英文名 |
| Items[].Nature | string | 课程性质 |
| Items[].Credits | decimal | 学分 |
| Items[].TotalHours | int | 总学时 |
| Items[].LectureHours | int | 讲授学时 |
| Items[].PracticeHours | int | 实践学时 |
| Items[].LabHours | int | 实验学时 |
| Items[].OnlineHours | int | 线上学时 |
| Items[].OpenSemesters | string | 开设学期 |
| Items[].Status | int | 状态 |

### 8.2 创建/更新/删除/切换状态
```
POST /api/v1/courses
PUT /api/v1/courses/{id}
DELETE /api/v1/courses/{id}
PATCH /api/v1/courses/{id}/status
```

---

## 九、专业管理 (Majors)

### 9.1 获取专业列表
```
GET /api/v1/majors
```
**Response:**
| 字段 | 类型 | 说明 |
|------|------|------|
| Items[].Id | GUID | 专业ID |
| Items[].Code | string | 专业编码 |
| Items[].Name | string | 专业名称 |
| Items[].NameEn | string | 英文名 |
| Items[].InstitutionName | string | 所属机构 |
| Items[].DepartmentName | string | 所属部门 |
| Items[].DegreeLevel | string | 学历层次 |
| Items[].Duration | int | 学制 |
| Items[].DegreeName | string | 学位名称 |
| Items[].Status | int | 状态 |

### 9.2 CRUD操作
```
POST /api/v1/majors
PUT /api/v1/majors/{id}
DELETE /api/v1/majors/{id}
PATCH /api/v1/majors/{id}/status
```

---

## 十、班级管理 (Classes)

### 10.1 获取班级列表
```
GET /api/v1/classes
```
**Response:**
| 字段 | 类型 | 说明 |
|------|------|------|
| Items[].Id | GUID | 班级ID |
| Items[].Code | string | 班级编码 |
| Items[].Name | string | 班级名称 |
| Items[].MajorName | string | 所属专业 |
| Items[].GradeName | string | 年级 |
| Items[].StudentCount | int | 学生人数 |
| Items[].Status | int | 状态 |

### 10.2 获取班级学生
```
GET /api/v1/classes/{id}/students
```
**Response:**
| 字段 | 类型 | 说明 |
|------|------|------|
| [].Id | GUID | 学生ID |
| [].Username | string | 用户名 |
| [].RealName | string | 真实姓名 |
| [].EmployeeNo | string | 学号 |
| [].Mobile | string | 手机号 |

### 10.3 分配学生
```
PUT /api/v1/classes/{id}/students
```
**Request:**
| 字段 | 类型 | 说明 |
|------|------|------|
| StudentIds | GUID[] | 学生ID列表 |

### 10.4 CRUD操作
```
POST /api/v1/classes
PUT /api/v1/classes/{id}
DELETE /api/v1/classes/{id}
PATCH /api/v1/classes/{id}/status
```

---

## 十一、教学任务 (TeachingTasks)

### 11.1 获取教学任务列表
```
GET /api/v1/teaching-tasks
```
**Response:**
| 字段 | 类型 | 说明 |
|------|------|------|
| Items[].Id | GUID | 任务ID |
| Items[].Code | string | 任务编码 |
| Items[].SemesterName | string | 学期名称 |
| Items[].CourseName | string | 课程名称 |
| Items[].MajorName | string | 专业名称 |
| Items[].ClassName | string | 班级名称 |
| Items[].WeeklyHours | int | 周课时 |
| Items[].StartWeek | int | 开始周 |
| Items[].EndWeek | int | 结束周 |
| Items[].ExamMode | string | 考核方式 |
| Items[].Status | int | 状态 |
| Items[].Teachers | TeacherBriefDto[] | 教师列表 |

### 11.2 分配教师
```
PUT /api/v1/teaching-tasks/{id}/teachers
```
**Request:**
| 字段 | 类型 | 说明 |
|------|------|------|
| Teachers[].TeacherId | GUID | 教师ID |
| Teachers[].Role | string | 角色(主讲/助教) |

### 11.3 CRUD操作
```
POST /api/v1/teaching-tasks
PUT /api/v1/teaching-tasks/{id}
DELETE /api/v1/teaching-tasks/{id}
```

---

## 十二、楼宇管理 (Buildings)

### 12.1 获取楼宇列表
```
GET /api/v1/buildings
```
**Response:**
| 字段 | 类型 | 说明 |
|------|------|------|
| Items[].Id | GUID | 楼宇ID |
| Items[].Code | string | 楼宇编码 |
| Items[].Name | string | 楼宇名称 |
| Items[].NameEn | string | 英文名 |
| Items[].InstitutionName | string | 所属机构 |
| Items[].Address | string | 地址 |
| Items[].TotalFloors | int | 总层数 |
| Items[].Area | decimal | 建筑面积 |
| Items[].BuildYear | int | 建造年份 |
| Items[].Status | int | 状态 |

### 12.2 CRUD操作
```
POST /api/v1/buildings
PUT /api/v1/buildings/{id}
DELETE /api/v1/buildings/{id}
```

---

## 十三、实验室管理 (Rooms)

### 13.1 获取实验室列表
```
GET /api/v1/rooms
```
**Response:**
| 字段 | 类型 | 说明 |
|------|------|------|
| Items[].Id | GUID | 实验室ID |
| Items[].Code | string | 实验室编码 |
| Items[].Name | string | 实验室名称 |
| Items[].BuildingName | string | 所属楼宇 |
| Items[].FloorNo | int | 楼层 |
| Items[].RoomNumber | string | 房间号 |
| Items[].SeatCount | int | 座位数 |
| Items[].Area | decimal | 面积 |
| Items[].RoomType | string | 实验室类型 |
| Items[].IsAvailable | int | 是否可用 |
| Items[].Status | int | 状态 |

### 13.2 CRUD操作
```
POST /api/v1/rooms
PUT /api/v1/rooms/{id}
DELETE /api/v1/rooms/{id}
```

---

## 十四、排课管理 (Schedules)

### 14.1 获取排课列表
```
GET /api/v1/schedules
```
**Query Parameters:**
| 字段 | 类型 | 说明 |
|------|------|------|
| SemesterId | GUID | 学期ID |
| WeekNo | int | 周次 |
| RoomId | GUID | 实验室ID |
| BuildingId | GUID | 楼宇ID |
| ClassId | GUID | 班级ID |
| TeacherId | GUID | 教师ID |
| Status | int | 状态 |
| Page | int | 页码 |
| PageSize | int | 每页数量 |

**Response:**
| 字段 | 类型 | 说明 |
|------|------|------|
| Items[].Id | GUID | 排课ID |
| Items[].SemesterName | string | 学期名称 |
| Items[].RoomName | string | 实验室名称 |
| Items[].BuildingName | string | 楼宇名称 |
| Items[].CourseName | string | 课程名称 |
| Items[].ClassName | string | 班级名称 |
| Items[].TeacherName | string | 教师姓名 |
| Items[].WeekNo | int | 周次 |
| Items[].DayOfWeek | int | 星期 |
| Items[].SectionNo | string | 节次 |
| Items[].ScheduleType | string | 排课类型 |
| Items[].Status | int | 状态 |

### 14.2 创建排课
```
POST /api/v1/schedules
```
**Request:**
| 字段 | 类型 | 必填 | 说明 |
|------|------|------|------|
| SemesterId | GUID | 是 | 学期ID |
| RoomId | GUID | 否 | 实验室ID |
| TeachingTaskId | GUID | 否 | 教学任务ID |
| WeekNo | int | 是 | 周次 |
| DayOfWeek | int | 是 | 星期几(1-7) |
| SectionNo | string | 是 | 节次 |
| CourseName | string | 否 | 课程名称 |
| ClassId | GUID | 否 | 班级ID |
| TeacherId | GUID | 否 | 教师ID |
| ExperimentItemId | GUID | 否 | 实验项目ID |
| ScheduleType | string | 否 | 排课类型 |
| Status | int | 否 | 状态(默认1) |

### 14.3 更新/删除排课
```
PUT /api/v1/schedules/{id}
DELETE /api/v1/schedules/{id}
```

### 14.4 获取仪表盘
```
GET /api/v1/schedules/dashboard
```
**Response:**
| 字段 | 类型 | 说明 |
|------|------|------|
| TotalRooms | int | 实验室总数 |
| AvailableRooms | int | 可用实验室数 |
| PendingBookings | int | 待审批预约数 |
| TodaySchedules | int | 今日排课数 |
| TodayOccupancyRate | double | 今日使用率 |
| WeekOccupancyRate | int | 本周使用率 |
| Alerts | AlertItemDto[] | 预警信息 |

---

## 十五、实验项目 (ExperimentItems)

### 15.1 获取实验项目列表
```
GET /api/v1/experiment-items
```
**Response:**
| 字段 | 类型 | 说明 |
|------|------|------|
| Items[].Id | GUID | 项目ID |
| Items[].Code | string | 项目编码 |
| Items[].Name | string | 项目名称 |
| Items[].CourseName | string | 所属课程 |
| Items[].ExperimentType | string | 实验类型 |
| Items[].StandardHours | int | 计划学时 |
| Items[].Status | int | 状态 |

### 15.2 CRUD操作
```
POST /api/v1/experiment-items
PUT /api/v1/experiment-items/{id}
DELETE /api/v1/experiment-items/{id}
```

---

## 十六、预约申请 (BookingApplies)

### 16.1 获取预约列表
```
GET /api/v1/booking-applies
```
**Query Parameters:**
| 字段 | 类型 | 说明 |
|------|------|------|
| RoomId | GUID | 实验室ID |
| ApplicantId | GUID | 申请人ID |
| AuditStatus | string | 审批状态 |
| StartDate | datetime | 开始日期 |
| EndDate | datetime | 结束日期 |

**Response:**
| 字段 | 类型 | 说明 |
|------|------|------|
| Items[].Id | GUID | 申请ID |
| Items[].Code | string | 申请单号 |
| Items[].ApplicantName | string | 申请人姓名 |
| Items[].ApplicantType | string | 申请人类型 |
| Items[].RoomName | string | 实验室名称 |
| Items[].Purpose | string | 用途 |
| Items[].TargetDate | datetime | 使用日期 |
| Items[].TargetSection | string | 使用节次 |
| Items[].WeekNo | int | 周次 |
| Items[].EstimatedPeople | int | 预计人数 |
| Items[].AuditStatus | string | 审批状态 |
| Items[].AuditOpinion | string | 审批意见 |
| Items[].AuditorName | string | 审批人 |
| Items[].AuditTime | datetime | 审批时间 |

### 16.2 创建预约申请
```
POST /api/v1/booking-applies
```
**Request:**
| 字段 | 类型 | 必填 | 说明 |
|------|------|------|------|
| Code | string | 是 | 申请单号 |
| ApplicantId | GUID | 是 | 申请人ID |
| ApplicantType | string | 是 | 申请人类型 |
| RoomId | GUID | 是 | 实验室ID |
| Purpose | string | 否 | 用途 |
| TargetDate | datetime | 是 | 使用日期 |
| TargetSection | string | 是 | 使用节次 |
| WeekNo | int | 是 | 周次 |
| EstimatedPeople | int | 否 | 预计人数 |

### 16.3 审批预约
```
POST /api/v1/booking-applies/{id}/audit
```
**Request:**
| 字段 | 类型 | 说明 |
|------|------|------|
| Approved | bool | 是否通过 |
| Opinion | string | 审批意见 |

---

## 十七、使用登记 (UsageRegisters)

### 17.1 获取使用记录列表
```
GET /api/v1/usage-registers
```
**Query Parameters:**
| 字段 | 类型 | 说明 |
|------|------|------|
| SemesterId | GUID | 学期ID |
| RoomId | GUID | 实验室ID |
| RegisterUserId | GUID | 填报人ID |
| RegisterStatus | string | 登记状态 |
| StartDate | datetime | 开始日期 |
| EndDate | datetime | 结束日期 |

**Response:**
| 字段 | 类型 | 说明 |
|------|------|------|
| Items[].Id | GUID | 记录ID |
| Items[].SemesterName | string | 学期 |
| Items[].RoomName | string | 实验室 |
| Items[].ItemName | string | 实验项目 |
| Items[].ExperimentType | string | 实验类型 |
| Items[].PlannedHours | int | 计划学时 |
| Items[].ActualHours | double | 实际学时 |
| Items[].ClassName | string | 班级名称 |
| Items[].ExpectedCount | int | 应到人数 |
| Items[].ActualCount | int | 实到人数 |
| Items[].AttendanceRecord | string | 考勤记录 |
| Items[].TeachingRecord | string | 教学情况 |
| Items[].DeviceRecord | string | 设备情况 |
| Items[].RegisterStatus | string | 登记状态 |
| Items[].RegisterUserName | string | 填报人 |
| Items[].RegisterTime | datetime | 填报时间 |

### 17.2 创建使用登记
```
POST /api/v1/usage-registers
```
**Request:**
| 字段 | 类型 | 说明 |
|------|------|------|
| ScheduleId | GUID | 排课ID |
| BookingApplyId | GUID | 预约申请ID |
| SemesterId | GUID | 学期ID |
| RoomId | GUID | 实验室ID |
| ItemName | string | 实验项目名称 |
| ExperimentType | string | 实验类型 |
| PlannedHours | int | 计划学时 |
| ActualHours | double | 实际学时 |
| ClassName | string | 班级名称 |
| ExpectedCount | int | 应到人数 |
| ActualCount | int | 实到人数 |
| AttendanceRecord | string | 考勤记录 |
| TeachingRecord | string | 教学情况 |
| DeviceRecord | string | 设备情况 |

---

## 十八、设备资产 (Assets)

### 18.1 获取设备列表
```
GET /api/v1/assets
```
**Query Parameters:**
| 字段 | 类型 | 说明 |
|------|------|------|
| Keyword | string | 搜索关键词 |
| DepartmentId | GUID | 部门ID |
| ResponsibleUserId | GUID | 责任人ID |
| Category | string | 分类 |
| DeviceStatus | string | 设备状态 |
| Page | int | 页码 |
| PageSize | int | 每页数量 |

**Response:**
| 字段 | 类型 | 说明 |
|------|------|------|
| Items[].Id | GUID | 设备ID |
| Items[].Code | string | 资产编号 |
| Items[].Name | string | 设备名称 |
| Items[].ModelNumber | string | 型号 |
| Items[].Specification | string | 规格 |
| Items[].Category | string | 分类 |
| Items[].Unit | string | 单位 |
| Items[].PurchaseDate | datetime | 购入日期 |
| Items[].Brand | string | 品牌 |
| Items[].SerialNumber | string | 序列号 |
| Items[].Price | decimal | 价格 |
| Items[].FundingSource | string | 经费来源 |
| Items[].ServiceLife | int | 使用年限 |
| Items[].Supplier | string | 供应商 |
| Items[].WarrantyPeriod | datetime | 保修期 |
| Items[].StorageLocation | string | 存放位置 |
| Items[].ResponsibleUserName | string | 责任人 |
| Items[].DepartmentName | string | 部门 |
| Items[].IsImportant | int | 是否重要设备 |
| Items[].DeviceStatus | string | 设备状态 |
| Items[].TotalQuantity | int | 总数量 |
| Items[].AvailableQuantity | int | 可用数量 |
| Items[].Status | int | 状态 |

### 18.2 获取设备统计
```
GET /api/v1/assets/statistics
```
**Response:**
| 字段 | 类型 | 说明 |
|------|------|------|
| TotalAssets | int | 设备种类数 |
| TotalQuantity | int | 总数量 |
| AvailableQuantity | int | 可用数量 |
| ByCategory | Dictionary | 按分类统计 |
| ByStatus | Dictionary | 按状态统计 |

### 18.3 CRUD操作
```
POST /api/v1/assets
PUT /api/v1/assets/{id}
DELETE /api/v1/assets/{id}
```

---

## 十九、设备借还 (LoanApplies)

### 19.1 获取借还记录列表
```
GET /api/v1/loan-applies
```
**Query Parameters:**
| 字段 | 类型 | 说明 |
|------|------|------|
| AssetId | GUID | 设备ID |
| BorrowerId | GUID | 借用人ID |
| AuditStatus | string | 审批状态 |
| StartDate | datetime | 开始日期 |
| EndDate | datetime | 结束日期 |

**Response:**
| 字段 | 类型 | 说明 |
|------|------|------|
| Items[].Id | GUID | 记录ID |
| Items[].AssetCode | string | 资产编号 |
| Items[].AssetName | string | 设备名称 |
| Items[].BorrowerName | string | 借用人 |
| Items[].BorrowerType | string | 借用人类型 |
| Items[].LoanPurpose | string | 借用用途 |
| Items[].LoanQuantity | int | 借出数量 |
| Items[].ExpectedReturnDate | datetime | 预计归还 |
| Items[].ActualLoanTime | datetime | 实际借出时间 |
| Items[].ActualReturnTime | datetime | 实际归还时间 |
| Items[].AuditStatus | string | 审批状态 |
| Items[].AuditOpinion | string | 审批意见 |
| Items[].DeviceCondition | string | 归还时设备状况 |

### 19.2 创建借还申请
```
POST /api/v1/loan-applies
```
**Request:**
| 字段 | 类型 | 必填 | 说明 |
|------|------|------|------|
| AssetId | GUID | 是 | 设备ID |
| BorrowerId | GUID | 是 | 借用人ID |
| BorrowerType | string | 是 | 借用人类型 |
| LoanPurpose | string | 否 | 借用用途 |
| LoanQuantity | int | 否 | 借出数量(默认1) |
| ExpectedReturnDate | datetime | 是 | 预计归还时间 |

### 19.3 审批
```
POST /api/v1/loan-applies/{id}/audit
```
**Request:**
| 字段 | 类型 | 说明 |
|------|------|------|
| Approved | bool | 是否通过 |
| Opinion | string | 审批意见 |

### 19.4 确认归还
```
POST /api/v1/loan-applies/{id}/confirm-return
```
**Request:**
| 字段 | 类型 | 说明 |
|------|------|------|
| DeviceCondition | string | 设备状况(完好/损坏) |
| Remark | string | 备注 |

### 19.5 续借
```
POST /api/v1/loan-applies/{id}/renew
```
**Request:**
| 字段 | 类型 | 说明 |
|------|------|------|
| NewReturnDate | datetime | 新归还日期 |

---

## 二十、耗材管理 (Consumables)

### 20.1 获取耗材列表
```
GET /api/v1/consumables
```
**Query Parameters:**
| 字段 | 类型 | 说明 |
|------|------|------|
| Keyword | string | 搜索关键词 |
| Category | string | 分类 |
| Supplier | string | 供应商 |
| IsLowStock | bool | 仅低库存 |
| Page | int | 页码 |
| PageSize | int | 每页数量 |

**Response:**
| 字段 | 类型 | 说明 |
|------|------|------|
| Items[].Id | GUID | 耗材ID |
| Items[].Code | string | 耗材编号 |
| Items[].Name | string | 耗材名称 |
| Items[].Category | string | 分类 |
| Items[].SpecModel | string | 规格型号 |
| Items[].Unit | string | 单位 |
| Items[].CurrentStock | decimal | 当前库存 |
| Items[].AvailableStock | decimal | 可用库存 |
| Items[].LockedStock | decimal | 锁定库存 |
| Items[].MinStockThreshold | decimal | 最低库存警戒线 |
| Items[].StorageLocation | string | 存放位置 |
| Items[].Supplier | string | 供应商 |
| Items[].UnitPrice | decimal | 单价 |
| Items[].IsLowStock | bool | 是否低库存 |
| Items[].Status | int | 状态 |

### 20.2 获取耗材统计
```
GET /api/v1/consumables/statistics
```
**Response:**
| 字段 | 类型 | 说明 |
|------|------|------|
| TotalTypes | int | 耗材种类数 |
| LowStockTypes | int | 低库存种类 |
| OutOfStockTypes | int | 缺货种类 |
| TotalStockValue | decimal | 总库存价值 |
| ByCategory | Dictionary | 按分类统计 |
| LowStockItems | LowStockItemDto[] | 低库存列表 |

### 20.3 CRUD操作
```
POST /api/v1/consumables
PUT /api/v1/consumables/{id}
DELETE /api/v1/consumables/{id}
```

---

## 二十一、耗材入库 (ConsumableInRecords)

### 21.1 获取入库记录
```
GET /api/v1/consumable-in-records
```
**Query Parameters:**
| 字段 | 类型 | 说明 |
|------|------|------|
| ConsumableId | GUID | 耗材ID |
| HandlerId | GUID | 经办人ID |
| Status | string | 审核状态 |
| StartDate | datetime | 开始日期 |
| EndDate | datetime | 结束日期 |

**Response:**
| 字段 | 类型 | 说明 |
|------|------|------|
| Items[].Id | GUID | 入库单ID |
| Items[].Code | string | 入库单号 |
| Items[].ConsumableName | string | 耗材名称 |
| Items[].Quantity | decimal | 入库数量 |
| Items[].UnitPrice | decimal | 单价 |
| Items[].TotalAmount | decimal | 总金额 |
| Items[].Supplier | string | 供应商 |
| Items[].InboundTime | datetime | 入库时间 |
| Items[].HandlerName | string | 经办人 |
| Items[].AuditStatus | string | 审核状态 |
| Items[].AuditorName | string | 审核人 |

### 21.2 创建入库单
```
POST /api/v1/consumable-in-records
```
**Request:**
| 字段 | 类型 | 必填 | 说明 |
|------|------|------|------|
| Code | string | 是 | 入库单号 |
| ConsumableId | GUID | 是 | 耗材ID |
| Quantity | decimal | 是 | 入库数量 |
| UnitPrice | decimal | 否 | 单价 |
| Supplier | string | 否 | 供应商 |
| InboundTime | datetime | 否 | 入库时间 |
| Remark | string | 否 | 备注 |

### 21.3 审核入库
```
POST /api/v1/consumable-in-records/{id}/approve
```
**Request:**
| 字段 | 类型 | 说明 |
|------|------|------|
| Approved | bool | 是否通过 |
| Comment | string | 审核意见 |

---

## 二十二、耗材出库 (ConsumableOutRecords)

### 22.1 获取出库记录
```
GET /api/v1/consumable-out-records
```
**Response:**
| 字段 | 类型 | 说明 |
|------|------|------|
| Items[].Id | GUID | 出库单ID |
| Items[].Code | string | 出库单号 |
| Items[].ConsumableName | string | 耗材名称 |
| Items[].Quantity | decimal | 领用数量 |
| Items[].Purpose | string | 使用用途 |
| Items[].TargetRoomName | string | 使用实验室 |
| Items[].ApplicantName | string | 申请人 |
| Items[].AuditStatus | string | 审核状态 |

### 22.2 创建出库单
```
POST /api/v1/consumable-out-records
```
**Request:**
| 字段 | 类型 | 必填 | 说明 |
|------|------|------|------|
| Code | string | 是 | 出库单号 |
| ConsumableId | GUID | 是 | 耗材ID |
| Quantity | decimal | 是 | 领用数量 |
| Purpose | string | 否 | 用途 |
| TargetRoomId | GUID | 否 | 目标实验室ID |
| Remark | string | 否 | 备注 |

### 22.3 审核出库
```
POST /api/v1/consumable-out-records/{id}/approve
```

---

## 二十三、库存日志 (ConsumableStockLogs)

### 23.1 获取库存变动日志
```
GET /api/v1/consumable-stock-logs
```
**Query Parameters:**
| 字段 | 类型 | 说明 |
|------|------|------|
| ConsumableId | GUID | 耗材ID |
| ChangeType | string | 变动类型 |
| StartDate | datetime | 开始日期 |
| EndDate | datetime | 结束日期 |

**Response:**
| 字段 | 类型 | 说明 |
|------|------|------|
| Items[].Id | GUID | 日志ID |
| Items[].ConsumableName | string | 耗材名称 |
| Items[].ChangeType | string | 变动类型 |
| Items[].ChangeQuantity | decimal | 变动数量 |
| Items[].BeforeStock | decimal | 变动前库存 |
| Items[].AfterStock | decimal | 变动后库存 |
| Items[].ReferenceNo | string | 关联单据号 |
| Items[].OperatorName | string | 操作人 |
| Items[].Remark | string | 备注 |
| Items[].CreatedAt | datetime | 变动时间 |

### 23.2 手动调整库存
```
POST /api/v1/consumable-stock-logs/adjust
```
**Request:**
| 字段 | 类型 | 必填 | 说明 |
|------|------|------|------|
| ConsumableId | GUID | 是 | 耗材ID |
| AdjustmentType | string | 是 | 调整类型 |
| AdjustmentQuantity | decimal | 是 | 调整数量 |
| Reason | string | 否 | 调整原因 |

---

## 错误码定义

| 错误码 | 说明 |
|--------|------|
| 200 | 成功 |
| 400 | 请求参数错误 |
| 401 | 未授权(未登录或Token无效) |
| 403 | 无权限(权限不足) |
| 404 | 资源不存在 |
| 409 | 资源冲突(如用户名已存在) |
| 500 | 服务器内部错误 |

---

## 分页规范

所有列表接口统一使用以下分页格式:
```
{
  "code": 200,
  "message": "success",
  "data": {
    "items": [...],
    "total": 100,
    "page": 1,
    "pageSize": 20,
    "totalPages": 5
  }
}
```
