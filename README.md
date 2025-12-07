# RightCodeTestTask (User + Finance microservices)

## Стек
- .NET 8, C#
- Clean Architecture, CQRS (MediatR)
- EF Core (write), Dapper (read)
- PostgreSQL
- JWT auth
- Refit (HTTP-клиенты)
- YARP (API Gateway)
- Background worker для синхронизации курсов ЦБ РФ
- Docker / docker-compose
- xUnit тесты (unit + integration)

## Сервисы
- **UserService**: регистрация, логин, избранные валюты, JWT.
- **FinanceService**: курсы валют по избранным пользователя.
- **BackgroundWorker**: периодически тянет `http://www.cbr.ru/scripts/XML_daily.asp` и обновляет таблицу currency.
- **MigrationsService**: применяет SQL-миграции (создание таблиц users, user_favorites, currency).
- **ApiGateway**: YARP прокси `/user/*` -> user-service, `/finance/*` -> finance-service.

## Быстрый старт (Docker)
1. Убедитесь, что Docker запущен.
2. Соберите и поднимите весь стек:
   ```bash
   docker-compose up --build
   ```
   Поднимутся postgres:5432, миграции, user-service:5001, finance-service:5002, worker, gateway:8080.
3. Проверьте API через gateway (`localhost:8080`), примеры curl:
   ```bash
   # регистрация
   curl -X POST http://localhost:8080/user/register \
     -H "Content-Type: application/json" \
     -d '{"name":"alice","password":"P@ssw0rd!"}'

   # логин -> JWT
   curl -X POST http://localhost:8080/user/login \
     -H "Content-Type: application/json" \
     -d '{"name":"alice","password":"P@ssw0rd!"}'

   # добавить избранную валюту (подставьте токен)
   curl -X POST "http://localhost:8080/user/favorites?code=USD" \
     -H "Authorization: Bearer <token>"

   # получить избранные
   curl -X GET http://localhost:8080/user/favorites \
     -H "Authorization: Bearer <token>"

   # курсы по избранным
   curl -X GET http://localhost:8080/finance/rates \
     -H "Authorization: Bearer <token>"
   ```

## Локальный запуск без Docker (для отладки)
1. Запустите Postgres и задайте переменные окружения:
   ```
   Database__Host=localhost
   Database__Port=5432
   Database__Database=currencies
   Database__Username=postgres
   Database__Password=postgres
   ```
2. Примените миграции (SQL мигратор):
   ```bash
   dotnet run --project src/MigrationsService/MigrationsService.csproj
   ```
3. Запустите сервисы:
   ```bash
   dotnet run --project src/UserService/src/UserService.Api/UserService.Api.csproj
   dotnet run --project src/FinanceService/src/CurrencyService.Api/CurrencyService.Api.csproj
   dotnet run --project src/BackgroundWorker/BackgroundWorker.csproj
   dotnet run --project src/ApiGateway/ApiGateway.csproj
   ```
   Gateway (Dev) в `appsettings.Development.json` можно настраивать на localhost.

## Тесты
```bash
dotnet test RightCodeTestTask.sln
```
- Unit: `src/UserService/tests/UserService.Tests`, `src/FinanceService/tests/CurrencyService.Tests`
- Integration: `tests/Integration` (WebApplicationFactory)

## Конфигурация
- БД/ JWT задаются через `appsettings*.json` либо переменными окружения (`Database__*`, `Jwt__*`).
- Gateway маршруты — `src/ApiGateway/appsettings*.json`.
- Worker конфиг — `src/BackgroundWorker/appsettings.json`.
- Миграции (SQL) — `src/MigrationsService/Program.cs`.

## Полезное
- Postman коллекция: `postman_collection.json`.
- Docker entrypoints: `docker-compose.yml` + Dockerfile в каждом сервисе.
