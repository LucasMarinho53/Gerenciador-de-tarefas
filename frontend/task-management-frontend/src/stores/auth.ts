import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import type { User, LoginRequest } from '../types'
import { authApi } from '../services/api'

export const useAuthStore = defineStore('auth', () => {
  const user = ref<User | null>(null)
  const token = ref<string | null>(null)
  const loading = ref(false)
  const error = ref<string | null>(null)

  const isAuthenticated = computed(() => !!token.value)

  const initAuth = () => {
    const savedToken = localStorage.getItem('token')
    const savedUser = localStorage.getItem('user')
    
    if (savedToken && savedUser) {
      token.value = savedToken
      user.value = JSON.parse(savedUser)
    }
  }

  const login = async (credentials: LoginRequest) => {
    loading.value = true
    error.value = null
    
    try {
      const response = await authApi.login(credentials)
      
      token.value = response.token
      user.value = response.user
      
      localStorage.setItem('token', response.token)
      localStorage.setItem('user', JSON.stringify(response.user))
      
      return true
    } catch (err: any) {
      error.value = err.response?.data?.message || 'Erro ao fazer login'
      return false
    } finally {
      loading.value = false
    }
  }

  const logout = () => {
    user.value = null
    token.value = null
    error.value = null
    
    localStorage.removeItem('token')
    localStorage.removeItem('user')
  }

  const clearError = () => {
    error.value = null
  }

  return {
    user,
    token,
    loading,
    error,
    isAuthenticated,
    initAuth,
    login,
    logout,
    clearError
  }
})

