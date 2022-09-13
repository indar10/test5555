-- ====================================================================      
-- Author: Saarthak Pande
-- Description: Insert Scripts for Configuration Table - Scheduled Tasks DataAxle Shipping Report
-- User Story - IDMS-499 - NUA | App Refactor : Batch Task - All Active Auto Scheduled Tasks (tblSchedule) - Shipping Report
-- =====================================================================

USE [DW_Admin];

INSERT INTO [dbo].[tblConfiguration]
           ([DivisionID]
           ,[DatabaseID]
           ,[cItem]
           ,[cDescription]
           ,[cValue]
           ,[iValue]
           ,[mValue]
           ,[iIsActive]
           ,[cCreatedBy]
           ,[dCreatedDate]
           ,[iIsEncrypted])
     VALUES
           (0,0,'FileAttachmentPathAWS','CountProcess','s3://idms-7933-webfiles/Other/',1,'',1,'SYSTEM',GetDate(),0),
		   (188,0,'AWSAUTOTASKS','CountProcess','1',1,'',1,'SYSTEM',GetDate(),0);
