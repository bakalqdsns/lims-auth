<template>
  <div class="consumables-container">
    <div class="page-header">
      <h2>耗材管理</h2>
    </div>

    <!-- 统计卡片 -->
    <div class="stats-grid" v-if="isAdmin" v-loading="statsLoading">
      <div class="stat-card">
        <div class="stat-value">{{ stats.totalTypes }}</div>
        <div class="stat-label">耗材种类</div>
      </div>
      <div class="stat-card stat-warning">
        <div class="stat-value">{{ stats.lowStockTypes }}</div>
        <div class="stat-label">库存不足</div>
      </div>
      <div class="stat-card stat-danger">
        <div class="stat-value">{{ stats.outOfStockTypes }}</div>
        <div class="stat-label">已用完</div>
      </div>
      <div class="stat-card stat-info">
        <div class="stat-value">{{ stats.activeTypes }}</div>
        <div class="stat-label">在用种类</div>
      </div>
      <div class="stat-card stat-value">
        <div class="stat-value">¥{{ formatPrice(stats.totalStockValue) }}</div>
        <div class="stat-label">库存总价值</div>
      </div>
      <div class="stat-card">
        <div class="stat-value">{{ stats.totalInRecords }}</div>
        <div class="stat-label">入库记录</div>
      </div>
    </div>

    <!-- 标签页 -->
    <el-tabs v-model="activeTab" class="main-tabs">
      <!-- 耗材列表 -->
      <el-tab-pane label="耗材管理" name="consumables">
        <ConsumableTab @navigate="navigateToStock" @refresh="loadStats" />
      </el-tab-pane>

      <!-- 入库管理 -->
      <el-tab-pane label="入库管理" name="in-records">
        <InRecordTab @refresh="loadStats" />
      </el-tab-pane>

      <!-- 出库管理 -->
      <el-tab-pane label="出库管理" name="out-records">
        <OutRecordTab @refresh="loadStats" />
      </el-tab-pane>

      <!-- 库存管理 -->
      <el-tab-pane label="库存管理" name="stock">
        <StockTab />
      </el-tab-pane>

      <!-- 分类管理 -->
      <el-tab-pane label="分类管理" name="categories" v-if="hasPermission('consumable:create')">
        <CategoryTab @refresh="loadStats" />
      </el-tab-pane>

      <!-- 统计分析 -->
      <el-tab-pane label="数据统计" name="statistics" v-if="hasPermission('consumable:statistics')">
        <StatisticsTab />
      </el-tab-pane>
    </el-tabs>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import { consumableApi, type ConsumableStatisticsDto } from '../../api/consumable'
import { useAuthStore } from '../../stores/auth'
import ConsumableTab from './components/ConsumableTab.vue'
import InRecordTab from './components/InRecordTab.vue'
import OutRecordTab from './components/OutRecordTab.vue'
import StockTab from './components/StockTab.vue'
import CategoryTab from './components/CategoryTab.vue'
import StatisticsTab from './components/StatisticsTab.vue'

const authStore = useAuthStore()
const hasPermission = authStore.hasPermission
const isAdmin = authStore.isAdmin

const activeTab = ref('consumables')
const statsLoading = ref(false)
const stats = ref<ConsumableStatisticsDto>({
  totalTypes: 0,
  lowStockTypes: 0,
  outOfStockTypes: 0,
  activeTypes: 0,
  totalStockValue: 0,
  totalInRecords: 0,
  totalOutRecords: 0,
  totalInAmount: 0,
  totalOutAmount: 0,
  byCategory: {},
  lowStockItems: [],
  monthlyConsumptions: []
})

const navigateToStock = () => {
  activeTab.value = 'stock'
}

const formatPrice = (val: number) => {
  if (!val) return '0.00'
  return val.toFixed(2).replace(/\B(?=(\d{3})+(?!\d))/g, ',')
}

const loadStats = async () => {
  statsLoading.value = true
  try {
    const res = await consumableApi.getStatistics()
    if (res.data.code === 200) {
      stats.value = res.data.data
    }
  } catch {
    // ignore
  } finally {
    statsLoading.value = false
  }
}

onMounted(() => {
  loadStats()
})
</script>

<style scoped>
.consumables-container {
  padding: 0;
}

.page-header {
  margin-bottom: 16px;
}

.page-header h2 {
  margin: 0;
  font-size: 18px;
  font-weight: 600;
  color: #303133;
}

.stats-grid {
  display: grid;
  grid-template-columns: repeat(6, 1fr);
  gap: 12px;
  margin-bottom: 16px;
}

.stat-card {
  background: #fff;
  border-radius: 8px;
  padding: 16px;
  text-align: center;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.06);
}

.stat-value {
  font-size: 24px;
  font-weight: 700;
  color: #409eff;
  line-height: 1.2;
}

.stat-label {
  font-size: 12px;
  color: #909399;
  margin-top: 6px;
}

.stat-warning .stat-value { color: #e6a23c; }
.stat-danger .stat-value { color: #f56c6c; }
.stat-info .stat-value { color: #67c23a; }
.stat-value .stat-value { color: #409eff; }

.main-tabs {
  background: #fff;
  border-radius: 8px;
  padding: 16px;
}

@media (max-width: 1200px) {
  .stats-grid {
    grid-template-columns: repeat(3, 1fr);
  }
}
</style>
