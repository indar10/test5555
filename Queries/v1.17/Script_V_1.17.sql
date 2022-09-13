USE [DW_Admin]
GO
/*--Update version to 1.17**/
UPDATE tblConfiguration
SET cValue = '1.17',
cModifiedBy = 'SYSTEM',
dModifiedDate = GETDATE()
WHERE cItem = 'DataLynxBuildVersion'
/**********************/
GO
