USE [DW_Admin]
GO
/*--Update version to 1.20**/
UPDATE tblConfiguration
SET cValue = '1.20',
cModifiedBy = 'SYSTEM',
dModifiedDate = GETDATE()
WHERE cItem = 'DataLynxBuildVersion'
/**********************/
GO
