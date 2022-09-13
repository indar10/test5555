USE [DW_Admin]
GO

/****** Object:  Table [dbo].[tblUserDatabaseMailer]    Script Date: 3/10/2020 4:10:06 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tblUserDatabaseMailer](
[ID] [int] IDENTITY(1,1) NOT NULL,
[UserID] [int] NOT NULL,
[DatabaseID] [int] NOT NULL,
[MailerID] [int] NOT NULL,
[cCreatedBy] [varchar](25) NOT NULL,
[dCreatedDate] [datetime] NOT NULL,
[cModifiedBy] [varchar](25) NULL,
[dModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_tblUserDatabaseMailer] PRIMARY KEY CLUSTERED 
(
[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[tblUserDatabaseMailer]  WITH CHECK ADD  CONSTRAINT [FK_tblUserDatabaseMailer_tblDatabase] FOREIGN KEY([DatabaseID])
REFERENCES [dbo].[tblDatabase] ([ID])
GO

ALTER TABLE [dbo].[tblUserDatabaseMailer] CHECK CONSTRAINT [FK_tblUserDatabaseMailer_tblDatabase]
GO

ALTER TABLE [dbo].[tblUserDatabaseMailer]  WITH CHECK ADD  CONSTRAINT [FK_tblUserDatabaseMailer_tblUser] FOREIGN KEY([UserID])
REFERENCES [dbo].[tblUser] ([ID])
GO

ALTER TABLE [dbo].[tblUserDatabaseMailer] CHECK CONSTRAINT [FK_tblUserDatabaseMailer_tblUser]
GO
