USE PhotoShowdownDB;
GO


CREATE TABLE RoundVotes (
	Id INT PRIMARY KEY IDENTITY,
	RoundPictureId INT NOT NULL,
	UserId INT NOT NULL,
	CONSTRAINT FK_RoundVotes_Users FOREIGN KEY (UserId) REFERENCES Users(Id),
	CONSTRAINT FK_RoundVotes_RoundPictures FOREIGN KEY (RoundPictureId) REFERENCES RoundPictures(Id),
);
GO 