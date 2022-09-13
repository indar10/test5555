-- ====================================================================      
-- Author: Sumit Shah
-- Description: Insert Scripts for Configuration Table - AWS migration: Shipping Email failing in AWS workflow
-- User Story - IDMS-1029 Campaign UI - AWS migration: Shipping Email failing in AWS workflow
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
VALUES (188, 0, 'CountProcess', 'FromEmail', 'noreply@idms.data-axle.com','',0,1,0,GETUTCDATE(),'sumits');
