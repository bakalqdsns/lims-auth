<template>
  <div class="tab-content">
    <div class="toolbar">
      <el-button type="primary" @click="handleCreate">
        <el-icon><Plus /></el-icon> 新增分类
      </el-button>
    </div>

    <el-card shadow="never">
      <el-table v-loading="loading" :data="list" stripe>
        <el-table-column prop="name" label="分类名称" min-width="200" />
        <el-table-column prop="remark" label="备注" min-width="300" />
        <el-table-column prop="isActive" label="状态" width="90">
          <template #default="{ row }">
            <el-tag :type="row.isActive ? 'success' : 'info'" size="small">
              {{ row.isActive ? '启用' : '停用' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="createdAt" label="创建时间" width="160">
          <template #default="{ row }">{{ formatDate(row.createdAt) }}</template>
        </el-table-column>
        <el-table-column label="操作" width="160" fixed="right">
          <template #default="{ row }">
            <el-button link type="primary" @click="handleEdit(row)">编辑</el-button>
            <el-popconfirm title="确定删除该分类吗？" @confirm="handleDelete(row)">
              <template #reference>
                <el-button link type="danger">删除</el-button>
              </template>
            </el-popconfirm>
          </template>
        </el-table-column>
      </el-table>
    </el-card>

    <el-dialog v-model="dialogVisible" :title="dialogType === 'create' ? '新增分类' : '编辑分类'" width="450px">
      <el-form ref="formRef" :model="form" :rules="formRules" label-width="90px">
        <el-form-item label="分类名称" prop="name">
          <el-input v-model="form.name" placeholder="请输入分类名称" />
        </el-form-item>
        <el-form-item label="备注">
          <el-input v-model="form.remark" type="textarea" :rows="3" placeholder="备注信息" />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button type="primary" :loading="saving" @click="handleSave">保存</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, type FormInstance, type FormRules } from 'element-plus'
import { Plus } from '@element-plus/icons-vue'
import { consumableApi, type CategoryDto } from '../../../api/consumable'

const emit = defineEmits<{ (e: 'refresh'): void }>()
const loading = ref(false)
const saving = ref(false)
const dialogVisible = ref(false)
const dialogType = ref<'create' | 'update'>('create')
const list = ref<CategoryDto[]>([])
const formRef = ref<FormInstance>()
const currentId = ref('')

const form = reactive({ name: '', remark: '' })
const formRules: FormRules = {
  name: [{ required: true, message: '请输入分类名称', trigger: 'blur' }]
}

const formatDate = (d: string) => d ? new Date(d).toLocaleString('zh-CN', { hour12: false }) : '-'

const loadData = async () => {
  loading.value = true
  try {
    const res = await consumableApi.getCategories()
    if (res.data.code === 200) list.value = res.data.data
  } catch {
    ElMessage.error('加载失败')
  } finally {
    loading.value = false
  }
}

const handleCreate = () => {
  dialogType.value = 'create'
  Object.assign(form, { name: '', remark: '' })
  dialogVisible.value = true
}

const handleEdit = (row: CategoryDto) => {
  dialogType.value = 'update'
  currentId.value = row.id
  Object.assign(form, { name: row.name, remark: row.remark || '' })
  dialogVisible.value = true
}

const handleSave = async () => {
  const valid = await formRef.value?.validate().catch(() => false)
  if (!valid) return

  saving.value = true
  try {
    if (dialogType.value === 'create') {
      const res = await consumableApi.createCategory(form)
      if (res.data.code === 200) {
        ElMessage.success('创建成功')
        dialogVisible.value = false
        loadData()
        emit('refresh')
      } else {
        ElMessage.error(res.data.message)
      }
    } else {
      const res = await consumableApi.updateCategory(currentId.value, form)
      if (res.data.code === 200) {
        ElMessage.success('更新成功')
        dialogVisible.value = false
        loadData()
        emit('refresh')
      } else {
        ElMessage.error(res.data.message)
      }
    }
  } catch (err: any) {
    ElMessage.error(err?.response?.data?.message || '保存失败')
  } finally {
    saving.value = false
  }
}

const handleDelete = async (row: CategoryDto) => {
  try {
    const res = await consumableApi.deleteCategory(row.id)
    if (res.data.code === 200) {
      ElMessage.success('删除成功')
      loadData()
      emit('refresh')
    } else {
      ElMessage.error(res.data.message)
    }
  } catch (err: any) {
    ElMessage.error(err?.response?.data?.message || '删除失败')
  }
}

onMounted(() => { loadData() })
</script>

<style scoped>
.tab-content { padding-top: 12px; }
.toolbar { margin-bottom: 12px; }
</style>
