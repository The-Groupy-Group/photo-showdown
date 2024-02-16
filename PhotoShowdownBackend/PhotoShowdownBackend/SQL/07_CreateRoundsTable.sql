USE PhotoShowdownDB;
GO

-- Create the Matches table
CREATE TABLE Rounds (
    MatchId INT NOT NULL,
	RoundId INT NOT NULL,
    WinnerId INT,
    StartDate DATE,
	EndDate DATE,
	CONSTRAINT FK_Rounds_Users FOREIGN KEY (WinnerId) REFERENCES Users(Id),
	CONSTRAINT FK_Rounds_Matches FOREIGN KEY (MatchId) REFERENCES Matches(Id),
	CONSTRAINT PK_Rounds PRIMARY KEY (MatchId, RoundId)
);
GO 