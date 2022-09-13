-- ====================================================================      
-- Author: Abhishek Kalekar
-- Description: Inserted JSON for Match & Append Shortcut Search in tblLookup.mField
-- VSTS ID - 2339 - Match & Append : Shortcut Search Option
-- =====================================================================

INSERT INTO tblLookup (cLookupValue,iOrderBy,cCode,cDescription,cField,mField,iField,iIsActive,cCreatedBy,cModifiedBy,dCreatedDate,dModifiedDate)
VALUES ('SHORTCUTSEARCH',3,3,'','','{
  "id": "MATCHAPPEND",
  "examplehelptexts": [
    {
      "text": "Client Name ABC and Database 992, C:ABC AND DB:992"
    }
  ],
  "searchfields": [
    {
      "shortcut": "DB",
      "fieldnametext": "D.cDatabaseName",
      "fieldnamenumber": "D.ID",
      "fielddescription": "Database Name/ID"
    },
    {
      "shortcut": "C",
      "fieldnametext": "MA.cClientName",
      "fieldnamenumber": "",
      "fielddescription": "Client Name"
    },
    {
      "shortcut": "B",
      "fieldnametext": "B.cDescription + '' (''+ b.cBuild + '')''",
      "fieldnamenumber": "MA.BuildID",
      "fielddescription": "Build Description/ID"
    },
	{
      "shortcut": "D",
      "fieldnametext": "MA.cRequestReason",
      "fieldnamenumber": "",
      "fielddescription": "Description"
    },
    {
      "shortcut": "S",
      "fieldnametext": "",
      "fieldnamenumber": "MAS.iStatusID",
      "fielddescription": "Status (Numbers Only)"
    },
    {
      "shortcut": "SD",
      "fieldnametext": "MAS.dCreatedDate",
      "fieldnamenumber": "",
      "fielddescription": "Status Date (MM/DD/YYYY)"
    }    
  ]
}',0,1,'SYSTEM',NULL,GETDATE(),NULL)