USE [DW_Admin]
GO
/****** Object:  StoredProcedure [dbo].[usp_AddPrevOrdersToMultipleSegments]    Script Date: 7/14/2020 11:06:27 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =======================================================
-- Author:		Amruta Bhogale
-- Create date: Jun-28-2010
-- Description:	
--              Option 1: Delete All Lists and then insert
--              Option 2: Append Lists if they dont exists
-- ========================================================
/*            
* * *********************UPDATION HISTORY*********************************************************            
 * Updated By       Updated Date     Comments            
 * ------------------------------------------------------------            
 * Saarthak Pande   July-6-2020      Added Changes for vNext's Bulk Operation - Campaign History 
 *                                   1. Added optional parameter @CommaSeparatedIds.
 *                                   2. Added return message
 ======================================================================================================================              
*/ 

ALTER PROCEDURE [dbo].[usp_AddPrevOrdersToMultipleSegments]
	-- Add the parameters for the stored procedure here
	@OrderID as INT,
	@cIncludeExclude as char(1),
	@cCompareFieldName as char(1),
	@CommaSeparatedNumbers as VARCHAR(8000),
	@Option as INT,
	@Content as VARCHAR(8000),
	@InitiatedBy as VARCHAR(25),
	@CommaSeparatedIds as VARCHAR(8000) = NULL  -- Segment Ids from vNext Bulk operations screen.   
AS
BEGIN
	DECLARE @SELECTEDSEGMENTS TABLE (
	SEGMENTID INT )
	DECLARE @SELECTEDLISTS TABLE (
	OrderID INT )
	DECLARE @Action varchar(10), @ReturnMessage varchar(max)
	SET @Action = 'appended.'
	--Fetch the selected segments
	IF (@CommaSeparatedIds IS NULL)
    BEGIN
		INSERT @SELECTEDSEGMENTS exec ('Select ID from tblSegment where OrderID =' + @OrderID + ' and iDedupeOrderSpecified in (' + @CommaSeparatedNumbers  + ')')
	END
    ELSE
    BEGIN
	   INSERT INTO @SELECTEDSEGMENTS (SEGMENTID) SELECT CAST(VALUE AS INT) FROM  STRING_SPLIT(@CommaSeparatedIds,',')
    END
	--If option is append or replace
	IF ( @Option  = 1  OR @Option  = 2)
	BEGIN  
		IF @Option = 1 -- If option is Replace 
		BEGIN
			-- Delete all the lists from selected segments
			SET @Action = 'replaced.'
			DELETE SPO FROM tblSegmentPrevOrders SPO  INNER JOIN  @SELECTEDSEGMENTS SS on SS.SEGMENTID = SPO.SegmentID	
		END
		
		-- If option is replace or append, select the lists and 
		INSERT @SELECTEDLISTS exec ('Select ID from tblOrder where ID in (' + @Content  + ')')
		
		INSERT INTO tblSegmentPrevOrders(SegmentID,PrevOrderID,cIncludeExclude,cCompareFieldName, dCreatedDate,cCreatedBy)
		select SS.SEGMENTID,SL.OrderID, @cIncludeExclude,@cCompareFieldName, GETDATE(),@InitiatedBy 
		FROM @SELECTEDSEGMENTS sS, @SELECTEDLISTS SL WHERE
		NOT EXISTS( select id FROM tblSegmentPrevOrders WHERE  tblSegmentPrevOrders .SegmentID = ss.SEGMENTID AND tblSegmentPrevOrders.PrevOrderID = SL.OrderID) 
		SET @ReturnMessage = CAST(@@ROWCOUNT as varchar) + ' campaign history ' + @Action
	END
	ELSE
	BEGIN
		INSERT @SELECTEDLISTS exec ('Select ID from tblOrder where ID in (' + @Content  + ')')
		DELETE S FROM tblSegmentPrevOrders S  INNER JOIN  @SELECTEDSEGMENTS SS on SS.SEGMENTID = S.SegmentID 
		INNER JOIN @SELECTEDLISTS as SL on SL.OrderID = S.PrevOrderID  	
		SET @ReturnMessage = CAST(@@ROWCOUNT as varchar) + ' campaign history deleted.'
	END
	
END

SELECT @ReturnMessage
