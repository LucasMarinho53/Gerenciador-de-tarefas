import { defineStore } from 'pinia'
import { ref } from 'vue'
import type { Task, CreateTaskRequest, UpdateTaskRequest, User } from '../types'
import { tasksApi, usersApi } from '../services/api'

export const useTasksStore = defineStore('tasks', () => {
  const tasks = ref<Task[]>([])
  const users = ref<User[]>([])
  const loading = ref(false)
  const error = ref<string | null>(null)

  const fetchTasks = async () => {
    loading.value = true
    error.value = null
    
    try {
      tasks.value = await tasksApi.getTasks()
    } catch (err: any) {
      error.value = err.response?.data?.message || 'Erro ao carregar tarefas'
    } finally {
      loading.value = false
    }
  }

  const fetchUsers = async () => {
    try {
      users.value = await usersApi.getUsers()
    } catch (err: any) {
      error.value = err.response?.data?.message || 'Erro ao carregar usuários'
    }
  }

  const createTask = async (taskData: CreateTaskRequest) => {
    loading.value = true
    error.value = null
    
    try {
      const newTask = await tasksApi.createTask(taskData)
      tasks.value.unshift(newTask)
      return true
    } catch (err: any) {
      error.value = err.response?.data?.message || 'Erro ao criar tarefa'
      return false
    } finally {
      loading.value = false
    }
  }

  const updateTask = async (id: string, taskData: UpdateTaskRequest) => {
    loading.value = true
    error.value = null
    
    try {
      const updatedTask = await tasksApi.updateTask(id, taskData)
      const index = tasks.value.findIndex(t => t.id === id)
      if (index !== -1) {
        tasks.value[index] = updatedTask
      }
      return true
    } catch (err: any) {
      error.value = err.response?.data?.message || 'Erro ao atualizar tarefa'
      return false
    } finally {
      loading.value = false
    }
  }

  const deleteTask = async (id: string) => {
    loading.value = true
    error.value = null
    
    try {
      await tasksApi.deleteTask(id)
      tasks.value = tasks.value.filter(t => t.id !== id)
      return true
    } catch (err: any) {
      error.value = err.response?.data?.message || 'Erro ao excluir tarefa'
      return false
    } finally {
      loading.value = false
    }
  }

  const assignTask = async (taskId: string, assigneeId: string) => {
    loading.value = true
    error.value = null
    
    try {
      const updatedTask = await tasksApi.assignTask(taskId, assigneeId)
      const index = tasks.value.findIndex(t => t.id === taskId)
      if (index !== -1) {
        tasks.value[index] = updatedTask
      }
      return true
    } catch (err: any) {
      error.value = err.response?.data?.message || 'Erro ao atribuir tarefa'
      return false
    } finally {
      loading.value = false
    }
  }

  const createRandomUsers = async (amount: number, userNameMask: string) => {
    loading.value = true
    error.value = null
    
    try {
      const response = await usersApi.createRandomUsers({ amount, userNameMask })
      await fetchUsers() // Refresh users list
      return response
    } catch (err: any) {
      error.value = err.response?.data?.message || 'Erro ao criar usuários'
      return null
    } finally {
      loading.value = false
    }
  }

  const clearError = () => {
    error.value = null
  }

  return {
    tasks,
    users,
    loading,
    error,
    fetchTasks,
    fetchUsers,
    createTask,
    updateTask,
    deleteTask,
    assignTask,
    createRandomUsers,
    clearError
  }
})

