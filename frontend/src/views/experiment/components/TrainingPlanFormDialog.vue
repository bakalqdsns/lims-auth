<template>
  <el-dialog
    :title="isEdit ? '编辑实训教学计划' : '新增实训教学计划'"
    v-model="visible"
    width="760px"
    destroy-on-close
  >
    <el-form ref="formRef" :model="form" label-width="140px">
      <el-form-item label="课程" prop="courseId">
        <el-select v-if="optionsReady" v-model="form.courseId" placeholder="请选择课程" style="width: 100%">
          <el-option v-for="c in courses" :key="c.id" :label="`${c.code} - ${c.name}`" :value="c.id" />
        </el-select>
      </el-form-item>
      <el-form-item label="组织方式"><el-input v-model="form.teachingOrganizationMethod" /></el-form-item>
      <el-form-item label="教学地点"><el-input v-model="form.teachingLocation" /></el-form-item>
      <el-form-item label="教学目的要求"><el-input v-model="form.teachingPurpose" type="textarea" :rows="2" /></el-form-item>
      <el-form-item label="教学内容安排"><el-input v-model="form.teachingContent" type="textarea" :rows="2" /></el-form-item>
      <el-form-item label="实训方法"><el-input v-model="form.trainingMethod" type="textarea" :rows="2" /></el-form-item>
      <el-form-item label="考核方式"><el-input v-model="form.assessmentMethod" /></el-form-item>
      <el-form-item label="质量保障措施"><el-input v-model="form.qualityAssuranceMeasures" type="textarea" :rows="2" /></el-form-item>
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
import { courseApi, type CourseDto } from '@/api/teaching'
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
const courses = ref<CourseDto[]>([])

const form = reactive({
  courseId: '', teachingOrganizationMethod: '', teachingLocation: '',
  teachingPurpose: '', teachingContent: '', trainingMethod: '',
  assessmentMethod: '', qualityAssuranceMeasures: '',
  experimentCenterOpinion: '', departmentOpinion: '',
  status: 'Active', sortOrder: 0, description: ''
})

const rules: FormRules = {
  courseId: [{ required: true, message: '请选择课程', trigger: 'change' }]
}

const loadCourses = async () => {
  const res = await courseApi.getList({ page: 1, pageSize: 999 })
  courses.value = res.data.data || []
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
    courseId: '', teachingOrganizationMethod: '', teachingLocation: '',
    teachingPurpose: '', teachingContent: '', trainingMethod: '',
    assessmentMethod: '', qualityAssuranceMeasures: '',
    experimentCenterOpinion: '', departmentOpinion: '',
    status: 'Active', sortOrder: 0, description: ''
  }
  Object.assign(form, reset, val || {})
}, { immediate: true })

onMounted(loadCourses)
</script>
