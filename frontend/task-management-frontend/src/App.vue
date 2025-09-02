<template>
  <div id="app">
    <nav class="navbar" v-if="authStore.isAuthenticated">
      <div class="container">
        <div style="display: flex; justify-content: space-between; align-items: center; width: 100%;">
          <router-link to="/" class="navbar-brand">Task Management</router-link>
          <ul class="navbar-nav">
            <li><router-link to="/tasks" class="nav-link">Tarefas</router-link></li>
            <li><router-link to="/users" class="nav-link">Usuários</router-link></li>
            <li><button @click="logout" class="nav-link" style="background: none; border: none; cursor: pointer;">Sair</button></li>
          </ul>
        </div>
      </div>
    </nav>
    
    <main class="container">
      <router-view />
    </main>
  </div>
</template>

<script setup lang="ts">
import { onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from './stores/auth'

const authStore = useAuthStore()
const router = useRouter()

onMounted(() => {
  authStore.initAuth()
})

const logout = () => {
  authStore.logout()
  router.push('/login')
}
</script>

