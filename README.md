# ToDoList API

Простой и быстрый REST API для управления списком задач (ToDo). 
Проект написан на **.NET 10** и использует **PostgreSQL** в качестве базы данных. Приложение и база данных полностью изолированы и разворачиваются через Docker.

## 🛠 Требования для запуска
Для запуска проекта на вашем компьютере должно быть установлено:
*[Docker Desktop](https://www.docker.com/products/docker-desktop/) (или Docker Engine + Docker Compose для Linux)

## 🚀 Быстрый старт

**Шаг 1. Клонирование репозитория**
```bash
git clone https://github.com/ВАШ_НИК/ВАШ_РЕПОЗИТОРИЙ.git
cd ВАШ_РЕПОЗИТОРИЙ
```

**Шаг 2. Настройка переменных окружения**
В корне проекта (там же, где лежит docker-compose.yml) создайте файл `.env`. Вы можете скопировать настройки из примера ниже:
```env
# Содержимое файла .env
DB_USER=postgres
DB_PASSWORD=password
DB_NAME=todo_db
JWT_SECRET=MyJwtSecretAtLeast32Characters!!
```

**Шаг 3. Запуск через Docker**
Выполните команду для сборки и запуска контейнеров в фоновом режиме:
```bash
docker-compose up --build -d
```
> **Примечание:** При первом запуске база данных будет создана автоматически, а Entity Framework сам накатит все необходимые миграции.

## 💻 Как работать с приложением
После успешного запуска приложение будет доступно на порту `8001`.

### 1. Документация API (Scalar)
Самый простой способ протестировать API — открыть встроенную панель **Scalar** (аналог Swagger для .NET) в браузере:
👉 **http://localhost:8001/scalar**

### 2. Основные эндпоинты (Примеры)

**Авторизация**
* `POST /api/auth/register` — регистрация пользователя
```json
{
  "Name": "User",
  "Email": "User@example.com",
  "Password": "User123",
  "PasswordRepeat": "User123"
}
```

* `POST /api/auth/login` — вход пользователя в систему
> ⚠️ **Важно:** Роуты ниже недоступны без авторизации. Для доступа к эндпоинтам через Scalar/Postman необходимо добавить к запросу заголовок `Authorization`, указав значение `Bearer your_access_token`.
```json
{
  "Name": "User",
  "Password": "User123"
}
```

**Задачи (Tasks)**
* `POST /api/task` — Создать задачу 
```json
{
  "Title": "Task",
  "Description": "Need to do something",
  "DueDate": "2026-03-19T18:00:00Z",
  "Priority": "High"
}
```

* `GET /api/task/{id}` — Получить задачу по ID.
* `GET /api/task/all-user` — Получить все свои задачи.
* `PUT /api/task/{id}` — Обновить существующую задачу.
```json
{
  "Title": "Task updated",
  "Description": "Need to do something anyway",
  "DueDate": "2026-03-20T18:00:00Z",
  "Priority": "Medium",
  "Status": "InProgress"
}
```
* `DELETE /api/task/{id}` — Удалить задачу.

## 🛑 Остановка проекта
Чтобы остановить работу приложения и базы данных, выполните в терминале (находясь в папке проекта):
```bash
docker-compose down
```
