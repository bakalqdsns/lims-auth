<template>
  <div class="schedule-search-container">
    <div class="page-header">
      <h2>排课查询</h2>
      <div class="header-actions">
        <el-button type="success" :loading="exporting" @click="handleExport">
          <el-icon><Download /></el-icon>
          导出
        </el-button>
      </div>
    </div>

    <!-- 搜索栏 -->
    <el-card class="search-card" shadow="never">
      <el-form :model="queryForm" inline>
        <el-form-item label="学期">
          <el-select
            v-model="queryForm.semesterId"
            placeholder="请选择学期"
            style="width: 200px"
            clearable
            @change="loadOptions"
          >
            <el-option
              v-for="s in semesters"
              :key="s.id"
              :label="s.name"
              :value="s.id"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="教学周">
          <el-input-number
            v-model="queryForm.weekNumber"
            :min="1"
            :max="20"
            style="width: 120px"
          />
        </el-form-item>
        <el-form-item label="楼宇">
          <el-select
            v-model="queryForm.buildingId"
            clearable
            placeholder="全部"
            style="width: 160px"
            @change="handleBuildingChange"
          >
            <el-option
              v-for="b in buildings"
              :key="b.id"
              :label="b.name"
              :value="b.id"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="实验室">
          <el-select
            v-model="queryForm.labId"
            clearable
            placeholder="全部"
            style="width: 180px"
          >
            <el-option
              v-for="l in filteredLabs"
              :key="l.id"
              :label="l.name"
              :value="l.id"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="排课来源">
          <el-select
            v-model="queryForm.source"
            clearable
            placeholder="全部"
            style="width: 140px"
          >
            <el-option label="集中排课" value="CentralScheduling" />
            <el-option label="预约" value="Reservation" />
            <el-option label="授课申请" value="TeachingRequest" />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="fetchData">
            <el-icon><Search /></el-icon>
            搜索
          </el-button>
          <el-button @click="handleReset">重置</el-button>
        </el-form-item>
      </el-form>
    </el-card>

    <!-- 列表视图 -->
    <el-card class="list-card" shadow="never">
      <el-table
        v-loading="loading"
        :data="listData"
        stripe
        @selection-change="handleSelectionChange"
      >
        <el-table-column type="selection" width="40" />
        <el-table-column type="index" label="序号" width="60" />
        <el-table-column prop="weekNumber" label="周次" width="70" />
        <el-table-column prop="dayOfWeek" label="星期" width="70">
          <template #default="{ row }">
            {{ formatDayOfWeek(row.dayOfWeek) }}
          </template>
        </el-table-column>
        <el-table-column prop="periodNumber" label="节次" width="70" />
        <el-table-column prop="courseName" label="课程/项目" min-width="140" show-overflow-tooltip />
        <el-table-column prop="teacherName" label="教师" width="100" show-overflow-tooltip />
        <el-table-column prop="className" label="班级" width="140" show-overflow-tooltip />
        <el-table-column prop="labName" label="实验室" width="140" show-overflow-tooltip />
        <el-table-column prop="source" label="来源" width="100">
          <template #default="{ row }">
            {{ formatSource(row.source) }}
          </template>
        </el-table-column>
        <el-table-column prop="studentCount" label="人数" width="70" />
        <el-table-column prop="hasConflict" label="冲突" width="70" align="center">
          <template #default="{ row }">
            <el-tag v-if="row.hasConflict" type="danger" size="small">有冲突</el-tag>
            <span v-else>-</span>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="120" fixed="right">
          <template #default="{ row }">
            <el-button link type="primary" size="small" @click="handleView(row)">详情</el-button>
            <el-button link type="primary" size="small" @click="handleEdit(row)">编辑</el-button>
          </template>
        </el-table-column>
      </el-table>

      <div class="pagination-container">
        <el-pagination
          v-model:current-page="queryForm.page"
          v-model:page-size="queryForm.pageSize"
          :page-sizes="[20, 50, 100]"
          :total="total"
          layout="total, sizes, prev, pager, next"
          @size-change="fetchData"
          @current-change="fetchData"
        />
      </div>
    </el-card>

    <!-- 详情对话框 -->
    <el-dialog v-model="detailDialogVisible" title="排课详情" width="600px" destroy-on-close>
      <el-descriptions v-if="currentRow" :column="2" border>
        <el-descriptions-item label="学期">{{ currentRow.semesterName || '-' }}</el-descriptions-item>
        <el-descriptions-item label="周次">第{{ currentRow.weekNumber }}周</el-descriptions-item>
        <el-descriptions-item label="星期">{{ formatDayOfWeek(currentRow.dayOfWeek) }}</el-descriptions-item>
        <el-descriptions-item label="节次">第{{ currentRow.periodNumber }}节</el-descriptions-item>
        <el-descriptions-item label="课程/项目">{{ currentRow.courseName || currentRow.projectName || '-' }}</el-descriptions-item>
        <el-descriptions-item label="来源">{{ formatSource(currentRow.source) }}</el-descriptions-item>
        <el-descriptions-item label="教师">{{ currentRow.teacherName || '-' }}</el-descriptions-item>
        <el-descriptions-item label="班级">{{ currentRow.className || '-' }}</el-descriptions-item>
        <el-descriptions-item label="实验室">{{ currentRow.labName || '-' }}</el-descriptions-item>
        <el-descriptions-item label="人数">{{ currentRow.studentCount || '-' }}</el-descriptions-item>
        <el-descriptions-item label="地点">{{ currentRow.buildingName || '' }} {{ currentRow.roomNumber || '' }}</el-descriptions-item>
        <el-descriptions-item label="冲突">{{ currentRow.hasConflict ? '有冲突' : '无' }}</el-descriptions-item>
        <el-descriptions-item label="备注" :span="2">{{ currentRow.remark || '-' }}</el-descriptions-item>
        <el-descriptions-item v-if="currentRow.conflictInfo" label="冲突信息" :span="2">
          <el-text type="danger">{{ currentRow.conflictInfo }}</el-text>
        </el-descriptions-item>
      </el-descriptions>
      <template #footer>
        <span v-if="!currentRow">加载中...</span>
        <span v-else>--</span>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import { Search, Download } from '@element-plus/icons-vue'

// 学期 API
const fetchSemesters = () => {
  return fetch('/api/v1/semesters/current', {
    headers: {
      'Authorization': `Bearer ${localStorage.getItem('token') || ''}`
    }
  }).then(r => r.json())
}

// 楼宇列表
const fetchBuildings = () => {
  return fetch('/api/v1/buildings', {
    headers: {
      'Authorization': `Bearer ${localStorage.getItem('token') || ''}`
    }
  }).then(r => r.json())
}

// 实验室列表
const fetchLabs = () => {
  return fetch('/api/v1/labs', {
    headers: {
      'Authorization': `Bearer ${localStorage.getItem('token') || ''}`
    }
  }).then(r => r.json())
}

// 排课列表
const fetchSchedules = (params: Record<string, any>) => {
  const qs = new URLSearchParams()
  for (const [k, v] of Object.entries(params)) {
    if (v !== undefined && v !== null && v !== '') {
      qs.append(k, String(v))
    }
  }
  return fetch(`/api/v1/schedules?${qs}`, {
    headers: {
      'Authorization': `Bearer ${localStorage.getItem('token') || ''}`
    }
  }).then(r => r.json())
}

const loading = ref(false)
const exporting = ref(false)
const semesters = ref<{ id: string; name: string }[]>([])
const buildings = ref<{ id: string; name: string }[]>([])
const allLabs = ref<{ id: string; name: string; buildingId?: string }[]>([])
const listData = ref<any[]>([])
const total = ref(0)
const detailDialogVisible = ref(false)
const currentRow = ref<any | null>(null)
const selectedRows = ref<any[]>([])

const queryForm = reactive({
  semesterId: null as string | null,
  weekNumber: 1,
  buildingId: '',
  labId: '',
  source: '',
  page: 1,
  pageSize: 20
})

const filteredLabs = computed(() => {
  if (!queryForm.buildingId) return allLabs.value
  return allLabs.value.filter(l => !l.buildingId || l.buildingId === queryForm.buildingId)
})

const formatDayOfWeek = (val?: number) => {
  const days = ['周日', '周一', '周二', '周三', '周四', '周五', '周六']
  if (val == null) return '-'
  return days[val - 1] || String(val)
}

const formatSource = (val?: string) => {
  const map: Record<string, string> = {
    CentralScheduling: '集中排课',
    Reservation: '预约',
    TeachingRequest: '授课申请'
  }
  return map[val || ''] || val || '-'
}

const fetchData = async () => {
  if (!queryForm.semesterId) {
    listData.value = []
    total.value = 0
    return
  }
  loading.value = true
  try {
    const res = await fetchSchedules(queryForm)
    if (res.code === 200) {
      listData.value = res.data || []
      total.value = res.data?.length || 0
    } else if (res.code !== 404) {
      ElMessage.error('获取排课数据失败: ' + (res.message || '未知错误'))
      listData.value = []
      total.value = 0
    }
  } catch (err) {
    console.error('获取排课数据失败:', err)
    ElMessage.error('获取排课数据失败')
    listData.value = []
    total.value = 0
  } finally {
    loading.value = false
  }
}

const loadOptions = async () => {
  try {
    const [semRes, buildRes, labRes] = await Promise.all([
      fetchSemesters(),
      fetchBuildings(),
      fetchLabs()
    ])
    if (semRes.code === 200 && semRes.data) {
      semesters.value = [semRes.data]
      queryForm.semesterId = semRes.data.id
    } else if (semRes.code === 404) {
      ElMessage.warning('暂无当前学期，请先在学期管理中创建并设置当前学期')
      semesters.value = []
      queryForm.semesterId = null
    } else {
      ElMessage.error('获取学期信息失败: ' + (semRes.message || '未知错误'))
      semesters.value = []
      queryForm.semesterId = null
    }
    if (buildRes.code === 200) {
      buildings.value = buildRes.data || []
    }
    if (labRes.code === 200) {
      allLabs.value = labRes.data || []
    }
  } catch (err) {
    console.error('加载选项数据失败:', err)
    ElMessage.error('加载选项数据失败')
  }
}

const handleReset = () => {
  queryForm.weekNumber = 1
  queryForm.buildingId = ''
  queryForm.labId = ''
  queryForm.source = ''
  queryForm.page = 1
  fetchData()
}

const handleBuildingChange = () => {
  queryForm.labId = ''
}

const handleSelectionChange = (rows: any[]) => {
  selectedRows.value = rows
}

const handleView = (row: any) => {
  currentRow.value = row
  detailDialogVisible.value = true
}

const handleEdit = (_row: any) => {
  ElMessage.info('编辑功能开发中')
}

const handleExport = () => {
  if (listData.value.length === 0) {
    ElMessage.warning('没有可导出的数据')
    return
  }
  exporting.value = true
  const headers = ['周次', '星期', '节次', '课程/项目', '教师', '班级', '实验室', '来源', '人数', '冲突']
  const rows = listData.value.map(item => [
    item.weekNumber,
    formatDayOfWeek(item.dayOfWeek),
    item.periodNumber,
    item.courseName || item.projectName || '-',
    item.teacherName || '-',
    item.className || '-',
    item.labName || '-',
    formatSource(item.source),
    item.studentCount || '-',
    item.hasConflict ? '有冲突' : '无'
  ])
  const csvContent = [headers, ...rows]
    .map(r => r.map(v => `"${String(v).replace(/"/g, '""')}"`).join(','))
    .join('\n')
  const blob = new Blob(['\ufeff' + csvContent], { type: 'text/csv;charset=utf-8' })
  const url = URL.createObjectURL(blob)
  const a = document.createElement('a')
  a.href = url
  a.download = `排课记录_${new Date().toISOString().slice(0, 10)}.csv`
  a.click()
  URL.revokeObjectURL(url)
  ElMessage.success('导出成功')
  exporting.value = false
}

onMounted(async () => {
  await loadOptions()
  if (queryForm.semesterId) {
    await fetchData()
  }
})
</script>

<style scoped>
.schedule-search-container {
  padding: 20px;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
}

.page-header h2 {
  margin: 0;
  font-size: 20px;
  font-weight: 500;
}

.header-actions {
  display: flex;
  gap: 12px;
}

.search-card {
  margin-bottom: 20px;
}

.search-card :deep(.el-card__body) {
  padding-bottom: 0;
}

.list-card {
  margin-bottom: 20px;
}

.pagination-container {
  display: flex;
  justify-content: flex-end;
  margin-top: 20px;
}
</style>
