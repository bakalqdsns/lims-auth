<template>
  <div class="page-container">
    <div class="page-header">
      <h2>实训教学计划</h2>
      <el-button type="primary" @click="openDialog()">新增计划</el-button>
    </div>
    <el-card shadow="never">
      <el-table :data="list" v-loading="loading" stripe>
        <el-table-column label="课程" min-width="180">
          <template #default="{ row }">{{ row.course?.name || row.courseId }}</template>
        </el-table-column>
        <el-table-column prop="teachingOrganizationMethod" label="组织方式" min-width="150" />
        <el-table-column prop="teachingLocation" label="教学地点" min-width="150" />
        <el-table-column prop="assessmentMethod" label="考核方式" min-width="120" />
        <el-table-column label="操作" width="120" fixed="right">
          <template #default="{ row }"><el-button link type="primary" @click="openDialog(row)">编辑</el-button></template>
        </el-table-column>
      </el-table>
    </el-card>

    <el-dialog v-model="dialogVisible" :title="form.id ? '编辑计划' : '新增计划'" width="760px">
      <el-form :model="form" label-width="140px">
        <el-form-item label="课程"><el-select v-model="form.courseId"><el-option v-for="c in courses" :key="c.id" :label="`${c.code} - ${c.name}`" :value="c.id" /></el-select></el-form-item>
        <el-form-item label="组织方式"><el-input v-model="form.teachingOrganizationMethod" /></el-form-item>
        <el-form-item label="教学地点"><el-input v-model="form.teachingLocation" /></el-form-item>
        <el-form-item label="教学目的要求"><el-input v-model="form.teachingPurpose" type="textarea" :rows="2" /></el-form-item>
        <el-form-item label="教学内容安排"><el-input v-model="form.teachingContent" type="textarea" :rows="2" /></el-form-item>
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
import { courseApi, type CourseDto } from '@/api/teaching'
import { experimentApi, type TrainingPlanDto } from '@/api/experiment'

const loading = ref(false)
const list = ref<TrainingPlanDto[]>([])
const courses = ref<CourseDto[]>([])
const dialogVisible = ref(false)

const emptyForm = () => ({
  id: '',
  courseId: '',
  teachingOrganizationMethod: '',
  teachingLocation: '',
  teachingPurpose: '',
  teachingContent: '',
  trainingMethod: '',
  assessmentMethod: '',
  qualityAssuranceMeasures: '',
  experimentCenterOpinion: '',
  departmentOpinion: '',
  status: 'Active',
  sortOrder: 0,
  description: ''
})
const form = reactive(emptyForm())

const loadList = async () => {
  loading.value = true
  try {
    const res = await experimentApi.getTrainingPlans()
    list.value = res.data.data || []
  } finally {
    loading.value = false
  }
}

const loadCourses = async () => {
  const res = await courseApi.getList({ page: 1, pageSize: 999 })
  courses.value = res.data.data || []
}

const openDialog = (row?: TrainingPlanDto) => {
  Object.assign(form, emptyForm(), row || {})
  dialogVisible.value = true
}

const save = async () => {
  const payload: any = { ...form }
  if (payload.id) await experimentApi.updateTrainingPlan(payload.id, payload)
  else {
    delete payload.id
    await experimentApi.createTrainingPlan(payload)
  }
  ElMessage.success('保存成功')
  dialogVisible.value = false
  loadList()
}

onMounted(async () => {
  await Promise.all([loadList(), loadCourses()])
})
</script>

<style scoped>
.page-container { padding: 20px; }
.page-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 16px; }
.page-header h2 { margin: 0; font-size: 20px; font-weight: 500; }
</style>

