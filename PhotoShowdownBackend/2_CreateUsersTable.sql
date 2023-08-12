USE PhotoShowdownDB;
GO

-- Create the User table
CREATE TABLE Users (
    Id INT PRIMARY KEY IDENTITY,
    Username NVARCHAR(50) NOT NULL,
    PasswordHash NVARCHAR(128) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    FirstName NVARCHAR(50),
    LastName NVARCHAR(50)
);
GO