<template>
  <div class="dashboard-container">
    <div class="page-header">
      <h2>实验室排课预约 - 可视化大屏</h2>
      <div class="header-right">
        <el-select v-model="queryForm.semesterId" placeholder="选择学期" style="width: 200px; margin-right: 12px">
          <el-option v-for="s in semesters" :key="s.id" :label="s.name" :value="s.id" />
        </el-select>
        <el-select v-model="queryForm.weekNumber" placeholder="选择周次" style="width: 120px; margin-right: 12px">
          <el-option v-for="w in 20" :key="w" :label="`第${w}周`" :value="w" />
        </el-select>
      </div>
    </div>

    <div v-loading="loading">
      <!-- 统计卡片 -->
      <el-row :gutter="16" class="stat-row">
        <el-col :span="6">
          <div class="stat-card primary">
            <div class="stat-icon"><el-icon :size="32"><HomeFilled /></el-icon></div>
            <div class="stat-info">
              <div class="stat-value">{{ data.today.totalLabs }}</div>
              <div class="stat-label">实验室总数</div>
            </div>
          </div>
        </el-col>
        <el-col :span="6">
          <div class="stat-card success">
            <div class="stat-icon"><el-icon :size="32"><Clock /></el-icon></div>
            <div class="stat-info">
              <div class="stat-value">{{ data.today.occupiedLabs }}</div>
              <div class="stat-label">当前使用中</div>
            </div>
          </div>
        </el-col>
        <el-col :span="6">
          <div class="stat-card warning">
            <div class="stat-icon"><el-icon :size="32"><TrendCharts /></el-icon></div>
            <div class="stat-info">
              <div class="stat-value">{{ data.today.occupancyRate }}%</div>
              <div class="stat-label">当前占用率</div>
            </div>
          </div>
        </el-col>
        <el-col :span="6">
          <div class="stat-card danger">
            <div class="stat-icon"><el-icon :size="32"><Bell /></el-icon></div>
            <div class="stat-info">
              <div class="stat-value">{{ data.today.pendingReservations }}</div>
              <div class="stat-label">待审批预约</div>
            </div>
          </div>
        </el-col>
      </el-row>

      <el-row :gutter="16" class="stat-row">
        <el-col :span="8">
          <div class="stat-card info">
            <div class="stat-icon"><el-icon :size="32"><Document /></el-icon></div>
            <div class="stat-info">
              <div class="stat-value">{{ data.week.usedSlots }}</div>
              <div class="stat-label">本周排课节次</div>
              <div class="stat-sub">/ 共 {{ data.week.totalSlots }} 节</div>
            </div>
          </div>
        </el-col>
        <el-col :span="8">
          <div class="stat-card success">
            <div class="stat-icon"><el-icon :size="32"><Reading /></el-icon></div>
            <div class="stat-info">
              <div class="stat-value">{{ data.week.totalStudentCount }}</div>
              <div class="stat-label">本周使用人次</div>
            </div>
          </div>
        </el-col>
        <el-col :span="8">
          <div class="stat-card primary">
            <div class="stat-icon"><el-icon :size="32"><Finished /></el-icon></div>
            <div class="stat-info">
              <div class="stat-value">{{ data.completionRate.rate }}%</div>
              <div class="stat-label">登记完成率</div>
              <div class="stat-sub">{{ data.completionRate.completed }} / {{ data.completionRate.total }}</div>
            </div>
          </div>
        </el-col>
      </el-row>

      <el-row :gutter="16" class="stat-row">
        <el-col :span="12">
          <el-card shadow="never">
            <template #header><div class="card-title">实验室使用率排行</div></template>
            <div class="rank-list">
              <div v-for="(lab, idx) in (data.labOccupancyList || []).slice(0, 8)" :key="lab.labName" class="rank-item">
                <div class="rank-index" :class="getRankClass(idx)">{{ idx + 1 }}</div>
                <div class="rank-name">{{ lab.labName }}</div>
                <div class="rank-bar-wrap">
                  <div class="rank-bar" :style="{ width: lab.occupancyRate + '%', background: getProgressColor(lab.occupancyRate) }" />
                </div>
                <div class="rank-value">{{ lab.occupancyRate }}%</div>
              </div>
              <el-empty v-if="!data.labOccupancyList?.length" description="暂无数据" />
            </div>
          </el-card>
        </el-col>
        <el-col :span="12">
          <el-card shadow="never">
            <template #header><div class="card-title">异常提醒 <el-badge :value="(data.alerts || []).length" :hidden="!data.alerts?.length" type="danger" /></div></template>
            <div v-if="data.alerts?.length" class="alert-list">
              <div v-for="alert in data.alerts" :key="alert.message + alert.time" class="alert-item" :class="getAlertClass(alert.type)">
                <div class="alert-icon"><el-icon><Clock v-if="alert.type === 'Overdue'" /><Bell v-else /></el-icon></div>
                <div class="alert-content">
                  <div class="alert-message">{{ alert.message }}</div>
                  <div class="alert-time">{{ formatTime(alert.time) }}</div>
                </div>
              </div>
            </div>
            <el-empty v-else description="暂无异常提醒" />
          </el-card>
        </el-col>
      </el-row>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import { HomeFilled, Clock, TrendCharts, Bell, Document, Reading, Finished } from '@element-plus/icons-vue'

const authHeaders = () => ({ 'Authorization': `Bearer ${localStorage.getItem('token') || ''}` })

const loading = ref(false)
const semesters = ref<{ id: string; name: string }[]>([])

const data = reactive<any>({
  today: {
    totalLabs: 0, occupiedLabs: 0, availableLabs: 0,
    occupancyRate: 0, totalSchedules: 0, pendingReservations: 0, pendingRegistrations: 0
  },
  week: {
    totalSlots: 0, usedSlots: 0, occupancyRate: 0,
    totalStudentCount: 0, totalReservations: 0, approvedReservations: 0,
    totalTeachingApplications: 0, approvedTeachingApplications: 0
  },
  labOccupancyList: [],
  completionRate: { total: 0, completed: 0, pending: 0, overdue: 0, rate: 0 },
  alerts: []
})

const queryForm = reactive({ semesterId: null as string | null, weekNumber: 1 })

const getProgressColor = (pct: number): string => {
  if (pct >= 80) return '#67c23a'
  if (pct >= 50) return '#e6a23c'
  return '#f56c6c'
}

const getRankClass = (idx: number): string => {
  if (idx === 0) return 'rank-gold'
  if (idx === 1) return 'rank-silver'
  if (idx === 2) return 'rank-bronze'
  return ''
}

const getAlertClass = (type: string): string =>
  type === 'Overdue' ? 'alert-danger' : 'alert-warning'

const formatTime = (time: string): string => {
  if (!time) return ''
  const d = new Date(time)
  return `${d.getMonth() + 1}-${d.getDate()} ${d.getHours().toString().padStart(2, '0')}:${d.getMinutes().toString().padStart(2, '0')}`
}

const fetchDashboard = async () => {
  if (!queryForm.semesterId) return
  loading.value = true
  try {
    const res = await fetch(
      `/api/v1/statistics/dashboard?semesterId=${queryForm.semesterId}&weekNumber=${queryForm.weekNumber}`,
      { headers: authHeaders() }
    ).then(async r => {
      if (!r.ok) {
        const text = await r.text()
        console.error('Dashboard API error:', r.status, text)
        throw new Error(text)
      }
      return r.json()
    })
    if (res.code === 200 && res.data) {
      Object.assign(data.today, res.data.today || {})
      Object.assign(data.week, res.data.week || {})
      data.labOccupancyList = res.data.labOccupancyList || []
      data.completionRate = res.data.completionRate || { total: 0, completed: 0, pending: 0, overdue: 0, rate: 0 }
      data.alerts = res.data.alerts || []
    }
  } catch (err) {
    console.error('Dashboard error:', err)
    ElMessage.error('加载大屏数据失败')
  } finally {
    loading.value = false
  }
}

onMounted(async () => {
  try {
    const res = await fetch('/api/v1/semesters', { headers: authHeaders() }).then(r => r.json())
    if (res.code === 200) {
      semesters.value = res.data || []
      const current = semesters.value.find((s: any) => s.isCurrent) || semesters.value[0]
      if (current) {
        queryForm.semesterId = current.id
        await fetchDashboard()
      }
    }
  } catch {}
})
</script>

<style scoped>
.dashboard-container {
  min-height: 100vh;
  background: #f5f7fa;
  padding: 20px;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 16px 24px;
  background: linear-gradient(135deg, #409eff, #1d7dfa);
  color: white;
  border-radius: 8px;
  margin-bottom: 20px;
}

.page-header h2 {
  margin: 0;
  font-size: 20px;
  font-weight: 600;
  letter-spacing: 2px;
}

.header-right {
  display: flex;
  align-items: center;
}

.stat-row {
  margin-bottom: 16px;
}

.stat-card {
  border-radius: 12px;
  padding: 20px;
  display: flex;
  align-items: center;
  gap: 16px;
  color: white;
  transition: transform 0.2s;
}

.stat-card:hover { transform: translateY(-2px); }
.stat-card.primary { background: linear-gradient(135deg, #409eff, #66b1ff); }
.stat-card.success { background: linear-gradient(135deg, #67c23a, #85ce61); }
.stat-card.warning { background: linear-gradient(135deg, #e6a23c, #ebb563); }
.stat-card.danger { background: linear-gradient(135deg, #f56c6c, #f78989); }
.stat-card.info { background: linear-gradient(135deg, #909399, #a6a9ad); }

.stat-value { font-size: 28px; font-weight: 700; }
.stat-label { font-size: 13px; opacity: 0.85; margin-top: 2px; }
.stat-sub { font-size: 12px; opacity: 0.65; margin-top: 2px; }

.card-title { font-size: 15px; font-weight: 600; display: flex; align-items: center; }

.rank-list { display: flex; flex-direction: column; gap: 10px; }
.rank-item { display: flex; align-items: center; gap: 12px; }
.rank-index {
  width: 22px; height: 22px; border-radius: 50%;
  background: #dcdfe6; color: white; font-size: 12px; font-weight: 700;
  display: flex; align-items: center; justify-content: center; flex-shrink: 0;
}
.rank-index.rank-gold { background: linear-gradient(135deg, #f5a623, #f7b500); }
.rank-index.rank-silver { background: linear-gradient(135deg, #9e9e9e, #bdbdbd); }
.rank-index.rank-bronze { background: linear-gradient(135deg, #b87333, #cd853f); }
.rank-name { flex: 1; font-size: 13px; color: #606266; white-space: nowrap; overflow: hidden; text-overflow: ellipsis; }
.rank-bar-wrap { width: 120px; height: 8px; background: #ebeef5; border-radius: 4px; overflow: hidden; flex-shrink: 0; }
.rank-bar { height: 100%; border-radius: 4px; transition: width 0.6s ease; }
.rank-value { width: 44px; font-size: 13px; font-weight: 600; color: #303133; text-align: right; flex-shrink: 0; }

.alert-list { display: flex; flex-direction: column; gap: 8px; max-height: 280px; overflow-y: auto; }
.alert-item { display: flex; align-items: flex-start; gap: 10px; padding: 10px 12px; border-radius: 8px; border-left: 3px solid; }
.alert-item.alert-danger { background: #fef0f0; border-left-color: #f56c6c; }
.alert-item.alert-warning { background: #fdf6ec; border-left-color: #e6a23c; }
.alert-icon { flex-shrink: 0; color: #909399; margin-top: 2px; }
.alert-danger .alert-icon { color: #f56c6c; }
.alert-warning .alert-icon { color: #e6a23c; }
.alert-message { font-size: 13px; color: #606266; line-height: 1.5; }
.alert-time { font-size: 11px; color: #c0c4cc; margin-top: 2px; }
</style>
