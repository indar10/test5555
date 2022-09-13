
USE [DW_Admin]
GO
/*--Update version to 1.8**/
UPDATE tblConfiguration
SET cValue = '1.8',
cModifiedBy = 'SYSTEM',
dModifiedDate = getdate()
WHERE cItem = 'DataLynxBuildVersion'
/**********************/
GO