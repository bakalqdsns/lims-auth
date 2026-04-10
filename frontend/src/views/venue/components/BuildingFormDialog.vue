<template>
  <el-dialog
    :model-value="modelValue"
    :title="isEdit ? '编辑楼宇' : '新增楼宇'"
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
      <el-form-item label="所属校区" prop="campusId">
        <el-select v-model="form.campusId" placeholder="请选择校区" style="width: 100%">
          <el-option
            v-for="campus in campusList"
            :key="campus.id"
            :label="campus.name"
            :value="campus.id"
          />
        </el-select>
      </el-form-item>
      <el-form-item label="楼宇编码" prop="code">
        <el-input v-model="form.code" placeholder="请输入楼宇编码" :disabled="isEdit" />
      </el-form-item>
      <el-form-item label="楼宇名称" prop="name">
        <el-input v-model="form.name" placeholder="请输入楼宇名称" />
      </el-form-item>
      <el-form-item label="楼宇类型" prop="buildingType">
        <el-select v-model="form.buildingType" placeholder="请选择楼宇类型" style="width: 100%">
          <el-option label="实验楼" value="实验楼" />
          <el-option label="教学楼" value="教学楼" />
          <el-option label="办公楼" value="办公楼" />
          <el-option label="图书馆" value="图书馆" />
          <el-option label="体育馆" value="体育馆" />
          <el-option label="其他" value="其他" />
        </el-select>
      </el-form-item>
      <el-form-item label="地址" prop="address">
        <el-input v-model="form.address" placeholder="请输入地址" />
      </el-form-item>
      <el-form-item label="楼层数" prop="floorCount">
        <el-input-number v-model="form.floorCount" :min="1" :max="100" style="width: 100%" />
      </el-form-item>
      <el-form-item label="建筑面积" prop="buildingArea">
        <el-input-number v-model="form.buildingArea" :min="0" style="width: 100%" />
      </el-form-item>
      <el-form-item label="建成年份" prop="builtYear">
        <el-input-number v-model="form.builtYear" :min="1900" :max="2100" style="width: 100%" />
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
import { buildingApi, campusApi } from '../../../api/venue'
import { userApi } from '../../../api/system'

const props = defineProps<{
  modelValue: boolean
  building: any
}>()

const emit = defineEmits(['update:modelValue', 'success'])

const formRef = ref<FormInstance>()
const submitting = ref(false)
const campusList = ref([])
const userList = ref([])

const isEdit = computed(() => !!props.building)

const form = reactive({
  campusId: '',
  code: '',
  name: '',
  buildingType: '实验楼',
  address: '',
  floorCount: 1,
  buildingArea: undefined as number | undefined,
  builtYear: new Date().getFullYear(),
  managerId: undefined as string | undefined,
  description: '',
  isActive: true
})

const rules: FormRules = {
  campusId: [
    { required: true, message: '请选择所属校区', trigger: 'change' }
  ],
  code: [
    { required: true, message: '请输入楼宇编码', trigger: 'blur' },
    { min: 2, max: 50, message: '长度在 2 到 50 个字符', trigger: 'blur' }
  ],
  name: [
    { required: true, message: '请输入楼宇名称', trigger: 'blur' },
    { min: 2, max: 200, message: '长度在 2 到 200 个字符', trigger: 'blur' }
  ],
  buildingType: [
    { required: true, message: '请选择楼宇类型', trigger: 'change' }
  ]
}

const fetchCampuses = async () => {
  try {
    const res = await campusApi.getList()
    if (res.data.code === 200) {
      campusList.value = res.data.data
    }
  } catch (error) {
    console.error('获取校区列表失败', error)
  }
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
  form.campusId = ''
  form.code = ''
  form.name = ''
  form.buildingType = '实验楼'
  form.address = ''
  form.floorCount = 1
  form.buildingArea = undefined
  form.builtYear = new Date().getFullYear()
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
      campusId: form.campusId,
      code: form.code,
      name: form.name,
      buildingType: form.buildingType,
      address: form.address || undefined,
      floorCount: form.floorCount,
      buildingArea: form.buildingArea,
      builtYear: form.builtYear,
      managerId: form.managerId,
      description: form.description || undefined,
      isActive: form.isActive
    }

    let res
    if (isEdit.value) {
      res = await buildingApi.update(props.building.id, data)
    } else {
      res = await buildingApi.create(data)
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
    if (props.building) {
      form.campusId = props.building.campusId
      form.code = props.building.code
      form.name = props.building.name
      form.buildingType = props.building.buildingType
      form.address = props.building.address || ''
      form.floorCount = props.building.floorCount
      form.buildingArea = props.building.buildingArea
      form.builtYear = props.building.builtYear
      form.managerId = props.building.managerId
      form.description = props.building.description || ''
      form.isActive = props.building.isActive
    } else {
      resetForm()
    }
    fetchCampuses()
    fetchUsers()
  }
})

onMounted(() => {
  fetchCampuses()
  fetchUsers()
})
</script>
