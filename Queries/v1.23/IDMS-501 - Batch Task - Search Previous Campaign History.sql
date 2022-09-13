-- ====================================================================================================      
-- Author: Saarthak Pande
-- Description: Config for Generic Downlaod Page's URL
-- User Story - IDMS-501- NUA | App Refactor : Batch Task - Search Previous Campaign History
-- =====================================================================================================

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
    (0,0,'S3DownloadURL','Web Processing','https://download1.idms.data-axle.com/download/?key=',0,'',1,'SYSTEM',GetDate(),0);
