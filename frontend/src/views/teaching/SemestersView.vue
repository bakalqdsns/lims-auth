<template>
  <div class="semesters-container">
    <div class="page-header">
      <h2>学期管理</h2>
      <el-button type="primary" @click="handleCreate">
        <el-icon><Plus /></el-icon>
        新增学期
      </el-button>
    </div>

    <!-- 搜索栏 -->
    <el-card class="search-card" shadow="never">
      <el-form :model="queryForm" inline>
        <el-form-item label="关键词">
          <el-input
            v-model="queryForm.keyword"
            placeholder="学期名称"
            clearable
            @keyup.enter="handleSearch"
          />
        </el-form-item>
        <el-form-item label="当前学期">
          <el-select v-model="queryForm.isCurrent" placeholder="全部" clearable style="width: 120px">
            <el-option label="是" :value="true" />
            <el-option label="否" :value="false" />
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

    <!-- 学期列表 -->
    <el-card shadow="never">
      <el-table v-loading="loading" :data="semesterList" stripe>
        <el-table-column prop="name" label="学期名称" min-width="200" />
        <el-table-column label="开始日期" width="120">
          <template #default="{ row }">
            {{ formatDate(row.startDate) }}
          </template>
        </el-table-column>
        <el-table-column label="结束日期" width="120">
          <template #default="{ row }">
            {{ formatDate(row.endDate) }}
          </template>
        </el-table-column>
        <el-table-column label="当前学期" width="100">
          <template #default="{ row }">
            <el-tag v-if="row.isCurrent" type="success">当前</el-tag>
            <el-tag v-else type="info">否</el-tag>
          </template>
        </el-table-column>
        <el-table-column label="状态" width="100">
          <template #default="{ row }">
            <el-switch v-model="row.isActive" @change="(val: boolean) => handleStatusChange(row, val)" />
          </template>
        </el-table-column>
        <el-table-column label="操作" width="300" fixed="right">
          <template #default="{ row }">
            <el-button link type="primary" @click="handleViewCalendar(row)">校历</el-button>
            <el-button v-if="!row.isCurrent" link type="success" @click="handleSetCurrent(row)">设为当前</el-button>
            <el-button link type="primary" @click="handleEdit(row)">编辑</el-button>
            <el-popconfirm title="确定删除该学期吗？" @confirm="handleDelete(row)">
              <template #reference>
                <el-button link type="danger">删除</el-button>
              </template>
            </el-popconfirm>
          </template>
        </el-table-column>
      </el-table>

      <div class="pagination-container">
        <el-pagination
          v-model:current-page="queryForm.page"
          v-model:page-size="queryForm.pageSize"
          :page-sizes="[10, 20, 50]"
          :total="total"
          layout="total, sizes, prev, pager, next"
          @size-change="handleSizeChange"
          @current-change="handlePageChange"
        />
      </div>
    </el-card>

    <!-- 学期表单对话框 -->
    <SemesterFormDialog v-model="dialogVisible" :type="dialogType" :semester-data="currentSemester" @success="handleSearch" />
    
    <!-- 校历对话框 -->
    <CalendarViewDialog v-model="calendarDialogVisible" :semester-id="currentSemester?.id" :semester-name="currentSemester?.name" />
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import { Plus, Search } from '@element-plus/icons-vue'
import { semesterApi, type SemesterDto } from '@/api/teaching'
import SemesterFormDialog from './components/SemesterFormDialog.vue'
import CalendarViewDialog from './components/CalendarViewDialog.vue'

const queryForm = reactive({
  keyword: '',
  isCurrent: undefined as boolean | undefined,
  page: 1,
  pageSize: 20
})

const loading = ref(false)
const semesterList = ref<SemesterDto[]>([])
const total = ref(0)

const dialogVisible = ref(false)
const dialogType = ref<'create' | 'edit'>('create')
const currentSemester = ref<SemesterDto | null>(null)

const calendarDialogVisible = ref(false)

const fetchSemesters = async () => {
  loading.value = true
  try {
    const res = await semesterApi.getList({
      keyword: queryForm.keyword || undefined,
      isCurrent: queryForm.isCurrent,
      page: queryForm.page,
      pageSize: queryForm.pageSize
    })
    if (res.data.code === 200) {
      semesterList.value = res.data.data
      total.value = res.data.data.length
    }
  } catch (error) {
    ElMessage.error('获取学期列表失败')
  } finally {
    loading.value = false
  }
}

const handleSearch = () => {
  queryForm.page = 1
  fetchSemesters()
}

const handleReset = () => {
  queryForm.keyword = ''
  queryForm.isCurrent = undefined
  queryForm.page = 1
  fetchSemesters()
}

const handleSizeChange = (val: number) => {
  queryForm.pageSize = val
  fetchSemesters()
}

const handlePageChange = (val: number) => {
  queryForm.page = val
  fetchSemesters()
}

const handleCreate = () => {
  dialogType.value = 'create'
  currentSemester.value = null
  dialogVisible.value = true
}

const handleEdit = (row: SemesterDto) => {
  dialogType.value = 'edit'
  currentSemester.value = row
  dialogVisible.value = true
}

const handleDelete = async (row: SemesterDto) => {
  try {
    const res = await semesterApi.delete(row.id)
    if (res.data.code === 200) {
      ElMessage.success('删除成功')
      fetchSemesters()
    } else {
      ElMessage.error(res.data.message)
    }
  } catch (error) {
    ElMessage.error('删除失败')
  }
}

const handleStatusChange = async (row: SemesterDto, val: boolean) => {
  try {
    const res = await semesterApi.update(row.id, { isActive: val })
    if (res.data.code === 200) {
      ElMessage.success(val ? '学期已启用' : '学期已禁用')
    } else {
      row.isActive = !val
      ElMessage.error(res.data.message)
    }
  } catch (error) {
    row.isActive = !val
    ElMessage.error('操作失败')
  }
}

const handleSetCurrent = async (row: SemesterDto) => {
  try {
    const res = await semesterApi.setCurrent(row.id)
    if (res.data.code === 200) {
      ElMessage.success('已设为当前学期')
      fetchSemesters()
    } else {
      ElMessage.error(res.data.message)
    }
  } catch (error) {
    ElMessage.error('设置失败')
  }
}

const handleViewCalendar = (row: SemesterDto) => {
  currentSemester.value = row
  calendarDialogVisible.value = true
}

const formatDate = (date: string) => {
  if (!date) return '-'
  return new Date(date).toLocaleDateString('zh-CN')
}

onMounted(() => {
  fetchSemesters()
})
</script>

<style scoped>
.semesters-container {
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
