  -- ====================================================================      
-- Author: Saarthak Pande
-- Description: Adding Cache Items in the Look
-- VSTS ID - 1643 - Caching for SIC and related Tables
-- =====================================================================

INSERT INTO tblLookup
(cLookupValue, iOrderBy, cCode, cDescription, cField, mField, iField, iIsActive, cCreatedBy, dCreatedDate)
VALUES ('REDISCACHEKEYS', 1, 'Configuration', 'Configuration', '', '', 0, 1, 'SYSTEM', GETDATE()),
       ('REDISCACHEKEYS', 2, 'SICFranchiseCode', 'Franchise Code', '', '', 0, 1, 'SYSTEM', GETDATE()),
	   ('REDISCACHEKEYS', 3, 'IndustryCode', 'Industry Code', '', '', 0, 1, 'SYSTEM', GETDATE()),
       ('REDISCACHEKEYS', 4, 'SICCodeRelated', 'Related SIC Code', '', '', 0, 1, 'SYSTEM', GETDATE()),
	   ('REDISCACHEKEYS', 5, 'SICCode', 'SIC Code', '', '', 0, 1, 'SYSTEM', GETDATE())


