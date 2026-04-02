<template>
  <div class="periods-view">
    <el-card>
      <template #header>
        <div class="card-header">
          <span>节次时间管理</span>
          <el-button type="primary" @click="handleAdd">
            <el-icon><Plus /></el-icon>新增节次
          </el-button>
        </div>
      </template>

      <el-table :data="periods" v-loading="loading" stripe>
        <el-table-column type="index" label="序号" width="60" />
        <el-table-column prop="periodNumber" label="节次编号" width="100" />
        <el-table-column prop="name" label="节次名称" width="120" />
        <el-table-column prop="startTime" label="开始时间" width="100" />
        <el-table-column prop="endTime" label="结束时间" width="100" />
        <el-table-column prop="duration" label="时长(分钟)" width="100" />
        <el-table-column prop="description" label="说明" />
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
    <PeriodTimeFormDialog
      v-model="dialogVisible"
      :period="currentPeriod"
      @success="loadData"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { Plus } from '@element-plus/icons-vue'
import { periodTimeApi } from '../../api/teaching'
import PeriodTimeFormDialog from './components/PeriodTimeFormDialog.vue'

interface PeriodTime {
  id: string
  periodNumber: number
  name: string
  startTime: string
  endTime: string
  duration: number
  description?: string
  isActive: boolean
}

const loading = ref(false)
const periods = ref<PeriodTime[]>([])
const dialogVisible = ref(false)
const currentPeriod = ref<PeriodTime | undefined>(undefined)

const loadData = async () => {
  loading.value = true
  try {
    const res = await periodTimeApi.getList()
    if (res.data.code === 200) {
      periods.value = res.data.data
    }
  } catch (error) {
    ElMessage.error('加载节次时间列表失败')
  } finally {
    loading.value = false
  }
}

const handleAdd = () => {
  currentPeriod.value = undefined
  dialogVisible.value = true
}

const handleEdit = (row: PeriodTime) => {
  currentPeriod.value = row
  dialogVisible.value = true
}

const handleToggleStatus = async (row: PeriodTime) => {
  try {
    await ElMessageBox.confirm(
      `确定要${row.isActive ? '禁用' : '启用'}该节次吗？`,
      '提示',
      { type: 'warning' }
    )
    const res = await periodTimeApi.toggleStatus(row.id, !row.isActive)
    if (res.data.code === 200) {
      ElMessage.success('操作成功')
      loadData()
    }
  } catch (error) {
    // 取消操作
  }
}

const handleDelete = async (row: PeriodTime) => {
  try {
    await ElMessageBox.confirm(
      `确定要删除节次 "${row.name}" 吗？此操作不可恢复！`,
      '警告',
      { type: 'error' }
    )
    const res = await periodTimeApi.delete(row.id)
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
.periods-view {
  padding: 20px;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}
</style>
