USE [DW_Admin]
GO

/****** Object:  Trigger [dbo].[UpdatePasswordHistory]    Script Date: 8/25/2020 6:23:51 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TRIGGER [dbo].[UpdatePasswordHistory] ON [dbo].[AbpUsers] AFTER
INSERT,
UPDATE AS 
BEGIN 
DECLARE @Id AS INT,@UserID as INT,@UserPassword AS VARCHAR(MAX)
DECLARE @Action AS char(1);
    SET @Action = (CASE WHEN EXISTS(SELECT * FROM INSERTED)
                         AND EXISTS(SELECT * FROM DELETED)
                        THEN 'U'  -- Set Action to Updated.
                        WHEN EXISTS(SELECT * FROM INSERTED)
                        THEN 'I'  -- Set Action to Insert.
                        ELSE NULL 
                    END)

SELECT @UserID = Id,@UserPassword = Password
FROM INSERTED 
SELECT @Id=id FROM [tblAbpUserPasswords] WHERE IsActive = 1 AND  UserId = @UserID AND Password <> @UserPassword

 IF @Action = 'U'
	BEGIN		
		SET NOCOUNT ON;
		INSERT INTO [tblAbpUserPasswords] (UserId, Password, IsActive, CreationTime, CreatorUserId)
		SELECT inserted.Id,
		       inserted.Password,
		       1,
		       GETDATE(),
		       CASE
		           WHEN LastModifierUserId IS NOT NULL THEN LastModifierUserId
		           ELSE ISNULL(inserted.CreatorUserId, 1)
		       END
		FROM INSERTED inserted
		INNER JOIN [tblAbpUserPasswords] userPassword ON inserted.id = userPassword.UserId
		WHERE userPassword.IsActive = 1
		  AND inserted.Password <> userpassword.Password 
		
		  UPDATE [tblAbpUserPasswords]
		  SET IsActive = 0
		  WHERE ID =@Id
	END
 IF @Action = 'I'
	BEGIN
		SET NOCOUNT ON;
		INSERT INTO [tblAbpUserPasswords] (UserId, Password, IsActive, CreationTime, CreatorUserId)
		SELECT inserted.Id,
		       inserted.Password,
		       1,
		       GETDATE(),
		       CASE
		           WHEN LastModifierUserId IS NOT NULL THEN LastModifierUserId
		           ELSE ISNULL(inserted.CreatorUserId, 1)
		       END
		FROM INSERTED inserted
		
	END
END 
GO
ALTER TABLE [dbo].[AbpUsers] ENABLE TRIGGER [UpdatePasswordHistory]
GO