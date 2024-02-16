﻿USE PhotoShowdownDB;
GO

-- Alter the User table to add IsActive and IsAdmin columns
ALTER TABLE Matches
ADD PictureSelectionTimeSeconds INT NOT NULL DEFAULT 30,
    VoteTimeSeconds INT NOT NULL DEFAULT 30,
	NumOfVotesToWin INT NOT NULL DEFAULT 100,
	NumOfRounds INT NOT NULL DEFAULT 500;
GO