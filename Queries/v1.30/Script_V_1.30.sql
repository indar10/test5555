USE [DW_Admin] 
GO 

/*-- Update version to 1.30**/ 

UPDATE tblConfiguration 
SET cValue = '1.30', 
cModifiedBy = 'SYSTEM', 
dModifiedDate = getdate() 
WHERE cItem = 'DataLynxBuildVersion' 

/**********************/ 

GO 