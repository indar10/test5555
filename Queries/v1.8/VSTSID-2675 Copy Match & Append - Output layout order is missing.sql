USE [DW_Admin]
GO
/****** Object:  StoredProcedure [dbo].[usp_CopyMatchAppend]    Script Date: 6/12/2020 10:47:45 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================  
-- Author:  Tanmay Patel  
-- Create date: Jul-30-2014  
-- Description: TFS#51552 : Consider iExportType column  
/* * *********************UPDATION HISTORY******************************  
 * Updated By  Updated Date   Comments  
 * ------------------------------------------------------------  
 * Tanmay Patel  18-Dec-2014      TFS# 54171: Match & Append Copy task - It should copy it to the latest build 
 * Tanmay Patel	28-Jun-2016		   TFS# 58426 - Match & Append - Output layout rearranged after a copy 
 * Caroline Burch 20170113 Added sort to tblMatchAppendInputLayout insert
 * Abhishek Kalekar 12-June-2020  VSTS#2675 - Copy Match & Append - Output layout order is missing
  =========================================================================================
*/  

ALTER PROCEDURE [dbo].[usp_CopyMatchAppend](   
 @SourceId as INT,  
 @UserId as varchar(25))  
AS  
BEGIN  
 SET NOCOUNT ON;  
Declare @MatchAppendID AS INT;  
  
 INSERT INTO tblMatchAppend (DatabaseID, BuildID, UploadFilePath, LK_FileType, LK_ExportFileFormatID,   
   cOrderType, cKeyCode, iSkipFirstRow, cClientName, cRequestReason, cSourceFilter, cInputFilter,  
    cIDMSMatchFieldName, cInputMatchFieldName, cClientFileName, dCreatedDate, cCreatedBy, iExportType)  
 SELECT TOP 1 B.DatabaseID, B.ID, UploadFilePath, LK_FileType, LK_ExportFileFormatID,   
   cOrderType, cKeyCode, iSkipFirstRow, cClientName, cRequestReason, cSourceFilter, cInputFilter,  
   cIDMSMatchFieldName, cInputMatchFieldName, cClientFileName, GETDATE(), @UserId,iExportType  
 FROM tblMatchAppend WITH (NOLOCK)  
INNER JOIN  tblBuild B WITH(NOLOCK) ON tblMatchAppend.DatabaseID = B.DatabaseID
 WHERE tblMatchAppend.ID = @SourceId  AND B.iIsOnDisk = 1
ORDER by B.ID Desc 
  
Select @MatchAppendID =  SCOPE_IDENTITY();  
  
INSERT INTO tblMatchAppendStatus (MatchAppendID, iStatusID, iIsCurrent, dCreatedDate, cCreatedBy)  
VALUES (@MatchAppendID, 10, 1, GETDATE(), @UserId)  
  
  
 INSERT INTO tblMatchAppendInputLayout   
   (MatchAppendID, cFieldName, iStartIndex, iEndIndex, iDataLength, iImportLayoutOrder, cMCMapping,  
    dCreatedDate, cCreatedBy)  
 SELECT   
   @MatchAppendID, cFieldName, iStartIndex, iEndIndex, iDataLength, iImportLayoutOrder, cMCMapping,  
    GETDATE(), @UserId  
 FROM tblMatchAppendInputLayout WITH (NOLOCK)  
 WHERE MatchAppendID = @SourceID  
 ORDER BY iImportLayoutOrder
  
  

-- Take all previous output fields in temp table
Select @MatchAppendID AS MatchAppendID, cTableName, cFieldName, iOutputLength, cOutputFieldName,iOutputLayoutOrder, GETDATE() AS dCreatedDate, 'systemcopy' AS cCreatedBy, ID As DispOrderID
 INTO #MATempOut 
 from tblMatchAppendOutputLayout MO with(nolock) where MatchAppendID = @SourceId

-- mark Input table and external table fields to be copied as it is.
update #MATempOut
SET cCreatedBy = 'Ignore'
WHERE cTableName like '%input%' OR cTableName  like '%tblExternal%'

-- Update maching fields from main table and child tables with new table names.
update MO
Set MO.cTableName  = BT.cTableName,
MO.cCreatedBy='Updated'
FROM #MATempOut MO  
INNER JOIN tblMatchAppend M With(NOLOCK) ON MO.MatchAppendID = M.ID
inner join tblBuildTable BT with(nolock) ON BT.BuildID = M.BuildID
inner join tblBuildTableLayout BTL with(nolock) ON BT.ID = BTL.BuildTableID
WHERE MO.cTableName != 'Input' AND LEFT(Mo.cTableName,charindex('_',mo.cTableName)) = LEFT(BT.cTableName,charindex('_',BT.cTableName)) 
AND Mo.cFieldName = BTL.cFieldName

--Insert all fields which are marked as updated or ignored
 INSERT INTO tblMatchAppendOutputLayout   
  (MatchAppendID, cTableName, cFieldName, iOutputLength, cOutputFieldName, dCreatedDate, cCreatedBy, iOutputLayoutOrder)  
 SELECT   
  @MatchAppendID, cTableName, cFieldName, iOutputLength, cOutputFieldName, GETDATE(), @UserId, iOutputLayoutOrder  
 FROM #MATempOut WITH (NOLOCK)  
WHERE cCreatedBy<>'systemcopy'
Order By DispOrderID
 	
  
END  
