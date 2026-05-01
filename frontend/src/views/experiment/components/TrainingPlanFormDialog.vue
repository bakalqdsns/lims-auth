<template>
  <el-dialog
    :title="isEdit ? '编辑实训教学计划' : '新增实训教学计划'"
    v-model="visible"
    width="900px"
    destroy-on-close
  >
    <el-form ref="formRef" :model="form" label-width="140px">
      <el-row :gutter="12">
        <el-col :span="12">
          <el-form-item label="学期" prop="semesterId">
            <el-select v-if="optionsReady" v-model="form.semesterId" placeholder="请选择学期" style="width: 100%">
              <el-option v-for="s in semesters" :key="s.id" :label="s.name" :value="s.id" />
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="课程" prop="courseId">
            <el-select v-if="optionsReady" v-model="form.courseId" placeholder="请选择课程" style="width: 100%">
              <el-option v-for="c in courses" :key="c.id" :label="`${c.code} - ${c.name}`" :value="c.id" />
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="专业">
            <el-select v-if="optionsReady" v-model="form.majorId" clearable placeholder="请选择专业" style="width: 100%">
              <el-option v-for="m in majors" :key="m.id" :label="m.name" :value="m.id" />
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="班级">
            <el-select v-if="optionsReady" v-model="form.classId" clearable placeholder="请选择班级" style="width: 100%">
              <el-option v-for="c in classes" :key="c.id" :label="c.name" :value="c.id" />
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :span="8">
          <el-form-item label="学生人数">
            <el-input-number v-model="form.studentCount" :min="0" style="width: 100%" />
          </el-form-item>
        </el-col>
        <el-col :span="8">
          <el-form-item label="学生层次">
            <el-select v-model="form.studentLevel" clearable placeholder="请选择层次" style="width: 100%">
              <el-option label="本科" value="本科" />
              <el-option label="专科" value="专科" />
              <el-option label="研究生" value="研究生" />
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :span="8">
          <el-form-item label="组织方式">
            <el-select v-model="form.teachingOrganizationMethod" clearable placeholder="请选择" style="width: 100%">
              <el-option label="校内集中" value="校内集中" />
              <el-option label="校内分散" value="校内分散" />
              <el-option label="校外实习" value="校外实习" />
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :span="24">
          <el-form-item label="教学地点"><el-input v-model="form.teachingLocation" /></el-form-item>
        </el-col>
        <el-col :span="24">
          <el-form-item label="教学目的要求"><el-input v-model="form.teachingPurpose" type="textarea" :rows="2" /></el-form-item>
        </el-col>
        <el-col :span="24">
          <el-form-item label="教学内容安排"><el-input v-model="form.teachingContent" type="textarea" :rows="2" /></el-form-item>
        </el-col>
        <el-col :span="24">
          <el-form-item label="教学进度安排"><el-input v-model="form.teachingProgressSchedule" type="textarea" :rows="2" /></el-form-item>
        </el-col>
        <el-col :span="24">
          <el-form-item label="实训方法"><el-input v-model="form.trainingMethod" type="textarea" :rows="2" /></el-form-item>
        </el-col>
        <el-col :span="24">
          <el-form-item label="轮组信息"><el-input v-model="form.cycleGroupInfo" type="textarea" :rows="2" /></el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="考核方式"><el-input v-model="form.assessmentMethod" /></el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="考核要求"><el-input v-model="form.assessmentRequirements" type="textarea" :rows="2" /></el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="质量保障措施"><el-input v-model="form.qualityAssuranceMeasures" type="textarea" :rows="2" /></el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="质量保障细则"><el-input v-model="form.qualityAssuranceDetails" type="textarea" :rows="2" /></el-form-item>
        </el-col>
        <el-col :span="24">
          <el-form-item label="实验中心意见"><el-input v-model="form.experimentCenterOpinion" /></el-form-item>
        </el-col>
        <el-col :span="24">
          <el-form-item label="院系意见"><el-input v-model="form.departmentOpinion" /></el-form-item>
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
import { courseApi, semesterApi, majorApi, classApi } from '@/api/teaching'
import type { CourseDto, SemesterDto, MajorDto, ClassDto } from '@/api/teaching'
import { experimentApi, type TrainingPlanDto } from '@/api/experiment'

const props = defineProps<{ modelValue: boolean; plan?: TrainingPlanDto }>()
const emit = defineEmits(['update:modelValue', 'success'])

const visible = computed({
  get: () => props.modelValue,
  set: (val) => emit('update:modelValue', val)
})
const isEdit = computed(() => !!props.plan)

const formRef = ref<FormInstance>()
const submitting = ref(false)
const optionsReady = ref(false)
const semesters = ref<SemesterDto[]>([])
const courses = ref<CourseDto[]>([])
const majors = ref<MajorDto[]>([])
const classes = ref<ClassDto[]>([])

const form = reactive({
  semesterId: '', courseId: '', majorId: '', classId: '',
  studentCount: 0, studentLevel: '',
  teachingOrganizationMethod: '', teachingLocation: '',
  teachingPurpose: '', teachingContent: '', teachingProgressSchedule: '',
  trainingMethod: '', cycleGroupInfo: '',
  assessmentMethod: '', assessmentRequirements: '',
  qualityAssuranceMeasures: '', qualityAssuranceDetails: '',
  experimentCenterOpinion: '', departmentOpinion: '',
  status: 'Active', sortOrder: 0, description: ''
})

const rules: FormRules = {
  semesterId: [{ required: true, message: '请选择学期', trigger: 'change' }],
  courseId: [{ required: true, message: '请选择课程', trigger: 'change' }]
}

const loadOptions = async () => {
  const [semRes, courseRes, majorRes, classRes] = await Promise.all([
    semesterApi.getList(),
    courseApi.getList({ page: 1, pageSize: 999 }),
    majorApi.getList({ page: 1, pageSize: 999 }),
    classApi.getList({ page: 1, pageSize: 999 })
  ])
  semesters.value = semRes.data.data || []
  courses.value = courseRes.data.data || []
  majors.value = majorRes.data.data || []
  classes.value = classRes.data.data || []
  optionsReady.value = true
}

const handleSubmit = async () => {
  const valid = await formRef.value?.validate().catch(() => false)
  if (!valid) return
  submitting.value = true
  try {
    const payload: any = { ...form }
    if (isEdit.value) await experimentApi.updateTrainingPlan(props.plan!.id, payload)
    else await experimentApi.createTrainingPlan(payload)
    ElMessage.success('保存成功')
    visible.value = false
    emit('success')
  } catch {
    ElMessage.error('保存失败')
  } finally {
    submitting.value = false
  }
}

watch(() => props.plan, (val) => {
  const reset = {
    semesterId: '', courseId: '', majorId: '', classId: '',
    studentCount: 0, studentLevel: '',
    teachingOrganizationMethod: '', teachingLocation: '',
    teachingPurpose: '', teachingContent: '', teachingProgressSchedule: '',
    trainingMethod: '', cycleGroupInfo: '',
    assessmentMethod: '', assessmentRequirements: '',
    qualityAssuranceMeasures: '', qualityAssuranceDetails: '',
    experimentCenterOpinion: '', departmentOpinion: '',
    status: 'Active', sortOrder: 0, description: ''
  }
  Object.assign(form, reset, val || {})
}, { immediate: true })

onMounted(loadOptions)
</script>
