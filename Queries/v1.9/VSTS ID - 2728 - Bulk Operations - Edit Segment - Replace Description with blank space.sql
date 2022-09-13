-- ====================================================================================================================              
-- Author:  Jayesh Patel              
-- Create date: Mar-10-2020              
-- Description: Bulk Operation Actions for vNext.           
                       
/*            
* * *********************UPDATION HISTORY******************************            
 * Updated By       Updated Date     Comments            
 * ------------------------------------------------------------            
 * Jayesh Patel     24-Jun-2020      Allowed empty string for @ReplaceValue param when used for tblSegment.cDescription            
 * Saarthak Pande   26-Jun-2020      Added History Section    
 ======================================================================================================================              
*/ 

USE [DW_Admin]
GO

/****** Object:  StoredProcedure [dbo].[usp_OrderBulkOperations]    Script Date: 6/26/2020 10:12:12 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



ALTER PROCEDURE [dbo].[usp_OrderBulkOperations]
    @Action varchar(50), -- (APPEND_RULES, DELETE_RULES, DELETE_SEGMENTS, EDIT_SEGMENTS, ADD_TO_FAVORITES)
    @OrderID int, -- Order ID
    @UserID varchar(50), -- User taking action
    @SourceSegment int, -- Segment ID to copy data from. Null in case of actions like DELETE_SEGMENTS, EDIT_SEGMENTS
    @TargetSegments varchar(max), -- Comma Separated Segment IDs
    @SearchValue varchar(max), -- Values to Find. Used for EDIT_SEGMENTS only. Null for other actions
    @ReplaceValue varchar(max), -- Values to Replace. Used for EDIT_SEGMENTS only. Null for other actions
    @FieldName varchar(100) -- Values to Find. Used for EDIT_SEGMENTS only. Null for other actions
AS

IF @Action IS NULL OR (@Action NOT IN ('APPEND_RULES', 'DELETE_RULES', 'DELETE_SEGMENTS', 'EDIT_SEGMENTS', 'FIND_REPLACE','ADD_TO_FAVORITES')) 
  THROW 50001, 'Invalid Action.', 1;

IF @OrderID IS NULL OR @OrderID <= 0 
  THROW 50002, 'Campaign ID is missing.', 1;

IF @Action <> 'ADD_TO_FAVORITES' AND (@TargetSegments IS NULL OR LEN(@TargetSegments) <= 6) 
  THROW 50003, 'Segment ID is missing.', 1;

IF @UserID IS NULL OR LEN(@UserID) <= 3 
  THROW 50004, 'User ID is missing.', 1;

DECLARE @RowsAffected int, @ReturnMessage varchar(max), @SQL varchar(max)
SET @RowsAffected = 0 
SET @ReturnMessage = 'Under Construction'

-- Search Value can be null or empty
IF @Action = 'EDIT_SEGMENTS' AND @FieldName IS NOT NULL --AND @ReplaceValue IS NOT NULL AND LEN(RTRIM(@ReplaceValue)) > 0 
BEGIN
	-- Check Input Rules
	IF @FieldName = 'cDescription' AND (@SearchValue IS NULL OR LEN(LTRIM(@SearchValue)) = 0) AND (@ReplaceValue IS NULL OR LEN(RTRIM(@ReplaceValue)) = 0)
	  THROW 50005, 'Segment Description is required.', 1

    SET @SQL = 'UPDATE tblSegment ' +
               '   SET ' + @FieldName + ' = ' + CASE WHEN @SearchValue IS NULL OR RTRIM(@SearchValue) = ''  THEN '''' + REPLACE(@ReplaceValue,'''','''''') + '''' ELSE 'CASE WHEN LEN(REPLACE(' + @FieldName + ',''' + REPLACE(@SearchValue,'''','''''') + ''',''' + REPLACE(@ReplaceValue,'''','''''') + ''')) = 0 THEN ''S'' ELSE REPLACE(' + @FieldName + ',''' + REPLACE(@SearchValue,'''','''''') + ''',''' + REPLACE(@ReplaceValue,'''','''''') + ''') END' END + 
               '       ,dModifiedDate = GETDATE(), cModifiedBy = ''' + @UserID + '''' +
               ' WHERE ID IN (' + ISNULL(@TargetSegments,'') + ')'

    EXEC(@SQL) 

    SET @ReturnMessage = CAST(@@ROWCOUNT as varchar) + ' segments updated.'
END


IF @Action = 'DELETE_RULES' AND @SourceSegment IS NOT NULL
BEGIN
    -- TBD: The following logic may not work to delete more than one delete rules. We may have to process one segment at a time in a CURSOR loop on @TargetSegments. 
    -- It deletes rules evenif Group # and/or Join Operator don't match.
    DELETE a
      FROM tblSegmentSelection a
     INNER JOIN (
                    SELECT cQuestionFieldName, cast(cValues as varchar(max)) cValues, cValueMode, cValueOperator, cFileName
                      FROM tblSegmentSelection
                     WHERE SegmentID = @SourceSegment
                 INTERSECT
                    SELECT cQuestionFieldName, cast(cValues as varchar(max)) cValues, cValueMode, cValueOperator, cFileName
                      FROM tblSegmentSelection
                     WHERE SegmentID in (SELECT CAST(value as int) SegmentID FROM STRING_SPLIT(@TargetSegments, ','))
                ) b
        ON a.cQuestionFieldName = b.cQuestionFieldName
       AND CAST(a.cValues as varchar(max)) = b.cValues
       AND a.cValueMode = b.cValueMode
       AND a.cValueOperator = b.cValueOperator
       AND a.cFileName = b.cFileName
     WHERE a.SegmentID IN (SELECT CAST(value as int) SegmentID FROM STRING_SPLIT(@TargetSegments, ','))
      
    SET @ReturnMessage = CAST(@@ROWCOUNT as varchar) + ' rules deleted.'
END

IF @Action = 'ADD_TO_FAVORITES' AND @SourceSegment IS NOT NULL
BEGIN
    INSERT INTO tblUserSavedSelectionDetail 
           (UserSavedSelectionID, cQuestionFieldName, cQuestionDescription, cJoinOperator, iGroupNumber, iGroupOrder, cGrouping, cValues, cValueMode, cDescriptions, cValueOperator, iIsActive, cTableName, dCreatedDate, cCreatedBy)
    SELECT @OrderID,              cQuestionFieldName, cQuestionDescription, cJoinOperator, iGroupNumber, iGroupOrder, cGrouping, cValues, cValueMode, cDescriptions, cValueOperator, 1, cTableName, GETDATE(), @UserID
      FROM tblSegmentSelection a
     WHERE a.SegmentID = @SourceSegment
     ORDER BY a.iGroupNumber, a.ID

    SET @ReturnMessage = CAST(@@ROWCOUNT as varchar) + ' rules added.'
END


IF @Action = 'APPEND_RULES' AND @SourceSegment IS NOT NULL
BEGIN
    -- Select Max Group Number from all Target Segments
    DECLARE @MaxGroupNo INT
    SELECT @MaxGroupNo = ISNULL(MAX(iGroupNumber),0)
      FROM tblSegmentSelection a
     WHERE SegmentID IN (SELECT CAST(value as int) SegmentID FROM STRING_SPLIT(@TargetSegments, ',')) 
       AND SegmentID IN (SELECT ID FROM tblSegment WHERE OrderID = @OrderID)
       AND SegmentID <> @SourceSegment

    INSERT INTO tblSegmentSelection
            (SegmentID,cQuestionFieldName,cQuestionDescription,cJoinOperator,iGroupNumber,iGroupOrder,cGrouping,cValues,cValueMode,cDescriptions,cValueOperator,cFileName,cSystemFileName,cTableName, dCreatedDate,cCreatedBy)
    SELECT b.SegmentID,cQuestionFieldName,cQuestionDescription,cJoinOperator,@MaxGroupNo + iGroupNumber,iGroupOrder,cGrouping,cValues,cValueMode,cDescriptions,cValueOperator,cFileName,cSystemFileName,cTableName, GETDATE(),@UserID
      FROM tblSegmentSelection a
     CROSS JOIN (SELECT CAST(value as int) SegmentID FROM STRING_SPLIT(@TargetSegments, ',') WHERE CAST(value as int) <> @SourceSegment) b
     WHERE a.SegmentID = @SourceSegment
     ORDER BY b.SegmentID, a.iGroupNumber, a.ID

    SET @ReturnMessage = CAST(@@ROWCOUNT as varchar) + ' rules appended.'

--    select * from #nothing;
END

IF @Action = 'DELETE_SEGMENTS'
BEGIN
    DROP TABLE IF EXISTS #tmpSegment
    CREATE TABLE #tmpSegment (SegmentOrder int identity(1,1), SegmentID int)

    DELETE a
      FROM tblSegment a
      JOIN (SELECT CAST(value as int) SegmentID FROM STRING_SPLIT(@TargetSegments, ',')) b
        ON a.ID = b.SegmentID
     WHERE a.OrderID =  @OrderID
       AND a.ID <> ISNULL(@SourceSegment, 0)
       AND a.iIsOrderLevel = 0

    SET @RowsAffected = @@ROWCOUNT
    
    -- Regenerate remaining Segment Dedupe Order
    IF @RowsAffected > 0
    BEGIN
      INSERT INTO #tmpSegment (SegmentID)
      SELECT ID
        FROM tblSegment a
       WHERE OrderID =  @OrderID
         AND iIsOrderLevel = 0
       ORDER BY iDedupeOrderSpecified 
      OPTION (MAXDOP 1)

      UPDATE a
         SET iDedupeOrderSpecified = b.SegmentOrder
        FROM tblSegment a
        JOIN #tmpSegment b 
          ON a.ID = b.SegmentID
       WHERE a.OrderID =  @OrderID
         AND a.iIsOrderLevel = 0
         AND a.iDedupeOrderSpecified <> b.SegmentOrder

    END

    DROP TABLE IF EXISTS #tmpSegment

    SET @ReturnMessage = CAST(@RowsAffected as varchar) + ' segments deleted.'
END


SELECT @ReturnMessage

GO


