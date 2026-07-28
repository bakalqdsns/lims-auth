<template>
  <div class="tab-content">
    <el-tabs v-model="stockSubTab">
      <!-- 实时库存查询 -->
      <el-tab-pane label="实时库存" name="stock-query">
        <div class="toolbar">
          <el-button type="warning" @click="handleStockCheck" v-permission="'consumable:adjust'">
            <el-icon><List /></el-icon> 库存盘点
          </el-button>
          <el-button @click="handleExportLogs">
            <el-icon><Download /></el-icon> 导出记录
          </el-button>
        </div>

        <el-card shadow="never" class="search-card">
          <el-form :model="queryForm" inline>
            <el-form-item label="关键词">
              <el-input v-model="queryForm.keyword" placeholder="耗材名称/编号" clearable @keyup.enter="loadStockData" />
            </el-form-item>
            <el-form-item label="分类">
              <el-select v-model="queryForm.categoryId" placeholder="全部" clearable style="width:150px">
                <el-option v-for="cat in categories" :key="cat.id" :label="cat.name" :value="cat.id" />
              </el-select>
            </el-form-item>
            <el-form-item>
              <el-button type="primary" @click="loadStockData"><el-icon><Search /></el-icon> 搜索</el-button>
              <el-button @click="queryForm.keyword='';queryForm.categoryId='';loadStockData()">重置</el-button>
            </el-form-item>
          </el-form>
        </el-card>

        <el-card shadow="never">
          <el-table v-loading="stockLoading" :data="stockList" stripe>
            <el-table-column prop="code" label="耗材编号" width="120" />
            <el-table-column prop="name" label="耗材名称" min-width="160" />
            <el-table-column prop="categoryName" label="分类" width="100" />
            <el-table-column prop="unit" label="单位" width="60" />
            <el-table-column label="当前库存" width="100" align="right">
              <template #default="{ row }">
                <span :class="{ 'low-stock': row.isLowStock }">{{ row.currentStock }}</span>
              </template>
            </el-table-column>
            <el-table-column label="可用库存" width="100" align="right">
              <template #default="{ row }">{{ row.availableStock }}</template>
            </el-table-column>
            <el-table-column label="锁定库存" width="100" align="right">
              <template #default="{ row }">{{ row.lockedStock }}</template>
            </el-table-column>
            <el-table-column label="最低库存" width="90" align="right">
              <template #default="{ row }">{{ row.minStock }}</template>
            </el-table-column>
            <el-table-column prop="location" label="存放位置" width="120" />
            <el-table-column label="状态" width="90">
              <template #default="{ row }">
                <el-tag v-if="row.isLowStock" type="danger" size="small">库存不足</el-tag>
                <el-tag v-else type="success" size="small">正常</el-tag>
              </template>
            </el-table-column>
            <el-table-column label="操作" width="140" fixed="right">
              <template #default="{ row }">
                <el-button link type="primary" @click="handleAdjust(row)" v-permission="'consumable:adjust'">调整</el-button>
                <el-button link type="info" @click="viewLogs(row)">明细</el-button>
              </template>
            </el-table-column>
          </el-table>
          <div class="pagination-wrapper">
            <el-pagination
              v-model:current-page="pagination.page"
              v-model:page-size="pagination.pageSize"
              :page-sizes="[10, 20, 50, 100]"
              :total="total"
              layout="total, sizes, prev, pager, next, jumper"
              @size-change="()=>{ pagination.page=1; loadStockData() }"
              @current-change="loadStockData"
            />
          </div>
        </el-card>
      </el-tab-pane>

      <!-- 库存日志 -->
      <el-tab-pane label="库存日志" name="logs">
        <el-card shadow="never" class="search-card">
          <el-form :model="logQuery" inline>
            <el-form-item label="耗材">
              <el-select v-model="logQuery.consumableId" placeholder="全部" clearable filterable style="width:200px">
                <el-option v-for="c in consumables" :key="c.id" :label="c.name" :value="c.id" />
              </el-select>
            </el-form-item>
            <el-form-item label="变动类型">
              <el-select v-model="logQuery.changeType" placeholder="全部" clearable style="width:150px">
                <el-option v-for="(label, key) in STOCK_CHANGE_TYPES" :key="key" :label="label" :value="key" />
              </el-select>
            </el-form-item>
            <el-form-item label="时间范围">
              <el-date-picker v-model="logDateRange" type="daterange" range-separator="至"
                value-format="YYYY-MM-DD" style="width:240px" @change="onLogDateChange" />
            </el-form-item>
            <el-form-item>
              <el-button type="primary" @click="loadLogData"><el-icon><Search /></el-icon> 搜索</el-button>
              <el-button @click="logQuery.consumableId='';logQuery.changeType='';logDateRange=[];loadLogData()">重置</el-button>
            </el-form-item>
          </el-form>
        </el-card>
        <el-card shadow="never">
          <el-table v-loading="logLoading" :data="logList" stripe>
            <el-table-column prop="createdAt" label="时间" width="160">
              <template #default="{ row }">{{ formatDate(row.createdAt) }}</template>
            </el-table-column>
            <el-table-column prop="consumableName" label="耗材名称" min-width="140" />
            <el-table-column prop="changeType" label="变动类型" width="100">
              <template #default="{ row }">{{ STOCK_CHANGE_TYPES[row.changeType] || row.changeType }}</template>
            </el-table-column>
            <el-table-column label="变动数量" width="100" align="right">
              <template #default="{ row }">
                <span :class="row.changeQuantity > 0 ? 'text-success' : 'text-danger'">
                  {{ row.changeQuantity > 0 ? '+' : '' }}{{ row.changeQuantity }}
                </span>
              </template>
            </el-table-column>
            <el-table-column label="变动前" width="80" align="right">
              <template #default="{ row }">{{ row.beforeStock }}</template>
            </el-table-column>
            <el-table-column label="变动后" width="80" align="right">
              <template #default="{ row }">{{ row.afterStock }}</template>
            </el-table-column>
            <el-table-column prop="referenceNo" label="关联单号" width="180" />
            <el-table-column prop="operatorName" label="操作人" width="100" />
            <el-table-column prop="remark" label="备注" min-width="150" />
          </el-table>
          <div class="pagination-wrapper">
            <el-pagination
              v-model:current-page="logPagination.page"
              v-model:page-size="logPagination.pageSize"
              :page-sizes="[10, 20, 50, 100]"
              :total="logTotal"
              layout="total, sizes, prev, pager, next, jumper"
              @size-change="()=>{ logPagination.page=1; loadLogData() }"
              @current-change="loadLogData"
            />
          </div>
        </el-card>
      </el-tab-pane>

      <!-- 调整记录 -->
      <el-tab-pane label="调整记录" name="adjustments">
        <el-card shadow="never" class="search-card">
          <el-form :model="adjQuery" inline>
            <el-form-item label="耗材">
              <el-select v-model="adjQuery.consumableId" placeholder="全部" clearable filterable style="width:200px">
                <el-option v-for="c in consumables" :key="c.id" :label="c.name" :value="c.id" />
              </el-select>
            </el-form-item>
            <el-form-item label="调整类型">
              <el-select v-model="adjQuery.adjustmentType" placeholder="全部" clearable style="width:150px">
                <el-option v-for="t in ADJUSTMENT_TYPES" :key="t" :label="t" :value="t" />
              </el-select>
            </el-form-item>
            <el-form-item>
              <el-button type="primary" @click="loadAdjData"><el-icon><Search /></el-icon> 搜索</el-button>
              <el-button @click="adjQuery.consumableId='';adjQuery.adjustmentType='';loadAdjData()">重置</el-button>
            </el-form-item>
          </el-form>
        </el-card>
        <el-card shadow="never">
          <el-table v-loading="adjLoading" :data="adjList" stripe>
            <el-table-column prop="createdAt" label="调整时间" width="160">
              <template #default="{ row }">{{ formatDate(row.createdAt) }}</template>
            </el-table-column>
            <el-table-column prop="consumableName" label="耗材名称" min-width="140" />
            <el-table-column prop="adjustmentType" label="调整类型" width="100" />
            <el-table-column label="调整前" width="80" align="right">
              <template #default="{ row }">{{ row.beforeQuantity }}</template>
            </el-table-column>
            <el-table-column label="调整量" width="100" align="right">
              <template #default="{ row }">
                <span :class="row.adjustmentQuantity > 0 ? 'text-success' : 'text-danger'">
                  {{ row.adjustmentQuantity > 0 ? '+' : '' }}{{ row.adjustmentQuantity }}
                </span>
              </template>
            </el-table-column>
            <el-table-column label="调整后" width="80" align="right">
              <template #default="{ row }">{{ row.afterQuantity }}</template>
            </el-table-column>
            <el-table-column prop="reason" label="调整原因" min-width="150" />
            <el-table-column prop="operatorName" label="操作人" width="100" />
          </el-table>
          <div class="pagination-wrapper">
            <el-pagination
              v-model:current-page="adjPagination.page"
              v-model:page-size="adjPagination.pageSize"
              :page-sizes="[10, 20, 50, 100]"
              :total="adjTotal"
              layout="total, sizes, prev, pager, next, jumper"
              @size-change="()=>{ adjPagination.page=1; loadAdjData() }"
              @current-change="loadAdjData"
            />
          </div>
        </el-card>
      </el-tab-pane>
    </el-tabs>

    <!-- 库存调整对话框 -->
    <el-dialog v-model="adjustDialogVisible" title="库存调整" width="500px">
      <el-form label-width="100px">
        <el-descriptions :column="2" border size="small" style="margin-bottom:16px">
          <el-descriptions-item label="耗材名称" :span="2">{{ adjustTarget?.name }}</el-descriptions-item>
          <el-descriptions-item label="当前库存">{{ adjustTarget?.currentStock }} {{ adjustTarget?.unit }}</el-descriptions-item>
          <el-descriptions-item label="可用库存">{{ adjustTarget?.availableStock }} {{ adjustTarget?.unit }}</el-descriptions-item>
        </el-descriptions>
        <el-form-item label="调整类型" required>
          <el-select v-model="adjustForm.adjustmentType" style="width:100%">
            <el-option v-for="t in ADJUSTMENT_TYPES" :key="t" :label="t" :value="t" />
          </el-select>
        </el-form-item>
        <el-form-item label="调整数量" required>
          <el-input-number v-model="adjustForm.adjustmentQuantity" :precision="3" style="width:100%" />
          <div class="form-tip">正数增加库存，负数减少库存</div>
        </el-form-item>
        <el-form-item label="调整原因">
          <el-input v-model="adjustForm.reason" type="textarea" :rows="3" placeholder="请输入调整原因" />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="adjustDialogVisible = false">取消</el-button>
        <el-button type="primary" :loading="adjustSaving" @click="confirmAdjust">确认调整</el-button>
      </template>
    </el-dialog>

    <!-- 盘点对话框 -->
    <el-dialog v-model="checkDialogVisible" title="库存盘点" width="700px" destroy-on-close>
      <div class="check-toolbar">
        <el-button type="primary" size="small" @click="handleSelectAllConsumables">加载全部耗材</el-button>
        <span class="check-tip">请输入每个耗材的实际盘点数量</span>
      </div>
      <el-form :model="checkForm" label-width="100px" style="max-height:400px;overflow-y:auto">
        <el-form-item v-for="item in checkItems" :key="item.consumableId" :label="item.name">
          <el-input-number v-model="item.actualQuantity" :min="0" :precision="3" style="width:150px" />
          <span style="margin-left:8px;color:#999">{{ item.unit }}（系统库存：{{ item.currentStock }}）</span>
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="checkDialogVisible = false">取消</el-button>
        <el-button type="primary" :loading="checkSaving" @click="confirmCheck">确认盘点</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, type FormInstance } from 'element-plus'
import { Search, Download, List } from '@element-plus/icons-vue'
import { consumableApi,
  type ConsumableDto,
  type ConsumableStockLogDto,
  type ConsumableStockAdjustmentDto,
  STOCK_CHANGE_TYPES,
  ADJUSTMENT_TYPES } from '../../../api/consumable'

const hasPermission = (p: string) => {
  const auth = (window as any).__pinia?.stateTree?.auth
  return auth ? auth.hasPermission(p) : false
}

const stockSubTab = ref('stock-query')
const stockLoading = ref(false)
const logLoading = ref(false)
const adjLoading = ref(false)
const adjustDialogVisible = ref(false)
const adjustSaving = ref(false)
const checkDialogVisible = ref(false)
const checkSaving = ref(false)

const stockList = ref<ConsumableDto[]>([])
const logList = ref<ConsumableStockLogDto[]>([])
const adjList = ref<ConsumableStockAdjustmentDto[]>([])
const categories = ref<any[]>([])
const consumables = ref<ConsumableDto[]>([])
const total = ref(0)
const logTotal = ref(0)
const adjTotal = ref(0)

const pagination = reactive({ page: 1, pageSize: 20 })
const logPagination = reactive({ page: 1, pageSize: 20 })
const adjPagination = reactive({ page: 1, pageSize: 20 })
const logDateRange = ref<string[]>([])

const queryForm = reactive({ keyword: '', categoryId: '' })
const logQuery = reactive({ consumableId: '', changeType: '', startDate: '', endDate: '' })
const adjQuery = reactive({ consumableId: '', adjustmentType: '' })

const adjustTarget = ref<ConsumableDto | null>(null)
const adjustForm = reactive({ adjustmentType: '盘点', adjustmentQuantity: 0, reason: '' })

interface CheckItem { consumableId: string; name: string; unit: string; currentStock: number; actualQuantity: number }
const checkItems = ref<CheckItem[]>([])

const formatDate = (d: string) => d ? new Date(d).toLocaleString('zh-CN', { hour12: false }) : '-'
const onLogDateChange = (val: string[]) => {
  logQuery.startDate = val?.[0] || ''
  logQuery.endDate = val?.[1] || ''
}

const loadStockData = async () => {
  stockLoading.value = true
  try {
    const params: any = { page: pagination.page, pageSize: pagination.pageSize }
    if (queryForm.keyword) params.keyword = queryForm.keyword
    if (queryForm.categoryId) params.categoryId = queryForm.categoryId
    const res = await consumableApi.getConsumables(params)
    if (res.data.code === 200) {
      stockList.value = res.data.data
      total.value = res.data.total
    }
  } catch {} finally { stockLoading.value = false }
}

const loadLogData = async () => {
  logLoading.value = true
  try {
    const params: any = { page: logPagination.page, pageSize: logPagination.pageSize }
    if (logQuery.consumableId) params.consumableId = logQuery.consumableId
    if (logQuery.changeType) params.changeType = logQuery.changeType
    if (logQuery.startDate) params.startDate = logQuery.startDate
    if (logQuery.endDate) params.endDate = logQuery.endDate
    const res = await consumableApi.getStockLogs(params)
    if (res.data.code === 200) {
      logList.value = res.data.data
      logTotal.value = res.data.total
    }
  } catch {} finally { logLoading.value = false }
}

const loadAdjData = async () => {
  adjLoading.value = true
  try {
    const params: any = { page: adjPagination.page, pageSize: adjPagination.pageSize }
    if (adjQuery.consumableId) params.consumableId = adjQuery.consumableId
    if (adjQuery.adjustmentType) params.adjustmentType = adjQuery.adjustmentType
    const res = await consumableApi.getStockAdjustments(params)
    if (res.data.code === 200) {
      adjList.value = res.data.data
      adjTotal.value = res.data.total
    }
  } catch {} finally { adjLoading.value = false }
}

const loadCategories = async () => {
  try {
    const res = await consumableApi.getCategories()
    if (res.data.code === 200) categories.value = res.data.data
  } catch {}
}

const loadConsumables = async () => {
  try {
    const res = await consumableApi.getConsumables({ page: 1, pageSize: 500 } as any)
    if (res.data.code === 200) consumables.value = res.data.data
  } catch {}
}

const handleAdjust = (row: ConsumableDto) => {
  adjustTarget.value = row
  Object.assign(adjustForm, { adjustmentType: '盘点', adjustmentQuantity: 0, reason: '' })
  adjustDialogVisible.value = true
}

const confirmAdjust = async () => {
  if (!adjustForm.adjustmentType || adjustForm.adjustmentQuantity === 0) {
    ElMessage.warning('请填写完整的调整信息')
    return
  }
  adjustSaving.value = true
  try {
    const res = await consumableApi.adjustStock({
      consumableId: adjustTarget.value!.id,
      adjustmentType: adjustForm.adjustmentType,
      adjustmentQuantity: adjustForm.adjustmentQuantity,
      reason: adjustForm.reason || undefined
    })
    if (res.data.code === 200) {
      ElMessage.success('库存调整成功')
      adjustDialogVisible.value = false
      loadStockData()
    } else {
      ElMessage.error(res.data.message)
    }
  } catch (err: any) {
    ElMessage.error(err?.response?.data?.message || '调整失败')
  } finally {
    adjustSaving.value = false
  }
}

const handleSelectAllConsumables = async () => {
  const res = await consumableApi.getConsumables({ page: 1, pageSize: 500 } as any)
  if (res.data.code === 200) {
    checkItems.value = res.data.data.map((c: ConsumableDto) => ({
      consumableId: c.id,
      name: c.name,
      unit: c.unit,
      currentStock: c.currentStock,
      actualQuantity: c.currentStock
    }))
  }
}

const confirmCheck = async () => {
  checkSaving.value = true
  try {
    const res = await consumableApi.stockCheck({
      items: checkItems.value.map(i => ({ consumableId: i.consumableId, actualQuantity: i.actualQuantity }))
    })
    if (res.data.code === 200) {
      ElMessage.success('盘点完成：' + res.data.message)
      checkDialogVisible.value = false
      loadStockData()
    } else {
      ElMessage.error(res.data.message)
    }
  } catch (err: any) {
    ElMessage.error(err?.response?.data?.message || '盘点失败')
  } finally {
    checkSaving.value = false
  }
}

const viewLogs = (row: ConsumableDto) => {
  stockSubTab.value = 'logs'
  logQuery.consumableId = row.id
  loadLogData()
}

const handleStockCheck = () => {
  checkItems.value = []
  checkDialogVisible.value = true
}

const handleExportLogs = () => {
  ElMessage.info('导出功能开发中')
}

onMounted(() => {
  loadStockData()
  loadCategories()
  loadConsumables()
})
</script>

<style scoped>
.tab-content { padding-top: 12px; }
.toolbar { margin-bottom: 12px; display: flex; gap: 8px; }
.search-card { margin-bottom: 12px; }
.pagination-wrapper { margin-top: 16px; display: flex; justify-content: flex-end; }
.low-stock { color: #f56c6c; font-weight: 600; }
.text-success { color: #67c23a; }
.text-danger { color: #f56c6c; }
.form-tip { font-size: 12px; color: #909399; margin-top: 4px; }
.check-toolbar { display: flex; align-items: center; gap: 12px; margin-bottom: 12px; }
.check-tip { color: #909399; font-size: 13px; }
</style>
