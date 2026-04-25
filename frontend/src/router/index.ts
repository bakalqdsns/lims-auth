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
    }
  ]
})

// 路由守卫
router.beforeEach((to, _from, next) => {
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

  // 检查权限
  if (to.meta.permission && !authStore.hasPermission(to.meta.permission as string)) {
    next('/home')
    return
  }

  next()
})

export default router
