USE [DW_Admin]
GO
/*--Update version to 1.16**/
UPDATE tblConfiguration
SET cValue = '1.16',
cModifiedBy = 'SYSTEM',
dModifiedDate = GETDATE()
WHERE cItem = 'DataLynxBuildVersion'
/**********************/
GO
