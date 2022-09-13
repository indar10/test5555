-- ==============================================================================================================      
-- Author: Saarthak Pande
-- Description: Added Date Filter(Created Date) to tblLook.mField by copying the JSON currenlty used in staging.
-- VSTSID-2220 - Implement Shortcut Search for Campaign List
-- ===============================================================================================================

USE [DW_Admin]
GO
UPDATE tblLookup
   SET mField = '{
  "id": "CAMPAIGN",
  "examplehelptexts": [
    {
      "text": "Campaings for Customer ABC, c:ABC"
    },
    {
      "text": "Campaings for Customer ABC and Database 992, c:ABC AND db:992"
    }
  ],
  "searchfields": [
    {
      "shortcut": "DB",
      "fieldnametext": "d.cDatabaseName",
      "fieldnamenumber": "d.ID",
      "fielddescription": "Database Name/ID"
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
    },
    {
      "shortcut": "P",
      "fieldnametext": "O.cLVAOrderNo",
      "fieldnamenumber": "",
      "fielddescription": "PO #"
    },
    {
      "shortcut": "S",
      "fieldnametext": "",
      "fieldnamenumber": "OS.iStatus",
      "fielddescription": "Campaign Status (Numbers Only)"
    },
    {
      "shortcut": "SD",
      "fieldnametext": "OS.dCreatedDate",
      "fieldnamenumber": "",
      "fielddescription": "Campaign Status Date (MM/DD/YYYY)"
    },
    {
      "shortcut": "D",
      "fieldnametext": "O.cDescription",
      "fieldnamenumber": "O.ID",
      "fielddescription": "Campaign Description/ID"
    }
  ]
}'      
 WHERE cLookupValue = 'SHORTCUTSEARCH' AND cCode = '1'
GO