-- ================================================================================      
-- Author: Abhishek Kalekar
-- Description: Added cMatchFieldName column for Campaign History
-- VSTS ID - 2534 Campaign History Changes
-- ================================================================================


ALTER TABLE tblSegmentPrevOrders ADD cMatchFieldName varchar(50) DEFAULT ''