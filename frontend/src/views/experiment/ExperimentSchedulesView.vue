<template>
  <div class="page-container">
    <div class="page-header">
      <h2>实验项目开出</h2>
      <div class="header-actions">
        <el-button @click="openExportDialog" :loading="exportingSchedule">
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
        <el-table-column prop="experimentRequirement" label="实验要求" width="80" />
        <el-table-column label="周-星期-节次" width="120">
          <template #default="{ row }">{{ row.weekNumber }}-{{ row.dayOfWeek }}-{{ row.periodNumber }}</template>
        </el-table-column>
        <el-table-column label="分组信息" width="120">
          <template #default="{ row }">{{ row.parallelGroups }}组×{{ row.studentsPerGroup }}人</template>
        </el-table-column>
        <el-table-column label="地点" min-width="160">
          <template #default="{ row }">{{ formatLocation(row) }}</template>
        </el-table-column>
        <el-table-column label="是否开出" width="90">
          <template #default="{ row }"><el-tag :type="row.isConducted ? 'success' : 'warning'" size="small">{{ row.isConducted ? '是' : '否' }}</el-tag></template>
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

    <!-- 表格预览弹窗：实验教学授课计划表 -->
    <el-dialog v-model="exportDialogVisible" title="实验教学授课计划表" width="1100px" top="20px">
      <div class="table-wrap">
        <h3 class="table-title">实验教学授课计划表</h3>
        <table border="1" cellpadding="4" cellspacing="0" width="100%" style="border-collapse:collapse; font-size:13px;">
          <thead>
          <tr style="background:#f0f0f0; text-align:center">
            <td style="min-width:40px">序号</td>
            <td style="min-width:160px">课程名称</td>
            <td style="min-width:140px">实验项目</td>
            <td style="min-width:60px">实验要求</td>
            <td style="min-width:80px">周-星期-节次</td>
            <td style="min-width:100px">分组信息</td>
            <td style="min-width:140px">实验地点</td>
            <td style="min-width:70px">是否开出</td>
            <td style="min-width:120px">备注</td>
          </tr>
          </thead>
          <tbody>
          <tr v-for="(item, idx) in exportTableData" :key="idx">
            <td style="text-align:center">{{ idx + 1 }}</td>
            <td><el-input v-model="item.courseName" placeholder="课程名称" /></td>
            <td><el-input v-model="item.itemName" placeholder="实验项目" /></td>
            <td><el-input v-model="item.requirement" placeholder="必做/选做" /></td>
            <td><el-input v-model="item.schedule" placeholder="如：3-2-4" /></td>
            <td><el-input v-model="item.groupInfo" placeholder="如：2组×20人" /></td>
            <td><el-input v-model="item.location" placeholder="实验室/地点" /></td>
            <td><el-input v-model="item.isConducted" placeholder="是/否" /></td>
            <td><el-input v-model="item.remark" placeholder="备注" /></td>
          </tr>
          <tr v-if="exportTableData.length === 0">
            <td colspan="9" style="text-align:center; color:#999">暂无数据，请先添加开出记录</td>
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
import { onMounted, reactive, ref } from 'vue'
import { ElMessage } from 'element-plus'
import { Download, Plus } from '@element-plus/icons-vue'
import { semesterApi, type SemesterDto } from '@/api/teaching'
import { experimentApi, type ExperimentScheduleDto } from '@/api/experiment'
import { downloadDoc } from '@/utils/doc'
import ExperimentScheduleFormDialog from './components/ExperimentScheduleFormDialog.vue'

const loading = ref(false)
const exportingSchedule = ref(false)
const list = ref<ExperimentScheduleDto[]>([])
const semesterOptions = ref<SemesterDto[]>([])
const dialogVisible = ref(false)
const currentSchedule = ref<ExperimentScheduleDto | undefined>(undefined)
const optionsReady = ref(false)

// 导出表格相关
const exportDialogVisible = ref(false)
const exportTableData = ref<any[]>([])
const formFillName = ref('')
const formCheckName = ref('')
const formDate = ref('')

const searchForm = reactive({ semesterId: '' })

const loadList = async () => {
  loading.value = true
  try {
    const res = await experimentApi.getSchedules()
    let data = res.data.data || []
    if (searchForm.semesterId) {
      data = data.filter((s: any) => s.experimentTask?.semesterId === searchForm.semesterId)
    }
    list.value = data
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

const openExportDialog = () => {
  exportTableData.value = list.value.map(row => ({
    courseName: row.experimentTask?.courseName || '',
    itemName: row.experimentItem?.experimentName || '',
    requirement: row.experimentRequirement || '',
    schedule: row.weekNumber ? `${row.weekNumber}-${row.dayOfWeek || ''}-${row.periodNumber || ''}` : '',
    groupInfo: row.parallelGroups && row.studentsPerGroup
      ? `${row.parallelGroups}组×${row.studentsPerGroup}人` : '',
    location: row.lab?.name || row.location || '',
    isConducted: row.isConducted ? '是' : '否',
    remark: row.description || ''
  }))
  exportDialogVisible.value = true
}

const downloadDocFn = () => {
  if (exportTableData.value.length === 0) { ElMessage.warning('表格无数据'); return }
  const rows = exportTableData.value.map((item, idx) => `
    <tr>
      <td style="text-align:center">${idx + 1}</td>
      <td>${item.courseName}</td>
      <td>${item.itemName}</td>
      <td>${item.requirement}</td>
      <td style="text-align:center">${item.schedule}</td>
      <td style="text-align:center">${item.groupInfo}</td>
      <td>${item.location}</td>
      <td style="text-align:center">${item.isConducted}</td>
      <td>${item.remark}</td>
    </tr>`).join('')
  const tableHtml = `
<h3>实验教学授课计划表</h3>
<table>
  <tr style="background:#ddd;text-align:center">
    <td>序号</td><td>课程名称</td><td>实验项目</td><td>实验要求</td>
    <td>周-星期-节次</td><td>分组信息</td><td>实验地点</td>
    <td>是否开出</td><td>备注</td>
  </tr>
  ${rows}
</table>
<div class="footer">
  <span>填表人：${formFillName.value}</span>
  <span>审核：${formCheckName.value}</span>
  <span>日期：${formDate.value}</span>
</div>`
  downloadDoc(tableHtml, '实验教学授课计划表')
  ElMessage.success('下载成功')
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
.table-wrap { padding: 10px; }
.table-title { text-align: center; margin: 10px 0 20px; font-size: 18px; }
.table-bottom { margin-top: 20px; display: flex; gap: 30px; align-items: center; flex-wrap: wrap; }
</style>
