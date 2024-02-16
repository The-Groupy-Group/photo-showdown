USE PhotoShowdownDB;
GO

EXEC sp_rename 'Rounds.RoundId', 'RoundIndex', 'COLUMN';
GO

ALTER TABLE CustomSentences
ALTER COLUMN Sentence DATE NOT NULL;
GO

ALTER TABLE Rounds
ADD RoundState INT NOT NULL DEFAULT 0;
GO
