  -- ====================================================================      
-- Author: Saarthak Pande
-- Description: Added DatabaseID column to tblState
-- VSTS ID - 1527 - Advanced Search : County/City Search
-- =====================================================================

ALTER TABLE tblState 
ADD DatabaseID int NOT NULL DEFAULT(0)
 
-- Look Up for tblState Caching

INSERT INTO tblLookup
(cLookupValue, iOrderBy, cCode, cDescription, cField, mField, iField, iIsActive, cCreatedBy, dCreatedDate)
VALUES ('REDISCACHEKEYS', 6, 'State', 'State', '', '', 0, 1, 'SYSTEM', GETDATE())  