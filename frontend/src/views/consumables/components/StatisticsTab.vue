<template>
  <div class="tab-content">
    <el-card shadow="never" class="search-card">
      <el-form :model="queryForm" inline>
        <el-form-item label="分类">
          <el-select v-model="queryForm.categoryId" placeholder="全部" clearable style="width:150px">
            <el-option v-for="cat in categories" :key="cat.id" :label="cat.name" :value="cat.id" />
          </el-select>
        </el-form-item>
        <el-form-item label="时间范围">
          <el-date-picker v-model="dateRange" type="daterange" range-separator="至"
            value-format="YYYY-MM-DD" style="width:240px" @change="onDateChange" />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="loadData"><el-icon><Search /></el-icon> 查询</el-button>
          <el-button @click="queryForm.categoryId='';dateRange=[];loadData()">重置</el-button>
        </el-form-item>
      </el-form>
    </el-card>

    <!-- 概览卡片 -->
    <div class="stats-grid">
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
      <div class="stat-card stat-success">
        <div class="stat-value">¥{{ formatPrice(stats.totalStockValue) }}</div>
        <div class="stat-label">库存总价值</div>
      </div>
      <div class="stat-card">
        <div class="stat-value">{{ stats.totalInRecords }}</div>
        <div class="stat-label">入库记录数</div>
      </div>
      <div class="stat-card">
        <div class="stat-value">{{ stats.totalOutRecords }}</div>
        <div class="stat-label">出库记录数</div>
      </div>
    </div>

    <!-- 低库存预警 -->
    <el-card shadow="never" class="section-card" v-if="stats.lowStockItems?.length">
      <template #header>
        <div class="card-header">
          <span>低库存预警</span>
          <el-tag type="danger" size="small">{{ stats.lowStockItems.length }} 项</el-tag>
        </div>
      </template>
      <el-table :data="stats.lowStockItems" stripe size="small">
        <el-table-column prop="consumableName" label="耗材名称" min-width="150" />
        <el-table-column prop="categoryName" label="分类" width="100" />
        <el-table-column label="当前库存" width="100" align="right">
          <template #default="{ row }">
            <span class="low-stock">{{ row.currentStock }} {{ row.unit }}</span>
          </template>
        </el-table-column>
        <el-table-column label="最低库存" width="100" align="right">
          <template #default="{ row }">{{ row.minStock }} {{ row.unit }}</template>
        </el-table-column>
        <el-table-column label="预警等级" width="100">
          <template #default="{ row }">
            <el-tag v-if="row.currentStock === 0" type="danger" size="small">已用完</el-tag>
            <el-tag v-else type="warning" size="small">不足</el-tag>
          </template>
        </el-table-column>
      </el-table>
    </el-card>

    <!-- 月度消耗趋势 -->
    <el-card shadow="never" class="section-card">
      <template #header>
        <div class="card-header">
          <span>月度消耗趋势</span>
        </div>
      </template>
      <div class="chart-area" v-if="monthlyChartData.length">
        <div class="chart-bars">
          <div class="chart-bar" v-for="item in monthlyChartData" :key="item.month">
            <div class="bar-value">{{ item.quantity.toFixed(0) }}</div>
            <div class="bar-fill" :style="{ height: getBarHeight(item.quantity) + 'px' }"></div>
            <div class="bar-label">{{ item.month }}</div>
          </div>
        </div>
      </div>
      <el-empty v-else description="暂无消耗数据" />

      <!-- 表格 -->
      <el-table :data="stats.monthlyConsumptions" stripe size="small" style="margin-top:16px">
        <el-table-column prop="month" label="月份" width="120" />
        <el-table-column prop="recordCount" label="领用次数" width="100" align="center" />
        <el-table-column label="消耗总量" width="120" align="right">
          <template #default="{ row }">{{ row.quantity }}</template>
        </el-table-column>
        <el-table-column label="趋势" width="100">
          <template #default="{ row }">
            <span class="trend-arrow" v-if="row.quantity > averageConsumption">
              <el-icon color="#67c23a"><ArrowUp /></el-icon>
            </span>
            <span class="trend-arrow" v-else>
              <el-icon color="#909399"><ArrowDown /></el-icon>
            </span>
          </template>
        </el-table-column>
      </el-table>
    </el-card>

    <!-- 分类统计 -->
    <el-card shadow="never" class="section-card">
      <template #header>
        <div class="card-header"><span>分类分布</span></div>
      </template>
      <div class="category-bars">
        <div class="category-bar-item" v-for="(count, name) in stats.byCategory" :key="name">
          <div class="category-name">{{ name }}</div>
          <div class="category-bar">
            <div class="category-fill" :style="{ width: getCategoryPercent(count) + '%' }"></div>
          </div>
          <div class="category-count">{{ count }} 种</div>
        </div>
      </div>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted } from 'vue'
import { Search, ArrowUp, ArrowDown } from '@element-plus/icons-vue'
import { consumableApi, type ConsumableStatisticsDto } from '../../../api/consumable'

const loading = ref(false)
const categories = ref<any[]>([])
const dateRange = ref<string[]>([])
const stats = ref<ConsumableStatisticsDto>({
  totalTypes: 0, lowStockTypes: 0, outOfStockTypes: 0, activeTypes: 0,
  totalStockValue: 0, totalInRecords: 0, totalOutRecords: 0,
  totalInAmount: 0, totalOutAmount: 0, byCategory: {},
  lowStockItems: [], monthlyConsumptions: []
})

const queryForm = reactive({ categoryId: '', startDate: '', endDate: '' })

const onDateChange = (val: string[]) => {
  queryForm.startDate = val?.[0] || ''
  queryForm.endDate = val?.[1] || ''
}

const formatPrice = (val: number) => val ? val.toFixed(2).replace(/\B(?=(\d{3})+(?!\d))/g, ',') : '0.00'

const monthlyChartData = computed(() => (stats.value.monthlyConsumptions || []).slice().reverse())
const averageConsumption = computed(() => {
  const arr = stats.value.monthlyConsumptions || []
  if (!arr.length) return 0
  return arr.reduce((s, i) => s + i.quantity, 0) / arr.length
})
const maxConsumption = computed(() => {
  return Math.max(...(stats.value.monthlyConsumptions || []).map(i => i.quantity), 1)
})
const getBarHeight = (val: number) => Math.max(10, (val / maxConsumption.value) * 120)

const getCategoryPercent = (count: number) => {
  const total = Object.values(stats.value.byCategory || {}).reduce((s, v) => s + (v as number), 0)
  return total ? (count / total * 100).toFixed(1) : 0
}

const loadData = async () => {
  loading.value = true
  try {
    const params: any = {}
    if (queryForm.categoryId) params.categoryId = queryForm.categoryId
    if (queryForm.startDate) params.startDate = queryForm.startDate
    if (queryForm.endDate) params.endDate = queryForm.endDate
    const res = await consumableApi.getStatistics(params)
    if (res.data.code === 200) stats.value = res.data.data
  } catch {} finally { loading.value = false }
}

const loadCategories = async () => {
  try {
    const res = await consumableApi.getCategories()
    if (res.data.code === 200) categories.value = res.data.data
  } catch {}
}

onMounted(() => { loadData(); loadCategories() })
</script>

<style scoped>
.tab-content { padding-top: 12px; }
.search-card { margin-bottom: 16px; }
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
  box-shadow: 0 1px 3px rgba(0,0,0,0.06);
}
.stat-value { font-size: 24px; font-weight: 700; color: #409eff; }
.stat-label { font-size: 12px; color: #909399; margin-top: 6px; }
.stat-warning .stat-value { color: #e6a23c; }
.stat-danger .stat-value { color: #f56c6c; }
.stat-success .stat-value { color: #67c23a; }

.section-card { margin-bottom: 16px; }
.card-header { display: flex; justify-content: space-between; align-items: center; }

.low-stock { color: #f56c6c; font-weight: 600; }

.chart-area { padding: 20px 0; }
.chart-bars { display: flex; align-items: flex-end; gap: 12px; height: 160px; }
.chart-bar { display: flex; flex-direction: column; align-items: center; flex: 1; height: 100%; }
.bar-value { font-size: 12px; color: #606266; margin-bottom: 4px; }
.bar-fill { width: 100%; max-width: 60px; background: #409eff; border-radius: 4px 4px 0 0; transition: height 0.3s; }
.bar-label { font-size: 12px; color: #909399; margin-top: 6px; }

.trend-arrow { font-size: 16px; }

.category-bars { padding: 16px 0; }
.category-bar-item { display: flex; align-items: center; gap: 12px; margin-bottom: 12px; }
.category-name { width: 100px; font-size: 13px; color: #606266; text-align: right; flex-shrink: 0; }
.category-bar { flex: 1; height: 20px; background: #f0f2f5; border-radius: 4px; overflow: hidden; }
.category-fill { height: 100%; background: #409eff; border-radius: 4px; transition: width 0.5s; }
.category-count { width: 60px; font-size: 13px; color: #909399; flex-shrink: 0; }
</style>
