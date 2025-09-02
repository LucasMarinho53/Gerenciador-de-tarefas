<template>
  <div>
    <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 2rem;">
      <h1>Usuários</h1>
      <button @click="showCreateUsersModal = true" class="btn btn-primary">
        Criar Usuários Aleatórios
      </button>
    </div>
    
    <div v-if="tasksStore.error" class="alert alert-error">
      {{ tasksStore.error }}
      <button @click="tasksStore.clearError" style="float: right; background: none; border: none; color: inherit;">×</button>
    </div>
    
    <div v-if="successMessage" class="alert alert-success">
      {{ successMessage }}
      <button @click="successMessage = ''" style="float: right; background: none; border: none; color: inherit;">×</button>
    </div>
    
    <div v-if="tasksStore.loading" class="loading">
      Carregando usuários...
    </div>
    
    <div v-else-if="tasksStore.users.length === 0" class="card">
      <p style="text-align: center; color: #666;">Nenhum usuário encontrado. Crie usuários aleatórios para começar!</p>
    </div>
    
    <div v-else class="card">
      <h3 style="margin-bottom: 1rem;">Lista de Usuários ({{ tasksStore.users.length }})</h3>
      <div style="overflow-x: auto;">
        <table style="width: 100%; border-collapse: collapse;">
          <thead>
            <tr style="border-bottom: 2px solid #ddd;">
              <th style="padding: 12px; text-align: left;">ID</th>
              <th style="padding: 12px; text-align: left;">Nome de Usuário</th>
              <th style="padding: 12px; text-align: left;">Email</th>
              <th style="padding: 12px; text-align: left;">Data de Criação</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="user in tasksStore.users" :key="user.id" style="border-bottom: 1px solid #eee;">
              <td style="padding: 12px; font-family: monospace; font-size: 0.9rem;">{{ user.id.substring(0, 8) }}...</td>
              <td style="padding: 12px; font-weight: 500;">{{ user.username }}</td>
              <td style="padding: 12px;">{{ user.email }}</td>
              <td style="padding: 12px;">{{ formatDate(user.createdAt) }}</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
    
    <!-- Create Random Users Modal -->
    <div v-if="showCreateUsersModal" class="modal" @click.self="closeModal">
      <div class="modal-content">
        <div class="modal-header">
          <h3 class="modal-title">Criar Usuários Aleatórios</h3>
          <button @click="closeModal" class="close-btn">×</button>
        </div>
        
        <form @submit.prevent="handleCreateUsers">
          <div class="form-group">
            <label class="form-label">Quantidade de Usuários</label>
            <input
              v-model.number="userForm.amount"
              type="number"
              class="form-control"
              min="1"
              max="1000"
              required
              :disabled="tasksStore.loading"
            />
            <small style="color: #666;">Máximo: 1000 usuários</small>
          </div>
          
          <div class="form-group">
            <label class="form-label">Máscara do Nome de Usuário</label>
            <input
              v-model="userForm.userNameMask"
              type="text"
              class="form-control"
              required
              :disabled="tasksStore.loading"
              placeholder="user_{{random}}"
            />
            <small style="color: #666;">Use {{ '{' }}{{ '{' }}random{{ '}' }}{{ '}' }} para inserir números aleatórios</small>
          </div>
          
          <div style="display: flex; gap: 1rem; justify-content: flex-end;">
            <button type="button" @click="closeModal" class="btn btn-secondary" :disabled="tasksStore.loading">
              Cancelar
            </button>
            <button type="submit" class="btn btn-primary" :disabled="tasksStore.loading">
              {{ tasksStore.loading ? 'Criando...' : 'Criar Usuários' }}
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted, reactive, ref } from 'vue'
import { useTasksStore } from '../stores/tasks'
import type { CreateRandomUsersRequest } from '../types'

const tasksStore = useTasksStore()

const showCreateUsersModal = ref(false)
const successMessage = ref('')

const userForm = reactive<CreateRandomUsersRequest>({
  amount: 10,
  userNameMask: 'user_{{random}}'
})

onMounted(async () => {
  await tasksStore.fetchUsers()
})

const formatDate = (dateString: string) => {
  return new Date(dateString).toLocaleDateString('pt-BR', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  })
}

const handleCreateUsers = async () => {
  const result = await tasksStore.createRandomUsers(userForm.amount, userForm.userNameMask)
  
  if (result) {
    successMessage.value = result.message
    closeModal()
  }
}

const closeModal = () => {
  showCreateUsersModal.value = false
  
  // Reset form
  userForm.amount = 10
  userForm.userNameMask = 'user_{{random}}'
}
</script>

