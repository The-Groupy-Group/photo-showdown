USE PhotoShowdownDB;
GO

-- Create the User table
CREATE TABLE Pictures (
    Id INT PRIMARY KEY IDENTITY,
    UserId INT NOT NULL,
    PicturePath NVARCHAR(128) NOT NULL,
	CONSTRAINT FK_Pictures_Users FOREIGN KEY (UserId) REFERENCES Users(Id)
);
GO