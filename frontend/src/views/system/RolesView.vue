<template>
  <div class="roles-container">
    <div class="page-header">
      <h2>角色管理</h2>
      <el-button
        v-permission="'role:create'"
        type="primary"
        @click="handleCreate"
      >
        <el-icon><Plus /></el-icon>
        新增角色
      </el-button>
    </div>

    <!-- 搜索栏 -->
    <el-card class="search-card" shadow="never">
      <el-form :model="queryForm" inline>
        <el-form-item label="关键词">
          <el-input
            v-model="queryForm.keyword"
            placeholder="角色编码/名称"
            clearable
            @keyup.enter="handleSearch"
          />
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

    <!-- 角色列表 -->
    <el-card shadow="never">
      <el-table
        v-loading="loading"
        :data="roleList"
        stripe
        style="width: 100%"
      >
        <el-table-column prop="code" label="角色编码" min-width="150" />
        <el-table-column prop="name" label="角色名称" min-width="150" />
        <el-table-column prop="description" label="描述" min-width="200" show-overflow-tooltip />
        <el-table-column label="类型" width="100">
          <template #default="{ row }">
            <el-tag v-if="row.isSystem" type="danger">系统</el-tag>
            <el-tag v-else type="info">自定义</el-tag>
          </template>
        </el-table-column>
        <el-table-column label="状态" width="100">
          <template #default="{ row }">
            <el-switch
              v-model="row.isActive"
              :disabled="row.isSystem || !hasPermission('role:update')"
              @change="(val: boolean) => handleStatusChange(row, val)"
            />
          </template>
        </el-table-column>
        <el-table-column prop="userCount" label="用户数" width="100" />
        <el-table-column prop="createdAt" label="创建时间" width="180">
          <template #default="{ row }">
            {{ formatDate(row.createdAt) }}
          </template>
        </el-table-column>
        <el-table-column label="操作" width="250" fixed="right">
          <template #default="{ row }">
            <el-button
              v-permission="'role:read'"
              link
              type="primary"
              @click="handleView(row)"
            >
              查看
            </el-button>
            <el-button
              v-permission="'role:update'"
              link
              type="primary"
              @click="handleEdit(row)"
            >
              编辑
            </el-button>
            <el-button
              v-permission="'permission:assign'"
              link
              type="warning"
              @click="handleAssignPermissions(row)"
            >
              权限
            </el-button>
            <el-popconfirm
              v-if="!row.isSystem"
              title="确定删除该角色吗？"
              @confirm="handleDelete(row)"
            >
              <template #reference>
                <el-button
                  v-permission="'role:delete'"
                  link
                  type="danger"
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
    <RoleFormDialog
      v-model="dialogVisible"
      :type="dialogType"
      :role-data="currentRole"
      @success="handleSearch"
    />

    <!-- 分配权限对话框 -->
    <AssignPermissionsDialog
      v-model="permissionsDialogVisible"
      :role-id="currentRole?.id"
      :role-name="currentRole?.name"
      @success="handleSearch"
    />

    <!-- 角色详情对话框 -->
    <RoleDetailDialog
      v-model="detailDialogVisible"
      :role-id="currentRole?.id"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import { Plus, Search } from '@element-plus/icons-vue'
import { useAuthStore } from '../../stores/auth'
import { roleApi, type RoleDto } from '../../api/system'
import RoleFormDialog from './components/RoleFormDialog.vue'
import AssignPermissionsDialog from './components/AssignPermissionsDialog.vue'
import RoleDetailDialog from './components/RoleDetailDialog.vue'

const authStore = useAuthStore()
const hasPermission = authStore.hasPermission

// 查询表单
const queryForm = reactive({
  keyword: '',
  isActive: undefined as boolean | undefined,
  page: 1,
  pageSize: 20
})

// 数据
const loading = ref(false)
const roleList = ref<RoleDto[]>([])
const total = ref(0)

// 对话框
const dialogVisible = ref(false)
const dialogType = ref<'create' | 'edit'>('create')
const currentRole = ref<RoleDto | null>(null)
const permissionsDialogVisible = ref(false)
const detailDialogVisible = ref(false)

// 获取角色列表
const fetchRoles = async () => {
  loading.value = true
  try {
    const res = await roleApi.getRoles({
      keyword: queryForm.keyword || undefined,
      isActive: queryForm.isActive,
      page: queryForm.page,
      pageSize: queryForm.pageSize
    })
    if (res.data.code === 200) {
      roleList.value = res.data.data.items
      total.value = res.data.data.total
    }
  } catch (error) {
    ElMessage.error('获取角色列表失败')
  } finally {
    loading.value = false
  }
}

// 搜索
const handleSearch = () => {
  queryForm.page = 1
  fetchRoles()
}

// 重置
const handleReset = () => {
  queryForm.keyword = ''
  queryForm.isActive = undefined
  queryForm.page = 1
  fetchRoles()
}

// 分页
const handleSizeChange = (val: number) => {
  queryForm.pageSize = val
  fetchRoles()
}

const handlePageChange = (val: number) => {
  queryForm.page = val
  fetchRoles()
}

// 创建角色
const handleCreate = () => {
  dialogType.value = 'create'
  currentRole.value = null
  dialogVisible.value = true
}

// 编辑角色
const handleEdit = (row: RoleDto) => {
  dialogType.value = 'edit'
  currentRole.value = row
  dialogVisible.value = true
}

// 查看角色
const handleView = (row: RoleDto) => {
  currentRole.value = row
  detailDialogVisible.value = true
}

// 分配权限
const handleAssignPermissions = (row: RoleDto) => {
  currentRole.value = row
  permissionsDialogVisible.value = true
}

// 删除角色
const handleDelete = async (row: RoleDto) => {
  try {
    const res = await roleApi.deleteRole(row.id)
    if (res.data.code === 200) {
      ElMessage.success('删除成功')
      fetchRoles()
    } else {
      ElMessage.error(res.data.message)
    }
  } catch (error) {
    ElMessage.error('删除失败')
  }
}

// 状态变更
const handleStatusChange = async (row: RoleDto, val: boolean) => {
  try {
    const res = await roleApi.updateRole(row.id, { isActive: val })
    if (res.data.code === 200) {
      ElMessage.success(val ? '角色已启用' : '角色已禁用')
    } else {
      row.isActive = !val
      ElMessage.error(res.data.message)
    }
  } catch (error) {
    row.isActive = !val
    ElMessage.error('操作失败')
  }
}

// 格式化日期
const formatDate = (date: string) => {
  if (!date) return '-'
  return new Date(date).toLocaleString('zh-CN')
}

onMounted(() => {
  fetchRoles()
})
</script>

<style scoped>
.roles-container {
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

.pagination-container {
  display: flex;
  justify-content: flex-end;
  margin-top: 20px;
}
</style>
