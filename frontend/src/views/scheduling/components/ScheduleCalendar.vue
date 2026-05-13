<template>
  <div class="schedule-calendar" :style="{ minWidth: tableWidth + 'px' }">
    <!-- 星期标题行 -->
    <div class="calendar-header">
      <div class="calendar-period-label">节次</div>
      <div
        v-for="day in weekDays"
        :key="day.value"
        class="calendar-day-header"
        :class="{ 'is-today': todayDayOfWeek === day.value }"
      >
        <div class="day-name">{{ day.label }}</div>
        <div class="day-date" v-if="weekInfo">{{ getDayDate(day.value) }}</div>
      </div>
    </div>

    <!-- 课程表行 -->
    <div
      v-for="row in tableData"
      :key="row.periodNumber"
      class="calendar-row"
    >
      <div class="period-label">
        <div class="period-number">{{ row.periodNumber }}</div>
        <div class="period-name">{{ row.periodName }}</div>
      </div>
      <div
        v-for="(cell, dayIndex) in row.cells"
        :key="dayIndex"
        class="calendar-cell"
        :class="getCellClass(cell)"
        @click="handleCellClick(cell, row.periodNumber, dayIndex + 1)"
      >
        <template v-if="cell">
          <div class="cell-course" :title="cell.courseName">{{ cell.courseName }}</div>
          <div class="cell-teacher" v-if="cell.teacherName">{{ cell.teacherName }}</div>
          <div class="cell-meta">
            <span class="cell-lab" v-if="cell.labName">{{ cell.labName }}</span>
            <span class="cell-students" v-if="cell.studentCount">{{ cell.studentCount }}人</span>
          </div>
          <div class="cell-source-badge" :class="getSourceClass(cell.source)">
            {{ getSourceLabel(cell.source) }}
          </div>
          <div class="cell-conflict-icon" v-if="cell.hasConflict">
            <el-icon color="#f56c6c"><Warning /></el-icon>
          </div>
        </template>
        <template v-else>
          <div class="cell-empty" @click="handleEmptyClick(row.periodNumber, dayIndex + 1)">
            <el-icon><Plus /></el-icon>
          </div>
        </template>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { Warning, Plus } from '@element-plus/icons-vue'
import type { ScheduleTableRow, ScheduleTableCell } from '@/api/schedule'

const props = defineProps<{
  tableData: ScheduleTableRow[]
  weekInfo?: {
    startDate: string
    endDate: string
    days: { date: string; dayOfWeek: number }[]
  }
  viewMode?: 'table' | 'week'
}>()

const emit = defineEmits<{
  cellClick: [cell: ScheduleTableCell | null, periodNumber: number, dayOfWeek: number]
}>()

const weekDays = [
  { label: '周一', value: 1 },
  { label: '周二', value: 2 },
  { label: '周三', value: 3 },
  { label: '周四', value: 4 },
  { label: '周五', value: 5 },
  { label: '周六', value: 6 },
  { label: '周日', value: 7 }
]

const todayDayOfWeek = computed(() => {
  const d = new Date()
  const dow = d.getDay()
  return dow === 0 ? 7 : dow
})

const tableWidth = computed(() => {
  return 100 + weekDays.length * 140
})

const getDayDate = (dayOfWeek: number): string => {
  if (!props.weekInfo?.days) return ''
  const day = props.weekInfo.days.find(d => d.dayOfWeek === dayOfWeek)
  return day ? new Date(day.date).getDate() + '日' : ''
}

const getCellClass = (cell: ScheduleTableCell | null): Record<string, boolean> => {
  return {
    'is-occupied': !!cell,
    'is-empty': !cell,
    'is-conflict': cell?.hasConflict,
    'is-today-day': weekDays.some(d => d.value === todayDayOfWeek.value)
  }
}

const getSourceClass = (source?: string): string => {
  const map: Record<string, string> = {
    'CentralScheduling': 'badge-primary',
    'Reservation': 'badge-success',
    'TeachingRequest': 'badge-warning',
    'PendingReservation': 'badge-info',
    'PendingTeachingRequest': 'badge-info'
  }
  return map[source ?? ''] || 'badge-default'
}

const getSourceLabel = (source?: string): string => {
  const map: Record<string, string> = {
    'CentralScheduling': '集中排课',
    'Reservation': '预约',
    'TeachingRequest': '授课申请',
    'PendingReservation': '待审批',
    'PendingTeachingRequest': '待审批'
  }
  return map[source ?? ''] || source || ''
}

const handleCellClick = (cell: ScheduleTableCell | null, periodNumber: number, dayOfWeek: number) => {
  emit('cellClick', cell, periodNumber, dayOfWeek)
}

const handleEmptyClick = (periodNumber: number, dayOfWeek: number) => {
  emit('cellClick', null, periodNumber, dayOfWeek)
}
</script>

<style scoped>
.schedule-calendar {
  font-size: 13px;
  user-select: none;
}

.calendar-header {
  display: flex;
  background: #f5f7fa;
  border-bottom: 2px solid #e4e7ed;
  position: sticky;
  top: 0;
  z-index: 10;
}

.calendar-period-label {
  width: 80px;
  min-width: 80px;
  padding: 8px 4px;
  text-align: center;
  font-weight: 600;
  color: #606266;
  border-right: 1px solid #e4e7ed;
  background: #f5f7fa;
}

.calendar-day-header {
  flex: 1;
  min-width: 130px;
  padding: 8px 4px;
  text-align: center;
  border-right: 1px solid #ebeef5;
}

.calendar-day-header.is-today .day-name {
  color: #409eff;
  font-weight: 700;
}

.day-name {
  font-weight: 600;
  color: #303133;
  font-size: 14px;
}

.day-date {
  font-size: 12px;
  color: #909399;
  margin-top: 2px;
}

.calendar-row {
  display: flex;
  border-bottom: 1px solid #ebeef5;
}

.calendar-row:last-child {
  border-bottom: none;
}

.period-label {
  width: 80px;
  min-width: 80px;
  padding: 8px 4px;
  text-align: center;
  border-right: 1px solid #e4e7ed;
  background: #fafafa;
  display: flex;
  flex-direction: column;
  justify-content: center;
}

.period-number {
  font-weight: 700;
  color: #409eff;
  font-size: 16px;
}

.period-name {
  font-size: 11px;
  color: #909399;
  margin-top: 2px;
}

.calendar-cell {
  flex: 1;
  min-width: 130px;
  min-height: 70px;
  padding: 4px;
  border-right: 1px solid #ebeef5;
  position: relative;
  cursor: pointer;
  transition: background 0.15s;
  display: flex;
  flex-direction: column;
  gap: 2px;
}

.calendar-cell:hover {
  background: #f0f9ff;
}

.calendar-cell.is-empty:hover {
  background: #ecf5ff;
}

.calendar-cell.is-occupied {
  background: #f0f9ff;
}

.calendar-cell.is-conflict {
  background: #fef0f0;
  border: 1px dashed #fab6b6;
}

.calendar-cell.is-today-day:not(.is-empty) {
  background: #ecf5ff;
}

.cell-course {
  font-weight: 600;
  color: #303133;
  font-size: 12px;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.cell-teacher {
  font-size: 11px;
  color: #606266;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.cell-meta {
  display: flex;
  gap: 4px;
  font-size: 11px;
  color: #909399;
  flex-wrap: wrap;
}

.cell-source-badge {
  position: absolute;
  top: 4px;
  right: 4px;
  font-size: 10px;
  padding: 1px 4px;
  border-radius: 3px;
  font-weight: 500;
}

.badge-primary { background: #409eff; color: #fff; }
.badge-success { background: #67c23a; color: #fff; }
.badge-warning { background: #e6a23c; color: #fff; }
.badge-info { background: #909399; color: #fff; }
.badge-default { background: #dcdfe6; color: #606266; }

.cell-conflict-icon {
  position: absolute;
  bottom: 4px;
  right: 4px;
}

.cell-empty {
  display: flex;
  align-items: center;
  justify-content: center;
  height: 100%;
  color: #c0c4cc;
  font-size: 18px;
}

.calendar-cell:hover .cell-empty {
  color: #409eff;
}
</style>
