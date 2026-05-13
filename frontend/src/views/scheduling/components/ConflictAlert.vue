<template>
  <div v-if="result" class="conflict-alert">
    <!-- 硬冲突：阻止提交 -->
    <el-alert
      v-if="result.hasHardConflict"
      type="error"
      :title="`存在 ${result.hardConflicts.length} 个硬冲突，无法排课`"
      :description="result.hardConflicts.map(c => c.message).join('\n')"
      :closable="false"
      show-icon
    >
      <template #default>
        <div class="conflict-list">
          <div v-for="(conflict, idx) in result.hardConflicts" :key="idx" class="conflict-item error">
            <el-icon><Close /></el-icon>
            <span>{{ conflict.message }}</span>
          </div>
        </div>
      </template>
    </el-alert>

    <!-- 软冲突：警告但可强制 -->
    <el-alert
      v-else-if="result.hasSoftConflict"
      type="warning"
      :title="`存在 ${result.softConflicts.length} 个软冲突（教师/班级时间冲突）`"
      :closable="false"
      show-icon
    >
      <template #default>
        <div class="conflict-list">
          <div v-for="(conflict, idx) in result.softConflicts" :key="idx" class="conflict-item warning">
            <el-icon><Warning /></el-icon>
            <span>{{ conflict.message }}</span>
          </div>
        </div>
        <div class="conflict-actions" v-if="showForceOption">
          <el-checkbox v-model="forceSchedule">
            强制排课（忽略软冲突警告）
          </el-checkbox>
        </div>
      </template>
    </el-alert>

    <!-- 无冲突 -->
    <el-alert
      v-else
      type="success"
      title="检测通过：当前时间无冲突"
      :closable="false"
      show-icon
    />
  </div>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue'
import { Close, Warning } from '@element-plus/icons-vue'
import type { ConflictCheckResult } from '@/api/schedule'

const props = defineProps<{
  result: ConflictCheckResult | null
  showForceOption?: boolean
}>()

const emit = defineEmits<{
  'update:forceSchedule': [boolean]
}>()

const forceSchedule = ref(false)

watch(forceSchedule, (val) => {
  emit('update:forceSchedule', val)
})

watch(() => props.result, () => {
  forceSchedule.value = false
})
</script>

<style scoped>
.conflict-alert {
  margin: 12px 0;
}

.conflict-list {
  margin-top: 8px;
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.conflict-item {
  display: flex;
  align-items: flex-start;
  gap: 6px;
  font-size: 13px;
  line-height: 1.5;
  padding: 4px 8px;
  border-radius: 4px;
}

.conflict-item.error {
  color: #c0392b;
  background: #fdf2f2;
}

.conflict-item.warning {
  color: #b7791f;
  background: #fefce8;
}

.conflict-actions {
  margin-top: 12px;
}
</style>
