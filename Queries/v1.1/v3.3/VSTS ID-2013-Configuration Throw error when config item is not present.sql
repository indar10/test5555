  -- ====================================================================      
-- Author: Saarthak Pande
-- Description: Values for Database ID = 0
-- VSTS ID - 2013 - Configuration : Throw error when (cItem,databaseId/0) is not present
-- =====================================================================

INSERT INTO tblConfiguration (DivisionID, DatabaseID, cItem, cDescription, cValue, iValue, dValue, mValue, iIsActive, cCreatedBy, dCreatedDate, iIsEncrypted)
VALUES 
(0, 0, 'KEYCODE1OUTPUTLENGTH', 'CountProcess', '15', 0, NULL, '', 1, 'SYSTEM', GETDATE(), 0),
(0, 0, 'SearchGeoRadius', 'CountProcess', '', 0, NULL, '', 1, 'SYSTEM', GETDATE(), 0),
(0, 0, 'StateCitySelection', 'Web Processing', '', 0, NULL, '', 1, 'SYSTEM', GETDATE(), 0),
(0, 0, 'SearchSICIndustry', 'Web Processing', '', 0, NULL, '', 1, 'SYSTEM', GETDATE(), 0),
(0, 0, 'MaxPerDefaultValue', 'BuildProcess', '', 0, NULL, '', 1, 'SYSTEM', GETDATE(), 0),
(0, 0, 'SegmentLevelMaxPerDefaultValue', 'CountProcess', '', 0, NULL, '', 1, 'SYSTEM', GETDATE(), 0),
(0, 0, 'OrderPOPrefix', 'Prefix Validation for DB', 'NONE', 0, NULL, '', 1, 'SYSTEM', GETDATE(), 0),
(0, 0, 'UseDefaultMailer', 'Web Processing', '0', 0, NULL, '', 1, 'SYSTEM', GETDATE(), 0),
(0, 0, 'UseDefaultOffer', 'Web Processing', '0', 0, NULL, '', 1, 'SYSTEM', GETDATE(), 0),
(0, 0, 'SegmentLevelXTabFields', 'WebProcess', '', 0, NULL, '', 1, 'SYSTEM', GETDATE(), 0),
(0, 0, 'SpecialField', 'CountProcess', '', 0, NULL, '', 1, 'SYSTEM', GETDATE(), 0),
(0, 0, 'QuickCountButton', 'Show Quick Count Button', '', 0, NULL, '', 1, 'SYSTEM', GETDATE(), 0)