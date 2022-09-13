  -- ====================================================================      
-- ====================================================================      
-- Author: Rohit Pandey
-- Description: Create a database configuration item that allows the admin to define if the campaign history match level default for users in the Campaign UI is at the Individual or Household.  

-- IDMS-1434-  DB Configuration for Campaign History Match Level Default
-- =====================================================================


USE [DW_Admin]
GO

-- Query for inserting configuration data into [tblConfiguration]

insert into tblConfiguration (DivisionId,DatabaseID,cItem,cDescription,cValue,iValue,mValue,iIsActive,cCreatedBy,dCreatedDate,iIsEncrypted)
values(188,0,'MatchLevel','Default Match Level','I',0,'',1,'SYSTEM', getdate(), 0);

insert into tblConfiguration (DivisionId,DatabaseID,cItem,cDescription,cValue,iValue,mValue,iIsActive,cCreatedBy,dCreatedDate,iIsEncrypted)
values(177,0,'MatchLevel','Default Match Level','I',0,'',1,'SYSTEM', getdate(), 0);