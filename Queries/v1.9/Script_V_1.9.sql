
USE [DW_Admin]
GO
/*--Update version to 1.9**/
UPDATE tblConfiguration
SET cValue = '1.9',
cModifiedBy = 'SYSTEM',
dModifiedDate = getdate()
WHERE cItem = 'DataLynxBuildVersion'
/**********************/
GO