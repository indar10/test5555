
USE [DW_Admin]
GO
/*--Update version to 1.12**/
UPDATE tblConfiguration
SET cValue = '1.12',
cModifiedBy = 'SYSTEM',
dModifiedDate = getdate()
WHERE cItem = 'DataLynxBuildVersion'
/**********************/
GO