<template>
  <el-dialog :title="dialogTitle" v-model="visible" width="700px" destroy-on-close>
    <el-form ref="formRef" :model="form" :rules="rules" label-width="100px">
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="课程代码" prop="code">
            <el-input v-model="form.code" placeholder="请输入课程代码" :disabled="type === 'edit'" />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="中文名称" prop="name">
            <el-input v-model="form.name" placeholder="请输入课程名称" />
          </el-form-item>
        </el-col>
      </el-row>
      
      <el-form-item label="英文名称">
        <el-input v-model="form.englishName" placeholder="请输入英文名称" />
      </el-form-item>
      
      <el-row :gutter="20">
        <el-col :span="8">
          <el-form-item label="修读性质" prop="courseType">
            <el-select v-model="form.courseType" placeholder="选择" style="width: 100%">
              <el-option label="必修" value="必修" />
              <el-option label="选修" value="选修" />
              <el-option label="限选" value="限选" />
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :span="8">
          <el-form-item label="学分" prop="credits">
            <el-input-number v-model="form.credits" :min="0" :max="20" :precision="1" style="width: 100%" />
          </el-form-item>
        </el-col>
        <el-col :span="8">
          <el-form-item label="设课学期" prop="semesterType">
            <el-select v-model="form.semesterType" placeholder="选择" style="width: 100%">
              <el-option label="上学期" :value="1" />
              <el-option label="下学期" :value="2" />
              <el-option label="全学年" :value="3" />
            </el-select>
          </el-form-item>
        </el-col>
      </el-row>
      
      <el-divider>学时分配</el-divider>
      
      <el-row :gutter="20">
        <el-col :span="8">
          <el-form-item label="总学时" prop="totalHours">
            <el-input-number v-model="form.totalHours" :min="0" :max="200" style="width: 100%" />
          </el-form-item>
        </el-col>
        <el-col :span="8">
          <el-form-item label="讲授学时" prop="theoryHours">
            <el-input-number v-model="form.theoryHours" :min="0" :max="200" style="width: 100%" />
          </el-form-item>
        </el-col>
        <el-col :span="8">
          <el-form-item label="实践学时" prop="practiceHours">
            <el-input-number v-model="form.practiceHours" :min="0" :max="200" style="width: 100%" />
          </el-form-item>
        </el-col>
      </el-row>
      
      <el-row :gutter="20">
        <el-col :span="8">
          <el-form-item label="实验学时" prop="experimentHours">
            <el-input-number v-model="form.experimentHours" :min="0" :max="200" style="width: 100%" />
          </el-form-item>
        </el-col>
        <el-col :span="8">
          <el-form-item label="网络学时" prop="onlineHours">
            <el-input-number v-model="form.onlineHours" :min="0" :max="200" style="width: 100%" />
          </el-form-item>
        </el-col>
        <el-col :span="8">
          <el-form-item label="开课部门">
            <el-select v-model="form.departmentId" placeholder="选择部门" clearable style="width: 100%">
              <el-option v-for="dept in departments" :key="dept.id" :label="dept.name" :value="dept.id" />
            </el-select>
          </el-form-item>
        </el-col>
      </el-row>
      
      <el-form-item label="课程描述">
        <el-input v-model="form.description" type="textarea" :rows="3" placeholder="请输入课程描述" />
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
import { courseApi, departmentApi, type CourseDto } from '@/api/teaching'

interface Props {
  modelValue: boolean
  type: 'create' | 'edit'
  courseData: CourseDto | null
}

const props = defineProps<Props>()
const emit = defineEmits(['update:modelValue', 'success'])

const visible = computed({
  get: () => props.modelValue,
  set: (val) => emit('update:modelValue', val)
})

const dialogTitle = computed(() => props.type === 'create' ? '新增课程' : '编辑课程')

const formRef = ref<FormInstance>()
const submitting = ref(false)
const departments = ref<{ id: string; name: string }[]>([])

const form = reactive({
  code: '',
  name: '',
  englishName: '',
  courseType: '必修',
  credits: 2,
  totalHours: 32,
  theoryHours: 16,
  practiceHours: 8,
  experimentHours: 8,
  onlineHours: 0,
  semesterType: 1,
  departmentId: undefined as string | undefined,
  description: ''
})

const rules: FormRules = {
  code: [{ required: true, message: '请输入课程代码', trigger: 'blur' }],
  name: [{ required: true, message: '请输入课程名称', trigger: 'blur' }],
  courseType: [{ required: true, message: '请选择修读性质', trigger: 'change' }],
  credits: [{ required: true, message: '请输入学分', trigger: 'blur' }],
  totalHours: [{ required: true, message: '请输入总学时', trigger: 'blur' }],
  theoryHours: [{ required: true, message: '请输入讲授学时', trigger: 'blur' }],
  practiceHours: [{ required: true, message: '请输入实践学时', trigger: 'blur' }],
  experimentHours: [{ required: true, message: '请输入实验学时', trigger: 'blur' }],
  onlineHours: [{ required: true, message: '请输入网络学时', trigger: 'blur' }],
  semesterType: [{ required: true, message: '请选择设课学期', trigger: 'change' }]
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

const resetForm = () => {
  form.code = ''
  form.name = ''
  form.englishName = ''
  form.courseType = '必修'
  form.credits = 2
  form.totalHours = 32
  form.theoryHours = 16
  form.practiceHours = 8
  form.experimentHours = 8
  form.onlineHours = 0
  form.semesterType = 1
  form.departmentId = undefined
  form.description = ''
}

const fillForm = () => {
  if (props.courseData) {
    form.code = props.courseData.code
    form.name = props.courseData.name
    form.englishName = props.courseData.englishName || ''
    form.courseType = props.courseData.courseType
    form.credits = props.courseData.credits
    form.totalHours = props.courseData.totalHours
    form.theoryHours = props.courseData.theoryHours
    form.practiceHours = props.courseData.practiceHours
    form.experimentHours = props.courseData.experimentHours
    form.onlineHours = props.courseData.onlineHours
    form.semesterType = props.courseData.semesterType
    form.departmentId = props.courseData.departmentId
    form.description = props.courseData.description || ''
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
      englishName: form.englishName || undefined,
      courseType: form.courseType,
      credits: form.credits,
      totalHours: form.totalHours,
      theoryHours: form.theoryHours,
      practiceHours: form.practiceHours,
      experimentHours: form.experimentHours,
      onlineHours: form.onlineHours,
      semesterType: form.semesterType,
      departmentId: form.departmentId,
      description: form.description || undefined
    }

    let res
    if (props.type === 'create') {
      res = await courseApi.create(data)
    } else {
      res = await courseApi.update(props.courseData!.id, data)
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
  }
})

onMounted(() => {
  fetchDepartments()
})
</script>
