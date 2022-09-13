  -- ====================================================================      
-- Author: Vishwakumar Ghanchekar
-- Description: Inserted JSON for Models Shortcut Search in tblLookup.mField
-- VSTS ID - 2203 - Models : Shortcut Search Option
-- =====================================================================

 INSERT INTO tblLookup (cLookupValue, iOrderBy, cCode, cDescription, cField, mField, iField, iIsActive, cCreatedBy, dCreatedDate)
 VALUES ('SHORTCUTSEARCH', 2, '2', '', '', 
 '{"id": "MODEL","examplehelptexts": [{"text": "Models of Model Name ABC and Database 992, M:ABC AND db:992"}],"searchfields": [{"shortcut": "DB","fieldnametext": "D.cDatabaseName","fieldnamenumber": "D.ID","fielddescription": "Database Name/ID"},{"shortcut": "M", "fieldnametext": "M.cModelName", "fieldnamenumber": "MD.ID", "fielddescription": "Model Name/Model Detail ID" }, { "shortcut": "B", "fieldnametext": "B.cDescription", "fieldnamenumber": "B.ID", "fielddescription": "Build Name/ID" }, { "shortcut": "S", "fieldnametext": "L.cDescription", "fieldnamenumber": "", "fielddescription": "Status (Numbers Only)" } ]}', 
 0, 1, 'SYSTEM', GETDATE());