USE [DW_Admin]
GO
/*--Update version to 1.18**/
UPDATE tblConfiguration
SET cValue = '1.18',
cModifiedBy = 'SYSTEM',
dModifiedDate = GETDATE()
WHERE cItem = 'DataLynxBuildVersion'
/**********************/
GO
