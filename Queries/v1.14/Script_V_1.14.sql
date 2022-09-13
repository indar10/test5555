USE [DW_Admin]
GO
/*--Update version to 1.14**/
UPDATE tblConfiguration
SET cValue = '1.14',
cModifiedBy = 'SYSTEM',
dModifiedDate = GETDATE()
WHERE cItem = 'DataLynxBuildVersion'
/**********************/
GO
