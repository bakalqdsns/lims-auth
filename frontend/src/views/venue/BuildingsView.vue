<template>
  <div class="buildings-container">
    <el-card>
      <template #header>
        <div class="card-header">
          <span>楼宇管理</span>
          <el-button
            v-if="hasPermission('building:create')"
            type="primary"
            @click="handleCreate"
          >
            <el-icon><Plus /></el-icon>新增楼宇
          </el-button>
        </div>
      </template>

      <!-- 搜索栏 -->
      <el-form :inline="true" :model="searchForm" class="search-form">
        <el-form-item label="校区">
          <el-select v-model="searchForm.campusId" placeholder="全部校区" clearable style="width: 180px">
            <el-option
              v-for="campus in campusList"
              :key="campus.id"
              :label="campus.name"
              :value="campus.id"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="关键词">
          <el-input
            v-model="searchForm.keyword"
            placeholder="楼宇名称/编码"
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
        :data="buildingList"
        border
        stripe
        style="width: 100%"
      >
        <el-table-column prop="code" label="楼宇编码" width="120" />
        <el-table-column prop="name" label="楼宇名称" width="150" />
        <el-table-column prop="campusName" label="所属校区" width="120" />
        <el-table-column prop="buildingType" label="楼宇类型" width="100" />
        <el-table-column prop="floorCount" label="楼层数" width="80" />
        <el-table-column prop="buildingArea" label="建筑面积(㎡)" width="120">
          <template #default="{ row }">
            {{ row.buildingArea ? row.buildingArea.toLocaleString() : '-' }}
          </template>
        </el-table-column>
        <el-table-column prop="builtYear" label="建成年份" width="100" />
        <el-table-column prop="labCount" label="实验室数" width="90" />
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
              v-if="hasPermission('building:update')"
              type="primary"
              size="small"
              @click="handleEdit(row)"
            >
              编辑
            </el-button>
            <el-button
              v-if="hasPermission('building:delete')"
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
    <BuildingFormDialog
      v-model="dialogVisible"
      :building="currentBuilding"
      @success="handleSearch"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { Plus, Search } from '@element-plus/icons-vue'
import { useAuthStore } from '../../stores/auth'
import { buildingApi, campusApi } from '../../api/venue'
import BuildingFormDialog from './components/BuildingFormDialog.vue'

const authStore = useAuthStore()
const hasPermission = authStore.hasPermission

const loading = ref(false)
const buildingList = ref([])
const campusList = ref([])
const dialogVisible = ref(false)
const currentBuilding = ref(null)

const searchForm = reactive({
  keyword: '',
  campusId: ''
})

const fetchCampuses = async () => {
  try {
    const res = await campusApi.getList()
    if (res.data.code === 200) {
      campusList.value = res.data.data
    }
  } catch (error) {
    console.error('获取校区列表失败', error)
  }
}

const fetchBuildings = async () => {
  loading.value = true
  try {
    const res = await buildingApi.getList(searchForm.keyword, searchForm.campusId || undefined)
    if (res.data.code === 200) {
      buildingList.value = res.data.data
    }
  } catch (error) {
    ElMessage.error('获取楼宇列表失败')
  } finally {
    loading.value = false
  }
}

const handleSearch = () => {
  fetchBuildings()
}

const handleReset = () => {
  searchForm.keyword = ''
  searchForm.campusId = ''
  fetchBuildings()
}

const handleCreate = () => {
  currentBuilding.value = null
  dialogVisible.value = true
}

const handleEdit = (row: any) => {
  currentBuilding.value = row
  dialogVisible.value = true
}

const handleDelete = async (row: any) => {
  try {
    await ElMessageBox.confirm(`确定删除楼宇 "${row.name}" 吗？`, '提示', {
      type: 'warning'
    })
    const res = await buildingApi.delete(row.id)
    if (res.data.code === 200) {
      ElMessage.success('删除成功')
      fetchBuildings()
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
  fetchBuildings()
})
</script>

<style scoped>
.buildings-container {
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
