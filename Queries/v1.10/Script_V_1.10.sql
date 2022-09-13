
USE [DW_Admin]
GO
/*--Update version to 1.10**/
UPDATE tblConfiguration
SET cValue = '1.10',
cModifiedBy = 'SYSTEM',
dModifiedDate = getdate()
WHERE cItem = 'DataLynxBuildVersion'
/**********************/
GO