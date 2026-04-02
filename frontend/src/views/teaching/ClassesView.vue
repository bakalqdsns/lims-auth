<template>
  <div class="classes-view">
    <el-card>
      <template #header>
        <div class="card-header">
          <span>班级管理</span>
          <el-button type="primary" @click="handleAdd">
            <el-icon><Plus /></el-icon>新增班级
          </el-button>
        </div>
      </template>

      <el-table :data="classes" v-loading="loading" stripe>
        <el-table-column type="index" label="序号" width="60" />
        <el-table-column prop="code" label="班级代码" width="120" />
        <el-table-column prop="name" label="班级名称" />
        <el-table-column prop="grade" label="年级" width="100" />
        <el-table-column prop="majorName" label="所属专业" width="150" />
        <el-table-column prop="studentCount" label="学生数" width="80" />
        <el-table-column prop="headTeacherName" label="班主任" width="100" />
        <el-table-column prop="isActive" label="状态" width="80">
          <template #default="{ row }">
            <el-tag :type="row.isActive ? 'success' : 'info'">
              {{ row.isActive ? '启用' : '禁用' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="220" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" link @click="handleEdit(row)">编辑</el-button>
            <el-button type="primary" link @click="handleManageStudents(row)">学生管理</el-button>
            <el-button type="danger" link @click="handleDelete(row)">删除</el-button>
          </template>
        </el-table-column>
      </el-table>
    </el-card>

    <!-- 表单对话框 -->
    <ClassFormDialog
      v-model="dialogVisible"
      :class-data="currentClass"
      @success="loadData"
    />

    <!-- 学生管理对话框 -->
    <ClassStudentsDialog
      v-model="studentsDialogVisible"
      :class-id="currentClassId"
      :class-name="currentClassName"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { Plus } from '@element-plus/icons-vue'
import { classApi } from '../../api/teaching'
import ClassFormDialog from './components/ClassFormDialog.vue'
import ClassStudentsDialog from './components/ClassStudentsDialog.vue'

interface Class {
  id: string
  code: string
  name: string
  grade: string
  majorId: string
  majorName?: string
  headTeacherId?: string
  headTeacherName?: string
  adminStudentId?: string
  studentCount: number
  isActive: boolean
}

const loading = ref(false)
const classes = ref<Class[]>([])
const dialogVisible = ref(false)
const studentsDialogVisible = ref(false)
const currentClass = ref<Class | undefined>(undefined)
const currentClassId = ref('')
const currentClassName = ref('')

const loadData = async () => {
  loading.value = true
  try {
    const res = await classApi.getList()
    if (res.data.code === 200) {
      classes.value = res.data.data
    }
  } catch (error) {
    ElMessage.error('加载班级列表失败')
  } finally {
    loading.value = false
  }
}

const handleAdd = () => {
  currentClass.value = undefined
  dialogVisible.value = true
}

const handleEdit = (row: Class) => {
  currentClass.value = row
  dialogVisible.value = true
}

const handleManageStudents = (row: Class) => {
  currentClassId.value = row.id
  currentClassName.value = row.name
  studentsDialogVisible.value = true
}

const handleDelete = async (row: Class) => {
  try {
    await ElMessageBox.confirm(
      `确定要删除班级 "${row.name}" 吗？此操作不可恢复！`,
      '警告',
      { type: 'error' }
    )
    const res = await classApi.delete(row.id)
    if (res.data.code === 200) {
      ElMessage.success('删除成功')
      loadData()
    }
  } catch (error) {
    // 取消操作
  }
}

onMounted(loadData)
</script>

<style scoped>
.classes-view {
  padding: 20px;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}
</style>
