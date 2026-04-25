<template>
  <div class="page-container">
    <div class="page-header">
      <h2>实验项目库</h2>
      <el-button type="primary" @click="openDialog()">新增项目</el-button>
    </div>
    <el-card shadow="never">
      <el-table :data="list" v-loading="loading" stripe>
        <el-table-column prop="courseCode" label="课程编号" width="140" />
        <el-table-column prop="experimentName" label="项目名称" min-width="180" />
        <el-table-column prop="experimentHours" label="学时" width="80" />
        <el-table-column prop="experimentType" label="类别" width="100" />
        <el-table-column prop="experimentRequirement" label="要求" width="100" />
        <el-table-column label="操作" width="180" fixed="right">
          <template #default="{ row }">
            <el-button link type="primary" @click="openDialog(row)">编辑</el-button>
            <el-popconfirm title="确认删除该项目？" @confirm="remove(row.id)">
              <template #reference><el-button link type="danger">删除</el-button></template>
            </el-popconfirm>
          </template>
        </el-table-column>
      </el-table>
    </el-card>

    <el-dialog v-model="dialogVisible" :title="form.id ? '编辑项目' : '新增项目'" width="560px">
      <el-form :model="form" label-width="110px">
        <el-form-item label="课程编号"><el-input v-model="form.courseCode" /></el-form-item>
        <el-form-item label="项目名称"><el-input v-model="form.experimentName" /></el-form-item>
        <el-form-item label="实验学时"><el-input-number v-model="form.experimentHours" :min="0" style="width: 100%" /></el-form-item>
        <el-form-item label="实验类别"><el-select v-model="form.experimentType"><el-option label="基础" value="基础" /><el-option label="综合" value="综合" /><el-option label="设计" value="设计" /></el-select></el-form-item>
        <el-form-item label="实验要求"><el-select v-model="form.experimentRequirement"><el-option label="必做" value="必做" /><el-option label="选做" value="选做" /></el-select></el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button type="primary" @click="save">保存</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { onMounted, reactive, ref } from 'vue'
import { ElMessage } from 'element-plus'
import { experimentApi, type ExperimentItemDto } from '@/api/experiment'

const loading = ref(false)
const list = ref<ExperimentItemDto[]>([])
const dialogVisible = ref(false)

const emptyForm = () => ({
  id: '',
  courseCode: '',
  experimentName: '',
  experimentHours: 0,
  experimentType: '',
  experimentRequirement: '必做',
  status: 'Active',
  sortOrder: 0,
  description: ''
})
const form = reactive(emptyForm())

const loadList = async () => {
  loading.value = true
  try {
    const res = await experimentApi.getItems()
    list.value = res.data.data || []
  } finally {
    loading.value = false
  }
}

const openDialog = (row?: ExperimentItemDto) => {
  Object.assign(form, emptyForm(), row || {})
  dialogVisible.value = true
}

const save = async () => {
  const payload: any = { ...form }
  if (payload.id) await experimentApi.updateItem(payload.id, payload)
  else {
    delete payload.id
    await experimentApi.createItem(payload)
  }
  ElMessage.success('保存成功')
  dialogVisible.value = false
  loadList()
}

const remove = async (id: string) => {
  await experimentApi.deleteItem(id)
  ElMessage.success('删除成功')
  loadList()
}

onMounted(loadList)
</script>

<style scoped>
.page-container { padding: 20px; }
.page-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 16px; }
.page-header h2 { margin: 0; font-size: 20px; font-weight: 500; }
</style>

