<template>
  <div class="page-container">
    <div class="page-header">
      <h2>实验课程教学质量</h2>
      <el-button type="primary" @click="openDialog()">新增评估</el-button>
    </div>
    <el-card shadow="never">
      <el-table :data="list" v-loading="loading" stripe>
        <el-table-column prop="courseName" label="课程名称" min-width="180" />
        <el-table-column prop="mainTeacher" label="主讲教师" width="120" />
        <el-table-column prop="className" label="授课班级" width="120" />
        <el-table-column prop="plannedExperimentCount" label="计划个数" width="90" />
        <el-table-column prop="actualExperimentCount" label="实际个数" width="90" />
        <el-table-column prop="assessmentMethod" label="考核方式" min-width="120" />
        <el-table-column label="操作" width="120" fixed="right">
          <template #default="{ row }">
            <el-button link type="primary" @click="openDialog(row)">编辑</el-button>
          </template>
        </el-table-column>
      </el-table>
    </el-card>

    <el-dialog v-model="dialogVisible" :title="form.id ? '编辑评估' : '新增评估'" width="760px">
      <el-form :model="form" label-width="120px">
        <el-row :gutter="12">
          <el-col :span="12"><el-form-item label="教学任务"><el-select v-model="form.experimentTaskId"><el-option v-for="t in tasks" :key="t.id" :label="t.courseName" :value="t.id" /></el-select></el-form-item></el-col>
          <el-col :span="12"><el-form-item label="课程名称"><el-input v-model="form.courseName" /></el-form-item></el-col>
          <el-col :span="12"><el-form-item label="主讲教师"><el-input v-model="form.mainTeacher" /></el-form-item></el-col>
          <el-col :span="12"><el-form-item label="授课班级"><el-input v-model="form.className" /></el-form-item></el-col>
          <el-col :span="12"><el-form-item label="计划个数"><el-input-number v-model="form.plannedExperimentCount" :min="0" style="width: 100%" /></el-form-item></el-col>
          <el-col :span="12"><el-form-item label="实际个数"><el-input-number v-model="form.actualExperimentCount" :min="0" style="width: 100%" /></el-form-item></el-col>
          <el-col :span="24"><el-form-item label="考核方式"><el-input v-model="form.assessmentMethod" /></el-form-item></el-col>
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
import { experimentApi, type ExperimentQualityDto, type ExperimentTaskDto } from '@/api/experiment'

const loading = ref(false)
const list = ref<ExperimentQualityDto[]>([])
const tasks = ref<ExperimentTaskDto[]>([])
const dialogVisible = ref(false)

const emptyForm = () => ({
  id: '',
  experimentTaskId: '',
  institutionId: undefined as string | undefined,
  courseName: '',
  experimentHours: 0,
  isIndependentCourse: false,
  mainTeacher: '',
  teacherTitle: '',
  technicalStaff: '',
  technicalTitle: '',
  className: '',
  classStudentCount: 0,
  plannedExperimentCount: 0,
  actualExperimentCount: 0,
  missedExperimentItems: '',
  assessmentMethod: '',
  assessmentStudentCount: 0,
  assessmentTime: '',
  status: 'Active',
  sortOrder: 0,
  description: ''
})
const form = reactive(emptyForm())

const loadList = async () => {
  loading.value = true
  try {
    const res = await experimentApi.getQualityList()
    list.value = res.data.data || []
  } finally {
    loading.value = false
  }
}

const loadTasks = async () => {
  const res = await experimentApi.getTasks()
  tasks.value = res.data.data || []
}

const openDialog = (row?: ExperimentQualityDto) => {
  Object.assign(form, emptyForm(), row || {})
  dialogVisible.value = true
}

const save = async () => {
  const payload: any = { ...form }
  if (payload.id) await experimentApi.updateQuality(payload.id, payload)
  else {
    delete payload.id
    await experimentApi.createQuality(payload)
  }
  ElMessage.success('保存成功')
  dialogVisible.value = false
  loadList()
}

onMounted(async () => {
  await Promise.all([loadList(), loadTasks()])
})
</script>

<style scoped>
.page-container { padding: 20px; }
.page-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 16px; }
.page-header h2 { margin: 0; font-size: 20px; font-weight: 500; }
</style>

