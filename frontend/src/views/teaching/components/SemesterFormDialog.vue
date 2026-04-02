<template>
  <el-dialog :title="dialogTitle" v-model="visible" width="500px" destroy-on-close>
    <el-form ref="formRef" :model="form" :rules="rules" label-width="100px">
      <el-form-item label="学期名称" prop="name">
        <el-input v-model="form.name" placeholder="如：2024-2025学年第一学期" />
      </el-form-item>
      <el-form-item label="开始日期" prop="startDate">
        <el-date-picker v-model="form.startDate" type="date" placeholder="选择开始日期" style="width: 100%" />
      </el-form-item>
      <el-form-item label="结束日期" prop="endDate">
        <el-date-picker v-model="form.endDate" type="date" placeholder="选择结束日期" style="width: 100%" />
      </el-form-item>
      <el-form-item label="当前学期" v-if="type === 'create'">
        <el-switch v-model="form.isCurrent" />
      </el-form-item>
    </el-form>
    <template #footer>
      <el-button @click="visible = false">取消</el-button>
      <el-button type="primary" :loading="submitting" @click="handleSubmit">确定</el-button>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, reactive, computed, watch } from 'vue'
import { ElMessage, type FormInstance, type FormRules } from 'element-plus'
import { semesterApi, type SemesterDto } from '@/api/teaching'

interface Props {
  modelValue: boolean
  type: 'create' | 'edit'
  semesterData: SemesterDto | null
}

const props = defineProps<Props>()
const emit = defineEmits(['update:modelValue', 'success'])

const visible = computed({
  get: () => props.modelValue,
  set: (val) => emit('update:modelValue', val)
})

const dialogTitle = computed(() => props.type === 'create' ? '新增学期' : '编辑学期')

const formRef = ref<FormInstance>()
const submitting = ref(false)

const form = reactive({
  name: '',
  startDate: '' as string | Date,
  endDate: '' as string | Date,
  isCurrent: false
})

const rules: FormRules = {
  name: [{ required: true, message: '请输入学期名称', trigger: 'blur' }],
  startDate: [{ required: true, message: '请选择开始日期', trigger: 'change' }],
  endDate: [{ required: true, message: '请选择结束日期', trigger: 'change' }]
}

const resetForm = () => {
  form.name = ''
  form.startDate = ''
  form.endDate = ''
  form.isCurrent = false
}

const fillForm = () => {
  if (props.semesterData) {
    form.name = props.semesterData.name
    form.startDate = props.semesterData.startDate
    form.endDate = props.semesterData.endDate
    form.isCurrent = props.semesterData.isCurrent
  }
}

const handleSubmit = async () => {
  const valid = await formRef.value?.validate().catch(() => false)
  if (!valid) return

  submitting.value = true
  try {
    const data = {
      name: form.name,
      startDate: typeof form.startDate === 'object' ? form.startDate.toISOString().split('T')[0] : form.startDate,
      endDate: typeof form.endDate === 'object' ? form.endDate.toISOString().split('T')[0] : form.endDate,
      isCurrent: form.isCurrent
    }

    let res
    if (props.type === 'create') {
      res = await semesterApi.create(data)
    } else {
      res = await semesterApi.update(props.semesterData!.id, data)
    }

    if (res.data.code === 200) {
      ElMessage.success(props.type === 'create' ? '创建成功' : '更新成功')
      visible.value = false
      emit('success')
    } else {
      ElMessage.error(res.data.message)
    }
  } catch (error) {
    ElMessage.error(props.type === 'create' ? '创建失败' : '更新失败')
  } finally {
    submitting.value = false
  }
}

watch(() => props.modelValue, (val) => {
  if (val) {
    if (props.type === 'create') {
      resetForm()
    } else {
      fillForm()
    }
  }
})
</script>
