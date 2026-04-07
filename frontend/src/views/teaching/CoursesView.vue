<template>
  <div class="courses-container">
    <div class="page-header">
      <h2>课程管理</h2>
      <el-button type="primary" @click="handleCreate">
        <el-icon><Plus /></el-icon>
        新增课程
      </el-button>
    </div>

    <!-- 搜索栏 -->
    <el-card class="search-card" shadow="never">
      <el-form :model="queryForm" inline>
        <el-form-item label="关键词">
          <el-input v-model="queryForm.keyword" placeholder="课程代码/名称" clearable @keyup.enter="handleSearch" />
        </el-form-item>
        <el-form-item label="修读性质">
          <el-select v-model="queryForm.courseType" placeholder="全部" clearable style="width: 120px">
            <el-option label="必修" value="必修" />
            <el-option label="选修" value="选修" />
            <el-option label="限选" value="限选" />
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

    <!-- 课程列表 -->
    <el-card shadow="never">
      <el-table v-loading="loading" :data="courseList" stripe>
        <el-table-column prop="code" label="课程代码" width="120" />
        <el-table-column prop="name" label="课程名称" min-width="180" />
        <el-table-column prop="englishName" label="英文名称" min-width="180" show-overflow-tooltip />
        <el-table-column prop="courseType" label="修读性质" width="100">
          <template #default="{ row }">
            <el-tag :type="getCourseTypeType(row.courseType)">{{ row.courseType }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="credits" label="学分" width="80" />
        <el-table-column label="学时" width="80">
          <template #default="{ row }">
            {{ row.totalHours }}
          </template>
        </el-table-column>
        <el-table-column label="学时分配" min-width="200">
          <template #default="{ row }">
            <el-tooltip :content="`讲授:${row.theoryHours} 实践:${row.practiceHours} 实验:${row.experimentHours} 网络:${row.onlineHours}`">
              <span>讲授{{ row.theoryHours }}/实践{{ row.practiceHours }}/实验{{ row.experimentHours }}</span>
            </el-tooltip>
          </template>
        </el-table-column>
        <el-table-column prop="managerName" label="课程负责人" width="120">
          <template #default="{ row }">
            <span v-if="row.managerName">{{ row.managerName }}</span>
            <span v-else class="text-gray">-</span>
          </template>
        </el-table-column>
        <el-table-column label="状态" width="100">
          <template #default="{ row }">
            <el-switch v-model="row.isActive" @change="(val: boolean) => handleStatusChange(row, val)" />
          </template>
        </el-table-column>
        <el-table-column label="操作" width="200" fixed="right">
          <template #default="{ row }">
            <el-button link type="primary" @click="handleEdit(row)">编辑</el-button>
            <el-popconfirm title="确定删除该课程吗？" @confirm="handleDelete(row)">
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

    <CourseFormDialog v-model="dialogVisible" :type="dialogType" :course-data="currentCourse" @success="handleSearch" />
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import { Plus, Search } from '@element-plus/icons-vue'
import { courseApi, type CourseDto } from '@/api/teaching'
import CourseFormDialog from './components/CourseFormDialog.vue'

const queryForm = reactive({
  keyword: '',
  courseType: '',
  page: 1,
  pageSize: 20
})

const loading = ref(false)
const courseList = ref<CourseDto[]>([])
const total = ref(0)

const dialogVisible = ref(false)
const dialogType = ref<'create' | 'edit'>('create')
const currentCourse = ref<CourseDto | null>(null)

const fetchCourses = async () => {
  loading.value = true
  try {
    const res = await courseApi.getList({
      keyword: queryForm.keyword || undefined,
      courseType: queryForm.courseType || undefined,
      page: queryForm.page,
      pageSize: queryForm.pageSize
    })
    if (res.data.code === 200) {
      courseList.value = res.data.data
      total.value = res.data.data.length
    }
  } catch (error) {
    ElMessage.error('获取课程列表失败')
  } finally {
    loading.value = false
  }
}

const handleSearch = () => {
  queryForm.page = 1
  fetchCourses()
}

const handleReset = () => {
  queryForm.keyword = ''
  queryForm.courseType = ''
  queryForm.page = 1
  fetchCourses()
}

const handleSizeChange = (val: number) => {
  queryForm.pageSize = val
  fetchCourses()
}

const handlePageChange = (val: number) => {
  queryForm.page = val
  fetchCourses()
}

const handleCreate = () => {
  dialogType.value = 'create'
  currentCourse.value = null
  dialogVisible.value = true
}

const handleEdit = (row: CourseDto) => {
  dialogType.value = 'edit'
  currentCourse.value = row
  dialogVisible.value = true
}

const handleDelete = async (row: CourseDto) => {
  try {
    const res = await courseApi.delete(row.id)
    if (res.data.code === 200) {
      ElMessage.success('删除成功')
      fetchCourses()
    } else {
      ElMessage.error(res.data.message)
    }
  } catch (error) {
    ElMessage.error('删除失败')
  }
}

const handleStatusChange = async (row: CourseDto, val: boolean) => {
  try {
    const res = await courseApi.toggleStatus(row.id, val)
    if (res.data.code === 200) {
      ElMessage.success(val ? '课程已启用' : '课程已禁用')
    } else {
      row.isActive = !val
      ElMessage.error(res.data.message)
    }
  } catch (error) {
    row.isActive = !val
    ElMessage.error('操作失败')
  }
}

const getCourseTypeType = (type: string) => {
  const typeMap: Record<string, string> = {
    '必修': 'danger',
    '选修': 'success',
    '限选': 'warning'
  }
  return typeMap[type] || ''
}

onMounted(() => {
  fetchCourses()
})
</script>

<style scoped>
.courses-container {
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
