# ReactBlog

Платформа для ведения блогов с фронтендом на React и бэкендом на ASP.NET Core. Пользователи могут публиковать записи, находить авторов, просматривать их профили и подписываться друг на друга.

## Возможности

- регистрация и вход через Keycloak по OpenID Connect;
- создание, просмотр, редактирование и удаление собственных записей;
- просмотр профилей и публикаций других авторов;
- страница **Discover** со списком пользователей и переходом в их профили;
- подписка и отписка от пользователей;
- списки **Followers** и **Following** в профиле с количеством подписок;
- защита операций с записями по владельцу: пользователь не может изменить или удалить чужую публикацию;
- REST API с документацией Swagger в режиме разработки.



## Стек технологий


| Слой             | Технология                                       |
| ---------------- | ------------------------------------------------ |
| Фронтенд         | React 19, Vite                                   |
| Бэкенд           | ASP.NET Core (.NET 10), C#                       |
| Аутентификация   | Keycloak, OpenID Connect, JWT Bearer             |
| База данных      | SQLite через Entity Framework Core               |
| Тесты            | xUnit, SQLite in-memory                          |
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
2. Создайте публичный OpenID Connect client `blog-spa`: **Client authentication** выключена, **Standard flow** включён, **Valid redirect URIs** и **Valid post logout redirect URIs** — `https://localhost:52482/`*, **Web origins** — `https://localhost:52482`.
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

Операции с собственным аккаунтом, записями и подписками требуют Bearer access token. Изменение и удаление записей ограничено их владельцем.

### Аккаунт и записи


| Метод          | Маршрут       | Описание                                         | Тело                          |
| -------------- | ------------- | ------------------------------------------------ | ----------------------------- |
| `GET` / `POST` | `/me`         | Синхронизировать пользователя из токена Keycloak | —                             |
| `GET`          | `/blogs`      | Мои записи                                       | —                             |
| `GET`          | `/blogs/{id}` | Получить мою запись                              | —                             |
| `POST`         | `/blogs`      | Создать запись                                   | `name`, `content` (form-data) |
| `PUT`          | `/blogs/{id}` | Обновить запись                                  | `name`, `content` (form-data) |
| `DELETE`       | `/blogs/{id}` | Удалить запись                                   | —                             |




### Пользователи и подписки


| Метод    | Маршрут                       | Описание                                     |
| -------- | ----------------------------- | -------------------------------------------- |
| `GET`    | `/users`                      | Получить страницу пользователей для Discover |
| `GET`    | `/users/{username}`           | Получить профиль пользователя                |
| `GET`    | `/users/{username}/blogs`     | Получить публикации пользователя             |
| `GET`    | `/users/{username}/followers` | Получить подписчиков пользователя            |
| `GET`    | `/users/{username}/following` | Получить его подписки                        |
| `POST`   | `/users/{username}/follow`    | Подписаться на пользователя                  |
| `DELETE` | `/users/{username}/follow`    | Отписаться от пользователя                   |


Повторная подписка и отписка реализованы идемпотентно, а подписка на самого себя отклоняется.

## Тесты и покрытие логики

Сервисный слой покрыт автоматическими тестами на xUnit. Тесты используют настоящую SQLite в памяти, поэтому проверяют не только методы сервисов, но и запросы и ограничения Entity Framework Core.

Проверяются ключевые сценарии:

- CRUD-операции с публикациями, включая отсутствующие и некорректные идентификаторы;
- изоляция данных владельцев: чтение, изменение и удаление чужой записи запрещены;
- подписка и отписка, согласованность списков Followers и Following;
- повторная подписка без создания дубликата.

Запуск тестов из корня проекта:

```bash
dotnet test ReactBlog.Server.Tests
```

## Структура проекта

```
BlogPlatform/
├── ReactBlog.Server/       # Бэкенд на ASP.NET Core
│   ├── Controllers/        # API-контроллеры
│   ├── Data/               # DbContext, модели, DTO
│   ├── Services/           # бизнес-логика
│   └── Program.cs          # Точка входа
├── ReactBlog.Server.Tests/ # Тесты сервисного слоя
└── reactblog.client/       # Фронтенд на React + Vite
    └── src/
        ├── components/     # UI-компоненты, включая Discover и Profile
        ├── App.jsx         # Корневой компонент / маршрутизация страниц
        └── blogTools.jsx   # Функции для работы с API
```

