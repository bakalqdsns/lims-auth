<template>
  <el-dialog
    :title="isEdit ? '编辑实验开出记录' : '新增实验开出记录'"
    v-model="visible"
    width="760px"
    destroy-on-close
  >
    <el-form ref="formRef" :model="form" label-width="120px">
      <el-row :gutter="12">
        <el-col :span="12">
          <el-form-item label="教学任务" prop="experimentTaskId">
            <el-select v-if="optionsReady" v-model="form.experimentTaskId" placeholder="请选择教学任务" style="width: 100%">
              <el-option v-for="t in tasks" :key="t.id" :label="t.courseName" :value="t.id" />
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="实验项目" prop="experimentItemId">
            <el-select v-if="optionsReady" v-model="form.experimentItemId" placeholder="请选择实验项目" style="width: 100%">
              <el-option v-for="i in items" :key="i.id" :label="i.experimentName" :value="i.id" />
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :span="8">
          <el-form-item label="周次" prop="weekNumber">
            <el-input-number v-model="form.weekNumber" :min="1" style="width: 100%" />
          </el-form-item>
        </el-col>
        <el-col :span="8">
          <el-form-item label="星期" prop="dayOfWeek">
            <el-input-number v-model="form.dayOfWeek" :min="1" :max="7" style="width: 100%" />
          </el-form-item>
        </el-col>
        <el-col :span="8">
          <el-form-item label="节次" prop="periodNumber">
            <el-input-number v-model="form.periodNumber" :min="1" style="width: 100%" />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="实验室">
            <el-select v-if="optionsReady" v-model="form.labId" clearable style="width: 100%">
              <el-option v-for="l in labs" :key="l.id" :label="l.name" :value="l.id" />
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="地点文本">
            <el-input v-model="form.location" />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="是否开出">
            <el-switch v-model="form.isConducted" />
          </el-form-item>
        </el-col>
        <el-col :span="24">
          <el-form-item label="未开出原因">
            <el-input v-model="form.reasonIfNotConducted" type="textarea" :rows="2" />
          </el-form-item>
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
import { labApi, type LabDto } from '@/api/lab'
import { experimentApi, type ExperimentItemDto, type ExperimentScheduleDto, type ExperimentTaskDto } from '@/api/experiment'

const props = defineProps<{ modelValue: boolean; schedule?: ExperimentScheduleDto }>()
const emit = defineEmits(['update:modelValue', 'success'])

const visible = computed({
  get: () => props.modelValue,
  set: (val) => emit('update:modelValue', val)
})
const isEdit = computed(() => !!props.schedule)

const formRef = ref<FormInstance>()
const submitting = ref(false)
const optionsReady = ref(false)
const tasks = ref<ExperimentTaskDto[]>([])
const items = ref<ExperimentItemDto[]>([])
const labs = ref<LabDto[]>([])

const form = reactive({
  experimentTaskId: undefined as string | undefined,
  experimentItemId: undefined as string | undefined,
  weekNumber: 1, dayOfWeek: 1, periodNumber: 1,
  parallelGroups: 1, studentsPerGroup: 1, cycleCount: 1,
  experimentRequirement: '必做', location: '',
  labId: undefined as string | undefined,
  isConducted: true, reasonIfNotConducted: '',
  status: 'Active', sortOrder: 0, description: ''
})

const rules: FormRules = {
  experimentTaskId: [{ required: true, message: '请选择教学任务', trigger: 'change' }],
  experimentItemId: [{ required: true, message: '请选择实验项目', trigger: 'change' }],
  weekNumber: [{ required: true, message: '请填写周次', trigger: 'blur' }]
}

const loadOptions = async () => {
  const [taskRes, itemRes, labRes] = await Promise.all([
    experimentApi.getTasks(),
    experimentApi.getItems(),
    labApi.getList()
  ])
  tasks.value = taskRes.data.data || []
  items.value = itemRes.data.data || []
  labs.value = labRes.data.data || []
  optionsReady.value = true
}

const handleSubmit = async () => {
  const valid = await formRef.value?.validate().catch(() => false)
  if (!valid) return
  submitting.value = true
  try {
    const payload: any = { ...form }
    if (isEdit.value) await experimentApi.updateSchedule(props.schedule!.id, payload)
    else await experimentApi.createSchedule(payload)
    ElMessage.success('保存成功')
    visible.value = false
    emit('success')
  } catch {
    ElMessage.error('保存失败')
  } finally {
    submitting.value = false
  }
}

watch(() => props.schedule, (val) => {
  const reset = {
    experimentTaskId: undefined as string | undefined,
    experimentItemId: undefined as string | undefined,
    weekNumber: 1, dayOfWeek: 1, periodNumber: 1,
    parallelGroups: 1, studentsPerGroup: 1, cycleCount: 1,
    experimentRequirement: '必做', location: '',
    labId: undefined as string | undefined,
    isConducted: true, reasonIfNotConducted: '',
    status: 'Active', sortOrder: 0, description: ''
  }
  if (val) {
    Object.assign(form, reset, {
      ...val,
      experimentTaskId: val.experimentTask?.id,
      experimentItemId: val.experimentItem?.id
    })
  } else {
    Object.assign(form, reset)
  }
}, { immediate: true })

onMounted(loadOptions)
</script>
