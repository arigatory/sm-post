# 📱 Social Media Post - CQRS & Event Sourcing приложение

Простое приложение для создания и управления постами в социальной сети, построенное на современных архитектурных паттернах.

## 📚 Оглавление
- [Что это за приложение?](#что-это-за-приложение)
- [Основные концепции](#основные-концепции)
- [Архитектура приложения](#архитектура-приложения)
- [Как это работает?](#как-это-работает)
- [Технологический стек](#технологический-стек)
- [Быстрый старт](#быстрый-старт)
- [Структура проекта](#структура-проекта)

---

## 🎯 Что это за приложение?

Это учебное приложение социальной сети, где пользователи могут:
- ✍️ Создавать посты
- ✏️ Редактировать сообщения в постах
- 👍 Ставить лайки
- 💬 Добавлять, редактировать и удалять комментарии
- 🗑️ Удалять посты
- 📊 Просматривать все посты и фильтровать их

### Чем это отличается от обычного приложения?

В обычном приложении мы:
1. Получаем запрос от пользователя
2. Сразу изменяем данные в базе
3. Отправляем ответ

Здесь используется **другой подход**:
1. Получаем команду (например, "создать пост")
2. **Сохраняем событие** "пост создан" в отдельную базу (Event Store)
3. Это событие **автоматически обрабатывается** и обновляет базу для чтения
4. Пользователь видит обновленные данные

---

## 🧠 Основные концепции

### 1. CQRS (Command Query Responsibility Segregation)

**Простыми словами:** Разделение записи и чтения данных.

```
┌─────────────────────────────────────────────────────────────┐
│                      ТРАДИЦИОННЫЙ ПОДХОД                     │
├─────────────────────────────────────────────────────────────┤
│  Один API → Одна База Данных → Делает ВСЁ                   │
│  (и запись, и чтение)                                        │
└─────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────┐
│                         CQRS ПОДХОД                          │
├──────────────────────────┬──────────────────────────────────┤
│  Command API             │  Query API                        │
│  (Команды - ЗАПИСЬ)      │  (Запросы - ЧТЕНИЕ)              │
│  - Создать пост          │  - Получить все посты            │
│  - Изменить пост         │  - Найти пост по ID              │
│  - Удалить пост          │  - Фильтровать посты             │
│  ↓                       │  ↓                                │
│  Event Store DB          │  Read DB (PostgreSQL)            │
│  (Marten/PostgreSQL)     │                                   │
└──────────────────────────┴──────────────────────────────────┘
```

**Зачем это нужно?**
- ✅ **Масштабируемость**: Можем независимо масштабировать запись и чтение
- ✅ **Производительность**: База для чтения оптимизирована под запросы
- ✅ **Гибкость**: Можем иметь несколько баз для чтения с разной структурой

### 2. Event Sourcing

**Простыми словами:** Вместо сохранения текущего состояния, мы сохраняем **историю всех изменений** (события).

**Традиционный подход:**
```sql
-- В базе хранится только текущее состояние
Posts
├── Id: 123
├── Message: "Привет, мир!"  ← Только последняя версия
└── Likes: 5
```

**Event Sourcing:**
```javascript
// В базе хранится история событий
Events [
  { type: "PostCreated", message: "Привет!" },
  { type: "MessageUpdated", message: "Привет, мир!" },
  { type: "PostLiked" },
  { type: "PostLiked" },
  { type: "PostLiked" },
  { type: "PostLiked" },
  { type: "PostLiked" }
]
// Текущее состояние = применить все события по порядку
```

**Преимущества:**
- 📜 **Полная история**: Видим все изменения, кто и когда их сделал
- ⏮️ **Откат изменений**: Можем "перемотать" время назад
- 🔍 **Аудит**: Знаем что, когда и почему изменилось
- 🛠️ **Восстановление**: Можем пересоздать Read DB из событий

### 3. Vertical Slice Architecture

**Простыми словами:** Каждая функция (feature) живет в своей папке со всем необходимым.

**Традиционный подход (по слоям):**
```
Controllers/
  ├── PostController.cs
  └── CommentController.cs
Services/
  ├── PostService.cs
  └── CommentService.cs
Repositories/
  ├── PostRepository.cs
  └── CommentRepository.cs
```
❌ Проблема: Чтобы изменить одну функцию, нужно трогать 3+ файла в разных папках

**Vertical Slice (по функциям):**
```
Features/
├── Posts/
│   ├── CreatePost/
│   │   ├── CreatePostCommand.cs
│   │   ├── CreatePostHandler.cs
│   │   └── CreatePostEndpoint.cs
│   ├── EditMessage/
│   │   └── ... (все для редактирования)
│   └── LikePost/
│       └── ... (все для лайков)
```
✅ Преимущество: Вся логика одной функции в одной папке!

---

## 🏗️ Архитектура приложения

### Общая схема

```
┌─────────────┐
│   Client    │  ← Next.js Frontend (React)
│  (Browser)  │
└──────┬──────┘
       │
       ├─────────────────┬─────────────────┐
       ↓                 ↓                 ↓
┌─────────────┐   ┌─────────────┐   ┌─────────────┐
│ Command API │   │  Query API  │   │  Kafka UI   │
│   :5262     │   │    :5263    │   │    :8080    │
└──────┬──────┘   └──────┬──────┘   └─────────────┘
       │                 │
       │                 │
       ↓                 ↓
┌─────────────┐   ┌─────────────┐
│   Marten    │   │ PostgreSQL  │
│ Event Store │   │  (Read DB)  │
│   :5433     │   │    :5432    │
└──────┬──────┘   └──────┬──────┘
       │                 ↑
       ↓                 │
┌──────────────────────────┐
│   Kafka (Event Bus)      │
│        :9092             │
└──────────────────────────┘
```

### Поток данных (пошагово)

#### 📝 Создание поста (Write - Command)

```
1. Пользователь → "Создать пост"
   ↓
2. Frontend отправляет POST запрос → Command API
   POST /api/posts/new
   {
     "author": "Иван",
     "message": "Мой первый пост!"
   }
   ↓
3. Command API:
   a) Получает команду через Carter Endpoint
   b) MediatR отправляет команду в Handler
   c) Handler создает Aggregate (бизнес-логика)
   d) Aggregate генерирует событие: PostCreatedEvent
   e) Событие сохраняется в Marten Event Store
   f) Событие публикуется в Kafka
   ↓
4. Kafka сохраняет событие в топик "SocialMediaPostEvents"
   ↓
5. Query API Consumer получает событие из Kafka
   ↓
6. MediatR Notification Handler обрабатывает событие
   ↓
7. PostCreatedEventHandler:
   - Создает PostEntity
   - Сохраняет в PostgreSQL Read DB
   ↓
8. Frontend получает ответ: "Пост создан!"
```

#### 📖 Чтение постов (Read - Query)

```
1. Пользователь → "Показать все посты"
   ↓
2. Frontend отправляет GET запрос → Query API
   GET /api/posts
   ↓
3. Query API:
   a) Получает запрос через Carter Endpoint
   b) MediatR отправляет Query в Handler
   c) Handler читает данные из PostgreSQL Read DB
   d) Возвращает список постов
   ↓
4. Frontend отображает посты пользователю
```

---

## 🔧 Как это работает?

### 1. Command Side (Сторона записи)

#### Aggregate (Агрегат)
```csharp
// PostAggregate.cs - Бизнес-логика поста
public class PostAggregate : AggregateRoot
{
    public void CreatePost(string author, string message)
    {
        // Валидация
        if (string.IsNullOrEmpty(message))
            throw new Exception("Сообщение не может быть пустым");

        // Создаем событие
        var @event = new PostCreatedEvent
        {
            Id = Guid.NewGuid(),
            Author = author,
            Message = message,
            DatePosted = DateTime.UtcNow
        };

        // Применяем событие
        RaiseEvent(@event);
    }

    // Метод применяет событие к состоянию агрегата
    public void Apply(PostCreatedEvent @event)
    {
        _id = @event.Id;
        _author = @event.Author;
        _message = @event.Message;
        _active = true;
    }
}
```

**Что здесь происходит?**
1. Метод `CreatePost` - это **команда** (что мы хотим сделать)
2. Мы **не изменяем состояние напрямую**, а создаем событие
3. Событие сохраняется в Event Store через `RaiseEvent`
4. Метод `Apply` восстанавливает состояние из событий

#### Event Store Repository
```csharp
// Сохранение событий в Marten
public async Task SaveEventsAsync(Guid aggregateId, 
                                   IEnumerable<BaseEvent> events)
{
    using var session = _store.LightweightSession();
    
    foreach (var @event in events)
    {
        var eventModel = new EventModel
        {
            AggregateIdentifier = aggregateId,
            AggregateType = nameof(PostAggregate),
            EventType = @event.GetType().Name,
            EventData = JsonSerializer.Serialize(@event)
        };
        
        session.Store(eventModel);
    }
    
    await session.SaveChangesAsync();
}
```

### 2. Event Bus (Kafka)

#### Kafka Producer
```csharp
// Публикация событий в Kafka
public async Task ProduceAsync<T>(string topic, Message<string, T> message)
{
    await _producer.ProduceAsync(topic, message);
}
```

#### Kafka Consumer
```csharp
// Получение событий из Kafka
public void Consume(string topic)
{
    _consumer.Subscribe(topic);
    
    while (true)
    {
        var consumeResult = _consumer.Consume();
        var @event = JsonSerializer.Deserialize<BaseEvent>(
            consumeResult.Message.Value
        );
        
        // Публикуем событие через MediatR
        await _mediator.Publish(@event);
    }
}
```

### 3. Query Side (Сторона чтения)

#### Event Handler (Обработчик событий)
```csharp
// PostCreatedEventHandler.cs
public class PostCreatedEventHandler 
    : INotificationHandler<PostCreatedEvent>
{
    private readonly IPostRepository _postRepository;

    public async Task Handle(PostCreatedEvent notification, 
                             CancellationToken cancellationToken)
    {
        // Создаем сущность для Read DB
        var post = new PostEntity
        {
            PostId = notification.Id,
            Author = notification.Author,
            Message = notification.Message,
            DatePosted = notification.DatePosted.ToUniversalTime()
        };

        // Сохраняем в PostgreSQL
        await _postRepository.CreateAsync(post);
    }
}
```

**Что здесь происходит?**
1. Kafka Consumer получает событие `PostCreatedEvent`
2. MediatR публикует это событие как Notification
3. `PostCreatedEventHandler` получает уведомление
4. Создается запись в Read DB (PostgreSQL)
5. Теперь Query API может прочитать этот пост

#### Query Handler
```csharp
// GetAllPostsHandler.cs
public class GetAllPostsHandler 
    : IRequestHandler<GetAllPostsQuery, List<PostEntity>>
{
    private readonly IPostRepository _postRepository;

    public async Task<List<PostEntity>> Handle(
        GetAllPostsQuery request, 
        CancellationToken cancellationToken)
    {
        // Просто читаем из PostgreSQL
        return await _postRepository.ListAllAsync();
    }
}
```

### 4. Frontend (Next.js)

```typescript
// app/page.tsx
export default function Home() {
  const { data: posts } = usePosts(); // Получаем посты из Query API

  const createPost = async (data: CreatePostData) => {
    // Отправляем команду в Command API
    await fetch('http://localhost:5262/api/posts/new', {
      method: 'POST',
      body: JSON.stringify(data)
    });
    
    // Обновляем список постов
    mutate();
  };

  return (
    <div>
      <CreatePostForm onSubmit={createPost} />
      <PostList posts={posts} />
    </div>
  );
}
```

---

## 💻 Технологический стек

### Backend
- **C# / .NET 8** - Основной язык и фреймворк
- **Marten** - Event Store на базе PostgreSQL
- **PostgreSQL** - База данных для Event Store и Read DB
- **Kafka** - Message broker для событий (KRaft режим без Zookeeper)
- **MediatR** - Реализация паттерна Mediator для CQRS
- **Carter** - Минималистичный API framework
- **Entity Framework Core** - ORM для Read DB

### Frontend
- **Next.js 15** - React framework
- **TypeScript** - Типизированный JavaScript
- **shadcn/ui** - Компоненты UI
- **TanStack Query** - Управление состоянием и кэшированием

### Infrastructure
- **Docker & Docker Compose** - Контейнеризация
- **Kubernetes** - Оркестрация контейнеров
- **Kafka UI** - Мониторинг Kafka

---

## 🚀 Быстрый старт

### Предварительные требования
- Docker Desktop (с включенным Kubernetes)
- .NET 8 SDK (опционально, для локальной разработки)
- Node.js 18+ (опционально, для фронтенда)

### 1. Запуск через Docker Compose (Рекомендуется)

```bash
# Клонируем репозиторий
git clone <repository-url>
cd sm-post

# Запускаем все сервисы
cd Infra
docker-compose up -d

# Проверяем статус
docker-compose ps
```

**Доступ к сервисам:**
- 🌐 Frontend: http://localhost:3000
- 📝 Command API: http://localhost:5262
- 📖 Query API: http://localhost:5263
- 📊 Kafka UI: http://localhost:8080
- 📄 Swagger (Command): http://localhost:5262/swagger
- 📄 Swagger (Query): http://localhost:5263/swagger

### 2. Запуск в Kubernetes

```bash
cd k8s

# Развернуть все сервисы
./deploy-all.sh

# Проверить статус
kubectl get pods -n cqrs-app

# Просмотреть логи
kubectl logs -f deployment/post-cmd-api -n cqrs-app
```

**Доступ к сервисам (NodePort):**
- 🌐 Frontend: http://localhost:30000
- 📝 Command API: http://localhost:30262
- 📖 Query API: http://localhost:30263
- 📊 Kafka UI: http://localhost:30080

### 3. Тестирование API

```bash
# Запустить тестовый скрипт
cd Infra
./test-service.sh
```

Этот скрипт:
1. ✅ Создает пост
2. ✅ Редактирует сообщение
3. ✅ Ставит лайк
4. ✅ Добавляет комментарий
5. ✅ Проверяет Query API
6. ✅ Удаляет пост
7. ✅ Восстанавливает Read DB из событий

---

## 📁 Структура проекта

```
sm-post/
├── CQRS-ES/                          # Общая CQRS библиотека
│   └── CQRS.Core/
│       ├── Commands/                 # Базовые классы команд
│       ├── Events/                   # Базовые классы событий
│       ├── Queries/                  # Базовые классы запросов
│       └── Domain/                   # Базовые domain классы
│
├── SM-Post/
│   ├── Post.Common/                  # Общие классы
│   │   ├── Events/                   # Все события приложения
│   │   │   ├── PostCreatedEvent.cs
│   │   │   ├── MessageUpdatedEvent.cs
│   │   │   ├── PostLikedEvent.cs
│   │   │   ├── CommentAddedEvent.cs
│   │   │   └── ...
│   │   └── DTOs/                     # Data Transfer Objects
│   │
│   ├── Post.Cmd/                     # Command Side (Запись)
│   │   ├── Post.Cmd.Api/             # API для команд
│   │   │   ├── Features/             # Vertical Slices
│   │   │   │   ├── Posts/
│   │   │   │   │   ├── CreatePost/
│   │   │   │   │   │   ├── CreatePostCommand.cs
│   │   │   │   │   │   ├── CreatePostHandler.cs
│   │   │   │   │   │   └── CreatePostEndpoint.cs
│   │   │   │   │   ├── EditMessage/
│   │   │   │   │   ├── LikePost/
│   │   │   │   │   └── DeletePost/
│   │   │   │   ├── Comments/
│   │   │   │   │   ├── AddComment/
│   │   │   │   │   ├── EditComment/
│   │   │   │   │   └── RemoveComment/
│   │   │   │   └── Admin/
│   │   │   │       └── RestoreReadDb/
│   │   │   └── Program.cs
│   │   │
│   │   ├── Post.Cmd.Domain/          # Бизнес-логика
│   │   │   └── Aggregates/
│   │   │       └── PostAggregate.cs  # Агрегат поста
│   │   │
│   │   └── Post.Cmd.Infrastructure/  # Инфраструктура
│   │       ├── Stores/
│   │       │   └── EventStore.cs     # Работа с Marten
│   │       ├── Producers/
│   │       │   └── EventProducer.cs  # Kafka Producer
│   │       └── Repositories/
│   │           └── EventStoreRepository.cs
│   │
│   └── Post.Query/                   # Query Side (Чтение)
│       ├── Post.Query.Api/           # API для запросов
│       │   ├── Features/             # Vertical Slices
│       │   │   └── Posts/
│       │   │       ├── GetAllPosts/
│       │   │       │   ├── GetAllPostsQuery.cs
│       │   │       │   ├── GetAllPostsHandler.cs
│       │   │       │   └── GetAllPostsEndpoint.cs
│       │   │       ├── GetPostById/
│       │   │       ├── GetPostsByAuthor/
│       │   │       ├── GetPostsWithComments/
│       │   │       └── GetPostsWithLikes/
│       │   └── Program.cs
│       │
│       ├── Post.Query.Domain/        # Domain модели
│       │   ├── Entities/
│       │   │   ├── PostEntity.cs
│       │   │   └── CommentEntity.cs
│       │   └── Repositories/
│       │       ├── IPostRepository.cs
│       │       └── ICommentRepository.cs
│       │
│       └── Post.Query.Infrastructure/ # Инфраструктура
│           ├── DataAccess/
│           │   └── DatabaseContext.cs # EF Core DbContext
│           ├── Repositories/
│           │   ├── PostRepository.cs
│           │   └── CommentRepository.cs
│           ├── Consumers/
│           │   ├── EventConsumer.cs   # Kafka Consumer
│           │   └── ConsumerHostedService.cs
│           └── Handlers/
│               └── Notifications/     # MediatR Handlers
│                   ├── PostCreatedEventHandler.cs
│                   ├── MessageUpdatedEventHandler.cs
│                   └── ...
│
├── client/                           # Next.js Frontend
│   ├── app/                          # App Router
│   │   ├── page.tsx                  # Главная страница
│   │   └── layout.tsx
│   ├── components/                   # React компоненты
│   │   ├── posts/
│   │   │   ├── post-list.tsx
│   │   │   ├── post-card.tsx
│   │   │   └── create-post-form.tsx
│   │   └── ui/                       # shadcn/ui компоненты
│   ├── lib/
│   │   └── api.ts                    # API клиент
│   └── package.json
│
├── Infra/                            # Инфраструктура
│   ├── docker-compose.yml            # Docker Compose конфигурация
│   ├── Dockerfile.cmd                # Dockerfile для Command API
│   ├── Dockerfile.query              # Dockerfile для Query API
│   ├── Dockerfile.client             # Dockerfile для Frontend
│   └── test-service.sh               # Скрипт тестирования
│
└── k8s/                              # Kubernetes манифесты
    ├── namespace.yaml
    ├── postgres-marten.yaml
    ├── postgres-read.yaml
    ├── kafka.yaml
    ├── kafka-ui.yaml
    ├── post-cmd-api.yaml             # 2 реплики
    ├── post-query-api.yaml           # 2 реплики
    ├── client.yaml                   # 2 реплики
    ├── deploy-all.sh
    └── cleanup.sh
```

---

## 🎓 Ключевые паттерны и концепции

### 1. Aggregate Root (Корневой агрегат)

```csharp
public abstract class AggregateRoot
{
    protected Guid _id;
    private readonly List<BaseEvent> _changes = new();

    public Guid Id => _id;
    public int Version { get; set; } = -1;

    public IEnumerable<BaseEvent> GetUncommittedChanges() => _changes;

    public void MarkChangesAsCommitted() => _changes.Clear();

    protected void RaiseEvent(BaseEvent @event)
    {
        ApplyChange(@event, true);
    }

    public void ReplayEvents(IEnumerable<BaseEvent> events)
    {
        foreach (var @event in events)
        {
            ApplyChange(@event, false);
        }
    }
}
```

**Зачем?**
- Обеспечивает **консистентность** бизнес-логики
- Сохраняет **историю изменений** через события
- Позволяет **восстановить состояние** из событий

### 2. MediatR Pattern (Посредник)

```csharp
// Вместо прямых вызовов:
var post = await postService.GetPostById(id);

// Используем MediatR:
var post = await _mediator.Send(new GetPostByIdQuery { Id = id });
```

**Преимущества:**
- ✅ Слабая связанность (loose coupling)
- ✅ Легче тестировать
- ✅ Централизованная обработка (можем добавить логирование, валидацию)

### 3. Repository Pattern (Репозиторий)

```csharp
public interface IPostRepository
{
    Task CreateAsync(PostEntity post);
    Task<PostEntity> GetByIdAsync(Guid postId);
    Task<List<PostEntity>> ListAllAsync();
    Task UpdateAsync(PostEntity post);
    Task DeleteAsync(Guid postId);
}
```

**Зачем?**
- Абстракция над доступом к данным
- Легко заменить PostgreSQL на другую БД
- Упрощает unit-тестирование (можем использовать mock)

### 4. Eventual Consistency (Конечная согласованность)

```
Время →

Command API: [Событие сохранено] ────────────────────────┐
                                                          ↓
Kafka:       ─────[Событие в очереди]──────────────────→ │
                                                          ↓
Query API:   ─────────────────[Событие обработано]───────→ [DB обновлена]
                                ↑
                                │
                         Небольшая задержка (обычно < 1 сек)
```

**Важно понимать:**
- После отправки команды данные **не сразу** доступны для чтения
- Обычная задержка: **100-500 мс**
- Это **нормально** для CQRS систем
- Frontend должен показывать оптимистичные обновления

---

## 🔍 Примеры использования

### Создание поста

**Request:**
```bash
POST http://localhost:5262/api/posts/new
Content-Type: application/json

{
  "author": "Иван Петров",
  "message": "Привет! Это мой первый пост в системе CQRS!"
}
```

**Response:**
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "message": "Post created successfully"
}
```

**Что происходит внутри:**
1. Carter Endpoint получает запрос
2. Создается `CreatePostCommand`
3. MediatR отправляет в `CreatePostHandler`
4. Handler создает `PostAggregate`
5. Aggregate генерирует `PostCreatedEvent`
6. Событие сохраняется в Marten (Event Store)
7. Событие публикуется в Kafka
8. Query API получает событие и обновляет Read DB

### Получение всех постов

**Request:**
```bash
GET http://localhost:5263/api/posts
```

**Response:**
```json
[
  {
    "postId": "550e8400-e29b-41d4-a716-446655440000",
    "author": "Иван Петров",
    "message": "Привет! Это мой первый пост в системе CQRS!",
    "datePosted": "2024-11-28T10:30:00Z",
    "likes": 5,
    "comments": [
      {
        "commentId": "660f9511-f3ac-52e5-b827-557766551111",
        "username": "Мария",
        "comment": "Отличный пост!",
        "commentDate": "2024-11-28T10:35:00Z"
      }
    ]
  }
]
```

### Восстановление Read DB из событий

**Request:**
```bash
POST http://localhost:5262/api/restore-read-db
```

**Что происходит:**
1. Удаляются все данные из Read DB
2. Читаются ВСЕ события из Event Store
3. События по порядку применяются к пустой Read DB
4. Read DB восстанавливается в актуальное состояние

**Зачем это нужно?**
- 🐛 Восстановление после сбоя
- 🔄 Изменение схемы Read DB
- 📊 Создание новых представлений данных
- 🧪 Тестирование

---

## 🧪 Тестирование

### Unit тесты (пример)

```csharp
[Fact]
public void CreatePost_WithValidData_ShouldRaisePostCreatedEvent()
{
    // Arrange
    var aggregate = new PostAggregate();
    var author = "Test Author";
    var message = "Test Message";

    // Act
    aggregate.CreatePost(author, message);

    // Assert
    var events = aggregate.GetUncommittedChanges();
    Assert.Single(events);
    Assert.IsType<PostCreatedEvent>(events.First());
}
```

### Integration тесты

```bash
# Запуск всех сервисов
docker-compose up -d

# Запуск автоматических тестов
./test-service.sh

# Ожидаемый результат:
# ✓ POST creation
# ✓ POST edit
# ✓ POST like
# ✓ Comment add
# ✓ Query by ID
# ✓ POST query
# ✓ POST delete
# ✓ Verify deletion
```

---

## 🚨 Типичные проблемы и решения

### 1. "Я создал пост, но не вижу его в списке"

**Причина:** Eventual consistency - событие еще не обработалось

**Решение:** 
- Подождите 1-2 секунды
- Реализуйте polling на фронтенде
- Показывайте оптимистичные обновления

### 2. "Kafka контейнер постоянно перезапускается"

**Причина:** Старые данные в volume

**Решение:**
```bash
docker-compose down -v  # Удалит volumes
docker-compose up -d
```

### 3. "DateTime ошибка в PostgreSQL"

**Причина:** PostgreSQL требует UTC время

**Решение:** Всегда конвертируйте в UTC:
```csharp
DatePosted = notification.DatePosted.ToUniversalTime()
```

### 4. "CORS ошибка при запросе с фронтенда"

**Причина:** API не разрешает запросы с localhost:3000

**Решение:** Проверьте `Program.cs`:
```csharp
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
```

---

## 📚 Дополнительные ресурсы

### Документация
- [CQRS Pattern](https://martinfowler.com/bliki/CQRS.html) - Martin Fowler
- [Event Sourcing](https://martinfowler.com/eaaDev/EventSourcing.html) - Martin Fowler
- [Vertical Slice Architecture](https://www.jimmybogard.com/vertical-slice-architecture/) - Jimmy Bogard
- [Marten Documentation](https://martendb.io/) - Event Store на PostgreSQL
- [MediatR](https://github.com/jbogard/MediatR) - Mediator pattern для .NET

### Похожие проекты
- [eShopOnContainers](https://github.com/dotnet-architecture/eShopOnContainers) - Microsoft
- [Practical Event Sourcing](https://github.com/oskardudycz/EventSourcing.NetCore) - Oskar Dudycz

---

## 🤝 Вклад в проект

Это учебный проект. Если вы нашли ошибку или хотите улучшить код:

1. Fork репозиторий
2. Создайте feature branch (`git checkout -b feature/amazing-feature`)
3. Commit изменения (`git commit -m 'Add amazing feature'`)
4. Push в branch (`git push origin feature/amazing-feature`)
5. Откройте Pull Request

---

## 📝 Лицензия

Этот проект создан в учебных целях.

---

## 👨‍💻 Автор

Создано для изучения современных архитектурных паттернов в .NET

---

## ❓ FAQ (Частые вопросы)

### Q: Зачем так сложно? Можно же просто REST API + PostgreSQL?

**A:** Да, для простого CRUD приложения это избыточно. Но этот проект показывает:
- Как строить **масштабируемые** системы
- Как сохранять **полную историю** изменений
- Как **разделять** нагрузку на чтение и запись
- Современные паттерны для **enterprise** приложений

### Q: Какая задержка между Command и Query?

**A:** Обычно **100-500 мс**. Зависит от:
- Производительности Kafka
- Скорости обработки событий
- Нагрузки на систему

### Q: Можно ли использовать это в production?

**A:** Да, но нужно добавить:
- ✅ Аутентификацию и авторизацию
- ✅ Логирование и мониторинг
- ✅ Error handling и retry policies
- ✅ Rate limiting
- ✅ Health checks
- ✅ Тесты (unit, integration, load)

### Q: Сколько событий можно хранить?

**A:** Неограниченно! Но для оптимизации используют:
- **Snapshots** - сохранение состояния агрегата каждые N событий
- **Archiving** - перенос старых событий в холодное хранилище
- **Partitioning** - разделение событий по времени/типу

### Q: Что если Query API упадет?

**A:** События сохраняются в Kafka, поэтому:
1. После перезапуска Query API продолжит обработку
2. Kafka хранит события согласно настройкам retention
3. Можно восстановить Read DB из Event Store командой `/restore-read-db`

---

**Удачи в изучении CQRS и Event Sourcing! 🚀**




# ... existing content ...

---

## 🏗️ Создание CQRS приложения с нуля: Пошаговое руководство

Если вы хотите создать подобное приложение самостоятельно, следуйте этому порядку. Это проверенный путь, который минимизирует проблемы и позволяет тестировать на каждом этапе.

---

### Этап 1: Подготовка базовой инфраструктуры (Day 1) 🏗️

#### Шаг 1.1: Создание solution и базовых проектов

```bash
# Создаем папку проекта
mkdir social-media-cqrs
cd social-media-cqrs

# Создаем solution
dotnet new sln -n SocialMedia

# Создаем базовую библиотеку CQRS
mkdir -p CQRS-ES/CQRS.Core
cd CQRS-ES/CQRS.Core
dotnet new classlib -f net8.0

# Добавляем в solution
cd ../..
dotnet sln add CQRS-ES/CQRS.Core/CQRS.Core.csproj
```

#### Шаг 1.2: Создание базовых абстракций CQRS

**Почему начинаем с этого?** 
- Это фундамент для всего приложения
- Определяем контракты, которые будут использоваться везде
- Без этих абстракций не получится правильно разделить Command и Query

**Создаем файлы:**

1. **Domain/AggregateRoot.cs** - базовый класс для агрегатов
```csharp
namespace CQRS.Core.Domain;

public abstract class AggregateRoot
{
    protected Guid _id;
    private readonly List<BaseEvent> _changes = new();

    public Guid Id => _id;
    public int Version { get; set; } = -1;

    public IEnumerable<BaseEvent> GetUncommittedChanges() => _changes;
    
    public void MarkChangesAsCommitted() => _changes.Clear();

    protected void RaiseEvent(BaseEvent @event)
    {
        ApplyChange(@event, true);
    }

    private void ApplyChange(BaseEvent @event, bool isNew)
    {
        var method = GetType().GetMethod("Apply", new[] { @event.GetType() });
        method?.Invoke(this, new object[] { @event });
        
        if (isNew) _changes.Add(@event);
    }

    public void ReplayEvents(IEnumerable<BaseEvent> events)
    {
        foreach (var @event in events)
            ApplyChange(@event, false);
    }
}
```

2. **Events/BaseEvent.cs** - базовый класс событий
3. **Commands/BaseCommand.cs** - базовый класс команд
4. **Queries/BaseQuery.cs** - базовый класс запросов

**✅ Checkpoint:** Библиотека CQRS.Core компилируется без ошибок

---

### Этап 2: Domain слой - Бизнес-логика (Day 2) 📋

#### Шаг 2.1: Создание проектов для Post

```bash
# Общая библиотека с событиями
mkdir -p SM-Post/Post.Common
cd SM-Post/Post.Common
dotnet new classlib -f net8.0
dotnet add reference ../../CQRS-ES/CQRS.Core/CQRS.Core.csproj

# Domain для Command
mkdir -p ../Post.Cmd/Post.Cmd.Domain
cd ../Post.Cmd/Post.Cmd.Domain
dotnet new classlib -f net8.0
dotnet add reference ../../Post.Common/Post.Common.csproj
```

#### Шаг 2.2: Определение событий

**Почему сейчас?** 
- События - это контракт между Command и Query
- Определяем их до реализации, чтобы обе стороны знали структуру
- Это ваш "язык бизнеса"

**Post.Common/Events/PostCreatedEvent.cs:**
```csharp
namespace Post.Common.Events;

public class PostCreatedEvent : BaseEvent
{
    public required string Author { get; set; }
    public required string Message { get; set; }
    public DateTime DatePosted { get; set; }
}
```

**Создаем все события:**
- PostCreatedEvent
- MessageUpdatedEvent
- PostLikedEvent
- CommentAddedEvent
- CommentUpdatedEvent
- CommentRemovedEvent
- PostRemovedEvent

#### Шаг 2.3: Создание Aggregate

**Post.Cmd.Domain/Aggregates/PostAggregate.cs:**
```csharp
public class PostAggregate : AggregateRoot
{
    private bool _active;
    private string _author = string.Empty;
    private readonly Dictionary<Guid, Tuple<string, string>> _comments = new();

    public PostAggregate() { }

    public PostAggregate(Guid id, string author, string message)
    {
        RaiseEvent(new PostCreatedEvent
        {
            Id = id,
            Author = author,
            Message = message,
            DatePosted = DateTime.UtcNow
        });
    }

    public void Apply(PostCreatedEvent @event)
    {
        _id = @event.Id;
        _active = true;
        _author = @event.Author;
    }

    // Методы для других команд: EditMessage, LikePost, AddComment...
}
```

**✅ Checkpoint:** 
- Domain логика написана
- События определены
- Aggregate компилируется

**🧪 Тест:** Напишите unit тест для PostAggregate

---

### Этап 3: Event Store - Хранение событий (Day 3) 💾

#### Шаг 3.1: Docker Compose для инфраструктуры

**Почему сейчас?** 
- Нужна база для хранения событий
- Docker изолирует зависимости
- Можем тестировать реальное сохранение

**Infra/docker-compose.yml:**
```yaml
version: '3.8'

services:
  postgres-marten:
    image: postgres:16
    environment:
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: postgres
      POSTGRES_DB: postdb
    ports:
      - "5433:5432"
    volumes:
      - postgres_marten_data:/var/lib/postgresql/data

volumes:
  postgres_marten_data:
```

**Запуск:**
```bash
cd Infra
docker-compose up -d
```

#### Шаг 3.2: Подключение Marten

**Post.Cmd.Infrastructure:**
```bash
mkdir -p SM-Post/Post.Cmd/Post.Cmd.Infrastructure
cd SM-Post/Post.Cmd/Post.Cmd.Infrastructure
dotnet new classlib -f net8.0
dotnet add package Marten --version 7.31.2
```

**Создание EventStoreRepository:**
```csharp
public class EventStoreRepository : IEventStoreRepository
{
    private readonly IDocumentStore _store;

    public async Task SaveEventsAsync(Guid aggregateId, 
                                       IEnumerable<BaseEvent> events, 
                                       int expectedVersion)
    {
        await using var session = _store.LightweightSession();
        
        foreach (var @event in events)
        {
            var eventModel = new EventModel
            {
                AggregateIdentifier = aggregateId,
                AggregateType = nameof(PostAggregate),
                Version = expectedVersion++,
                EventType = @event.GetType().Name,
                EventData = JsonSerializer.Serialize(@event)
            };
            
            session.Store(eventModel);
        }
        
        await session.SaveChangesAsync();
    }
}
```

**✅ Checkpoint:** 
- События сохраняются в Marten
- Можем читать историю событий
- Aggregate восстанавливается из событий

**🧪 Тест:** Сохраните событие и прочитайте его обратно

---

### Этап 4: Command API - Первый endpoint (Day 4) 🚀

#### Шаг 4.1: Создание Web API проекта

```bash
mkdir -p SM-Post/Post.Cmd/Post.Cmd.Api
cd SM-Post/Post.Cmd/Post.Cmd.Api
dotnet new webapi -f net8.0
dotnet add package MediatR --version 12.4.1
dotnet add package Carter --version 8.2.1
```

#### Шаг 4.2: Первая команда - CreatePost

**Почему начинаем с CreatePost?**
- Самая простая команда (нет зависимостей)
- Проверяем всю цепочку: API → Handler → Aggregate → EventStore
- Можем сразу протестировать

**Features/Posts/CreatePost/CreatePostCommand.cs:**
```csharp
public record CreatePostCommand(string Author, string Message) 
    : IRequest<Unit>;
```

**CreatePostHandler.cs:**
```csharp
public class CreatePostHandler : IRequestHandler<CreatePostCommand, Unit>
{
    private readonly IEventStore _eventStore;

    public async Task<Unit> Handle(CreatePostCommand request, 
                                    CancellationToken cancellationToken)
    {
        var aggregate = new PostAggregate(
            Guid.NewGuid(), 
            request.Author, 
            request.Message
        );
        
        await _eventStore.SaveEventsAsync(
            aggregate.Id, 
            aggregate.GetUncommittedChanges(), 
            -1
        );
        
        return Unit.Value;
    }
}
```

**CreatePostEndpoint.cs (Carter):**
```csharp
public class CreatePostEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/posts/new", async (
            CreatePostCommand command,
            IMediator mediator) =>
        {
            await mediator.Send(command);
            return Results.Created();
        });
    }
}
```

**Program.cs:**
```csharp
var builder = WebApplication.CreateBuilder(args);

// Marten Event Store
builder.Services.AddMarten(options =>
{
    options.Connection(builder.Configuration.GetConnectionString("Marten")!);
    options.AutoCreateSchemaObjects = AutoCreate.All;
});

// MediatR
builder.Services.AddMediatR(cfg => 
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

// Carter
builder.Services.AddCarter();

var app = builder.Build();

app.MapCarter();
app.Run();
```

**✅ Checkpoint:** 
```bash
# Запускаем API
dotnet run

# Тестируем
curl -X POST http://localhost:5262/api/posts/new \
  -H "Content-Type: application/json" \
  -d '{"author":"Test","message":"Hello CQRS!"}'

# Проверяем в БД
docker exec -it postgres-marten psql -U postgres -d postdb
SELECT * FROM mt_events;
```

**🎉 Первая веха:** Command API работает и сохраняет события!

---

### Этап 5: Kafka - Event Bus (Day 5) 📨

#### Шаг 5.1: Добавляем Kafka в Docker Compose

**Почему сейчас?**
- События сохраняются, теперь нужно их публиковать
- Kafka - связующее звено между Command и Query
- Без Kafka Query не узнает о событиях

```yaml
kafka:
  image: apache/kafka:3.7.0
  environment:
    KAFKA_NODE_ID: 1
    KAFKA_PROCESS_ROLES: 'broker,controller'
    KAFKA_LISTENERS: 'PLAINTEXT://kafka:9092,CONTROLLER://kafka:9094'
    KAFKA_ADVERTISED_LISTENERS: 'PLAINTEXT://kafka:9092'
    KAFKA_CONTROLLER_QUORUM_VOTERS: '1@kafka:9094'
  ports:
    - "9092:9092"
```

#### Шаг 5.2: Kafka Producer в Command API

```bash
dotnet add package Confluent.Kafka --version 2.3.0
```

**EventProducer.cs:**
```csharp
public class EventProducer : IEventProducer
{
    private readonly IProducer<string, string> _producer;

    public async Task ProduceAsync<T>(string topic, T @event) where T : BaseEvent
    {
        var message = new Message<string, string>
        {
            Key = @event.Id.ToString(),
            Value = JsonSerializer.Serialize(@event, @event.GetType())
        };

        await _producer.ProduceAsync(topic, message);
    }
}
```

**Обновляем EventStoreRepository:**
```csharp
public async Task SaveEventsAsync(...)
{
    // ... сохранение в Marten ...
    
    // Публикуем в Kafka
    foreach (var @event in events)
    {
        await _eventProducer.ProduceAsync("SocialMediaPostEvents", @event);
    }
}
```

**✅ Checkpoint:**
- Kafka запущен
- События публикуются после сохранения
- Можем увидеть события через Kafka UI

---

### Этап 6: Query API - Read Side (Day 6-7) 📖

#### Шаг 6.1: Создание Query проектов

```bash
# Query Domain
mkdir -p SM-Post/Post.Query/Post.Query.Domain
cd SM-Post/Post.Query/Post.Query.Domain
dotnet new classlib -f net8.0

# Query Infrastructure
mkdir -p ../Post.Query.Infrastructure
cd ../Post.Query.Infrastructure
dotnet new classlib -f net8.0
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add package Confluent.Kafka

# Query API
mkdir -p ../Post.Query.Api
cd ../Post.Query.Api
dotnet new webapi -f net8.0
```

#### Шаг 6.2: Entity Framework модели

**Почему Entity Framework?**
- Query DB оптимизирована для чтения
- Денормализованные данные (JOIN-ы уже сделаны)
- Обычная реляционная модель

**Post.Query.Domain/Entities/PostEntity.cs:**
```csharp
public class PostEntity
{
    public Guid PostId { get; set; }
    public required string Author { get; set; }
    public required string Message { get; set; }
    public DateTime DatePosted { get; set; }
    public int Likes { get; set; }
    public ICollection<CommentEntity> Comments { get; set; } = new List<CommentEntity>();
}
```

#### Шаг 6.3: Kafka Consumer

**Почему сложный?**
- Работает в фоне (BackgroundService)
- Слушает события 24/7
- Обновляет Read DB при получении событий

**EventConsumer.cs:**
```csharp
public class EventConsumer : IEventConsumer
{
    private readonly IConsumer<string, string> _consumer;
    private readonly IMediator _mediator;

    public void Consume(string topic)
    {
        _consumer.Subscribe(topic);

        while (true)
        {
            var consumeResult = _consumer.Consume();
            
            var eventType = Type.GetType(
                $"Post.Common.Events.{consumeResult.Message.Headers...}"
            );
            
            var @event = JsonSerializer.Deserialize(
                consumeResult.Message.Value, 
                eventType
            );
            
            await _mediator.Publish(@event); // MediatR Notification!
        }
    }
}
```

#### Шаг 6.4: Event Handlers (MediatR Notifications)

**PostCreatedEventHandler.cs:**
```csharp
public class PostCreatedEventHandler 
    : INotificationHandler<PostCreatedEvent>
{
    private readonly IPostRepository _postRepository;

    public async Task Handle(PostCreatedEvent notification, 
                             CancellationToken cancellationToken)
    {
        var post = new PostEntity
        {
            PostId = notification.Id,
            Author = notification.Author,
            Message = notification.Message,
            DatePosted = notification.DatePosted.ToUniversalTime()
        };

        await _postRepository.CreateAsync(post);
    }
}
```

**Создаем handlers для всех событий:**
- PostCreatedEventHandler ✅
- MessageUpdatedEventHandler
- PostLikedEventHandler
- CommentAddedEventHandler
- etc.

#### Шаг 6.5: Query Endpoints

**Features/Posts/GetAllPosts/GetAllPostsQuery.cs:**
```csharp
public record GetAllPostsQuery : IRequest<List<PostEntity>>;
```

**GetAllPostsHandler.cs:**
```csharp
public class GetAllPostsHandler 
    : IRequestHandler<GetAllPostsQuery, List<PostEntity>>
{
    private readonly IPostRepository _postRepository;

    public async Task<List<PostEntity>> Handle(
        GetAllPostsQuery request, 
        CancellationToken cancellationToken)
    {
        return await _postRepository.ListAllAsync();
    }
}
```

**GetAllPostsEndpoint.cs:**
```csharp
public class GetAllPostsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/posts", async (IMediator mediator) =>
        {
            var posts = await mediator.Send(new GetAllPostsQuery());
            return posts.Any() ? Results.Ok(posts) : Results.NoContent();
        });
    }
}
```

**✅ Checkpoint:**
```bash
# Терминал 1: Command API
cd SM-Post/Post.Cmd/Post.Cmd.Api
dotnet run

# Терминал 2: Query API
cd SM-Post/Post.Query/Post.Query.Api
dotnet run

# Терминал 3: Создаем пост
curl -X POST http://localhost:5262/api/posts/new \
  -H "Content-Type: application/json" \
  -d '{"author":"Test","message":"Hello CQRS!"}'

# Ждем 1-2 секунды (eventual consistency)

# Читаем пост
curl http://localhost:5263/api/posts
```

**🎉 Вторая веха:** CQRS работает end-to-end!

---

### Этап 7: Остальные команды (Day 8-9) ⚡

Теперь, когда основа готова, добавляем остальные команды **по той же схеме**:

#### Приоритет команд:

1. **EditMessage** - изменение текста поста
   - Простая, нет зависимостей
   - Проверяем что aggregate может изменяться

2. **LikePost** - простой счетчик
   - Учимся работать с числовыми полями
   - Проверяем обновление в Query DB

3. **AddComment** - добавление связанной сущности
   - Более сложная логика
   - Новая таблица в Query DB

4. **EditComment**, **RemoveComment** - работа с вложенными сущностями

5. **DeletePost** - soft delete
   - Флаг active: false
   - Событие не удаляет, а помечает

**Для каждой команды делаем:**
```
1. Событие в Post.Common/Events/
2. Метод в PostAggregate с Apply()
3. Command + Handler + Endpoint в Command API
4. NotificationHandler в Query API
5. Тест через curl
```

---

### Этап 8: Frontend (Day 10-11) 🎨

#### Шаг 8.1: Создание Next.js приложения

```bash
npx create-next-app@latest client
cd client
npx shadcn@latest init
npx shadcn@latest add button card form input textarea
npm install @tanstack/react-query axios
```

#### Шаг 8.2: API Client

**lib/api.ts:**
```typescript
const COMMAND_API = 'http://localhost:5262';
const QUERY_API = 'http://localhost:5263';

export const api = {
  // Commands
  createPost: (data: CreatePostData) =>
    axios.post(`${COMMAND_API}/api/posts/new`, data),
  
  // Queries
  getAllPosts: () =>
    axios.get(`${QUERY_API}/api/posts`).then(res => res.data),
};
```

#### Шаг 8.3: Компоненты

Создаем по порядку:
1. **PostList** - список постов (читаем данные)
2. **CreatePostForm** - форма создания (отправляем команды)
3. **PostCard** - карточка поста с кнопками
4. **CommentList** - список комментариев
5. **AddCommentForm** - добавление комментария

**✅ Checkpoint:** UI работает, можем создавать посты через браузер

---

### Этап 9: Dockerization (Day 12) 🐳

#### Шаг 9.1: Dockerfile для каждого API

**Infra/Dockerfile.cmd:**
```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore SM-Post/Post.Cmd/Post.Cmd.Api/Post.Cmd.Api.csproj
RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "Post.Cmd.Api.dll"]
```

#### Шаг 9.2: Обновление docker-compose

Добавляем сервисы приложений:
```yaml
post-cmd-api:
  build:
    context: ..
    dockerfile: Infra/Dockerfile.cmd
  ports:
    - "5262:8080"
  depends_on:
    - postgres-marten
    - kafka
```

**✅ Checkpoint:**
```bash
docker-compose up -d
# Все работает в контейнерах!
```

---

### Этап 10: Kubernetes (Day 13-14) ☸️

#### Шаг 10.1: Создание манифестов

Порядок создания:
1. **namespace.yaml** - изоляция
2. **postgres-*.yaml** - stateful сервисы с PVC
3. **kafka.yaml** - event bus
4. **post-cmd-api.yaml** - 2 реплики (отказоустойчивость!)
5. **post-query-api.yaml** - 2 реплики
6. **client.yaml** - 2 реплики

**Почему 2 реплики?**
- Отказоустойчивость (один под упал - второй работает)
- Load balancing автоматически
- Zero-downtime deployments

#### Шаг 10.2: Скрипты деплоя

**k8s/deploy-all.sh:**
```bash
#!/bin/bash
kubectl apply -f namespace.yaml
kubectl apply -f postgres-marten.yaml
kubectl apply -f postgres-read.yaml
kubectl apply -f kafka.yaml
sleep 10  # ждем БД
kubectl apply -f post-cmd-api.yaml
kubectl apply -f post-query-api.yaml
kubectl apply -f client.yaml
kubectl apply -f kafka-ui.yaml
```

**✅ Checkpoint:**
```bash
./deploy-all.sh
kubectl get pods -n cqrs-app
# Все поды Running!
```

---

### Этап 11: Тестирование и документация (Day 15) 📝

#### Шаг 11.1: Автоматические тесты

**test-service.sh:**
```bash
#!/bin/bash

# 1. Create Post
POST_ID=$(curl -X POST http://localhost:5262/api/posts/new ...)

# 2. Edit Message
curl -X PUT http://localhost:5262/api/posts/edit-message ...

# 3. Like Post
curl -X PUT http://localhost:5262/api/posts/like ...

# 4. Add Comment
curl -X POST http://localhost:5262/api/posts/add-comment ...

# 5. Query Posts
curl http://localhost:5263/api/posts

# 6. Delete Post
curl -X DELETE http://localhost:5262/api/posts/$POST_ID

# 7. Verify deletion
curl http://localhost:5263/api/posts/$POST_ID
```

#### Шаг 11.2: README.md

Документируем:
- Что такое CQRS и Event Sourcing
- Архитектура приложения
- Как запустить
- API endpoints
- Troubleshooting

---

## 📊 Временная шкала проекта

```
Week 1: Foundation
├── Day 1-2:  CQRS Core + Domain ✅
├── Day 3:    Event Store (Marten) ✅
├── Day 4:    Command API (первый endpoint) ✅
└── Day 5:    Kafka integration ✅

Week 2: Implementation
├── Day 6-7:  Query API + Consumers ✅
├── Day 8-9:  Остальные команды ✅
└── Day 10-11: Frontend (Next.js) ✅

Week 3: Production Ready
├── Day 12:   Docker Compose ✅
├── Day 13-14: Kubernetes ✅
└── Day 15:   Testing + Documentation ✅
```

**Total: ~3 недели** для полнофункционального CQRS приложения

---

## ⚠️ Частые ошибки новичков

### ❌ Ошибка 1: Начинают с UI

**Проблема:** Непонятно как работает CQRS
**Решение:** Начинайте с backend, тестируйте через curl

### ❌ Ошибка 2: Сразу делают все команды

**Проблема:** Много кода, сложно отлаживать
**Решение:** Делайте по одной команде, тестируйте каждую

### ❌ Ошибка 3: Пропускают Kafka

**Проблема:** Command и Query не синхронизированы
**Решение:** Kafka обязателен для CQRS

### ❌ Ошибка 4: Забывают про eventual consistency

**Проблема:** "Я создал пост, но не вижу его!"
**Решение:** Добавляйте задержки, показывайте loading

### ❌ Ошибка 5: Сразу в Kubernetes

**Проблема:** Сложно отлаживать
**Решение:** Сначала Docker Compose, потом K8s

---

## 🎯 Чеклист готовности к следующему этапу

### ✅ После Этапа 1 (CQRS Core):
- [ ] Компилируется без ошибок
- [ ] Понимаю что такое Aggregate, Event, Command
- [ ] Могу объяснить паттерн Event Sourcing

### ✅ После Этапа 4 (Command API):
- [ ] POST запрос создает пост
- [ ] События сохраняются в Marten
- [ ] Могу увидеть события в БД
- [ ] Aggregate восстанавливается из событий

### ✅ После Этапа 6 (Query API):
- [ ] Создаю пост через Command API
- [ ] Вижу его через Query API
- [ ] Понимаю eventual consistency
- [ ] Kafka доставляет события

### ✅ После Этапа 8 (Frontend):
- [ ] UI работает в браузере
- [ ] Могу создавать посты через форму
- [ ] Вижу обновления в реальном времени
- [ ] Понимаю как работает polling

### ✅ После Этапа 10 (Kubernetes):
- [ ] Все поды Running
- [ ] 2 реплики каждого API
- [ ] Load balancing работает
- [ ] Могу убить под - система продолжает работать

---

## 🚀 Следующие шаги после завершения

1. **Добавить аутентификацию** (JWT)
2. **Реализовать Saga Pattern** для сложных бизнес-процессов
3. **Добавить Outbox Pattern** для гарантированной доставки
4. **Snapshot-ы** для оптимизации восстановления агрегатов
5. **Monitoring** (Prometheus + Grafana)
6. **Distributed Tracing** (OpenTelemetry)

---

**Главный совет:** Не спешите! Делайте по шагам, тестируйте каждый этап. CQRS сложнее обычного CRUD, но в итоге вы получите масштабируемую и гибкую архитектуру. 🎓
