<template>
  <div class="page-container">
    <div class="page-header">
      <h2>实验课程教学质量</h2>
      <el-button type="primary" @click="handleAdd">新增评估</el-button>
    </div>
    <el-card shadow="never">
      <el-table :data="list" v-loading="loading" stripe>
        <el-table-column prop="courseName" label="课程名称" min-width="180" />
        <el-table-column prop="mainTeacher" label="主讲教师" width="120" />
        <el-table-column prop="className" label="授课班级" width="120" />
        <el-table-column prop="plannedExperimentCount" label="计划个数" width="90" />
        <el-table-column prop="actualExperimentCount" label="实际个数" width="90" />
        <el-table-column prop="assessmentMethod" label="考核方式" min-width="120" />
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

