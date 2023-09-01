USE PhotoShowdownDB;
GO

-- Create the Matches table
CREATE TABLE Matches (
    Id INT PRIMARY KEY IDENTITY,
    OwnerId INT NOT NULL,
    StartDate DATE,
	EndDate DATE,
	CONSTRAINT FK_Matches_Users FOREIGN KEY (OwnerId) REFERENCES Users(Id)
);
GO 
ALTER TABLE Users
ADD MatchId INT,
CONSTRAINT FK_Users_Matches FOREIGN KEY (MatchId) REFERENCES Matches(Id);
GO