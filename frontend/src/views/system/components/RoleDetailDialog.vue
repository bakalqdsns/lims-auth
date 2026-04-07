<template>
  <el-dialog
    title="角色详情"
    v-model="visible"
    width="600px"
    destroy-on-close
  >
    <el-descriptions :column="2" border>
      <el-descriptions-item label="角色编码">{{ role?.code }}</el-descriptions-item>
      <el-descriptions-item label="角色名称">{{ role?.name }}</el-descriptions-item>
      <el-descriptions-item label="类型">
        <el-tag v-if="role?.isSystem" type="danger">系统内置</el-tag>
        <el-tag v-else type="info">自定义</el-tag>
      </el-descriptions-item>
      <el-descriptions-item label="状态">
        <el-tag v-if="role?.isActive" type="success">启用</el-tag>
        <el-tag v-else type="danger">禁用</el-tag>
      </el-descriptions-item>
      <el-descriptions-item label="用户数">{{ role?.userCount }}</el-descriptions-item>
      <el-descriptions-item label="创建时间">{{ formatDate(role?.createdAt) }}</el-descriptions-item>
      <el-descriptions-item label="描述" :span="2">{{ role?.description || '-' }}</el-descriptions-item>
    </el-descriptions>

    <h4 style="margin: 20px 0 10px">权限列表</h4>
    <div v-loading="loading" class="permissions-list">
      <div v-for="module in groupedPermissions" :key="module.module" class="permission-module">
        <h5>{{ module.moduleName }}</h5>
        <div class="permission-tags">
          <el-tag
            v-for="perm in module.permissions"
            :key="perm.id"
            size="small"
            class="permission-tag"
          >
            {{ perm.name }}
          </el-tag>
        </div>
      </div>
      <el-empty v-if="!groupedPermissions.length" description="暂无权限" />
    </div>

    <template #footer>
      <el-button @click="visible = false">关闭</el-button>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { roleApi, type RoleDetailDto, type PermissionBrief } from '../../../api/system'

interface Props {
  modelValue: boolean
  roleId?: string
}

const props = defineProps<Props>()
const emit = defineEmits(['update:modelValue'])

const visible = computed({
  get: () => props.modelValue,
  set: (val) => emit('update:modelValue', val)
})

const loading = ref(false)
const role = ref<RoleDetailDto | null>(null)

const groupedPermissions = computed(() => {
  if (!role.value?.permissions) return []

  const groups: Record<string, { module: string; moduleName: string; permissions: PermissionBrief[] }> = {}

  const moduleNames: Record<string, string> = {
    user: '用户管理',
    role: '角色管理',
    permission: '权限管理',
    department: '部门管理',
    equipment: '设备管理',
    lab: '实验室管理',
    course: '课程管理',
    report: '报告管理',
    system: '系统管理'
  }

  role.value.permissions.forEach(p => {
    const moduleCode = p.code.split(':')[0]
    if (!groups[moduleCode]) {
      groups[moduleCode] = {
        module: moduleCode,
        moduleName: moduleNames[moduleCode] || moduleCode,
        permissions: []
      }
    }
    groups[moduleCode].permissions.push(p)
  })

  return Object.values(groups)
})

const fetchRoleDetail = async () => {
  if (!props.roleId) return

  loading.value = true
  try {
    const res = await roleApi.getRole(props.roleId)
    if (res.data.code === 200) {
      role.value = res.data.data
    }
  } catch (error) {
    console.error('获取角色详情失败', error)
  } finally {
    loading.value = false
  }
}

const formatDate = (date?: string) => {
  if (!date) return '-'
  return new Date(date).toLocaleString('zh-CN')
}

watch(() => props.modelValue, (val) => {
  if (val && props.roleId) {
    fetchRoleDetail()
  }
})
</script>

<style scoped>
.permissions-list {
  max-height: 400px;
  overflow-y: auto;
}

.permission-module {
  margin-bottom: 15px;
}

.permission-module h5 {
  margin: 0 0 8px;
  color: #606266;
  font-size: 14px;
}

.permission-tags {
  display: flex;
  flex-wrap: wrap;
  gap: 8px;
}

.permission-tag {
  margin: 0;
}
</style>
