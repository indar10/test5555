
USE [DW_Admin]
GO

/****** Object:  Table [dbo].[tblAbpUserPasswords]    Script Date: 8/24/2020 2:50:48 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tblAbpUserPasswords](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[UserId] [bigint] NOT NULL,
	[Password] [nvarchar](128) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[CreationTime] [datetime2](7) NOT NULL,
	[CreatorUserId] [bigint] NULL,
 CONSTRAINT [PK_AbpAppUserPassword] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[tblAbpUserPasswords]  WITH CHECK ADD  CONSTRAINT [FK_AbpUserPasswords_AbpUsers] FOREIGN KEY([UserId])
REFERENCES [dbo].[AbpUsers] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[tblAbpUserPasswords] CHECK CONSTRAINT [FK_AbpUserPasswords_AbpUsers]
GO

-- Query for inserting intial data into [tblAbpUserPasswords]

INSERT INTO [tblAbpUserPasswords] (UserId, Password, IsActive, CreationTime, CreatorUserId)
SELECT Id,
       Password,
       1,
       CASE
           WHEN LastModificationTime IS NOT NULL THEN LastModificationTime
           ELSE ISNull(CreationTime, GETDATE())
       END,
       CASE
           WHEN LastModifierUserId IS NOT NULL THEN LastModifierUserId
           ELSE ISNull(CreatorUserId, 2)
       END
FROM AbpUsers