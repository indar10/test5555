-- ====================================================================      
-- Author: Pratibha Deokar
-- Description: Added tblOrderFavorite
-- VSTSID-2046-Campaign Favorites Functionality
-- =====================================================================

USE [DW_Admin]
GO

/****** Object:  Table [dbo].[tblOrderFavorite]    Script Date: 2/25/2020 3:37:07 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tblOrderFavorite](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[OrderID] [int] NOT NULL,
	[UserID] [int] NOT NULL,
	[dCreatedDate] [datetime] NOT NULL,
	[cCreatedBy] [varchar](25) NOT NULL,
	[dModifiedDate] [datetime] NULL,
	[cModifiedBy] [varchar](25) NULL,
 CONSTRAINT [PK_tblOrderFavorite] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX IX_tblOrderFavorite_OrderID ON dbo.tblOrderFavorite
(
    OrderID ASC
)
GO


ALTER TABLE [dbo].[tblOrderFavorite]  WITH CHECK ADD  CONSTRAINT [FK_tblOrderFavorite_tblOrder] FOREIGN KEY([OrderID])
REFERENCES [dbo].[tblOrder] ([ID])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[tblOrderFavorite] CHECK CONSTRAINT [FK_tblOrderFavorite_tblOrder]
GO


