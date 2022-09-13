/****** Script for SelectTopNRows command from SSMS  ******/
  -- ====================================================================      
-- Author: Saarthak Pande
-- Description: Industry and County/City Screen will use TblConfiguration.iValue for DataLynx
--              No change in IDMS it will check if the config exists as before 
-- VSTS ID - 1908 - Redis Refactoring : TblConfiguration
-- =====================================================================

-- Updating the database specific config items for 'SearchSICIndustry' and 'StateCitySelection'

  UPDATE [tblConfiguration]
  SET iValue = 1
  WHERE cItem IN ('SearchSICIndustry','StateCitySelection','SearchGeoRadius')
  AND DatabaseID <> 0

