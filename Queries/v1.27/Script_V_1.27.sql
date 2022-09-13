USE [DW_Admin] 
GO 

/*-- Update version to 1.27**/ 

UPDATE tblConfiguration 
SET cValue = '1.27', 
cModifiedBy = 'SYSTEM', 
dModifiedDate = getdate() 
WHERE cItem = 'DataLynxBuildVersion' 

/**********************/ 

GO 