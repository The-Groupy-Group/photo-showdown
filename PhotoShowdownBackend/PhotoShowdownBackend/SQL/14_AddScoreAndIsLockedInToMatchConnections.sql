USE PhotoShowdownDB;
GO

-- Step 1: Add the columns as NULLABLE
ALTER TABLE MatchConnections
ADD IsLockedIn BIT NULL;
GO

ALTER TABLE MatchConnections
ADD Score FLOAT NULL;
GO

-- Step 2: Update existing records to set a non-null value
UPDATE MatchConnections
SET IsLockedIn = 0;
GO

UPDATE MatchConnections
SET Score = 0.0;
GO

-- Step 3: Alter the columns to be NOT NULL
ALTER TABLE MatchConnections
ALTER COLUMN IsLockedIn BIT NOT NULL;
GO

ALTER TABLE MatchConnections
ALTER COLUMN Score FLOAT NOT NULL;
GO