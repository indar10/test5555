USE [DW_Admin] 
GO 

/*-- Update version to 1.29**/ 

UPDATE tblConfiguration 
SET cValue = '1.29', 
cModifiedBy = 'SYSTEM', 
dModifiedDate = getdate() 
WHERE cItem = 'DataLynxBuildVersion' 

/**********************/ 

GO 