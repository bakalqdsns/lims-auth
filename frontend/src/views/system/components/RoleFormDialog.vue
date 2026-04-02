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
      <el-form-item label="角色编码" prop="code">
        <el-input
          v-model="form.code"
          placeholder="请输入角色编码"
          :disabled="type === 'edit'"
        />
      </el-form-item>

      <el-form-item label="角色名称" prop="name">
        <el-input
          v-model="form.name"
          placeholder="请输入角色名称"
        />
      </el-form-item>

      <el-form-item label="描述" prop="description">
        <el-input
          v-model="form.description"
          type="textarea"
          :rows="3"
          placeholder="请输入角色描述"
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
import { roleApi, type RoleDto } from '../../../api/system'

interface Props {
  modelValue: boolean
  type: 'create' | 'edit'
  roleData: RoleDto | null
}

const props = defineProps<Props>()
const emit = defineEmits(['update:modelValue', 'success'])

const visible = computed({
  get: () => props.modelValue,
  set: (val) => emit('update:modelValue', val)
})

const dialogTitle = computed(() => props.type === 'create' ? '新增角色' : '编辑角色')

const formRef = ref<FormInstance>()
const submitting = ref(false)

const form = reactive({
  code: '',
  name: '',
  description: ''
})

const rules: FormRules = {
  code: [
    { required: true, message: '请输入角色编码', trigger: 'blur' },
    { min: 2, max: 50, message: '长度在 2 到 50 个字符', trigger: 'blur' },
    { pattern: /^[a-z_]+$/, message: '只能使用小写字母和下划线', trigger: 'blur' }
  ],
  name: [
    { required: true, message: '请输入角色名称', trigger: 'blur' },
    { min: 2, max: 100, message: '长度在 2 到 100 个字符', trigger: 'blur' }
  ]
}

// 重置表单
const resetForm = () => {
  form.code = ''
  form.name = ''
  form.description = ''
}

// 填充表单
const fillForm = () => {
  if (props.roleData) {
    form.code = props.roleData.code
    form.name = props.roleData.name
    form.description = props.roleData.description || ''
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
      res = await roleApi.createRole({
        code: form.code,
        name: form.name,
        description: form.description || undefined
      })
    } else {
      res = await roleApi.updateRole(props.roleData!.id, {
        name: form.name,
        description: form.description || undefined
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
  }
})
</script>
