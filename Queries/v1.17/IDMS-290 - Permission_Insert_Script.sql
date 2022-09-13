  -- ====================================================================      
-- Author: Rohit Pandey
-- Description: Insert permission entries for Campaign Output Layout and Database Output layout
-- IDMS-290 -  Database User Permissions - Add Campaign and Database Output Layouts
-- =====================================================================

USE [DW_Admin]
GO

-- Query for inserting permission entries for Campaign Output Layout and Database Output layout in [TBLACCESSOBJECT]

insert into TBLACCESSOBJECT (cCode,cDescription,iMainMenuID,iOrderID,cPath,dCreatedDate,cCreatedBy)
values ('CampaignOutputLayout','Campaign Output Layout',99999,9,'',GETDATE(),'SYSTEM'),
('DatabaseOutputLayout','Database Output Layout',99999,10,'',GETDATE(),'SYSTEM')

