<template>
  <view class="admin-depts-page">
    <scroll-view class="list-area" scroll-y refresher-enabled @refresherrefresh="loadData">
      <view v-for="dept in departments" :key="dept.id" class="dept-card">
        <view class="dept-card__header">
          <text class="dept-name">{{ dept.name }}</text>
          <text class="dept-code">{{ dept.code }}</text>
        </view>
        <text v-if="dept.parentName" class="dept-parent">上级: {{ dept.parentName }}</text>
      </view>

      <view v-if="!isLoading && departments.length === 0" class="empty">
        <Icon name="inbox" :size="48" color="#d0d0d0" />
        <text class="empty-text">暂无部门</text>
      </view>

      <view :style="{ height: '40px' }" />
    </scroll-view>
  </view>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { get } from '@/utils/request'
import Icon from '@/components/Icon.vue'

interface Department { id: number; name: string; code: string; parentId?: number; parentName?: string }

const isLoading = ref(false)
const departments = ref<Department[]>([])

async function loadData() {
  isLoading.value = true
  try {
    const data = await get<Department[]>('/departments')
    departments.value = data as unknown as Department[]
  } catch { /* ignore */ } finally { isLoading.value = false }
}

onMounted(async () => { await loadData() })
</script>

<style lang="scss" scoped>
.admin-depts-page { min-height: 100vh; background: #f5f7fa; }

.list-area { height: calc(100vh - 88rpx); padding: 24rpx; }

.dept-card {
  background: #fff;
  border-radius: 24rpx;
  padding: 28rpx;
  margin-bottom: 20rpx;
  box-shadow: 0 4rpx 16rpx rgba(0,0,0,0.06);

  &__header { display: flex; align-items: center; gap: 12rpx; margin-bottom: 8rpx; }
}

.dept-name { font-size: 30rpx; font-weight: bold; color: #303133; }
.dept-code { font-size: 24rpx; color: #909399; }
.dept-parent { font-size: 24rpx; color: #909399; display: block; }

.empty { display: flex; flex-direction: column; align-items: center; padding: 120rpx 0; gap: 16rpx; }
.empty-icon { font-size: 80rpx; color: #d0d0d0; }
.empty-text { font-size: 28rpx; color: #909399; }
</style>
