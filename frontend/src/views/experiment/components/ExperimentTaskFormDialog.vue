<template>
  <el-dialog
    :title="isEdit ? '编辑实验教学任务' : '新增实验教学任务'"
    v-model="visible"
    width="760px"
    destroy-on-close
  >
    <el-form ref="formRef" :model="form" label-width="130px">
      <el-row :gutter="12">
        <el-col :span="12">
          <el-form-item label="学期" prop="semesterId">
            <el-select v-if="optionsReady" v-model="form.semesterId" placeholder="请选择学期" style="width: 100%">
              <el-option v-for="s in semesterOptions" :key="s.id" :label="s.name" :value="s.id" />
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="专业" prop="majorId">
            <el-select v-if="optionsReady" v-model="form.majorId" placeholder="请选择专业" style="width: 100%">
              <el-option v-for="m in majorOptions" :key="m.id" :label="m.name" :value="m.id" />
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="班级" prop="classId">
            <el-select v-if="optionsReady" v-model="form.classId" placeholder="请选择班级" style="width: 100%">
              <el-option v-for="c in classOptions" :key="c.id" :label="c.name" :value="c.id" />
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="课程名称" prop="courseName">
            <el-input v-model="form.courseName" placeholder="请输入课程名称" />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="课程类型">
            <el-select v-model="form.courseType" clearable placeholder="请选择" style="width: 100%">
              <el-option label="必修课" value="必修课" />
              <el-option label="选修课" value="选修课" />
              <el-option label="专业课" value="专业课" />
              <el-option label="专业核心课" value="专业核心课" />
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="独立设课">
            <el-switch v-model="form.isIndependentCourse" />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="学生人数"><el-input-number v-model="form.studentCount" :min="0" style="width: 100%" /></el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="学生层次">
            <el-select v-model="form.studentLevel" clearable placeholder="请选择" style="width: 100%">
              <el-option label="专科" value="专科" />
              <el-option label="本科" value="本科" />
              <el-option label="研究生" value="研究生" />
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="实验总学时"><el-input-number v-model="form.totalExperimentHours" :min="0" style="width: 100%" /></el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="本学期实验"><el-input-number v-model="form.currentSemesterExperimentHours" :min="0" style="width: 100%" /></el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="实践总学时"><el-input-number v-model="form.totalPracticeHours" :min="0" style="width: 100%" /></el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="本学期实践"><el-input-number v-model="form.currentSemesterPracticeHours" :min="0" style="width: 100%" /></el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="实训总学时"><el-input-number v-model="form.totalTrainingHours" :min="0" style="width: 100%" /></el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="本学期实训"><el-input-number v-model="form.currentSemesterTrainingHours" :min="0" style="width: 100%" /></el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="主讲教师"><el-input v-model="form.teacherNames" /></el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="教师职称"><el-input v-model="form.teacherTitles" /></el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="实验技术人员"><el-input v-model="form.technicalStaff" /></el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="技术人员职称"><el-input v-model="form.technicalTitle" /></el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="教材"><el-input v-model="form.textbookName" /></el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="实验指导书"><el-input v-model="form.experimentGuideName" /></el-form-item>
        </el-col>
        <el-col :span="24">
          <el-form-item label="备注"><el-input v-model="form.description" type="textarea" :rows="2" /></el-form-item>
        </el-col>
      </el-row>
    </el-form>

    <template #footer>
      <el-button @click="visible = false">取消</el-button>
      <el-button type="primary" :loading="submitting" @click="handleSubmit">保存</el-button>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, reactive, computed, watch, onMounted } from 'vue'
import { ElMessage, type FormInstance, type FormRules } from 'element-plus'
import { classApi, majorApi, semesterApi, type ClassDto, type MajorDto, type SemesterDto } from '@/api/teaching'
import { experimentApi, type ExperimentTaskDto } from '@/api/experiment'

interface Props {
  modelValue: boolean
  task?: ExperimentTaskDto
}

const props = defineProps<Props>()
const emit = defineEmits(['update:modelValue', 'success'])

const visible = computed({
  get: () => props.modelValue,
  set: (val) => emit('update:modelValue', val)
})

const isEdit = computed(() => !!props.task)

const formRef = ref<FormInstance>()
const submitting = ref(false)
const optionsReady = ref(false)
const semesterOptions = ref<SemesterDto[]>([])
const majorOptions = ref<MajorDto[]>([])
const classOptions = ref<ClassDto[]>([])

const form = reactive({
  semesterId: undefined as string | undefined,
  majorId: undefined as string | undefined,
  classId: undefined as string | undefined,
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
  teacherNames: '',
  teacherTitles: '',
  technicalStaff: '',
  technicalTitle: '',
  textbookName: '',
  experimentGuideName: '',
  status: 'Active',
  sortOrder: 0,
  description: ''
})

const rules: FormRules = {
  semesterId: [{ required: true, message: '请选择学期', trigger: 'change' }],
  majorId: [{ required: true, message: '请选择专业', trigger: 'change' }],
  classId: [{ required: true, message: '请选择班级', trigger: 'change' }],
  courseName: [{ required: true, message: '请输入课程名称', trigger: 'blur' }]
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

const handleSubmit = async () => {
  const valid = await formRef.value?.validate().catch(() => false)
  if (!valid) return

  submitting.value = true
  try {
    const payload: any = { ...form }
    if (isEdit.value) {
      await experimentApi.updateTask(props.task!.id, payload)
    } else {
      await experimentApi.createTask(payload)
    }
    ElMessage.success('保存成功')
    visible.value = false
    emit('success')
  } catch {
    ElMessage.error('保存失败')
  } finally {
    submitting.value = false
  }
}

watch(() => props.task, (val) => {
  const reset = {
    semesterId: undefined as string | undefined,
    majorId: undefined as string | undefined,
    classId: undefined as string | undefined,
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
    teacherNames: '',
    teacherTitles: '',
    technicalStaff: '',
    technicalTitle: '',
    textbookName: '',
    experimentGuideName: '',
    status: 'Active',
    sortOrder: 0,
    description: ''
  }
  if (val) {
    Object.assign(form, reset, {
      ...val,
      semesterId: val.semester?.id,
      majorId: val.major?.id,
      classId: val.class?.id
    })
  } else {
    Object.assign(form, reset)
  }
}, { immediate: true })

onMounted(loadOptions)
</script>
