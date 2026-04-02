<template>
  <div class="users-container">
    <div class="page-header">
      <h2>用户管理</h2>
      <el-button
        v-permission="'user:create'"
        type="primary"
        @click="handleCreate"
      >
        <el-icon><Plus /></el-icon>
        新增用户
      </el-button>
    </div>

    <!-- 搜索栏 -->
    <el-card class="search-card" shadow="never">
      <el-form :model="queryForm" inline>
        <el-form-item label="关键词">
          <el-input
            v-model="queryForm.keyword"
            placeholder="用户名/姓名/邮箱"
            clearable
            @keyup.enter="handleSearch"
          />
        </el-form-item>
        <el-form-item label="部门">
          <el-select
            v-model="queryForm.departmentId"
            placeholder="选择部门"
            clearable
            style="width: 180px"
          >
            <el-option
              v-for="dept in departmentOptions"
              :key="dept.id"
              :label="dept.name"
              :value="dept.id"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="状态">
          <el-select
            v-model="queryForm.isActive"
            placeholder="选择状态"
            clearable
            style="width: 120px"
          >
            <el-option label="启用" :value="true" />
            <el-option label="禁用" :value="false" />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">
            <el-icon><Search /></el-icon>
            搜索
          </el-button>
          <el-button @click="handleReset">重置</el-button>
        </el-form-item>
      </el-form>
    </el-card>

    <!-- 用户列表 -->
    <el-card shadow="never">
      <el-table
        v-loading="loading"
        :data="userList"
        stripe
        style="width: 100%"
      >
        <el-table-column prop="username" label="用户名" min-width="120" />
        <el-table-column prop="fullName" label="姓名" min-width="120" />
        <el-table-column prop="email" label="邮箱" min-width="180" />
        <el-table-column prop="phone" label="电话" min-width="130" />
        <el-table-column label="部门" min-width="150">
          <template #default="{ row }">
            {{ row.department?.name || '-' }}
          </template>
        </el-table-column>
        <el-table-column label="角色" min-width="200">
          <template #default="{ row }">
            <el-tag
              v-for="role in row.roles"
              :key="role.id"
              size="small"
              :type="getRoleType(role.code)"
              class="role-tag"
            >
              {{ role.name }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="状态" width="100">
          <template #default="{ row }">
            <el-switch
              v-model="row.isActive"
              :disabled="!hasPermission('user:update') || isSuperAdminRow(row)"
              @change="(val: boolean) => handleStatusChange(row, val)"
            />
          </template>
        </el-table-column>
        <el-table-column prop="createdAt" label="创建时间" width="180">
          <template #default="{ row }">
            {{ formatDate(row.createdAt) }}
          </template>
        </el-table-column>
        <el-table-column label="操作" width="250" fixed="right">
          <template #default="{ row }">
            <el-button
              v-permission="'user:read'"
              link
              type="primary"
              @click="handleView(row)"
            >
              查看
            </el-button>
            <el-button
              v-permission="'user:update'"
              link
              type="primary"
              :disabled="isSuperAdminRow(row)"
              @click="handleEdit(row)"
            >
              编辑
            </el-button>
            <el-button
              v-permission="'role:assign'"
              link
              type="warning"
              :disabled="isSuperAdminRow(row)"
              @click="handleAssignRoles(row)"
            >
              角色
            </el-button>
            <el-button
              v-permission="'user:reset_password'"
              link
              type="warning"
              :disabled="isSuperAdminRow(row)"
              @click="handleResetPassword(row)"
            >
              重置密码
            </el-button>
            <el-popconfirm
              title="确定删除该用户吗？"
              @confirm="handleDelete(row)"
            >
              <template #reference>
                <el-button
                  v-permission="'user:delete'"
                  link
                  type="danger"
                  :disabled="isSuperAdminRow(row)"
                >
                  删除
                </el-button>
              </template>
            </el-popconfirm>
          </template>
        </el-table-column>
      </el-table>

      <!-- 分页 -->
      <div class="pagination-container">
        <el-pagination
          v-model:current-page="queryForm.page"
          v-model:page-size="queryForm.pageSize"
          :page-sizes="[10, 20, 50, 100]"
          :total="total"
          layout="total, sizes, prev, pager, next, jumper"
          @size-change="handleSizeChange"
          @current-change="handlePageChange"
        />
      </div>
    </el-card>

    <!-- 新增/编辑对话框 -->
    <UserFormDialog
      v-model="dialogVisible"
      :type="dialogType"
      :user-data="currentUser"
      @success="handleSearch"
    />

    <!-- 分配角色对话框 -->
    <AssignRolesDialog
      v-model="rolesDialogVisible"
      :user-id="currentUser?.id"
      :user-roles="currentUser?.roles || []"
      @success="handleSearch"
    />

    <!-- 重置密码对话框 -->
    <ResetPasswordDialog
      v-model="passwordDialogVisible"
      :user-id="currentUser?.id"
      :username="currentUser?.username"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import { Plus, Search } from '@element-plus/icons-vue'
import { useAuthStore } from '../../stores/auth'
import { userApi, departmentApi, type UserListItem } from '../../api/system'
import UserFormDialog from './components/UserFormDialog.vue'
import AssignRolesDialog from './components/AssignRolesDialog.vue'
import ResetPasswordDialog from './components/ResetPasswordDialog.vue'

const authStore = useAuthStore()
const hasPermission = authStore.hasPermission

// 查询表单
const queryForm = reactive({
  keyword: '',
  departmentId: undefined as string | undefined,
  isActive: undefined as boolean | undefined,
  page: 1,
  pageSize: 20
})

// 数据
const loading = ref(false)
const userList = ref<UserListItem[]>([])
const total = ref(0)
const departmentOptions = ref<{ id: string; name: string }[]>([])

// 对话框
const dialogVisible = ref(false)
const dialogType = ref<'create' | 'edit'>('create')
const currentUser = ref<UserListItem | null>(null)
const rolesDialogVisible = ref(false)
const passwordDialogVisible = ref(false)

// 获取用户列表
const fetchUsers = async () => {
  loading.value = true
  try {
    const res = await userApi.getUsers({
      keyword: queryForm.keyword || undefined,
      departmentId: queryForm.departmentId,
      isActive: queryForm.isActive,
      page: queryForm.page,
      pageSize: queryForm.pageSize
    })
    if (res.data.code === 200) {
      userList.value = res.data.data.items
      total.value = res.data.data.total
    }
  } catch (error) {
    ElMessage.error('获取用户列表失败')
  } finally {
    loading.value = false
  }
}

// 获取部门选项
const fetchDepartments = async () => {
  try {
    const res = await departmentApi.getAllDepartments()
    if (res.data.code === 200) {
      departmentOptions.value = res.data.data
    }
  } catch (error) {
    console.error('获取部门列表失败', error)
  }
}

// 搜索
const handleSearch = () => {
  queryForm.page = 1
  fetchUsers()
}

// 重置
const handleReset = () => {
  queryForm.keyword = ''
  queryForm.departmentId = undefined
  queryForm.isActive = undefined
  queryForm.page = 1
  fetchUsers()
}

// 分页
const handleSizeChange = (val: number) => {
  queryForm.pageSize = val
  fetchUsers()
}

const handlePageChange = (val: number) => {
  queryForm.page = val
  fetchUsers()
}

// 创建用户
const handleCreate = () => {
  dialogType.value = 'create'
  currentUser.value = null
  dialogVisible.value = true
}

// 编辑用户
const handleEdit = (row: UserListItem) => {
  dialogType.value = 'edit'
  currentUser.value = row
  dialogVisible.value = true
}

// 查看用户
const handleView = (row: UserListItem) => {
  // TODO: 实现查看详情
  ElMessage.info(`查看用户: ${row.username}`)
}

// 分配角色
const handleAssignRoles = (row: UserListItem) => {
  currentUser.value = row
  rolesDialogVisible.value = true
}

// 重置密码
const handleResetPassword = (row: UserListItem) => {
  currentUser.value = row
  passwordDialogVisible.value = true
}

// 删除用户
const handleDelete = async (row: UserListItem) => {
  try {
    const res = await userApi.deleteUser(row.id)
    if (res.data.code === 200) {
      ElMessage.success('删除成功')
      fetchUsers()
    } else {
      ElMessage.error(res.data.message)
    }
  } catch (error) {
    ElMessage.error('删除失败')
  }
}

// 状态变更
const handleStatusChange = async (row: UserListItem, val: boolean) => {
  try {
    const res = await userApi.updateUserStatus(row.id, val)
    if (res.data.code === 200) {
      ElMessage.success(val ? '用户已启用' : '用户已禁用')
    } else {
      row.isActive = !val
      ElMessage.error(res.data.message)
    }
  } catch (error) {
    row.isActive = !val
    ElMessage.error('操作失败')
  }
}

// 判断是否是超级管理员
const isSuperAdminRow = (row: UserListItem) => {
  return row.roles.some(r => r.code === 'super_admin')
}

// 获取角色标签类型
const getRoleType = (code: string) => {
  const typeMap: Record<string, string> = {
    super_admin: 'danger',
    lab_admin: 'warning',
    teacher: 'success',
    student: 'info',
    auditor: ''
  }
  return typeMap[code] || ''
}

// 格式化日期
const formatDate = (date: string) => {
  if (!date) return '-'
  return new Date(date).toLocaleString('zh-CN')
}

onMounted(() => {
  fetchUsers()
  fetchDepartments()
})
</script>

<style scoped>
.users-container {
  padding: 20px;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
}

.page-header h2 {
  margin: 0;
  font-size: 20px;
  font-weight: 500;
}

.search-card {
  margin-bottom: 20px;
}

.role-tag {
  margin-right: 5px;
  margin-bottom: 2px;
}

.pagination-container {
  display: flex;
  justify-content: flex-end;
  margin-top: 20px;
}
</style>
