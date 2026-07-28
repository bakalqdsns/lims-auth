<template>
  <div class="tab-content">
    <!-- 搜索栏 -->
    <el-card shadow="never" class="search-card">
      <el-form :model="queryForm" inline>
        <el-form-item label="关键词">
          <el-input v-model="queryForm.keyword" placeholder="编号/名称/规格" clearable @keyup.enter="handleSearch" />
        </el-form-item>
        <el-form-item label="分类">
          <el-select v-model="queryForm.categoryId" placeholder="全部" clearable style="width: 150px">
            <el-option v-for="cat in categories" :key="cat.id" :label="cat.name" :value="cat.id" />
          </el-select>
        </el-form-item>
        <el-form-item label="供应商">
          <el-input v-model="queryForm.supplier" placeholder="供应商" clearable />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">
            <el-icon><Search /></el-icon> 搜索
          </el-button>
          <el-button @click="handleReset">重置</el-button>
        </el-form-item>
      </el-form>
    </el-card>

    <!-- 操作栏 -->
    <div class="toolbar" v-if="hasPermission('consumable:create')">
      <el-button type="primary" @click="handleCreate">
        <el-icon><Plus /></el-icon> 新增耗材
      </el-button>
      <el-button @click="handleBatchImport">
        <el-icon><Upload /></el-icon> 批量导入
      </el-button>
      <el-button @click="handleLowStockFilter">
        <el-icon><Warning /></el-icon> 库存预警
      </el-button>
    </div>

    <!-- 列表 -->
    <el-card shadow="never">
      <el-table v-loading="loading" :data="list" stripe>
        <el-table-column prop="code" label="耗材编号" width="130" />
        <el-table-column prop="name" label="耗材名称" min-width="160" />
        <el-table-column prop="categoryName" label="分类" width="100">
          <template #default="{ row }">
            {{ row.categoryName || '-' }}
          </template>
        </el-table-column>
        <el-table-column prop="specification" label="规格型号" width="120">
          <template #default="{ row }">
            {{ row.specification || '-' }}
          </template>
        </el-table-column>
        <el-table-column prop="unit" label="单位" width="60" />
        <el-table-column label="当前库存" width="100" align="right">
          <template #default="{ row }">
            <span :class="{ 'low-stock': row.isLowStock }">
              {{ row.currentStock }}
            </span>
          </template>
        </el-table-column>
        <el-table-column label="可用库存" width="100" align="right">
          <template #default="{ row }">
            {{ row.availableStock }}
          </template>
        </el-table-column>
        <el-table-column label="最低库存" width="90" align="right">
          <template #default="{ row }">
            {{ row.minStock }}
          </template>
        </el-table-column>
        <el-table-column prop="location" label="存放位置" width="120">
          <template #default="{ row }">
            {{ row.location || '-' }}
          </template>
        </el-table-column>
        <el-table-column prop="supplier" label="供应商" min-width="120">
          <template #default="{ row }">
            {{ row.supplier || '-' }}
          </template>
        </el-table-column>
        <el-table-column label="状态" width="80">
          <template #default="{ row }">
            <el-tag v-if="row.isLowStock" type="danger" size="small">库存不足</el-tag>
            <el-tag v-else-if="row.isActive" type="success" size="small">正常</el-tag>
            <el-tag v-else type="info" size="small">停用</el-tag>
          </template>
        </el-table-column>
        <el-table-column v-if="hasPermission('consumable:update') || hasPermission('consumable:delete')"
          label="操作" width="180" fixed="right">
          <template #default="{ row }">
            <el-button link type="primary" @click="handleEdit(row)" v-permission="'consumable:update'">编辑</el-button>
            <el-popconfirm
              v-if="hasPermission('consumable:delete')"
              title="确定删除该耗材吗？"
              @confirm="handleDelete(row)">
              <template #reference>
                <el-button link type="danger">删除</el-button>
              </template>
            </el-popconfirm>
          </template>
        </el-table-column>
      </el-table>

      <div class="pagination-wrapper">
        <el-pagination
          v-model:current-page="pagination.page"
          v-model:page-size="pagination.pageSize"
          :page-sizes="[10, 20, 50, 100]"
          :total="total"
          layout="total, sizes, prev, pager, next, jumper"
          @size-change="handleSizeChange"
          @current-change="handlePageChange"
        />
      </div>
    </el-card>

    <!-- 表单对话框 -->
    <el-dialog
      v-model="dialogVisible"
      :title="dialogType === 'create' ? '新增耗材' : '编辑耗材'"
      width="650px"
      destroy-on-close>
      <el-form ref="formRef" :model="form" :rules="formRules" label-width="100px">
        <el-row :gutter="16">
          <el-col :span="12">
            <el-form-item label="耗材编号" prop="code">
              <el-input v-model="form.code" placeholder="唯一编号" :disabled="dialogType === 'update'" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="耗材名称" prop="name">
              <el-input v-model="form.name" placeholder="耗材名称" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="16">
          <el-col :span="12">
            <el-form-item label="分类" prop="categoryId">
              <el-select v-model="form.categoryId" placeholder="选择分类" clearable style="width: 100%">
                <el-option v-for="cat in categories" :key="cat.id" :label="cat.name" :value="cat.id" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="规格型号">
              <el-input v-model="form.specification" placeholder="规格型号" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="16">
          <el-col :span="12">
            <el-form-item label="单位">
              <el-input v-model="form.unit" placeholder="如：个、瓶、盒" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="单价(元)">
              <el-input-number v-model="form.unitPrice" :min="0" :precision="2" placeholder="单价" style="width: 100%" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="16">
          <el-col :span="8">
            <el-form-item label="当前库存" prop="currentStock">
              <el-input-number v-model="form.currentStock" :min="0" :precision="3" style="width: 100%" />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="最低库存">
              <el-input-number v-model="form.minStock" :min="0" :precision="3" style="width: 100%" />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="存放位置">
              <el-input v-model="form.location" placeholder="存放位置" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="16">
          <el-col :span="12">
            <el-form-item label="供应商">
              <el-input v-model="form.supplier" placeholder="供应商" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="单次最大量">
              <el-input-number v-model="form.maxSingleRequest" :min="0" :precision="3" style="width: 100%" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-form-item label="描述">
          <el-input v-model="form.description" type="textarea" :rows="2" placeholder="备注描述" />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button type="primary" :loading="saving" @click="handleSave">保存</el-button>
      </template>
    </el-dialog>

    <!-- 批量导入对话框 -->
    <el-dialog v-model="importDialogVisible" title="批量导入耗材" width="500px">
      <el-upload
        ref="uploadRef"
        class="upload-area"
        drag
        :limit="1"
        accept=".xlsx,.xls"
        :auto-upload="false"
        :on-change="handleFileChange"
      >
        <el-icon><UploadFilled /></el-icon>
        <div class="el-upload__text">将 Excel 文件拖到此处，或<em>点击上传</em></div>
        <template #tip>
          <div class="el-upload__tip">支持 .xlsx 和 .xls 格式</div>
        </template>
      </el-upload>
      <div class="import-actions">
        <el-button type="primary" :loading="importing" :disabled="!importFile" @click="handleImport">
          开始导入
        </el-button>
        <el-button @click="handleDownloadTemplate">下载模板</el-button>
      </div>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, type FormInstance, type FormRules } from 'element-plus'
import { Search, Plus, Warning, Upload, UploadFilled } from '@element-plus/icons-vue'
import { consumableApi, type ConsumableDto, type CreateConsumableRequest, type UpdateConsumableRequest, type CategoryDto } from '../../../api/consumable'
import { useAuthStore } from '../../../stores/auth'

const emit = defineEmits<{ (e: 'navigate', tab: string): void; (e: 'refresh'): void }>()
const authStore = useAuthStore()
const hasPermission = authStore.hasPermission

const loading = ref(false)
const saving = ref(false)
const importing = ref(false)
const dialogVisible = ref(false)
const importDialogVisible = ref(false)
const importFile = ref<File | null>(null)
const uploadRef = ref()
const dialogType = ref<'create' | 'update'>('create')
const formRef = ref<FormInstance>()

const list = ref<ConsumableDto[]>([])
const categories = ref<CategoryDto[]>([])
const total = ref(0)

const pagination = reactive({ page: 1, pageSize: 20 })

const queryForm = reactive({
  keyword: '',
  categoryId: '',
  supplier: '',
  isLowStock: false
})

const form = reactive<Record<string, any>>({
  code: '',
  name: '',
  categoryId: '',
  specification: '',
  unit: '个',
  currentStock: 0,
  minStock: 0,
  location: '',
  supplier: '',
  unitPrice: undefined,
  maxSingleRequest: 999999,
  description: ''
})

const currentId = ref('')

const formRules: FormRules = {
  code: [{ required: true, message: '请输入耗材编号', trigger: 'blur' }],
  name: [{ required: true, message: '请输入耗材名称', trigger: 'blur' }],
  currentStock: [{ required: true, message: '请输入当前库存', trigger: 'blur' }]
}

const loadCategories = async () => {
  try {
    const res = await consumableApi.getCategories()
    if (res.data.code === 200) {
      categories.value = res.data.data
    }
  } catch {}
}

const loadData = async () => {
  loading.value = true
  try {
    const params: any = {
      page: pagination.page,
      pageSize: pagination.pageSize
    }
    if (queryForm.keyword) params.keyword = queryForm.keyword
    if (queryForm.categoryId) params.categoryId = queryForm.categoryId
    if (queryForm.supplier) params.supplier = queryForm.supplier
    if (queryForm.isLowStock) params.isLowStock = true

    const res = await consumableApi.getConsumables(params)
    if (res.data.code === 200) {
      list.value = res.data.data
      total.value = res.data.total
    }
  } catch {
    ElMessage.error('加载失败')
  } finally {
    loading.value = false
  }
}

const handleSearch = () => {
  pagination.page = 1
  loadData()
}

const handleReset = () => {
  queryForm.keyword = ''
  queryForm.categoryId = ''
  queryForm.supplier = ''
  queryForm.isLowStock = false
  pagination.page = 1
  loadData()
}

const handleLowStockFilter = () => {
  queryForm.isLowStock = true
  pagination.page = 1
  loadData()
}

const handleSizeChange = () => {
  pagination.page = 1
  loadData()
}

const handlePageChange = () => {
  loadData()
}

const handleCreate = () => {
  dialogType.value = 'create'
  Object.assign(form, {
    code: '', name: '', categoryId: '', specification: '',
    unit: '个', currentStock: 0, minStock: 0,
    location: '', supplier: '', unitPrice: undefined,
    maxSingleRequest: 999999, description: ''
  })
  dialogVisible.value = true
}

const handleEdit = (row: ConsumableDto) => {
  dialogType.value = 'update'
  currentId.value = row.id
  Object.assign(form, {
    code: row.code, name: row.name, categoryId: row.categoryId || '',
    specification: row.specification || '', unit: row.unit,
    currentStock: row.currentStock, minStock: row.minStock,
    location: row.location || '', supplier: row.supplier || '',
    unitPrice: row.unitPrice, maxSingleRequest: row.maxSingleRequest,
    description: row.description || ''
  })
  dialogVisible.value = true
}

const handleSave = async () => {
  const valid = await formRef.value?.validate().catch(() => false)
  if (!valid) return

  saving.value = true
  try {
    if (dialogType.value === 'create') {
      const data: CreateConsumableRequest = {
        code: form.code,
        name: form.name,
        categoryId: form.categoryId || undefined,
        specification: form.specification || undefined,
        unit: form.unit,
        currentStock: form.currentStock,
        minStock: form.minStock,
        location: form.location || undefined,
        supplier: form.supplier || undefined,
        unitPrice: form.unitPrice,
        maxSingleRequest: form.maxSingleRequest,
        description: form.description || undefined
      }
      const res = await consumableApi.createConsumable(data)
      if (res.data.code === 200) {
        ElMessage.success('创建成功')
        dialogVisible.value = false
        loadData()
        emit('refresh')
      } else {
        ElMessage.error(res.data.message)
      }
    } else {
      const data: UpdateConsumableRequest = {
        name: form.name,
        categoryId: form.categoryId || undefined,
        specification: form.specification || undefined,
        unit: form.unit,
        currentStock: form.currentStock,
        minStock: form.minStock,
        location: form.location || undefined,
        supplier: form.supplier || undefined,
        unitPrice: form.unitPrice,
        maxSingleRequest: form.maxSingleRequest,
        description: form.description || undefined
      }
      const res = await consumableApi.updateConsumable(currentId.value, data)
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

const handleDelete = async (row: ConsumableDto) => {
  try {
    const res = await consumableApi.deleteConsumable(row.id)
    if (res.data.code === 200) {
      ElMessage.success('删除成功')
      loadData()
      emit('refresh')
    } else {
      ElMessage.error(res.data.message)
    }
  } catch {
    ElMessage.error('删除失败')
  }
}

const handleBatchImport = () => {
  importFile.value = null
  importDialogVisible.value = true
}

const handleFileChange = (file: any) => {
  importFile.value = file.raw
}

const handleImport = async () => {
  if (!importFile.value) {
    ElMessage.warning('请先选择文件')
    return
  }
  importing.value = true
  try {
    const formData = new FormData()
    formData.append('file', importFile.value)
    const res = await consumableApi.importConsumables(formData)
    if (res.data.code === 200) {
      ElMessage.success(res.data.message || '导入成功')
      importDialogVisible.value = false
      loadData()
      emit('refresh')
    } else {
      ElMessage.error(res.data.message || '导入失败')
    }
  } catch (err: any) {
    ElMessage.error(err?.response?.data?.message || '导入失败')
  } finally {
    importing.value = false
  }
}

const handleDownloadTemplate = () => {
  consumableApi.downloadConsumableTemplate().then(res => {
    const blob = new Blob([res.data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' })
    const url = URL.createObjectURL(blob)
    const a = document.createElement('a')
    a.href = url
    a.download = '耗材导入模板.xlsx'
    a.click()
    URL.revokeObjectURL(url)
  }).catch(() => ElMessage.error('模板下载失败'))
}

onMounted(() => {
  loadCategories()
  loadData()
})
</script>

<style scoped>
.tab-content {
  padding-top: 12px;
}

.search-card {
  margin-bottom: 12px;
}

.toolbar {
  margin-bottom: 12px;
}

.low-stock {
  color: #f56c6c;
  font-weight: 600;
}

.pagination-wrapper {
  margin-top: 16px;
  display: flex;
  justify-content: flex-end;
}

.upload-area {
  margin-bottom: 16px;
}

.import-actions {
  display: flex;
  gap: 8px;
}
</style>
