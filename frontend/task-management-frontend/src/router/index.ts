import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '../stores/auth'
import LoginView from '../views/LoginView.vue'
import TasksView from '../views/TasksView.vue'
import UsersView from '../views/UsersView.vue'

const router = createRouter({
  history: createWebHistory(),
  routes: [
    {
      path: '/',
      redirect: '/tasks'
    },
    {
      path: '/login',
      name: 'Login',
      component: LoginView,
      meta: { requiresGuest: true }
    },
    {
      path: '/tasks',
      name: 'Tasks',
      component: TasksView,
      meta: { requiresAuth: true }
    },
    {
      path: '/users',
      name: 'Users',
      component: UsersView,
      meta: { requiresAuth: true }
    }
  ]
})

router.beforeEach((to, _from, next) => {
  const authStore = useAuthStore()
  
  if (to.meta.requiresAuth && !authStore.isAuthenticated) {
    next('/login')
  } else if (to.meta.requiresGuest && authStore.isAuthenticated) {
    next('/tasks')
  } else {
    next()
  }
})

export default router

