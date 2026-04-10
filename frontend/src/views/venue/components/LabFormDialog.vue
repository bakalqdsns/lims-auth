<template>
  <el-dialog :title="dialogTitle" v-model="visible" width="700px" destroy-on-close>
    <el-form ref="formRef" :model="form" :rules="rules" label-width="100px">
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="实验室代码" prop="code">
            <el-input v-model="form.code" placeholder="请输入实验室代码" :disabled="type === 'edit'" />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="实验室名称" prop="name">
            <el-input v-model="form.name" placeholder="请输入实验室名称" />
          </el-form-item>
        </el-col>
      </el-row>

      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="实验室类型" prop="labType">
            <el-select v-model="form.labType" placeholder="选择类型" style="width: 100%">
              <el-option v-for="type in LAB_TYPES" :key="type" :label="type" :value="type" />
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="安全等级" prop="safetyLevel">
            <el-select v-model="form.safetyLevel" placeholder="选择等级" style="width: 100%">
              <el-option v-for="level in SAFETY_LEVELS" :key="level" :label="level" :value="level" />
            </el-select>
          </el-form-item>
        </el-col>
      </el-row>

      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="所属部门">
            <el-select v-model="form.departmentId" placeholder="选择部门" clearable style="width: 100%">
              <el-option v-for="dept in departments" :key="dept.id" :label="dept.name" :value="dept.id" />
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="负责人">
            <el-select v-model="form.managerId" placeholder="选择负责人" clearable filterable style="width: 100%">
              <el-option v-for="user in managers" :key="user.id" :label="user.fullName || user.username" :value="user.id" />
            </el-select>
          </el-form-item>
        </el-col>
      </el-row>

      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="地点">
            <el-input v-model="form.location" placeholder="请输入地点" />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="容纳人数">
            <el-input-number v-model="form.capacity" :min="0" :max="500" style="width: 100%" />
          </el-form-item>
        </el-col>
      </el-row>

      <el-form-item label="描述">
        <el-input v-model="form.description" type="textarea" :rows="3" placeholder="请输入描述" />
      </el-form-item>
    </el-form>

    <template #footer>
      <el-button @click="visible = false">取消</el-button>
      <el-button type="primary" :loading="submitting" @click="handleSubmit">确定</el-button>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, reactive, computed, watch, onMounted } from 'vue'
import { ElMessage, type FormInstance, type FormRules } from 'element-plus'
import { labApi, type LabDto, LAB_TYPES, SAFETY_LEVELS } from '../../../api/lab'
import { departmentApi, userApi } from '../../../api/system'

interface Props {
  modelValue: boolean
  type: 'create' | 'edit'
  labData: LabDto | null
}

const props = defineProps<Props>()
const emit = defineEmits(['update:modelValue', 'success'])

const visible = computed({
  get: () => props.modelValue,
  set: (val) => emit('update:modelValue', val)
})

const dialogTitle = computed(() => props.type === 'create' ? '新增实验室' : '编辑实验室')

const formRef = ref<FormInstance>()
const submitting = ref(false)
const departments = ref<{ id: string; name: string }[]>([])
const managers = ref<{ id: string; fullName?: string; username: string }[]>([])

const form = reactive({
  code: '',
  name: '',
  labType: '普通实验室',
  safetyLevel: '一般',
  departmentId: undefined as string | undefined,
  managerId: undefined as string | undefined,
  location: '',
  capacity: 0,
  description: ''
})

const rules: FormRules = {
  code: [{ required: true, message: '请输入实验室代码', trigger: 'blur' }],
  name: [{ required: true, message: '请输入实验室名称', trigger: 'blur' }],
  labType: [{ required: true, message: '请选择实验室类型', trigger: 'change' }],
  safetyLevel: [{ required: true, message: '请选择安全等级', trigger: 'change' }]
}

const fetchDepartments = async () => {
  try {
    const res = await departmentApi.getAllDepartments()
    if (res.data.code === 200) {
      departments.value = res.data.data
    }
  } catch (error) {
    console.error('获取部门列表失败', error)
  }
}

const fetchManagers = async () => {
  try {
    const res = await userApi.getUsers({ page: 1, pageSize: 1000 })
    if (res.data.code === 200) {
      managers.value = res.data.data.items.filter((u: any) =>
        u.roles?.some((r: any) => r.code === 'teacher' || r.code === 'super_admin' || r.code === 'lab_admin')
      )
    }
  } catch (error) {
    console.error('获取负责人列表失败', error)
  }
}

const resetForm = () => {
  form.code = ''
  form.name = ''
  form.labType = '普通实验室'
  form.safetyLevel = '一般'
  form.departmentId = undefined
  form.managerId = undefined
  form.location = ''
  form.capacity = 0
  form.description = ''
}

const fillForm = () => {
  if (props.labData) {
    form.code = props.labData.code
    form.name = props.labData.name
    form.labType = props.labData.labType
    form.safetyLevel = props.labData.safetyLevel
    form.departmentId = props.labData.departmentId
    form.managerId = props.labData.managerId
    form.location = props.labData.location || ''
    form.capacity = props.labData.capacity
    form.description = props.labData.description || ''
  }
}

const handleSubmit = async () => {
  const valid = await formRef.value?.validate().catch(() => false)
  if (!valid) return

  submitting.value = true
  try {
    const data = {
      code: form.code,
      name: form.name,
      labType: form.labType,
      safetyLevel: form.safetyLevel,
      departmentId: form.departmentId,
      managerId: form.managerId,
      location: form.location || undefined,
      capacity: form.capacity,
      description: form.description || undefined
    }

    let res
    if (props.type === 'create') {
      res = await labApi.create(data)
    } else {
      res = await labApi.update(props.labData!.id, data)
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
    fetchDepartments()
    fetchManagers()
  }
})

onMounted(() => {
  fetchDepartments()
  fetchManagers()
})
</script>
