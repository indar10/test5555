 
INSERT INTO tblLookup (cLookupValue, iOrderBy, cCode, cDescription, cField, mField, iField, iIsActive, cCreatedBy, dCreatedDate)
 VALUES ('SHORTCUTSEARCH', 8, '8', '', '', 
'{"id": "SelectionFieldCountReports","examplehelptexts": [{"text": "SelectionFieldCountReports of FieldName Con and Status 10,
F:Con AND S:10"},{"text": "Status Date for Month and Year, SM:4 AND SY:2020"}],"searchfields": [  
{"shortcut": "F","fieldnametext": "S.cQuestionFieldName","fieldnamenumber": "", "fielddescription": "Field Name"},   
{"shortcut": "S", "fieldnametext": "", "fieldnamenumber": "OS.iStatus",   "fielddescription": "Status And Description" },
{ "shortcut": "SM", "fieldnametext": "", "fieldnamenumber": "MONTH(OS.dCreatedDate)",   "fielddescription": "Status Date Month (Numbers Only)" },  
 { "shortcut": "SD", "fieldnametext": "", "fieldnamenumber": "DAY(OS.dCreatedDate)",   "fielddescription": "Status Date Day (Numbers Only)" },   
 { "shortcut": "SY", "fieldnametext": "", "fieldnamenumber": "YEAR(OS.dCreatedDate)", "fielddescription": "Status Date Year (Numbers Only)" }, ]}',
  0, 1, 'SYSTEM', GETDATE());