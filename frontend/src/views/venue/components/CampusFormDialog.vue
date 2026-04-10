<template>
  <el-dialog
    :model-value="modelValue"
    :title="isEdit ? '编辑校区' : '新增校区'"
    width="600px"
    destroy-on-close
    @update:model-value="$emit('update:modelValue', $event)"
  >
    <el-form
      ref="formRef"
      :model="form"
      :rules="rules"
      label-width="100px"
    >
      <el-form-item label="校区编码" prop="code">
        <el-input v-model="form.code" placeholder="请输入校区编码" :disabled="isEdit" />
      </el-form-item>
      <el-form-item label="校区名称" prop="name">
        <el-input v-model="form.name" placeholder="请输入校区名称" />
      </el-form-item>
      <el-form-item label="校区类型" prop="campusType">
        <el-select v-model="form.campusType" placeholder="请选择校区类型" style="width: 100%">
          <el-option label="主校区" value="主校区" />
          <el-option label="分校区" value="分校区" />
        </el-select>
      </el-form-item>
      <el-form-item label="地址" prop="address">
        <el-input v-model="form.address" placeholder="请输入地址" />
      </el-form-item>
      <el-form-item label="面积(㎡)" prop="area">
        <el-input-number v-model="form.area" :min="0" style="width: 100%" />
      </el-form-item>
      <el-form-item label="联系电话" prop="contactPhone">
        <el-input v-model="form.contactPhone" placeholder="请输入联系电话" />
      </el-form-item>
      <el-form-item label="负责人" prop="managerId">
        <el-select
          v-model="form.managerId"
          placeholder="请选择负责人"
          clearable
          style="width: 100%"
        >
          <el-option
            v-for="user in userList"
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
          placeholder="请输入描述"
        />
      </el-form-item>
      <el-form-item label="状态" prop="isActive" v-if="isEdit">
        <el-switch
          v-model="form.isActive"
          active-text="启用"
          inactive-text="禁用"
        />
      </el-form-item>
    </el-form>
    <template #footer>
      <el-button @click="$emit('update:modelValue', false)">取消</el-button>
      <el-button type="primary" :loading="submitting" @click="handleSubmit">
        确定
      </el-button>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, reactive, computed, watch, onMounted } from 'vue'
import { ElMessage, type FormInstance, type FormRules } from 'element-plus'
import { campusApi } from '../../../api/venue'
import { userApi } from '../../../api/system'

const props = defineProps<{
  modelValue: boolean
  campus: any
}>()

const emit = defineEmits(['update:modelValue', 'success'])

const formRef = ref<FormInstance>()
const submitting = ref(false)
const userList = ref([])

const isEdit = computed(() => !!props.campus)

const form = reactive({
  code: '',
  name: '',
  campusType: '主校区',
  address: '',
  area: undefined as number | undefined,
  contactPhone: '',
  managerId: undefined as string | undefined,
  description: '',
  isActive: true
})

const rules: FormRules = {
  code: [
    { required: true, message: '请输入校区编码', trigger: 'blur' },
    { min: 2, max: 50, message: '长度在 2 到 50 个字符', trigger: 'blur' }
  ],
  name: [
    { required: true, message: '请输入校区名称', trigger: 'blur' },
    { min: 2, max: 200, message: '长度在 2 到 200 个字符', trigger: 'blur' }
  ],
  campusType: [
    { required: true, message: '请选择校区类型', trigger: 'change' }
  ]
}

const fetchUsers = async () => {
  try {
    const res = await userApi.getUsers({ page: 1, pageSize: 1000 })
    if (res.data.code === 200) {
      userList.value = res.data.data.items || []
    }
  } catch (error) {
    console.error('获取用户列表失败', error)
  }
}

const resetForm = () => {
  form.code = ''
  form.name = ''
  form.campusType = '主校区'
  form.address = ''
  form.area = undefined
  form.contactPhone = ''
  form.managerId = undefined
  form.description = ''
  form.isActive = true
}

const handleSubmit = async () => {
  const valid = await formRef.value?.validate().catch(() => false)
  if (!valid) return

  submitting.value = true
  try {
    const data = {
      code: form.code,
      name: form.name,
      campusType: form.campusType,
      address: form.address || undefined,
      area: form.area,
      contactPhone: form.contactPhone || undefined,
      managerId: form.managerId,
      description: form.description || undefined,
      isActive: form.isActive
    }

    let res
    if (isEdit.value) {
      res = await campusApi.update(props.campus.id, data)
    } else {
      res = await campusApi.create(data)
    }

    if (res.data.code === 200) {
      ElMessage.success(isEdit.value ? '更新成功' : '创建成功')
      emit('success')
      emit('update:modelValue', false)
    } else {
      ElMessage.error(res.data.message)
    }
  } catch (error: any) {
    ElMessage.error(error.response?.data?.message || (isEdit.value ? '更新失败' : '创建失败'))
  } finally {
    submitting.value = false
  }
}

watch(() => props.modelValue, (val) => {
  if (val) {
    if (props.campus) {
      form.code = props.campus.code
      form.name = props.campus.name
      form.campusType = props.campus.campusType
      form.address = props.campus.address || ''
      form.area = props.campus.area
      form.contactPhone = props.campus.contactPhone || ''
      form.managerId = props.campus.managerId
      form.description = props.campus.description || ''
      form.isActive = props.campus.isActive
    } else {
      resetForm()
    }
    fetchUsers()
  }
})

onMounted(() => {
  fetchUsers()
})
</script>
