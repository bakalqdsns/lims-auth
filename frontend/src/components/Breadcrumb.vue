<template>
  <el-breadcrumb separator="/">
    <el-breadcrumb-item v-for="(item, index) in breadcrumbList" :key="item.path">
      <router-link
        v-if="index < breadcrumbList.length - 1"
        :to="item.path"
        class="breadcrumb-link"
      >
        {{ item.title }}
      </router-link>
      <span v-else>{{ item.title }}</span>
    </el-breadcrumb-item>
  </el-breadcrumb>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useRoute } from 'vue-router'

const route = useRoute()

const routeNameMap: Record<string, string> = {
  home: '首页',
  users: '用户管理',
  roles: '角色管理',
  departments: '部门管理',
  semesters: '学期管理',
  courses: '课程管理',
  majors: '专业管理',
  classes: '班级管理',
  tasks: '教学任务',
  periods: '节次时间',
  experimentTasks: '实验教学任务',
  experimentItems: '实验项目库',
  experimentSchedules: '项目开出',
  experimentQuality: '教学质量',
  experimentPlans: '实训计划',
  campuses: '校区管理',
  buildings: '楼宇管理',
  floorPlan: '建筑平面图',
  labs: '实验室管理',
  equipments: '设备管理',
  scheduleSearch: '排课查询',
  centralScheduling: '集中排课',
  reservations: '预约申请',
  reservationApproval: '预约审批',
  teachingApplications: '教学申请',
  usageRegistration: '使用登记',
  scheduleStatistics: '统计分析',
  scheduleDashboard: '排课看板'
}

const breadcrumbList = computed(() => {
  const matched = route.matched.filter(r => r.path !== '/' && r.name)
  return matched.map(r => ({
    path: r.path,
    title: routeNameMap[String(r.name)] || String(r.name)
  }))
})
</script>

<style scoped>
.breadcrumb-link {
  color: #606266;
  text-decoration: none;
  transition: color 0.2s;
}

.breadcrumb-link:hover {
  color: #409EFF;
}
</style>

