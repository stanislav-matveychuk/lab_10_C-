# Лабораторна робота №10 — варіант 19
Медичний портал: запис до лікарів, історія хвороб, телемедицина.
Мова: C#, ASP.NET Core, .NET 10; сховище: EF Core SQLite.

## Архітектура
- Domain: сутності й інтерфейс репозиторію, без залежностей від інших проєктів.
- Application: бізнес-правила через IRepository, залежить лише від Domain.
- Infrastructure: AppDbContext, SQLite, реалізація репозиторію.
- API: контролер, HTTP-відповіді, конфігурація Dependency Injection.

## Запуск
Потрібен .NET 10 SDK (або IDE з його підтримкою).
Відкрийте MedicalPortal.sln або виконайте з папки рішення:
```powershell
dotnet restore
dotnet build
dotnet run --project MedicalPortal.API -- --urls http://localhost:5100
```
Swagger: http://localhost:5100/swagger
SQLite medical.db створюється в папці API автоматично, разом із демоданими.
EnsureCreated використано для каркаса; міграції тут не застосовуються.

## Демонстрація
1. GET /api/doctors і /api/patients: демонстраційні записи з Id=1.
2. POST /api/appointments: {"doctorId":1,"patientId":1,"startsAtUtc":"2030-01-01T10:00:00Z","isOnline":true}
3. POST /api/records: {"appointmentId":1,"notes":"Демонстраційний запис історії."}
4. GET /api/patients/1/history: історія пацієнта.
5. POST /api/appointments/1/telemedicine: код кімнати консультації.
6. Повторне бронювання того самого слота: 409; невідомий пацієнт: 404; некоректний час: 400.

Прийоми мають фіксовані 30-хвилинні слоти. Час надсилається в UTC із Z.
Телемедицина реалізована як каркас сесій, без відеозв'язку.
Це навчальний API з вигаданими даними: автентифікація та розмежування доступу
для роботи з реальними медичними даними не реалізовані.
