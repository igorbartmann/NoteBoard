CREATE TABLE [NoteBoard].[User]
(
  [Id]            INT               NOT NULL  IDENTITY(1,1),
  [Name]          NVARCHAR(50)      NOT NULL,
  [Email]         NVARCHAR(254)     NOT NULL,
  [PasswordHash]  NVARCHAR(256)     NOT NULL,
  [CreatedAt]     DATETIMEOFFSET(3) NOT NULL,
  [UpdatedAt]     DATETIMEOFFSET(3) NULL,
  CONSTRAINT [PK_User] PRIMARY KEY ([Id]),
  CONSTRAINT [UQ_User_Email] UNIQUE ([Email])
);
