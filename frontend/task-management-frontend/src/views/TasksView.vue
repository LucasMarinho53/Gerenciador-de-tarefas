<template>
  <div>
    <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 2rem;">
      <h1>Minhas Tarefas</h1>
      <button @click="showCreateModal = true" class="btn btn-primary">
        Nova Tarefa
      </button>
    </div>
    
    <div v-if="tasksStore.error" class="alert alert-error">
      {{ tasksStore.error }}
      <button @click="tasksStore.clearError" style="float: right; background: none; border: none; color: inherit;">×</button>
    </div>
    
    <div v-if="tasksStore.loading" class="loading">
      Carregando tarefas...
    </div>
    
    <div v-else-if="tasksStore.tasks.length === 0" class="card">
      <p style="text-align: center; color: #666;">Nenhuma tarefa encontrada. Crie sua primeira tarefa!</p>
    </div>
    
    <div v-else class="task-grid">
      <div v-for="task in tasksStore.tasks" :key="task.id" class="task-card">
        <div class="task-title">{{ task.title }}</div>
        <div class="task-description">{{ task.description }}</div>
        <div class="task-meta">
          <span>Vencimento: {{ formatDate(task.dueDate) }}</span>
          <span class="task-status" :class="getStatusClass(task.status)">
            {{ getStatusText(task.status) }}
          </span>
        </div>
        <div class="task-actions">
          <button @click="editTask(task)" class="btn btn-secondary">Editar</button>
          <button @click="deleteTaskConfirm(task)" class="btn btn-danger">Excluir</button>
          <button @click="showAssignModal(task)" class="btn btn-primary">Atribuir</button>
        </div>
      </div>
    </div>
    
    <!-- Create/Edit Task Modal -->
    <div v-if="showCreateModal || showEditModal" class="modal" @click.self="closeModals">
      <div class="modal-content">
        <div class="modal-header">
          <h3 class="modal-title">{{ showEditModal ? 'Editar Tarefa' : 'Nova Tarefa' }}</h3>
          <button @click="closeModals" class="close-btn">×</button>
        </div>
        
        <form @submit.prevent="handleSubmit">
          <div class="form-group">
            <label class="form-label">Título</label>
            <input
              v-model="taskForm.title"
              type="text"
              class="form-control"
              required
              :disabled="tasksStore.loading"
            />
          </div>
          
          <div class="form-group">
            <label class="form-label">Descrição</label>
            <textarea
              v-model="taskForm.description"
              class="form-control"
              rows="3"
              :disabled="tasksStore.loading"
            ></textarea>
          </div>
          
          <div class="form-group">
            <label class="form-label">Data de Vencimento</label>
            <input
              v-model="taskForm.dueDate"
              type="datetime-local"
              class="form-control"
              required
              :disabled="tasksStore.loading"
            />
          </div>
          
          <div v-if="showEditModal" class="form-group">
            <label class="form-label">Status</label>
            <select v-model="taskForm.status" class="form-control" :disabled="tasksStore.loading">
              <option :value="0">Pendente</option>
              <option :value="1">Em Progresso</option>
              <option :value="2">Concluída</option>
            </select>
          </div>
          
          <div style="display: flex; gap: 1rem; justify-content: flex-end;">
            <button type="button" @click="closeModals" class="btn btn-secondary" :disabled="tasksStore.loading">
              Cancelar
            </button>
            <button type="submit" class="btn btn-primary" :disabled="tasksStore.loading">
              {{ tasksStore.loading ? 'Salvando...' : 'Salvar' }}
            </button>
          </div>
        </form>
      </div>
    </div>
    
    <!-- Assign Task Modal -->
    <div v-if="showAssignTaskModal" class="modal" @click.self="closeModals">
      <div class="modal-content">
        <div class="modal-header">
          <h3 class="modal-title">Atribuir Tarefa</h3>
          <button @click="closeModals" class="close-btn">×</button>
        </div>
        
        <div v-if="tasksStore.users.length === 0" class="alert alert-error">
          Nenhum usuário disponível. Crie usuários primeiro na aba "Usuários".
        </div>
        
        <form v-else @submit.prevent="handleAssignTask">
          <div class="form-group">
            <label class="form-label">Selecionar Usuário</label>
            <select v-model="selectedUserId" class="form-control" required :disabled="tasksStore.loading">
              <option value="">Selecione um usuário</option>
              <option v-for="user in tasksStore.users" :key="user.id" :value="user.id">
                {{ user.username }} ({{ user.email }})
              </option>
            </select>
          </div>
          
          <div style="display: flex; gap: 1rem; justify-content: flex-end;">
            <button type="button" @click="closeModals" class="btn btn-secondary" :disabled="tasksStore.loading">
              Cancelar
            </button>
            <button type="submit" class="btn btn-primary" :disabled="tasksStore.loading || !selectedUserId">
              {{ tasksStore.loading ? 'Atribuindo...' : 'Atribuir' }}
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
import type { Task, CreateTaskRequest, UpdateTaskRequest } from '../types'
import { TaskStatus } from '../types'

const tasksStore = useTasksStore()

const showCreateModal = ref(false)
const showEditModal = ref(false)
const showAssignTaskModal = ref(false)
const editingTask = ref<Task | null>(null)
const selectedTaskForAssign = ref<Task | null>(null)
const selectedUserId = ref('')

const taskForm = reactive<CreateTaskRequest & { status?: TaskStatus }>({
  title: '',
  description: '',
  dueDate: '',
  status: TaskStatus.Pending
})

onMounted(async () => {
  await tasksStore.fetchTasks()
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

const getStatusText = (status: TaskStatus) => {
  switch (status) {
    case TaskStatus.Pending: return 'Pendente'
    case TaskStatus.InProgress: return 'Em Progresso'
    case TaskStatus.Completed: return 'Concluída'
    default: return 'Desconhecido'
  }
}

const getStatusClass = (status: TaskStatus) => {
  switch (status) {
    case TaskStatus.Pending: return 'status-pending'
    case TaskStatus.InProgress: return 'status-inprogress'
    case TaskStatus.Completed: return 'status-completed'
    default: return ''
  }
}

const editTask = (task: Task) => {
  editingTask.value = task
  taskForm.title = task.title
  taskForm.description = task.description
  taskForm.dueDate = new Date(task.dueDate).toISOString().slice(0, 16)
  taskForm.status = task.status
  showEditModal.value = true
}

const showAssignModal = (task: Task) => {
  selectedTaskForAssign.value = task
  selectedUserId.value = ''
  showAssignTaskModal.value = true
}

const deleteTaskConfirm = async (task: Task) => {
  if (confirm(`Tem certeza que deseja excluir a tarefa "${task.title}"?`)) {
    await tasksStore.deleteTask(task.id)
  }
}

const handleSubmit = async () => {
  if (showEditModal.value && editingTask.value) {
    const updateData: UpdateTaskRequest = {
      title: taskForm.title,
      description: taskForm.description,
      dueDate: new Date(taskForm.dueDate).toISOString(),
      status: taskForm.status || TaskStatus.Pending
    }
    
    const success = await tasksStore.updateTask(editingTask.value.id, updateData)
    if (success) {
      closeModals()
    }
  } else {
    const createData: CreateTaskRequest = {
      title: taskForm.title,
      description: taskForm.description,
      dueDate: new Date(taskForm.dueDate).toISOString()
    }
    
    const success = await tasksStore.createTask(createData)
    if (success) {
      closeModals()
    }
  }
}

const handleAssignTask = async () => {
  if (selectedTaskForAssign.value && selectedUserId.value) {
    const success = await tasksStore.assignTask(selectedTaskForAssign.value.id, selectedUserId.value)
    if (success) {
      closeModals()
    }
  }
}

const closeModals = () => {
  showCreateModal.value = false
  showEditModal.value = false
  showAssignTaskModal.value = false
  editingTask.value = null
  selectedTaskForAssign.value = null
  selectedUserId.value = ''
  
  // Reset form
  taskForm.title = ''
  taskForm.description = ''
  taskForm.dueDate = ''
  taskForm.status = TaskStatus.Pending
}
</script>

