<template>
  <el-dialog
    :title="isEdit ? '编辑专业' : '新增专业'"
    v-model="visible"
    width="600px"
    destroy-on-close
  >
    <el-form
      ref="formRef"
      :model="form"
      :rules="rules"
      label-width="100px"
    >
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="专业代码" prop="code">
            <el-input v-model="form.code" placeholder="请输入专业代码" />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="专业名称" prop="name">
            <el-input v-model="form.name" placeholder="请输入专业名称" />
          </el-form-item>
        </el-col>
      </el-row>

      <el-form-item label="所属院系" prop="departmentId">
        <el-select v-model="form.departmentId" placeholder="请选择所属院系" style="width: 100%">
          <el-option
            v-for="dept in departments"
            :key="dept.id"
            :label="dept.name"
            :value="dept.id"
          />
        </el-select>
      </el-form-item>

      <el-form-item label="专业简介" prop="description">
        <el-input
          v-model="form.description"
          type="textarea"
          :rows="3"
          placeholder="请输入专业简介"
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
import { ref, reactive, computed, watch, onMounted } from 'vue'
import { ElMessage, type FormInstance, type FormRules } from 'element-plus'
import { majorApi } from '../../../api/teaching'
import { departmentApi } from '../../../api/system'

interface Props {
  modelValue: boolean
  major?: {
    id: string
    code: string
    name: string
    departmentId: string
    description?: string
  }
}

const props = defineProps<Props>()
const emit = defineEmits(['update:modelValue', 'success'])

const visible = computed({
  get: () => props.modelValue,
  set: (val) => emit('update:modelValue', val)
})

const isEdit = computed(() => !!props.major)

const formRef = ref<FormInstance>()
const submitting = ref(false)
const departments = ref<any[]>([])

const form = reactive({
  code: '',
  name: '',
  departmentId: '',
  description: ''
})

const rules: FormRules = {
  code: [
    { required: true, message: '请输入专业代码', trigger: 'blur' },
    { min: 2, max: 20, message: '长度在 2 到 20 个字符', trigger: 'blur' }
  ],
  name: [
    { required: true, message: '请输入专业名称', trigger: 'blur' },
    { min: 2, max: 100, message: '长度在 2 到 100 个字符', trigger: 'blur' }
  ],
  departmentId: [
    { required: true, message: '请选择所属院系', trigger: 'change' }
  ]
}

const loadDepartments = async () => {
  try {
    const res = await departmentApi.getAllDepartments()
    if (res.data.code === 200) {
      departments.value = res.data.data
    }
  } catch (error) {
    console.error('加载部门列表失败', error)
  }
}

const handleSubmit = async () => {
  const valid = await formRef.value?.validate().catch(() => false)
  if (!valid) return

  submitting.value = true
  try {
    const api = isEdit.value
      ? () => majorApi.update(props.major!.id, form)
      : () => majorApi.create(form)

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

watch(() => props.major, (val) => {
  if (val) {
    form.code = val.code
    form.name = val.name
    form.departmentId = val.departmentId
    form.description = val.description || ''
  } else {
    form.code = ''
    form.name = ''
    form.departmentId = ''
    form.description = ''
  }
}, { immediate: true })

onMounted(loadDepartments)
</script>
