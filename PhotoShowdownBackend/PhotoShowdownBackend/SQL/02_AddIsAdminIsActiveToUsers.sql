USE PhotoShowdownDB;
GO

-- Alter the User table to add IsActive and IsAdmin columns
ALTER TABLE Users
ADD IsActive BIT NOT NULL DEFAULT 1,
    IsAdmin BIT NOT NULL DEFAULT 0;
GO