-- ====================================================================      
-- Author: Sumit Shah
-- Description: Insert Scripts for Configuration Table - AWS migration: Problem with PGP key
-- User Story - IDMS-947 Campaign UI - AWS migration: Problem with PGP key
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
VALUES (188, 0, 'CountProcess', 'PGPKeyFilePath', 'R:\Templates\PGPKeys','',0,1,0,GETUTCDATE(),'jayesh.patel');
