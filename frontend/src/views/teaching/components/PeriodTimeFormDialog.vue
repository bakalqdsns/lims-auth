<template>
  <el-dialog
    :title="isEdit ? '编辑节次' : '新增节次'"
    v-model="visible"
    width="500px"
    destroy-on-close
  >
    <el-form
      ref="formRef"
      :model="form"
      :rules="rules"
      label-width="100px"
    >
      <el-form-item label="节次编号" prop="periodNumber">
        <el-input-number v-model="form.periodNumber" :min="1" :max="20" style="width: 100%" />
      </el-form-item>

      <el-form-item label="节次名称" prop="name">
        <el-input v-model="form.name" placeholder="如：第1-2节" />
      </el-form-item>

      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="开始时间" prop="startTime">
            <el-time-picker
              v-model="startTimeObj"
              format="HH:mm"
              value-format="HH:mm"
              placeholder="开始时间"
              style="width: 100%"
            />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="结束时间" prop="endTime">
            <el-time-picker
              v-model="endTimeObj"
              format="HH:mm"
              value-format="HH:mm"
              placeholder="结束时间"
              style="width: 100%"
            />
          </el-form-item>
        </el-col>
      </el-row>

      <el-form-item label="时长(分钟)" prop="duration">
        <el-input-number v-model="form.duration" :min="1" :max="300" style="width: 100%" />
      </el-form-item>

      <el-form-item label="说明" prop="description">
        <el-input
          v-model="form.description"
          type="textarea"
          :rows="2"
          placeholder="请输入说明"
        />
      </el-form-item>
    </el-form>

    <template #footer>
      <el-button @click="visible = false">取消</el-button>
      <el-button type="primary" :loading="submitting" @click="handleSubmit">
        确定
      </el-button>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, reactive, computed, watch } from 'vue'
import { ElMessage, type FormInstance, type FormRules } from 'element-plus'
import { periodTimeApi } from '../../../api/teaching'

interface Props {
  modelValue: boolean
  period?: {
    id: string
    periodNumber: number
    name: string
    startTime: string
    endTime: string
    duration: number
    description?: string
  }
}

const props = defineProps<Props>()
const emit = defineEmits(['update:modelValue', 'success'])

const visible = computed({
  get: () => props.modelValue,
  set: (val) => emit('update:modelValue', val)
})

const isEdit = computed(() => !!props.period)

const formRef = ref<FormInstance>()
const submitting = ref(false)
const startTimeObj = ref<any>(null)
const endTimeObj = ref<any>(null)

const form = reactive({
  periodNumber: 1,
  name: '',
  startTime: '',
  endTime: '',
  duration: 45,
  description: ''
})

const rules: FormRules = {
  periodNumber: [
    { required: true, message: '请输入节次编号', trigger: 'blur' }
  ],
  name: [
    { required: true, message: '请输入节次名称', trigger: 'blur' }
  ],
  startTime: [
    { required: true, message: '请选择开始时间', trigger: 'change' }
  ],
  endTime: [
    { required: true, message: '请选择结束时间', trigger: 'change' }
  ],
  duration: [
    { required: true, message: '请输入时长', trigger: 'blur' }
  ]
}

const handleSubmit = async () => {
  // 同步时间选择器的值到表单
  form.startTime = startTimeObj.value || ''
  form.endTime = endTimeObj.value || ''

  const valid = await formRef.value?.validate().catch(() => false)
  if (!valid) return

  submitting.value = true
  try {
    const api = isEdit.value
      ? () => periodTimeApi.update(props.period!.id, form)
      : () => periodTimeApi.create(form)

    const res = await api()
    if (res.data.code === 200) {
      ElMessage.success(isEdit.value ? '编辑成功' : '创建成功')
      visible.value = false
      emit('success')
    } else {
      ElMessage.error(res.data.message)
    }
  } catch (error) {
    ElMessage.error(isEdit.value ? '编辑失败' : '创建失败')
  } finally {
    submitting.value = false
  }
}

watch(() => props.period, (val) => {
  if (val) {
    form.periodNumber = val.periodNumber
    form.name = val.name
    form.startTime = val.startTime
    form.endTime = val.endTime
    form.duration = val.duration
    form.description = val.description || ''
    // 设置时间选择器
    const today = new Date().toISOString().split('T')[0]
    startTimeObj.value = val.startTime ? new Date(`${today}T${val.startTime}`) : null
    endTimeObj.value = val.endTime ? new Date(`${today}T${val.endTime}`) : null
  } else {
    form.periodNumber = 1
    form.name = ''
    form.startTime = ''
    form.endTime = ''
    form.duration = 45
    form.description = ''
    startTimeObj.value = null
    endTimeObj.value = null
  }
}, { immediate: true })
</script>
