<template>
  <div class="page-container">
    <div class="page-header">
      <h2>实验教学任务</h2>
      <div class="header-actions">
        <el-button @click="exportTaskList" :loading="exportingTaskList">
          <el-icon><Download /></el-icon>导出任务一览表
        </el-button>
        <el-button type="primary" @click="handleAdd">
          <el-icon><Plus /></el-icon>新增任务
        </el-button>
      </div>
    </div>

    <el-card shadow="never">
      <el-skeleton :rows="1" animated style="margin-bottom: 14px" v-if="!optionsReady" />
      <el-form v-else :inline="true" :model="searchForm" class="search-form">
        <el-form-item label="学期">
          <el-select v-model="searchForm.semesterId" placeholder="全部学期" clearable style="width: 160px">
            <el-option v-for="s in semesterOptions" :key="s.id" :label="s.name" :value="s.id" />
          </el-select>
        </el-form-item>
        <el-form-item label="专业">
          <el-select v-model="searchForm.majorId" placeholder="全部专业" clearable style="width: 160px">
            <el-option v-for="m in majorOptions" :key="m.id" :label="m.name" :value="m.id" />
          </el-select>
        </el-form-item>
        <el-form-item label="班级">
          <el-select v-model="searchForm.classId" placeholder="全部班级" clearable style="width: 160px">
            <el-option v-for="c in classOptions" :key="c.id" :label="c.name" :value="c.id" />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="loadList">搜索</el-button>
          <el-button @click="searchForm.semesterId = ''; searchForm.majorId = ''; searchForm.classId = ''; loadList()">重置</el-button>
        </el-form-item>
      </el-form>

      <el-table :data="list" v-loading="loading" stripe>
        <el-table-column prop="semester.name" label="学期" min-width="150" />
        <el-table-column prop="major.name" label="专业" min-width="140" />
        <el-table-column prop="class.name" label="班级" min-width="140" />
        <el-table-column prop="courseName" label="课程名称" min-width="160" />
        <el-table-column prop="courseType" label="课程类型" width="110" />
        <el-table-column label="独立设课" width="90">
          <template #default="{ row }"><el-tag :type="row.isIndependentCourse ? 'success' : 'info'" size="small">{{ row.isIndependentCourse ? '是' : '否' }}</el-tag></template>
        </el-table-column>
        <el-table-column label="实验/实践/实训学时" width="150">
          <template #default="{ row }">{{ row.currentSemesterExperimentHours }} / {{ row.currentSemesterPracticeHours }} / {{ row.currentSemesterTrainingHours }}</template>
        </el-table-column>
        <el-table-column prop="studentCount" label="人数" width="70" />
        <el-table-column prop="studentLevel" label="层次" width="80" />
        <el-table-column prop="status" label="状态" width="80">
          <template #default="{ row }"><el-tag size="small">{{ row.status }}</el-tag></template>
        </el-table-column>
        <el-table-column label="操作" width="180" fixed="right">
          <template #default="{ row }">
            <el-button link type="primary" @click="handleEdit(row)">编辑</el-button>
            <el-popconfirm title="确认删除该任务？" @confirm="remove(row.id)">
              <template #reference>
                <el-button link type="danger">删除</el-button>
              </template>
            </el-popconfirm>
          </template>
        </el-table-column>
      </el-table>
    </el-card>

    <ExperimentTaskFormDialog
      v-model="dialogVisible"
      :task="currentTask"
      @success="loadList"
    />
  </div>
</template>

<script setup lang="ts">
import { onMounted, reactive, ref } from 'vue'
import { ElMessage } from 'element-plus'
import { Download, Plus } from '@element-plus/icons-vue'
import { classApi, majorApi, semesterApi, type ClassDto, type MajorDto, type SemesterDto } from '@/api/teaching'
import { experimentApi, exportApi, type ExperimentTaskDto } from '@/api/experiment'
import ExperimentTaskFormDialog from './components/ExperimentTaskFormDialog.vue'

const loading = ref(false)
const exportingTaskList = ref(false)
const list = ref<ExperimentTaskDto[]>([])
const semesterOptions = ref<SemesterDto[]>([])
const majorOptions = ref<MajorDto[]>([])
const classOptions = ref<ClassDto[]>([])

const dialogVisible = ref(false)
const currentTask = ref<ExperimentTaskDto | undefined>(undefined)
const optionsReady = ref(false)

const searchForm = reactive({
  semesterId: '',
  majorId: '',
  classId: ''
})

const loadList = async () => {
  loading.value = true
  try {
    const res = await experimentApi.getTasks({
      semesterId: searchForm.semesterId || undefined,
      majorId: searchForm.majorId || undefined,
      classId: searchForm.classId || undefined
    })
    list.value = res.data.data || []
  } finally {
    loading.value = false
  }
}

const loadOptions = async () => {
  const [semRes, majorRes, classRes] = await Promise.all([
    semesterApi.getList(),
    majorApi.getAll(),
    classApi.getList({ page: 1, pageSize: 999 })
  ])
  semesterOptions.value = semRes.data.data || []
  majorOptions.value = majorRes.data.data || []
  classOptions.value = classRes.data.data || []
  optionsReady.value = true
}

const handleAdd = () => {
  currentTask.value = undefined
  dialogVisible.value = true
}

const handleEdit = (row: ExperimentTaskDto) => {
  currentTask.value = row
  dialogVisible.value = true
}

const remove = async (id: string) => {
  await experimentApi.deleteTask(id)
  ElMessage.success('删除成功')
  loadList()
}

const exportTaskList = async () => {
  exportingTaskList.value = true
  try {
    const res = await exportApi.exportTaskList({
      semesterId: searchForm.semesterId || undefined,
      majorId: searchForm.majorId || undefined,
      classId: searchForm.classId || undefined
    })
    const blob = new Blob([res.data], { type: 'application/vnd.openxmlformats-officedocument.wordprocessingml.document' })
    const url = URL.createObjectURL(blob)
    const a = document.createElement('a')
    a.href = url
    a.download = `实验课程教学任务一览表_${new Date().toISOString().slice(0, 10).replace(/-/g, '')}.docx`
    a.click()
    URL.revokeObjectURL(url)
    ElMessage.success('导出成功')
  } catch {
    ElMessage.error('导出失败')
  } finally {
    exportingTaskList.value = false
  }
}

onMounted(async () => {
  await Promise.all([loadList(), loadOptions()])
})
</script>

<style scoped>
.page-container { padding: 20px; }
.page-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 16px; }
.page-header h2 { margin: 0; font-size: 20px; font-weight: 500; }
.header-actions { display: flex; gap: 10px; }
.search-form { margin-bottom: 14px; }
</style>
