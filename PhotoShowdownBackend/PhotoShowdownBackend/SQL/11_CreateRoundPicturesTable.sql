USE PhotoShowdownDB;
GO


CREATE TABLE RoundPictures (
	Id INT PRIMARY KEY IDENTITY,
	PictureId INT NOT NULL,
	UserId INT NOT NULL,
    MatchId INT NOT NULL,
	RoundIndex INT NOT NULL,
    StartDate DATE,
	EndDate DATE,
	CONSTRAINT FK_RoundPictures_Users FOREIGN KEY (UserId) REFERENCES Users(Id),
	CONSTRAINT FK_RoundPictures_Matches FOREIGN KEY (MatchId) REFERENCES Matches(Id),
	CONSTRAINT FK_RoundPictures_Rounds FOREIGN KEY (MatchId, RoundIndex) REFERENCES Rounds(MatchId, RoundIndex),
);
GO 