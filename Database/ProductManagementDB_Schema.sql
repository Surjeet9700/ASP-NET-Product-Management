-- ================================================
-- Product Management Database Schema
-- ================================================
-- Author: Surjeet Kumar
-- Date: November 13, 2025
-- Description: SQL Server database schema for Product Management API
-- ================================================

USE master;
GO

-- Create Database if not exists
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'ProductManagementDB')
BEGIN
    CREATE DATABASE ProductManagementDB;
    PRINT 'Database ProductManagementDB created successfully.';
END
ELSE
BEGIN
    PRINT 'Database ProductManagementDB already exists.';
END
GO

USE ProductManagementDB;
GO

-- ================================================
-- Table: Products
-- Description: Stores product information
-- ================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Products')
BEGIN
    CREATE TABLE Products (
        Id INT PRIMARY KEY IDENTITY(1,1),
        Name NVARCHAR(100) NOT NULL,
        Description NVARCHAR(500) NULL,
        Price DECIMAL(18,2) NOT NULL,
        Category NVARCHAR(50) NOT NULL,
        Brand NVARCHAR(50) NULL,
        StockQuantity INT NOT NULL DEFAULT 0,
        SKU NVARCHAR(50) NOT NULL UNIQUE,
        IsActive BIT NOT NULL DEFAULT 1,
        CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedDate DATETIME2 NULL,
        ImageUrl NVARCHAR(500) NULL,
        
        -- Constraints
        CONSTRAINT CK_Products_Price CHECK (Price >= 0.01 AND Price <= 999999.99),
        CONSTRAINT CK_Products_StockQuantity CHECK (StockQuantity >= 0)
    );
    
    -- Create indexes for better query performance
    CREATE INDEX IX_Products_Category ON Products(Category);
    CREATE INDEX IX_Products_Brand ON Products(Brand);
    CREATE INDEX IX_Products_IsActive ON Products(IsActive);
    CREATE INDEX IX_Products_CreatedDate ON Products(CreatedDate DESC);
    
    PRINT 'Table Products created successfully.';
END
ELSE
BEGIN
    PRINT 'Table Products already exists.';
END
GO

-- ================================================
-- Table: ExternalApiLogs
-- Description: Stores logs of external API calls
-- ================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ExternalApiLogs')
BEGIN
    CREATE TABLE ExternalApiLogs (
        Id INT PRIMARY KEY IDENTITY(1,1),
        ApiName NVARCHAR(100) NOT NULL,
        RequestUrl NVARCHAR(500) NOT NULL,
        RequestMethod NVARCHAR(50) NULL,
        RequestBody NVARCHAR(MAX) NULL,
        ResponseBody NVARCHAR(MAX) NULL,
        StatusCode INT NULL,
        RequestedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        IsSuccessful BIT NOT NULL DEFAULT 0,
        ErrorMessage NVARCHAR(1000) NULL,
        ResponseTimeMs INT NOT NULL DEFAULT 0
    );
    
    -- Create indexes for better query performance
    CREATE INDEX IX_ExternalApiLogs_ApiName ON ExternalApiLogs(ApiName);
    CREATE INDEX IX_ExternalApiLogs_RequestedAt ON ExternalApiLogs(RequestedAt DESC);
    CREATE INDEX IX_ExternalApiLogs_IsSuccessful ON ExternalApiLogs(IsSuccessful);
    
    PRINT 'Table ExternalApiLogs created successfully.';
END
ELSE
BEGIN
    PRINT 'Table ExternalApiLogs already exists.';
END
GO

-- ================================================
-- Seed Data: Products
-- ================================================
IF NOT EXISTS (SELECT * FROM Products WHERE Id = 1)
BEGIN
    SET IDENTITY_INSERT Products ON;
    
    INSERT INTO Products (Id, Name, Description, Price, Category, Brand, StockQuantity, SKU, IsActive, CreatedDate, ImageUrl)
    VALUES 
    (1, 'Laptop Dell XPS 15', 'High-performance laptop with 16GB RAM and 512GB SSD', 1299.99, 'Electronics', 'Dell', 25, 'DELL-XPS15-001', 1, GETUTCDATE(), 'https://example.com/images/dell-xps15.jpg'),
    (2, 'iPhone 15 Pro', 'Latest iPhone with A17 Pro chip and titanium design', 999.99, 'Electronics', 'Apple', 50, 'APPL-IP15P-001', 1, GETUTCDATE(), 'https://example.com/images/iphone15pro.jpg'),
    (3, 'Samsung Galaxy S24', 'Flagship Android phone with 256GB storage', 849.99, 'Electronics', 'Samsung', 40, 'SAMS-GS24-001', 1, GETUTCDATE(), 'https://example.com/images/galaxy-s24.jpg');
    
    SET IDENTITY_INSERT Products OFF;
    
    PRINT 'Sample product data inserted successfully.';
END
ELSE
BEGIN
    PRINT 'Sample product data already exists.';
END
GO

-- ================================================
-- Stored Procedures
-- ================================================

-- Procedure to get all active products
IF OBJECT_ID('sp_GetActiveProducts', 'P') IS NOT NULL
    DROP PROCEDURE sp_GetActiveProducts;
GO

CREATE PROCEDURE sp_GetActiveProducts
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT Id, Name, Description, Price, Category, Brand, StockQuantity, 
           SKU, IsActive, CreatedDate, UpdatedDate, ImageUrl
    FROM Products
    WHERE IsActive = 1
    ORDER BY CreatedDate DESC;
END
GO

-- Procedure to get low stock products
IF OBJECT_ID('sp_GetLowStockProducts', 'P') IS NOT NULL
    DROP PROCEDURE sp_GetLowStockProducts;
GO

CREATE PROCEDURE sp_GetLowStockProducts
    @Threshold INT = 10
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT Id, Name, Description, Price, Category, Brand, StockQuantity, 
           SKU, IsActive, CreatedDate, UpdatedDate, ImageUrl
    FROM Products
    WHERE StockQuantity <= @Threshold AND IsActive = 1
    ORDER BY StockQuantity ASC;
END
GO

-- Procedure to get API logs by date range
IF OBJECT_ID('sp_GetApiLogsByDateRange', 'P') IS NOT NULL
    DROP PROCEDURE sp_GetApiLogsByDateRange;
GO

CREATE PROCEDURE sp_GetApiLogsByDateRange
    @StartDate DATETIME2,
    @EndDate DATETIME2
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT Id, ApiName, RequestUrl, RequestMethod, StatusCode, 
           RequestedAt, IsSuccessful, ErrorMessage, ResponseTimeMs
    FROM ExternalApiLogs
    WHERE RequestedAt BETWEEN @StartDate AND @EndDate
    ORDER BY RequestedAt DESC;
END
GO

-- ================================================
-- Views
-- ================================================

-- View for product summary
IF OBJECT_ID('vw_ProductSummary', 'V') IS NOT NULL
    DROP VIEW vw_ProductSummary;
GO

CREATE VIEW vw_ProductSummary
AS
SELECT 
    Id,
    Name,
    Category,
    Brand,
    Price,
    StockQuantity,
    IsActive,
    CreatedDate
FROM Products;
GO

-- View for API logs summary
IF OBJECT_ID('vw_ApiLogsSummary', 'V') IS NOT NULL
    DROP VIEW vw_ApiLogsSummary;
GO

CREATE VIEW vw_ApiLogsSummary
AS
SELECT 
    ApiName,
    COUNT(*) AS TotalCalls,
    SUM(CASE WHEN IsSuccessful = 1 THEN 1 ELSE 0 END) AS SuccessfulCalls,
    SUM(CASE WHEN IsSuccessful = 0 THEN 1 ELSE 0 END) AS FailedCalls,
    AVG(ResponseTimeMs) AS AvgResponseTime,
    MAX(RequestedAt) AS LastRequestedAt
FROM ExternalApiLogs
GROUP BY ApiName;
GO

-- ================================================
-- Verification Queries
-- ================================================
PRINT '================================================';
PRINT 'Database Setup Completed Successfully!';
PRINT '================================================';
PRINT '';
PRINT 'Products Count: ' + CAST((SELECT COUNT(*) FROM Products) AS VARCHAR);
PRINT 'API Logs Count: ' + CAST((SELECT COUNT(*) FROM ExternalApiLogs) AS VARCHAR);
PRINT '';
PRINT '================================================';
GO

-- Sample queries for testing
-- SELECT * FROM Products;
-- SELECT * FROM ExternalApiLogs;
-- SELECT * FROM vw_ProductSummary;
-- SELECT * FROM vw_ApiLogsSummary;
-- EXEC sp_GetActiveProducts;
-- EXEC sp_GetLowStockProducts @Threshold = 30;
