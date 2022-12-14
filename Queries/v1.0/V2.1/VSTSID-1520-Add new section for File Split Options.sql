USE [DW_Admin]
GO
/****** Object:  StoredProcedure [dbo].[usp_GetNExportParts]    Script Date: 11/27/2019 11:39:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO








-- =============================================================================================
-- Author:		Amruta Bhogale
-- Create date: Sept-28-2009
-- Description:	Export Parts
-- ==============================================================================================
-- ==============================================================================================
-- Updated by:	Abhishek Kalekar
-- Create date: Nov-27-2019
-- Description:	Export Parts - As for new application i.e. idmsVnext iDedupeOrderSpecified column 
--				needs to displayed on screen. Hence, updated stored procedure to add column in 
--				#tblTempExportPartsNPerSegment. (User story -1520)
-- ==============================================================================================
ALTER PROCEDURE [dbo].[usp_GetNExportParts]
	-- Add the parameters for the stored procedure here
	@OrderID as INT,
	@N as INT
AS
BEGIN

	CREATE Table #tblTempExportPartsNPerSegment (
	SegmentID	int,
	cDescription varchar(50),
	iDedupeOrderSpecified int,
	iProvidedQty int,
	iOutputQty int,
	iQuantity1	int,
	iQuantity2	int,
	iQuantity3	int,
	iQuantity4	int,
	iQuantity5	int,
	iQuantity6	int,
	iQuantity7	int,
	iQuantity8	int,
	iQuantity9	int,
	iQuantity10	int
	)

	Insert into #tblTempExportPartsNPerSegment (SegmentID,iDedupeOrderSpecified,cDescription,iProvidedQty, iOutputQty , iQuantity1,iQuantity2, iQuantity3,iQuantity4,iQuantity5,
	iQuantity6,iQuantity7,iQuantity8,iQuantity9,iQuantity10)
	SELECT  tblSegment.ID, tblSegment.iDedupeOrderSpecified,cDescription, iProvidedQty, CASE WHEN iOutputQty = -1 then iProvidedQty else iOutputQty end , [1],[2],[3],[4],[5],[6],[7], [8],[9], [10]
	FROM  ( SELECT   SegmentID,iQuantity, cPartNo  
          FROM      tblOrderExportPart WHERE tblOrderExportPart.OrderID = @OrderID 
        ) p PIVOT ( MIN(iQuantity)
                    FOR [cPartNo] 
                      IN ([1],[2],[3],[4],[5],[6],[7],[8],[9],[10])
                  ) AS pvt RIGHT JOIN tblSegment ON tblSegment.ID = SegmentID 
	where tblSegment.OrderID  = @OrderID  and tblSegment.iIsOrderLevel = 0 and tblSegment.iOutputQty <> 0 and tblSegment.iProvidedQty <> 0  Order By tblSegment.ID
    	
		
    SELECT * from #tblTempExportPartsNPerSegment         
    
END