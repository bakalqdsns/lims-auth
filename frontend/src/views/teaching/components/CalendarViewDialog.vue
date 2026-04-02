<template>
  <el-dialog :title="`校历 - ${semesterName}`" v-model="visible" width="900px" destroy-on-close>
    <div class="calendar-header">
      <el-button type="primary" @click="handleGenerate">生成校历</el-button>
      <el-button @click="fetchCalendar">刷新</el-button>
      <span class="week-info" v-if="currentWeekInfo">当前：第 {{ currentWeekInfo.weekNumber }} 周</span>
    </div>
    
    <el-calendar v-model="calendarDate" v-if="calendarData.length > 0">
      <template #date-cell="{ data }">
        <div class="calendar-cell" :class="getCellClass(data.day)">
          <div class="day-number">{{ new Date(data.day).getDate() }}</div>
          <div class="day-info" v-if="getCalendarDay(data.day)">
            <el-tag v-if="getCalendarDay(data.day)?.isHoliday" type="danger" size="small">假</el-tag>
            <span v-else class="week-number">第{{ getCalendarDay(data.day)?.weekNumber }}周</span>
          </div>
        </div>
      </template>
    </el-calendar>
    
    <el-empty v-else description="暂无校历数据，请点击生成校历" />
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { ElMessage } from 'element-plus'
import { calendarApi, type AcademicCalendarDto, type WeekInfoDto } from '@/api/teaching'

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

const fetchCalendar = async () => {
  if (!props.semesterId) return
  try {
    const res = await calendarApi.getBySemester(props.semesterId)
    if (res.data.code === 200) {
      calendarData.value = res.data.data
    }
  } catch (error) {
    console.error('获取校历失败', error)
  }
}

const handleGenerate = async () => {
  if (!props.semesterId) return
  try {
    const res = await calendarApi.generate(props.semesterId)
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
  return calendarData.value.find(d => d.date.split('T')[0] === dateStr)
}

const getCellClass = (dateStr: string): string => {
  const day = getCalendarDay(dateStr)
  if (!day) return ''
  if (day.isHoliday) return 'holiday'
  if (day.dayOfWeek === 6 || day.dayOfWeek === 7) return 'weekend'
  return ''
}

watch(() => props.modelValue, (val) => {
  if (val && props.semesterId) {
    fetchCalendar()
  }
})
</script>

<style scoped>
.calendar-header {
  display: flex;
  align-items: center;
  gap: 10px;
  margin-bottom: 20px;
}

.week-info {
  margin-left: auto;
  color: #606266;
}

.calendar-cell {
  height: 100%;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
}

.day-number {
  font-size: 14px;
  font-weight: 500;
}

.day-info {
  margin-top: 4px;
  font-size: 12px;
}

.week-number {
  color: #909399;
}

.holiday {
  background-color: #fef0f0;
}

.weekend {
  background-color: #f5f7fa;
}
</style>
