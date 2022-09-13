USE [DW_Admin]
GO
/*-- Update version to 1.3**/
UPDATE tblConfiguration
SET cValue = '1.3',
cModifiedBy = 'SYSTEM',
dModifiedDate = getdate()
WHERE cItem = 'DataLynxBuildVersion'
/**********************/
GO