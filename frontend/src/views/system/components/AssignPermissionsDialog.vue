<template>
  <el-dialog
    :title="`分配权限 - ${roleName}`"
    v-model="visible"
    width="700px"
    destroy-on-close
  >
    <el-alert
      title="勾选权限后点击确定保存"
      type="info"
      :closable="false"
      style="margin-bottom: 20px"
    />

    <div v-loading="loading" class="permissions-container">
      <el-collapse v-model="activeModules">
        <el-collapse-item
          v-for="module in permissionModules"
          :key="module.module"
          :title="`${module.moduleName} (${getSelectedCount(module)} / ${module.permissions.length})`"
          :name="module.module"
        >
          <el-checkbox-group v-model="selectedPermissions">
            <el-checkbox
              v-for="perm in module.permissions"
              :key="perm.id"
              :label="perm.id"
              class="permission-checkbox"
            >
              {{ perm.name }}
              <el-tag size="small" type="info" style="margin-left: 5px">
                {{ perm.code }}
              </el-tag>
            </el-checkbox>
          </el-checkbox-group>
        </el-collapse-item>
      </el-collapse>
    </div>

    <template #footer>
      <el-button @click="visible = false">取消</el-button>
      <el-button type="primary" :loading="submitting" @click="handleSubmit">
        确定
      </el-button>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { ElMessage } from 'element-plus'
import { roleApi, permissionApi, type PermissionModuleDto } from '../../../api/system'

interface Props {
  modelValue: boolean
  roleId?: string
  roleName?: string
}

const props = defineProps<Props>()
const emit = defineEmits(['update:modelValue', 'success'])

const visible = computed({
  get: () => props.modelValue,
  set: (val) => emit('update:modelValue', val)
})

const loading = ref(false)
const submitting = ref(false)
const permissionModules = ref<PermissionModuleDto[]>([])
const selectedPermissions = ref<string[]>([])
const activeModules = ref<string[]>([])

// 获取权限列表
const fetchPermissions = async () => {
  loading.value = true
  try {
    const [modulesRes, roleRes] = await Promise.all([
      permissionApi.getPermissionsByModule(),
      props.roleId ? roleApi.getRole(props.roleId) : Promise.resolve(null)
    ])

    if (modulesRes.data.code === 200) {
      permissionModules.value = modulesRes.data.data
      activeModules.value = modulesRes.data.data.map(m => m.module)
    }

    if (roleRes?.data.code === 200) {
      selectedPermissions.value = roleRes.data.data.permissions.map((p: any) => p.id)
    }
  } catch (error) {
    ElMessage.error('获取权限列表失败')
  } finally {
    loading.value = false
  }
}

// 获取模块已选数量
const getSelectedCount = (module: PermissionModuleDto) => {
  return module.permissions.filter(p => selectedPermissions.value.includes(p.id)).length
}

// 提交
const handleSubmit = async () => {
  if (!props.roleId) return

  submitting.value = true
  try {
    const res = await roleApi.updateRolePermissions(props.roleId, selectedPermissions.value)
    if (res.data.code === 200) {
      ElMessage.success('权限分配成功')
      visible.value = false
      emit('success')
    } else {
      ElMessage.error(res.data.message)
    }
  } catch (error) {
    ElMessage.error('权限分配失败')
  } finally {
    submitting.value = false
  }
}

watch(() => props.modelValue, (val) => {
  if (val && props.roleId) {
    fetchPermissions()
  }
})
</script>

<style scoped>
.permissions-container {
  max-height: 500px;
  overflow-y: auto;
}

.permission-checkbox {
  display: block;
  margin: 8px 0;
}

.permission-checkbox :deep(.el-checkbox__label) {
  display: flex;
  align-items: center;
}
</style>
