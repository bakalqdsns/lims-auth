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
    }
  ]
})

// 路由守卫
router.beforeEach((to, from, next) => {
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
