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
├─ DailyBasket.Api/                  ASP.NET Core Web API (.NET 9)
│  ├─ Controllers/                   API endpoints (Categories, Products, Customers, Cart, Orders)
│  ├─ DTOs/                          Request and response Data Transfer Objects
│  ├─ Data/                          EF Core DbContext and database initializer
│  ├─ Exceptions/                    Custom HTTP exceptions
│  ├─ Mappings/                      Mapping profiles (Entity ↔ DTO)
│  ├─ Middleware/                    Global exception and error handling middleware
│  ├─ Models/                        Domain models/entities (Product, Category, Customer, etc.)
│  ├─ Properties/                    Application properties (launchSettings.json)
│  ├─ Repositories/                  Data access layer (Repository Pattern)
│  ├─ Services/                      Business logic/service layer
│  ├─ Program.cs                     App entry point & DI container configuration
│  └─ appsettings.json               MSSQL connection string and CORS configuration
├─ database/
│  └─ DailyBasket.sql                MSSQL schema structure & seed data script
├─ frontend/
│  └─ dailybasket-client/            Vite + React frontend application
│     ├─ src/
│     │  ├─ api/                     Axios client & API service layer
│     │  ├─ components/              Reusable UI components (Charts, Modals, Layout, etc.)
│     │  ├─ pages/                   Application views (Dashboard, Products, Cart, Orders, etc.)
│     │  ├─ App.jsx                  Main App router & layout container
│     │  ├─ index.css                Tailwind CSS & global styles
│     │  └─ main.jsx                 Vite application entry point
│     ├─ index.html                  Vite HTML template
│     ├─ package.json                Frontend project dependencies & scripts
│     └─ vite.config.js              Vite environment configuration
├─ DailyBasket.sln                   Backend solution file
└─ NuGet.Config                      NuGet configuration file
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
