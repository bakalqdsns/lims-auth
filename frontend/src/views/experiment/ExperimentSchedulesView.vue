<template>
  <div class="page-container">
    <div class="page-header">
      <h2>实验项目开出</h2>
      <div class="header-actions">
        <el-button @click="exportSchedulePlan" :loading="exportingSchedule">
          <el-icon><Download /></el-icon>导出授课计划表
        </el-button>
        <el-button type="primary" @click="handleAdd">
          <el-icon><Plus /></el-icon>新增开出记录
        </el-button>
      </div>
    </div>
    <el-card shadow="never">
      <el-skeleton :rows="1" animated style="margin-bottom: 14px" v-if="!optionsReady" />
      <el-form v-else :inline="true" :model="searchForm" class="search-form">
        <el-form-item label="学期">
          <el-select v-model="searchForm.semesterId" placeholder="全部学期" clearable style="width: 160px" @change="loadList">
            <el-option v-for="s in semesterOptions" :key="s.id" :label="s.name" :value="s.id" />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="loadList">搜索</el-button>
          <el-button @click="searchForm.semesterId = ''; loadList()">重置</el-button>
        </el-form-item>
      </el-form>

      <el-table :data="list" v-loading="loading" stripe>
        <el-table-column prop="experimentTask.courseName" label="教学任务" min-width="180" />
        <el-table-column prop="experimentItem.experimentName" label="实验项目" min-width="160" />
        <el-table-column prop="weekNumber" label="周次" width="70" />
        <el-table-column prop="dayOfWeek" label="星期" width="70" />
        <el-table-column prop="periodNumber" label="节次" width="70" />
        <el-table-column label="地点" min-width="180">
          <template #default="{ row }">{{ formatLocation(row) }}</template>
        </el-table-column>
        <el-table-column label="是否开出" width="90">
          <template #default="{ row }"><el-tag :type="row.isConducted ? 'success' : 'warning'">{{ row.isConducted ? '是' : '否' }}</el-tag></template>
        </el-table-column>
        <el-table-column label="操作" width="180" fixed="right">
          <template #default="{ row }">
            <el-button link type="primary" @click="handleEdit(row)">编辑</el-button>
            <el-popconfirm title="确认删除该记录？" @confirm="remove(row.id)">
              <template #reference><el-button link type="danger">删除</el-button></template>
            </el-popconfirm>
          </template>
        </el-table-column>
      </el-table>
    </el-card>

    <ExperimentScheduleFormDialog
      v-model="dialogVisible"
      :schedule="currentSchedule"
      @success="loadList"
    />
  </div>
</template>

<script setup lang="ts">
import { onMounted, reactive, ref } from 'vue'
import { ElMessage } from 'element-plus'
import { Download, Plus } from '@element-plus/icons-vue'
import { semesterApi, type SemesterDto } from '@/api/teaching'
import { experimentApi, exportApi, type ExperimentScheduleDto } from '@/api/experiment'
import ExperimentScheduleFormDialog from './components/ExperimentScheduleFormDialog.vue'

const loading = ref(false)
const exportingSchedule = ref(false)
const list = ref<ExperimentScheduleDto[]>([])
const semesterOptions = ref<SemesterDto[]>([])
const dialogVisible = ref(false)
const currentSchedule = ref<ExperimentScheduleDto | undefined>(undefined)
const optionsReady = ref(false)

const searchForm = reactive({ semesterId: '' })

const loadList = async () => {
  loading.value = true
  try {
    const res = await experimentApi.getSchedules()
    list.value = res.data.data || []
  } finally {
    loading.value = false
  }
}

const loadSemesters = async () => {
  const res = await semesterApi.getList()
  semesterOptions.value = res.data.data || []
  optionsReady.value = true
}

const handleAdd = () => {
  currentSchedule.value = undefined
  dialogVisible.value = true
}

const handleEdit = (row: ExperimentScheduleDto) => {
  currentSchedule.value = row
  dialogVisible.value = true
}

const remove = async (id: string) => {
  await experimentApi.deleteSchedule(id)
  ElMessage.success('删除成功')
  loadList()
}

const formatLocation = (row: ExperimentScheduleDto) => {
  if (row.lab?.building?.name || row.lab?.roomNumber || row.lab?.name) {
    return [row.lab?.building?.name, row.lab?.roomNumber, row.lab?.name].filter(Boolean).join(' / ')
  }
  return row.location || '-'
}

const exportSchedulePlan = async () => {
  exportingSchedule.value = true
  try {
    const res = await exportApi.exportSchedulePlan({ semesterId: searchForm.semesterId || undefined })
    const blob = new Blob([res.data], { type: 'application/vnd.openxmlformats-officedocument.wordprocessingml.document' })
    const url = URL.createObjectURL(blob)
    const a = document.createElement('a')
    a.href = url
    a.download = `实验教学授课计划表_${new Date().toISOString().slice(0, 10).replace(/-/g, '')}.docx`
    a.click()
    URL.revokeObjectURL(url)
    ElMessage.success('导出成功')
  } catch {
    ElMessage.error('导出失败')
  } finally {
    exportingSchedule.value = false
  }
}

onMounted(async () => {
  await Promise.all([loadList(), loadSemesters()])
})
</script>

<style scoped>
.page-container { padding: 20px; }
.page-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 16px; }
.page-header h2 { margin: 0; font-size: 20px; font-weight: 500; }
.header-actions { display: flex; gap: 10px; }
.search-form { margin-bottom: 14px; }
</style>
