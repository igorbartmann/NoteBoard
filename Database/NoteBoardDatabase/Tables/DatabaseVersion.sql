CREATE TABLE [NoteBoard].[DatabaseVersion]
(
  [Id]        INT               NOT NULL IDENTITY(1,1),
  [Version]   INT               NOT NULL,
  [AppliedAt] DATETIMEOFFSET(3) NOT NULL,
  CONSTRAINT [PK_DatabaseVersion] PRIMARY KEY ([Id])
);
