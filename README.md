# ReactBlog

Платформа для ведения блогов с фронтендом на React и бэкендом на ASP.NET Core. Поддерживает создание, просмотр, редактирование и удаление записей через REST API.

## Стек технологий

| Слой      | Технология                                         |
|-----------|----------------------------------------------------|
| Фронтенд  | React 18, Vite                                     |
| Бэкенд    | ASP.NET Core (.NET 10), C#                         |
| База данных | SQLite через Entity Framework Core               |
| Документация API | Swagger / OpenAPI (доступен в режиме разработки) |

## Требования

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js 18+](https://nodejs.org/) и npm

## Запуск

### 1. Установить зависимости

```bash
# Бэкенд
cd ReactBlog.Server
dotnet restore

# Фронтенд
cd ../reactblog.client
npm install
```

### 2. Доверять dev-сертификату (только при первом запуске)

```bash
dotnet dev-certs https --trust
```

### 3. Запустить

Бэкенд работает как SPA-прокси - достаточно запустить его одной командой. Vite-сервер для фронтенда стартует автоматически.

```bash
cd ReactBlog.Server
dotnet run
```

После запуска доступно:

- **Приложение** - `https://localhost:52482` (Vite dev-сервер, проксирует API на бэкенд)
- **API** - `https://localhost:7161`
- **Swagger UI** - `https://localhost:7161/swagger`

> База данных SQLite (`Blogs.db`) создаётся автоматически при первом запуске - миграции применять не нужно.

### Раздельный запуск (опционально)

Если нужно запустить фронтенд и бэкенд независимо:

```bash
# Терминал 1 — бэкенд
cd ReactBlog.Server
dotnet run

# Терминал 2 — фронтенд
cd reactblog.client
npm run dev
```

## Справочник API

| Метод    | Маршрут            | Описание                  | Тело (form-data)      |
|----------|--------------------|---------------------------|-----------------------|
| `GET`    | `/blogs`           | Список всех записей       | —                     |
| `GET`    | `/blogs/{id}`      | Получить одну запись      | —                     |
| `POST`   | `/blogs`           | Создать новую запись      | `name`, `content`     |
| `PUT`    | `/blogs/{id}`      | Обновить запись           | `name`, `content`     |
| `DELETE` | `/blogs/{id}`      | Удалить запись            | —                     |

## Структура проекта

```
BlogPlatform/
├── ReactBlog.Server/       # Бэкенд на ASP.NET Core
│   ├── Controllers/        # API-контроллеры
│   ├── Data/               # DbContext, модели, DTO
│   ├── Services/           # бизнес-логика
│   └── Program.cs          # Точка входа
└── reactblog.client/       # Фронтенд на React + Vite
    └── src/
        ├── components/     # UI-компоненты
        ├── App.jsx         # Корневой компонент / маршрутизация страниц
        └── blogTools.jsx   # Вспомогательные функции для работы с API
```
