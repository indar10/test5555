USE [DW_Admin]
GO
/*--Update version to 1.24**/
UPDATE tblConfiguration
SET cValue = '1.24',
cModifiedBy = 'SYSTEM',
dModifiedDate = GETDATE()
WHERE cItem = 'DataLynxBuildVersion'
/**********************/
GO
