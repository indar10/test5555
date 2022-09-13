-- ===========================================================================================================      
-- Author: Saarthak Pande
-- Description: Occupation Search Queries
--              1. Default configuration
--              2. Setting tblConfiguration.iValue = 1 for the active databases for 'OccupationSelection'.
-- VSTS ID - 2566 DL#92 - IG Occupation DB: Advanced Title Search
-- ===========================================================================================================

-- 1. Query for default configuration.

INSERT INTO [tblConfiguration]
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
           (0
           ,0
           ,'OccupationSelection'
           ,'Web Processing'
           ,'0'
           ,0
           ,''
           ,1
           ,'SYSTEM'
           ,GETDATE()
           ,0)
GO

-- 2. This will show occupation option on databases other than 0.

UPDATE [tblConfiguration]
SET [iValue] = 1
WHERE cItem = 'OccupationSelection'
and DatabaseID <> 0


