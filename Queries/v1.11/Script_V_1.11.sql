
USE [DW_Admin]
GO
/*--Update version to 1.11**/
UPDATE tblConfiguration
SET cValue = '1.11',
cModifiedBy = 'SYSTEM',
dModifiedDate = getdate()
WHERE cItem = 'DataLynxBuildVersion'
/**********************/
GO