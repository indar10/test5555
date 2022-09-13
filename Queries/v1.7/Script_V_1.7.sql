
USE [DW_Admin]
GO
/*-- Update version to 1.7**/
UPDATE tblConfiguration
SET cValue = '1.7',
cModifiedBy = 'SYSTEM',
dModifiedDate = getdate()
WHERE cItem = 'DataLynxBuildVersion'
/**********************/
GO