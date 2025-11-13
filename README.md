# Product Management API - ASP.NET Core Web API

**Author:** Surjeet Kumar  
**Date:** November 13, 2025  
**Assignment:** ASP.NET Core Web API Development

---

## 📋 Project Overview

This is a complete ASP.NET Core Web API project that provides:

1. **Product Management System** - Full CRUD operations (Create, Read, Update, Delete) for managing products with 12 properties
2. **Database Integration** - SQL Server database with Entity Framework Core
3. **Third-Party API Integration** - Weather API and Exchange Rate API with logging functionality
4. **API Documentation** - Swagger/OpenAPI integration for easy testing
5. **Postman Collection** - Ready-to-use API collection for testing

---

## 🏗️ Project Structure

```
MyAspNetCoreApp/
├── Controllers/
│   ├── ProductsController.cs          # Product CRUD operations
│   └── ExternalApiController.cs       # Third-party API integration
├── Models/
│   ├── Product.cs                     # Product model with 12 properties
│   ├── ExternalApiLog.cs              # API log model
│   └── WeatherResponse.cs             # DTOs for external APIs
├── Data/
│   └── ApplicationDbContext.cs        # EF Core DbContext
├── Database/
│   └── ProductManagementDB_Schema.sql # Complete SQL schema
├── Postman/
│   └── ProductManagementAPI_Collection.json # Postman collection
├── Program.cs                         # Application configuration
├── appsettings.json                   # Configuration & connection strings
└── MyAspNetCoreApp.csproj            # Project dependencies
```

---

## 🚀 Features

### 1. Product Management API

**Product Model Properties (12 total):**
- `Id` (int) - Primary key
- `Name` (string) - Product name (required)
- `Description` (string) - Product description
- `Price` (decimal) - Product price (required)
- `Category` (string) - Product category (required)
- `Brand` (string) - Product brand
- `StockQuantity` (int) - Available stock
- `SKU` (string) - Stock Keeping Unit (unique)
- `IsActive` (bool) - Active status
- `CreatedDate` (DateTime) - Creation timestamp
- `UpdatedDate` (DateTime) - Last update timestamp
- `ImageUrl` (string) - Product image URL

**API Endpoints:**
- `GET /api/Products` - Get all products
- `GET /api/Products/{id}` - Get product by ID
- `POST /api/Products` - Create new product
- `PUT /api/Products/{id}` - Update product
- `DELETE /api/Products/{id}` - Delete product

### 2. Third-Party API Integration

**Weather API:**
- `GET /api/ExternalApi/weather` - Get current weather data
- Uses Open-Meteo free weather API
- Logs all requests and responses

**Exchange Rate API:**
- `GET /api/ExternalApi/exchange-rates` - Get currency exchange rates
- Supports multiple base currencies
- Logs all requests and responses

**API Logs:**
- `GET /api/ExternalApi/logs` - Get all API call logs
- `GET /api/ExternalApi/logs/{id}` - Get specific log by ID

### 3. Database Tables

**Products Table:**
- Stores all product information
- Includes constraints and indexes
- Pre-seeded with sample data

**ExternalApiLogs Table:**
- Logs all third-party API calls
- Tracks request/response details
- Monitors API performance

---

## 🛠️ Technology Stack

- **Framework:** ASP.NET Core 10.0
- **Database:** SQL Server (LocalDB/SQL Server Express)
- **ORM:** Entity Framework Core 9.0
- **API Documentation:** Swagger/OpenAPI
- **Language:** C# 12.0

---

## 📦 Installation & Setup

### Prerequisites

1. .NET 10.0 SDK or later
2. Visual Studio 2022 or Visual Studio Code
3. SQL Server (LocalDB or SQL Server Express)
4. Postman (for API testing)

### Step 1: Clone/Download the Project

```bash
cd MyAspNetCoreApp
```

### Step 2: Restore NuGet Packages

```bash
dotnet restore
```

### Step 3: Update Database Connection String (if needed)

Edit `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=ProductManagementDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
  }
}
```

### Step 4: Create Database

**Option A: Using SQL Script (Recommended)**

1. Open SQL Server Management Studio (SSMS)
2. Open the file: `Database/ProductManagementDB_Schema.sql`
3. Execute the script
4. This creates the database, tables, indexes, and sample data

**Option B: Using Entity Framework Migrations**

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### Step 5: Build the Project

```bash
dotnet build
```

### Step 6: Run the Application

```bash
dotnet run
```

The API will start on:
- HTTPS: `https://localhost:7000` (or your configured port)
- HTTP: `http://localhost:5000` (or your configured port)

### Step 7: Access Swagger UI

Open your browser and navigate to:
```
https://localhost:7000
```

Swagger UI will open automatically, showing all available endpoints.

---

## 🧪 Testing with Postman

### Import Postman Collection

1. Open Postman
2. Click **Import**
3. Select the file: `Postman/ProductManagementAPI_Collection.json`
4. Update the `baseUrl` variable to match your API URL (default: `https://localhost:7000`)

### Test Scenarios

#### 1. Test Product CRUD Operations

**Create a Product:**
```http
POST /api/Products
Content-Type: application/json

{
  "name": "MacBook Pro M3",
  "description": "Latest MacBook Pro with M3 chip",
  "price": 1999.99,
  "category": "Electronics",
  "brand": "Apple",
  "stockQuantity": 15,
  "sku": "APPL-MBP-M3-001",
  "isActive": true,
  "imageUrl": "https://example.com/images/macbook.jpg"
}
```

**Get All Products:**
```http
GET /api/Products
```

**Get Product by ID:**
```http
GET /api/Products/1
```

**Update Product:**
```http
PUT /api/Products/1
Content-Type: application/json

{
  "id": 1,
  "name": "Updated Product Name",
  "price": 1599.99,
  ...
}
```

**Delete Product:**
```http
DELETE /api/Products/1
```

#### 2. Test Third-Party API Integration

**Get Weather Data:**
```http
GET /api/ExternalApi/weather?latitude=52.52&longitude=13.41
```

**Get Exchange Rates:**
```http
GET /api/ExternalApi/exchange-rates?baseCurrency=USD
```

**View API Logs:**
```http
GET /api/ExternalApi/logs
```

---

## 📊 Database Schema

### Products Table

| Column | Type | Constraints |
|--------|------|-------------|
| Id | INT | PRIMARY KEY, IDENTITY |
| Name | NVARCHAR(100) | NOT NULL |
| Description | NVARCHAR(500) | NULL |
| Price | DECIMAL(18,2) | NOT NULL, CHECK (>= 0.01) |
| Category | NVARCHAR(50) | NOT NULL |
| Brand | NVARCHAR(50) | NULL |
| StockQuantity | INT | NOT NULL, CHECK (>= 0) |
| SKU | NVARCHAR(50) | NOT NULL, UNIQUE |
| IsActive | BIT | NOT NULL, DEFAULT 1 |
| CreatedDate | DATETIME2 | NOT NULL |
| UpdatedDate | DATETIME2 | NULL |
| ImageUrl | NVARCHAR(500) | NULL |

### ExternalApiLogs Table

| Column | Type | Constraints |
|--------|------|-------------|
| Id | INT | PRIMARY KEY, IDENTITY |
| ApiName | NVARCHAR(100) | NOT NULL |
| RequestUrl | NVARCHAR(500) | NOT NULL |
| RequestMethod | NVARCHAR(50) | NULL |
| RequestBody | NVARCHAR(MAX) | NULL |
| ResponseBody | NVARCHAR(MAX) | NULL |
| StatusCode | INT | NULL |
| RequestedAt | DATETIME2 | NOT NULL |
| IsSuccessful | BIT | NOT NULL |
| ErrorMessage | NVARCHAR(1000) | NULL |
| ResponseTimeMs | INT | NOT NULL |

---

## 📝 API Endpoints Documentation

### Products Controller

| Method | Endpoint | Description | Request Body | Response |
|--------|----------|-------------|--------------|----------|
| GET | `/api/Products` | Get all products | None | 200 OK (List of products) |
| GET | `/api/Products/{id}` | Get product by ID | None | 200 OK (Product) / 404 Not Found |
| POST | `/api/Products` | Create product | Product JSON | 201 Created (Product) |
| PUT | `/api/Products/{id}` | Update product | Product JSON | 204 No Content / 404 Not Found |
| DELETE | `/api/Products/{id}` | Delete product | None | 204 No Content / 404 Not Found |

### External API Controller

| Method | Endpoint | Description | Parameters | Response |
|--------|----------|-------------|------------|----------|
| GET | `/api/ExternalApi/weather` | Get weather data | latitude, longitude | 200 OK (Weather data) |
| GET | `/api/ExternalApi/exchange-rates` | Get exchange rates | baseCurrency | 200 OK (Exchange rates) |
| GET | `/api/ExternalApi/logs` | Get all API logs | None | 200 OK (List of logs) |
| GET | `/api/ExternalApi/logs/{id}` | Get log by ID | None | 200 OK (Log) / 404 Not Found |

---

## 🔧 Configuration

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=ProductManagementDB;..."
  },
  "ExternalApis": {
    "WeatherApiUrl": "https://api.open-meteo.com/v1/forecast",
    "ExchangeRateApiUrl": "https://api.exchangerate-api.com/v4/latest"
  }
}
```

---

## 📤 Submission Checklist

- [x] Complete Product CRUD API with 12 properties
- [x] SQL Server database with 2 tables (Products, ExternalApiLogs)
- [x] Third-party API integration (Weather & Exchange Rate)
- [x] Database logging for external API calls
- [x] Swagger/OpenAPI documentation
- [x] Postman collection with all endpoints
- [x] SQL schema file with sample data
- [x] Complete README documentation
- [x] Proper error handling and logging
- [x] Model validation and constraints

---

## 🎯 Testing Instructions

### 1. Verify Database Setup

```sql
USE ProductManagementDB;
SELECT * FROM Products;
SELECT * FROM ExternalApiLogs;
```

### 2. Test All Endpoints in Postman

- Import the collection
- Run all requests in sequence
- Verify responses and status codes
- Check database for changes

### 3. Verify Swagger Documentation

- Navigate to `https://localhost:7000`
- Test each endpoint using Swagger UI
- Verify request/response schemas

---

## 📧 Submission Details

**GitHub Repository:** [Your GitHub URL]

**Deliverables:**
1. ✅ Complete source code
2. ✅ SQL database schema file (`Database/ProductManagementDB_Schema.sql`)
3. ✅ Postman collection (`Postman/ProductManagementAPI_Collection.json`)
4. ✅ README documentation (this file)

**Test Status:**
- ✅ All Product CRUD operations working
- ✅ Database tables created and populated
- ✅ Third-party APIs integrated
- ✅ API logging functional
- ✅ Postman collection tested
- ✅ Swagger documentation accessible

---

## 🐛 Troubleshooting

### Issue: Database Connection Failed

**Solution:** 
- Verify SQL Server is running
- Check connection string in `appsettings.json`
- Run the SQL schema file manually

### Issue: Port Already in Use

**Solution:**
- Edit `Properties/launchSettings.json`
- Change the port numbers
- Update Postman collection base URL

### Issue: Migration Errors

**Solution:**
```bash
dotnet ef database drop -f
dotnet ef migrations remove
dotnet ef migrations add InitialCreate
dotnet ef database update
```

---

## 📞 Contact

**Developer:** Surjeet Kumar  
**Email:** surjeethkumar4@gmail.com.com  
**Date:** November 13, 2025

---

## 📜 License

This project is created for educational purposes as part of an ASP.NET Core assignment.

---

**Thank you for reviewing this project!** 🚀
