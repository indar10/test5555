  -- ====================================================================      
-- Author: Rohit Pandey
-- Description: Add new config item at division level "MaxEmailAttachmentSize" and set it to 20 (MB). 
-- IDMS-249 -  Configure and Limit file upload size for Documents Tab files
-- =====================================================================

USE [DW_Admin]
GO

-- Query for inserting configuration data into [tblConfiguration]

INSERT INTO tblConfiguration (DivisionID,DatabaseID,cItem,cDescription,cValue,
							iValue,mValue,iIsActive,cCreatedBy,dCreatedDate,iIsEncrypted )
 VALUES (0,0,'MaxEmailAttachmentSize','WebProcess',20,0,'',1,'SYSTEM',GETDATE(),0)

