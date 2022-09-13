
USE [DW_Admin]
GO
/*--Update version to 1.13**/
UPDATE tblConfiguration
SET cValue = '1.13',
cModifiedBy = 'SYSTEM',
dModifiedDate = getdate()
WHERE cItem = 'DataLynxBuildVersion'
/**********************/
GO