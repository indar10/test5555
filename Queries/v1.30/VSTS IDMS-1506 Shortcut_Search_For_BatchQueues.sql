INSERT INTO tblLookup (cLookupValue, iOrderBy, cCode, cDescription, cField, mField, iField, iIsActive, cCreatedBy, dCreatedDate)
 VALUES ('SHORTCUTSEARCH', 9, '9', '', '', 
  '{"id": "BatchProcessQueue","examplehelptexts": [{"text": "Batch Process Queues of QueueID 38745 and ProcessType AUD, QID:38745 AND PR:AUD"}],"searchfields": [

{"shortcut": "QID","fieldnametext": "","fieldnamenumber": "BQ.ID","fielddescription": "Queue ID"},
{"shortcut": "PR", "fieldnametext": "LKPT.cDescription", "fieldnamenumber": "", "fielddescription": "Process Type " }, 
{ "shortcut": "S", "fieldnametext": "LKQS.cDescription", "fieldnamenumber": "", "fielddescription": "Status" }, 
{ "shortcut": "AK", "fieldnametext": "BQ.FieldName", "fieldnamenumber": "", "fielddescription": "Audit Key" },
{ "shortcut": "CI", "fieldnametext": "BQ.ParmData", "fieldnamenumber": "", "fielddescription": "Count Id" },
{ "shortcut": "D", "fieldnametext": "DB.cDatabaseName", "fieldnamenumber": "", "fielddescription": "Database" }  ,
{ "shortcut": "BU", "fieldnametext": "BLD.cDescription" ,"fieldnamenumber": "", "fielddescription": "Build"},
{ "shortcut": "SB", "fieldnametext": "BQ.cScheduledBy" , "fieldnamenumber": "", "fielddescription": "Scheduled By"},
{ "shortcut": "SD", "fieldnametext": "BQ.dScheduled"  , "fieldnamenumber": "", "fielddescription": "Scheduled Date"}
]}', 
 0, 1, 'SYSTEM', GETDATE());