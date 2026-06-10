<template>
  <view class="borrow-page">
    <!-- 顶部 Tab -->
    <view class="tab-bar">
      <view
        v-for="tab in tabs"
        :key="tab.value"
        class="tab-item"
        :class="{ active: currentTab === tab.value }"
        @tap="switchTab(tab.value)"
      >
        <text>{{ tab.label }}</text>
        <view v-if="tab.count" class="tab-badge">{{ tab.count }}</view>
      </view>
    </view>

    <!-- 列表 -->
    <scroll-view class="list-area" scroll-y refresher-enabled @refresherrefresh="loadData" @scrolltolower="loadMore">
      <view v-for="item in records" :key="item.id" class="borrow-card" @tap="goDetail(item.id)">
        <view class="borrow-card__header">
          <view class="borrow-card__equip">
            <text class="borrow-card__name">{{ item.equipmentName }}</text>
            <text class="borrow-card__code">{{ item.equipmentCode }}</text>
          </view>
          <view class="status-tag" :class="'status--' + item.status">{{ statusLabel(item.status) }}</view>
        </view>
        <view class="borrow-card__body">
          <view class="borrow-meta">
            <text class="meta-item">借出: {{ item.borrowDate }}</text>
            <text class="meta-item">应还: {{ item.expectedReturnDate }}</text>
          </view>
          <text v-if="item.purpose" class="borrow-purpose">{{ item.purpose }}</text>
        </view>
        <view class="borrow-card__footer">
          <text class="borrow-no">{{ item.recordNo }}</text>
          <text class="borrow-detail">查看详情 &#xe6c5;</text>
        </view>
      </view>

      <view v-if="!isLoading && records.length === 0" class="empty">
        <text class="empty-icon">&#xe6c6;</text>
        <text class="empty-text">暂无借用记录</text>
      </view>

      <view v-if="hasMore && records.length > 0" class="load-more"><text>加载更多...</text></view>
      <view :style="{ height: '40px' }" />
    </scroll-view>

    <!-- 借用按钮 -->
    <view class="bottom-bar">
      <button class="new-borrow-btn" @tap="showBorrowSheet = true">+ 新建借用</button>
    </view>

    <!-- 新建借用弹窗 -->
    <view v-if="showBorrowSheet" class="borrow-overlay" @tap="showBorrowSheet = false">
      <view class="borrow-sheet" @tap.stop>
        <view class="borrow-sheet__header">
          <text class="borrow-sheet__title">新建借用</text>
          <text class="borrow-sheet__close" @tap="showBorrowSheet = false">&#xe6c7;</text>
        </view>
        <scroll-view class="borrow-sheet__body" scroll-y>
          <view class="form-item">
            <text class="form-label">选择设备</text>
            <picker mode="selector" :range="equipmentOptions" range-key="name" @change="onEquipChange">
              <view class="picker-val">{{ newBorrow.equipmentName || '请选择设备' }}</view>
            </picker>
          </view>
          <view class="form-item">
            <text class="form-label">借用日期</text>
            <picker mode="date" :value="newBorrow.borrowDate" @change="onBorrowDateChange">
              <view class="picker-val">{{ newBorrow.borrowDate || '请选择日期' }}</view>
            </picker>
          </view>
          <view class="form-item">
            <text class="form-label">预计归还日期</text>
            <picker mode="date" :value="newBorrow.expectedReturnDate" @change="onReturnDateChange">
              <view class="picker-val">{{ newBorrow.expectedReturnDate || '请选择日期' }}</view>
            </picker>
          </view>
          <view class="form-item">
            <text class="form-label">借用用途</text>
            <textarea v-model="newBorrow.purpose" class="form-textarea" placeholder="请输入借用用途" :maxlength="200" />
          </view>
          <view class="form-item">
            <text class="form-label">备注</text>
            <input v-model="newBorrow.remark" class="form-input" placeholder="备注信息（可选）" />
          </view>
        </scroll-view>
        <view class="borrow-sheet__footer">
          <button class="submit-btn" :disabled="!canSubmit" @tap="submitBorrow">提交申请</button>
        </view>
      </view>
    </view>
  </view>
</template>

<script setup lang="ts">
import { ref, computed, reactive, onMounted } from 'vue'
import { getBorrowRecords, createBorrowRecord } from '@/api/borrow-record'
import { getEquipments } from '@/api/equipment'
import type { BorrowRecord, BorrowStatus, CreateBorrowRequest } from '@/types/borrow'
import type { Equipment } from '@/types/equipment'

const currentTab = ref<BorrowStatus | 'all'>('all')
const page = ref(1)
const pageSize = 20
const isLoading = ref(false)
const records = ref<BorrowRecord[]>([])
const total = ref(0)
const showBorrowSheet = ref(false)
const allEquipments = ref<Equipment[]>([])

const newBorrow = reactive({
  equipmentId: undefined as number | undefined,
  equipmentName: '',
  borrowDate: '',
  expectedReturnDate: '',
  purpose: '',
  remark: '',
})

const tabs = [
  { label: '全部', value: 'all' as const, count: null },
  { label: '我借的', value: 'borrowed' as const, count: null },
  { label: '待归还', value: 'pending' as const, count: null },
  { label: '逾期', value: 'overdue' as const, count: null },
]

const equipmentOptions = computed(() =>
  allEquipments.value.map((e) => ({ id: e.id, name: `${e.name} (${e.code}) 可用:${e.availableQuantity}` }))
)

const hasMore = computed(() => records.value.length < total.value)

const canSubmit = computed(
  () => newBorrow.equipmentId && newBorrow.borrowDate && newBorrow.expectedReturnDate && newBorrow.purpose.trim()
)

function statusLabel(status: BorrowStatus | string): string {
  const map: Record<string, string> = {
    pending: '待审批',
    approved: '已通过',
    borrowed: '已借出',
    returning: '归还中',
    returned: '已归还',
    renewing: '续借中',
    rejected: '已拒绝',
    cancelled: '已取消',
    overdue: '已逾期',
  }
  return map[status] || status
}

async function switchTab(tab: BorrowStatus | 'all') {
  currentTab.value = tab
  page.value = 1
  await loadData()
}

async function loadData() {
  isLoading.value = true
  page.value = 1
  try {
    const query: Record<string, string | number> = { page: 1, pageSize }
    if (currentTab.value === 'pending') query.status = 'borrowed'
    else if (currentTab.value !== 'all') query.status = currentTab.value

    const resp = await getBorrowRecords(query)
    records.value = resp?.items ?? []
    total.value = resp.total
  } catch {
    // ignore
  } finally {
    isLoading.value = false
  }
}

async function loadMore() {
  if (!hasMore.value || isLoading.value) return
  page.value++
  try {
    const resp = await getBorrowRecords({ page: page.value, pageSize })
    records.value.push(...(resp?.items ?? []))
  } catch {
    // ignore
  }
}

function goDetail(id: number) {
  uni.navigateTo({ url: `/pages/borrow-detail/index?id=${id}` })
}

function onEquipChange(e: { detail: { value: number } }) {
  const eq = allEquipments.value[e.detail.value]
  newBorrow.equipmentId = eq.id
  newBorrow.equipmentName = eq.name
}

function onBorrowDateChange(e: { detail: { value: string } }) {
  newBorrow.borrowDate = e.detail.value
}

function onReturnDateChange(e: { detail: { value: string } }) {
  newBorrow.expectedReturnDate = e.detail.value
}

async function submitBorrow() {
  if (!canSubmit.value || !newBorrow.equipmentId) return
  try {
    uni.showLoading({ title: '提交中...' })
    await createBorrowRecord({
      equipmentId: newBorrow.equipmentId,
      borrowDate: newBorrow.borrowDate,
      expectedReturnDate: newBorrow.expectedReturnDate,
      purpose: newBorrow.purpose,
      remark: newBorrow.remark || undefined,
    } as CreateBorrowRequest)
    uni.hideLoading()
    uni.showToast({ title: '申请已提交', icon: 'success' })
    showBorrowSheet.value = false
    await loadData()
  } catch {
    uni.hideLoading()
  }
}

onMounted(async () => {
  await loadData()
  try {
    const resp = await getEquipments({ pageSize: 100 })
    allEquipments.value = (resp?.items ?? []).filter((e) => e.availableQuantity > 0)
  } catch {
    // ignore
  }
})
</script>

<style lang="scss" scoped>
$primary: #667eea;

.borrow-page { min-height: 100vh; background: #f5f7fa; }

.tab-bar {
  display: flex;
  background: #fff;
  padding: 0 16rpx;
  border-bottom: 1rpx solid #f0f0f0;
}

.tab-item {
  flex: 1;
  height: 88rpx;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 8rpx;
  font-size: 28rpx;
  color: #606266;
  position: relative;

  &.active {
    color: $primary;
    font-weight: bold;
    &::after {
      content: '';
      position: absolute;
      bottom: 0;
      left: 50%;
      transform: translateX(-50%);
      width: 48rpx;
      height: 6rpx;
      background: $primary;
      border-radius: 3rpx;
    }
  }
}

.tab-badge {
  background: #f56c6c;
  color: #fff;
  font-size: 20rpx;
  padding: 2rpx 10rpx;
  border-radius: 999rpx;
}

.list-area { height: calc(100vh - 88rpx); padding: 24rpx; }

.borrow-card {
  background: #fff;
  border-radius: 24rpx;
  padding: 28rpx;
  margin-bottom: 20rpx;
  box-shadow: 0 4rpx 16rpx rgba(0, 0, 0, 0.06);

  &__header {
    display: flex;
    align-items: flex-start;
    justify-content: space-between;
    margin-bottom: 16rpx;
  }

  &__equip {}

  &__name {
    font-size: 30rpx;
    font-weight: bold;
    color: #303133;
    display: block;
    margin-bottom: 6rpx;
  }

  &__code {
    font-size: 24rpx;
    color: #909399;
  }

  &__body { margin-bottom: 16rpx; }

  &__footer {
    display: flex;
    align-items: center;
    justify-content: space-between;
    padding-top: 16rpx;
    border-top: 1rpx solid #f0f0f0;
  }
}

.status-tag {
  font-size: 24rpx;
  padding: 4rpx 16rpx;
  border-radius: 999rpx;
  &.status--pending { background: #fef0f0; color: #f56c6c; }
  &.status--approved, &.status--borrowed { background: #f0f9eb; color: #67c23a; }
  &.status--returned { background: #e8f4ff; color: #909399; }
  &.status--overdue { background: #f56c6c; color: #fff; }
  &.status--rejected, &.status--cancelled { background: #f5f5f5; color: #909399; }
}

.borrow-meta {
  display: flex;
  gap: 24rpx;
  margin-bottom: 8rpx;
}

.meta-item { font-size: 26rpx; color: #909399; }

.borrow-purpose {
  font-size: 26rpx;
  color: #606266;
  display: block;
}

.borrow-no { font-size: 22rpx; color: #c0c4cc; }

.borrow-detail { font-size: 24rpx; color: $primary; }

.empty {
  display: flex;
  flex-direction: column;
  align-items: center;
  padding: 120rpx 0;
  gap: 16rpx;
}

.empty-icon { font-size: 80rpx; color: #d0d0d0; }
.empty-text { font-size: 28rpx; color: #909399; }

.load-more { text-align: center; padding: 24rpx; font-size: 24rpx; color: #909399; }

.bottom-bar {
  position: fixed;
  bottom: 0;
  left: 0;
  right: 0;
  padding: 24rpx;
  background: #fff;
  box-shadow: 0 -4rpx 16rpx rgba(0, 0, 0, 0.06);
}

.new-borrow-btn {
  width: 100%;
  height: 88rpx;
  background: linear-gradient(135deg, $primary 0%, #764ba2 100%);
  border-radius: 16rpx;
  color: #fff;
  font-size: 32rpx;
  font-weight: bold;
  display: flex;
  align-items: center;
  justify-content: center;
  border: none;
  &::after { border: none; }
}

.borrow-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.5);
  z-index: 999;
  display: flex;
  align-items: flex-end;
}

.borrow-sheet {
  width: 100%;
  max-height: 80vh;
  background: #fff;
  border-radius: 32rpx 32rpx 0 0;
  display: flex;
  flex-direction: column;

  &__header {
    display: flex;
    align-items: center;
    justify-content: space-between;
    padding: 32rpx;
    border-bottom: 1rpx solid #f0f0f0;
  }

  &__title { font-size: 32rpx; font-weight: bold; color: #303133; }
  &__close { font-size: 36rpx; color: #909399; }

  &__body {
    flex: 1;
    padding: 32rpx;
    max-height: 50vh;
  }

  &__footer {
    padding: 24rpx 32rpx;
    border-top: 1rpx solid #f0f0f0;
  }
}

.form-item { margin-bottom: 28rpx; }
.form-label { font-size: 28rpx; color: #606266; display: block; margin-bottom: 12rpx; }

.form-input,
.picker-val {
  width: 100%;
  height: 80rpx;
  background: #f5f7fa;
  border-radius: 16rpx;
  padding: 0 24rpx;
  font-size: 28rpx;
  color: #303133;
  box-sizing: border-box;
  border: 2rpx solid transparent;
}

.form-textarea {
  width: 100%;
  min-height: 160rpx;
  background: #f5f7fa;
  border-radius: 16rpx;
  padding: 20rpx 24rpx;
  font-size: 28rpx;
  color: #303133;
  box-sizing: border-box;
  border: 2rpx solid transparent;
}

.submit-btn {
  width: 100%;
  height: 88rpx;
  background: linear-gradient(135deg, $primary 0%, #764ba2 100%);
  border-radius: 16rpx;
  color: #fff;
  font-size: 32rpx;
  font-weight: bold;
  display: flex;
  align-items: center;
  justify-content: center;
  border: none;
  &::after { border: none; }
  &[disabled] { background: #d0d0d0; color: #909399; }
}
</style>
