/****** Script for SelectTopNRows command from SSMS  ******/
  -- ====================================================================      
-- Author: Saarthak Pande
-- Description: Changed values for look item "REDISCACHEKEYS". 
--              It will now store key prefix instead of full keys.
-- VSTS ID - 1908 - Redis Screen changes
-- =====================================================================
DELETE
FROM [tblLookup]
WHERE [cLookupValue] = 'REDISCACHEKEYS'

INSERT INTO tblLookup (cLookupValue, iOrderBy, cCode, cDescription, cField, mField, iField, iIsActive, cCreatedBy, dCreatedDate)
VALUES ('REDISCACHEKEYS', 1, 'CFG', 'Configuration', '', '', 0, 1, 'SYSTEM', GETDATE()),
       ('REDISCACHEKEYS', 2, 'LKP', 'Look Up', '', '', 0, 1, 'SYSTEM', GETDATE()),
	     ('REDISCACHEKEYS', 3, 'SCC', 'State', '', '', 0, 1, 'SYSTEM', GETDATE()),
	     ('REDISCACHEKEYS', 4, 'USR', 'User', '', '', 0, 1, 'SYSTEM', GETDATE())
