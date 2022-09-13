-- ====================================================================      
-- Author: Sumit Shah
-- Description: Insert Scripts for Configuration Table - AWS migration: Upload Rules - Download template
-- User Story - IDMS-947 Campaign UI - AWS migration: Upload Rules -> Template download gives error
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
VALUES 
	(188, 0, 'Web Processing', 'TemplatePathAWS', 's3://idms-7933-webfiles/Templates/All','',0,1,0,GETUTCDATE(),'sumits');
