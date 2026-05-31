# DailyBasket

DailyBasket is a grocery ordering and inventory management system.

## Stack

- React + Vite frontend
- Axios API services
- Tailwind CSS styling
- ASP.NET Core Web API on .NET 9
- Entity Framework Core
- MSSQL database

## Structure

```text
DailyBasketSWE310/
├─ DailyBasket.Api/              ASP.NET Core Web API
│  ├─ Controllers/               API endpoints
│  ├─ Services/                  Business rules
│  ├─ Repositories/              Data access
│  ├─ Data/                      EF Core DbContext and seed data
│  ├─ Models/                    Domain entities
│  ├─ DTOs/                      Request and response DTOs
│  ├─ Mappings/                  Entity to DTO mapping
│  ├─ Middleware/                Global error handling
│  └─ appsettings.json           MSSQL connection string and CORS origins
├─ frontend/dailybasket-client/  React application
├─ database/DailyBasket.sql      MSSQL schema and seed script
└─ docs/                         Report-ready notes
```

## API Modules

- Categories: full CRUD
- Products: full CRUD with category filtering
- Customers: full CRUD
- Cart: list, add, update quantity, remove, clear
- Orders: list all, list by customer, checkout from cart

## Run Backend

Update `DailyBasket.Api/appsettings.json` if your MSSQL server name is different (if its name is not localhost).

```powershell
dotnet restore DailyBasket.sln
dotnet build DailyBasket.sln
dotnet run --project DailyBasket.Api
```

## Run Frontend

```powershell
cd frontend/dailybasket-client
npm install
npm run dev
```
