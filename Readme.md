# Telegram Subscription Manager

## Описание проекта
Этот проект представляет собой Telegram-бота для управления подписками на различные сервисы. Пользователь может подписываться на сервисы, изменять текущие подписки и управлять ими.

## Технологический стек
- .NET 8.0
- ASP.NET Core
- EF Core
- PostgreSQL
- Docker
- ngrook

## Запуск проекта

### С использованием Docker
1. Клонируйте репозиторий:
   ```bash
   git clone <repository_url>
2. Перейдите в каталог проекта:
```bash
	cd <project_directory>

```
3.Соберите и запустите контейнеры:
```bash
	
	docker-compose up --build
```

## Запуск тестов

```bash
	dotnet test
```