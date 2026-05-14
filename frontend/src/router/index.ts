import { createRouter, createWebHistory } from 'vue-router'
import LoginView from '../views/LoginView.vue'
import HomeView from '../views/HomeView.vue'
import { useAuthStore } from '../stores/auth'

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
      meta: { requiresAuth: true }
    },
    {
      path: '/system',
      name: 'system',
      component: HomeView,
      meta: { requiresAuth: true },
      children: [
        {
          path: 'users',
          name: 'users',
          component: () => import('../views/system/UsersView.vue'),
          meta: { requiresAuth: true, permission: 'user:read' }
        },
        {
          path: 'roles',
          name: 'roles',
          component: () => import('../views/system/RolesView.vue'),
          meta: { requiresAuth: true, permission: 'role:read' }
        },
        {
          path: 'departments',
          name: 'departments',
          component: () => import('../views/system/DepartmentsView.vue'),
          meta: { requiresAuth: true, permission: 'department:read' }
        }
      ]
    },
    {
      path: '/teaching',
      name: 'teaching',
      component: HomeView,
      meta: { requiresAuth: true },
      children: [
        {
          path: 'semesters',
          name: 'semesters',
          component: () => import('../views/teaching/SemestersView.vue'),
          meta: { requiresAuth: true, permission: 'course:schedule' }
        },
        {
          path: 'courses',
          name: 'courses',
          component: () => import('../views/teaching/CoursesView.vue'),
          meta: { requiresAuth: true, permission: 'course:read' }
        },
        {
          path: 'majors',
          name: 'majors',
          component: () => import('../views/teaching/MajorsView.vue'),
          meta: { requiresAuth: true }
        },
        {
          path: 'classes',
          name: 'classes',
          component: () => import('../views/teaching/ClassesView.vue'),
          meta: { requiresAuth: true }
        },
        {
          path: 'tasks',
          name: 'tasks',
          component: () => import('../views/teaching/TeachingTasksView.vue'),
          meta: { requiresAuth: true, permission: 'course:schedule' }
        },
        {
          path: 'periods',
          name: 'periods',
          component: () => import('../views/teaching/PeriodTimesView.vue'),
          meta: { requiresAuth: true }
        }
      ]
    },
    {
      path: '/experiment',
      name: 'experiment',
      component: HomeView,
      meta: { requiresAuth: true },
      children: [
        {
          path: 'tasks',
          name: 'experimentTasks',
          component: () => import('../views/experiment/ExperimentTasksView.vue'),
          meta: { requiresAuth: true }
        },
        {
          path: 'items',
          name: 'experimentItems',
          component: () => import('../views/experiment/ExperimentItemsView.vue'),
          meta: { requiresAuth: true }
        },
        {
          path: 'schedules',
          name: 'experimentSchedules',
          component: () => import('../views/experiment/ExperimentSchedulesView.vue'),
          meta: { requiresAuth: true }
        },
        {
          path: 'quality',
          name: 'experimentQuality',
          component: () => import('../views/experiment/ExperimentQualityView.vue'),
          meta: { requiresAuth: true }
        },
        {
          path: 'plans',
          name: 'experimentPlans',
          component: () => import('../views/experiment/TrainingPlansView.vue'),
          meta: { requiresAuth: true }
        }
      ]
    },
    {
      path: '/venue',
      name: 'venue',
      component: HomeView,
      meta: { requiresAuth: true },
      children: [
        {
          path: 'campuses',
          name: 'campuses',
          component: () => import('../views/venue/CampusesView.vue'),
          meta: { requiresAuth: true, permission: 'campus:read' }
        },
        {
          path: 'buildings',
          name: 'buildings',
          component: () => import('../views/venue/BuildingsView.vue'),
          meta: { requiresAuth: true, permission: 'building:read' }
        },
        {
          path: 'floor-plan',
          name: 'floorPlan',
          component: () => import('../views/venue/BuildingFloorPlanView.vue'),
          meta: { requiresAuth: true, permission: 'building:read' }
        },
        {
          path: 'labs',
          name: 'labs',
          component: () => import('../views/venue/LabsView.vue'),
          meta: { requiresAuth: true, permission: 'lab:read' }
        },
        {
          path: 'equipments',
          name: 'equipments',
          component: () => import('../views/venue/EquipmentsView.vue'),
          meta: { requiresAuth: true, permission: 'equipment:read' }
        }
      ]
    },
    {
      path: '/scheduling',
      name: 'scheduling',
      redirect: '/scheduling/list',
      component: HomeView,
      meta: { requiresAuth: true },
      children: [
        {
          path: 'list',
          name: 'scheduleSearch',
          component: () => import('../views/scheduling/ScheduleSearchView.vue'),
          meta: { requiresAuth: true, permission: 'schedule:read' }
        },
        {
          path: 'central',
          name: 'centralScheduling',
          component: () => import('../views/scheduling/CentralSchedulingView.vue'),
          meta: { requiresAuth: true, permission: 'schedule:create' }
        },
        {
          path: 'reservations',
          name: 'reservations',
          component: () => import('../views/scheduling/ReservationView.vue'),
          meta: { requiresAuth: true, permission: 'reservation:read' }
        },
        {
          path: 'reservations/approval',
          name: 'reservationApproval',
          component: () => import('../views/scheduling/ReservationApprovalView.vue'),
          meta: { requiresAuth: true, permission: 'reservation:approve' }
        },
        {
          path: 'teaching-applications',
          name: 'teachingApplications',
          component: () => import('../views/scheduling/TeachingApplicationView.vue'),
          meta: { requiresAuth: true, permission: 'teaching_application:read' }
        },
        {
          path: 'usage-registration',
          name: 'usageRegistration',
          component: () => import('../views/scheduling/UsageRegistrationView.vue'),
          meta: { requiresAuth: true, permission: 'usage_registration:read' }
        },
        {
          path: 'statistics',
          name: 'scheduleStatistics',
          component: () => import('../views/scheduling/StatisticsView.vue'),
          meta: { requiresAuth: true, permission: 'statistics:read' }
        },
        {
          path: 'dashboard',
          name: 'scheduleDashboard',
          component: () => import('../views/scheduling/DashboardView.vue'),
          meta: { requiresAuth: true, permission: 'statistics:dashboard' }
        }
      ]
    }
  ]
})

// 路由守卫
router.beforeEach(async (to, _from, next) => {
  const authStore = useAuthStore()

  // 需要登录的页面
  if (to.meta.requiresAuth && !authStore.token) {
    next('/')
    return
  }

  // 已登录用户访问登录页，跳转到首页
  if (to.path === '/' && authStore.token) {
    next('/home')
    return
  }

  // 有 token 但 user 信息尚未加载，先获取用户信息
  if (to.meta.requiresAuth && authStore.token && !authStore.user) {
    await authStore.fetchCurrentUser()
  }

  // 超级管理员拥有所有权限，跳过权限检查
  if (authStore.isSuperAdmin) {
    next()
    return
  }

  // 权限拦截：无权限时跳转到首页
  if (to.meta.permission && !authStore.hasPermission(to.meta.permission as string)) {
    console.warn(`[权限警告] 页面 "${to.path}" 需要权限 "${to.meta.permission}"，当前用户未拥有该权限`)
    next('/home')
    return
  }

  next()
})

export default router
