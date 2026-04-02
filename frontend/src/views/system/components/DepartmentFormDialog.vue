<template>
  <el-dialog
    :title="dialogTitle"
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
      <el-form-item label="上级部门">
        <el-input
          v-if="parentDepartment"
          :model-value="parentDepartment.name"
          disabled
        />
        <el-input
          v-else-if="type === 'edit' && currentDepartment?.parentName"
          :model-value="currentDepartment?.parentName"
          disabled
        />
        <el-input
          v-else
          model-value="无(顶级部门)"
          disabled
        />
      </el-form-item>

      <el-form-item label="部门编码" prop="code">
        <el-input
          v-model="form.code"
          placeholder="请输入部门编码"
          :disabled="type === 'edit'"
        />
      </el-form-item>

      <el-form-item label="部门名称" prop="name">
        <el-input
          v-model="form.name"
          placeholder="请输入部门名称"
        />
      </el-form-item>

      <el-form-item label="负责人" prop="managerId">
        <el-select
          v-model="form.managerId"
          placeholder="选择负责人"
          clearable
          filterable
          style="width: 100%"
        >
          <el-option
            v-for="user in users"
            :key="user.id"
            :label="user.fullName || user.username"
            :value="user.id"
          />
        </el-select>
      </el-form-item>

      <el-form-item label="描述" prop="description">
        <el-input
          v-model="form.description"
          type="textarea"
          :rows="3"
          placeholder="请输入部门描述"
        />
      </el-form-item>

      <el-form-item v-if="type === 'edit'" label="状态" prop="isActive">
        <el-switch
          v-model="form.isActive"
          active-text="启用"
          inactive-text="禁用"
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
import { departmentApi, userApi, type DepartmentDto } from '../../../api/system'

interface UserOption {
  id: string
  username: string
  fullName?: string
}

interface Props {
  modelValue: boolean
  type: 'create' | 'edit'
  departmentData: DepartmentDto | null
  parentDepartment: DepartmentDto | null
}

const props = defineProps<Props>()
const emit = defineEmits(['update:modelValue', 'success'])

const visible = computed({
  get: () => props.modelValue,
  set: (val) => emit('update:modelValue', val)
})

const dialogTitle = computed(() => props.type === 'create' ? '新增部门' : '编辑部门')
const currentDepartment = computed(() => props.departmentData)

const formRef = ref<FormInstance>()
const submitting = ref(false)
const users = ref<UserOption[]>([])

const form = reactive({
  code: '',
  name: '',
  managerId: undefined as string | undefined,
  description: '',
  isActive: true
})

const rules: FormRules = {
  code: [
    { required: true, message: '请输入部门编码', trigger: 'blur' },
    { min: 2, max: 50, message: '长度在 2 到 50 个字符', trigger: 'blur' }
  ],
  name: [
    { required: true, message: '请输入部门名称', trigger: 'blur' },
    { min: 2, max: 100, message: '长度在 2 到 100 个字符', trigger: 'blur' }
  ]
}

// 获取用户列表(用于选择负责人)
const fetchUsers = async () => {
  try {
    const res = await userApi.getUsers({ pageSize: 1000 })
    if (res.data.code === 200) {
      users.value = res.data.data.items.map(u => ({
        id: u.id,
        username: u.username,
        fullName: u.fullName
      }))
    }
  } catch (error) {
    console.error('获取用户列表失败', error)
  }
}

// 重置表单
const resetForm = () => {
  form.code = ''
  form.name = ''
  form.managerId = undefined
  form.description = ''
  form.isActive = true
}

// 填充表单
const fillForm = () => {
  if (props.departmentData) {
    form.code = props.departmentData.code
    form.name = props.departmentData.name
    form.managerId = props.departmentData.managerId
    form.description = props.departmentData.description || ''
    form.isActive = props.departmentData.isActive
  }
}

// 提交
const handleSubmit = async () => {
  const valid = await formRef.value?.validate().catch(() => false)
  if (!valid) return

  submitting.value = true
  try {
    let res
    if (props.type === 'create') {
      res = await departmentApi.createDepartment({
        code: form.code,
        name: form.name,
        parentId: props.parentDepartment?.id,
        managerId: form.managerId,
        description: form.description || undefined
      })
    } else {
      res = await departmentApi.updateDepartment(props.departmentData!.id, {
        name: form.name,
        managerId: form.managerId,
        description: form.description || undefined,
        isActive: form.isActive
      })
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
    fetchUsers()
  }
})

onMounted(() => {
  fetchUsers()
})
</script>
