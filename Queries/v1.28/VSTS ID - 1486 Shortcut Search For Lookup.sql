  -- ====================================================================      
-- Author: Priya Shukla
-- Description: Inserted JSON for Lookup Shortcut Search in tblLookup.mField
-- VSTS ID - 1486 - Lookups : Shortcut Search Option
-- =====================================================================

 INSERT INTO tblLookup (cLookupValue, iOrderBy, cCode, cDescription, cField, mField, iField, iIsActive, cCreatedBy, dCreatedDate)
 VALUES ('SHORTCUTSEARCH', 7, '7', '', '', 
 '{"id": "Lookups","examplehelptexts": [{"text": "Lookups of Lookup Value TAB and ID 31363, L:TAB AND ID:31363"}],"searchfields": [
 {"shortcut": "ID","fieldnametext": "","fieldnamenumber": "l.ID","fielddescription": "Lookup ID"},
 {"shortcut": "L", "fieldnametext": "l.cLookupValue", "fieldnamenumber": "", "fielddescription": "Lookup Value" }, 
 { "shortcut": "D", "fieldnametext": "l.cDescription", "fieldnamenumber": "", "fielddescription": "Lookup Description" }, 
 { "shortcut": "C", "fieldnametext": "l.cCode", "fieldnamenumber": "", "fielddescription": "Lookup Code" },
 {"shortcut": "O","fieldnametext": "l.iOrderBy","fieldnamenumber": "","fielddescription": "Lookup Order By"},
 {"shortcut": "CF","fieldnametext": "l.cField","fieldnamenumber": "","fielddescription": "Lookup character Field"},
 {"shortcut": "IF","fieldnametext": "l.iField","fieldnamenumber": "","fielddescription": "Lookup Integer Field"},
 {"shortcut": "M", "fieldnametext": "l.mFiled", "fieldnamenumber": "", "fielddescription": "Memo Field" }]}', 
 0, 1, 'SYSTEM', GETDATE());