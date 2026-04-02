<template>
  <div class="departments-container">
    <div class="page-header">
      <h2>部门管理</h2>
      <el-button
        v-permission="'department:create'"
        type="primary"
        @click="handleCreate"
      >
        <el-icon><Plus /></el-icon>
        新增部门
      </el-button>
    </div>

    <el-card shadow="never">
      <el-tree
        v-loading="loading"
        :data="departmentTree"
        :props="{ label: 'name', children: 'children' }"
        node-key="id"
        default-expand-all
        :expand-on-click-node="false"
      >
        <template #default="{ node, data }">
          <div class="custom-tree-node">
            <span class="node-label">
              {{ data.name }}
              <el-tag size="small" type="info" style="margin-left: 8px">{{ data.code }}</el-tag>
              <el-tag v-if="!data.isActive" size="small" type="danger" style="margin-left: 5px">已禁用</el-tag>
            </span>
            <span class="node-actions">
              <span class="node-info">
                <template v-if="data.managerName">
                  负责人: {{ data.managerName }}
                </template>
              </span>
              <el-button
                v-permission="'department:create'"
                link
                type="primary"
                size="small"
                @click="handleCreateChild(data)"
              >
                添加子部门
              </el-button>
              <el-button
                v-permission="'department:update'"
                link
                type="primary"
                size="small"
                @click="handleEdit(data)"
              >
                编辑
              </el-button>
              <el-popconfirm
                title="确定删除该部门吗？"
                @confirm="handleDelete(data)"
              >
                <template #reference>
                  <el-button
                    v-permission="'department:delete'"
                    link
                    type="danger"
                    size="small"
                  >
                    删除
                  </el-button>
                </template>
              </el-popconfirm>
            </span>
          </div>
        </template>
      </el-tree>
    </el-card>

    <!-- 新增/编辑对话框 -->
    <DepartmentFormDialog
      v-model="dialogVisible"
      :type="dialogType"
      :department-data="currentDepartment"
      :parent-department="parentDepartment"
      @success="fetchDepartments"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import { Plus } from '@element-plus/icons-vue'
import { departmentApi, type DepartmentDto } from '../../api/system'
import DepartmentFormDialog from './components/DepartmentFormDialog.vue'

// 数据
const loading = ref(false)
const departmentTree = ref<DepartmentDto[]>([])

// 对话框
const dialogVisible = ref(false)
const dialogType = ref<'create' | 'edit'>('create')
const currentDepartment = ref<DepartmentDto | null>(null)
const parentDepartment = ref<DepartmentDto | null>(null)

// 获取部门树
const fetchDepartments = async () => {
  loading.value = true
  try {
    const res = await departmentApi.getDepartmentTree()
    if (res.data.code === 200) {
      departmentTree.value = res.data.data
    }
  } catch (error) {
    ElMessage.error('获取部门列表失败')
  } finally {
    loading.value = false
  }
}

// 创建部门
const handleCreate = () => {
  dialogType.value = 'create'
  currentDepartment.value = null
  parentDepartment.value = null
  dialogVisible.value = true
}

// 创建子部门
const handleCreateChild = (parent: DepartmentDto) => {
  dialogType.value = 'create'
  currentDepartment.value = null
  parentDepartment.value = parent
  dialogVisible.value = true
}

// 编辑部门
const handleEdit = (data: DepartmentDto) => {
  dialogType.value = 'edit'
  currentDepartment.value = data
  parentDepartment.value = null
  dialogVisible.value = true
}

// 删除部门
const handleDelete = async (data: DepartmentDto) => {
  try {
    const res = await departmentApi.deleteDepartment(data.id)
    if (res.data.code === 200) {
      ElMessage.success('删除成功')
      fetchDepartments()
    } else {
      ElMessage.error(res.data.message)
    }
  } catch (error) {
    ElMessage.error('删除失败')
  }
}

onMounted(() => {
  fetchDepartments()
})
</script>

<style scoped>
.departments-container {
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

.custom-tree-node {
  flex: 1;
  display: flex;
  align-items: center;
  justify-content: space-between;
  font-size: 14px;
  padding-right: 8px;
}

.node-label {
  display: flex;
  align-items: center;
}

.node-actions {
  display: flex;
  align-items: center;
  gap: 8px;
}

.node-info {
  color: #909399;
  font-size: 13px;
  margin-right: 16px;
}
</style>
