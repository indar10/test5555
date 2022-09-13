USE [DW_Admin]
GO
/*--Update version to 1.21**/
UPDATE tblConfiguration
SET cValue = '1.21',
cModifiedBy = 'SYSTEM',
dModifiedDate = GETDATE()
WHERE cItem = 'DataLynxBuildVersion'
/**********************/
GO
