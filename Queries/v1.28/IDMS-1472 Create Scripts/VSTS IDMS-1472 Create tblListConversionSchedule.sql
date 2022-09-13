USE [DW_Admin]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tblListConversionSchedule](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ListId] [int] NOT NULL,
	[BuildId] [int] NOT NULL,
	[LK_ListConversionFrequency] [char](1) NOT NULL,
	[iInterval] [int] NULL,
	[cScheduleTime] [varchar](5) NULL,
	[cSystemFileNameReadyToLoad] [varchar](255) NULL,
	[cScheduledBy] [varchar](100) NOT NULL,
	[iIsActive] [bit] NOT NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[tblListConversionSchedule] ADD  DEFAULT ((0)) FOR [BuildId]
GO

ALTER TABLE [dbo].[tblListConversionSchedule] ADD  DEFAULT ((1)) FOR [iIsActive]
GO


