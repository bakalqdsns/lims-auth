<template>
  <div class="majors-view">
    <el-card>
      <template #header>
        <div class="card-header">
          <span>专业管理</span>
          <el-button type="primary" @click="handleAdd">
            <el-icon><Plus /></el-icon>新增专业
          </el-button>
        </div>
      </template>

      <el-table :data="majors" v-loading="loading" stripe>
        <el-table-column type="index" label="序号" width="60" />
        <el-table-column prop="code" label="专业代码" width="120" />
        <el-table-column prop="name" label="专业名称" />
        <el-table-column prop="description" label="描述" show-overflow-tooltip />
        <el-table-column prop="isActive" label="状态" width="80">
          <template #default="{ row }">
            <el-tag :type="row.isActive ? 'success' : 'info'">
              {{ row.isActive ? '启用' : '禁用' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="180" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" link @click="handleEdit(row)">编辑</el-button>
            <el-button type="primary" link @click="handleToggleStatus(row)">
              {{ row.isActive ? '禁用' : '启用' }}
            </el-button>
            <el-button type="danger" link @click="handleDelete(row)">删除</el-button>
          </template>
        </el-table-column>
      </el-table>
    </el-card>

    <!-- 表单对话框 -->
    <MajorFormDialog
      v-model="dialogVisible"
      :major="currentMajor"
      @success="loadData"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { Plus } from '@element-plus/icons-vue'
import { majorApi } from '../../api/teaching'
import MajorFormDialog from './components/MajorFormDialog.vue'

interface Major {
  id: string
  code: string
  name: string
  englishName?: string
  departmentId: string
  departmentName?: string
  duration: number
  degreeType: string
  description?: string
  isActive: boolean
}

const loading = ref(false)
const majors = ref<Major[]>([])
const dialogVisible = ref(false)
const currentMajor = ref<Major | undefined>(undefined)

const loadData = async () => {
  loading.value = true
  try {
    const res = await majorApi.getList()
    if (res.data.code === 200) {
      majors.value = res.data.data
    }
  } catch (error) {
    ElMessage.error('加载专业列表失败')
  } finally {
    loading.value = false
  }
}

const handleAdd = () => {
  currentMajor.value = undefined
  dialogVisible.value = true
}

const handleEdit = (row: Major) => {
  currentMajor.value = row
  dialogVisible.value = true
}

const handleToggleStatus = async (row: Major) => {
  try {
    await ElMessageBox.confirm(
      `确定要${row.isActive ? '禁用' : '启用'}专业 "${row.name}" 吗？`,
      '提示',
      { type: 'warning' }
    )
    const res = await majorApi.toggleStatus(row.id, !row.isActive)
    if (res.data.code === 200) {
      ElMessage.success('操作成功')
      loadData()
    }
  } catch (error) {
    // 取消操作
  }
}

const handleDelete = async (row: Major) => {
  try {
    await ElMessageBox.confirm(
      `确定要删除专业 "${row.name}" 吗？此操作不可恢复！`,
      '警告',
      { type: 'error' }
    )
    const res = await majorApi.delete(row.id)
    if (res.data.code === 200) {
      ElMessage.success('删除成功')
      loadData()
    }
  } catch (error) {
    // 取消操作
  }
}

onMounted(loadData)
</script>

<style scoped>
.majors-view {
  padding: 20px;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}
</style>
