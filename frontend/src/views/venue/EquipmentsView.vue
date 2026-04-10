<template>
  <div class="equipments-container">
    <div class="page-header">
      <div>
        <h2>设备管理</h2>
        <el-breadcrumb v-if="route.query.labName" class="breadcrumb">
          <el-breadcrumb-item :to="{ path: '/venue/labs' }">实验室管理</el-breadcrumb-item>
          <el-breadcrumb-item>{{ route.query.labName }}</el-breadcrumb-item>
        </el-breadcrumb>
      </div>
      <el-button type="primary" @click="handleCreate" v-permission="'equipment:create'">
        <el-icon><Plus /></el-icon>
        新增设备
      </el-button>
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
        <el-table-column label="操作" width="200" fixed="right">
          <template #default="{ row }">
            <el-button link type="primary" @click="handleEdit(row)" v-permission="'equipment:update'">编辑</el-button>
            <el-button link type="primary" @click="handleUpdateStatus(row)">更新状态</el-button>
            <el-popconfirm title="确定删除该设备吗？" @confirm="handleDelete(row)" v-permission="'equipment:delete'">
              <template #reference>
                <el-button link type="danger">删除</el-button>
              </template>
            </el-popconfirm>
          </template>
        </el-table-column>
      </el-table>
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
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import { ElMessage } from 'element-plus'
import { Plus, Search } from '@element-plus/icons-vue'
import { equipmentApi, labApi, type EquipmentDto, type LabDto, EQUIPMENT_CATEGORIES, EQUIPMENT_STATUSES } from '../../api/lab'
import EquipmentFormDialog from './components/EquipmentFormDialog.vue'

const route = useRoute()

const queryForm = reactive({
  keyword: '',
  labId: route.query.labId as string || '',
  category: '',
  status: ''
})

const loading = ref(false)
const equipmentList = ref<EquipmentDto[]>([])
const labs = ref<LabDto[]>([])

const dialogVisible = ref(false)
const dialogType = ref<'create' | 'edit'>('create')
const currentEquipment = ref<EquipmentDto | null>(null)

const statusDialogVisible = ref(false)
const newStatus = ref('')
const statusEquipmentId = ref('')

const fetchEquipments = async () => {
  loading.value = true
  try {
    const res = await equipmentApi.getList({
      keyword: queryForm.keyword || undefined,
      labId: queryForm.labId || undefined,
      category: queryForm.category || undefined,
      status: queryForm.status || undefined
    })
    if (res.data.code === 200) {
      equipmentList.value = res.data.data
    }
  } catch (error) {
    ElMessage.error('获取设备列表失败')
  } finally {
    loading.value = false
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
  fetchEquipments()
}

const handleReset = () => {
  queryForm.keyword = ''
  queryForm.labId = ''
  queryForm.category = ''
  queryForm.status = ''
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
    } else {
      ElMessage.error(res.data.message)
    }
  } catch (error) {
    ElMessage.error('状态更新失败')
  }
}

const getStatusType = (status: string) => {
  const typeMap: Record<string, string> = {
    '正常': 'success',
    '维修中': 'warning',
    '报废': 'danger',
    '借用中': 'info',
    '闲置': ''
  }
  return typeMap[status] || ''
}

onMounted(() => {
  fetchLabs()
  fetchEquipments()
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
  margin-bottom: 20px;
}

.page-header h2 {
  margin: 0;
  font-size: 20px;
  font-weight: 500;
}

.breadcrumb {
  margin-top: 8px;
}

.search-card {
  margin-bottom: 20px;
}

.text-gray {
  color: #909399;
}
</style>
