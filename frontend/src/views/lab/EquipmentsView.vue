<template>
  <div class="equipments-container">
    <div class="page-header">
      <div>
        <h2>设备管理</h2>
        <el-breadcrumb v-if="route.query.labName" class="breadcrumb">
          <el-breadcrumb-item :to="{ path: '/lab/equipments' }">设备台账</el-breadcrumb-item>
          <el-breadcrumb-item>{{ route.query.labName }}</el-breadcrumb-item>
        </el-breadcrumb>
      </div>
      <div class="header-actions">
        <el-button type="success" @click="handleExport" :loading="exporting">
          <el-icon><Download /></el-icon>
          导出 Excel
        </el-button>
        <el-button type="primary" @click="handleCreate" v-permission="'equipment:create'">
          <el-icon><Plus /></el-icon>
          新增设备
        </el-button>
      </div>
    </div>

    <!-- 统计卡片 -->
    <div class="stats-grid" v-loading="statsLoading">
      <div class="stat-card">
        <div class="stat-value">{{ stats.total }}</div>
        <div class="stat-label">设备总数</div>
      </div>
      <div class="stat-card stat-normal">
        <div class="stat-value">{{ stats.normalCount }}</div>
        <div class="stat-label">在库-可用</div>
      </div>
      <div class="stat-card stat-borrowed">
        <div class="stat-value">{{ stats.borrowedCount }}</div>
        <div class="stat-label">借出</div>
      </div>
      <div class="stat-card stat-maintenance">
        <div class="stat-value">{{ stats.maintenanceCount }}</div>
        <div class="stat-label">维修中</div>
      </div>
      <div class="stat-card stat-scrapped">
        <div class="stat-value">{{ stats.scrappedCount }}</div>
        <div class="stat-label">已报废</div>
      </div>
      <div class="stat-card stat-value">
        <div class="stat-value">¥{{ formatPrice(stats.totalValue) }}</div>
        <div class="stat-label">设备总价值</div>
      </div>
    </div>

    <!-- 搜索栏 -->
    <el-card class="search-card" shadow="never">
      <el-form :model="queryForm" inline>
        <el-form-item label="关键词">
          <el-input v-model="queryForm.keyword" placeholder="设备代码/名称/型号" clearable @keyup.enter="handleSearch" />
        </el-form-item>
        <el-form-item label="所属实验室">
          <el-select v-model="queryForm.labId" placeholder="全部" clearable style="width: 180px">
            <el-option v-for="lab in labs" :key="lab.id" :label="lab.name" :value="lab.id" />
          </el-select>
        </el-form-item>
        <el-form-item label="设备分类">
          <el-select v-model="queryForm.category" placeholder="全部" clearable style="width: 150px">
            <el-option v-for="cat in EQUIPMENT_CATEGORIES" :key="cat" :label="cat" :value="cat" />
          </el-select>
        </el-form-item>
        <el-form-item label="状态">
          <el-select v-model="queryForm.status" placeholder="全部" clearable style="width: 120px">
            <el-option v-for="status in EQUIPMENT_STATUSES" :key="status" :label="status" :value="status" />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">
            <el-icon><Search /></el-icon>
            搜索
          </el-button>
          <el-button @click="handleReset">重置</el-button>
        </el-form-item>
      </el-form>
    </el-card>

    <!-- 设备列表 -->
    <el-card shadow="never">
      <template #header>
        <div class="table-header">
          <span>设备列表（共 {{ total }} 台）</span>
        </div>
      </template>
      <el-table v-loading="loading" :data="equipmentList" stripe>
        <el-table-column prop="code" label="设备代码" width="120" />
        <el-table-column prop="name" label="设备名称" min-width="180" />
        <el-table-column prop="model" label="型号" width="120" />
        <el-table-column prop="category" label="分类" width="120" />
        <el-table-column prop="labName" label="所属实验室" width="150">
          <template #default="{ row }">
            <span v-if="row.labName">{{ row.labName }}</span>
            <span v-else class="text-gray">未分配</span>
          </template>
        </el-table-column>
        <el-table-column prop="status" label="状态" width="100">
          <template #default="{ row }">
            <el-tag :type="getStatusType(row.status)">{{ row.status }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="requiresBooking" label="需预约" width="80">
          <template #default="{ row }">
            <el-tag v-if="row.requiresBooking" type="warning" size="small">是</el-tag>
            <span v-else class="text-gray">否</span>
          </template>
        </el-table-column>
        <el-table-column prop="price" label="价格(元)" width="100" align="right">
          <template #default="{ row }">
            <span v-if="row.price">{{ row.price.toFixed(2) }}</span>
            <span v-else class="text-gray">-</span>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="240" fixed="right">
          <template #default="{ row }">
            <el-button link type="primary" @click="handleEdit(row)" v-permission="'equipment:update'">编辑</el-button>
            <el-button link type="primary" @click="handleUpdateStatus(row)">状态</el-button>
            <el-button v-if="row.status === '在库-可用'" link type="success" @click="handleOpenBorrow(row)">借出</el-button>
            <el-popconfirm title="确定删除该设备吗？" @confirm="handleDelete(row)" v-permission="'equipment:delete'">
              <template #reference>
                <el-button link type="danger">删除</el-button>
              </template>
            </el-popconfirm>
          </template>
        </el-table-column>
      </el-table>

      <!-- 分页 -->
      <div class="pagination-wrapper">
        <el-pagination
          v-model:current-page="pagination.page"
          v-model:page-size="pagination.pageSize"
          :page-sizes="[10, 20, 50, 100]"
          :total="total"
          layout="total, sizes, prev, pager, next, jumper"
          @size-change="handleSizeChange"
          @current-change="handlePageChange"
        />
      </div>
    </el-card>

    <EquipmentFormDialog v-model="dialogVisible" :type="dialogType" :equipment-data="currentEquipment" :labs="labs" @success="handleSearch" />

    <!-- 状态更新对话框 -->
    <el-dialog title="更新设备状态" v-model="statusDialogVisible" width="400px">
      <el-form label-width="100px">
        <el-form-item label="设备状态">
          <el-select v-model="newStatus" placeholder="选择状态" style="width: 100%">
            <el-option v-for="status in EQUIPMENT_STATUSES" :key="status" :label="status" :value="status" />
          </el-select>
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="statusDialogVisible = false">取消</el-button>
        <el-button type="primary" @click="confirmUpdateStatus">确定</el-button>
      </template>
    </el-dialog>

    <!-- 借出申请对话框 -->
    <el-dialog title="设备借出申请" v-model="borrowDialogVisible" width="550px" destroy-on-close>
      <el-form ref="borrowFormRef" :model="borrowForm" :rules="borrowRules" label-width="100px">
        <el-descriptions :column="2" border size="small" style="margin-bottom: 16px">
          <el-descriptions-item label="设备名称" :span="2">{{ borrowForm.equipmentName }}</el-descriptions-item>
          <el-descriptions-item label="设备编号">{{ borrowForm.equipmentCode }}</el-descriptions-item>
          <el-descriptions-item label="所属实验室">{{ borrowForm.labName || '未分配' }}</el-descriptions-item>
        </el-descriptions>
        <el-form-item label="联系电话" prop="phone">
          <el-input v-model="borrowForm.phone" placeholder="请输入联系电话" maxlength="50" />
        </el-form-item>
        <el-form-item label="借用时间" prop="borrowDate" required>
          <el-date-picker v-model="borrowForm.borrowDate" type="date" value-format="YYYY-MM-DD" placeholder="选择借用日期" style="width: 100%" />
        </el-form-item>
        <el-form-item label="计划归还" prop="expectedReturnDate" required>
          <el-date-picker v-model="borrowForm.expectedReturnDate" type="date" value-format="YYYY-MM-DD" placeholder="选择计划归还日期" style="width: 100%" />
        </el-form-item>
        <el-form-item label="使用地点" prop="usageLocation">
          <el-input v-model="borrowForm.usageLocation" placeholder="请输入使用地点" maxlength="200" />
        </el-form-item>
        <el-form-item label="借用用途" prop="purpose" required>
          <el-input v-model="borrowForm.purpose" type="textarea" :rows="3" placeholder="请输入借用用途" maxlength="500" show-word-limit />
        </el-form-item>
        <el-form-item label="备注">
          <el-input v-model="borrowForm.remarks" type="textarea" :rows="2" placeholder="备注信息（可选）" maxlength="500" />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="borrowDialogVisible = false">取消</el-button>
        <el-button type="primary" :loading="borrowLoading" @click="submitBorrow">提交申请</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import { ElMessage } from 'element-plus'
import { Plus, Search, Download } from '@element-plus/icons-vue'
import { equipmentApi, labApi, borrowApi, type EquipmentDto, type LabDto, type EquipmentStatisticsDto, EQUIPMENT_CATEGORIES, EQUIPMENT_STATUSES, type CreateBorrowRequest } from '@/api/lab'
import EquipmentFormDialog from './components/EquipmentFormDialog.vue'

const route = useRoute()

const queryForm = reactive({
  keyword: '',
  labId: route.query.labId as string || '',
  category: '',
  status: ''
})

const pagination = reactive({
  page: 1,
  pageSize: 20
})

const loading = ref(false)
const statsLoading = ref(false)
const exporting = ref(false)
const equipmentList = ref<EquipmentDto[]>([])
const labs = ref<LabDto[]>([])
const total = ref(0)

const stats = reactive<EquipmentStatisticsDto>({
  total: 0, activeCount: 0, inactiveCount: 0,
  normalCount: 0, maintenanceCount: 0, borrowedCount: 0, scrappedCount: 0,
  requiresBookingCount: 0, totalValue: 0,
  byCategory: {}, byStatus: {}, byLab: {}
})

const dialogVisible = ref(false)
const dialogType = ref<'create' | 'edit'>('create')
const currentEquipment = ref<EquipmentDto | null>(null)

const statusDialogVisible = ref(false)
const newStatus = ref('')
const statusEquipmentId = ref('')

const borrowDialogVisible = ref(false)
const borrowLoading = ref(false)
const borrowFormRef = ref()
const borrowForm = reactive<CreateBorrowRequest & { equipmentName: string; equipmentCode: string; labName: string }>({
  equipmentId: '',
  equipmentName: '',
  equipmentCode: '',
  labName: '',
  borrowDate: '',
  expectedReturnDate: '',
  purpose: '',
  phone: '',
  usageLocation: '',
  remarks: ''
})

const validateReturnDate = (_rule: any, value: string, callback: any) => {
  if (!borrowForm.borrowDate) {
    callback(new Error('请先选择借用日期'))
  } else if (value && new Date(value) <= new Date(borrowForm.borrowDate)) {
    callback(new Error('计划归还日期必须晚于借用日期'))
  } else {
    callback()
  }
}

const borrowRules: Record<string, any[]> = {
  phone: [{ required: true, message: '请输入联系电话', trigger: 'blur' }],
  borrowDate: [{ required: true, message: '请选择借用日期', trigger: 'change' }],
  expectedReturnDate: [
    { required: true, message: '请选择计划归还日期', trigger: 'change' },
    { validator: validateReturnDate, trigger: 'change' }
  ],
  usageLocation: [{ required: true, message: '请输入使用地点', trigger: 'blur' }],
  purpose: [{ required: true, message: '请输入借用用途', trigger: 'blur' }]
}

const fetchEquipments = async () => {
  loading.value = true
  try {
    const res = await equipmentApi.getList({
      keyword: queryForm.keyword || undefined,
      labId: queryForm.labId || undefined,
      category: queryForm.category || undefined,
      status: queryForm.status || undefined,
      page: pagination.page,
      pageSize: pagination.pageSize
    })
    if (res.data.code === 200) {
      equipmentList.value = res.data.data
      total.value = res.data.total ?? res.data.data.length
    }
  } catch (error) {
    ElMessage.error('获取设备列表失败')
  } finally {
    loading.value = false
  }
}

const fetchStats = async () => {
  statsLoading.value = true
  try {
    const res = await equipmentApi.getStatistics()
    if (res.data.code === 200) {
      Object.assign(stats, res.data.data)
    }
  } catch (error) {
    console.error('获取统计数据失败', error)
  } finally {
    statsLoading.value = false
  }
}

const fetchLabs = async () => {
  try {
    const res = await labApi.getList()
    if (res.data.code === 200) {
      labs.value = res.data.data
    }
  } catch (error) {
    console.error('获取实验室列表失败', error)
  }
}

const handleSearch = () => {
  pagination.page = 1
  fetchEquipments()
}

const handleReset = () => {
  queryForm.keyword = ''
  queryForm.labId = ''
  queryForm.category = ''
  queryForm.status = ''
  pagination.page = 1
  fetchEquipments()
}

const handleSizeChange = () => {
  pagination.page = 1
  fetchEquipments()
}

const handlePageChange = () => {
  fetchEquipments()
}

const handleCreate = () => {
  dialogType.value = 'create'
  currentEquipment.value = null
  dialogVisible.value = true
}

const handleEdit = (row: EquipmentDto) => {
  dialogType.value = 'edit'
  currentEquipment.value = row
  dialogVisible.value = true
}

const handleDelete = async (row: EquipmentDto) => {
  try {
    const res = await equipmentApi.delete(row.id)
    if (res.data.code === 200) {
      ElMessage.success('删除成功')
      fetchEquipments()
      fetchStats()
    } else {
      ElMessage.error(res.data.message)
    }
  } catch (error) {
    ElMessage.error('删除失败')
  }
}

const handleUpdateStatus = (row: EquipmentDto) => {
  statusEquipmentId.value = row.id
  newStatus.value = row.status
  statusDialogVisible.value = true
}

const confirmUpdateStatus = async () => {
  try {
    const res = await equipmentApi.updateEquipmentStatus(statusEquipmentId.value, newStatus.value)
    if (res.data.code === 200) {
      ElMessage.success('状态更新成功')
      statusDialogVisible.value = false
      fetchEquipments()
      fetchStats()
    } else {
      ElMessage.error(res.data.message)
    }
  } catch (error) {
    ElMessage.error('状态更新失败')
  }
}

const handleOpenBorrow = (row: EquipmentDto) => {
  borrowForm.equipmentId = row.id
  borrowForm.equipmentName = row.name
  borrowForm.equipmentCode = row.code
  borrowForm.labName = row.labName || ''
  borrowForm.borrowDate = ''
  borrowForm.expectedReturnDate = ''
  borrowForm.purpose = ''
  borrowForm.phone = ''
  borrowForm.usageLocation = ''
  borrowForm.remarks = ''
  borrowDialogVisible.value = true
}

const submitBorrow = async () => {
  const valid = await borrowFormRef.value?.validate().catch(() => false)
  if (!valid) return

  borrowLoading.value = true
  try {
    const res = await borrowApi.createRequest({
      equipmentId: borrowForm.equipmentId,
      borrowDate: borrowForm.borrowDate,
      expectedReturnDate: borrowForm.expectedReturnDate,
      purpose: borrowForm.purpose,
      phone: borrowForm.phone,
      usageLocation: borrowForm.usageLocation,
      remarks: borrowForm.remarks
    })
    if (res.data.code === 200) {
      ElMessage.success('借出申请已提交')
      borrowDialogVisible.value = false
      fetchEquipments()
      fetchStats()
    } else {
      ElMessage.error(res.data.message)
    }
  } catch (err: any) {
    ElMessage.error(err?.response?.data?.message || '提交失败')
  } finally {
    borrowLoading.value = false
  }
}

const handleExport = async () => {
  exporting.value = true
  try {
    const res = await equipmentApi.exportExcel({
      keyword: queryForm.keyword || undefined,
      category: queryForm.category || undefined,
      status: queryForm.status || undefined
    })
    const blob = new Blob([res.data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' })
    const url = URL.createObjectURL(blob)
    const link = document.createElement('a')
    link.href = url
    link.download = `设备台账_${new Date().toISOString().slice(0, 10)}.xlsx`
    link.click()
    URL.revokeObjectURL(url)
    ElMessage.success('导出成功')
  } catch (error) {
    ElMessage.error('导出失败')
  } finally {
    exporting.value = false
  }
}

const formatPrice = (value: number) => {
  if (value >= 10000) return (value / 10000).toFixed(1) + '万'
  return value.toFixed(0)
}

const getStatusType = (status: string) => {
  const typeMap: Record<string, string> = {
    '在库-可用': 'success', '在库-待维修': 'warning', '在库-已预约': 'info',
    '借出': 'primary', '送修': 'danger', '报废': 'info', '丢失': 'danger'
  }
  return typeMap[status] || ''
}

onMounted(() => {
  fetchLabs()
  fetchEquipments()
  fetchStats()
})
</script>

<style scoped>
.equipments-container {
  padding: 20px;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 16px;
}

.page-header h2 {
  margin: 0;
  font-size: 20px;
  font-weight: 500;
}

.header-actions {
  display: flex;
  gap: 8px;
}

.breadcrumb {
  margin-top: 8px;
}

/* 统计卡片 */
.stats-grid {
  display: grid;
  grid-template-columns: repeat(6, 1fr);
  gap: 12px;
  margin-bottom: 16px;
}

.stat-card {
  background: #fff;
  border-radius: 8px;
  padding: 16px;
  text-align: center;
  border: 1px solid #ebeef5;
}

.stat-value {
  font-size: 24px;
  font-weight: 600;
  color: #303133;
  line-height: 1.2;
}

.stat-label {
  font-size: 12px;
  color: #909399;
  margin-top: 4px;
}

.stat-normal .stat-value { color: #67c23a; }
.stat-borrowed .stat-value { color: #409eff; }
.stat-maintenance .stat-value { color: #e6a23c; }
.stat-scrapped .stat-value { color: #f56c6c; }
.stat-value .stat-value { color: #303133; }

.search-card {
  margin-bottom: 16px;
}

.table-header {
  font-size: 14px;
  color: #606266;
}

.pagination-wrapper {
  margin-top: 16px;
  display: flex;
  justify-content: flex-end;
}

.text-gray {
  color: #909399;
}
</style>
