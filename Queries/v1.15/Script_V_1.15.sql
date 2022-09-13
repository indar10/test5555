USE [DW_Admin]
GO
/*--Update version to 1.15**/
UPDATE tblConfiguration
SET cValue = '1.15',
cModifiedBy = 'SYSTEM',
dModifiedDate = GETDATE()
WHERE cItem = 'DataLynxBuildVersion'
/**********************/
GO
