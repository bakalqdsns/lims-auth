<template>
  <el-dialog :title="`校历 - ${semesterName}`" v-model="visible" width="1000px" destroy-on-close>
    <!-- 工具栏 -->
    <div class="calendar-toolbar">
      <div class="toolbar-left">
        <el-button type="primary" @click="handleGenerate">
          <el-icon><Refresh /></el-icon>生成校历
        </el-button>
        <el-button @click="fetchCalendar">
          <el-icon><RefreshRight /></el-icon>刷新
        </el-button>
        <el-button @click="showHolidayDialog = true">
          <el-icon><Plus /></el-icon>添加节假日
        </el-button>
      </div>
      <div class="toolbar-center">
        <el-radio-group v-model="filterType" size="small">
          <el-radio-button label="all">全部</el-radio-button>
          <el-radio-button label="teaching">教学日</el-radio-button>
          <el-radio-button label="holiday">节假日</el-radio-button>
          <el-radio-button label="exam">考试</el-radio-button>
        </el-radio-group>
      </div>
      <div class="toolbar-right">
        <span class="week-info" v-if="currentWeekInfo">
          当前：第 {{ currentWeekInfo.weekNumber }} 周
        </span>
      </div>
    </div>

    <!-- 图例 -->
    <div class="calendar-legend">
      <el-tag size="small" effect="plain" style="margin-right: 8px;">
        <span class="legend-dot" style="background: #67C23A;"></span>教学日
      </el-tag>
      <el-tag size="small" effect="plain" type="danger" style="margin-right: 8px;">
        <span class="legend-dot" style="background: #F56C6C;"></span>节假日
      </el-tag>
      <el-tag size="small" effect="plain" type="warning" style="margin-right: 8px;">
        <span class="legend-dot" style="background: #E6A23C;"></span>考试
      </el-tag>
      <el-tag size="small" effect="plain" type="info">
        <span class="legend-dot" style="background: #909399;"></span>其他
      </el-tag>
    </div>
    
    <!-- 日历 -->
    <el-calendar v-model="calendarDate" v-if="filteredCalendarData.length > 0">
      <template #date-cell="{ data }">
        <div 
          class="calendar-cell" 
          :class="getCellClass(data.day)"
          @click="handleCellClick(data.day)"
        >
          <div class="day-number">{{ new Date(data.day).getDate() }}</div>
          <div class="day-info" v-if="getCalendarDay(data.day)">
            <el-tag 
              v-if="getCalendarDay(data.day)?.isHoliday" 
              type="danger" 
              size="small"
              class="day-tag"
            >
              {{ getCalendarDay(data.day)?.holidayName || '假' }}
            </el-tag>
            <el-tag 
              v-else-if="getCalendarDay(data.day)?.eventType === 'Exam'" 
              type="warning" 
              size="small"
              class="day-tag"
            >
              考试
            </el-tag>
            <el-tag 
              v-else-if="getCalendarDay(data.day)?.eventType === 'Registration'" 
              type="primary" 
              size="small"
              class="day-tag"
            >
              注册
            </el-tag>
            <span v-else class="week-number">第{{ getCalendarDay(data.day)?.weekNumber }}周</span>
          </div>
          <div class="day-actions" v-if="getCalendarDay(data.day)">
            <el-icon class="action-icon" @click.stop="handleEditDay(data.day)"><Edit /></el-icon>
          </div>
        </div>
      </template>
    </el-calendar>
    
    <el-empty v-else description="暂无校历数据，请点击生成校历" />

    <!-- 日期编辑对话框 -->
    <el-dialog v-model="editDialogVisible" title="编辑日期" width="400px" append-to-body>
      <el-form :model="editForm" label-width="100px">
        <el-form-item label="日期">
          <span>{{ editForm.date }}</span>
        </el-form-item>
        <el-form-item label="事件类型">
          <el-select v-model="editForm.eventType" placeholder="选择事件类型">
            <el-option label="教学日" value="Teaching" />
            <el-option label="考试" value="Exam" />
            <el-option label="节假日" value="Holiday" />
            <el-option label="注册" value="Registration" />
            <el-option label="选课" value="CourseSelection" />
            <el-option label="成绩录入" value="GradeEntry" />
            <el-option label="运动会" value="Sports" />
            <el-option label="校园活动" value="Activity" />
            <el-option label="自定义" value="Custom" />
          </el-select>
        </el-form-item>
        <el-form-item label="事件名称">
          <el-input v-model="editForm.eventName" placeholder="如：国庆节、期中考试" />
        </el-form-item>
        <el-form-item label="是否节假日">
          <el-switch v-model="editForm.isHoliday" />
        </el-form-item>
        <el-form-item label="是否工作日">
          <el-switch v-model="editForm.isWorkday" />
        </el-form-item>
        <el-form-item label="是否教学日">
          <el-switch v-model="editForm.isTeachingDay" />
        </el-form-item>
        <el-form-item label="描述">
          <el-input v-model="editForm.description" type="textarea" rows="2" />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="editDialogVisible = false">取消</el-button>
        <el-button type="primary" @click="saveDayEdit">保存</el-button>
      </template>
    </el-dialog>

    <!-- 添加节假日对话框 -->
    <el-dialog v-model="showHolidayDialog" title="添加节假日" width="400px" append-to-body>
      <el-form :model="holidayForm" label-width="100px">
        <el-form-item label="日期">
          <el-date-picker v-model="holidayForm.date" type="date" placeholder="选择日期" />
        </el-form-item>
        <el-form-item label="名称">
          <el-input v-model="holidayForm.name" placeholder="如：国庆节" />
        </el-form-item>
        <el-form-item label="类型">
          <el-select v-model="holidayForm.type" placeholder="选择类型">
            <el-option label="法定节假日" value="法定假日" />
            <el-option label="校定假日" value="校定假日" />
            <el-option label="调休" value="调休" />
          </el-select>
        </el-form-item>
        <el-form-item label="调休为工作日">
          <el-switch v-model="holidayForm.isWorkday" />
        </el-form-item>
        <el-form-item label="描述">
          <el-input v-model="holidayForm.description" type="textarea" rows="2" />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="showHolidayDialog = false">取消</el-button>
        <el-button type="primary" @click="addHoliday">添加</el-button>
      </template>
    </el-dialog>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { ElMessage } from 'element-plus'
import { Refresh, RefreshRight, Plus, Edit } from '@element-plus/icons-vue'
import { calendarApi, semesterApi, type AcademicCalendarDto, type WeekInfoDto } from '@/api/teaching'

interface Props {
  modelValue: boolean
  semesterId?: string
  semesterName?: string
}

const props = defineProps<Props>()
const emit = defineEmits(['update:modelValue'])

const visible = computed({
  get: () => props.modelValue,
  set: (val) => emit('update:modelValue', val)
})

const calendarDate = ref(new Date())
const calendarData = ref<AcademicCalendarDto[]>([])
const currentWeekInfo = ref<WeekInfoDto | null>(null)
const filterType = ref('all')
const editDialogVisible = ref(false)
const showHolidayDialog = ref(false)

const editForm = ref({
  id: '',
  date: '',
  eventType: 'Teaching',
  eventName: '',
  isHoliday: false,
  isWorkday: true,
  isTeachingDay: true,
  description: ''
})

const holidayForm = ref({
  date: '',
  name: '',
  type: '法定假日',
  isWorkday: false,
  description: ''
})

// 根据筛选条件过滤日历数据
const filteredCalendarData = computed(() => {
  if (filterType.value === 'all') return calendarData.value
  return calendarData.value.filter(day => {
    switch (filterType.value) {
      case 'teaching':
        return day.isTeachingDay
      case 'holiday':
        return day.isHoliday
      case 'exam':
        return day.eventType === 'Exam'
      default:
        return true
    }
  })
})

const fetchCalendar = async () => {
  if (!props.semesterId) return
  try {
    const res = await calendarApi.getBySemester(props.semesterId)
    if (res.data.code === 200) {
      calendarData.value = res.data.data
      // 获取当前周信息
      const today = new Date().toISOString().split('T')[0]
      const todayData = calendarData.value.find(d => d.date.split('T')[0] === today)
      if (todayData) {
        currentWeekInfo.value = {
          weekNumber: todayData.weekNumber,
          semesterId: props.semesterId,
          startDate: '',
          endDate: '',
          days: []
        }
      }
    }
  } catch (error) {
    console.error('获取校历失败', error)
    ElMessage.error('获取校历失败')
  }
}

const handleGenerate = async () => {
  if (!props.semesterId) return
  try {
    const res = await semesterApi.generateCalendar(props.semesterId)
    if (res.data.code === 200) {
      ElMessage.success('校历生成成功')
      fetchCalendar()
    } else {
      ElMessage.error(res.data.message)
    }
  } catch (error) {
    ElMessage.error('生成失败')
  }
}

const getCalendarDay = (dateStr: string): AcademicCalendarDto | undefined => {
  return calendarData.value.find((d: any) => d.date.split('T')[0] === dateStr)
}

const getCellClass = (dateStr: string): string => {
  const day = getCalendarDay(dateStr)
  if (!day) return ''
  const classes: string[] = []
  if (day.isHoliday) classes.push('holiday')
  if (day.dayOfWeek === 6 || day.dayOfWeek === 7) classes.push('weekend')
  if (day.eventType === 'Exam') classes.push('exam')
  if (day.eventType === 'Teaching' && day.isTeachingDay) classes.push('teaching')
  return classes.join(' ')
}

const handleCellClick = (dateStr: string) => {
  const day = getCalendarDay(dateStr)
  if (day) {
    console.log('点击日期:', day)
  }
}

const handleEditDay = (dateStr: string) => {
  const day = getCalendarDay(dateStr)
  if (day) {
    editForm.value = {
      id: day.id,
      date: day.date.split('T')[0],
      eventType: day.eventType || 'Teaching',
      eventName: day.eventName || '',
      isHoliday: day.isHoliday,
      isWorkday: day.isWorkday ?? true,
      isTeachingDay: day.isTeachingDay ?? true,
      description: day.description || ''
    }
    // 同步上方筛选与当前日期的事件类型
    syncFilterWithEventType(day.eventType || 'Teaching')
    editDialogVisible.value = true
  }
}

// 同步上方筛选与事件类型
const syncFilterWithEventType = (eventType: string) => {
  switch (eventType) {
    case 'Teaching':
      filterType.value = 'teaching'
      break
    case 'Holiday':
      filterType.value = 'holiday'
      break
    case 'Exam':
      filterType.value = 'exam'
      break
    default:
      filterType.value = 'all'
  }
}

// 监听编辑表单事件类型变化，同步更新上方筛选
watch(() => editForm.value.eventType, (newType) => {
  if (editDialogVisible.value && newType) {
    syncFilterWithEventType(newType)
  }
})

// 监听上方筛选变化，同步更新编辑表单的事件类型
watch(() => filterType.value, (newFilter) => {
  if (editDialogVisible.value) {
    switch (newFilter) {
      case 'teaching':
        editForm.value.eventType = 'Teaching'
        break
      case 'holiday':
        editForm.value.eventType = 'Holiday'
        break
      case 'exam':
        editForm.value.eventType = 'Exam'
        break
    }
  }
})

const saveDayEdit = async () => {
  try {
    const res = await calendarApi.update(editForm.value.id, {
      eventType: editForm.value.eventType,
      eventName: editForm.value.eventName,
      isHoliday: editForm.value.isHoliday,
      isWorkday: editForm.value.isWorkday,
      isTeachingDay: editForm.value.isTeachingDay,
      description: editForm.value.description
    })
    if (res.data.code === 200) {
      ElMessage.success('保存成功')
      editDialogVisible.value = false
      fetchCalendar()
    }
  } catch (error) {
    ElMessage.error('保存失败')
  }
}

const addHoliday = async () => {
  if (!props.semesterId) return
  if (!holidayForm.value.date || !holidayForm.value.name) {
    ElMessage.warning('请填写完整信息')
    return
  }
  
  // 这里应该调用添加节假日的API
  // 暂时使用更新校历日的方式
  const dateStr = new Date(holidayForm.value.date).toISOString().split('T')[0]
  const day = calendarData.value.find(d => d.date.split('T')[0] === dateStr)
  
  if (day) {
    try {
      const res = await calendarApi.update(day.id, {
        isHoliday: true,
        holidayName: holidayForm.value.name,
        isWorkday: holidayForm.value.isWorkday,
        isTeachingDay: false,
        eventType: 'Holiday',
        description: holidayForm.value.description
      })
      if (res.data.code === 200) {
        ElMessage.success('添加成功')
        showHolidayDialog.value = false
        fetchCalendar()
      }
    } catch (error) {
      ElMessage.error('添加失败')
    }
  } else {
    ElMessage.warning('该日期不在校历范围内')
  }
}

watch(() => props.modelValue, (val) => {
  if (val && props.semesterId) {
    fetchCalendar()
  }
})
</script>

<style scoped>
.calendar-toolbar {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 15px;
  flex-wrap: wrap;
  gap: 10px;
}

.toolbar-left, .toolbar-center, .toolbar-right {
  display: flex;
  align-items: center;
  gap: 8px;
}

.calendar-legend {
  margin-bottom: 15px;
  padding: 10px;
  background: #f5f7fa;
  border-radius: 4px;
}

.legend-dot {
  display: inline-block;
  width: 12px;
  height: 12px;
  border-radius: 50%;
  margin-right: 4px;
}

.week-info {
  color: #606266;
  font-size: 14px;
}

.calendar-cell {
  height: 100%;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 4px;
  cursor: pointer;
  position: relative;
  transition: background-color 0.2s;
}

.calendar-cell:hover {
  background-color: #f5f7fa;
}

.day-number {
  font-size: 14px;
  font-weight: 500;
}

.day-info {
  margin-top: 4px;
  font-size: 12px;
}

.day-tag {
  max-width: 100%;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.week-number {
  color: #909399;
  font-size: 11px;
}

.day-actions {
  position: absolute;
  top: 2px;
  right: 2px;
  opacity: 0;
  transition: opacity 0.2s;
}

.calendar-cell:hover .day-actions {
  opacity: 1;
}

.action-icon {
  font-size: 14px;
  color: #409EFF;
  cursor: pointer;
  padding: 2px;
}

.action-icon:hover {
  color: #66b1ff;
}

.holiday {
  background-color: #fef0f0 !important;
}

.weekend {
  background-color: #f5f7fa;
}

.exam {
  background-color: #fdf6ec !important;
}

.teaching {
  background-color: #f0f9eb !important;
}

:deep(.el-calendar-day) {
  height: 80px;
  padding: 0;
}
</style>