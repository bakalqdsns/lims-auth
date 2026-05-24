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
            @change="handleSemesterChange"
          >
            <el-option
              v-for="s in semesters"
              :key="s.id"
              :label="s.name"
              :value="s.id"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="周次范围">
          <el-select v-model="queryForm.startWeek" placeholder="起始周" style="width: 100px" clearable>
            <el-option v-for="w in 20" :key="w" :label="w + '周'" :value="w" />
          </el-select>
          <span style="margin: 0 8px">至</span>
          <el-select v-model="queryForm.endWeek" placeholder="结束周" style="width: 100px" clearable>
            <el-option v-for="w in 20" :key="w" :label="w + '周'" :value="w" />
          </el-select>
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

    <!-- 排课记录列表 -->
    <el-card class="schedule-list-card" shadow="never">
      <template #header>
        <div class="card-header">
          <span>排课记录列表</span>
          <span class="course-count">共 {{ listData.length }} 条记录</span>
        </div>
      </template>
      <el-table
        v-loading="loading"
        :data="listData"
        stripe
        style="width: 100%"
      >
        <el-table-column type="index" label="序号" width="60" />
        <el-table-column prop="courseName" label="课程名称" min-width="150" show-overflow-tooltip />
        <el-table-column prop="teacherName" label="教师" width="100" />
        <el-table-column prop="className" label="班级" width="150" show-overflow-tooltip />
        <el-table-column prop="buildingName" label="楼宇" width="100" show-overflow-tooltip />
        <el-table-column prop="labName" label="实验室" width="120" show-overflow-tooltip />
        <el-table-column label="周次范围" width="100">
          <template #default="{ row }">
            第{{ row.startWeek || row.weekNumber }}-{{ row.endWeek || row.weekNumber }}周
          </template>
        </el-table-column>
        <el-table-column label="星期" width="80">
          <template #default="{ row }">
            {{ formatDayOfWeek(row.dayOfWeek) }}
          </template>
        </el-table-column>
        <el-table-column label="节次" width="80">
          <template #default="{ row }">
            第{{ row.periodNumber }}节
          </template>
        </el-table-column>
        <el-table-column prop="source" label="来源" width="100">
          <template #default="{ row }">
            {{ formatSource(row.source) }}
          </template>
        </el-table-column>
        <el-table-column prop="studentCount" label="人数" width="80" />
        <el-table-column label="冲突" width="80" align="center">
          <template #default="{ row }">
            <el-tag :type="row.hasConflict ? 'danger' : 'success'" size="small">
              {{ row.hasConflict ? '有冲突' : '无' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="150" fixed="right">
          <template #default="{ row }">
            <el-button link type="primary" size="small" @click="handleView(row)">查看</el-button>
            <el-button link type="danger" size="small" @click="handleDelete(row)">删除</el-button>
          </template>
        </el-table-column>
      </el-table>
    </el-card>

    <!-- 课程表视图 -->
    <el-card class="timetable-card" shadow="never">
      <template #header>
        <div class="card-header">
          <span>课程表</span>
          <div class="timetable-controls">
            <el-select v-model="currentWeek" placeholder="选择周次" style="width: 120px" @change="loadTimetable">
              <el-option v-for="w in 20" :key="w" :label="`第${w}周`" :value="w" />
            </el-select>
          </div>
        </div>
      </template>
      <div class="timetable-container">
        <table class="timetable">
          <thead>
            <tr>
              <th class="time-column">节次/时间</th>
              <th v-for="day in weekDays" :key="day.value">{{ day.label }}</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="period in 12" :key="period">
              <td class="time-column">
                <div class="period-info">
                  <div class="period-number">第{{ period }}节</div>
                  <div class="period-time">{{ getPeriodTime(period) }}</div>
                </div>
              </td>
              <td v-for="day in weekDays" :key="day.value" class="timetable-cell">
                <div
                  v-for="item in getTimetableItems(day.value, period)"
                  :key="item.id"
                  class="schedule-item"
                  :class="{ 'has-conflict': item.hasConflict }"
                  @click="handleView(item)"
                >
                  <div class="schedule-course">{{ item.courseName || item.projectName }}</div>
                  <div class="schedule-teacher">{{ item.teacherName }}</div>
                  <div class="schedule-lab">{{ item.labName }}</div>
                  <div class="schedule-weeks">第{{ item.startWeek || item.weekNumber }}-{{ item.endWeek || item.weekNumber }}周</div>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </el-card>

    <!-- 详情对话框 -->
    <el-dialog v-model="detailDialogVisible" title="排课详情" width="600px" destroy-on-close>
      <el-descriptions v-if="currentRow" :column="2" border>
        <el-descriptions-item label="学期">{{ currentRow.semesterName || '-' }}</el-descriptions-item>
        <el-descriptions-item label="周次范围">第{{ currentRow.startWeek || currentRow.weekNumber }}-{{ currentRow.endWeek || currentRow.weekNumber }}周</el-descriptions-item>
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
import { ElMessage, ElMessageBox } from 'element-plus'
import { Search, Download } from '@element-plus/icons-vue'
import { courseApi, type CourseDto, semesterApi } from '@/api/teaching'
import { scheduleApi } from '@/api/schedule'

const loading = ref(false)
const exporting = ref(false)
const courseLoading = ref(false)
const semesters = ref<{ id: string; name: string }[]>([])
const buildings = ref<{ id: string; name: string }[]>([])
const allLabs = ref<{ id: string; name: string; buildingId?: string }[]>([])
const listData = ref<any[]>([])
const courseList = ref<CourseDto[]>([])
const timetableData = ref<any[]>([])
const total = ref(0)
const detailDialogVisible = ref(false)
const currentRow = ref<any | null>(null)
const selectedRows = ref<any[]>([])
const currentWeek = ref(1)

const weekDays = [
  { label: '周一', value: 1 },
  { label: '周二', value: 2 },
  { label: '周三', value: 3 },
  { label: '周四', value: 4 },
  { label: '周五', value: 5 },
  { label: '周六', value: 6 },
  { label: '周日', value: 7 }
]

const periodTimes = [
  { period: 1, time: '08:00-08:45' },
  { period: 2, time: '08:55-09:40' },
  { period: 3, time: '10:00-10:45' },
  { period: 4, time: '10:55-11:40' },
  { period: 5, time: '14:00-14:45' },
  { period: 6, time: '14:55-15:40' },
  { period: 7, time: '16:00-16:45' },
  { period: 8, time: '16:55-17:40' },
  { period: 9, time: '19:00-19:45' },
  { period: 10, time: '19:55-20:40' },
  { period: 11, time: '20:50-21:35' },
  { period: 12, time: '21:45-22:30' }
]

const queryForm = reactive({
  semesterId: null as string | null,
  startWeek: null as number | null,
  endWeek: null as number | null,
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

const getPeriodTime = (period: number) => {
  const pt = periodTimes.find(p => p.period === period)
  return pt ? pt.time : ''
}

const getTimetableItems = (dayOfWeek: number, periodNumber: number) => {
  return timetableData.value.filter(
    item => item.dayOfWeek === dayOfWeek && 
    item.periodNumber === periodNumber &&
    currentWeek.value >= (item.startWeek || item.weekNumber) &&
    currentWeek.value <= (item.endWeek || item.weekNumber)
  )
}

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
    const params: Record<string, any> = {
      semesterId: queryForm.semesterId,
      page: queryForm.page,
      pageSize: queryForm.pageSize
    }
    if (queryForm.startWeek) params.startWeek = queryForm.startWeek
    if (queryForm.endWeek) params.endWeek = queryForm.endWeek
    if (queryForm.buildingId) params.buildingId = queryForm.buildingId
    if (queryForm.labId) params.labId = queryForm.labId
    if (queryForm.source) params.source = queryForm.source

    const res = await scheduleApi.getList(params)
    const response = res.data
    if (response.code === 200) {
      listData.value = response.data || []
      total.value = response.data?.length || 0
    } else if (response.code !== 404) {
      ElMessage.error('获取排课数据失败: ' + (response.message || '未知错误'))
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

const loadTimetable = async () => {
  if (!queryForm.semesterId) {
    timetableData.value = []
    return
  }
  loading.value = true
  try {
    const params: Record<string, any> = {
      semesterId: queryForm.semesterId,
      startWeek: currentWeek.value,
      endWeek: currentWeek.value
    }
    if (queryForm.buildingId) params.buildingId = queryForm.buildingId
    if (queryForm.labId) params.labId = queryForm.labId
    if (queryForm.source) params.source = queryForm.source

    const res = await scheduleApi.getList(params)
    const response = res.data
    if (response.code === 200) {
      timetableData.value = response.data || []
    }
  } catch (err) {
    console.error('加载课程表失败:', err)
    ElMessage.error('加载课程表失败')
  } finally {
    loading.value = false
  }
}

const loadCourses = async () => {
  courseLoading.value = true
  try {
    const res = await courseApi.getList()
    if (res.code === 200) {
      courseList.value = res.data || []
    }
  } catch (err) {
    console.error('加载课程列表失败:', err)
    ElMessage.error('加载课程列表失败')
  } finally {
    courseLoading.value = false
  }
}

const handleDeleteCourse = async (row: CourseDto) => {
  try {
    await ElMessageBox.confirm(
      `确定要删除课程"${row.name}"吗？此操作不可恢复。`,
      '删除确认',
      {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }
    )
    const res = await courseApi.delete(row.id)
    if (res.data.code === 200) {
      ElMessage.success('删除成功')
      await loadCourses()
    } else {
      ElMessage.error('删除失败: ' + (res.data.message || '未知错误'))
    }
  } catch (err: any) {
    if (err !== 'cancel') {
      console.error('删除课程失败:', err)
      ElMessage.error('删除失败')
    }
  }
}

const handleSemesterChange = async () => {
  await loadOptions()
  if (queryForm.semesterId) {
    await fetchData()
    await loadTimetable()
  }
}

const loadOptions = async () => {
  try {
    const [semRes, buildRes, labRes] = await Promise.all([
      semesterApi.getList(),
      fetch('/api/v1/buildings', {
        headers: { 'Authorization': `Bearer ${localStorage.getItem('token') || ''}` }
      }).then(r => r.json()),
      fetch('/api/v1/labs', {
        headers: { 'Authorization': `Bearer ${localStorage.getItem('token') || ''}` }
      }).then(r => r.json())
    ])
    if (semRes.data?.code === 200) {
      semesters.value = semRes.data.data || []
      const current = semesters.value.find((s: any) => s.isCurrent) || semesters.value[0]
      if (current) {
        queryForm.semesterId = current.id
      }
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
  queryForm.startWeek = null
  queryForm.endWeek = null
  queryForm.buildingId = ''
  queryForm.labId = ''
  queryForm.source = ''
  queryForm.page = 1
  fetchData()
  loadTimetable()
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

const handleDelete = async (row: any) => {
  try {
    await ElMessageBox.confirm(
      `确定要删除该排课记录吗？此操作不可恢复。`,
      '删除确认',
      {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }
    )
    console.log('删除前 listData.length:', listData.value.length)
    const res = await scheduleApi.delete(row.id)
    console.log('删除响应:', res.data)
    if (res.data.code === 200) {
      ElMessage.success('删除成功')
      console.log('开始刷新数据...')
      await fetchData()
      console.log('刷新后 listData.length:', listData.value.length)
      await loadTimetable()
    } else {
      ElMessage.error('删除失败: ' + (res.data.message || '未知错误'))
    }
  } catch (err: any) {
    if (err !== 'cancel') {
      console.error('删除排课记录失败:', err)
      ElMessage.error('删除失败')
    }
  }
}

const handleExport = () => {
  if (listData.value.length === 0) {
    ElMessage.warning('没有可导出的数据')
    return
  }
  exporting.value = true
  const headers = ['周次范围', '星期', '节次', '课程/项目', '教师', '班级', '实验室', '来源', '人数', '冲突']
  const rows = listData.value.map(item => [
    `${item.startWeek || item.weekNumber}-${item.endWeek || item.weekNumber}`,
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
  await loadCourses()
  await fetchData()
  await loadTimetable()
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

.course-list-card {
  margin-bottom: 20px;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.course-count {
  font-size: 12px;
  color: #909399;
}

.timetable-card {
  margin-bottom: 20px;
}

.timetable-controls {
  display: flex;
  gap: 12px;
}

.timetable-container {
  overflow-x: auto;
}

.timetable {
  width: 100%;
  border-collapse: collapse;
  table-layout: fixed;
}

.timetable th,
.timetable td {
  border: 1px solid #ebeef5;
  padding: 8px;
  text-align: center;
  vertical-align: top;
}

.timetable th {
  background-color: #f5f7fa;
  font-weight: 500;
}

.time-column {
  width: 100px;
  background-color: #fafafa;
}

.period-info {
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.period-number {
  font-weight: 500;
}

.period-time {
  font-size: 12px;
  color: #909399;
}

.timetable-cell {
  min-height: 80px;
  position: relative;
}

.schedule-item {
  background-color: #ecf5ff;
  border-left: 3px solid #409eff;
  padding: 6px;
  margin-bottom: 4px;
  border-radius: 4px;
  cursor: pointer;
  transition: all 0.2s;
}

.schedule-item:hover {
  background-color: #d9ecff;
  transform: translateY(-1px);
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.schedule-item.has-conflict {
  background-color: #fef0f0;
  border-left-color: #f56c6c;
}

.schedule-course {
  font-weight: 500;
  font-size: 13px;
  margin-bottom: 2px;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.schedule-teacher,
.schedule-lab,
.schedule-weeks {
  font-size: 11px;
  color: #606266;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
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
