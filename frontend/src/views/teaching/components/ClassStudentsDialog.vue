<template>
  <el-dialog
    :title="`管理班级学生 - ${className}`"
    v-model="visible"
    width="800px"
    destroy-on-close
  >
    <el-row :gutter="20" class="mb-4">
      <el-col :span="16">
        <el-select
          v-model="selectedStudents"
          multiple
          filterable
          remote
          reserve-keyword
          placeholder="搜索学生添加到班级"
          :remote-method="searchStudents"
          :loading="searching"
          style="width: 100%"
        >
          <el-option
            v-for="student in availableStudents"
            :key="student.id"
            :label="`${student.fullName || student.username} (${student.studentId || '无学号'})`"
            :value="student.id"
          />
        </el-select>
      </el-col>
      <el-col :span="8">
        <el-button type="primary" @click="handleAddStudents" :disabled="!selectedStudents.length">
          添加到班级
        </el-button>
      </el-col>
    </el-row>

    <el-table :data="classStudents" v-loading="loading" stripe>
      <el-table-column type="index" label="序号" width="60" />
      <el-table-column prop="studentId" label="学号" width="120" />
      <el-table-column prop="fullName" label="姓名" />
      <el-table-column prop="username" label="用户名" />
      <el-table-column label="操作" width="100">
        <template #default="{ row }">
          <el-button type="danger" link @click="handleRemoveStudent(row)">移除</el-button>
        </template>
      </el-table-column>
    </el-table>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, computed, watch, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { classApi } from '../../../api/teaching'
import { userApi } from '../../../api/system'

interface Props {
  modelValue: boolean
  classId: string
  className: string
}

const props = defineProps<Props>()
const emit = defineEmits(['update:modelValue'])

const visible = computed({
  get: () => props.modelValue,
  set: (val) => emit('update:modelValue', val)
})

const loading = ref(false)
const searching = ref(false)
const classStudents = ref<any[]>([])
const availableStudents = ref<any[]>([])
const selectedStudents = ref<string[]>([])

const loadClassStudents = async () => {
  if (!props.classId) return
  loading.value = true
  try {
    const res = await classApi.getStudents(props.classId)
    if (res.data.code === 200) {
      classStudents.value = res.data.data
    }
  } catch (error) {
    ElMessage.error('加载班级学生失败')
  } finally {
    loading.value = false
  }
}

const searchStudents = async (query: string) => {
  if (query.length < 1) return
  searching.value = true
  try {
    const res = await userApi.getUsers({
      keyword: query,
      page: 1,
      pageSize: 20
    })
    if (res.data.code === 200) {
      // 过滤掉已在班级中的学生
      const existingIds = new Set(classStudents.value.map(s => s.id))
      availableStudents.value = res.data.data.items.filter(
        (u: any) => !existingIds.has(u.id) && u.roles?.some((r: any) => r.code === 'student')
      )
    }
  } catch (error) {
    console.error('搜索学生失败', error)
  } finally {
    searching.value = false
  }
}

const handleAddStudents = async () => {
  if (!selectedStudents.value.length) return
  try {
    const res = await classApi.addStudents(props.classId, selectedStudents.value)
    if (res.data.code === 200) {
      ElMessage.success('添加成功')
      selectedStudents.value = []
      loadClassStudents()
    }
  } catch (error) {
    ElMessage.error('添加学生失败')
  }
}

const handleRemoveStudent = async (row: any) => {
  try {
    await ElMessageBox.confirm(
      `确定要将 "${row.fullName || row.username}" 从班级中移除吗？`,
      '提示',
      { type: 'warning' }
    )
    const res = await classApi.removeStudent(props.classId, row.id)
    if (res.data.code === 200) {
      ElMessage.success('移除成功')
      loadClassStudents()
    }
  } catch (error) {
    // 取消操作
  }
}

watch(() => props.classId, (val) => {
  if (val) {
    loadClassStudents()
  }
})

onMounted(() => {
  if (props.classId) {
    loadClassStudents()
  }
})
</script>

<style scoped>
.mb-4 {
  margin-bottom: 16px;
}
</style>
