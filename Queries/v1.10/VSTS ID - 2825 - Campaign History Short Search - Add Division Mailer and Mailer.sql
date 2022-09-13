-- ===========================================================================================================      
-- Author: Saarthak Pande
-- Description: Added Mailer and Customer to Campaign History Short Search.
-- VSTS ID - 2825 Campaign History Short Search - Add Division Mailer and Mailer
-- ===========================================================================================================

UPDATE tblLookup
SET mField = '{
  "id": "CAMPAIGNHISTORY",
  "examplehelptexts": [
    {
      "text": "Campaigns for description ABC, D:ABC"
    },
	{
      "text": "Campaigns for ID 123456, D:123456"
    }
  ],
  "searchfields": [
    {
      "shortcut": "D",
      "fieldnametext": "O.cDescription",
      "fieldnamenumber": "O.ID",
      "fielddescription": "Campaign Description/ID"
    },
    {
      "shortcut": "P",
      "fieldnametext": "O.cLVAOrderNo",
      "fieldnamenumber": "",
      "fielddescription": "PO #"
    },
    {
      "shortcut": "M",
      "fieldnametext": "M.cCompany",
      "fieldnamenumber": "M.ID",
      "fielddescription": "Mailer Name/ID (Data Warehouse & Apogee Only)"
    },
   {
      "shortcut": "C",
      "fieldnametext": "DM.cCompany",
      "fieldnamenumber": "DM.ID",
      "fielddescription": "Customer Name/ID"
    }
  ]
}'
WHERE cLookupValue = 'SHORTCUTSEARCH'
AND cCode = 5;
