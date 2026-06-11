<template>
  <view class="admin-users-page">
    <!-- 搜索栏 -->
    <view class="search-bar">
      <text class="search-icon">&#xe6c0;</text>
      <input v-model="keyword" class="search-input" placeholder="搜索用户名/姓名" confirm-type="search" @confirm="loadData" />
      <button class="add-btn" @tap="showAddSheet = true">+ 新增</button>
    </view>

    <!-- 用户列表 -->
    <scroll-view class="list-area" scroll-y refresher-enabled @refresherrefresh="loadData" @scrolltolower="loadMore">
      <view v-for="user in users" :key="user.id" class="user-card">
        <view class="user-card__avatar">
          <text>{{ user.fullName?.charAt(0) || '?' }}</text>
        </view>
        <view class="user-card__info">
          <view class="user-card__top">
            <text class="user-name">{{ user.fullName }}</text>
            <text class="user-status" :class="user.status === 1 ? 'active' : 'inactive'">
              {{ user.status === 1 ? '启用' : '禁用' }}
            </text>
          </view>
          <text class="user-meta">{{ user.username }} | {{ user.roles?.join(', ') || '无角色' }}</text>
          <text v-if="user.email" class="user-meta">{{ user.email }}</text>
        </view>
        <view class="user-card__actions">
          <text class="action-icon" @tap="editUser(user)">&#xe6d4;</text>
          <text class="action-icon action-icon--danger" @tap="deleteUser(user)">&#xe6d5;</text>
        </view>
      </view>

      <view v-if="!isLoading && users.length === 0" class="empty">
        <text class="empty-icon">&#xe6c6;</text>
        <text class="empty-text">暂无用户</text>
      </view>

      <view v-if="hasMore && users.length > 0" class="load-more"><text>加载更多...</text></view>
      <view :style="{ height: '40px' }" />
    </scroll-view>

    <!-- 新增/编辑弹窗 -->
    <view v-if="showAddSheet" class="overlay" @tap="showAddSheet = false">
      <view class="sheet" @tap.stop>
        <view class="sheet__header">
          <text class="sheet__title">{{ editingUser ? '编辑用户' : '新增用户' }}</text>
          <text class="sheet__close" @tap="showAddSheet = false">&#xe6c7;</text>
        </view>
        <scroll-view class="sheet__body" scroll-y>
          <view class="form-item">
            <text class="form-label">用户名 *</text>
            <input v-model="formData.username" class="form-input" placeholder="请输入用户名" :disabled="!!editingUser" />
          </view>
          <view class="form-item" v-if="!editingUser">
            <text class="form-label">密码 *</text>
            <input v-model="formData.password" type="password" class="form-input" placeholder="请输入密码" />
          </view>
          <view class="form-item">
            <text class="form-label">姓名 *</text>
            <input v-model="formData.fullName" class="form-input" placeholder="请输入姓名" />
          </view>
          <view class="form-item">
            <text class="form-label">邮箱</text>
            <input v-model="formData.email" class="form-input" placeholder="请输入邮箱" />
          </view>
          <view class="form-item">
            <text class="form-label">手机</text>
            <input v-model="formData.phone" class="form-input" placeholder="请输入手机号" />
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
import { getUsers, createUser, updateUser, deleteUser as deleteApi } from '@/api/user'
import type { UserInfo } from '@/types/user'

const keyword = ref('')
const page = ref(1)
const pageSize = 20
const isLoading = ref(false)
const users = ref<UserInfo[]>([])
const total = ref(0)
const showAddSheet = ref(false)
const editingUser = ref<UserInfo | null>(null)

const formData = reactive({
  username: '', password: '', fullName: '', email: '', phone: '',
})

const hasMore = computed(() => users.value.length < total.value)

const canSubmit = computed(() =>
  formData.username.trim() && (editingUser.value ? true : formData.password.trim()) && formData.fullName.trim()
)

async function loadData() {
  isLoading.value = true
  page.value = 1
  try {
    const resp = await getUsers({ page: 1, pageSize, search: keyword.value || undefined })
    users.value = resp.items
    total.value = resp.total
  } catch { /* ignore */ } finally { isLoading.value = false }
}

async function loadMore() {
  if (!hasMore.value || isLoading.value) return
  page.value++
  try {
    const resp = await getUsers({ page: page.value, pageSize, search: keyword.value || undefined })
    users.value.push(...resp.items)
  } catch { /* ignore */ }
}

function editUser(user: UserInfo) {
  editingUser.value = user
  Object.assign(formData, { username: user.username, password: '', fullName: user.fullName, email: user.email || '', phone: user.phone || '' })
  showAddSheet.value = true
}

async function submitForm() {
  if (!canSubmit.value) return
  try {
    uni.showLoading({ title: '保存中...' })
    if (editingUser.value) {
      await updateUser(editingUser.value.id, formData)
    } else {
      await createUser(formData as any)
    }
    uni.hideLoading()
    uni.showToast({ title: '保存成功', icon: 'success' })
    showAddSheet.value = false
    editingUser.value = null
    Object.assign(formData, { username: '', password: '', fullName: '', email: '', phone: '' })
    await loadData()
  } catch { uni.hideLoading() }
}

function deleteUser(user: UserInfo) {
  uni.showModal({
    title: '确认删除',
    content: `确定删除用户 "${user.fullName}" 吗？`,
    success: async (res) => {
      if (res.confirm) {
        try {
          uni.showLoading({ title: '删除中...' })
          await deleteApi(user.id)
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

.admin-users-page { min-height: 100vh; background: #f5f7fa; }

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

.list-area { height: calc(100vh - 104rpx); padding: 24rpx; }

.user-card {
  background: #fff;
  border-radius: 24rpx;
  padding: 24rpx;
  margin-bottom: 20rpx;
  box-shadow: 0 4rpx 16rpx rgba(0, 0, 0, 0.06);
  display: flex;
  align-items: center;
  gap: 20rpx;

  &__avatar {
    width: 80rpx;
    height: 80rpx;
    border-radius: 50%;
    background: linear-gradient(135deg, $primary, #764ba2);
    display: flex;
    align-items: center;
    justify-content: center;
    flex-shrink: 0;
    text { font-size: 32rpx; font-weight: bold; color: #fff; }
  }

  &__info { flex: 1; }

  &__top { display: flex; align-items: center; gap: 12rpx; margin-bottom: 8rpx; }

  &__actions { display: flex; gap: 20rpx; }
}

.user-name { font-size: 30rpx; font-weight: bold; color: #303133; }

.user-status {
  font-size: 22rpx;
  padding: 2rpx 12rpx;
  border-radius: 999rpx;
  &.active { background: #e8f8e8; color: #67c23a; }
  &.inactive { background: #f5f5f5; color: #909399; }
}

.user-meta { font-size: 24rpx; color: #909399; display: block; }

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
