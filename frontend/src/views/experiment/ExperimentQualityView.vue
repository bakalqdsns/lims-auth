<template>
  <div class="page-container">
    <div class="page-header">
      <h2>实验课程教学质量</h2>
      <div class="header-actions">
        <el-button @click="openExportDialog">
          <el-icon><Download /></el-icon>导出教学质量表
        </el-button>
        <el-button type="primary" @click="handleAdd">新增评估</el-button>
      </div>
    </div>
    <el-card shadow="never">
      <el-table :data="list" v-loading="loading" stripe>
        <el-table-column prop="courseName" label="课程名称" min-width="160" />
        <el-table-column prop="mainTeacher" label="主讲教师" width="100" />
        <el-table-column prop="teacherTitle" label="教师职称" width="90" />
        <el-table-column prop="className" label="授课班级" width="130" />
        <el-table-column prop="classStudentCount" label="班级人数" width="90" />
        <el-table-column prop="experimentHours" label="实验课时" width="90" />
        <el-table-column label="计划/实际" width="110">
          <template #default="{ row }">{{ row.plannedExperimentCount }} / {{ row.actualExperimentCount }}</template>
        </el-table-column>
        <el-table-column prop="assessmentMethod" label="考核方式" min-width="120" />
        <el-table-column prop="assessmentTime" label="考核时间" width="90" />
        <el-table-column label="独立设课" width="90">
          <template #default="{ row }"><el-tag :type="row.isIndependentCourse ? 'success' : 'info'" size="small">{{ row.isIndependentCourse ? '是' : '否' }}</el-tag></template>
        </el-table-column>
        <el-table-column label="操作" width="120" fixed="right">
          <template #default="{ row }"><el-button link type="primary" @click="handleEdit(row)">编辑</el-button></template>
        </el-table-column>
      </el-table>
    </el-card>

    <ExperimentQualityFormDialog v-model="dialogVisible" :quality="currentQuality" @success="loadList" />

    <!-- 表格预览弹窗：实验教学质量表 -->
    <el-dialog v-model="exportDialogVisible" title="实验课程教学质量表" width="1100px" top="20px">
      <div class="table-wrap">
        <h3 class="table-title">实验课程教学质量表</h3>
        <table border="1" cellpadding="4" cellspacing="0" width="100%" style="border-collapse:collapse; font-size:13px;">
          <thead>
          <tr style="background:#f0f0f0; text-align:center">
            <td style="min-width:40px">序号</td>
            <td style="min-width:140px">课程名称</td>
            <td style="min-width:70px">实验课时</td>
            <td style="min-width:70px">独立设课</td>
            <td style="min-width:80px">主讲教师</td>
            <td style="min-width:70px">教师职称</td>
            <td style="min-width:110px">授课班级</td>
            <td style="min-width:70px">班级人数</td>
            <td style="min-width:80px">计划实验个数</td>
            <td style="min-width:80px">实际实验个数</td>
            <td style="min-width:100px">未做实验项目</td>
            <td style="min-width:80px">考核方式</td>
            <td style="min-width:70px">考核时间</td>
          </tr>
          </thead>
          <tbody>
          <tr v-for="(item, idx) in exportTableData" :key="idx">
            <td style="text-align:center">{{ idx + 1 }}</td>
            <td><el-input v-model="item.courseName" placeholder="课程名称" /></td>
            <td><el-input v-model="item.experimentHours" placeholder="课时" /></td>
            <td><el-input v-model="item.isIndependent" placeholder="是/否" /></td>
            <td><el-input v-model="item.teacher" placeholder="教师" /></td>
            <td><el-input v-model="item.teacherTitle" placeholder="职称" /></td>
            <td><el-input v-model="item.className" placeholder="班级" /></td>
            <td><el-input v-model="item.studentCount" placeholder="人数" /></td>
            <td><el-input v-model="item.plannedCount" placeholder="计划" /></td>
            <td><el-input v-model="item.actualCount" placeholder="实际" /></td>
            <td><el-input v-model="item.missedItems" placeholder="未做项目" /></td>
            <td><el-input v-model="item.assessMethod" placeholder="考核方式" /></td>
            <td><el-input v-model="item.assessTime" placeholder="考核时间" /></td>
          </tr>
          <tr v-if="exportTableData.length === 0">
            <td colspan="13" style="text-align:center; color:#999">暂无数据，请先添加教学质量记录</td>
          </tr>
          </tbody>
        </table>
        <div class="table-bottom">
          <span>填表人：<el-input v-model="formFillName" style="width:100px" /></span>
          <span>审核：<el-input v-model="formCheckName" style="width:100px" /></span>
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
import { onMounted, ref } from 'vue'
import { ElMessage } from 'element-plus'
import { Download } from '@element-plus/icons-vue'
import { experimentApi, type ExperimentQualityDto } from '@/api/experiment'
import { downloadDoc } from '@/utils/doc'
import ExperimentQualityFormDialog from './components/ExperimentQualityFormDialog.vue'

const loading = ref(false)
const list = ref<ExperimentQualityDto[]>([])
const dialogVisible = ref(false)
const currentQuality = ref<ExperimentQualityDto | undefined>(undefined)

const loadList = async () => {
  loading.value = true
  try {
    const res = await experimentApi.getQualityList()
    list.value = res.data.data || []
  } finally {
    loading.value = false
  }
}

const handleAdd = () => {
  currentQuality.value = undefined
  dialogVisible.value = true
}

const handleEdit = (row: ExperimentQualityDto) => {
  currentQuality.value = row
  dialogVisible.value = true
}

// ====== 导出表格 ======
const exportDialogVisible = ref(false)
const exportTableData = ref<any[]>([])
const formFillName = ref('')
const formCheckName = ref('')
const formDate = ref('')

const openExportDialog = () => {
  exportTableData.value = list.value.map(row => ({
    courseName: row.courseName || '',
    experimentHours: row.experimentHours?.toString() || '',
    isIndependent: row.isIndependentCourse ? '是' : '否',
    teacher: row.mainTeacher || '',
    teacherTitle: row.teacherTitle || '',
    className: row.className || '',
    studentCount: row.classStudentCount?.toString() || '',
    plannedCount: row.plannedExperimentCount?.toString() || '',
    actualCount: row.actualExperimentCount?.toString() || '',
    missedItems: row.missedExperimentItems || '',
    assessMethod: row.assessmentMethod || '',
    assessTime: row.assessmentTime || ''
  }))
  exportDialogVisible.value = true
}

const downloadDocFn = () => {
  if (exportTableData.value.length === 0) { ElMessage.warning('表格无数据'); return }
  const rows = exportTableData.value.map((item, idx) => `
    <tr>
      <td style="text-align:center">${idx + 1}</td>
      <td>${item.courseName}</td>
      <td style="text-align:center">${item.experimentHours}</td>
      <td style="text-align:center">${item.isIndependent}</td>
      <td>${item.teacher}</td>
      <td>${item.teacherTitle}</td>
      <td>${item.className}</td>
      <td style="text-align:center">${item.studentCount}</td>
      <td style="text-align:center">${item.plannedCount}</td>
      <td style="text-align:center">${item.actualCount}</td>
      <td>${item.missedItems}</td>
      <td>${item.assessMethod}</td>
      <td>${item.assessTime}</td>
    </tr>`).join('')
  const tableHtml = `
<h3>实验课程教学质量表</h3>
<table>
  <tr style="background:#ddd;text-align:center">
    <td>序号</td><td>课程名称</td><td>实验课时</td><td>独立设课</td>
    <td>主讲教师</td><td>教师职称</td><td>授课班级</td><td>班级人数</td>
    <td>计划实验个数</td><td>实际实验个数</td><td>未做实验项目</td>
    <td>考核方式</td><td>考核时间</td>
  </tr>
  ${rows}
</table>
<div class="footer">
  <span>填表人：${formFillName.value}</span>
  <span>审核：${formCheckName.value}</span>
  <span>日期：${formDate.value}</span>
</div>`
  downloadDoc(tableHtml, '实验课程教学质量表')
  ElMessage.success('下载成功')
}

onMounted(loadList)
</script>

<style scoped>
.page-container { padding: 20px; }
.page-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 16px; }
.page-header h2 { margin: 0; font-size: 20px; font-weight: 500; }
.header-actions { display: flex; gap: 10px; }
.table-wrap { padding: 10px; }
.table-title { text-align: center; margin: 10px 0 20px; font-size: 18px; }
.table-bottom { margin-top: 20px; display: flex; gap: 30px; align-items: center; flex-wrap: wrap; }
</style>
