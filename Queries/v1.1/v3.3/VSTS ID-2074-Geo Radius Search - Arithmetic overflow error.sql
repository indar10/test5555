-- =============================================  
-- Author:  Saarthak Pande  
-- Create date: Feb-24-2020  
-- Description: Change @radius type from DECIMAL(5,2) to FLOAT
/*  
* * *********************UPDATION HISTORY******************************  
 * Updated By  Updated Date   Comments  
 * ------------------------------------------------------------  
-- =============================================  
*/  
USE [DW_Admin]
GO

/****** Object:  StoredProcedure [dbo].[usp_GetZipRadius]    Script Date: 2/24/2020 1:56:39 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


ALTER PROCEDURE [dbo].[usp_GetZipRadius]
	-- Add the parameters for the stored procedure here
	@ZipRadiusHashList VARCHAR(max)
AS
BEGIN
	
	DECLARE @spot INT, @str VARCHAR(8000), @sql VARCHAR(8000) , @zip VARCHAR(6), @radius FLOAT, @Type char(3)
	DECLARE @tblTempZipCodes TABLE 
	(
		ZipCode	varchar(6)
	)
	
    WHILE @ZipRadiusHashList <> CAST('' AS varchar(MAX)) 
    BEGIN 
    
        SET @spot = CHARINDEX(',', @ZipRadiusHashList) 
        IF @spot>0 
            BEGIN 
                SET @str = CAST(LEFT(@ZipRadiusHashList, @spot-1) AS VARCHAR) 
                SET @ZipRadiusHashList = RIGHT(@ZipRadiusHashList, LEN(@ZipRadiusHashList)-@spot) 
            END 
        ELSE 
            BEGIN 
                SET @str = CAST(@ZipRadiusHashList AS VARCHAR) 
                SET @ZipRadiusHashList = '' 
            END 
        SET @str = LTRIM(RTRIM(@str))
        
		/*Old Code before Canadian Zip Code Merge
        SET @zip = SUBSTRING(@str,1,5)
        SET @radius = SUBSTRING(@str,7,LEN(@str)-5)
        */
		IF ISNUMERIC(LEFT(@str, 5)) = 1
			BEGIN
				SET @Type = 'USA'
				SET @zip = SUBSTRING(@str,1,5)
				SET @radius = SUBSTRING(@str,7,LEN(@str)-5)
			END
		ELSE
			BEGIN
				SET @Type = 'CAN'
				SET @zip = SUBSTRING(@str,1,6)
				SET @radius = SUBSTRING(@str,8,LEN(@str)-6)
			END

        INSERT INTO @tblTempZipCodes(ZipCode)
		SELECT DISTINCT Z1.cZipCode
		  FROM tblZipRadius AS Z1
		 INNER JOIN tblZipRadius AS Z2
		    ON Z2.cZipCode in (@zip)
		 WHERE Z1.cType = @Type AND Z2.cType = @Type
		   AND (
				(Z1.fLatitude = Z2.FLatitude AND Z1.FLongitude = Z2.FLongitude)
				OR (3963.0 * acos(sin(Z1.FLatitude/57.2958) * sin(Z2.FLatitude/57.2958) + 
				cos(Z1.FLatitude/57.2958) * cos(Z2.FLatitude/57.2958) *  cos((Z2.FLongitude/57.2958 -Z1.FLongitude/57.2958) + .00000001)) <=  @radius)
			   )
    END 
    SELECT DISTINCT ZipCode 
	  FROM @tblTempZipCodes 
END

/*
Call samples
usp_GetZipRadius '60680-250'
usp_GetZipRadius '07663-3'
usp_GetZipRadius '10965-5,10001-2'
usp_GetZipRadius 'M4C1M5-5,V5H3Z7-10'
usp_GetZipRadius 'Y1A7A2-5,V5H3Z7-10'

CREATE INDEX I4Type ON tblZipRadius (cType)
 
ALTER TABLE tblZipRadius ADD cType char(3)

select top 100 * from tblZipRadius
where cType = 'CAN'

UPDATE tblZipRadius SET cType = 'USA'
WHERE ISNUMERIC(cZipCode) = 1

SELECT  distinct cZipCode,fLatitude,fLongitude,cType 
From tempdata.dbo.ZipRadius 


UPDATE tblZipRadius SET cType = 'CAN'
select top 100 *
from tblZipRadius
order by 1 desc

WHERE ISNUMERIC(cZipCode) = 0
*/
GO


