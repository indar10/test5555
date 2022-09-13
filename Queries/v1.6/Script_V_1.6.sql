USE [DW_Admin]
GO
/*-- Update version to 1.6**/
UPDATE tblConfiguration
SET cValue = '1.6',
cModifiedBy = 'SYSTEM',
dModifiedDate = getdate()
WHERE cItem = 'DataLynxBuildVersion'
/**********************/
GO