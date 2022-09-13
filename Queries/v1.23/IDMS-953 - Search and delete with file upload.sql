-- ====================================================================      
-- Author: Sumit Shah
-- Description: Batch Task - Search and delete with file upload
-- User Story - IDMS-953 NUA | App Refactor : Batch Task - Search and delete with file upload
-- =====================================================================

USE [DW_Admin];

INSERT INTO tblConfiguration 
	(
		DivisionID, 
		DatabaseID, 
		cDescription,  
		cItem, 
		cValue, 
		mValue, 
		iValue, 
		iIsActive, 
		iIsEncrypted, 
		dCreatedDate, 
		cCreatedBy
	)
VALUES (0, 0, 'BuildProcess', 'PathToSaveSearchDeleteFileAWS', 's3://idms-2722-webfiles/Other/','',0,1,0,GETUTCDATE(),'jayesh.patel');
