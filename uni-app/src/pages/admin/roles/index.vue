<template>
  <view class="admin-roles-page">
    <scroll-view class="list-area" scroll-y refresher-enabled @refresherrefresh="loadData">
      <view v-for="role in roles" :key="role.id" class="role-card">
        <view class="role-card__header">
          <text class="role-name">{{ role.name }}</text>
          <text class="role-code">{{ role.code }}</text>
        </view>
        <text v-if="role.description" class="role-desc">{{ role.description }}</text>
        <view class="role-card__footer">
          <text class="role-count">{{ role.userCount }} 位用户</text>
          <view class="role-perms">
            <text v-for="(p, i) in role.permissions?.slice(0, 3)" :key="i" class="perm-tag">{{ p }}</text>
            <text v-if="(role.permissions?.length || 0) > 3" class="perm-more">+{{ (role.permissions?.length || 0) - 3 }}</text>
          </view>
        </view>
      </view>

      <view v-if="!isLoading && roles.length === 0" class="empty">
        <Icon name="inbox" :size="48" color="#d0d0d0" />
        <text class="empty-text">暂无角色</text>
      </view>

      <view :style="{ height: '40px' }" />
    </scroll-view>
  </view>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { getRoles } from '@/api/role'
import Icon from '@/components/Icon.vue'
import type { Role } from '@/types/role'

const isLoading = ref(false)
const roles = ref<Role[]>([])

async function loadData() {
  isLoading.value = true
  try {
    const resp = await getRoles({ pageSize: 100 })
    roles.value = resp.items
  } catch { /* ignore */ } finally { isLoading.value = false }
}

onMounted(async () => { await loadData() })
</script>

<style lang="scss" scoped>
$primary: #667eea;

.admin-roles-page { min-height: 100vh; background: #f5f7fa; }

.list-area { height: calc(100vh - 88rpx); padding: 24rpx; }

.role-card {
  background: #fff;
  border-radius: 24rpx;
  padding: 28rpx;
  margin-bottom: 20rpx;
  box-shadow: 0 4rpx 16rpx rgba(0,0,0,0.06);

  &__header { display: flex; align-items: center; gap: 12rpx; margin-bottom: 12rpx; }
  &__footer { display: flex; align-items: center; justify-content: space-between; padding-top: 16rpx; border-top: 1rpx solid #f0f0f0; margin-top: 12rpx; }
}

.role-name { font-size: 30rpx; font-weight: bold; color: #303133; }
.role-code { font-size: 24rpx; color: #909399; }
.role-desc { font-size: 26rpx; color: #606266; display: block; margin-bottom: 12rpx; }
.role-count { font-size: 24rpx; color: #909399; }

.role-perms { display: flex; flex-wrap: wrap; gap: 8rpx; }
.perm-tag { background: #f0f0f0; color: #909399; font-size: 20rpx; padding: 4rpx 12rpx; border-radius: 8rpx; }
.perm-more { font-size: 20rpx; color: #909399; }

.empty { display: flex; flex-direction: column; align-items: center; padding: 120rpx 0; gap: 16rpx; }
.empty-icon { font-size: 80rpx; color: #d0d0d0; }
.empty-text { font-size: 28rpx; color: #909399; }
</style>
