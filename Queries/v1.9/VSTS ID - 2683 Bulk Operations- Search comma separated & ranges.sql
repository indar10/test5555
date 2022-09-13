  -- ====================================================================      
-- Author: Vishwakumar Ghanchekar
-- Description: Updated JSON for Bulk operation in tblLookup.mField added example for comma seperated and range for segment #
-- VSTS ID - 2683 - Bulk Operations: Search comma separated & ranges
-- =====================================================================

UPDATE [dbo].[tblLookup]
   SET [mField] = '{"id":"BULK_OPERATIONS","examplehelptexts":[{"text":"Search for segment # with range and comma, 1-3,5,7,9-12"},{"text":"Search for Key Code 1 like emails, k:emails"},{"text":"Search for Key Code 2 like emails, k2:emails"}],"searchfields":[{"shortcut":"K","fieldnametext":"S.cKeyCode1","fieldnamenumber":"","fielddescription":"Key Code 1"},{"shortcut":"K2","fieldnametext":"S.cKeyCode2","fieldnamenumber":"","fielddescription":"Key Code 2"},{"shortcut":"N","fieldnametext":"","fieldnamenumber":"S.iDedupeOrderSpecified","fielddescription":"Segment #"},{"shortcut":"D","fieldnametext":"S.cDescription","fieldnamenumber":"","fielddescription":"Segment Description"}]}'   
      ,[cModifiedBy] = 'SYSTEM'
      ,[dModifiedDate] = GETDATE()
 WHERE [cLookupValue] = 'SHORTCUTSEARCH' and [cCode] = '0'
GO


