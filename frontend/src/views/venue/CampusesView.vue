<template>
  <div class="campuses-container">
    <el-card>
      <template #header>
        <div class="card-header">
          <span>校区管理</span>
          <el-button
            v-if="hasPermission('campus:create')"
            type="primary"
            @click="handleCreate"
          >
            <el-icon><Plus /></el-icon>新增校区
          </el-button>
        </div>
      </template>

      <!-- 搜索栏 -->
      <el-form :inline="true" :model="searchForm" class="search-form">
        <el-form-item label="关键词">
          <el-input
            v-model="searchForm.keyword"
            placeholder="校区名称/编码"
            clearable
            @keyup.enter="handleSearch"
          />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">
            <el-icon><Search /></el-icon>搜索
          </el-button>
          <el-button @click="handleReset">重置</el-button>
        </el-form-item>
      </el-form>

      <!-- 数据表格 -->
      <el-table
        v-loading="loading"
        :data="campusList"
        border
        stripe
        style="width: 100%"
      >
        <el-table-column prop="code" label="校区编码" width="120" />
        <el-table-column prop="name" label="校区名称" width="150" />
        <el-table-column prop="campusType" label="校区类型" width="100" />
        <el-table-column prop="address" label="地址" min-width="200" show-overflow-tooltip />
        <el-table-column prop="area" label="面积(㎡)" width="120">
          <template #default="{ row }">
            {{ row.area ? row.area.toLocaleString() : '-' }}
          </template>
        </el-table-column>
        <el-table-column prop="buildingCount" label="楼宇数" width="80" />
        <el-table-column prop="managerName" label="负责人" width="100" />
        <el-table-column prop="isActive" label="状态" width="80">
          <template #default="{ row }">
            <el-tag :type="row.isActive ? 'success' : 'info'">
              {{ row.isActive ? '启用' : '禁用' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="200" fixed="right">
          <template #default="{ row }">
            <el-button
              v-if="hasPermission('campus:update')"
              type="primary"
              size="small"
              @click="handleEdit(row)"
            >
              编辑
            </el-button>
            <el-button
              v-if="hasPermission('campus:delete')"
              type="danger"
              size="small"
              @click="handleDelete(row)"
            >
              删除
            </el-button>
          </template>
        </el-table-column>
      </el-table>
    </el-card>

    <!-- 新增/编辑对话框 -->
    <CampusFormDialog
      v-model="dialogVisible"
      :campus="currentCampus"
      @success="handleSearch"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { Plus, Search } from '@element-plus/icons-vue'
import { useAuthStore } from '../../stores/auth'
import { campusApi } from '../../api/venue'
import CampusFormDialog from './components/CampusFormDialog.vue'

const authStore = useAuthStore()
const hasPermission = authStore.hasPermission

const loading = ref(false)
const campusList = ref([])
const dialogVisible = ref(false)
const currentCampus = ref(null)

const searchForm = reactive({
  keyword: ''
})

const fetchCampuses = async () => {
  loading.value = true
  try {
    const res = await campusApi.getList(searchForm.keyword)
    if (res.data.code === 200) {
      campusList.value = res.data.data
    }
  } catch (error) {
    ElMessage.error('获取校区列表失败')
  } finally {
    loading.value = false
  }
}

const handleSearch = () => {
  fetchCampuses()
}

const handleReset = () => {
  searchForm.keyword = ''
  fetchCampuses()
}

const handleCreate = () => {
  currentCampus.value = null
  dialogVisible.value = true
}

const handleEdit = (row: any) => {
  currentCampus.value = row
  dialogVisible.value = true
}

const handleDelete = async (row: any) => {
  try {
    await ElMessageBox.confirm(`确定删除校区 "${row.name}" 吗？`, '提示', {
      type: 'warning'
    })
    const res = await campusApi.delete(row.id)
    if (res.data.code === 200) {
      ElMessage.success('删除成功')
      fetchCampuses()
    } else {
      ElMessage.error(res.data.message)
    }
  } catch (error: any) {
    if (error !== 'cancel') {
      ElMessage.error(error.response?.data?.message || '删除失败')
    }
  }
}

onMounted(() => {
  fetchCampuses()
})
</script>

<style scoped>
.campuses-container {
  padding: 20px;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.search-form {
  margin-bottom: 20px;
}
</style>
