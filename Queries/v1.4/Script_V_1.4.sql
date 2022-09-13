USE [DW_Admin] 
GO 

/*-- Update version to 1.4**/ 

UPDATE tblConfiguration 
SET cValue = '1.4', 
cModifiedBy = 'SYSTEM', 
dModifiedDate = getdate() 
WHERE cItem = 'DataLynxBuildVersion' 

/**********************/ 

GO 