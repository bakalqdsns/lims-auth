<template>
  <div class="stats-container">
    <div class="page-header">
      <h2>设备统计报表</h2>
      <div>
        <el-button @click="fetchStatistics" :loading="loading">
          <el-icon><Refresh /></el-icon> 刷新
        </el-button>
        <el-button type="primary" @click="handleExport" v-permission="'equipment:export'">
          <el-icon><Download /></el-icon> 导出报表
        </el-button>
      </div>
    </div>

    <el-row :gutter="16" class="stat-cards">
      <el-col :span="6">
        <el-card shadow="hover" class="stat-card">
          <div class="stat-icon" style="background: #409eff">总</div>
          <div class="stat-info">
            <div class="stat-value">{{ stats.totalCount }}</div>
            <div class="stat-label">设备总数</div>
          </div>
        </el-card>
      </el-col>
      <el-col :span="6">
        <el-card shadow="hover" class="stat-card">
          <div class="stat-icon" style="background: #67c23a">可</div>
          <div class="stat-info">
            <div class="stat-value">{{ stats.availableCount }}</div>
            <div class="stat-label">在库可用</div>
          </div>
        </el-card>
      </el-col>
      <el-col :span="6">
        <el-card shadow="hover" class="stat-card">
          <div class="stat-icon" style="background: #e6a23c">借</div>
          <div class="stat-info">
            <div class="stat-value">{{ stats.borrowedCount }}</div>
            <div class="stat-label">当前借出</div>
          </div>
        </el-card>
      </el-col>
      <el-col :span="6">
        <el-card shadow="hover" class="stat-card">
          <div class="stat-icon" style="background: #f56c6c">维</div>
          <div class="stat-info">
            <div class="stat-value">{{ stats.maintenanceCount }}</div>
            <div class="stat-label">维修中</div>
          </div>
        </el-card>
      </el-col>
    </el-row>

    <el-row :gutter="16" style="margin-top: 16px">
      <el-col :span="6">
        <el-card shadow="hover" class="stat-card">
          <div class="stat-icon" style="background: #909399">报</div>
          <div class="stat-info">
            <div class="stat-value">{{ stats.scrapCount }}</div>
            <div class="stat-label">已报废</div>
          </div>
        </el-card>
      </el-col>
      <el-col :span="6">
        <el-card shadow="hover" class="stat-card">
          <div class="stat-icon" style="background: #c0c4cc">丢</div>
          <div class="stat-info">
            <div class="stat-value">{{ stats.lostCount }}</div>
            <div class="stat-label">已丢失</div>
          </div>
        </el-card>
      </el-col>
      <el-col :span="6">
        <el-card shadow="hover" class="stat-card">
          <div class="stat-icon" style="background: #8e44ad">预</div>
          <div class="stat-info">
            <div class="stat-value">{{ stats.reservedCount }}</div>
            <div class="stat-label">已预约</div>
          </div>
        </el-card>
      </el-col>
      <el-col :span="6">
        <el-card shadow="hover" class="stat-card">
          <div class="stat-icon" style="background: #2ecc71">总</div>
          <div class="stat-info">
            <div class="stat-value">¥{{ stats.totalValue?.toLocaleString() }}</div>
            <div class="stat-label">设备总值</div>
          </div>
        </el-card>
      </el-col>
    </el-row>

    <el-row :gutter="16" style="margin-top: 16px">
      <!-- 分类统计 -->
      <el-col :span="12">
        <el-card shadow="never" title="分类统计">
          <template #header>
            <div class="card-header">分类统计</div>
          </template>
          <el-table :data="stats.categoryStats" stripe size="small">
            <el-table-column prop="category" label="分类" />
            <el-table-column prop="totalCount" label="总数" align="center" />
            <el-table-column prop="availableCount" label="可用" align="center">
              <template #default="{ row }">
                <el-tag type="success" size="small">{{ row.availableCount }}</el-tag>
              </template>
            </el-table-column>
            <el-table-column prop="borrowedCount" label="借出" align="center">
              <template #default="{ row }">
                <el-tag type="primary" size="small">{{ row.borrowedCount }}</el-tag>
              </template>
            </el-table-column>
          </el-table>
        </el-card>
      </el-col>

      <!-- 状态分布 -->
      <el-col :span="12">
        <el-card shadow="never">
          <template #header>
            <div class="card-header">状态分布</div>
          </template>
          <div class="status-distribution">
            <div v-for="(count, status) in stats.statusDistribution" :key="status" class="status-item">
              <span class="status-label">{{ status }}</span>
              <el-progress :percentage="stats.totalCount > 0 ? Math.round(count / stats.totalCount * 100) : 0" :color="getStatusColor(status)" />
              <span class="status-count">{{ count }}</span>
            </div>
          </div>
        </el-card>
      </el-col>
    </el-row>

    <!-- 逾期清单 -->
    <el-card shadow="never" style="margin-top: 16px">
      <template #header>
        <div class="card-header">
          <span>逾期清单</span>
          <el-tag type="danger" size="small">{{ stats.overdueRecords?.length || 0 }} 条逾期记录</el-tag>
        </div>
      </template>
      <el-table v-if="stats.overdueRecords?.length" :data="stats.overdueRecords" stripe size="small">
        <el-table-column prop="recordNo" label="单号" width="180" />
        <el-table-column prop="equipmentName" label="设备名称" min-width="150" />
        <el-table-column prop="equipmentCode" label="资产编号" width="120" />
        <el-table-column prop="applicantName" label="申请人" width="100" />
        <el-table-column prop="expectedReturnDate" label="应还日期" width="110">
          <template #default="{ row }">{{ row.expectedReturnDate?.slice(0, 10) }}</template>
        </el-table-column>
        <el-table-column prop="daysOverdue" label="逾期天数" width="100">
          <template #default="{ row }">
            <el-tag type="danger">{{ row.daysOverdue }}天</el-tag>
          </template>
        </el-table-column>
      </el-table>
      <el-empty v-else description="暂无逾期记录" />
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import { Refresh, Download } from '@element-plus/icons-vue'
import { equipmentApi, type EquipmentStatisticsDto } from '@/api/lab'

const loading = ref(false)
const stats = reactive<EquipmentStatisticsDto>({
  totalCount: 0, availableCount: 0, borrowedCount: 0, maintenanceCount: 0,
  scrapCount: 0, lostCount: 0, reservedCount: 0, totalValue: 0,
  categoryDistribution: {}, statusDistribution: {}, categoryStats: [], overdueRecords: []
})

const fetchStatistics = async () => {
  loading.value = true
  try {
    const res = await equipmentApi.getStatistics()
    if (res.data.code === 200) {
      Object.assign(stats, res.data.data)
    }
  } catch {
    ElMessage.error('获取统计数据失败')
  } finally {
    loading.value = false
  }
}

const handleExport = async () => {
  try {
    const res = await equipmentApi.exportExcel()
    const blob = new Blob([res.data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' })
    const url = URL.createObjectURL(blob)
    const a = document.createElement('a')
    a.href = url
    a.download = `设备统计报表_${new Date().toISOString().slice(0, 10)}.xlsx`
    a.click()
    URL.revokeObjectURL(url)
    ElMessage.success('导出成功')
  } catch {
    ElMessage.error('导出失败')
  }
}

const getStatusColor = (status: string): string => {
  const map: Record<string, string> = {
    '在库-可用': '#67c23a', '在库-待维修': '#e6a23c', '在库-已预约': '#909399',
    '借出': '#409eff', '送修': '#f56c6c', '报废': '#c0c4cc', '丢失': '#f56c6c'
  }
  return map[status] || '#409eff'
}

onMounted(() => { fetchStatistics() })
</script>

<style scoped>
.stats-container { padding: 20px; }
.page-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 16px; }
.page-header h2 { margin: 0; font-size: 20px; font-weight: 500; }
.stat-cards { margin-bottom: 0; }
.stat-card { display: flex; align-items: center; gap: 16px; }
.stat-icon { width: 56px; height: 56px; border-radius: 50%; display: flex; align-items: center; justify-content: center; color: #fff; font-size: 20px; font-weight: bold; flex-shrink: 0; }
.stat-info { flex: 1; }
.stat-value { font-size: 28px; font-weight: bold; color: #303133; line-height: 1.2; }
.stat-label { font-size: 13px; color: #909399; margin-top: 4px; }
.card-header { display: flex; justify-content: space-between; align-items: center; }
.status-distribution { display: flex; flex-direction: column; gap: 12px; }
.status-item { display: flex; align-items: center; gap: 12px; }
.status-label { width: 90px; font-size: 13px; color: #606266; flex-shrink: 0; }
.status-count { width: 40px; text-align: right; font-weight: bold; flex-shrink: 0; }
.el-progress { flex: 1; }
</style>
