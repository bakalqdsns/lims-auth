<template>
  <div class="page-container">
    <div class="page-header">
      <h2>实验教学任务</h2>
      <el-button type="primary" @click="openDialog()">新增任务</el-button>
    </div>

    <el-card shadow="never">
      <el-table :data="list" v-loading="loading" stripe>
        <el-table-column prop="semester.name" label="学期" min-width="140" />
        <el-table-column prop="major.name" label="专业" min-width="120" />
        <el-table-column prop="class.name" label="班级" min-width="120" />
        <el-table-column prop="courseName" label="课程名称" min-width="160" />
        <el-table-column prop="studentCount" label="人数" width="80" />
        <el-table-column prop="studentLevel" label="层次" width="90" />
        <el-table-column label="操作" width="180" fixed="right">
          <template #default="{ row }">
            <el-button link type="primary" @click="openDialog(row)">编辑</el-button>
            <el-popconfirm title="确认删除该任务？" @confirm="remove(row.id)">
              <template #reference>
                <el-button link type="danger">删除</el-button>
              </template>
            </el-popconfirm>
          </template>
        </el-table-column>
      </el-table>
    </el-card>

    <el-dialog v-model="dialogVisible" :title="form.id ? '编辑任务' : '新增任务'" width="760px">
      <el-form :model="form" label-width="130px">
        <el-row :gutter="12">
          <el-col :span="12"><el-form-item label="学期"><el-select v-model="form.semesterId"><el-option v-for="s in semesterOptions" :key="s.id" :label="s.name" :value="s.id" /></el-select></el-form-item></el-col>
          <el-col :span="12"><el-form-item label="专业"><el-select v-model="form.majorId"><el-option v-for="m in majorOptions" :key="m.id" :label="m.name" :value="m.id" /></el-select></el-form-item></el-col>
          <el-col :span="12"><el-form-item label="班级"><el-select v-model="form.classId"><el-option v-for="c in classOptions" :key="c.id" :label="c.name" :value="c.id" /></el-select></el-form-item></el-col>
          <el-col :span="12"><el-form-item label="课程名称"><el-input v-model="form.courseName" /></el-form-item></el-col>
          <el-col :span="12"><el-form-item label="学生人数"><el-input-number v-model="form.studentCount" :min="0" style="width: 100%" /></el-form-item></el-col>
          <el-col :span="12"><el-form-item label="学生层次"><el-select v-model="form.studentLevel"><el-option label="专科" value="专科" /><el-option label="本科" value="本科" /><el-option label="研究生" value="研究生" /></el-select></el-form-item></el-col>
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
import { classApi, majorApi, semesterApi, type ClassDto, type MajorDto, type SemesterDto } from '@/api/teaching'
import { experimentApi, type ExperimentTaskDto } from '@/api/experiment'

const loading = ref(false)
const list = ref<ExperimentTaskDto[]>([])
const semesterOptions = ref<SemesterDto[]>([])
const majorOptions = ref<MajorDto[]>([])
const classOptions = ref<ClassDto[]>([])

const dialogVisible = ref(false)
const emptyForm = () => ({
  id: '',
  semesterId: '',
  majorId: '',
  classId: '',
  studentCount: 0,
  studentLevel: '',
  courseName: '',
  courseType: '',
  isIndependentCourse: false,
  totalExperimentHours: 0,
  currentSemesterExperimentHours: 0,
  totalPracticeHours: 0,
  currentSemesterPracticeHours: 0,
  totalTrainingHours: 0,
  currentSemesterTrainingHours: 0,
  institutionId: undefined as string | undefined,
  departmentId: undefined as string | undefined,
  teacherIds: '',
  teacherTitles: '',
  technicalStaff: '',
  technicalTitle: '',
  textbookName: '',
  experimentGuideName: '',
  status: 'Active',
  sortOrder: 0,
  description: ''
})
const form = reactive(emptyForm())

const loadList = async () => {
  loading.value = true
  try {
    const res = await experimentApi.getTasks()
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
}

const openDialog = (row?: ExperimentTaskDto) => {
  Object.assign(form, emptyForm(), row || {})
  dialogVisible.value = true
}

const save = async () => {
  const payload: any = { ...form }
  if (payload.id) {
    await experimentApi.updateTask(payload.id, payload)
  } else {
    delete payload.id
    await experimentApi.createTask(payload)
  }
  ElMessage.success('保存成功')
  dialogVisible.value = false
  loadList()
}

const remove = async (id: string) => {
  await experimentApi.deleteTask(id)
  ElMessage.success('删除成功')
  loadList()
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

