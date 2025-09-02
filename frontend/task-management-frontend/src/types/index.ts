export interface User {
  id: string
  username: string
  email: string
  createdAt: string
}

export interface Task {
  id: string
  title: string
  description: string
  dueDate: string
  status: TaskStatus
  assignedUserId: string
  createdAt: string
  updatedAt: string
}

export enum TaskStatus {
  Pending = 0,
  InProgress = 1,
  Completed = 2
}

export interface CreateTaskRequest {
  title: string
  description: string
  dueDate: string
}

export interface UpdateTaskRequest {
  title: string
  description: string
  dueDate: string
  status: TaskStatus
}

export interface LoginRequest {
  username: string
  password: string
}

export interface LoginResponse {
  token: string
  user: User
}

export interface CreateRandomUsersRequest {
  amount: number
  userNameMask: string
}

export interface ApiResponse<T> {
  data?: T
  message?: string
  error?: string
}

