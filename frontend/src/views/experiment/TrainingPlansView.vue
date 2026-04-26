<template>
  <div class="page-container">
    <div class="page-header">
      <h2>实训教学计划</h2>
      <el-button type="primary" @click="handleAdd">新增计划</el-button>
    </div>
    <el-card shadow="never">
      <el-table :data="list" v-loading="loading" stripe>
        <el-table-column label="课程" min-width="180">
          <template #default="{ row }">{{ row.course?.name || row.courseId }}</template>
        </el-table-column>
        <el-table-column prop="teachingOrganizationMethod" label="组织方式" min-width="150" />
        <el-table-column prop="teachingLocation" label="教学地点" min-width="150" />
        <el-table-column prop="assessmentMethod" label="考核方式" min-width="120" />
        <el-table-column label="操作" width="120" fixed="right">
          <template #default="{ row }"><el-button link type="primary" @click="handleEdit(row)">编辑</el-button></template>
        </el-table-column>
      </el-table>
    </el-card>

    <TrainingPlanFormDialog v-model="dialogVisible" :plan="currentPlan" @success="loadList" />
  </div>
</template>

<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { ElMessage } from 'element-plus'
import { experimentApi, type TrainingPlanDto } from '@/api/experiment'
import TrainingPlanFormDialog from './components/TrainingPlanFormDialog.vue'

const loading = ref(false)
const list = ref<TrainingPlanDto[]>([])
const dialogVisible = ref(false)
const currentPlan = ref<TrainingPlanDto | undefined>(undefined)

const loadList = async () => {
  loading.value = true
  try {
    const res = await experimentApi.getTrainingPlans()
    list.value = res.data.data || []
  } finally {
    loading.value = false
  }
}

const handleAdd = () => {
  currentPlan.value = undefined
  dialogVisible.value = true
}

const handleEdit = (row: TrainingPlanDto) => {
  currentPlan.value = row
  dialogVisible.value = true
}

onMounted(loadList)
</script>

<style scoped>
.page-container { padding: 20px; }
.page-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 16px; }
.page-header h2 { margin: 0; font-size: 20px; font-weight: 500; }
</style>

