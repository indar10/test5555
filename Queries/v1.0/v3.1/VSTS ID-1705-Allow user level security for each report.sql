  -- ====================================================================      
-- Author: Saarthak Pande
-- Description: Added tblReport and tblUserReport
-- VSTS ID-1705-Allow user level security for each report
-- =====================================================================

USE [DW_Admin]
GO
/****** Object:  Table [dbo].[tblReport]    Script Date: 1/17/2020 5:17:56 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblReport](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[cReportID] [varchar](max) NOT NULL,
	[cReportName] [varchar](255) NOT NULL,
	[cReportWorkSpaceID] [varchar](max) NOT NULL,
	[cReportConfig] [varchar](max) NOT NULL,
 CONSTRAINT [PK_tblReport] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblUserReport]    Script Date: 1/17/2020 5:17:58 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblUserReport](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[UserID] [int] NULL,
	[ReportID] [int] NULL,
 CONSTRAINT [PK_tblUserReport] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[tblUserReport]  WITH CHECK ADD  CONSTRAINT [FK_tblUserReport_tblReport] FOREIGN KEY([ReportID])
REFERENCES [dbo].[tblReport] ([ID])
GO
ALTER TABLE [dbo].[tblUserReport] CHECK CONSTRAINT [FK_tblUserReport_tblReport]
GO
ALTER TABLE [dbo].[tblUserReport]  WITH CHECK ADD  CONSTRAINT [FK_tblUserReport_tblUser] FOREIGN KEY([UserID])
REFERENCES [dbo].[tblUser] ([ID])
GO
ALTER TABLE [dbo].[tblUserReport] CHECK CONSTRAINT [FK_tblUserReport_tblUser]
GO
