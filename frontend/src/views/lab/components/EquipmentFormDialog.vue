<template>
  <el-dialog :title="dialogTitle" v-model="visible" width="800px" destroy-on-close>
    <el-form ref="formRef" :model="form" :rules="rules" label-width="120px">
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="设备代码" prop="code">
            <el-input v-model="form.code" placeholder="请输入设备代码" :disabled="type === 'edit'" />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="设备名称" prop="name">
            <el-input v-model="form.name" placeholder="请输入设备名称" />
          </el-form-item>
        </el-col>
      </el-row>

      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="型号">
            <el-input v-model="form.model" placeholder="请输入型号" />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="制造商">
            <el-input v-model="form.manufacturer" placeholder="请输入制造商" />
          </el-form-item>
        </el-col>
      </el-row>

      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="序列号">
            <el-input v-model="form.serialNumber" placeholder="请输入序列号" />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="设备分类" prop="category">
            <el-select v-model="form.category" placeholder="选择分类" style="width: 100%">
              <el-option v-for="cat in EQUIPMENT_CATEGORIES" :key="cat" :label="cat" :value="cat" />
            </el-select>
          </el-form-item>
        </el-col>
      </el-row>

      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="所属实验室">
            <el-select v-model="form.labId" placeholder="选择实验室" clearable style="width: 100%">
              <el-option v-for="lab in labs" :key="lab.id" :label="lab.name" :value="lab.id" />
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="存放位置">
            <el-input v-model="form.location" placeholder="请输入存放位置" />
          </el-form-item>
        </el-col>
      </el-row>

      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="购买日期">
            <el-date-picker v-model="form.purchaseDate" type="date" placeholder="选择日期" style="width: 100%" />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="保修期(月)">
            <el-input-number v-model="form.warrantyMonths" :min="0" :max="120" style="width: 100%" />
          </el-form-item>
        </el-col>
      </el-row>

      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="购买价格">
            <el-input-number v-model="form.price" :min="0" :precision="2" style="width: 100%" />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="设备状态" prop="status">
            <el-select v-model="form.status" placeholder="选择状态" style="width: 100%">
              <el-option v-for="status in EQUIPMENT_STATUSES" :key="status" :label="status" :value="status" />
            </el-select>
          </el-form-item>
        </el-col>
      </el-row>

      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="需要预约">
            <el-switch v-model="form.requiresBooking" />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="最大预约时长">
            <el-input-number v-model="form.maxBookingHours" :min="1" :max="168" :disabled="!form.requiresBooking" style="width: 100%" />
          </el-form-item>
        </el-col>
      </el-row>

      <el-form-item label="设备图片URL">
        <el-input v-model="form.imageUrl" placeholder="请输入图片URL" />
      </el-form-item>

      <el-form-item label="使用说明">
        <el-input v-model="form.instructions" type="textarea" :rows="3" placeholder="请输入使用说明" />
      </el-form-item>

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
import { ref, reactive, computed, watch } from 'vue'
import { ElMessage, type FormInstance, type FormRules } from 'element-plus'
import { equipmentApi, type EquipmentDto, type LabDto, EQUIPMENT_CATEGORIES, EQUIPMENT_STATUSES } from '@/api/lab'

interface Props {
  modelValue: boolean
  type: 'create' | 'edit'
  equipmentData: EquipmentDto | null
  labs: LabDto[]
}

const props = defineProps<Props>()
const emit = defineEmits(['update:modelValue', 'success'])

const visible = computed({
  get: () => props.modelValue,
  set: (val) => emit('update:modelValue', val)
})

const dialogTitle = computed(() => props.type === 'create' ? '新增设备' : '编辑设备')

const formRef = ref<FormInstance>()
const submitting = ref(false)

const form = reactive({
  code: '',
  name: '',
  model: '',
  manufacturer: '',
  serialNumber: '',
  category: '通用设备',
  labId: undefined as string | undefined,
  location: '',
  purchaseDate: undefined as string | undefined,
  warrantyMonths: undefined as number | undefined,
  price: undefined as number | undefined,
  status: '正常',
  requiresBooking: false,
  maxBookingHours: undefined as number | undefined,
  imageUrl: '',
  instructions: '',
  description: ''
})

const rules: FormRules = {
  code: [{ required: true, message: '请输入设备代码', trigger: 'blur' }],
  name: [{ required: true, message: '请输入设备名称', trigger: 'blur' }],
  category: [{ required: true, message: '请选择设备分类', trigger: 'change' }],
  status: [{ required: true, message: '请选择设备状态', trigger: 'change' }]
}

const resetForm = () => {
  form.code = ''
  form.name = ''
  form.model = ''
  form.manufacturer = ''
  form.serialNumber = ''
  form.category = '通用设备'
  form.labId = undefined
  form.location = ''
  form.purchaseDate = undefined
  form.warrantyMonths = undefined
  form.price = undefined
  form.status = '正常'
  form.requiresBooking = false
  form.maxBookingHours = undefined
  form.imageUrl = ''
  form.instructions = ''
  form.description = ''
}

const fillForm = () => {
  if (props.equipmentData) {
    form.code = props.equipmentData.code
    form.name = props.equipmentData.name
    form.model = props.equipmentData.model || ''
    form.manufacturer = props.equipmentData.manufacturer || ''
    form.serialNumber = props.equipmentData.serialNumber || ''
    form.category = props.equipmentData.category
    form.labId = props.equipmentData.labId
    form.location = props.equipmentData.location || ''
    form.purchaseDate = props.equipmentData.purchaseDate
    form.warrantyMonths = props.equipmentData.warrantyMonths
    form.price = props.equipmentData.price
    form.status = props.equipmentData.status
    form.requiresBooking = props.equipmentData.requiresBooking
    form.maxBookingHours = props.equipmentData.maxBookingHours
    form.imageUrl = props.equipmentData.imageUrl || ''
    form.instructions = props.equipmentData.instructions || ''
    form.description = props.equipmentData.description || ''
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
      model: form.model || undefined,
      manufacturer: form.manufacturer || undefined,
      serialNumber: form.serialNumber || undefined,
      category: form.category,
      labId: form.labId,
      location: form.location || undefined,
      purchaseDate: form.purchaseDate,
      warrantyMonths: form.warrantyMonths,
      price: form.price,
      status: form.status,
      requiresBooking: form.requiresBooking,
      maxBookingHours: form.maxBookingHours,
      imageUrl: form.imageUrl || undefined,
      instructions: form.instructions || undefined,
      description: form.description || undefined
    }

    let res
    if (props.type === 'create') {
      res = await equipmentApi.create(data)
    } else {
      res = await equipmentApi.update(props.equipmentData!.id, data)
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
