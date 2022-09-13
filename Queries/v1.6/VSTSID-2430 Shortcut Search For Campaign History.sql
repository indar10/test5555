-- ====================================================================      
-- Author: Abhishek Kalekar
-- Description: Inserted JSON for Campaign History Shortcut Search in tblLookup.mField
-- VSTS ID - 2430 - Campaign History : Shortcut Search Option
-- =====================================================================
INSERT INTO tblLookup (cLookupValue,iOrderBy,cCode,cDescription,cField,mField,iField,iIsActive,cCreatedBy,dCreatedDate)
VALUES ('SHORTCUTSEARCH',5,5,'','','{
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
    }
  ]
}',0,1,'SYSTEM',GETDATE())