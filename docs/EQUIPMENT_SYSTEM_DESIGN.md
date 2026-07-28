# 实验室设备管理模块设计文档

## 一、模块概述

设备管理模块是 LIMS 高校实验室管理系统的核心组成部分，面向设备管理员、教师、学生三类角色，实现设备全流程规范化、可追溯管理。

### 1.1 功能范围

1. **设备登记** - 单台录入 + Excel 批量导入、资产编号唯一性校验
2. **设备管理** - 多维度查询、状态管理、位置/责任人更新、检定提醒
3. **设备借还** - 申请-审批-核验-归还全流程、续借与逾期处理
4. **库存监控** - 多维实时查询、统计报表、Excel 导出

### 1.2 技术栈

- **后端**: .NET 8.0 + ASP.NET Core Web API + Entity Framework Core + SQLite
- **前端**: Vue 3 + TypeScript + Element Plus + Vite + Axios
- **Excel处理**: EPPlus 7.5.2

---

## 二、数据库设计

### 2.1 实体清单

| 实体 | 表名 | 说明 |
|------|------|------|
| Equipment | equipments | 设备主表 |
| EquipmentBorrowRecord | equipment_borrow_records | 设备借还记录 |
| EquipmentCategory | equipment_categories | 设备分类字典 |

### 2.2 Equipment 设备实体

```csharp
[Table("equipments")]
public class Equipment
{
    public Guid Id { get; set; }           // 主键
    public string Code { get; set; }       // 资产编号（唯一）
    public string Name { get; set; }       // 设备名称
    public string? Model { get; set; }     // 型号
    public string? Brand { get; set; }     // 品牌
    public string? Manufacturer { get; set; } // 生产厂家
    public string? Supplier { get; set; }  // 供应商
    public string? SerialNumber { get; set; } // 序列号
    public Guid? LabId { get; set; }      // 所属实验室
    public Guid? DepartmentId { get; set; } // 所属部门
    public string Unit { get; set; }      // 计量单位
    public string Category { get; set; }   // 设备分类
    public decimal? Price { get; set; }    // 价格
    public DateTime? PurchaseDate { get; set; } // 购入日期
    public int? WarrantyMonths { get; set; } // 保修期（月）
    public DateTime? VerificationDate { get; set; } // 检定日期
    public DateTime? NextVerificationDate { get; set; } // 下次检定日期
    public string Status { get; set; }     // 设备状态
    public string? StatusReason { get; set; } // 状态变更原因
    public string? Location { get; set; }  // 存放位置
    public Guid? ResponsibleUserId { get; set; } // 责任人
    public string? ImageUrl { get; set; } // 设备照片
    public string? ContractUrl { get; set; } // 合同
    public string? ManualUrl { get; set; } // 说明书
    public string? CertificateUrl { get; set; } // 证书
    public string? Instructions { get; set; } // 使用说明
    public bool RequiresBooking { get; set; } // 需要预约
    public int? MaxBookingHours { get; set; } // 最大预约时长
    public int TotalQuantity { get; set; }  // 总数量
    public int AvailableQuantity { get; set; } // 可用数量
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

### 2.3 设备状态枚举

| 状态值 | 说明 | 颜色标签 |
|--------|------|----------|
| 在库-可用 | 正常在库，可借出 | success |
| 在库-待维修 | 需要维修 | warning |
| 在库-已预约 | 已被预约 | info |
| 借出 | 已被借出 | primary |
| 送修 | 正在外送维修 | danger |
| 报废 | 已报废 | info |
| 丢失 | 已丢失 | danger |

### 2.4 EquipmentBorrowRecord 借还记录实体

```csharp
[Table("equipment_borrow_records")]
public class EquipmentBorrowRecord
{
    public Guid Id { get; set; }
    public string RecordNo { get; set; }   // 借还单号（唯一）
    public Guid EquipmentId { get; set; }
    public Guid ApplicantId { get; set; }  // 申请人
    public Guid? ApproverId { get; set; } // 当前审批人
    public string Status { get; set; }     // 记录状态

    // 时间
    public DateTime BorrowDate { get; set; }        // 计划借出
    public DateTime ExpectedReturnDate { get; set; } // 计划归还
    public DateTime? ActualBorrowDate { get; set; }  // 实际借出
    public DateTime? ActualReturnDate { get; set; } // 实际归还

    // 用途
    public string Purpose { get; set; }
    public string? Remarks { get; set; }

    // 导师审批
    public string? SupervisorApprovalStatus { get; set; }
    public string? SupervisorApprovalRemark { get; set; }
    public DateTime? SupervisorApprovalDate { get; set; }

    // 管理员审批
    public string? AdminApprovalStatus { get; set; }
    public string? AdminApprovalRemark { get; set; }
    public DateTime? AdminApprovalDate { get; set; }

    // 归还验收
    public string? ReturnCondition { get; set; } // 完好/损坏/缺件
    public string? ReturnRemarks { get; set; }
    public Guid? ReturnCheckerId { get; set; }
    public DateTime? ReturnCheckDate { get; set; }

    // 续借
    public bool IsRenewed { get; set; }
    public DateTime? RenewedReturnDate { get; set; }
    public string? RenewApprovalStatus { get; set; }
}
```

### 2.5 借还状态流转

```
[学生提交申请]
    ↓
[待审批] → [导师审批中] → [管理员审批中] → [已借出]
    ↓(拒绝)                                         ↓
[已拒绝]                                      [归还审批中]
                                            ↓
                                      [已归还]
                                            ↓
                              (归还条件:完好/损坏/缺件)
```

---

## 三、API 端点设计

### 3.1 设备管理 API (/api/v1/equipments)

| 方法 | 端点 | 权限 | 说明 |
|------|------|------|------|
| GET | /api/v1/equipments | `equipment:read` | 多维度筛选列表 |
| GET | /api/v1/equipments/available | `equipment:read` | 可用设备（可借） |
| GET | /api/v1/equipments/{id} | `equipment:read` | 设备详情 |
| GET | /api/v1/equipments/code/{code} | `equipment:read` | 按资产编号查询 |
| POST | /api/v1/equipments | `equipment:create` | 单台录入 |
| POST | /api/v1/equipments/import | `equipment:create` | Excel批量导入 |
| PUT | /api/v1/equipments/{id} | `equipment:update` | 更新设备 |
| DELETE | /api/v1/equipments/{id} | `equipment:delete` | 删除设备 |
| PATCH | /api/v1/equipments/{id}/status | `equipment:update` | 启用/禁用 |
| PATCH | /api/v1/equipments/{id}/equipment-status | `equipment:update` | 更新设备状态 |
| GET | /api/v1/equipments/statistics | `equipment:statistics` | 统计数据 |
| GET | /api/v1/equipments/export | `equipment:export` | 导出Excel |

### 3.2 设备借还 API (/api/v1/borrow-records)

| 方法 | 端点 | 权限 | 说明 |
|------|------|------|------|
| POST | /api/v1/borrow-records | `equipment:borrow` | 提交借出申请 |
| GET | /api/v1/borrow-records/my | - | 我的借还记录 |
| GET | /api/v1/borrow-records/pending | `equipment:approve` | 待审批列表 |
| GET | /api/v1/borrow-records | `equipment:read` | 全部记录 |
| GET | /api/v1/borrow-records/{id} | - | 记录详情 |
| GET | /api/v1/borrow-records/no/{recordNo} | - | 按单号查询（扫码） |
| POST | /api/v1/borrow-records/{id}/supervisor-approve | `equipment:approve` | 导师审批 |
| POST | /api/v1/borrow-records/{id}/admin-approve | `equipment:approve` | 管理员审批 |
| POST | /api/v1/borrow-records/{id}/confirm-borrow | `equipment:approve` | 确认借出 |
| POST | /api/v1/borrow-records/{id}/submit-return | - | 提交归还 |
| POST | /api/v1/borrow-records/{id}/confirm-return | `equipment:approve` | 确认归还 |
| POST | /api/v1/borrow-records/{id}/renew | - | 续借申请 |
| GET | /api/v1/borrow-records/overdue | `equipment:read` | 逾期清单 |
| GET | /api/v1/borrow-records/expiring | `equipment:read` | 即将到期提醒 |
| GET | /api/v1/borrow-records/flow | `equipment:read` | 借还流水账 |

---

## 四、前端页面实现

### 4.1 页面清单

| 页面 | 路径 | 功能 |
|------|------|------|
| 设备台账 | /lab/equipments | 设备列表、多维查询、导入导出、详情抽屉 |
| 设备借还 | /lab/borrow-records | 我的申请/待审批/全部/逾期 四个标签页 |
| 统计报表 | /lab/statistics | 卡片统计、分类表格、状态分布、逾期清单 |

### 4.2 组件清单

| 组件 | 路径 | 功能 |
|------|------|------|
| EquipmentFormDialog | components/EquipmentFormDialog.vue | 新增/编辑设备表单 |
| EquipmentImportDialog | components/EquipmentImportDialog.vue | Excel批量导入 |

---

## 五、权限配置

### 5.1 权限清单

| 权限代码 | 说明 |
|----------|------|
| equipment:read | 查看设备列表和详情 |
| equipment:create | 创建设备、批量导入 |
| equipment:update | 更新设备信息、状态 |
| equipment:delete | 删除设备 |
| equipment:borrow | 提交借出申请 |
| equipment:approve | 审批借还申请、确认借出/归还 |
| equipment:statistics | 查看统计报表 |
| equipment:export | 导出Excel |

### 5.2 角色对应权限

| 角色 | 权限 |
|------|------|
| admin | 全部权限 |
| 教师 | equipment:read, equipment:borrow, equipment:approve |
| 学生 | equipment:read, equipment:borrow |

---

## 六、通用规则覆盖说明

| 规则 | 状态 | 说明 |
|------|------|------|
| 6.1 资产标识 | 部分实现 | Code字段唯一，可扫码操作；二维码生成待后续扩展 |
| 6.2 权限隔离 | 待实现 | 需在查询中按部门过滤 |
| 6.3 操作日志 | 待实现 | 需新建 OperationLog 实体 |
| 6.4 消息通知 | 待实现 | 需新建 Notification 实体 |
| 6.5 数据导出 | 已实现 | GET /export 返回 Excel |

---

## 七、文件清单

### 7.1 后端

| 操作 | 文件 |
|------|------|
| 修改 | `backend/LimsAuth.Api/Models/Lab.cs` (Equipment 实体) |
| 新增 | `backend/LimsAuth.Api/Models/EquipmentBorrow.cs` |
| 修改 | `backend/LimsAuth.Api/Services/EquipmentService.cs` |
| 新增 | `backend/LimsAuth.Api/Services/EquipmentBorrowService.cs` |
| 修改 | `backend/LimsAuth.Api/Controllers/EquipmentsController.cs` |
| 新增 | `backend/LimsAuth.Api/Controllers/BorrowRecordsController.cs` |
| 修改 | `backend/LimsAuth.Api/Data/AppDbContext.cs` |
| 修改 | `backend/LimsAuth.Api/Program.cs` |
| 修改 | `backend/LimsAuth.Api/LimsAuth.Api.csproj` (+EPPlus) |

### 7.2 前端

| 操作 | 文件 |
|------|------|
| 修改 | `frontend/src/api/lab.ts` |
| 修改 | `frontend/src/views/lab/EquipmentsView.vue` |
| 修改 | `frontend/src/views/lab/components/EquipmentFormDialog.vue` |
| 新增 | `frontend/src/views/lab/components/EquipmentImportDialog.vue` |
| 新增 | `frontend/src/views/lab/BorrowRecordsView.vue` |
| 新增 | `frontend/src/views/lab/EquipmentStatisticsView.vue` |
| 修改 | `frontend/src/router/index.ts` |
| 修改 | `frontend/src/views/HomeView.vue` |
| 修改 | `frontend/src/components/Breadcrumb.vue` |

---

## 八、开发记录

### 2026-05-27 设备管理模块重构完成

1. 数据库实体扩展 - Equipment 新增 Brand/Supplier/Unit/DepartmentId/ResponsibleUserId/VerificationDate/NextVerificationDate/StatusReason 等字段
2. 新增借还记录实体 - EquipmentBorrowRecord 支持完整借还流程
3. 新增分类字典 - EquipmentCategory
4. 重构 EquipmentService - 增加多维筛选、可用设备查询、统计、Excel导入导出
5. 新建 EquipmentBorrowService - 实现借还全流程（申请、导师审批、管理员审批、确认、核验、续借）
6. 前端改造 - 设备台账、借还管理、统计报表三大页面
7. 新增 Excel 批量导入功能
8. 新增权限：equipment:borrow, equipment:approve, equipment:statistics, equipment:export
