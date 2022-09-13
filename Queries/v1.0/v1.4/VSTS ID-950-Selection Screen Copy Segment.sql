USE [DW_Admin]
GO
/****** Object:  StoredProcedure [dbo].[usp_CopySegmentsOtherOrder_NG]    Script Date: 8/7/2019 5:58:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================  
-- Author:  Saarthak Pande  
-- Create date: Aug-7-2019  
-- Description: Added output parameter 'SegmentIDIdentity' on stored procedure 'usp_CopySegmentsOtherOrder'
/*  
* * *********************UPDATION HISTORY******************************  
 * Updated By  Updated Date   Comments  
 * ------------------------------------------------------------  
-- =============================================  
*/  
  
CREATE PROCEDURE [dbo].[usp_CopySegmentsOtherOrder_NG]  
 -- Add the parameters for the stored procedure here  
 @ToOrderID as INT,  
 @FromOrderID as INT,  
 @InitiatedBy as VARCHAR(50),  
 @SegmentHashList VARCHAR(max),  
 @SegmentDesc as VARCHAR(50) = NULL , -- Allow to change Segment Descriptions while copying Segment  
 @KeyCode1 as VARCHAR(50) = NULL , -- Allow to change Segment Keycode1 while copying Segment  
 @KeyCode2 as VARCHAR(15) = NULL , -- Allow to change Segment Keycode2 while copying Segment  
 @MaxPerGroup as CHAR(2) = NULL , -- Allow to change Segment MaxPerGroup while copying Segment  
 @NoOfSegmentsAdded INT OUTPUT , 
 @SegmentIDIdentity INT OUTPUT -- Newly created segment ID 

AS  
BEGIN  
 DECLARE @BuildYYYYMM char(6)  
   
   
 DECLARE @spot INT, @str VARCHAR(8000), @sql VARCHAR(8000) ,  @SegmentID INT, @BuildID INT , @OldBuild INT ,  @MaxDedupe INT  
 SET @NoOfSegmentsAdded = 0  
   
 SELEct @OldBuild=BuildID from tblOrder where ID =@FromOrderID  
 SELEct @BuildID=BuildID from tblOrder where ID =@ToOrderID  
   
 Select @BuildYYYYMM = cBuild from tblBuild where ID= @BuildID  
   
    WHILE @SegmentHashList <> CAST('' AS varchar(MAX))   
    BEGIN   
      
        SET @spot = CHARINDEX(',', @SegmentHashList)   
        IF @spot>0   
            BEGIN   
                SET @str = CAST(LEFT(@SegmentHashList, @spot-1) AS INT)   
                SET @SegmentHashList = RIGHT(@SegmentHashList, LEN(@SegmentHashList)-@spot)   
            END   
        ELSE   
            BEGIN   
                SET @str = CAST(@SegmentHashList AS INT)   
                SET @SegmentHashList = ''   
            END   
        print CONVERT(VARCHAR(100),@str)  
          
  SET @SegmentID = 0  
    
  SELECT @SegmentID = ID from tblSegment WHERE iDedupeOrderSpecified = CAST(@str AS INT) AND OrderID = @FromOrderID  
    
  IF @SegmentID > 0  
  BEGIN  
    
        SELECT @MaxDedupe = ISNULL(max(iDedupeOrderSpecified),0) FROM  tblSegment  WHERE iIsOrderLevel <> 1 and OrderID = @ToOrderID  
        SET @MaxDedupe = @MaxDedupe + 1  
        --Copy Segment  
        DECLARE @Identity as Int   
  INSERT INTO tblSegment(OrderID,cDescription,iUseAutosuppress,cKeyCode1,cKeyCode2,iRequiredQty,iProvidedQty,iAvailableQty,iDedupeOrderSpecified,  
  iDedupeOrderUsed,cMaxPerGroup,cTitleSuppression,cFixedTitle,iDoubleMultiBuyerCount,iIsOrderLevel,  
  dCreatedDate,cCreatedBy,iGroup,iIsRandomRadiusNth)  
  -- Allow to change Segment Descriptions, Keycode1, Keycode2, MaxperGroup while copying Segment  
  SELECT @ToOrderID, ISNULL(@SegmentDesc,RTRIM(LTRIM(LEFT('Copy of ' + cDescription, 50) ))),iUseAutosuppress,
  ISNULL(@KeyCode1,cKeyCode1),
  ISNULL(@KeyCode2,cKeyCode2),
  iRequiredQty,0,0,@MaxDedupe,  
  iDedupeOrderUsed,
  ISNULL(@MaxPerGroup, cMaxPerGroup),cTitleSuppression,cFixedTitle,iDoubleMultiBuyerCount,iIsOrderLevel,  
  GETDATE(), @InitiatedBy,iGroup,iIsRandomRadiusNth FROM  tblSegment WHERE ID=@SegmentID  
    
  -- New ID created when copying Segment   
  SET @Identity  =  @@IDENTITY  

  SET @SegmentIDIdentity  =  @Identity  
      
  -- Copy Lists from the current segment to the copy   
  INSERT INTO tblSegmentList(SegmentID,MasterLOLID,iIsHouseList,dCreatedDate,cCreatedBy)  
  SELECT @Identity,MasterLOLID,iIsHouseList,GETDATE(),@InitiatedBy FROM   
  tblSegmentList WHERE SegmentID = @SegmentID order by tblSegmentList.id  
    
  -- Copy Previous Orders from the current segment to the copy   
  INSERT INTO tblSegmentPrevOrders(SegmentID,PrevOrderID,cIncludeExclude,cCompareFieldName,cPrevSegmentID,cPrevSegmentNumber,dCreatedDate,cCreatedBy)  
  (SELECT @Identity,PrevOrderID,cIncludeExclude,cCompareFieldName,cPrevSegmentID,cPrevSegmentNumber,GETDATE(),@InitiatedBy FROM   
  tblSegmentPrevOrders   
  --Adding Line -AB on Nov 26 so that only Prev Orders from Last 365 days are copied.  
  INNER JOIN tblOrderStatus WITH (NOLOCK) ON tblOrderStatus.OrderID = tblSegmentPrevOrders.PrevOrderID AND tblOrderStatus.iIsCurrent = 1   
  AND  tblOrderStatus.dCreatedDate >= dateadd(day,datediff(day,0,getdate())-365,0)   
  AND tblOrderStatus.iStatus > = 40 AND tblOrderStatus.iStatus <> 150 AND tblOrderStatus.iStatus <> 100 AND tblOrderStatus.iStatus <> 50  
  --End of adding line  
  WHERE SegmentID = @SegmentID)  
    
  IF @BuildID = @OldBuild  
  BEGIN  
   -- Copy Segment selection  
   INSERT INTO tblSegmentSelection(SegmentID,cQuestionFieldName,cQuestionDescription,cJoinOperator,iGroupNumber,iGroupOrder,cGrouping,cValues,cValueMode,  
   cDescriptions,cValueOperator,cFileName,cSystemFileName,dCreatedDate,cCreatedBy,cTableName)  
   SELECT @Identity,cQuestionFieldName,cQuestionDescription,cJoinOperator,iGroupNumber,iGroupOrder,cGrouping,cValues,cValueMode,  
   cDescriptions,cValueOperator,CASE  WHEN (cValueMode = 'F')THEN cFileName ELSE ''END ,  
   CASE  WHEN (cValueMode = 'F') THEN cSystemFileName ELSE ''END,GETDATE(),@InitiatedBy,cTableName  
   FROM tblSegmentSelection WHERE SegmentID = @SegmentID Order BY iGroupNumber, tblSegmentSelection.ID  
  END  
  ELSE  
  BEGIN  
   -- Copy Segment selection  
   INSERT INTO tblSegmentSelection(SegmentID,cQuestionFieldName,cQuestionDescription,cJoinOperator,iGroupNumber,iGroupOrder,cGrouping,cValues,cValueMode,  
   cDescriptions,cValueOperator,cFileName,cSystemFileName,dCreatedDate,cCreatedBy,cTableName)  
   SELECT @Identity,cQuestionFieldName,cQuestionDescription,cJoinOperator,iGroupNumber,iGroupOrder,cGrouping,cValues,cValueMode,  
   cDescriptions,cValueOperator,CASE  WHEN (cValueMode = 'F')THEN cFileName ELSE ''END ,  
   CASE  WHEN (cValueMode = 'F') THEN cSystemFileName ELSE ''END,GETDATE(),@InitiatedBy,  
   CASE  WHEN (cTableName LIKE '%External%') THEN cTableName   
   ELSE  (SUBSTRING(cTableName,1, CHARINDEX('_',cTableName,1)-1)+ '_' +  CAST(@BuildID AS VARCHAR) +  '_' + @BuildYYYYMM) END as cTableName  
   FROM tblSegmentSelection WHERE SegmentID = @SegmentID Order BY iGroupNumber, tblSegmentSelection.ID  
  END  
   
    
  DECLARE @NumberRecordsSubSelects int, @RowCountSubSelects int  
   
  DECLARE @SubSelID INT  
  DECLARE tblTempSubSelect_Cursor CURSOR FOR   
        SELECT ID FROM tblSubSelect WHERE SegmentID = @SegmentID  
          
        OPEN tblTempSubSelect_Cursor  
        FETCH NEXT FROM tblTempSubSelect_Cursor   
    INTO @SubSelID  
        WHILE @@FETCH_STATUS = 0   
            BEGIN  
              
            INSERT INTO tblSubSelect(SegmentID,cIncludeExclude,cCompanyIndividual,dCreatedDate,cCreatedBy)  
   (SELECT @Identity,cIncludeExclude,cCompanyIndividual,GETDATE(),@InitiatedBy FROM   
   tblSubSelect WHERE ID = @SubSelID)  
    
   DECLARE @SubSetIdentity as Int   
   -- New ID created when copying Segment   
   SET @SubSetIdentity  =  @@IDENTITY  
     
   -- Copy Lists from the current segment to the copy   
   INSERT INTO tblSubSelectList(SubSelectID,MasterLOLID,iIsHouseList,dCreatedDate,cCreatedBy)  
   SELECT @SubSetIdentity,tblSubSelectList.MasterLOLID,iIsHouseList,GETDATE(),@InitiatedBy FROM   
   tblSubSelectList INNER JOIN tblOrderCASApproval   
   ON tblSubSelectList.MasterLOLID = tblOrderCASApproval.MasterLOLID AND tblOrderCASApproval.OrderID = @ToOrderID  
   WHERE SubSelectID = @SubSelID  order by tblSubSelectList.id  
     
     
   IF @BuildID = @OldBuild  
   BEGIN  
    -- Copy Selection from the current subselection to the copy   
    INSERT INTO tblSubSelectSelection(SubSelectID,cQuestionFieldName,cQuestionDescription,cJoinOperator,iGroupNumber,iGroupOrder,  
    cGrouping,cValues,cValueMode,cDescriptions,cValueOperator,cTableName,dCreatedDate,cCreatedBy)  
    SELECT @SubSetIdentity,cQuestionFieldName,cQuestionDescription,cJoinOperator,iGroupNumber,iGroupOrder,  
    cGrouping,cValues,cValueMode,cDescriptions,cValueOperator,cTableName,GETDATE(),@InitiatedBy FROM   
    tblSubSelectSelection WHERE SubSelectID = @SubSelID Order BY iGroupNumber, tblSubSelectSelection.ID  
   END  
   ELSE  
   BEGIN  
    -- If old and new build is not same but copy the main build table name from the newer build  
    INSERT INTO tblSubSelectSelection(SubSelectID,cQuestionFieldName,cQuestionDescription,cJoinOperator,iGroupNumber,iGroupOrder,  
    cGrouping,cValues,cValueMode,cDescriptions,cValueOperator,cTableName,dCreatedDate,cCreatedBy)  
    SELECT @SubSetIdentity,cQuestionFieldName,cQuestionDescription,cJoinOperator,iGroupNumber,iGroupOrder,  
    cGrouping,cValues,cValueMode,cDescriptions,cValueOperator,  
    CASE WHEN (cTableName LIKE '%External%') THEN cTableName   
    ELSE (SUBSTRING(cTableName,1, CHARINDEX('_',cTableName,1)-1)+ '_' +  CAST(@BuildID AS VARCHAR) +  '_' + @BuildYYYYMM) END as cTableName,  
    GETDATE(),@InitiatedBy FROM   
    tblSubSelectSelection WHERE SubSelectID = @SubSelID Order BY iGroupNumber, tblSubSelectSelection.ID  
      
   END  
   FETCH NEXT FROM tblTempSubSelect_Cursor   
    INTO @SubSelID  
                                                  
            END  
        CLOSE tblTempSubSelect_Cursor  
        DEALLOCATE tblTempSubSelect_Cursor  
  SET @NoOfSegmentsAdded = @NoOfSegmentsAdded + 1  
  SET @SegmentID = 0  
  END  
    END   
   
END  
  
 