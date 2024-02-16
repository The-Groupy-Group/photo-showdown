USE PhotoShowdownDB;
GO

CREATE Table CustomSentences (

	Id INT PRIMARY KEY,
	MatchId INT NOT NULL,
	Sentence NVARCHAR(256),
	CONSTRAINT FK_CustomSenctences_Matches FOREIGN KEY (MatchId) REFERENCES Matches(Id)

);
GO