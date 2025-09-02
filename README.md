# Sistema de Gerenciamento de Tarefas

Este projeto implementa um sistema completo de gerenciamento de tarefas desenvolvido como parte do desafio técnico da VSoft para a posição de Desenvolvedor Fullstack Sênior.

## 🚀 Tecnologias Utilizadas

### Backend
- **.NET 8 Web API** - Framework principal
- **Entity Framework Core** - ORM com banco em memória
- **RabbitMQ** - Sistema de mensageria para notificações
- **JWT Authentication** - Autenticação baseada em tokens
- **Swagger/OpenAPI** - Documentação da API

### Frontend
- **Vue.js 3** - Framework JavaScript reativo
- **TypeScript** - Tipagem estática
- **Vue Router** - Roteamento SPA
- **Pinia** - Gerenciamento de estado
- **Axios** - Cliente HTTP

### DevOps
- **Docker & Docker Compose** - Containerização
- **Nginx** - Servidor web para o frontend

## 📋 Funcionalidades Implementadas

### ✅ Requisitos Obrigatórios
- [x] **CRUD de Tarefas** - Criação, listagem, atualização e exclusão
- [x] **Atribuição de Tarefas** - Atribuir tarefas a diferentes usuários
- [x] **Notificações RabbitMQ** - Notificação quando tarefa é atribuída
- [x] **Autenticação JWT** - Apenas usuários autenticados podem acessar
- [x] **Endpoint de Usuários Aleatórios** - `POST /users/createRandom`
- [x] **Gerenciamento de Estado** - Pinia para compartilhamento entre componentes
- [x] **Princípios de Clean Code** - KISS, YAGNI, DRY aplicados
- [x] **Testes Unitários** - Cobertura das principais funcionalidades
- [x] **Testes de Integração** - Pelo menos 1 endpoint testado
- [x] **Docker Compose** - Execução com `docker compose up -d`

### 🎯 Modelo de Dados

#### Task
```csharp
{
  "id": "guid",
  "title": "string",
  "description": "string", 
  "dueDate": "datetime",
  "status": "Pending | InProgress | Completed",
  "assignedUserId": "guid",
  "createdAt": "datetime",
  "updatedAt": "datetime"
}
```

#### User
```csharp
{
  "id": "guid",
  "username": "string",
  "email": "string",
  "password": "string (hashed)",
  "createdAt": "datetime"
}
```

## 🛠️ Como Executar

### Pré-requisitos
- Docker
- Docker Compose

### Execução Completa
```bash
# Clone o repositório
git clone <repository-url>
cd task-management-system

# Execute todos os serviços
docker compose up -d
```

### Acessos
- **Frontend**: http://localhost:3000
- **Backend API**: http://localhost:5000
- **Swagger UI**: http://localhost:5000/swagger
- **RabbitMQ Management**: http://localhost:15672 (guest/guest)

### Credenciais Padrão
- **Usuário**: admin
- **Senha**: admin123

## 🏗️ Arquitetura

### Backend (.NET 8)
```
TaskManagementApi/
├── Controllers/          # Controladores da API
│   ├── AuthController.cs
│   ├── TasksController.cs
│   └── UsersController.cs
├── Models/              # Modelos de dados
│   ├── TaskItem.cs
│   ├── User.cs
│   └── DTOs/
├── Services/            # Serviços de negócio
│   ├── AuthService.cs
│   ├── RabbitMQService.cs
│   └── Interfaces/
├── Data/               # Contexto do banco
│   └── TaskManagementContext.cs
└── Program.cs          # Configuração da aplicação
```

### Frontend (Vue.js 3)
```
src/
├── components/         # Componentes reutilizáveis
├── views/             # Páginas da aplicação
│   ├── LoginView.vue
│   ├── TasksView.vue
│   └── UsersView.vue
├── stores/            # Gerenciamento de estado (Pinia)
│   ├── auth.ts
│   └── tasks.ts
├── services/          # Serviços de API
│   └── api.ts
├── types/            # Definições TypeScript
│   └── index.ts
├── router/           # Configuração de rotas
│   └── index.ts
└── App.vue           # Componente raiz
```

## 🔧 Endpoints da API

### Autenticação
- `POST /api/auth/login` - Login do usuário

### Tarefas
- `GET /api/tasks` - Listar tarefas do usuário
- `GET /api/tasks/{id}` - Obter tarefa específica
- `POST /api/tasks` - Criar nova tarefa
- `PUT /api/tasks/{id}` - Atualizar tarefa
- `DELETE /api/tasks/{id}` - Excluir tarefa
- `POST /api/tasks/{id}/assign/{userId}` - Atribuir tarefa

### Usuários
- `GET /api/users` - Listar usuários
- `POST /api/users/createRandom` - Criar usuários aleatórios
- `POST /api/users/register` - Registrar novo usuário

## 🧪 Testes

### Executar Testes do Backend
```bash
cd backend/TaskManagementApi
dotnet test
```

### Cobertura de Testes
- Testes unitários para serviços de autenticação
- Testes unitários para serviços de tarefas
- Testes de integração para endpoints de tarefas
- Testes de validação de modelos

## 🔔 Sistema de Notificações

O sistema utiliza RabbitMQ para enviar notificações em tempo real quando:
- Uma nova tarefa é criada
- Uma tarefa é atribuída a um usuário
- O status de uma tarefa é alterado

### Estrutura da Mensagem
```json
{
  "userId": "guid",
  "taskId": "guid", 
  "taskTitle": "string",
  "message": "string",
  "timestamp": "datetime"
}
```

## 🎨 Interface do Usuário

### Funcionalidades da UI
- **Dashboard de Tarefas** - Visualização em cards com filtros por status
- **Formulários Modais** - Criação e edição de tarefas
- **Atribuição de Tarefas** - Interface para atribuir tarefas a usuários
- **Gerenciamento de Usuários** - Criação em massa de usuários para testes
- **Notificações Visuais** - Feedback visual para ações do usuário
- **Design Responsivo** - Compatível com desktop e mobile

### Estados das Tarefas
- 🟡 **Pendente** - Tarefa criada, aguardando início
- 🔵 **Em Progresso** - Tarefa sendo executada
- 🟢 **Concluída** - Tarefa finalizada

## 🔒 Segurança

- **Autenticação JWT** - Tokens seguros com expiração
- **Autorização** - Usuários só acessam suas próprias tarefas
- **Validação de Dados** - Validação tanto no frontend quanto backend
- **Hash de Senhas** - Senhas armazenadas com hash SHA256
- **CORS Configurado** - Comunicação segura entre frontend e backend

## 📊 Monitoramento

### RabbitMQ Management
- Acesse http://localhost:15672
- Monitore filas de mensagens
- Visualize estatísticas de consumo

### Logs da Aplicação
```bash
# Visualizar logs do backend
docker logs task-backend

# Visualizar logs do frontend  
docker logs task-frontend

# Visualizar logs do RabbitMQ
docker logs task-rabbitmq
```

## 🚀 Melhorias Futuras

### Funcionalidades Adicionais
- [ ] **Integração iCalendar** - Exportar tarefas para Outlook/Thunderbird
- [ ] **Notificações Push** - Notificações em tempo real no frontend
- [ ] **Filtros Avançados** - Filtrar por data, usuário, prioridade
- [ ] **Dashboard Analytics** - Métricas e relatórios de produtividade
- [ ] **Comentários em Tarefas** - Sistema de comentários colaborativo
- [ ] **Anexos** - Upload de arquivos nas tarefas
- [ ] **Subtarefas** - Hierarquia de tarefas
- [ ] **Etiquetas/Tags** - Categorização de tarefas

### Melhorias Técnicas
- [ ] **Banco de Dados Persistente** - PostgreSQL ou SQL Server
- [ ] **Cache Redis** - Cache de dados frequentemente acessados
- [ ] **Logs Estruturados** - Serilog com ElasticSearch
- [ ] **Health Checks** - Monitoramento de saúde dos serviços
- [ ] **Rate Limiting** - Proteção contra abuso da API
- [ ] **Versionamento da API** - Suporte a múltiplas versões
- [ ] **Testes E2E** - Testes end-to-end com Playwright
- [ ] **CI/CD Pipeline** - Automação de deploy

## 👥 Contribuição

Este projeto foi desenvolvido seguindo as melhores práticas de desenvolvimento:

- **Clean Code** - Código limpo e legível
- **SOLID Principles** - Princípios de design orientado a objetos
- **DRY** - Don't Repeat Yourself
- **KISS** - Keep It Simple, Stupid
- **YAGNI** - You Aren't Gonna Need It

## 📝 Licença

Este projeto foi desenvolvido para fins de avaliação técnica.

---

**Desenvolvido por:** [Seu Nome]  
**Data:** [Data Atual]  
**Versão:** 1.0.0

