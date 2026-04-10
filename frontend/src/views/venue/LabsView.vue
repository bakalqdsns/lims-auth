<template>
  <div class="labs-container">
    <div class="page-header">
      <h2>实验室管理</h2>
      <el-button type="primary" @click="handleCreate" v-permission="'lab:create'">
        <el-icon><Plus /></el-icon>
        新增实验室
      </el-button>
    </div>

    <!-- 搜索栏 -->
    <el-card class="search-card" shadow="never">
      <el-form :model="queryForm" inline>
        <el-form-item label="关键词">
          <el-input v-model="queryForm.keyword" placeholder="实验室代码/名称" clearable @keyup.enter="handleSearch" />
        </el-form-item>
        <el-form-item label="实验室类型">
          <el-select v-model="queryForm.labType" placeholder="全部" clearable style="width: 150px">
            <el-option v-for="type in LAB_TYPES" :key="type" :label="type" :value="type" />
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

    <!-- 实验室列表 -->
    <el-card shadow="never">
      <el-table v-loading="loading" :data="labList" stripe>
        <el-table-column prop="code" label="实验室代码" width="120" />
        <el-table-column prop="name" label="实验室名称" min-width="180" />
        <el-table-column prop="labType" label="类型" width="120" />
        <el-table-column prop="location" label="地点" width="150" />
        <el-table-column prop="capacity" label="容纳人数" width="100" />
        <el-table-column prop="safetyLevel" label="安全等级" width="100">
          <template #default="{ row }">
            <el-tag :type="getSafetyLevelType(row.safetyLevel)">{{ row.safetyLevel }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="managerName" label="负责人" width="120">
          <template #default="{ row }">
            <span v-if="row.managerName">{{ row.managerName }}</span>
            <span v-else class="text-gray">-</span>
          </template>
        </el-table-column>
        <el-table-column prop="equipmentCount" label="设备数" width="80" />
        <el-table-column label="状态" width="100">
          <template #default="{ row }">
            <el-switch 
              v-model="row.isActive" 
              @change="(val: boolean) => handleStatusChange(row, val)"
              v-permission="'lab:update'"
            />
          </template>
        </el-table-column>
        <el-table-column label="操作" width="200" fixed="right">
          <template #default="{ row }">
            <el-button link type="primary" @click="handleEdit(row)" v-permission="'lab:update'">编辑</el-button>
            <el-button link type="primary" @click="handleViewEquipments(row)">查看设备</el-button>
            <el-popconfirm title="确定删除该实验室吗？" @confirm="handleDelete(row)" v-permission="'lab:delete'">
              <template #reference>
                <el-button link type="danger">删除</el-button>
              </template>
            </el-popconfirm>
          </template>
        </el-table-column>
      </el-table>
    </el-card>

    <LabFormDialog v-model="dialogVisible" :type="dialogType" :lab-data="currentLab" @success="handleSearch" />
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessage } from 'element-plus'
import { Plus, Search } from '@element-plus/icons-vue'
import { labApi, type LabDto, LAB_TYPES } from '../../api/lab'
import LabFormDialog from './components/LabFormDialog.vue'

const router = useRouter()

const queryForm = reactive({
  keyword: '',
  labType: ''
})

const loading = ref(false)
const labList = ref<LabDto[]>([])

const dialogVisible = ref(false)
const dialogType = ref<'create' | 'edit'>('create')
const currentLab = ref<LabDto | null>(null)

const fetchLabs = async () => {
  loading.value = true
  try {
    const res = await labApi.getList({
      keyword: queryForm.keyword || undefined,
      labType: queryForm.labType || undefined
    })
    if (res.data.code === 200) {
      labList.value = res.data.data
    }
  } catch (error) {
    ElMessage.error('获取实验室列表失败')
  } finally {
    loading.value = false
  }
}

const handleSearch = () => {
  fetchLabs()
}

const handleReset = () => {
  queryForm.keyword = ''
  queryForm.labType = ''
  fetchLabs()
}

const handleCreate = () => {
  dialogType.value = 'create'
  currentLab.value = null
  dialogVisible.value = true
}

const handleEdit = (row: LabDto) => {
  dialogType.value = 'edit'
  currentLab.value = row
  dialogVisible.value = true
}

const handleDelete = async (row: LabDto) => {
  try {
    const res = await labApi.delete(row.id)
    if (res.data.code === 200) {
      ElMessage.success('删除成功')
      fetchLabs()
    } else {
      ElMessage.error(res.data.message)
    }
  } catch (error) {
    ElMessage.error('删除失败')
  }
}

const handleStatusChange = async (row: LabDto, val: boolean) => {
  try {
    const res = await labApi.toggleStatus(row.id, val)
    if (res.data.code === 200) {
      ElMessage.success(val ? '实验室已启用' : '实验室已禁用')
    } else {
      row.isActive = !val
      ElMessage.error(res.data.message)
    }
  } catch (error) {
    row.isActive = !val
    ElMessage.error('操作失败')
  }
}

const handleViewEquipments = (row: LabDto) => {
  router.push({
    path: '/venue/equipments',
    query: { labId: row.id, labName: row.name }
  })
}

const getSafetyLevelType = (level: string) => {
  const typeMap: Record<string, string> = {
    '一般': 'success',
    '中等': 'warning',
    '高危': 'danger'
  }
  return typeMap[level] || ''
}

onMounted(() => {
  fetchLabs()
})
</script>

<style scoped>
.labs-container {
  padding: 20px;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
}

.page-header h2 {
  margin: 0;
  font-size: 20px;
  font-weight: 500;
}

.search-card {
  margin-bottom: 20px;
}

.text-gray {
  color: #909399;
}
</style>
