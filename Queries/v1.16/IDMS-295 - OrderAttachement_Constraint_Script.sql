  -- ====================================================================      
-- Author: Rohit Pandey
-- Description: Add a constraint for table tblOrderAttachment. 
-- IDMS-295 -  Campaign UI : Multiple files uploading for same attachment type
-- =====================================================================

USE [DW_Admin]
GO

-- Query for deleting duplicate entries from [tblOrderAttachment]

delete from tblOrderAttachment where ID IN(
select ID from (
select *,ROW_NUMBER() over(partition by OrderID,LK_AttachmentType order by dCreatedDate desc) AS RN from tblOrderAttachment) a
where RN != 1)


-- Query for adding constraint for [tblOrderAttachment]

ALTER TABLE tblOrderAttachment
ADD CONSTRAINT UK_OrderAttachment_1 UNIQUE (OrderID,LK_AttachmentType);

