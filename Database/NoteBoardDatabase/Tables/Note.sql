CREATE TABLE [NoteBoard].[Note]
(
  [Id]          INT             NOT NULL  IDENTITY(1,1),
  [Title]       NVARCHAR(50)    NOT NULL,
  [Content]     NVARCHAR(350)   NOT NULL,
  [Color]       INT             NOT NULL,
  [IsCompleted] BIT             NOT NULL,
  [CreatedAt]   DATETIMEOFFSET  NOT NULL,
  [CreatedBy]   INT             NOT NULL,
  [UpdatedAt]   DATETIMEOFFSET  NULL,
  CONSTRAINT [PK_Note] PRIMARY KEY ([Id]),
  CONSTRAINT [FK_Note_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [NoteBoard].[User]([Id]) ON DELETE CASCADE
);
