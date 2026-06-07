<template>
  <el-dialog title="Excel批量导入设备" v-model="visible" width="700px" destroy-on-close>
    <div v-if="!uploaded">
      <el-upload
        class="upload-area"
        drag
        :auto-upload="false"
        :limit="1"
        accept=".xlsx,.xls"
        :on-change="handleFileChange"
        ref="uploadRef"
      >
        <el-icon class="el-icon--upload"><UploadFilled /></el-icon>
        <div class="el-upload__text">将Excel文件拖到此处，或<em>点击上传</em></div>
        <template #tip>
          <div class="el-upload__tip">
            支持 .xlsx 和 .xls 格式。<br />
            请按以下列顺序准备数据：<br />
            资产编号, 设备名称, 型号, 品牌, 序列号, 类别, 单位, 价格, 购入日期, 供应商, 存放位置, 所属实验室名称
          </div>
        </template>
      </el-upload>
      <div class="template-download">
        <el-button type="text" @click="downloadTemplate">
          <el-icon><Download /></el-icon> 下载导入模板
        </el-button>
      </div>
    </div>

    <div v-else>
      <el-alert type="success" :closable="false" style="margin-bottom: 16px">
        文件已选择: {{ fileName }}
      </el-alert>
      <el-descriptions :column="2" border size="small">
        <el-descriptions-item label="成功条数"><el-tag type="success">{{ result?.success || 0 }}</el-tag></el-descriptions-item>
        <el-descriptions-item label="失败条数"><el-tag type="danger">{{ result?.failed || 0 }}</el-tag></el-descriptions-item>
      </el-descriptions>
      <el-table v-if="result?.errors?.length" :data="result.errors.map((e: string, i: number) => ({ error: e }))" max-height="250" style="margin-top: 12px">
        <el-table-column prop="error" label="错误信息" />
      </el-table>
      <div v-if="!result?.errors?.length && result?.success > 0" type="success" style="margin-top: 16px; color: #67c23a;">
        全部导入成功！
      </div>
    </div>

    <template #footer>
      <template v-if="!uploaded">
        <el-button @click="visible = false">取消</el-button>
        <el-button type="primary" :loading="importing" @click="handleImport">开始导入</el-button>
      </template>
      <template v-else>
        <el-button @click="reset">继续导入</el-button>
        <el-button type="primary" @click="visible = false; emit('success')">完成</el-button>
      </template>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { ElMessage } from 'element-plus'
import { UploadFilled, Download } from '@element-plus/icons-vue'
import { equipmentApi } from '@/api/lab'

interface Props {
  modelValue: boolean
}

const props = defineProps<Props>()
const emit = defineEmits(['update:modelValue', 'success'])

const visible = computed({
  get: () => props.modelValue,
  set: (val) => emit('update:modelValue', val)
})

const uploadRef = ref()
const fileObj = ref<File | null>(null)
const fileName = ref('')
const importing = ref(false)
const uploaded = ref(false)
const result = ref<{ success: number; failed: number; errors: string[]; duplicateCodes: string[] } | null>(null)

const handleFileChange = (file: any) => {
  fileObj.value = file.raw
  fileName.value = file.name
}

const handleImport = async () => {
  if (!fileObj.value) {
    ElMessage.warning('请先选择文件')
    return
  }
  importing.value = true
  try {
    const res = await equipmentApi.importExcel(fileObj.value)
    const payload = res.data
    if (payload.code === 200) {
      result.value = payload.data
      uploaded.value = true
      if (payload.data.failed > 0) {
        ElMessage.warning(`导入完成：成功 ${payload.data.success} 条，失败 ${payload.data.failed} 条`)
      } else {
        ElMessage.success('全部导入成功')
      }
    } else {
      ElMessage.error(payload.message)
    }
  } catch (err: any) {
    ElMessage.error(err?.response?.data?.message || '导入失败')
  } finally {
    importing.value = false
  }
}

const reset = () => {
  uploaded.value = false
  result.value = null
  fileObj.value = null
  fileName.value = ''
  uploadRef.value?.clearFiles()
}

const downloadTemplate = async () => {
  try {
    const res = await equipmentApi.downloadTemplate()
    const blob = new Blob([res.data], {
      type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
    })
    const url = URL.createObjectURL(blob)
    const a = document.createElement('a')
    a.href = url
    a.download = '设备导入模板.xlsx'
    a.click()
    URL.revokeObjectURL(url)
  } catch {
    ElMessage.error('模板下载失败')
  }
}
</script>

<style scoped>
.upload-area {
  text-align: center;
  padding: 20px 0;
}
.template-download {
  text-align: center;
  margin-top: 12px;
}
</style>
