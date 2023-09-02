ALTER TABLE Users
DROP CONSTRAINT FK_Users_MatchConnections;

ALTER TABLE Users
DROP COLUMN ConnectionId;

DROP TABLE MatchConnections;

-- Create the MatchConnections table
CREATE TABLE MatchConnections (
    UserId INT PRIMARY KEY,
    MatchId INT NOT NULL,
	CONSTRAINT FK_MatchConnections_Users FOREIGN KEY (UserId) REFERENCES Users(Id),
	CONSTRAINT FK_MatchConnections_Matches FOREIGN KEY (MatchId) REFERENCES Matches(Id)
);

DELETE FROM Matches;
GO