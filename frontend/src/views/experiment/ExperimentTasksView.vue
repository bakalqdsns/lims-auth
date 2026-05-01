<template>
  <div class="page-container">
    <div class="page-header">
      <h2>实验教学任务</h2>
      <div class="header-actions">
        <el-button @click="openExportDialog" :loading="exportingTaskList">
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

    <!-- 表格预览弹窗：实验课程教学任务一览表 -->
    <el-dialog v-model="exportDialogVisible" title="实验课程教学任务一览表" width="1100px" top="20px">
      <div class="table-wrap">
        <h3 class="table-title">实验课程教学任务一览表</h3>
        <table border="1" cellpadding="6" cellspacing="0" width="100%" style="border-collapse:collapse; font-size:13px;">
          <thead>
          <tr style="background:#f0f0f0; text-align:center;">
            <td rowspan="2" style="min-width:40px">序号</td>
            <td rowspan="2" style="min-width:120px">专业、班级</td>
            <td rowspan="2" style="min-width:60px">学生人数</td>
            <td rowspan="2" style="min-width:180px">实验课程名称</td>
            <td rowspan="2" style="min-width:70px">实验总学时</td>
            <td rowspan="2" style="min-width:80px">本学期实验学时</td>
            <td rowspan="2" style="min-width:120px">课程承担部门</td>
            <td colspan="2" style="text-align:center">实验指导教师</td>
          </tr>
          <tr style="background:#f0f0f0; text-align:center;">
            <td style="min-width:80px">姓名</td>
            <td style="min-width:80px">职称</td>
          </tr>
          </thead>
          <tbody>
          <tr v-for="(item, idx) in exportTableData" :key="idx">
            <td style="text-align:center">{{ idx + 1 }}</td>
            <td><el-input v-model="item.className" placeholder="填写班级" /></td>
            <td><el-input v-model="item.studentCount" placeholder="人数" /></td>
            <td><el-input v-model="item.courseName" placeholder="课程名称" /></td>
            <td><el-input v-model="item.totalHours" placeholder="总学时" /></td>
            <td><el-input v-model="item.currentHours" placeholder="本学期" /></td>
            <td><el-input v-model="item.department" placeholder="部门" /></td>
            <td><el-input v-model="item.teacherName" placeholder="姓名" /></td>
            <td><el-input v-model="item.teacherTitle" placeholder="职称" /></td>
          </tr>
          <tr v-if="exportTableData.length === 0">
            <td colspan="9" style="text-align:center; color:#999">暂无数据，请先添加实验教学任务</td>
          </tr>
          </tbody>
        </table>
        <div class="table-bottom">
          <span>填表人：<el-input v-model="formFillName" style="width:100px" /></span>
          <span>院系（中心）盖章：<el-input v-model="formDeptSign" style="width:100px" /></span>
          <span>日期：<el-input v-model="formDate" style="width:120px" placeholder="年  月  日" /></span>
        </div>
      </div>
      <template #footer>
        <el-button @click="exportDialogVisible = false">关闭</el-button>
        <el-button type="primary" @click="downloadDocFn">下载 doc 文件</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { onMounted, reactive, ref } from 'vue'
import { ElMessage } from 'element-plus'
import { Download, Plus } from '@element-plus/icons-vue'
import { classApi, majorApi, semesterApi, type ClassDto, type MajorDto, type SemesterDto } from '@/api/teaching'
import { experimentApi, exportApi, type ExperimentTaskDto } from '@/api/experiment'
import { downloadDoc } from '@/utils/doc'
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

// 导出表格相关
const exportDialogVisible = ref(false)
const exportTableData = ref<any[]>([])
const formFillName = ref('')
const formDeptSign = ref('')
const formDate = ref('')

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

const openExportDialog = () => {
  exportTableData.value = list.value.map(row => ({
    className: `${row.major?.name || ''}${row.class?.name || ''}`,
    studentCount: row.studentCount?.toString() || '',
    courseName: row.courseName || '',
    totalHours: row.totalExperimentHours?.toString() || '',
    currentHours: row.currentSemesterExperimentHours?.toString() || '',
    department: '',
    teacherName: row.teacherNames || '',
    teacherTitle: row.teacherTitles || ''
  }))
  exportDialogVisible.value = true
}

const downloadDocFn = () => {
  if (exportTableData.value.length === 0) {
    ElMessage.warning('表格无数据')
    return
  }
  const dateStr = formDate.value ? `（${formDate.value}）` : ''
  const tableRows = exportTableData.value.map((item, idx) => `
    <tr>
      <td style="text-align:center">${idx + 1}</td>
      <td>${item.className}</td>
      <td style="text-align:center">${item.studentCount}</td>
      <td>${item.courseName}</td>
      <td style="text-align:center">${item.totalHours}</td>
      <td style="text-align:center">${item.currentHours}</td>
      <td>${item.department}</td>
      <td>${item.teacherName}</td>
      <td>${item.teacherTitle}</td>
    </tr>`).join('')
  const tableHtml = `
<h3>实验课程教学任务一览表</h3>
<table>
  <tr style="background:#ddd;text-align:center">
    <td rowspan="2">序号</td><td rowspan="2">专业、班级</td><td rowspan="2">学生人数</td>
    <td rowspan="2">实验课程名称</td><td rowspan="2">实验总学时</td>
    <td rowspan="2">本学期实验学时</td><td rowspan="2">课程承担部门</td>
    <td colspan="2">实验指导教师</td>
  </tr>
  <tr style="background:#ddd;text-align:center">
    <td>姓名</td><td>职称</td>
  </tr>
  ${tableRows}
</table>
<div class="footer">
  <span>填表人：${formFillName.value}</span>
  <span>院系（中心）盖章：${formDeptSign.value}</span>
  <span>日期：${formDate.value}</span>
</div>`
  downloadDoc(tableHtml, `实验课程教学任务一览表${dateStr}`)
  ElMessage.success('下载成功')
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
.table-wrap { padding: 10px; }
.table-title { text-align: center; margin: 10px 0 20px; font-size: 18px; }
.table-bottom { margin-top: 20px; display: flex; gap: 30px; align-items: center; flex-wrap: wrap; }
</style>
