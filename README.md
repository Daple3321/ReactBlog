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
- [Docker](https://www.docker.com/) для локального Keycloak

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

### 3. Настроить Keycloak

Запустите Keycloak:

```bash
docker compose up -d
```

Откройте `http://localhost:8080`, войдите как `admin` / `admin` и:

1. Создайте realm `blog-platform`; в **Realm settings → Login** включите **User registration**.
2. Создайте публичный OpenID Connect client `blog-spa`: **Client authentication** выключена, **Standard flow** включён, **Valid redirect URIs** и **Valid post logout redirect URIs** — `https://localhost:52482/*`, **Web origins** — `https://localhost:52482`.
3. Создайте OpenID Connect client `blog-api`: **Client authentication** включена, интерактивные flows выключены.
4. В `blog-spa` откройте **Client scopes → Dedicated scope → Add mapper → By configuration → Audience** и добавьте `blog-api` в **Included Client Audience** с **Add to access token**.

API принимает только access token с audience `blog-api`. Регистрация и пароли полностью обслуживаются Keycloak.

### 4. Запустить

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

Все маршруты требуют Bearer access token и работают только с записями текущего пользователя.

| Метод    | Маршрут            | Описание                  | Тело (form-data)      |
|----------|--------------------|---------------------------|-----------------------|
| `GET`    | `/blogs`           | Мои записи                | —                     |
| `GET`    | `/blogs/{id}`      | Получить мою запись       | —                     |
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
