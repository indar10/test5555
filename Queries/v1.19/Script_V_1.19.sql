USE [DW_Admin]
GO
/*--Update version to 1.19**/
UPDATE tblConfiguration
SET cValue = '1.19',
cModifiedBy = 'SYSTEM',
dModifiedDate = GETDATE()
WHERE cItem = 'DataLynxBuildVersion'
/**********************/
GO
