<template>
  <el-dialog
    :title="dialogTitle"
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
      <el-form-item label="用户名" prop="username">
        <el-input
          v-model="form.username"
          placeholder="请输入用户名"
          :disabled="type === 'edit'"
        />
      </el-form-item>

      <el-form-item
        v-if="type === 'create'"
        label="密码"
        prop="password"
      >
        <el-input
          v-model="form.password"
          type="password"
          placeholder="请输入密码"
          show-password
        />
      </el-form-item>

      <el-form-item label="姓名" prop="fullName">
        <el-input
          v-model="form.fullName"
          placeholder="请输入姓名"
        />
      </el-form-item>

      <el-form-item label="邮箱" prop="email">
        <el-input
          v-model="form.email"
          placeholder="请输入邮箱"
        />
      </el-form-item>

      <el-form-item label="电话" prop="phone">
        <el-input
          v-model="form.phone"
          placeholder="请输入电话"
        />
      </el-form-item>

      <el-form-item label="部门" prop="departmentId">
        <el-select
          v-model="form.departmentId"
          placeholder="选择部门"
          clearable
          style="width: 100%"
        >
          <el-option
            v-for="dept in departments"
            :key="dept.id"
            :label="dept.name"
            :value="dept.id"
          />
        </el-select>
      </el-form-item>

      <el-form-item
        v-if="type === 'create'"
        label="角色"
        prop="roleIds"
      >
        <el-select
          v-model="form.roleIds"
          multiple
          placeholder="选择角色"
          style="width: 100%"
        >
          <el-option
            v-for="role in roles"
            :key="role.id"
            :label="role.name"
            :value="role.id"
          />
        </el-select>
      </el-form-item>

      <el-form-item label="状态" prop="isActive">
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
import { userApi, roleApi, departmentApi, type UserListItem } from '../../../api/system'

interface Props {
  modelValue: boolean
  type: 'create' | 'edit'
  userData: UserListItem | null
}

const props = defineProps<Props>()
const emit = defineEmits(['update:modelValue', 'success'])

const visible = computed({
  get: () => props.modelValue,
  set: (val) => emit('update:modelValue', val)
})

const dialogTitle = computed(() => props.type === 'create' ? '新增用户' : '编辑用户')

const formRef = ref<FormInstance>()
const submitting = ref(false)
const departments = ref<{ id: string; name: string }[]>([])
const roles = ref<{ id: string; name: string }[]>([])

const form = reactive({
  username: '',
  password: '',
  fullName: '',
  email: '',
  phone: '',
  departmentId: undefined as string | undefined,
  roleIds: [] as string[],
  isActive: true
})

const rules: FormRules = {
  username: [
    { required: true, message: '请输入用户名', trigger: 'blur' },
    { min: 3, max: 50, message: '长度在 3 到 50 个字符', trigger: 'blur' }
  ],
  password: [
    { required: props.type === 'create', message: '请输入密码', trigger: 'blur' },
    { min: 6, max: 20, message: '长度在 6 到 20 个字符', trigger: 'blur' }
  ],
  email: [
    { type: 'email', message: '请输入正确的邮箱地址', trigger: 'blur' }
  ],
  phone: [
    { pattern: /^1[3-9]\d{9}$/, message: '请输入正确的手机号', trigger: 'blur' }
  ]
}

// 获取部门和角色选项
const fetchOptions = async () => {
  try {
    const [deptRes, roleRes] = await Promise.all([
      departmentApi.getAllDepartments(),
      roleApi.getAllRoles()
    ])
    if (deptRes.data.code === 200) {
      departments.value = deptRes.data.data
    }
    if (roleRes.data.code === 200) {
      roles.value = roleRes.data.data
    }
  } catch (error) {
    console.error('获取选项失败', error)
  }
}

// 重置表单
const resetForm = () => {
  form.username = ''
  form.password = ''
  form.fullName = ''
  form.email = ''
  form.phone = ''
  form.departmentId = undefined
  form.roleIds = []
  form.isActive = true
}

// 填充表单
const fillForm = () => {
  if (props.userData) {
    form.username = props.userData.username
    form.fullName = props.userData.fullName || ''
    form.email = props.userData.email || ''
    form.phone = props.userData.phone || ''
    form.departmentId = props.userData.department?.id
    form.isActive = props.userData.isActive
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
      res = await userApi.createUser({
        username: form.username,
        password: form.password,
        fullName: form.fullName || undefined,
        email: form.email || undefined,
        phone: form.phone || undefined,
        departmentId: form.departmentId,
        roleIds: form.roleIds,
        isActive: form.isActive
      })
    } else {
      res = await userApi.updateUser(props.userData!.id, {
        fullName: form.fullName || undefined,
        email: form.email || undefined,
        phone: form.phone || undefined,
        departmentId: form.departmentId,
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
    fetchOptions()
  }
})

onMounted(() => {
  fetchOptions()
})
</script>
