<template>
  <div class="tasks-view">
    <el-card>
      <template #header>
        <div class="card-header">
          <span>教学任务管理</span>
          <el-button type="primary" @click="handleAdd">
            <el-icon><Plus /></el-icon>新增教学任务
          </el-button>
        </div>
      </template>

      <el-table :data="tasks" v-loading="loading" stripe>
        <el-table-column type="index" label="序号" width="60" />
        <el-table-column prop="semesterName" label="学期" width="180" />
        <el-table-column prop="courseName" label="课程" />
        <el-table-column prop="courseCode" label="课程代码" width="120" />
        <el-table-column prop="className" label="班级" />
        <el-table-column prop="teachers" label="任课教师" min-width="150">
          <template #default="{ row }">
            <el-tag
              v-for="teacher in row.teachers"
              :key="teacher.id"
              size="small"
              class="mr-1"
            >
              {{ teacher.name }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="studentCount" label="学生数" width="80" />
        <el-table-column prop="isActive" label="状态" width="80">
          <template #default="{ row }">
            <el-tag :type="row.isActive ? 'success' : 'info'">
              {{ row.isActive ? '启用' : '禁用' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="180" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" link @click="handleEdit(row)">编辑</el-button>
            <el-button type="primary" link @click="handleToggleStatus(row)">
              {{ row.isActive ? '禁用' : '启用' }}
            </el-button>
            <el-button type="danger" link @click="handleDelete(row)">删除</el-button>
          </template>
        </el-table-column>
      </el-table>
    </el-card>

    <!-- 表单对话框 -->
    <TeachingTaskFormDialog
      v-model="dialogVisible"
      :task="currentTask"
      @success="loadData"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { Plus } from '@element-plus/icons-vue'
import { teachingTaskApi } from '../../api/teaching'
import TeachingTaskFormDialog from './components/TeachingTaskFormDialog.vue'

interface TeachingTask {
  id: string
  semesterId: string
  semesterName: string
  courseId: string
  courseName: string
  courseCode: string
  classId: string
  className: string
  teachers: { id: string; name: string }[]
  studentCount: number
  description?: string
  isActive: boolean
}

const loading = ref(false)
const tasks = ref<TeachingTask[]>([])
const dialogVisible = ref(false)
const currentTask = ref<TeachingTask | undefined>(undefined)

const loadData = async () => {
  loading.value = true
  try {
    const res = await teachingTaskApi.getList()
    if (res.data.code === 200) {
      tasks.value = res.data.data
    }
  } catch (error) {
    ElMessage.error('加载教学任务列表失败')
  } finally {
    loading.value = false
  }
}

const handleAdd = () => {
  currentTask.value = undefined
  dialogVisible.value = true
}

const handleEdit = (row: TeachingTask) => {
  currentTask.value = row
  dialogVisible.value = true
}

const handleToggleStatus = async (row: TeachingTask) => {
  try {
    await ElMessageBox.confirm(
      `确定要${row.isActive ? '禁用' : '启用'}该教学任务吗？`,
      '提示',
      { type: 'warning' }
    )
    const res = await teachingTaskApi.toggleStatus(row.id, !row.isActive)
    if (res.data.code === 200) {
      ElMessage.success('操作成功')
      loadData()
    }
  } catch (error) {
    // 取消操作
  }
}

const handleDelete = async (row: TeachingTask) => {
  try {
    await ElMessageBox.confirm(
      '确定要删除该教学任务吗？此操作不可恢复！',
      '警告',
      { type: 'error' }
    )
    const res = await teachingTaskApi.delete(row.id)
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
.tasks-view {
  padding: 20px;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.mr-1 {
  margin-right: 4px;
}
</style>
