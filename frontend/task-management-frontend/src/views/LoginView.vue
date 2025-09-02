<template>
  <div class="login-container">
    <div class="card" style="max-width: 400px; margin: 2rem auto;">
      <h2 style="text-align: center; margin-bottom: 2rem;">Login</h2>
      
      <div v-if="authStore.error" class="alert alert-error">
        {{ authStore.error }}
      </div>
      
      <form @submit.prevent="handleLogin">
        <div class="form-group">
          <label class="form-label">Usuário</label>
          <input
            v-model="credentials.username"
            type="text"
            class="form-control"
            required
            :disabled="authStore.loading"
          />
        </div>
        
        <div class="form-group">
          <label class="form-label">Senha</label>
          <input
            v-model="credentials.password"
            type="password"
            class="form-control"
            required
            :disabled="authStore.loading"
          />
        </div>
        
        <button
          type="submit"
          class="btn btn-primary"
          style="width: 100%;"
          :disabled="authStore.loading"
        >
          {{ authStore.loading ? 'Entrando...' : 'Entrar' }}
        </button>
      </form>
      
      <div style="margin-top: 1rem; text-align: center; font-size: 0.9rem; color: #666;">
        <p>Usuário padrão: <strong>admin</strong></p>
        <p>Senha padrão: <strong>admin123</strong></p>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { reactive } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../stores/auth'
import type { LoginRequest } from '../types'

const authStore = useAuthStore()
const router = useRouter()

const credentials = reactive<LoginRequest>({
  username: '',
  password: ''
})

const handleLogin = async () => {
  authStore.clearError()
  
  const success = await authStore.login(credentials)
  if (success) {
    router.push('/tasks')
  }
}
</script>

