# LIMS 高校实验室管理系统 - 后端 API 文档

> 本文档详细描述 backend-new 目录下新重构后端的数据库设计。所有字段均来自原始需求文档，无任何猜测。

---

## 数据库实体设计 (25张表)

### 一、系统基础表 (7张)

#### 1. Sys_User (用户表)

| 字段名 | 数据类型 | 说明 | 约束 |
|--------|----------|------|------|
| UserID | GUID | 用户唯一标识 | PK |
| UserName | NVARCHAR(50) | 用户名 | NOT NULL, UNIQUE |
| Password | NVARCHAR(255) | 密码(BCrypt加密) | NOT NULL |
| RealName | NVARCHAR(100) | 真实姓名 | NOT NULL |
| EmployeeNo | NVARCHAR(50) | 工号 | UNIQUE |
| IDCard | NVARCHAR(18) | 身份证号 | |
| Gender | INT | 性别(0女/1男/2未知) | DEFAULT 2 |
| Mobile | NVARCHAR(20) | 手机号 | |
| Email | NVARCHAR(100) | 邮箱 | |
| MainInstitutionID | GUID | 主机构ID | FK(Sys_Institution) |
| MainDepartmentID | GUID | 主部门ID | FK(Sys_Department) |
| UserType | NVARCHAR(20) | 用户类型(Admin/Teacher/Student) | |
| Status | INT | 状态(0禁用/1启用) | DEFAULT 1 |
| Avatar | NVARCHAR(500) | 头像URL | |
| LastLoginTime | DATETIME | 最后登录时间 | |
| LastLoginIP | NVARCHAR(50) | 最后登录IP | |
| PasswordUpdateTime | DATETIME | 密码更新时间 | |
| LoginFailCount | INT | 登录失败次数 | DEFAULT 0 |
| IsDeleted | BIT | 逻辑删除 | DEFAULT 0 |
| CreatedAt | DATETIME | 创建时间 | |
| CreatedBy | GUID | 创建人ID | |
| UpdatedAt | DATETIME | 更新时间 | |
| UpdatedBy | GUID | 更新人ID | |

#### 2. Sys_Role (角色表)

| 字段名 | 数据类型 | 说明 | 约束 |
|--------|----------|------|------|
| RoleID | GUID | 角色唯一标识 | PK |
| RoleCode | NVARCHAR(50) | 角色编码 | NOT NULL, UNIQUE |
| RoleName | NVARCHAR(100) | 角色名称 | NOT NULL |
| Description | NVARCHAR(500) | 描述 | |
| IsSystem | INT | 是否系统内置(0否/1是) | DEFAULT 0 |
| Status | INT | 状态(0禁用/1启用) | DEFAULT 1 |
| CreatedAt | DATETIME | 创建时间 | |
| CreatedBy | GUID | 创建人ID | |

#### 3. Sys_Permission (权限表)

| 字段名 | 数据类型 | 说明 | 约束 |
|--------|----------|------|------|
| PermissionID | GUID | 权限唯一标识 | PK |
| PermissionCode | NVARCHAR(100) | 权限编码(module:action) | NOT NULL, UNIQUE |
| PermissionName | NVARCHAR(100) | 权限名称 | NOT NULL |
| Module | NVARCHAR(50) | 所属模块 | NOT NULL |
| Description | NVARCHAR(500) | 描述 | |
| CreatedAt | DATETIME | 创建时间 | |

#### 4. Sys_UserRole (用户角色关联表)

| 字段名 | 数据类型 | 说明 | 约束 |
|--------|----------|------|------|
| UserID | GUID | 用户ID | PK, FK(Sys_User) |
| RoleID | GUID | 角色ID | PK, FK(Sys_Role) |
| AssignedAt | DATETIME | 分配时间 | |

#### 5. Sys_RolePermission (角色权限关联表)

| 字段名 | 数据类型 | 说明 | 约束 |
|--------|----------|------|------|
| RoleID | GUID | 角色ID | PK, FK(Sys_Role) |
| PermissionID | GUID | 权限ID | PK, FK(Sys_Permission) |
| AssignedAt | DATETIME | 分配时间 | |

#### 6. Sys_Institution (机构表)

| 字段名 | 数据类型 | 说明 | 约束 |
|--------|----------|------|------|
| InstitutionID | GUID | 机构唯一标识 | PK |
| InstitutionCode | NVARCHAR(50) | 机构编码 | NOT NULL, UNIQUE |
| InstitutionName | NVARCHAR(100) | 机构名称 | NOT NULL |
| ParentID | GUID | 上级机构ID | FK(Sys_Institution) |
| InstitutionType | NVARCHAR(50) | 机构类型(University/College/Department/Center) | |
| Level | INT | 机构层级(从1开始) | DEFAULT 1 |
| FullPath | NVARCHAR(500) | 完整路径(1/2/5) | |
| Status | INT | 状态(0禁用/1启用) | DEFAULT 1 |
| Description | NVARCHAR(500) | 描述 | |
| SortOrder | INT | 排序号 | DEFAULT 0 |
| CreatedAt | DATETIME | 创建时间 | |
| CreatedBy | GUID | 创建人ID | |
| UpdatedAt | DATETIME | 更新时间 | |
| UpdatedBy | GUID | 更新人ID | |

#### 7. Sys_Department (部门表)

| 字段名 | 数据类型 | 说明 | 约束 |
|--------|----------|------|------|
| DepartmentID | GUID | 部门唯一标识 | PK |
| DepartmentCode | NVARCHAR(50) | 部门编码 | NOT NULL, UNIQUE |
| DepartmentName | NVARCHAR(100) | 部门名称 | NOT NULL |
| InstitutionID | GUID | 所属机构ID | FK(Sys_Institution) |
| ParentID | GUID | 父部门ID | FK(Sys_Department) |
| DepartmentType | NVARCHAR(50) | 部门类型 | |
| Level | INT | 部门层级 | DEFAULT 1 |
| FullPath | NVARCHAR(500) | 完整路径 | |
| ManagerID | GUID | 部门负责人ID | FK(Sys_User) |
| Status | INT | 状态(0禁用/1启用) | DEFAULT 1 |
| Description | NVARCHAR(500) | 描述 | |
| SortOrder | INT | 排序号 | DEFAULT 0 |
| CreatedAt | DATETIME | 创建时间 | |
| CreatedBy | GUID | 创建人ID | |
| UpdatedAt | DATETIME | 更新时间 | |
| UpdatedBy | GUID | 更新人ID | |

---

### 二、基础教学表 (6张)

#### 8. Edu_Semester (学期表)

| 字段名 | 数据类型 | 说明 | 约束 |
|--------|----------|------|------|
| SemesterID | GUID | 学期唯一标识 | PK |
| SemesterCode | NVARCHAR(50) | 学期编码 | NOT NULL, UNIQUE |
| SemesterName | NVARCHAR(100) | 学期名称 | NOT NULL |
| SchoolYear | NVARCHAR(20) | 学年(如2024-2025) | |
| SemesterNo | INT | 学期序号(1或2) | |
| StartDate | DATETIME | 学期开始日期 | |
| EndDate | DATETIME | 学期结束日期 | |
| TotalWeeks | INT | 总周数 | |
| IsCurrent | INT | 是否当前学期(0否/1是) | DEFAULT 0 |
| Status | INT | 状态(0禁用/1启用) | DEFAULT 1 |
| Description | NVARCHAR(500) | 描述 | |
| SortOrder | INT | 排序号 | DEFAULT 0 |
| IsDeleted | BIT | 逻辑删除 | DEFAULT 0 |
| CreatedAt | DATETIME | 创建时间 | |
| CreatedBy | GUID | 创建人ID | |
| UpdatedAt | DATETIME | 更新时间 | |
| UpdatedBy | GUID | 更新人ID | |

#### 9. Edu_Course (课程表)

| 字段名 | 数据类型 | 说明 | 约束 |
|--------|----------|------|------|
| CourseID | GUID | 课程唯一标识 | PK |
| CourseCode | NVARCHAR(50) | 课程编码 | NOT NULL, UNIQUE |
| CourseName | NVARCHAR(200) | 课程中文名 | NOT NULL |
| CourseNameEn | NVARCHAR(200) | 课程英文名 | |
| CourseNature | NVARCHAR(20) | 课程性质(必修/选修/公选) | |
| Credits | DECIMAL(10,2) | 学分 | |
| TotalHours | INT | 总学时 | |
| LectureHours | INT | 讲授学时 | |
| PracticeHours | INT | 实践学时 | |
| LabHours | INT | 实验学时 | |
| OnlineHours | INT | 线上学时 | |
| OpenSemesters | NVARCHAR(50) | 开设学期(如"1,2") | |
| SortOrder | INT | 排序号 | DEFAULT 0 |
| Status | INT | 状态(0禁用/1启用) | DEFAULT 1 |
| Description | NVARCHAR(1000) | 课程描述 | |
| IsDeleted | BIT | 逻辑删除 | DEFAULT 0 |
| CreatedAt | DATETIME | 创建时间 | |
| CreatedBy | GUID | 创建人ID | |
| UpdatedAt | DATETIME | 更新时间 | |
| UpdatedBy | GUID | 更新人ID | |

#### 10. Edu_Major (专业表)

| 字段名 | 数据类型 | 说明 | 约束 |
|--------|----------|------|------|
| MajorID | GUID | 专业唯一标识 | PK |
| MajorCode | NVARCHAR(50) | 专业编码 | NOT NULL, UNIQUE |
| MajorName | NVARCHAR(200) | 专业中文名 | NOT NULL |
| MajorNameEn | NVARCHAR(200) | 专业英文名 | |
| InstitutionID | GUID | 所属机构ID | FK(Sys_Institution) |
| DepartmentID | GUID | 所属部门ID | FK(Sys_Department) |
| DegreeLevel | NVARCHAR(20) | 学历层次(本科/硕士/博士/专科) | |
| Duration | INT | 学制年限(年) | |
| DegreeName | NVARCHAR(100) | 学位名称(工学学士等) | |
| SortOrder | INT | 排序号 | DEFAULT 0 |
| Status | INT | 状态(0禁用/1启用) | DEFAULT 1 |
| Description | NVARCHAR(500) | 描述 | |
| IsDeleted | BIT | 逻辑删除 | DEFAULT 0 |
| CreatedAt | DATETIME | 创建时间 | |
| CreatedBy | GUID | 创建人ID | |
| UpdatedAt | DATETIME | 更新时间 | |
| UpdatedBy | GUID | 更新人ID | |

#### 11. Edu_Class (班级表)

| 字段名 | 数据类型 | 说明 | 约束 |
|--------|----------|------|------|
| ClassID | GUID | 班级唯一标识 | PK |
| ClassCode | NVARCHAR(50) | 班级编码 | NOT NULL, UNIQUE |
| ClassName | NVARCHAR(100) | 班级名称 | NOT NULL |
| InstitutionID | GUID | 所属机构ID | FK(Sys_Institution) |
| DepartmentID | GUID | 所属部门ID | FK(Sys_Department) |
| MajorID | GUID | 所属专业ID | FK(Edu_Major) |
| GradeName | NVARCHAR(20) | 年级名称(如2021) | |
| MonitorID | GUID | 班长ID | FK(Sys_User) |
| HeadTeacherID | GUID | 班主任ID | FK(Sys_User) |
| StudentCount | INT | 学生人数 | |
| SortOrder | INT | 排序号 | DEFAULT 0 |
| Status | INT | 状态(0禁用/1启用) | DEFAULT 1 |
| Description | NVARCHAR(500) | 描述 | |
| IsDeleted | BIT | 逻辑删除 | DEFAULT 0 |
| CreatedAt | DATETIME | 创建时间 | |
| CreatedBy | GUID | 创建人ID | |
| UpdatedAt | DATETIME | 更新时间 | |
| UpdatedBy | GUID | 更新人ID | |

#### 12. Edu_ClassStudent (班级学生关联表)

| 字段名 | 数据类型 | 说明 | 约束 |
|--------|----------|------|------|
| ClassID | GUID | 班级ID | PK, FK(Edu_Class) |
| StudentID | GUID | 学生ID | PK, FK(Sys_User) |
| JoinedAt | DATETIME | 加入时间 | |

#### 13. Edu_TeachingTask (教学任务表)

| 字段名 | 数据类型 | 说明 | 约束 |
|--------|----------|------|------|
| TaskID | GUID | 教学任务唯一标识 | PK |
| TaskCode | NVARCHAR(50) | 教学任务编码 | NOT NULL, UNIQUE |
| SemesterID | GUID | 所属学期ID | FK(Edu_Semester) |
| CourseID | GUID | 课程ID | FK(Edu_Course) |
| MajorID | GUID | 专业ID | FK(Edu_Major) |
| ClassID | GUID | 班级ID | FK(Edu_Class) |
| WeeklyHours | INT | 周课时 | |
| StartWeek | INT | 开始周次 | |
| EndWeek | INT | 结束周次 | |
| ExamMode | NVARCHAR(50) | 考核方式 | |
| SortOrder | INT | 排序号 | DEFAULT 0 |
| Status | INT | 状态(0禁用/1启用) | DEFAULT 1 |
| Description | NVARCHAR(500) | 描述 | |
| IsDeleted | BIT | 逻辑删除 | DEFAULT 0 |
| CreatedAt | DATETIME | 创建时间 | |
| CreatedBy | GUID | 创建人ID | |
| UpdatedAt | DATETIME | 更新时间 | |
| UpdatedBy | GUID | 更新人ID | |

#### 14. Edu_TeachingTaskTeacher (教学任务教师关联表)

| 字段名 | 数据类型 | 说明 | 约束 |
|--------|----------|------|------|
| ID | GUID | 主键 | PK |
| TaskID | GUID | 教学任务ID | FK(Edu_TeachingTask) |
| TeacherID | GUID | 教师ID | FK(Sys_User) |
| TeacherRole | NVARCHAR(20) | 教师角色(主讲/助教) | DEFAULT '主讲' |
| AssignedAt | DATETIME | 分配时间 | |

---

### 三、场地管理表 (2张)

#### 15. Ven_Building (楼宇表)

| 字段名 | 数据类型 | 说明 | 约束 |
|--------|----------|------|------|
| BuildingID | GUID | 楼宇唯一标识 | PK |
| BuildingCode | NVARCHAR(50) | 楼宇编码 | NOT NULL, UNIQUE |
| BuildingName | NVARCHAR(200) | 楼宇名称 | NOT NULL |
| BuildingNameEn | NVARCHAR(200) | 楼宇英文名 | |
| InstitutionID | GUID | 所属机构ID | FK(Sys_Institution) |
| Address | NVARCHAR(500) | 地址 | |
| TotalFloors | INT | 总层数 | |
| Area | DECIMAL(18,2) | 建筑面积 | |
| BuildYear | INT | 建造年份 | |
| UseType | NVARCHAR(50) | 用途类型 | |
| SortOrder | INT | 排序号 | DEFAULT 0 |
| Status | INT | 状态(0禁用/1启用) | DEFAULT 1 |
| Description | NVARCHAR(500) | 描述 | |
| IsDeleted | BIT | 逻辑删除 | DEFAULT 0 |
| CreatedAt | DATETIME | 创建时间 | |
| CreatedBy | GUID | 创建人ID | |
| UpdatedAt | DATETIME | 更新时间 | |
| UpdatedBy | GUID | 更新人ID | |

#### 16. Ven_Room (实验室/场地表)

| 字段名 | 数据类型 | 说明 | 约束 |
|--------|----------|------|------|
| RoomID | GUID | 场地唯一标识 | PK |
| RoomCode | NVARCHAR(50) | 场地编码 | NOT NULL, UNIQUE |
| RoomName | NVARCHAR(200) | 场地名称 | NOT NULL |
| BuildingID | GUID | 所属楼宇ID | FK(Ven_Building) |
| FloorNo | INT | 楼层号 | |
| RoomNumber | NVARCHAR(50) | 房间号 | |
| SeatCount | INT | 座位数量 | |
| Area | DECIMAL(18,2) | 面积 | |
| RoomType | NVARCHAR(50) | 场地类型(计算机实验室/多媒体教室等) | |
| Photo | NVARCHAR(1000) | 照片URL | |
| IsAvailable | INT | 是否可用(0否/1是) | DEFAULT 1 |
| SortOrder | INT | 排序号 | DEFAULT 0 |
| Status | INT | 状态(0禁用/1启用) | DEFAULT 1 |
| Description | NVARCHAR(500) | 描述 | |
| IsDeleted | BIT | 逻辑删除 | DEFAULT 0 |
| CreatedAt | DATETIME | 创建时间 | |
| CreatedBy | GUID | 创建人ID | |
| UpdatedAt | DATETIME | 更新时间 | |
| UpdatedBy | GUID | 更新人ID | |

---

### 四、排课预约表 (4张)

#### 17. Lab_Schedule (排课记录表)

| 字段名 | 数据类型 | 说明 | 约束 |
|--------|----------|------|------|
| ScheduleID | GUID | 排课唯一标识 | PK |
| SemesterID | GUID | 学期ID | FK(Edu_Semester) |
| RoomID | GUID | 实验室ID | FK(Ven_Room) |
| TaskID | GUID | 教学任务ID | FK(Edu_TeachingTask) |
| WeekNo | INT | 教学周次 | |
| DayOfWeek | INT | 星期几(1-7) | |
| SectionNo | NVARCHAR(50) | 节次(如"1-2节") | |
| CourseName | NVARCHAR(200) | 课程名称 | |
| ClassID | GUID | 班级ID | FK(Edu_Class) |
| TeacherID | GUID | 授课教师ID | FK(Sys_User) |
| ExperimentItemID | GUID | 实验项目ID | FK(Lab_ExperimentItem) |
| ScheduleType | NVARCHAR(50) | 排课类型(CentralScheduling/BookingApply/SelfBooking) | |
| Status | INT | 状态(0禁用/1启用) | DEFAULT 1 |
| Remark | NVARCHAR(500) | 备注 | |
| IsDeleted | BIT | 逻辑删除 | DEFAULT 0 |
| CreatedAt | DATETIME | 创建时间 | |
| CreatedBy | GUID | 操作人ID | |
| UpdatedAt | DATETIME | 更新时间 | |

#### 18. Lab_ExperimentItem (实验项目库)

| 字段名 | 数据类型 | 说明 | 约束 |
|--------|----------|------|------|
| ItemID | GUID | 项目唯一标识 | PK |
| ItemCode | NVARCHAR(50) | 项目编码 | NOT NULL, UNIQUE |
| ItemName | NVARCHAR(200) | 实验项目名称 | NOT NULL |
| CourseID | GUID | 所属课程ID | FK(Edu_Course) |
| ExperimentType | NVARCHAR(50) | 实验类型(基础/综合/设计/其他) | |
| StandardHours | INT | 计划学时 | |
| Status | INT | 状态(0禁用/1启用) | DEFAULT 1 |
| IsDeleted | BIT | 逻辑删除 | DEFAULT 0 |
| CreatedAt | DATETIME | 创建时间 | |
| CreatedBy | GUID | 创建人ID | |

#### 19. Lab_BookingApply (预约申请表)

| 字段名 | 数据类型 | 说明 | 约束 |
|--------|----------|------|------|
| ApplyID | GUID | 申请唯一标识 | PK |
| ApplyCode | NVARCHAR(50) | 申请单号 | NOT NULL, UNIQUE |
| ApplicantID | GUID | 申请人ID | FK(Sys_User) |
| ApplicantType | NVARCHAR(20) | 申请人身份(教师/学生) | |
| RoomID | GUID | 申请实验室ID | FK(Ven_Room) |
| Purpose | NVARCHAR(1000) | 申请用途/原因 | |
| TargetDate | DATETIME | 使用日期 | |
| TargetSection | NVARCHAR(50) | 使用节次 | |
| WeekNo | INT | 所属周次 | |
| EstimatedPeople | INT | 预计人数 | |
| AuditStatus | NVARCHAR(20) | 审批状态(Pending/Approved/Rejected) | |
| AuditOpinion | NVARCHAR(500) | 审批意见/驳回原因 | |
| AuditorID | GUID | 审批管理员ID | FK(Sys_User) |
| AuditTime | DATETIME | 审批时间 | |
| IsDeleted | BIT | 逻辑删除 | DEFAULT 0 |
| CreatedAt | DATETIME | 申请提交时间 | |
| CreatedBy | GUID | 操作人ID | |

#### 20. Lab_UsageRegister (使用登记表)

| 字段名 | 数据类型 | 说明 | 约束 |
|--------|----------|------|------|
| RegisterID | GUID | 登记唯一标识 | PK |
| ScheduleID | GUID | 关联排课记录ID | FK(Lab_Schedule) |
| BookingApplyID | GUID | 关联预约申请ID | FK(Lab_BookingApply) |
| SemesterID | GUID | 学期ID | FK(Edu_Semester) |
| RoomID | GUID | 实验室ID | FK(Ven_Room) |
| ItemName | NVARCHAR(200) | 实验项目名称/授课内容 | |
| ExperimentType | NVARCHAR(50) | 实验类型 | |
| PlannedHours | INT | 计划学时 | |
| ActualHours | DECIMAL(10,2) | 实际使用时长 | |
| ClassName | NVARCHAR(100) | 使用班级名称 | |
| ExpectedCount | INT | 应到人数 | |
| ActualCount | INT | 实到人数 | |
| AttendanceRecord | NVARCHAR(500) | 考勤记录 | |
| TeachingRecord | NVARCHAR(200) | 教学情况记录 | |
| DeviceRecord | NVARCHAR(200) | 仪器设备情况 | |
| RegisterStatus | NVARCHAR(20) | 登记状态(Pending/Completed/Overdue) | |
| RegisterUserID | GUID | 填报人ID | FK(Sys_User) |
| RegisterTime | DATETIME | 填报时间 | |
| IsDeleted | BIT | 逻辑删除 | DEFAULT 0 |
| CreatedAt | DATETIME | 创建时间 | |
| CreatedBy | GUID | 操作人ID | |

---

### 五、设备管理表 (2张)

#### 21. Dev_Asset (设备资产台账)

| 字段名 | 数据类型 | 说明 | 约束 |
|--------|----------|------|------|
| AssetID | GUID | 设备唯一标识 | PK |
| AssetCode | NVARCHAR(50) | 资产编号(条码号) | NOT NULL, UNIQUE |
| DeviceName | NVARCHAR(200) | 设备名称 | NOT NULL |
| ModelNumber | NVARCHAR(100) | 型号 | |
| Specification | NVARCHAR(200) | 规格 | |
| Category | NVARCHAR(50) | 类别/分类 | |
| Unit | NVARCHAR(20) | 计量单位 | DEFAULT '台' |
| PurchaseDate | DATETIME | 购入日期 | |
| Brand | NVARCHAR(100) | 品牌 | |
| SerialNumber | NVARCHAR(100) | 序列号/出厂编号 | |
| Price | DECIMAL(18,2) | 价格 | |
| FundingSource | NVARCHAR(100) | 经费来源 | |
| ServiceLife | INT | 使用年限 | |
| Supplier | NVARCHAR(200) | 供应商 | |
| WarrantyPeriod | DATETIME | 保修期至 | |
| StorageLocation | NVARCHAR(200) | 存放位置 | |
| ResponsibleUserID | GUID | 责任人ID | FK(Sys_User) |
| DepartmentID | GUID | 所属部门ID | FK(Sys_Department) |
| IsImportant | INT | 是否重要设备(0否/1是) | DEFAULT 0 |
| LabelInfo | NVARCHAR(200) | 标签/备注信息 | |
| PhotoPath | NVARCHAR(1000) | 设备照片路径 | |
| DeviceStatus | NVARCHAR(50) | 设备状态(Available/OnLoan/Maintenance/Scrap) | DEFAULT 'Available' |
| TotalQuantity | INT | 总数量 | DEFAULT 1 |
| AvailableQuantity | INT | 可用数量 | DEFAULT 1 |
| Status | INT | 状态(0禁用/1启用) | DEFAULT 1 |
| Description | NVARCHAR(500) | 描述 | |
| IsDeleted | BIT | 逻辑删除 | DEFAULT 0 |
| CreatedAt | DATETIME | 入库登记时间 | |
| CreatedBy | GUID | 创建人ID | |
| UpdatedAt | DATETIME | 更新时间 | |
| UpdatedBy | GUID | 更新人ID | |

#### 22. Dev_LoanApply (设备借还申请)

| 字段名 | 数据类型 | 说明 | 约束 |
|--------|----------|------|------|
| LoanID | GUID | 借还唯一标识 | PK |
| AssetID | GUID | 设备资产ID | FK(Dev_Asset) |
| BorrowerID | GUID | 借用人ID | FK(Sys_User) |
| BorrowerType | NVARCHAR(20) | 借用人类型(教师/学生) | |
| LoanPurpose | NVARCHAR(1000) | 借用用途/关联课程班级 | |
| LoanQuantity | INT | 借出数量 | DEFAULT 1 |
| ExpectedReturnDate | DATETIME | 预计归还时间 | |
| ActualLoanTime | DATETIME | 实际借出放行时间 | |
| ActualReturnTime | DATETIME | 实际归还时间 | |
| AuditStatus | NVARCHAR(20) | 审批状态(Pending/Approved/Rejected/Returned) | |
| AuditorID | GUID | 审批管理员ID | FK(Sys_User) |
| AuditTime | DATETIME | 审批时间 | |
| AuditOpinion | NVARCHAR(500) | 审批意见 | |
| ReturnConfirmUserID | GUID | 归还确认人ID | FK(Sys_User) |
| DeviceCondition | NVARCHAR(50) | 归还时设备状况(完好/损坏) | |
| Remark | NVARCHAR(500) | 备注 | |
| IsDeleted | BIT | 逻辑删除 | DEFAULT 0 |
| CreatedAt | DATETIME | 创建时间 | |
| CreatedBy | GUID | 操作人ID | |

---

### 六、耗材管理表 (4张)

#### 23. Mat_Consumable (耗材基础表)

| 字段名 | 数据类型 | 说明 | 约束 |
|--------|----------|------|------|
| MatID | GUID | 耗材唯一标识 | PK |
| MatCode | NVARCHAR(50) | 耗材编号 | NOT NULL, UNIQUE |
| MatName | NVARCHAR(200) | 耗材名称 | NOT NULL |
| Category | NVARCHAR(50) | 分类(化学试剂/电子元件/玻璃仪器等) | |
| SpecModel | NVARCHAR(200) | 规格型号 | |
| Unit | NVARCHAR(20) | 单位(瓶/个/盒) | DEFAULT '个' |
| CurrentStock | DECIMAL(18,4) | 当前总库存 | DEFAULT 0 |
| AvailableStock | DECIMAL(18,4) | 可用库存 | DEFAULT 0 |
| LockedStock | DECIMAL(18,4) | 锁定库存(审批中被预占) | DEFAULT 0 |
| MinStockThreshold | DECIMAL(18,4) | 最低库存警戒线 | DEFAULT 0 |
| StorageLocation | NVARCHAR(200) | 存放位置 | |
| Supplier | NVARCHAR(200) | 供应商 | |
| UnitPrice | DECIMAL(18,2) | 单价 | |
| Status | INT | 状态(0禁用/1启用) | DEFAULT 1 |
| IsDeleted | BIT | 逻辑删除 | DEFAULT 0 |
| CreatedAt | DATETIME | 创建时间 | |
| CreatedBy | GUID | 创建人ID | |
| UpdatedAt | DATETIME | 更新时间 | |
| UpdatedBy | GUID | 更新人ID | |

#### 24. Mat_InboundOrder (耗材入库单)

| 字段名 | 数据类型 | 说明 | 约束 |
|--------|----------|------|------|
| InboundID | GUID | 入库单唯一标识 | PK |
| InboundCode | NVARCHAR(50) | 入库单号 | NOT NULL, UNIQUE |
| MatID | GUID | 耗材ID | FK(Mat_Consumable) |
| InboundQty | DECIMAL(18,4) | 入库数量 | |
| UnitPrice | DECIMAL(18,2) | 采购单价 | |
| Supplier | NVARCHAR(200) | 供应商 | |
| InboundTime | DATETIME | 入库时间 | |
| HandlerID | GUID | 经办人ID | FK(Sys_User) |
| AuditStatus | NVARCHAR(20) | 审核状态(Pending/Approved/Rejected) | |
| AuditorID | GUID | 审核管理员ID | FK(Sys_User) |
| AuditTime | DATETIME | 审核时间 | |
| AuditRemark | NVARCHAR(500) | 审核意见 | |
| Remark | NVARCHAR(500) | 备注 | |
| IsDeleted | BIT | 逻辑删除 | DEFAULT 0 |
| CreatedAt | DATETIME | 创建时间 | |
| CreatedBy | GUID | 操作人ID | |

#### 25. Mat_OutboundOrder (耗材出库单)

| 字段名 | 数据类型 | 说明 | 约束 |
|--------|----------|------|------|
| OutboundID | GUID | 出库单唯一标识 | PK |
| OutboundCode | NVARCHAR(50) | 出库单号/领用单号 | NOT NULL, UNIQUE |
| MatID | GUID | 耗材ID | FK(Mat_Consumable) |
| RequestQty | DECIMAL(18,4) | 申请领用数量 | |
| Purpose | NVARCHAR(1000) | 使用用途/实验项目说明 | |
| TargetRoomID | GUID | 使用实验室ID | FK(Ven_Room) |
| RequesterID | GUID | 领用申请人ID | FK(Sys_User) |
| OutTime | DATETIME | 实际出库/扣减库存时间 | |
| AuditStatus | NVARCHAR(20) | 审批状态(Pending/Approved/Rejected) | |
| AuditorID | GUID | 审批人ID | FK(Sys_User) |
| AuditTime | DATETIME | 审批时间 | |
| AuditRemark | NVARCHAR(500) | 审批意见 | |
| Remark | NVARCHAR(500) | 备注 | |
| IsDeleted | BIT | 逻辑删除 | DEFAULT 0 |
| CreatedAt | DATETIME | 创建时间 | |
| CreatedBy | GUID | 操作人ID | |

#### 26. Mat_StockLog (库存变动日志)

| 字段名 | 数据类型 | 说明 | 约束 |
|--------|----------|------|------|
| LogID | GUID | 日志唯一标识 | PK |
| MatID | GUID | 耗材ID | FK(Mat_Consumable) |
| ChangeType | NVARCHAR(50) | 变动类型(Inbound/Outbound/Adjustment/Loss/Scrap) | |
| OldQty | DECIMAL(18,4) | 变动前库存 | |
| ChangeQty | DECIMAL(18,4) | 变动数量(正数或负数) | |
| NewQty | DECIMAL(18,4) | 变动后库存 | |
| ReferenceID | GUID | 关联单据ID(入库单/出库单) | |
| ReferenceNo | NVARCHAR(50) | 关联单据号 | |
| OperatorID | GUID | 操作人ID | FK(Sys_User) |
| Reason | NVARCHAR(500) | 变动说明 | |
| CreatedAt | DATETIME | 变动时间 | |

---

## 数据库 ER 关系图

```mermaid
erDiagram
    Sys_User ||--o{ Sys_UserRole : has
    Sys_Role ||--o{ Sys_UserRole : assigned_to
    Sys_Role ||--o{ Sys_RolePermission : has
    Sys_Permission ||--o{ Sys_RolePermission : assigned_to
    Sys_Institution ||--o{ Sys_Institution : parent
    Sys_Institution ||--o{ Sys_Department : contains
    Sys_Department ||--o{ Sys_Department : parent
    Sys_Department ||--o{ Sys_User : contains
    Edu_Semester ||--o{ Edu_TeachingTask : contains
    Edu_Course ||--o{ Edu_TeachingTask : used_in
    Edu_Major ||--o{ Edu_Class : contains
    Edu_Class ||--o{ Edu_ClassStudent : contains
    Sys_User ||--o{ Edu_ClassStudent : student
    Edu_TeachingTask ||--o{ Edu_TeachingTaskTeacher : assigned
    Sys_User ||--o{ Edu_TeachingTaskTeacher : teacher
    Sys_Institution ||--o{ Ven_Building : contains
    Ven_Building ||--o{ Ven_Room : contains
    Edu_Semester ||--o{ Lab_Schedule : used_in
    Ven_Room ||--o{ Lab_Schedule : used_in
    Edu_TeachingTask ||--o{ Lab_Schedule : used_in
    Edu_Class ||--o{ Lab_Schedule : used_in
    Sys_User ||--o{ Lab_Schedule : teacher
    Lab_ExperimentItem ||--o{ Lab_Schedule : used_in
    Edu_Course ||--o{ Lab_ExperimentItem : belongs_to
    Sys_User ||--o{ Lab_BookingApply : applicant
    Ven_Room ||--o{ Lab_BookingApply : applied_for
    Sys_User ||--o{ Lab_UsageRegister : register_user
    Ven_Room ||--o{ Lab_UsageRegister : used_in
    Edu_Semester ||--o{ Lab_UsageRegister : belongs_to
    Sys_Department ||--o{ Dev_Asset : contains
    Sys_User ||--o{ Dev_Asset : responsible
    Dev_Asset ||--o{ Dev_LoanApply : borrowed
    Sys_User ||--o{ Dev_LoanApply : borrower
    Mat_Consumable ||--o{ Mat_InboundOrder : inbound
    Sys_User ||--o{ Mat_InboundOrder : handler
    Mat_Consumable ||--o{ Mat_OutboundOrder : outbound
    Sys_User ||--o{ Mat_OutboundOrder : requester
    Ven_Room ||--o{ Mat_OutboundOrder : target
    Mat_Consumable ||--o{ Mat_StockLog : changes
    Sys_User ||--o{ Mat_StockLog : operator
```

---

## 索引设计

| 表名 | 索引字段 | 索引类型 | 唯一 |
|------|----------|----------|------|
| Sys_User | UserName | UNIQUE | YES |
| Sys_User | EmployeeNo | | NO |
| Sys_User | MainDepartmentID | | NO |
| Sys_Role | RoleCode | UNIQUE | YES |
| Sys_Permission | PermissionCode | UNIQUE | YES |
| Sys_Institution | InstitutionCode | UNIQUE | YES |
| Sys_Department | DepartmentCode | UNIQUE | YES |
| Edu_Semester | SemesterCode | UNIQUE | YES |
| Edu_Course | CourseCode | UNIQUE | YES |
| Edu_Major | MajorCode | UNIQUE | YES |
| Edu_Class | ClassCode | UNIQUE | YES |
| Ven_Building | BuildingCode | UNIQUE | YES |
| Ven_Room | RoomCode | UNIQUE | YES |
| Lab_ExperimentItem | ItemCode | UNIQUE | YES |
| Lab_BookingApply | ApplyCode | UNIQUE | YES |
| Dev_Asset | AssetCode | UNIQUE | YES |
| Mat_Consumable | MatCode | UNIQUE | YES |
| Mat_InboundOrder | InboundCode | UNIQUE | YES |
| Mat_OutboundOrder | OutboundCode | UNIQUE | YES |
| Mat_StockLog | MatID, CreatedAt | Composite | NO |

---

## 种子数据

系统初始包含以下数据:

### 角色 (3个)
- super_admin (超级管理员) - 拥有所有权限
- teacher (教师) - 基础读写权限
- student (学生) - 只读权限

### 用户 (3个)
- admin / admin123 - 超级管理员
- teacher / teacher123 - 教师
- student / student123 - 学生

### 机构 (2个)
- 测试大学 (University)
- 计算机学院 (College)

### 部门 (1个)
- 计算机系

### 学期 (1个)
- 2024-2025学年第一学期 (当前学期)

### 课程 (2个)
- CS101 数据结构
- CS201 算法设计

### 专业 (1个)
- 计算机科学与技术

### 班级 (1个)
- 计算机21级1班

### 楼宇 (1个)
- 理学楼A

### 实验室 (2个)
- 计算机实验室101
- 计算机实验室102

### 权限点 (65个)
覆盖 user/role/permission/institution/department/semester/course/major/class/teachingtask/building/room/schedule/booking/usage/asset/loan/consumable/experimentitem 等19个模块
