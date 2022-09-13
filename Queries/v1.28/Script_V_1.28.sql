USE [DW_Admin] 
GO 

/*-- Update version to 1.28**/ 

UPDATE tblConfiguration 
SET cValue = '1.28', 
cModifiedBy = 'SYSTEM', 
dModifiedDate = getdate() 
WHERE cItem = 'DataLynxBuildVersion' 

/**********************/ 

GO 