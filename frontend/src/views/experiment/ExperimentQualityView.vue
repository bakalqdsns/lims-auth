<template>
  <div class="page-container">
    <div class="page-header">
      <h2>实验课程教学质量</h2>
      <el-button type="primary" @click="handleAdd">新增评估</el-button>
    </div>
    <el-card shadow="never">
      <el-table :data="list" v-loading="loading" stripe>
        <el-table-column prop="courseName" label="课程名称" min-width="160" />
        <el-table-column prop="mainTeacher" label="主讲教师" width="100" />
        <el-table-column prop="teacherTitle" label="教师职称" width="90" />
        <el-table-column prop="className" label="授课班级" width="130" />
        <el-table-column prop="classStudentCount" label="班级人数" width="90" />
        <el-table-column prop="experimentHours" label="实验课时" width="90" />
        <el-table-column label="计划/实际" width="110">
          <template #default="{ row }">{{ row.plannedExperimentCount }} / {{ row.actualExperimentCount }}</template>
        </el-table-column>
        <el-table-column prop="assessmentMethod" label="考核方式" min-width="120" />
        <el-table-column prop="assessmentTime" label="考核时间" width="90" />
        <el-table-column label="独立设课" width="90">
          <template #default="{ row }"><el-tag :type="row.isIndependentCourse ? 'success' : 'info'" size="small">{{ row.isIndependentCourse ? '是' : '否' }}</el-tag></template>
        </el-table-column>
        <el-table-column label="操作" width="120" fixed="right">
          <template #default="{ row }"><el-button link type="primary" @click="handleEdit(row)">编辑</el-button></template>
        </el-table-column>
      </el-table>
    </el-card>

    <ExperimentQualityFormDialog v-model="dialogVisible" :quality="currentQuality" @success="loadList" />
  </div>
</template>

<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { experimentApi, type ExperimentQualityDto } from '@/api/experiment'
import ExperimentQualityFormDialog from './components/ExperimentQualityFormDialog.vue'

const loading = ref(false)
const list = ref<ExperimentQualityDto[]>([])
const dialogVisible = ref(false)
const currentQuality = ref<ExperimentQualityDto | undefined>(undefined)

const loadList = async () => {
  loading.value = true
  try {
    const res = await experimentApi.getQualityList()
    list.value = res.data.data || []
  } finally {
    loading.value = false
  }
}

const handleAdd = () => {
  currentQuality.value = undefined
  dialogVisible.value = true
}

const handleEdit = (row: ExperimentQualityDto) => {
  currentQuality.value = row
  dialogVisible.value = true
}

onMounted(loadList)
</script>

<style scoped>
.page-container { padding: 20px; }
.page-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 16px; }
.page-header h2 { margin: 0; font-size: 20px; font-weight: 500; }
</style>

