PRINT 'Updating database version to $(DatabaseVersion)...';

IF NOT EXISTS (
    SELECT 1 
    FROM [NoteBoard].[DatabaseVersion] 
    WHERE [Version] = $(DatabaseVersion)
)
BEGIN
    INSERT INTO [NoteBoard].[DatabaseVersion] ([Version], [AppliedAt])
    VALUES ($(DatabaseVersion), SYSUTCDATETIME());
    
    PRINT 'Database version successfully updated to $(DatabaseVersion).';
END
ELSE
BEGIN
    PRINT 'Database version $(DatabaseVersion) has already been applied previously. Skipping insertion.';
END
GO
