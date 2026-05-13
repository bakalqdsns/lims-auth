<template>
  <el-card shadow="never" class="lab-selector-card">
    <template #header>
      <div class="card-header">
        <span>实验室选择</span>
        <el-input
          v-model="keyword"
          placeholder="搜索实验室"
          prefix-icon="Search"
          clearable
          style="width: 200px"
          @input="handleSearch"
        />
      </div>
    </template>

    <div class="lab-list" v-loading="loading">
      <el-empty v-if="!loading && filteredLabs.length === 0" description="暂无可用实验室" />

      <div
        v-for="lab in filteredLabs"
        :key="lab.id"
        class="lab-item"
        :class="{
          'is-selected': modelValue === lab.id,
          'is-occupied': isLabOccupied(lab.id)
        }"
        @click="handleSelect(lab)"
      >
        <div class="lab-info">
          <div class="lab-name">{{ lab.name }}</div>
          <div class="lab-meta">
            <el-tag size="small">{{ lab.labType || '实验室' }}</el-tag>
            <span class="lab-capacity">
              <el-icon><User /></el-icon>
              {{ lab.capacity }}人
            </span>
            <span class="lab-location" v-if="lab.location || lab.roomNumber">
              <el-icon><Location /></el-icon>
              {{ lab.location || lab.roomNumber }}
            </span>
          </div>
          <div class="lab-building" v-if="lab.building">
            <el-icon><OfficeBuilding /></el-icon>
            {{ lab.building.name }}
          </div>
        </div>
        <div class="lab-status">
          <el-tag v-if="modelValue === lab.id" type="success" size="small">已选择</el-tag>
          <el-tag v-else-if="isLabOccupied(lab.id)" type="danger" size="small">已占用</el-tag>
          <el-tag v-else type="info" size="small">空闲</el-tag>
        </div>
      </div>
    </div>

    <!-- 已选实验室详情 -->
    <div class="selected-detail" v-if="selectedLab">
      <el-divider content-position="left">已选实验室详情</el-divider>
      <el-descriptions :column="2" border size="small">
        <el-descriptions-item label="实验室名称">{{ selectedLab.name }}</el-descriptions-item>
        <el-descriptions-item label="实验室编号">{{ selectedLab.code }}</el-descriptions-item>
        <el-descriptions-item label="容纳人数">{{ selectedLab.capacity }}</el-descriptions-item>
        <el-descriptions-item label="实验室类型">{{ selectedLab.labType || '-' }}</el-descriptions-item>
        <el-descriptions-item label="地点" :span="2">{{ selectedLab.location || selectedLab.roomNumber || '-' }}</el-descriptions-item>
        <el-descriptions-item label="负责人">{{ selectedLab.managerName || '-' }}</el-descriptions-item>
        <el-descriptions-item label="安全等级" v-if="selectedLab.safetyLevel">{{ selectedLab.safetyLevel }}</el-descriptions-item>
      </el-descriptions>
    </div>
  </el-card>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { Search, User, Location, OfficeBuilding } from '@element-plus/icons-vue'
import type { LabDto } from '@/api/lab'

const props = defineProps<{
  modelValue?: string
  labs: LabDto[]
  occupiedLabIds?: string[]
  loading?: boolean
}>()

const emit = defineEmits<{
  'update:modelValue': [id: string | undefined]
  'change': [lab: LabDto | undefined]
}>()

const keyword = ref('')

const filteredLabs = computed(() => {
  if (!keyword.value) return props.labs
  const kw = keyword.value.toLowerCase()
  return props.labs.filter(l =>
    l.name?.toLowerCase().includes(kw) ||
    l.code?.toLowerCase().includes(kw) ||
    l.location?.toLowerCase().includes(kw) ||
    l.roomNumber?.toLowerCase().includes(kw) ||
    l.labType?.toLowerCase().includes(kw)
  )
})

const selectedLab = computed(() =>
  props.labs.find(l => l.id === props.modelValue)
)

const isLabOccupied = (labId: string): boolean => {
  return props.occupiedLabIds?.includes(labId) ?? false
}

const handleSearch = () => {}

const handleSelect = (lab: LabDto) => {
  if (isLabOccupied(lab.id)) return
  const newId = props.modelValue === lab.id ? undefined : lab.id
  emit('update:modelValue', newId)
  emit('change', newId ? lab : undefined)
}

watch(() => props.modelValue, (newId) => {
  if (!newId) {
    emit('change', undefined)
  }
})
</script>

<style scoped>
.lab-selector-card {
  max-height: 600px;
  display: flex;
  flex-direction: column;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.lab-list {
  overflow-y: auto;
  max-height: 400px;
}

.lab-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 10px 12px;
  border-bottom: 1px solid #f0f0f0;
  cursor: pointer;
  transition: background 0.15s;
  border-radius: 4px;
  margin-bottom: 4px;
}

.lab-item:hover {
  background: #f5f7fa;
}

.lab-item.is-selected {
  background: #ecf5ff;
  border: 1px solid #409eff;
}

.lab-item.is-occupied {
  opacity: 0.6;
  cursor: not-allowed;
  background: #fafafa;
}

.lab-name {
  font-weight: 600;
  color: #303133;
  margin-bottom: 4px;
}

.lab-meta {
  display: flex;
  align-items: center;
  gap: 8px;
  flex-wrap: wrap;
}

.lab-capacity,
.lab-location {
  display: flex;
  align-items: center;
  gap: 2px;
  font-size: 12px;
  color: #909399;
}

.lab-building {
  display: flex;
  align-items: center;
  gap: 4px;
  font-size: 12px;
  color: #c0c4cc;
  margin-top: 4px;
}

.lab-status {
  flex-shrink: 0;
}

.selected-detail {
  margin-top: 16px;
}
</style>
