<template>
  <div class="page-container">
    <div class="page-header">
      <h2>实验项目开出</h2>
      <el-button type="primary" @click="openDialog()">新增开出记录</el-button>
    </div>
    <el-card shadow="never">
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
            <el-button link type="primary" @click="openDialog(row)">编辑</el-button>
            <el-popconfirm title="确认删除该记录？" @confirm="remove(row.id)">
              <template #reference><el-button link type="danger">删除</el-button></template>
            </el-popconfirm>
          </template>
        </el-table-column>
      </el-table>
    </el-card>

    <el-dialog v-model="dialogVisible" :title="form.id ? '编辑开出记录' : '新增开出记录'" width="760px">
      <el-form :model="form" label-width="120px">
        <el-row :gutter="12">
          <el-col :span="12"><el-form-item label="教学任务"><el-select v-model="form.experimentTaskId"><el-option v-for="t in tasks" :key="t.id" :label="t.courseName" :value="t.id" /></el-select></el-form-item></el-col>
          <el-col :span="12"><el-form-item label="实验项目"><el-select v-model="form.experimentItemId"><el-option v-for="i in items" :key="i.id" :label="i.experimentName" :value="i.id" /></el-select></el-form-item></el-col>
          <el-col :span="8"><el-form-item label="周次"><el-input-number v-model="form.weekNumber" :min="1" style="width: 100%" /></el-form-item></el-col>
          <el-col :span="8"><el-form-item label="星期"><el-input-number v-model="form.dayOfWeek" :min="1" :max="7" style="width: 100%" /></el-form-item></el-col>
          <el-col :span="8"><el-form-item label="节次"><el-input-number v-model="form.periodNumber" :min="1" style="width: 100%" /></el-form-item></el-col>
          <el-col :span="12"><el-form-item label="实验室"><el-select v-model="form.labId" clearable><el-option v-for="l in labs" :key="l.id" :label="l.name" :value="l.id" /></el-select></el-form-item></el-col>
          <el-col :span="12"><el-form-item label="地点文本"><el-input v-model="form.location" /></el-form-item></el-col>
          <el-col :span="12"><el-form-item label="是否开出"><el-switch v-model="form.isConducted" /></el-form-item></el-col>
          <el-col :span="24"><el-form-item label="未开出原因"><el-input v-model="form.reasonIfNotConducted" type="textarea" :rows="2" /></el-form-item></el-col>
        </el-row>
      </el-form>
      <template #footer>
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button type="primary" @click="save">保存</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { onMounted, reactive, ref } from 'vue'
import { ElMessage } from 'element-plus'
import { labApi, type LabDto } from '@/api/lab'
import { experimentApi, type ExperimentItemDto, type ExperimentScheduleDto, type ExperimentTaskDto } from '@/api/experiment'

const loading = ref(false)
const list = ref<ExperimentScheduleDto[]>([])
const tasks = ref<ExperimentTaskDto[]>([])
const items = ref<ExperimentItemDto[]>([])
const labs = ref<LabDto[]>([])
const dialogVisible = ref(false)

const emptyForm = () => ({
  id: '',
  experimentTaskId: '',
  experimentItemId: '',
  weekNumber: 1,
  dayOfWeek: 1,
  periodNumber: 1,
  parallelGroups: 1,
  studentsPerGroup: 1,
  cycleCount: 1,
  experimentRequirement: '必做',
  location: '',
  labId: undefined as string | undefined,
  isConducted: true,
  reasonIfNotConducted: '',
  status: 'Active',
  sortOrder: 0,
  description: ''
})
const form = reactive(emptyForm())

const loadList = async () => {
  loading.value = true
  try {
    const res = await experimentApi.getSchedules()
    list.value = res.data.data || []
  } finally {
    loading.value = false
  }
}

const loadOptions = async () => {
  const [taskRes, itemRes, labRes] = await Promise.all([experimentApi.getTasks(), experimentApi.getItems(), labApi.getList()])
  tasks.value = taskRes.data.data || []
  items.value = itemRes.data.data || []
  labs.value = labRes.data.data || []
}

const openDialog = (row?: ExperimentScheduleDto) => {
  Object.assign(form, emptyForm(), row || {})
  dialogVisible.value = true
}

const save = async () => {
  const payload: any = { ...form }
  if (payload.id) await experimentApi.updateSchedule(payload.id, payload)
  else {
    delete payload.id
    await experimentApi.createSchedule(payload)
  }
  ElMessage.success('保存成功')
  dialogVisible.value = false
  loadList()
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

onMounted(async () => {
  await Promise.all([loadList(), loadOptions()])
})
</script>

<style scoped>
.page-container { padding: 20px; }
.page-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 16px; }
.page-header h2 { margin: 0; font-size: 20px; font-weight: 500; }
</style>

