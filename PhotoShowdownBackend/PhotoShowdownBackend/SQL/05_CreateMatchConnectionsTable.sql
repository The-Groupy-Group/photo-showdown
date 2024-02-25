USE PhotoShowdownDB;
GO

-- Create the MatchConnections table
CREATE TABLE MatchConnections (
    Id INT PRIMARY KEY IDENTITY,
    UserId INT NOT NULL,
    MatchId INT NOT NULL,
	CONSTRAINT FK_MatchConnections_Users FOREIGN KEY (UserId) REFERENCES Users(Id),
	CONSTRAINT FK_MatchConnections_Matches FOREIGN KEY (MatchId) REFERENCES Matches(Id)
);
GO 
ALTER TABLE Users
DROP CONSTRAINT FK_Users_Matches;
GO
ALTER TABLE Users
DROP COLUMN	MatchId;
GO
ALTER TABLE Users
ADD ConnectionId INT;
GO
ALTER TABLE Users
ADD CONSTRAINT FK_Users_MatchConnections FOREIGN KEY (ConnectionId) REFERENCES MatchConnections(Id);
GO