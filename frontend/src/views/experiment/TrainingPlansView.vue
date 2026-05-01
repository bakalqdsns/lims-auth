<template>
  <div class="page-container">
    <div class="page-header">
      <h2>实训教学计划</h2>
      <div class="header-actions">
        <el-button @click="openTable1">
          <el-icon><Download /></el-icon>导出实训教学计划表
        </el-button>
        <el-button @click="openTable2">
          <el-icon><Download /></el-icon>导出实训任务一览表
        </el-button>
        <el-button type="primary" @click="handleAdd">新增计划</el-button>
      </div>
    </div>
    <el-card shadow="never">
      <el-table :data="list" v-loading="loading" stripe>
        <el-table-column prop="semester.name" label="学期" min-width="160" />
        <el-table-column prop="course.name" label="课程" min-width="180">
          <template #default="{ row }">{{ row.course?.name || row.courseName || row.courseId }}</template>
        </el-table-column>
        <el-table-column prop="major.name" label="专业" min-width="140" />
        <el-table-column prop="class.name" label="班级" min-width="140" />
        <el-table-column prop="studentCount" label="人数" width="70" />
        <el-table-column prop="studentLevel" label="层次" width="80" />
        <el-table-column prop="teachingOrganizationMethod" label="组织方式" min-width="120" />
        <el-table-column prop="teachingLocation" label="教学地点" min-width="160" />
        <el-table-column prop="assessmentMethod" label="考核方式" min-width="120" />
        <el-table-column label="操作" width="120" fixed="right">
          <template #default="{ row }"><el-button link type="primary" @click="handleEdit(row)">编辑</el-button></template>
        </el-table-column>
      </el-table>
    </el-card>

    <TrainingPlanFormDialog v-model="dialogVisible" :plan="currentPlan" @success="loadList" />

    <!-- 表格一：实训教学计划表 -->
    <el-dialog v-model="dialogTable1" title="实训教学计划表" width="1100px" top="20px">
      <div class="table-wrap">
        <h3 class="table-title">实训教学计划表</h3>
        <table border="1" cellpadding="4" cellspacing="0" width="100%" style="border-collapse:collapse; font-size:13px;">
          <thead>
          <tr style="background:#f0f0f0; text-align:center">
            <td style="min-width:40px">序号</td>
            <td style="min-width:120px">学期</td>
            <td style="min-width:160px">课程名称</td>
            <td style="min-width:120px">专业</td>
            <td style="min-width:120px">班级</td>
            <td style="min-width:50px">人数</td>
            <td style="min-width:70px">层次</td>
            <td style="min-width:100px">组织方式</td>
            <td style="min-width:140px">教学地点</td>
            <td style="min-width:100px">考核方式</td>
          </tr>
          </thead>
          <tbody>
          <tr v-for="(item, idx) in table1Data" :key="idx">
            <td style="text-align:center">{{ idx + 1 }}</td>
            <td><el-input v-model="item.semester" placeholder="学期" /></td>
            <td><el-input v-model="item.course" placeholder="课程名称" /></td>
            <td><el-input v-model="item.major" placeholder="专业" /></td>
            <td><el-input v-model="item.className" placeholder="班级" /></td>
            <td><el-input v-model="item.count" placeholder="人数" /></td>
            <td><el-input v-model="item.level" placeholder="层次" /></td>
            <td><el-input v-model="item.orgMethod" placeholder="组织方式" /></td>
            <td><el-input v-model="item.address" placeholder="教学地点" /></td>
            <td><el-input v-model="item.assess" placeholder="考核方式" /></td>
          </tr>
          <tr v-if="table1Data.length === 0">
            <td colspan="10" style="text-align:center; color:#999">暂无数据，请先添加实训教学计划</td>
          </tr>
          </tbody>
        </table>
        <div class="table-bottom">
          <span>填表人：<el-input v-model="fillUser1" style="width:100px" /></span>
          <span>审核人：<el-input v-model="checkUser1" style="width:100px" /></span>
          <span>日期：<el-input v-model="fillDate1" style="width:120px" placeholder="年  月  日" /></span>
        </div>
      </div>
      <template #footer>
        <el-button @click="dialogTable1 = false">关闭</el-button>
        <el-button type="primary" @click="downloadDoc1">下载 doc 文件</el-button>
      </template>
    </el-dialog>

    <!-- 表格二：实训任务一览表 -->
    <el-dialog v-model="dialogTable2" title="实训课程教学任务一览表" width="1000px" top="20px">
      <div class="table-wrap">
        <h3 class="table-title">实训课程教学任务一览表</h3>
        <table border="1" cellpadding="4" cellspacing="0" width="100%" style="border-collapse:collapse; font-size:13px;">
          <thead>
          <tr style="background:#f0f0f0; text-align:center">
            <td style="min-width:40px">序号</td>
            <td style="min-width:160px">课程名称</td>
            <td style="min-width:180px">教学目的要求</td>
            <td style="min-width:200px">教学内容安排</td>
            <td style="min-width:120px">实训方式</td>
            <td style="min-width:100px">考核方式</td>
          </tr>
          </thead>
          <tbody>
          <tr v-for="(item, idx) in table2Data" :key="idx">
            <td style="text-align:center">{{ idx + 1 }}</td>
            <td><el-input v-model="item.course" placeholder="课程名称" /></td>
            <td><el-input v-model="item.purpose" type="textarea" :rows="2" placeholder="教学目的要求" /></td>
            <td><el-input v-model="item.content" type="textarea" :rows="2" placeholder="教学内容安排" /></td>
            <td><el-input v-model="item.trainMethod" placeholder="实训方式" /></td>
            <td><el-input v-model="item.assess" placeholder="考核方式" /></td>
          </tr>
          <tr v-if="table2Data.length === 0">
            <td colspan="6" style="text-align:center; color:#999">暂无数据，请先添加实训教学计划</td>
          </tr>
          </tbody>
        </table>
        <div class="table-bottom">
          <span>填表人：<el-input v-model="fillUser2" style="width:100px" /></span>
          <span>审核：<el-input v-model="checkUser2" style="width:100px" /></span>
          <span>日期：<el-input v-model="fillDate2" style="width:120px" placeholder="年  月  日" /></span>
        </div>
      </div>
      <template #footer>
        <el-button @click="dialogTable2 = false">关闭</el-button>
        <el-button type="primary" @click="downloadDoc2">下载 doc 文件</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { ElMessage } from 'element-plus'
import { Download } from '@element-plus/icons-vue'
import { experimentApi, type TrainingPlanDto } from '@/api/experiment'
import { downloadDoc } from '@/utils/doc'
import TrainingPlanFormDialog from './components/TrainingPlanFormDialog.vue'

const loading = ref(false)
const list = ref<TrainingPlanDto[]>([])
const dialogVisible = ref(false)
const currentPlan = ref<TrainingPlanDto | undefined>(undefined)

const loadList = async () => {
  loading.value = true
  try {
    const res = await experimentApi.getTrainingPlans()
    list.value = res.data.data || []
  } finally {
    loading.value = false
  }
}

const handleAdd = () => {
  currentPlan.value = undefined
  dialogVisible.value = true
}

const handleEdit = (row: TrainingPlanDto) => {
  currentPlan.value = row
  dialogVisible.value = true
}

// ====== 表格一：实训教学计划表 ======
const dialogTable1 = ref(false)
const table1Data = ref<any[]>([])
const fillUser1 = ref('')
const checkUser1 = ref('')
const fillDate1 = ref('')

const openTable1 = () => {
  table1Data.value = list.value.map(item => ({
    semester: item.semester?.name || '',
    course: item.courseName || item.course?.name || '',
    major: item.major?.name || '',
    className: item.class?.name || '',
    count: item.studentCount?.toString() || '',
    level: item.studentLevel || '',
    orgMethod: item.teachingOrganizationMethod || '',
    address: item.teachingLocation || '',
    assess: item.assessmentMethod || ''
  }))
  dialogTable1.value = true
}

const downloadDoc1 = () => {
  if (table1Data.value.length === 0) { ElMessage.warning('表格无数据'); return }
  const rows = table1Data.value.map((item, idx) => `
    <tr>
      <td style="text-align:center">${idx + 1}</td>
      <td>${item.semester}</td>
      <td>${item.course}</td>
      <td>${item.major}</td>
      <td>${item.className}</td>
      <td style="text-align:center">${item.count}</td>
      <td>${item.level}</td>
      <td>${item.orgMethod}</td>
      <td>${item.address}</td>
      <td>${item.assess}</td>
    </tr>`).join('')
  const tableHtml = `
<h3>实训教学计划表</h3>
<table>
  <tr style="background:#ddd;text-align:center">
    <td>序号</td><td>学期</td><td>课程名称</td><td>专业</td>
    <td>班级</td><td>人数</td><td>层次</td>
    <td>组织方式</td><td>教学地点</td><td>考核方式</td>
  </tr>
  ${rows}
</table>
<div class="footer">
  <span>填表人：${fillUser1.value}</span>
  <span>审核人：${checkUser1.value}</span>
  <span>日期：${fillDate1.value}</span>
</div>`
  downloadDoc(tableHtml, '实训教学计划表')
  ElMessage.success('下载成功')
}

// ====== 表格二：实训任务一览表 ======
const dialogTable2 = ref(false)
const table2Data = ref<any[]>([])
const fillUser2 = ref('')
const checkUser2 = ref('')
const fillDate2 = ref('')

const openTable2 = () => {
  table2Data.value = list.value.map(item => ({
    course: item.courseName || item.course?.name || '',
    purpose: item.teachingPurpose || '',
    content: item.teachingContent || '',
    trainMethod: item.trainingMethod || '',
    assess: item.assessmentMethod || ''
  }))
  dialogTable2.value = true
}

const downloadDoc2 = () => {
  if (table2Data.value.length === 0) { ElMessage.warning('表格无数据'); return }
  const rows = table2Data.value.map((item, idx) => `
    <tr>
      <td style="text-align:center">${idx + 1}</td>
      <td>${item.course}</td>
      <td>${item.purpose}</td>
      <td>${item.content}</td>
      <td>${item.trainMethod}</td>
      <td>${item.assess}</td>
    </tr>`).join('')
  const tableHtml = `
<h3>实训课程教学任务一览表</h3>
<table>
  <tr style="background:#ddd;text-align:center">
    <td>序号</td><td>课程名称</td><td>教学目的要求</td>
    <td>教学内容安排</td><td>实训方式</td><td>考核方式</td>
  </tr>
  ${rows}
</table>
<div class="footer">
  <span>填表人：${fillUser2.value}</span>
  <span>审核：${checkUser2.value}</span>
  <span>日期：${fillDate2.value}</span>
</div>`
  downloadDoc(tableHtml, '实训课程教学任务一览表')
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
