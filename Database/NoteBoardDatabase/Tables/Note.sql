CREATE TABLE [NoteBoard].[Note]
(
  [Id]        INT               NOT NULL  IDENTITY(1,1),
  [Title]     NVARCHAR(50)      NOT NULL,
  [Content]   NVARCHAR(350)     NOT NULL,
  [Color]     INT               NOT NULL,
  [CreatedAt] DATETIMEOFFSET(3) NOT NULL,
  [CreatedBy] INT               NOT NULL,
  [UpdatedAt] DATETIMEOFFSET(3) NULL,
  [UpdatedBy] INT               NULL,
  CONSTRAINT [PK_Note] PRIMARY KEY ([Id]),
  CONSTRAINT [FK_Note_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [NoteBoard].[User]([Id]),
  CONSTRAINT [FK_Note_UpdatedBy] FOREIGN KEY ([UpdatedBy]) REFERENCES [NoteBoard].[User]([Id])
);
