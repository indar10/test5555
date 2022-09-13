USE [DW_Admin]
GO
/*--Update version to 1.23**/
UPDATE tblConfiguration
SET cValue = '1.23',
cModifiedBy = 'SYSTEM',
dModifiedDate = GETDATE()
WHERE cItem = 'DataLynxBuildVersion'
/**********************/
GO
