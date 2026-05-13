<template>
  <div class="statistics-container">
    <div class="page-header">
      <h2>统计分析</h2>
      <div class="header-actions">
        <el-button type="success" @click="handleExport"><el-icon><Download /></el-icon>导出</el-button>
      </div>
    </div>

    <el-card class="search-card" shadow="never">
      <el-form :model="queryForm" inline>
        <el-form-item label="学期">
          <el-select v-model="queryForm.semesterId" placeholder="请选择学期" style="width: 200px" @change="fetchData">
            <el-option v-for="s in semesters" :key="s.id" :label="s.name" :value="s.id" />
          </el-select>
        </el-form-item>
        <el-form-item label="起始周">
          <el-input-number v-model="queryForm.startWeek" :min="1" :max="20" style="width: 100px" />
        </el-form-item>
        <el-form-item label="结束周">
          <el-input-number v-model="queryForm.endWeek" :min="1" :max="20" style="width: 100px" />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="fetchData"><el-icon><Search /></el-icon>查询</el-button>
        </el-form-item>
      </el-form>
    </el-card>

    <el-card shadow="never">
      <el-radio-group v-model="activeReport" size="large" style="margin-bottom: 20px; flex-wrap: wrap">
        <el-radio-button value="lab-usage">实验室使用人次</el-radio-button>
        <el-radio-button value="by-major">分专业统计</el-radio-button>
        <el-radio-button value="by-class">分班级统计</el-radio-button>
        <el-radio-button value="reservation">预约统计</el-radio-button>
        <el-radio-button value="completion">登记完成率</el-radio-button>
      </el-radio-group>

      <div v-if="activeReport === 'lab-usage'" v-loading="loading">
        <h4>实验室使用人次统计</h4>
        <el-table :data="labUsageData" stripe style="margin-top: 12px">
          <el-table-column prop="labName" label="实验室名称" min-width="160" />
          <el-table-column prop="usedSlots" label="已用节次数" width="120" align="center" />
          <el-table-column prop="totalSlots" label="总节次数" width="120" align="center" />
          <el-table-column label="使用率" width="120" align="center">
            <template #default="{ row }">
              <el-progress :percentage="row.occupancyRate || 0" :color="getProgressColor(row.occupancyRate)" />
            </template>
          </el-table-column>
        </el-table>
        <el-empty v-if="!loading && !labUsageData.length" description="暂无数据" />
      </div>

      <div v-else-if="activeReport === 'by-major'" v-loading="loading">
        <h4>分专业使用统计</h4>
        <el-table :data="categoryData" stripe style="margin-top: 12px">
          <el-table-column type="index" label="序号" width="60" />
          <el-table-column prop="category" label="专业名称" min-width="160" />
          <el-table-column prop="count" label="排课次数" width="120" align="center" />
          <el-table-column label="占比" width="120" align="center">
            <template #default="{ row }">
              <el-progress :percentage="row.percentage || 0" />
            </template>
          </el-table-column>
        </el-table>
        <el-empty v-if="!loading && !categoryData.length" description="暂无数据" />
      </div>

      <div v-else-if="activeReport === 'by-class'" v-loading="loading">
        <h4>分班级使用统计</h4>
        <el-table :data="categoryData" stripe style="margin-top: 12px">
          <el-table-column type="index" label="序号" width="60" />
          <el-table-column prop="category" label="班级名称" min-width="160" />
          <el-table-column prop="count" label="排课次数" width="120" align="center" />
          <el-table-column label="占比" width="120" align="center">
            <template #default="{ row }">
              <el-progress :percentage="row.percentage || 0" />
            </template>
          </el-table-column>
        </el-table>
        <el-empty v-if="!loading && !categoryData.length" description="暂无数据" />
      </div>

      <div v-else-if="activeReport === 'reservation'" v-loading="loading">
        <h4>预约申请统计</h4>
        <el-table :data="categoryData" stripe style="margin-top: 12px">
          <el-table-column type="index" label="序号" width="60" />
          <el-table-column prop="category" label="状态" width="120" />
          <el-table-column prop="count" label="数量" width="120" align="center" />
          <el-table-column label="占比" width="120" align="center">
            <template #default="{ row }">
              <el-progress :percentage="row.percentage || 0" />
            </template>
          </el-table-column>
        </el-table>
        <el-empty v-if="!loading && !categoryData.length" description="暂无数据" />
      </div>

      <div v-else-if="activeReport === 'completion'" v-loading="loading">
        <h4>使用登记完成率</h4>
        <el-row :gutter="16" style="margin-top: 16px">
          <el-col :span="6"><el-statistic title="总记录数" :value="completionData.total || 0" /></el-col>
          <el-col :span="6"><el-statistic title="已完成" :value="completionData.completed || 0" /></el-col>
          <el-col :span="6"><el-statistic title="待登记" :value="completionData.pending || 0" /></el-col>
          <el-col :span="6"><el-statistic title="已逾期" :value="completionData.overdue || 0">
            <template #suffix><el-tag type="danger" size="small">逾期</el-tag></template>
          </el-statistic></el-col>
        </el-row>
        <div style="margin-top: 24px; text-align: center">
          <el-progress type="circle" :percentage="completionData.rate || 0" :color="getProgressColor(completionData.rate)" :width="160">
            <template #default>
              <div>
                <div style="font-size: 28px; font-weight: 700; color: #303133">{{ completionData.rate || 0 }}%</div>
                <div style="color: #909399; font-size: 13px">完成率</div>
              </div>
            </template>
          </el-progress>
        </div>
      </div>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import { Search, Download } from '@element-plus/icons-vue'

const authHeaders = () => ({ 'Authorization': `Bearer ${localStorage.getItem('token') || ''}` })

const loading = ref(false)
const activeReport = ref('lab-usage')
const semesters = ref<any[]>([])
const labUsageData = ref<any[]>([])
const categoryData = ref<any[]>([])
const completionData = ref<any>({ total: 0, completed: 0, pending: 0, overdue: 0, rate: 0 })

const queryForm = reactive({ semesterId: null as string | null, startWeek: 1, endWeek: 20 })

const getProgressColor = (pct?: number) => {
  if (!pct) return '#909399'
  if (pct >= 80) return '#67c23a'
  if (pct >= 50) return '#e6a23c'
  return '#f56c6c'
}

const buildQuery = () => {
  const params = new URLSearchParams()
  if (queryForm.semesterId) params.append('semesterId', queryForm.semesterId)
  params.append('startWeek', String(queryForm.startWeek))
  params.append('endWeek', String(queryForm.endWeek))
  return params
}

const fetchData = async () => {
  if (!queryForm.semesterId) return
  loading.value = true
  labUsageData.value = []
  categoryData.value = []
  try {
    const qs = buildQuery()
    let endpoint = ''
    switch (activeReport.value) {
      case 'lab-usage': endpoint = `/api/v1/statistics/lab-usage?${qs}`; break
      case 'by-major': endpoint = `/api/v1/statistics/by-major?${qs}`; break
      case 'by-class': endpoint = `/api/v1/statistics/by-class?${qs}`; break
      case 'reservation': endpoint = `/api/v1/statistics/reservation?${qs}`; break
      case 'completion': endpoint = `/api/v1/statistics/completion-rate?${qs}`; break
    }
    if (!endpoint) return
    const res = await fetch(endpoint, { headers: authHeaders() }).then(r => r.json())
    if (res.code === 200) {
      if (activeReport.value === 'lab-usage') {
        labUsageData.value = res.data || []
      } else if (activeReport.value === 'completion') {
        completionData.value = res.data || { total: 0, completed: 0, pending: 0, overdue: 0, rate: 0 }
      } else {
        categoryData.value = res.data || []
      }
    }
  } catch {
    ElMessage.error('加载报表失败')
  } finally {
    loading.value = false
  }
}

const handleExport = () => {
  ElMessage.info('导出功能开发中')
}

onMounted(async () => {
  try {
    const semRes = await fetch('/api/v1/semesters', { headers: authHeaders() }).then(r => r.json())
    if (semRes.code === 200) {
      semesters.value = semRes.data || []
      const current = semesters.value.find((s: any) => s.isCurrent) || semesters.value[0]
      if (current) { queryForm.semesterId = current.id; await fetchData() }
    }
  } catch {}
})
</script>

<style scoped>
.statistics-container { padding: 20px; }
.page-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 20px; }
.page-header h2 { margin: 0; font-size: 20px; font-weight: 500; }
.header-actions { display: flex; gap: 12px; }
.search-card { margin-bottom: 20px; }
</style>
