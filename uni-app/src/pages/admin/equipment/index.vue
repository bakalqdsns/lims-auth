<template>
  <view class="admin-equipment-page">
    <view class="search-bar">
      <text class="search-icon">&#xe6c0;</text>
      <input v-model="keyword" class="search-input" placeholder="搜索设备名称/编号" confirm-type="search" @confirm="loadData" />
      <button class="add-btn" @tap="showAddSheet = true">+ 新增</button>
    </view>

    <!-- 统计卡片 -->
    <view v-if="stats" class="stats-bar">
      <view class="stat-item"><text class="stat-num">{{ stats.totalCount }}</text><text class="stat-label">总数</text></view>
      <view class="stat-item"><text class="stat-num stat-num--green">{{ stats.availableCount }}</text><text class="stat-label">可用</text></view>
      <view class="stat-item"><text class="stat-num stat-num--yellow">{{ stats.borrowedCount }}</text><text class="stat-label">已借</text></view>
      <view class="stat-item"><text class="stat-num stat-num--red">{{ stats.maintenanceCount }}</text><text class="stat-label">维护</text></view>
    </view>

    <!-- 设备列表 -->
    <scroll-view class="list-area" scroll-y refresher-enabled @refresherrefresh="loadData" @scrolltolower="loadMore">
      <view v-for="eq in equipments" :key="eq.id" class="equip-card">
        <view class="equip-card__left">
          <view class="equip-icon"><text>&#xe6a3;</text></view>
        </view>
        <view class="equip-card__info">
          <text class="equip-name">{{ eq.name }}</text>
          <text class="equip-code">{{ eq.code }}</text>
          <view class="equip-meta">
            <text class="meta-tag">{{ eq.category || '未分类' }}</text>
            <text class="meta-qty">库存 {{ eq.availableQuantity }}/{{ eq.totalQuantity }}</text>
          </view>
        </view>
        <view class="equip-card__actions">
          <text class="action-icon" @tap="editEquip(eq)">&#xe6d4;</text>
          <text class="action-icon action-icon--danger" @tap="deleteEquip(eq)">&#xe6d5;</text>
        </view>
      </view>

      <view v-if="!isLoading && equipments.length === 0" class="empty">
        <text class="empty-icon">&#xe6c6;</text>
        <text class="empty-text">暂无设备</text>
      </view>

      <view v-if="hasMore && equipments.length > 0" class="load-more"><text>加载更多...</text></view>
      <view :style="{ height: '40px' }" />
    </scroll-view>

    <!-- 新增/编辑弹窗 -->
    <view v-if="showAddSheet" class="overlay" @tap="showAddSheet = false">
      <view class="sheet" @tap.stop>
        <view class="sheet__header">
          <text class="sheet__title">{{ editingEquip ? '编辑设备' : '新增设备' }}</text>
          <text class="sheet__close" @tap="showAddSheet = false">&#xe6c7;</text>
        </view>
        <scroll-view class="sheet__body" scroll-y>
          <view class="form-item">
            <text class="form-label">设备名称 *</text>
            <input v-model="formData.name" class="form-input" placeholder="请输入设备名称" />
          </view>
          <view class="form-item">
            <text class="form-label">设备编号 *</text>
            <input v-model="formData.code" class="form-input" placeholder="请输入设备编号" :disabled="!!editingEquip" />
          </view>
          <view class="form-item">
            <text class="form-label">分类</text>
            <input v-model="formData.category" class="form-input" placeholder="请输入设备分类" />
          </view>
          <view class="form-item">
            <text class="form-label">型号</text>
            <input v-model="formData.model" class="form-input" placeholder="请输入型号" />
          </view>
          <view class="form-item">
            <text class="form-label">数量 *</text>
            <input v-model.number="formData.totalQuantity" type="number" class="form-input" placeholder="请输入数量" />
          </view>
        </scroll-view>
        <view class="sheet__footer">
          <button class="submit-btn" :disabled="!canSubmit" @tap="submitForm">保存</button>
        </view>
      </view>
    </view>
  </view>
</template>

<script setup lang="ts">
import { ref, computed, reactive, onMounted } from 'vue'
import { getEquipments, createEquipment, updateEquipment, deleteEquipment as deleteApi, getEquipmentStatistics } from '@/api/equipment'
import type { Equipment, EquipmentStatistics } from '@/types/equipment'

const keyword = ref('')
const page = ref(1)
const pageSize = 20
const isLoading = ref(false)
const equipments = ref<Equipment[]>([])
const total = ref(0)
const stats = ref<EquipmentStatistics | null>(null)
const showAddSheet = ref(false)
const editingEquip = ref<Equipment | null>(null)

const formData = reactive({ name: '', code: '', category: '', model: '', totalQuantity: 1 })

const hasMore = computed(() => equipments.value.length < total.value)
const canSubmit = computed(() => formData.name.trim() && formData.code.trim() && formData.totalQuantity > 0)

async function loadData() {
  isLoading.value = true
  page.value = 1
  try {
    const [eqResp, statResp] = await Promise.all([
      getEquipments({ page: 1, pageSize, keyword: keyword.value || undefined }),
      getEquipmentStatistics(),
    ])
    equipments.value = eqResp.items
    total.value = eqResp.total
    stats.value = statResp
  } catch { /* ignore */ } finally { isLoading.value = false }
}

async function loadMore() {
  if (!hasMore.value || isLoading.value) return
  page.value++
  try {
    const resp = await getEquipments({ page: page.value, pageSize, keyword: keyword.value || undefined })
    equipments.value.push(...resp.items)
  } catch { /* ignore */ }
}

function editEquip(eq: Equipment) {
  editingEquip.value = eq
  Object.assign(formData, { name: eq.name, code: eq.code, category: eq.category || '', model: eq.model || '', totalQuantity: eq.totalQuantity })
  showAddSheet.value = true
}

async function submitForm() {
  if (!canSubmit.value) return
  try {
    uni.showLoading({ title: '保存中...' })
    if (editingEquip.value) {
      await updateEquipment(editingEquip.value.id, formData)
    } else {
      await createEquipment(formData as any)
    }
    uni.hideLoading()
    uni.showToast({ title: '保存成功', icon: 'success' })
    showAddSheet.value = false
    editingEquip.value = null
    Object.assign(formData, { name: '', code: '', category: '', model: '', totalQuantity: 1 })
    await loadData()
  } catch { uni.hideLoading() }
}

function deleteEquip(eq: Equipment) {
  uni.showModal({
    title: '确认删除',
    content: `确定删除设备 "${eq.name}" 吗？`,
    success: async (res) => {
      if (res.confirm) {
        try {
          uni.showLoading({ title: '删除中...' })
          await deleteApi(eq.id)
          uni.hideLoading()
          uni.showToast({ title: '已删除', icon: 'success' })
          await loadData()
        } catch { uni.hideLoading() }
      }
    },
  })
}

onMounted(async () => { await loadData() })
</script>

<style lang="scss" scoped>
$primary: #667eea;

.admin-equipment-page { min-height: 100vh; background: #f5f7fa; }

.search-bar {
  display: flex;
  align-items: center;
  background: $primary;
  padding: 20rpx 24rpx;
  gap: 16rpx;

  .search-icon { font-size: 32rpx; color: rgba(255,255,255,0.8); }

  .search-input {
    flex: 1;
    height: 64rpx;
    background: rgba(255, 255, 255, 0.2);
    border-radius: 999rpx;
    padding: 0 24rpx;
    font-size: 28rpx;
    color: #fff;
  }

  .add-btn {
    background: rgba(255, 255, 255, 0.2);
    color: #fff;
    font-size: 26rpx;
    padding: 0 20rpx;
    height: 64rpx;
    border-radius: 999rpx;
    border: none;
    margin: 0;
    display: flex;
    align-items: center;
    justify-content: center;
    &::after { border: none; }
  }
}

.stats-bar {
  display: flex;
  background: #fff;
  padding: 24rpx 0;
  margin-bottom: 2rpx;
}

.stat-item {
  flex: 1;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 6rpx;
}

.stat-num { font-size: 36rpx; font-weight: bold; color: #303133; &--green { color: #67c23a; } &--yellow { color: #e6a23c; } &--red { color: #f56c6c; } }
.stat-label { font-size: 22rpx; color: #909399; }

.list-area { height: calc(100vh - 280rpx); padding: 24rpx; }

.equip-card {
  background: #fff;
  border-radius: 24rpx;
  padding: 24rpx;
  margin-bottom: 20rpx;
  box-shadow: 0 4rpx 16rpx rgba(0, 0, 0, 0.06);
  display: flex;
  align-items: center;
  gap: 20rpx;

  &__info { flex: 1; }
  &__actions { display: flex; gap: 20rpx; }
}

.equip-icon {
  width: 80rpx;
  height: 80rpx;
  background: #f0f9eb;
  border-radius: 20rpx;
  display: flex;
  align-items: center;
  justify-content: center;
  text { font-size: 40rpx; color: #67c23a; }
}

.equip-name { font-size: 30rpx; font-weight: bold; color: #303133; display: block; margin-bottom: 6rpx; }
.equip-code { font-size: 24rpx; color: #909399; display: block; margin-bottom: 12rpx; }

.equip-meta { display: flex; gap: 12rpx; align-items: center; }
.meta-tag { background: #f0f0f0; color: #909399; font-size: 22rpx; padding: 4rpx 12rpx; border-radius: 8rpx; }
.meta-qty { font-size: 24rpx; color: #67c23a; }

.action-icon { font-size: 36rpx; color: $primary; }
.action-icon--danger { color: #f56c6c; }

.empty { display: flex; flex-direction: column; align-items: center; padding: 120rpx 0; gap: 16rpx; }
.empty-icon { font-size: 80rpx; color: #d0d0d0; }
.empty-text { font-size: 28rpx; color: #909399; }
.load-more { text-align: center; padding: 24rpx; font-size: 24rpx; color: #909399; }

.overlay { position: fixed; top: 0; left: 0; right: 0; bottom: 0; background: rgba(0, 0, 0, 0.5); z-index: 999; display: flex; align-items: flex-end; }

.sheet {
  width: 100%;
  max-height: 70vh;
  background: #fff;
  border-radius: 32rpx 32rpx 0 0;
  display: flex;
  flex-direction: column;

  &__header { display: flex; align-items: center; justify-content: space-between; padding: 32rpx; border-bottom: 1rpx solid #f0f0f0; }
  &__title { font-size: 32rpx; font-weight: bold; color: #303133; }
  &__close { font-size: 36rpx; color: #909399; }
  &__body { flex: 1; padding: 32rpx; max-height: 45vh; }
  &__footer { padding: 24rpx 32rpx; border-top: 1rpx solid #f0f0f0; }
}

.form-item { margin-bottom: 28rpx; }
.form-label { font-size: 28rpx; color: #606266; display: block; margin-bottom: 12rpx; }

.form-input {
  width: 100%;
  height: 80rpx;
  background: #f5f7fa;
  border-radius: 16rpx;
  padding: 0 24rpx;
  font-size: 28rpx;
  color: #303133;
  box-sizing: border-box;
  border: 2rpx solid transparent;
  &[disabled] { opacity: 0.6; }
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
