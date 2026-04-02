<template>
  <el-dialog
    title="分配角色"
    v-model="visible"
    width="500px"
    destroy-on-close
  >
    <el-form label-width="80px">
      <el-form-item label="选择角色">
        <el-checkbox-group v-model="selectedRoles">
          <el-checkbox
            v-for="role in allRoles"
            :key="role.id"
            :label="role.id"
            :disabled="role.code === 'super_admin'"
          >
            {{ role.name }}
            <el-tag
              v-if="role.isSystem"
              size="small"
              type="info"
              style="margin-left: 5px"
            >
              系统
            </el-tag>
          </el-checkbox>
        </el-checkbox-group>
      </el-form-item>
    </el-form>

    <template #footer>
      <el-button @click="visible = false">取消</el-button>
      <el-button type="primary" :loading="submitting" @click="handleSubmit">
        确定
      </el-button>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, computed, watch, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import { roleApi, userApi, type RoleBrief } from '../../../api/system'

interface Props {
  modelValue: boolean
  userId?: string
  userRoles: RoleBrief[]
}

const props = defineProps<Props>()
const emit = defineEmits(['update:modelValue', 'success'])

const visible = computed({
  get: () => props.modelValue,
  set: (val) => emit('update:modelValue', val)
})

const allRoles = ref<RoleBrief[]>([])
const selectedRoles = ref<string[]>([])
const submitting = ref(false)

// 获取所有角色
const fetchRoles = async () => {
  try {
    const res = await roleApi.getAllRoles()
    if (res.data.code === 200) {
      allRoles.value = res.data.data
    }
  } catch (error) {
    ElMessage.error('获取角色列表失败')
  }
}

// 提交
const handleSubmit = async () => {
  if (!props.userId) return

  submitting.value = true
  try {
    const res = await userApi.updateUserRoles(props.userId, selectedRoles.value)
    if (res.data.code === 200) {
      ElMessage.success('角色分配成功')
      visible.value = false
      emit('success')
    } else {
      ElMessage.error(res.data.message)
    }
  } catch (error) {
    ElMessage.error('角色分配失败')
  } finally {
    submitting.value = false
  }
}

watch(() => props.modelValue, (val) => {
  if (val && props.userId) {
    selectedRoles.value = props.userRoles.map(r => r.id)
    fetchRoles()
  }
})

onMounted(() => {
  fetchRoles()
})
</script>

<style scoped>
.el-checkbox-group {
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.el-checkbox {
  margin-right: 0;
}
</style>
