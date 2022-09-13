  -- ====================================================================      
-- Author: Rohit Pandey
-- Description: Added JSON for Decoy Maintenance Short Search.
-- VSTS ID - 2758 -  Decoy Maintenance
-- =====================================================================

INSERT INTO tblLookup (cLookupValue, iOrderBy, cCode, cDescription, cField, mField, iField, iIsActive, cCreatedBy, dCreatedDate)
 VALUES ('SHORTCUTSEARCH', 6, '6', '', '',
 '{    "id": "DECOY",    
		"examplehelptexts": [      
{"text": "Company Name ABC and Code XYZ12, M:ABC AND C:XYZ12"}, 
{"text": "Active 1, A:1 (1:Active, 0:Inactive)"}    
],
"searchfields": [      
{"shortcut": "M","fieldnametext": "M.CCOMPANY","fieldnamenumber": "M.ID","fielddescription": "Company Name/ID"},
{"shortcut": "C","fieldnametext": "M.CCODE","fieldnamenumber": "","fielddescription": "Code"},      
{"shortcut": "SN","fieldnametext": "D.cName","fieldnamenumber": "","fielddescription": "Seed Name"},   
{"shortcut": "SZ","fieldnametext": "D.cZip","fieldnamenumber": "","fielddescription": "Seed Zip"},      
{"shortcut": "SA","fieldnametext": "D.cAddress","fieldnamenumber": "","fielddescription": "Seed Address"},      
{"shortcut": "A","fieldnametext": "M.IISACTIVE","fieldnamenumber": "","fielddescription": "Active/Inactive (Numbers Only)"}]}',
 0, 1, 'SYSTEM', GETDATE());

