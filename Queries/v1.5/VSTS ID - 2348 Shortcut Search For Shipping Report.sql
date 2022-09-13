-- ====================================================================      
-- Author: Pratik Saraf
-- Description: Inserted JSON for Shipping Reports Shortcut Search in tblLookup.mField
-- VSTS ID - 2348 - Shipping Report : Shortcut Search Option
-- =====================================================================

INSERT INTO tblLookup (cLookupValue, iOrderBy, cCode, cDescription, cField, mField, iField, iIsActive, cCreatedBy, dCreatedDate)
 VALUES ('SHORTCUTSEARCH', 4, '4', '', '', 
 '{"id": "ShippedReport","examplehelptexts": [{"text": "Campaigns for Campaign Description and Database 992, D:ABC AND DB:992"},
                                              {"text": "Ship Date for Month and Year, SM:4 AND SY:2020"}],   
				"searchfields": [{"shortcut": "D","fieldnametext": "O.cDescription","fieldnamenumber": "O.ID","fielddescription": "Campaign Description/ID"},                    
				                 {"shortcut": "DB","fieldnametext": "db.cDatabaseName","fieldnamenumber": "db.ID","fielddescription": "Database Name/ID"},                    
								 {"shortcut": "M", "fieldnametext": "M.cCompany", "fieldnamenumber": "M.ID", "fielddescription": "Mailer Name/ID (Data Warehouse & Apogee Only)" },        
								 {"shortcut": "C", "fieldnametext": "DM.cCompany", "fieldnamenumber": "DM.ID", "fielddescription": "Customer Name/ID" },          
								 { "shortcut": "NO", "fieldnametext": "O.iIsNetUse", "fieldnamenumber": "", "fielddescription": "Net Order (1 or 0 numbers  Only)" },         
								 { "shortcut": "NU", "fieldnametext": "O.iIsNoUsage", "fieldnamenumber": "", "fielddescription": "No Usage (1 or 0 numbers Only)" },        
								 { "shortcut": "BP", "fieldnametext": "O.cBrokerPONo", "fieldnamenumber": "", "fielddescription": "Broker PO #" },        
								 { "shortcut": "P", "fieldnametext": "o.cLVAOrderNo", "fieldnamenumber": "", "fielddescription": "PO #" },        
								 { "shortcut": "SM", "fieldnametext": "", "fieldnamenumber": "MONTH(o.dShipDateShipped)", "fielddescription": "Ship Date Month (Numbers Only)" },        
								 { "shortcut": "SY", "fieldnametext": "", "fieldnamenumber": "YEAR(o.dShipDateShipped)", "fielddescription": "Ship Date Year (Numbers Only)" },        
								 {"shortcut": "PQ", "fieldnametext": "", "fieldnamenumber": "O.iProvidedCount", "fielddescription": "Provided Qty (Numbers Only)" },        
								 {"shortcut": "OL", "fieldnametext": "O.cExportLayout", "fieldnamenumber": "", "fielddescription": "Output Layout Name" },        
								 {"shortcut": "B", "fieldnametext": "BR.cCompany", "fieldnamenumber": "BR.ID", "fielddescription": "Broker Name/ID (Data Warehouse & Apogee Only)" },        
								 {"shortcut": "BD", "fieldnametext": "DBR.cCompany", "fieldnamenumber": "DBR.ID", "fielddescription": "Broker Name/ID" },        
								 {"shortcut": "T", "fieldnametext": "F.LK_OfferType", "fieldnamenumber": "", "fielddescription": "Type" },        
								 {"shortcut": "SB", "fieldnametext": "OS.cCreatedBy", "fieldnamenumber": "", "fielddescription": "Shipped By" },        
								 {"shortcut": "CB", "fieldnametext": "O.cCreatedBy", "fieldnamenumber": "", "fielddescription": "Created By" } ]}', 
 0, 1, 'SYSTEM', GETDATE());
