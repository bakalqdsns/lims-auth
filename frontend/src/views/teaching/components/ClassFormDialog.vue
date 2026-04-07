<template>
  <el-dialog
    :title="isEdit ? '编辑班级' : '新增班级'"
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
          <el-form-item label="班级代码" prop="code">
            <el-input v-model="form.code" placeholder="请输入班级代码" />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="班级名称" prop="name">
            <el-input v-model="form.name" placeholder="请输入班级名称" />
          </el-form-item>
        </el-col>
      </el-row>

      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="年级" prop="grade">
            <el-select v-model="form.grade" placeholder="请选择年级" style="width: 100%">
              <el-option
                v-for="year in gradeOptions"
                :key="year"
                :label="year"
                :value="year"
              />
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="所属专业" prop="majorId">
            <el-select v-model="form.majorId" placeholder="请选择专业" style="width: 100%">
              <el-option
                v-for="major in majors"
                :key="major.id"
                :label="major.name"
                :value="major.id"
              />
            </el-select>
          </el-form-item>
        </el-col>
      </el-row>

      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="班主任" prop="headTeacherId">
            <el-select
              v-model="form.headTeacherId"
              placeholder="请选择班主任"
              style="width: 100%"
              clearable
            >
              <el-option
                v-for="teacher in teachers"
                :key="teacher.id"
                :label="teacher.fullName || teacher.username"
                :value="teacher.id"
              />
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="班级管理员" prop="adminStudentId">
            <el-select
              v-model="form.adminStudentId"
              placeholder="请选择班级管理员"
              style="width: 100%"
              clearable
            >
              <el-option
                v-for="student in students"
                :key="student.id"
                :label="student.fullName || student.username"
                :value="student.id"
              />
            </el-select>
          </el-form-item>
        </el-col>
      </el-row>
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
import { classApi, majorApi } from '../../../api/teaching'
import { userApi } from '../../../api/system'

interface Props {
  modelValue: boolean
  classData?: {
    id: string
    code: string
    name: string
    grade: string
    majorId: string
    headTeacherId?: string
    adminStudentId?: string
  }
}

const props = defineProps<Props>()
const emit = defineEmits(['update:modelValue', 'success'])

const visible = computed({
  get: () => props.modelValue,
  set: (val) => emit('update:modelValue', val)
})

const isEdit = computed(() => !!props.classData)

const formRef = ref<FormInstance>()
const submitting = ref(false)
const majors = ref<any[]>([])
const teachers = ref<any[]>([])
const students = ref<any[]>([])

// 生成年级选项（前后5年）
const currentYear = new Date().getFullYear()
const gradeOptions = Array.from({ length: 10 }, (_, i) => String(currentYear - 5 + i))

const form = reactive({
  code: '',
  name: '',
  grade: String(currentYear),
  majorId: '',
  departmentId: '',
  headTeacherId: '',
  adminStudentId: ''
})

const rules: FormRules = {
  code: [
    { required: true, message: '请输入班级代码', trigger: 'blur' },
    { min: 2, max: 20, message: '长度在 2 到 20 个字符', trigger: 'blur' }
  ],
  name: [
    { required: true, message: '请输入班级名称', trigger: 'blur' },
    { min: 2, max: 100, message: '长度在 2 到 100 个字符', trigger: 'blur' }
  ],
  grade: [
    { required: true, message: '请选择年级', trigger: 'change' }
  ],
  majorId: [
    { required: true, message: '请选择所属专业', trigger: 'change' }
  ]
}

const loadMajors = async () => {
  try {
    const res = await majorApi.getList()
    if (res.data.code === 200) {
      majors.value = res.data.data.filter((m: any) => m.isActive)
    }
  } catch (error) {
    console.error('加载专业列表失败', error)
  }
}

const loadTeachers = async () => {
  try {
    const res = await userApi.getUsers({ page: 1, pageSize: 1000 })
    if (res.data.code === 200) {
      teachers.value = res.data.data.items.filter((u: any) =>
        u.roles?.some((r: any) => r.code === 'teacher' || r.code === 'super_admin')
      )
    }
  } catch (error) {
    console.error('加载教师列表失败', error)
  }
}

const loadStudents = async () => {
  try {
    const res = await userApi.getUsers({ page: 1, pageSize: 1000 })
    if (res.data.code === 200) {
      students.value = res.data.data.items.filter((u: any) =>
        u.roles?.some((r: any) => r.code === 'student')
      )
    }
  } catch (error) {
    console.error('加载学生列表失败', error)
  }
}

const handleSubmit = async () => {
  const valid = await formRef.value?.validate().catch(() => false)
  if (!valid) return

  submitting.value = true
  try {
    // 从选中的专业获取 departmentId
    const selectedMajor = majors.value.find(m => m.id === form.majorId)
    const submitData = {
      ...form,
      departmentId: selectedMajor?.departmentId || ''
    }

    const api = isEdit.value
      ? () => classApi.update(props.classData!.id, submitData)
      : () => classApi.create(submitData)

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

watch(() => props.classData, (val) => {
  if (val) {
    form.code = val.code
    form.name = val.name
    form.grade = val.grade
    form.majorId = val.majorId
    form.headTeacherId = val.headTeacherId || ''
    form.adminStudentId = val.adminStudentId || ''
  } else {
    form.code = ''
    form.name = ''
    form.grade = String(currentYear)
    form.majorId = ''
    form.headTeacherId = ''
    form.adminStudentId = ''
  }
}, { immediate: true })

onMounted(() => {
  loadMajors()
  loadTeachers()
  loadStudents()
})
</script>
