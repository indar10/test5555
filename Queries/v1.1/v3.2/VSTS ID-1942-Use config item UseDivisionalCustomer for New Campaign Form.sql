/****** Script for SelectTopNRows command from SSMS  ******/
  -- ====================================================================      
-- Author: Saarthak Pande
-- Description: Add new config item "UseDivisionalCustomer" for loading divisonal
-- new campaign when divisional mailer is not selected
-- Appropriate division Ids needs to be added for production environment.  
-- VSTS ID - 1942 - Use config item "UseDivisionalCustomer" for New Campaign Form
-- =====================================================================

INSERT INTO [DW_Admin].[dbo].[tblConfiguration]
           ([DivisionID]
           ,[DatabaseID]
           ,[cItem]
           ,[cDescription]
           ,[cValue]
           ,[iValue]
           ,[dValue]
           ,[mValue]
           ,[iIsActive]
           ,[cCreatedBy]
           ,[cModifiedBy]
           ,[dCreatedDate]
           ,[iIsEncrypted])
           VALUES
           (188
           ,0
           ,'UseDivisionalCustomer'
		   ,'WebProcess'
		   ,'181,182,183,184,185,186,188'
           ,0
           ,null
           ,''
           ,1
           ,'SYSTEM'
           ,''
           ,GETDATE()
           ,0)

