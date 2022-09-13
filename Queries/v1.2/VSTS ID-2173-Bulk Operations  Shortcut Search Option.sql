  -- ====================================================================      
-- Author: Saarthak Pande
-- Description: Inserted JSON for Bulk Operations Shortcut Search in tblLookup.mField
-- VSTS ID - 2173 - Bulk Operations : Shortcut Search Option
-- =====================================================================

 INSERT INTO tblLookup (cLookupValue, iOrderBy, cCode, cDescription, cField, mField, iField, iIsActive, cCreatedBy, dCreatedDate)
 VALUES ('SHORTCUTSEARCH', 1, '0', '', '', 
 '{"id":"BULK_OPERATIONS","examplehelptexts":[{"text":"Search for Key Code 1 like emails, k:emails"},{"text":"Search for Key Code 2 like emails, k2:emails"}],"searchfields":[{"shortcut":"K","fieldnametext":"S.cKeyCode1","fieldnamenumber":"","fielddescription":"Key Code 1"},{"shortcut":"K2","fieldnametext":"S.cKeyCode2","fieldnamenumber":"","fielddescription":"Key Code 2"},{"shortcut":"N","fieldnametext":"","fieldnamenumber":"S.iDedupeOrderSpecified","fielddescription":"Segment #"},{"shortcut":"D","fieldnametext":"S.cDescription","fieldnamenumber":"","fielddescription":"Segment Description"}]}', 
 0, 1, 'SYSTEM', GETDATE()); 