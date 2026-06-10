import { createRouter, createWebHistory } from 'vue-router'
import LoginView from '../views/LoginView.vue'
import HomeView from '../views/HomeView.vue'
import { useAuthStore } from '../stores/auth'

type Role = 'super_admin' | 'admin' | 'lab_admin' | 'teacher' | 'student'

const router = createRouter({
  history: createWebHistory(),
  routes: [
    {
      path: '/',
      name: 'login',
      component: LoginView,
      meta: { public: true }
    },
    {
      path: '/home',
      name: 'home',
      component: HomeView,
      meta: { requiresAuth: true, roles: ['super_admin', 'admin', 'lab_admin', 'teacher', 'student'] }
    },

    // ─── 个人资料 ─── 所有登录用户可见
    {
      path: '/profile',
      name: 'profile',
      component: () => import('../views/profile/ProfileView.vue'),
      meta: { requiresAuth: true, roles: ['super_admin', 'admin', 'lab_admin', 'teacher', 'student'] }
    },

    // ─── 系统管理 ─── 仅管理员可见
    {
      path: '/system',
      name: 'system',
      component: HomeView,
      meta: { requiresAuth: true, roles: ['super_admin', 'admin', 'lab_admin'] },
      children: [
        {
          path: 'users',
          name: 'users',
          component: () => import('../views/system/UsersView.vue'),
          meta: { requiresAuth: true, roles: ['super_admin', 'admin', 'lab_admin'], permission: 'user:read' }
        },
        {
          path: 'roles',
          name: 'roles',
          component: () => import('../views/system/RolesView.vue'),
          meta: { requiresAuth: true, roles: ['super_admin', 'admin', 'lab_admin'], permission: 'role:read' }
        },
        {
          path: 'departments',
          name: 'departments',
          component: () => import('../views/system/DepartmentsView.vue'),
          meta: { requiresAuth: true, roles: ['super_admin', 'admin', 'lab_admin'], permission: 'department:read' }
        }
      ]
    },

    // ─── 教学管理 ─── 管理员全部可见，教师可见课程/班级/任务，学生不可见
    {
      path: '/teaching',
      name: 'teaching',
      component: HomeView,
      meta: { requiresAuth: true, roles: ['super_admin', 'admin', 'lab_admin', 'teacher'] },
      children: [
        {
          path: 'semesters',
          name: 'semesters',
          component: () => import('../views/teaching/SemestersView.vue'),
          meta: { requiresAuth: true, roles: ['super_admin', 'admin', 'lab_admin'], permission: 'course:schedule' }
        },
        {
          path: 'courses',
          name: 'courses',
          component: () => import('../views/teaching/CoursesView.vue'),
          meta: { requiresAuth: true, roles: ['super_admin', 'admin', 'lab_admin', 'teacher'], permission: 'course:read' }
        },
        {
          path: 'majors',
          name: 'majors',
          component: () => import('../views/teaching/MajorsView.vue'),
          meta: { requiresAuth: true, roles: ['super_admin', 'admin', 'lab_admin'] }
        },
        {
          path: 'classes',
          name: 'classes',
          component: () => import('../views/teaching/ClassesView.vue'),
          meta: { requiresAuth: true, roles: ['super_admin', 'admin', 'lab_admin', 'teacher'] }
        },
        {
          path: 'tasks',
          name: 'tasks',
          component: () => import('../views/teaching/TeachingTasksView.vue'),
          meta: { requiresAuth: true, roles: ['super_admin', 'admin', 'lab_admin', 'teacher'], permission: 'course:schedule' }
        },
        {
          path: 'periods',
          name: 'periods',
          component: () => import('../views/teaching/PeriodTimesView.vue'),
          meta: { requiresAuth: true, roles: ['super_admin', 'admin', 'lab_admin'] }
        }
      ]
    },

    // ─── 实验教学管理 ─── 管理员全部可见，教师可见任务/项目库/开出/实训，学生不可见
    {
      path: '/experiment',
      name: 'experiment',
      component: HomeView,
      meta: { requiresAuth: true, roles: ['super_admin', 'admin', 'lab_admin', 'teacher'] },
      children: [
        {
          path: 'tasks',
          name: 'experimentTasks',
          component: () => import('../views/experiment/ExperimentTasksView.vue'),
          meta: { requiresAuth: true, roles: ['super_admin', 'admin', 'lab_admin', 'teacher'] }
        },
        {
          path: 'items',
          name: 'experimentItems',
          component: () => import('../views/experiment/ExperimentItemsView.vue'),
          meta: { requiresAuth: true, roles: ['super_admin', 'admin', 'lab_admin', 'teacher'] }
        },
        {
          path: 'schedules',
          name: 'experimentSchedules',
          component: () => import('../views/experiment/ExperimentSchedulesView.vue'),
          meta: { requiresAuth: true, roles: ['super_admin', 'admin', 'lab_admin', 'teacher'] }
        },
        {
          path: 'quality',
          name: 'experimentQuality',
          component: () => import('../views/experiment/ExperimentQualityView.vue'),
          meta: { requiresAuth: true, roles: ['super_admin', 'admin', 'lab_admin'] }
        },
        {
          path: 'plans',
          name: 'experimentPlans',
          component: () => import('../views/experiment/TrainingPlansView.vue'),
          meta: { requiresAuth: true, roles: ['super_admin', 'admin', 'lab_admin', 'teacher'] }
        }
      ]
    },

    // ─── 实验室设备管理 ─── 管理员/教师/学生均可访问
    {
      path: '/lab',
      name: 'lab',
      component: HomeView,
      meta: { requiresAuth: true, roles: ['super_admin', 'admin', 'lab_admin', 'teacher', 'student'] },
      children: [
        {
          path: 'equipments',
          name: 'labEquipments',
          component: () => import('../views/lab/EquipmentsView.vue'),
          meta: { requiresAuth: true, roles: ['super_admin', 'admin', 'lab_admin', 'teacher', 'student'], permission: 'equipment:read' }
        },
        {
          path: 'borrow-records',
          name: 'borrowRecords',
          component: () => import('../views/lab/BorrowRecordsView.vue'),
          meta: { requiresAuth: true, roles: ['super_admin', 'admin', 'lab_admin', 'teacher', 'student'], permission: 'equipment:read' }
        }
      ]
    },

    // ─── 场馆信息管理 ─── 管理员全部可写，教师/学生仅浏览
    {
      path: '/venue',
      name: 'venue',
      component: HomeView,
      meta: { requiresAuth: true, roles: ['super_admin', 'admin', 'lab_admin', 'teacher', 'student'] },
      children: [
        {
          path: 'campuses',
          name: 'campuses',
          component: () => import('../views/venue/CampusesView.vue'),
          meta: { requiresAuth: true, roles: ['super_admin', 'admin', 'lab_admin', 'teacher', 'student'], permission: 'campus:read' }
        },
        {
          path: 'buildings',
          name: 'buildings',
          component: () => import('../views/venue/BuildingsView.vue'),
          meta: { requiresAuth: true, roles: ['super_admin', 'admin', 'lab_admin', 'teacher', 'student'], permission: 'building:read' }
        },
        {
          path: 'floor-plan',
          name: 'floorPlan',
          component: () => import('../views/venue/BuildingFloorPlanView.vue'),
          meta: { requiresAuth: true, roles: ['super_admin', 'admin', 'lab_admin', 'teacher', 'student'], permission: 'building:read' }
        },
        {
          path: 'labs',
          name: 'labs',
          component: () => import('../views/venue/LabsView.vue'),
          meta: { requiresAuth: true, roles: ['super_admin', 'admin', 'lab_admin', 'teacher', 'student'], permission: 'lab:read' }
        }
      ]
    },

    // ─── 预约排课管理 ─── 集中排课/统计分析仅管理员；查询/预约/审批/使用登记按角色分层
    {
      path: '/scheduling',
      name: 'scheduling',
      redirect: '/scheduling/list',
      component: HomeView,
      meta: { requiresAuth: true, roles: ['super_admin', 'admin', 'lab_admin', 'teacher', 'student'] },
      children: [
        {
          path: 'list',
          name: 'scheduleSearch',
          component: () => import('../views/scheduling/ScheduleSearchView.vue'),
          meta: { requiresAuth: true, roles: ['super_admin', 'admin', 'lab_admin', 'teacher', 'student'], permission: 'schedule:read' }
        },
        {
          path: 'central',
          name: 'centralScheduling',
          component: () => import('../views/scheduling/CentralSchedulingView.vue'),
          meta: { requiresAuth: true, roles: ['super_admin', 'admin', 'lab_admin'], permission: 'schedule:create' }
        },
        {
          path: 'reservations',
          name: 'reservations',
          component: () => import('../views/scheduling/ReservationView.vue'),
          meta: { requiresAuth: true, roles: ['super_admin', 'admin', 'lab_admin', 'teacher', 'student'], permission: 'reservation:read' }
        },
        {
          path: 'reservations/approval',
          name: 'reservationApproval',
          component: () => import('../views/scheduling/ReservationApprovalView.vue'),
          meta: { requiresAuth: true, roles: ['super_admin', 'admin', 'lab_admin'], permission: 'reservation:approve' }
        },
        {
          path: 'teaching-applications',
          name: 'teachingApplications',
          component: () => import('../views/scheduling/TeachingApplicationView.vue'),
          meta: { requiresAuth: true, roles: ['super_admin', 'admin', 'lab_admin', 'teacher'], permission: 'teaching_application:read' }
        },
        {
          path: 'usage-registration',
          name: 'usageRegistration',
          component: () => import('../views/scheduling/UsageRegistrationView.vue'),
          meta: { requiresAuth: true, roles: ['super_admin', 'admin', 'lab_admin', 'teacher'], permission: 'usage_registration:read' }
        },
        {
          path: 'statistics',
          name: 'scheduleStatistics',
          component: () => import('../views/scheduling/StatisticsView.vue'),
          meta: { requiresAuth: true, roles: ['super_admin', 'admin', 'lab_admin'], permission: 'statistics:read' }
        },
        {
          path: 'dashboard',
          name: 'scheduleDashboard',
          component: () => import('../views/scheduling/DashboardView.vue'),
          meta: { requiresAuth: true, roles: ['super_admin', 'admin', 'lab_admin'], permission: 'statistics:dashboard' }
        }
      ]
    }
  ]
})

// ─── 路由守卫 ───
router.beforeEach(async (to, _from, next) => {
  const authStore = useAuthStore()

  // 公开页面直接放行
  if (to.meta.public) {
    // 支持通过 ?token=xxx 自动登录（iframe 场景）
    if (to.query.token && !authStore.token) {
      authStore.token = to.query.token as string
      localStorage.setItem('token', to.query.token as string)
      axios.defaults.headers.common['Authorization'] = `Bearer ${to.query.token}`
    }
    // 清理 iframe 嵌入参数，保持 URL 干净
    if (to.query.embed === '1') {
      const cleanQuery = { ...to.query }
      delete cleanQuery.token
      delete cleanQuery.embed
      next({ path: to.path, query: cleanQuery as Record<string, string> })
      return
    }
    next()
    return
  }

  // 需要登录但未登录
  if (to.meta.requiresAuth && !authStore.token) {
    // iframe 场景：尝试从 query.token 注入
    if (to.query.token) {
      authStore.token = to.query.token as string
      localStorage.setItem('token', to.query.token as string)
      axios.defaults.headers.common['Authorization'] = `Bearer ${to.query.token}`
    } else {
      next('/')
      return
    }
  }

  // 已登录用户访问登录页，跳转到首页
  if (to.path === '/' && authStore.token) {
    next('/home')
    return
  }

  // 有 token 但 user 信息尚未加载
  if (to.meta.requiresAuth && authStore.token && !authStore.user) {
    await authStore.fetchCurrentUser()
  }

  // 角色拦截：路由指定了 roles 且用户不属于任何允许的角色
  const allowedRoles = to.meta.roles as Role[] | undefined
  if (allowedRoles && allowedRoles.length > 0) {
    const hasAllowedRole = allowedRoles.some(r => authStore.hasRole(r))
    if (!hasAllowedRole) {
      console.warn(`[权限警告] 页面 "${to.path}" 仅允许角色 [${allowedRoles.join(', ')}]，当前用户角色: [${authStore.userRoles.join(', ')}]`)
      next('/home')
      return
    }
  }

  // 超级管理员跳过权限检查
  if (authStore.isSuperAdmin) {
    next()
    return
  }

  // 学生角色跳过细粒度权限检查（允许访问排课查询和预约申请页面）
  if (authStore.isStudent) {
    next()
    return
  }

  // 细粒度权限拦截
  if (to.meta.permission && !authStore.hasPermission(to.meta.permission as string)) {
    console.warn(`[权限警告] 页面 "${to.path}" 需要权限 "${to.meta.permission}"，当前用户未拥有该权限`)
    next('/home')
    return
  }

  next()
})

export default router
