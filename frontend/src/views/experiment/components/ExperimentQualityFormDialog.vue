<template>
  <el-dialog
    :title="isEdit ? '编辑实验课程教学质量' : '新增实验课程教学质量'"
    v-model="visible"
    width="760px"
    destroy-on-close
  >
    <el-form ref="formRef" :model="form" label-width="130px">
      <el-row :gutter="12">
        <el-col :span="12">
          <el-form-item label="教学任务" prop="experimentTaskId">
            <el-select v-if="optionsReady" v-model="form.experimentTaskId" placeholder="请选择教学任务" style="width: 100%">
              <el-option v-for="t in tasks" :key="t.id" :label="t.courseName" :value="t.id" />
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="课程名称"><el-input v-model="form.courseName" /></el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="实验课时"><el-input-number v-model="form.experimentHours" :min="0" style="width: 100%" /></el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="独立设课"><el-switch v-model="form.isIndependentCourse" /></el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="主讲教师"><el-input v-model="form.mainTeacher" /></el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="教师职称"><el-input v-model="form.teacherTitle" /></el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="实验技术人员"><el-input v-model="form.technicalStaff" /></el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="技术人员职称"><el-input v-model="form.technicalTitle" /></el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="授课班级"><el-input v-model="form.className" /></el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="班级人数"><el-input-number v-model="form.classStudentCount" :min="0" style="width: 100%" /></el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="计划实验个数"><el-input-number v-model="form.plannedExperimentCount" :min="0" style="width: 100%" /></el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="实际实验个数"><el-input-number v-model="form.actualExperimentCount" :min="0" style="width: 100%" /></el-form-item>
        </el-col>
        <el-col :span="24">
          <el-form-item label="未做实验项目"><el-input v-model="form.missedExperimentItems" type="textarea" :rows="2" /></el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="考核方式"><el-input v-model="form.assessmentMethod" /></el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="考核人数"><el-input-number v-model="form.assessmentStudentCount" :min="0" style="width: 100%" /></el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="考核时间"><el-input v-model="form.assessmentTime" placeholder="如：第18周" /></el-form-item>
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
import { ElMessage, type FormInstance } from 'element-plus'
import { experimentApi, type ExperimentQualityDto, type ExperimentTaskDto } from '@/api/experiment'

const props = defineProps<{ modelValue: boolean; quality?: ExperimentQualityDto }>()
const emit = defineEmits(['update:modelValue', 'success'])

const visible = computed({
  get: () => props.modelValue,
  set: (val) => emit('update:modelValue', val)
})
const isEdit = computed(() => !!props.quality)

const formRef = ref<FormInstance>()
const submitting = ref(false)
const optionsReady = ref(false)
const tasks = ref<ExperimentTaskDto[]>([])

const form = reactive({
  experimentTaskId: '',
  courseName: '', experimentHours: 0, isIndependentCourse: false,
  mainTeacher: '', teacherTitle: '', technicalStaff: '', technicalTitle: '',
  className: '', classStudentCount: 0,
  plannedExperimentCount: 0, actualExperimentCount: 0,
  missedExperimentItems: '', assessmentMethod: '',
  assessmentStudentCount: 0, assessmentTime: '',
  institutionId: undefined as string | undefined,
  status: 'Active', sortOrder: 0, description: ''
})

const loadTasks = async () => {
  const res = await experimentApi.getTasks()
  tasks.value = res.data.data || []
  optionsReady.value = true
}

const handleSubmit = async () => {
  const valid = await formRef.value?.validate().catch(() => false)
  if (!valid) return
  submitting.value = true
  try {
    const payload: any = { ...form }
    if (isEdit.value) await experimentApi.updateQuality(props.quality!.id, payload)
    else await experimentApi.createQuality(payload)
    ElMessage.success('保存成功')
    visible.value = false
    emit('success')
  } catch {
    ElMessage.error('保存失败')
  } finally {
    submitting.value = false
  }
}

watch(() => props.quality, (val) => {
  const reset = {
    experimentTaskId: '',
    courseName: '', experimentHours: 0, isIndependentCourse: false,
    mainTeacher: '', teacherTitle: '', technicalStaff: '', technicalTitle: '',
    className: '', classStudentCount: 0,
    plannedExperimentCount: 0, actualExperimentCount: 0,
    missedExperimentItems: '', assessmentMethod: '',
    assessmentStudentCount: 0, assessmentTime: '',
    institutionId: undefined as string | undefined,
    status: 'Active', sortOrder: 0, description: ''
  }
  Object.assign(form, reset, val || {})
}, { immediate: true })

onMounted(loadTasks)
</script>
