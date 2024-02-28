USE PhotoShowdownDB;
GO

DROP TABLE CustomSentences;
GO

CREATE Table CustomSentences (

	Id INT PRIMARY KEY IDENTITY,
	MatchId INT NOT NULL,
	Sentence NVARCHAR(256) NOT NULL,
	CONSTRAINT FK_CustomSenctences_Matches FOREIGN KEY (MatchId) REFERENCES Matches(Id)
);
GO
