import axios from 'axios'
import type { 
  Task, 
  User, 
  CreateTaskRequest, 
  UpdateTaskRequest, 
  LoginRequest, 
  LoginResponse,
  CreateRandomUsersRequest 
} from '../types'

const API_BASE_URL = 'http://localhost:5000/api'

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json'
  }
})

// Add auth token to requests
api.interceptors.request.use((config) => {
  const token = localStorage.getItem('token')
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

// Handle auth errors
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem('token')
      localStorage.removeItem('user')
      window.location.href = '/login'
    }
    return Promise.reject(error)
  }
)

export const authApi = {
  login: (credentials: LoginRequest): Promise<LoginResponse> =>
    api.post('/auth/login', credentials).then(res => res.data),
}

export const tasksApi = {
  getTasks: (): Promise<Task[]> =>
    api.get('/tasks').then(res => res.data),
  
  getTask: (id: string): Promise<Task> =>
    api.get(`/tasks/${id}`).then(res => res.data),
  
  createTask: (task: CreateTaskRequest): Promise<Task> =>
    api.post('/tasks', task).then(res => res.data),
  
  updateTask: (id: string, task: UpdateTaskRequest): Promise<Task> =>
    api.put(`/tasks/${id}`, task).then(res => res.data),
  
  deleteTask: (id: string): Promise<void> =>
    api.delete(`/tasks/${id}`),
  
  assignTask: (taskId: string, assigneeId: string): Promise<Task> =>
    api.post(`/tasks/${taskId}/assign/${assigneeId}`).then(res => res.data)
}

export const usersApi = {
  getUsers: (): Promise<User[]> =>
    api.get('/users').then(res => res.data),
  
  createRandomUsers: (request: CreateRandomUsersRequest): Promise<{ message: string, users: User[] }> =>
    api.post('/users/createRandom', request).then(res => res.data)
}

export default api

