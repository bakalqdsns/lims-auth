<template>
  <el-dialog
    :title="isEdit ? '编辑教学任务' : '新增教学任务'"
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
      <el-form-item label="学期" prop="semesterId">
        <el-select v-model="form.semesterId" placeholder="请选择学期" style="width: 100%">
          <el-option
            v-for="semester in semesters"
            :key="semester.id"
            :label="semester.name"
            :value="semester.id"
          />
        </el-select>
      </el-form-item>

      <el-form-item label="课程" prop="courseId">
        <el-select v-model="form.courseId" placeholder="请选择课程" style="width: 100%">
          <el-option
            v-for="course in courses"
            :key="course.id"
            :label="`${course.code} - ${course.name}`"
            :value="course.id"
          />
        </el-select>
      </el-form-item>

      <el-form-item label="班级" prop="classId">
        <el-select v-model="form.classId" placeholder="请选择班级" style="width: 100%">
          <el-option
            v-for="cls in classes"
            :key="cls.id"
            :label="`${cls.grade}级 ${cls.name}`"
            :value="cls.id"
          />
        </el-select>
      </el-form-item>

      <el-form-item label="任课教师" prop="teacherIds">
        <el-select
          v-model="form.teacherIds"
          multiple
          placeholder="请选择任课教师（可多选）"
          style="width: 100%"
        >
          <el-option
            v-for="teacher in teachers"
            :key="teacher.id"
            :label="teacher.fullName || teacher.username"
            :value="teacher.id"
          />
        </el-select>
      </el-form-item>

      <el-form-item label="备注" prop="description">
        <el-input
          v-model="form.description"
          type="textarea"
          :rows="3"
          placeholder="请输入备注信息"
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
import { teachingTaskApi, semesterApi, courseApi, classApi } from '../../../api/teaching'
import { userApi } from '../../../api/system'

interface Props {
  modelValue: boolean
  task?: {
    id: string
    semesterId: string
    courseId: string
    classId: string
    teachers: { id: string; name: string }[]
    description?: string
  }
}

const props = defineProps<Props>()
const emit = defineEmits(['update:modelValue', 'success'])

const visible = computed({
  get: () => props.modelValue,
  set: (val) => emit('update:modelValue', val)
})

const isEdit = computed(() => !!props.task)

const formRef = ref<FormInstance>()
const submitting = ref(false)
const semesters = ref<any[]>([])
const courses = ref<any[]>([])
const classes = ref<any[]>([])
const teachers = ref<any[]>([])

const form = reactive({
  semesterId: '',
  courseId: '',
  classId: '',
  teacherIds: [] as string[],
  description: ''
})

const rules: FormRules = {
  semesterId: [
    { required: true, message: '请选择学期', trigger: 'change' }
  ],
  courseId: [
    { required: true, message: '请选择课程', trigger: 'change' }
  ],
  classId: [
    { required: true, message: '请选择班级', trigger: 'change' }
  ],
  teacherIds: [
    { required: true, message: '请选择任课教师', trigger: 'change', type: 'array', min: 1 }
  ]
}

const loadSemesters = async () => {
  try {
    const res = await semesterApi.getList()
    if (res.data.code === 200) {
      semesters.value = res.data.data.filter((s: any) => s.isActive)
    }
  } catch (error) {
    console.error('加载学期列表失败', error)
  }
}

const loadCourses = async () => {
  try {
    const res = await courseApi.getList()
    if (res.data.code === 200) {
      courses.value = res.data.data.filter((c: any) => c.isActive)
    }
  } catch (error) {
    console.error('加载课程列表失败', error)
  }
}

const loadClasses = async () => {
  try {
    const res = await classApi.getList()
    if (res.data.code === 200) {
      classes.value = res.data.data.filter((c: any) => c.isActive)
    }
  } catch (error) {
    console.error('加载班级列表失败', error)
  }
}

const loadTeachers = async () => {
  try {
    const res = await userApi.getList({ page: 1, pageSize: 1000 })
    if (res.data.code === 200) {
      teachers.value = res.data.data.items.filter((u: any) =>
        u.roles?.includes('teacher') || u.roles?.includes('super_admin')
      )
    }
  } catch (error) {
    console.error('加载教师列表失败', error)
  }
}

const handleSubmit = async () => {
  const valid = await formRef.value?.validate().catch(() => false)
  if (!valid) return

  submitting.value = true
  try {
    const api = isEdit.value
      ? () => teachingTaskApi.update(props.task!.id, form)
      : () => teachingTaskApi.create(form)

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

watch(() => props.task, (val) => {
  if (val) {
    form.semesterId = val.semesterId
    form.courseId = val.courseId
    form.classId = val.classId
    form.teacherIds = val.teachers.map(t => t.id)
    form.description = val.description || ''
  } else {
    form.semesterId = ''
    form.courseId = ''
    form.classId = ''
    form.teacherIds = []
    form.description = ''
  }
}, { immediate: true })

onMounted(() => {
  loadSemesters()
  loadCourses()
  loadClasses()
  loadTeachers()
})
</script>
