USE [DW_Admin]
GO
/****** Object:  StoredProcedure [dbo].[usp_CopyOrder]    Script Date: 5/27/2020 12:32:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
    
-- ========================================================================================================              
-- Author:  Amruta Bhogale              
-- Create date: Oct-16-2009              
-- Description: Used make a copy of Order            
            
-- Modified By: Surendra Sharma              
-- Modified date: Dec-05-2013              
-- Description: TFS Id 42674 - To add columns for Order Level Max per group              
/*            
* * *********************UPDATION HISTORY******************************            
 * Updated By  Updated Date   Comments            
 * ------------------------------------------------------------            
 * Tanmay Patel  23-Jan-2014      TFS# 42824: Copy tblOrderPrevOrders content on copy order            
 * Surendra Sharma 08-Apr-2014      TFS# 44657: Copy cChannelType on copy order            
 * Surendra Sharma 28-Jul-2014      TFS# 51553 - Add Output File Label.          
 * Tanmay Patel    11-Oct-2014  TFS# 52719 - Copy xtab report type.          
 * Tanmay Patel    16-Dec-2014     TFS#54172 - Not to copy over ftp info when copying order           
 * Tanmay Patel    13-May-2015    TFS#55533 - Copy SAN number          
 * Ketaki Bhale    9-Oct-2015     TFS#57149 - Export Only Data File - Order Level      
 * Tanmay Patel	  18-Mar-2016		TFS#57814 - Do not copy export layout if source layout is deleted  
 * Tanmay Patel	  18-Mar-2016		TFS#57814 - Do not copy export layout if source layout is deleted  
 * Pratik Saraf	 19-May-2017		TFS#59780 - Added iIsUnCompressed column
 * Pratik Saraf	 31-May-2017		TFS#59780 - Added LK_PGPKeyFile column  
 * Jayesh Patel	 16-March-2018		VSTS#416 - Added iIsRandomRadiusNth column  
 * Jayesh Patel	 26-July-2018		Added logic to switch Apogee Model selection with new table name  
 * Saarthak Pande 19-Oct-2018       VSTS#578 Uppercase & Lowercase Output - Order Level
 * Pratik Saraf	 29-Nov-2018		VSTS#633- Added LK_Media column
 * Abhishek Kalekar 27-May-2020		VSTS#2594 - Adding newly added column for previous order tblSegmentPrevOrders.cMatchFieldName
 =============================================================================================================              
*/            
              
ALTER PROCEDURE [dbo].[usp_CopyOrder]              
 -- Add the parameters for the stored procedure here              
 @OrderID as INT,              
 @MailerID as INT,              
 @OfferID as INT,              
 @BuildID as INT,              
 @UserID as INT,              
 @Description  VARCHAR(50),              
 @InitiatedBy as VARCHAR(25)              
AS              
BEGIN              
 -- SET NOCOUNT ON added to prevent extra result sets from              
 -- interfering with SELECT statements.              
 SET NOCOUNT ON;              
 DECLARE @OrderIdentity Int, @DatabaseID int               
 DECLARE @OrderSplitType Int               
 DECLARE @BuildYYYYMM char(6)              
 DECLARE @SourceExportLayout varchar(50)               
 
 SELECT @BuildYYYYMM = cBuild, @DatabaseID = DatabaseID from tblBuild where ID= @BuildID              
              
 SET @OrderSplitType = 0              
   
SELECT @SourceExportLayout = cExportLayout   
FROM tblOrder with(nolock) where ID = @OrderID  

IF NOT EXISTS(SELECT EL.ID    
    FROM tblExportLayout EL  
    INNER JOIN tblBuild B ON EL.DatabaseID = B.DatabaseID  
    INNER JOIN tblOrder O ON O.BuildID = B.ID  
    INNER JOIN tblOffer F ON F.ID = O.OfferID  
    INNER JOIN tblMailer M ON O.MailerID = M.ID  
    INNER JOIN tblGroupBroker GB ON GB.GroupID = EL.GroupID  
     AND M.BrokerID = GB.BrokerID  
    WHERE EL.iIsActive = 1   
     AND (  
      (  
       F.LK_OfferType = 'P'  
       AND EL.iHasPhone = 0  
       )  
      OR (F.LK_OfferType = 'T')  
      )  
     AND O.ID = @OrderID  
     AND EL.cDescription = @SourceExportLayout  
     )   
		SELECT @SourceExportLayout = ''  
   
               
               
 INSERT INTO tblOrder(BuildID,MailerID,OfferID,cOrderType,UserID,iIsOrder,cDescription,iProvidedCount,iAvailableQty,iIsRandomExecution,iHouseFilePriority,              
 iIsNetUse,dDateLastRun,cSortFields,cOrderNo,cLVAOrderNo,cNextMarkOrderNo,cBrokerPONo,cNotes,iDecoyQty,iDecoyKeyMethod,              
 cDecoyKey,cDecoyKey1,cDecoysByKeycode,cSpecialProcess,cShiptoType,cShipTOEmail,cShipCCEmail,cShipSUBJECT,dShipDateShipped,              
 LK_ExportFileFormatID,dCreatedDate,cCreatedBy,iSplitType,iSplitIntoNParts,cExportLayout,              
 iIsAddHeader,cDatabaseName,iIsNoUsage,cOutputCase,
 --iIsOutputToUpper, replaced by cOutputCase
 cOfferName,DivisionMailerID,DivisionBrokerID,dMailDate,iMailDatePlus,iMailDateMinus,             
 iMinQuantityOrderLevelMaxPer , iMaxQuantityOrderLevelMaxPer , cMaxPerFieldOrderLevel , cChannelType, cFileLabel, cSANNumber 
 ,iIsExportDataFileOnly, iIsUnCompressed, LK_PGPKeyFile
 ,LK_AccountCode,LK_Media,iExportLayoutID)              
 (SELECT @BuildID,@MailerID,@OfferID,cOrderType,@UserID,0,@Description,0,0,iIsRandomExecution,iHouseFilePriority,iIsNetUse,NULL,              
     cSortFields, '','','',cBrokerPONo,cNotes,iDecoyQty,iDecoyKeyMethod,CASE WHEN (iDecoyKeyMethod = 0) THEN cDecoyKey ELSE '' END ,cDecoyKey1,cDecoysByKeycode,cSpecialProcess,              
     cShiptoType,cShipTOEmail,cShipCCEmail,cShipSUBJECT,NULL,LK_ExportFileFormatID,GETDATE(),@InitiatedBy,              
     iSplitType,iSplitIntoNParts,@SourceExportLayout,iIsAddHeader,'',0,cOutputCase,
	 --iIsOutputToUpper,replaced by cOutputCase
	 cOfferName,DivisionMailerID,DivisionBrokerID,dMailDate,iMailDatePlus,iMailDateMinus,            
     iMinQuantityOrderLevelMaxPer , iMaxQuantityOrderLevelMaxPer , cMaxPerFieldOrderLevel , cChannelType, cFileLabel,cSANNumber 
	 ,iIsExportDataFileOnly, iIsUnCompressed, LK_PGPKeyFile
	 ,LK_AccountCode,LK_Media,iExportLayoutID       
       FROM  tblOrder WHERE ID = @OrderID)              
              
 -- New ID created when copying Order               
 SET @OrderIdentity  =  @@IDENTITY              
               
 --INSERT INTO FTP TABLE -Neeraj Jain Jan-19-12              
 --INSERT INTO tblOrderFTP(OrderID,cFTPServer,cUserID,cPassword,cCreatedBy,dCreatedDate)              
 --SELECT @OrderIdentity,cFTPServer,cUserID,cPassword,cCreatedBy,getdate() from tblOrderFTP              
 -- Where OrderID=@OrderID              
                
  -- Copy Export Layout Update: Copy it no matter what dont check for fields etc..Order will fail if field does not exists              
 -- Update on Mar 19 2012              
 IF (ISNULL(@SourceExportLayout,'')<>'')  
BEGIN   
 INSERT INTO tblOrderExportLayout(OrderID,cFieldName,cOutputFieldName, iExportOrder,iWidth, cCalculation, cTableNamePrefix ,dCreatedDate,cCreatedBy)              
 (SELECT @OrderIdentity,cFieldName,cOutputFieldName, iExportOrder, iWidth, cCalculation, cTableNamePrefix ,GETDATE(),@InitiatedBy FROM               
  tblOrderExportLayout WHERE OrderID = @OrderID)              
 END  
               
 SELECT @OrderSplitType=iSplitType FROM tblOrder WHERE ID = @OrderIdentity               
               
 --Get Decoys By Mailer               
 DECLARE @OldMailer as Int               
 DECLARE @OldBuild as Int               
               
 SELECT @OldMailer= MailerID, @OldBuild = BuildID from tblOrder where ID = @OrderID              
               
                
 IF @MailerID = @OldMailer              
 BEGIN              
  -- If old and new mailer are same copy as is               
  INSERT INTO tblOrderDecoy (OrderID,cFirstName,cLastName,cName,cAddress1,cAddress2,cCity,cState,cZip,cCompany,cTitle,cEmail,cPhone,              
  cFax,dCreatedDate,cCreatedBy,cDecoyType,cZip4,cKeyCode1,cDecoyGroup )              
  SELECT @OrderIdentity,D.cFirstName,D.cLastName, D.cName, D.cAddress1, D.cAddress2,               
  D.cCity, D.cState, D.cZip,D.cCompany, D.cTitle, D.cEmail, D.cPhone, D.cFax,GETDATE(),@InitiatedBy, D.cDecoyType,              
  D.cZip4, D.cKeyCode1, D.cDecoyGroup FROM   tblOrderDecoy D WHERE OrderID = @OrderID order by id              
 END              
 ELSE              
 BEGIN              
  INSERT INTO tblOrderDecoy (OrderID,cFirstName,cLastName,cName,cAddress1,cAddress2,cCity,cState,cZip,cCompany,cTitle,cEmail,cPhone,              
  cFax,dCreatedDate,cCreatedBy,cDecoyType,cZip4,cKeyCode1, cDecoyGroup)              
  SELECT @OrderIdentity,D.cFirstName,D.cLastName, D.cName, D.cAddress1, D.cAddress2,               
  D.cCity, D.cState, D.cZip,D.cCompany, D.cTitle, D.cEmail, D.cPhone, D.cFax,GETDATE(),@InitiatedBy, D.cDecoyType,              
  D.cZip4, D.cKeyCode1,D.cDecoyGroup FROM   tblDecoy D WHERE               
  (cDecoyType = 'G' and D.DatabaseID in (Select DatabaseID from tblMailer M where M.ID = @MailerID))              
  OR (MailerID = @MailerID and cDecoyGroup = 'A')              
                
 END              
              
               
              
 -- Copy Export Layout              
 --DECLARE @NotInCurrentBuild as Int               
               
 --SELECT @NotInCurrentBuild = COUNT(*)              
 --FROM tblOrderExportLayout WHERE OrderID = @OrderID              
 --and cFieldName NOT IN (    
 --SELECT tblBuildTableLayout.cFieldName              
 --FROM tblBuild INNER JOIN tblBuildTable ON tblBuildTable.BuildID = tblBuild.ID              
 --INNER JOIN tblBuildTableLayout ON tblBuildTableLayout.BuildTableID = tblBuildTable.ID              
 --LEFT JOIN tblBuildRAWFieldName BR ON BR.BuildTableLayoutID = tblBuildTableLayout.ID AND BR.BuildLOLID = 0              
 --WHERE tblBuild.ID = @BuildID)              
 -- Do not copy it anymore...               
 ----do not copy any records from this table if one field is missing, otherwise the layout will be incomplete.              
 --IF (@NotInCurrentBuild = 0 )              
 --BEGIN              
  --INSERT INTO tblOrderExportLayout(OrderID,cFieldName,cOutputFieldName, iExportOrder,iWidth, cCalculation,dCreatedDate,cCreatedBy)              
  --(SELECT @OrderIdentity,cFieldName,cOutputFieldName, iExportOrder, iWidth,cCalculation,GETDATE(),@InitiatedBy FROM               
   --tblOrderExportLayout WHERE OrderID = @OrderID)              
 --END              
               
               
              
 -- Copy Order Status              
 INSERT INTO tblOrderStatus(OrderID,iStatus,iIsCurrent,cNotes,dCreatedDate,cCreatedBy)              
 VALUES(@OrderIdentity,10,1,'',GETDATE(),@InitiatedBy)              
              
 -- Copy MaxPer              
 INSERT INTO tblOrderMaxPer(OrderID,cGroup,cMaxPerField,iMaxPerQuantity,dCreatedDate,cCreatedBy)              
 SELECT @OrderIdentity,cGroup, cMaxPerField, iMaxPerQuantity,              
  GETDATE(), @InitiatedBy              
  FROM tblOrderMaxPer                
     WHERE  OrderID = @OrderID Order By ID              
              
              
 -- Copy CAS Approval              
 INSERT INTO tblOrderCASApproval(OrderID,MasterLOLID,cStatus,nBasePrice,dCreatedDate,cCreatedBy)              
  (Select @OrderIdentity,tblCASApproval.MasterLOLID,'A', nBasePrice,  GETDATE(),@InitiatedBy               
  from tblCASApproval inner join tblBuildLol on tblCASApproval.MasterLOLID=tblBuildLol.MasterLoLID               
  and tblBuildLol.BuildID= @BuildID and tblBuildLol.LK_Action in ('N','R','O','A')  WHERE cStatus = 'A' and OfferID = @OfferID              
  UNION  Select @OrderIdentity, tblBuildLol.MasterLolID ,'A', case when tblOffer.LK_OfferType='P' then LIST.nBasePrice_Postal when tblOffer.LK_OfferType='T'               
  then LIST.nBasePrice_Telemarketing end as nBasePrice, GETDATE(),@InitiatedBy  From dbo.tblMasterLol as LIST               
  INNER JOIN tblBuildLol ON tblBuildLol.MasterLoLID=LIST.ID  AND tblBuildLol.BuildID=@BuildID and tblBuildLol.LK_Action in ('N','R','O','A')              
  inner join tblListMailer ON tblListMailer.ListID=tblBuildLol.MasterLoLID  INNER JOIN tblOffer on tblOffer.MailerID=tblLISTMailer.MailerID AND tblOffer.ID=@OfferID)              
              
               
 -- Copy X Tab Report              
 INSERT INTO tblOrderXTabReport(OrderID,cXField,cYField,iXTabBySegment, dCreatedDate,cCreatedBy, cType, cSegmentNumbers)              
 SELECT @OrderIdentity, cXField,cYField,iXTabBySegment, GETDATE(),@InitiatedBy, cType, cSegmentNumbers              
 FROM tblOrderXTabReport WHERE  OrderID = @OrderID Order BY tblOrderXTabReport.ID              
               
 -- Copy MultiColumn Tab Report               
 INSERT INTO tblOrderMultiColumnReport(OrderID,cFields,cFieldsDescription,iMultiColBySegment,dCreatedDate,cCreatedBy,cSegmentNumbers)              
 SELECT @OrderIdentity, cFields,cFieldsDescription,iMultiColBySegment, GETDATE(),@InitiatedBy,cSegmentNumbers
 FROM tblOrderMultiColumnReport WHERE  OrderID = @OrderID Order BY tblOrderMultiColumnReport.ID              
               
 --Copy Order Level Previous order            
  INSERT INTO tblOrderPrevOrders  (OrderID, PrevOrderId, cFieldName, cIncludeExclude, dCreatedDate, cCreatedBy)            
  (SELECT @OrderIdentity,PO.PrevOrderId,PO.cFieldName, PO.cIncludeExclude,GETDATE(),@InitiatedBy            
  FROM tblOrderPrevOrders PO WITH(NOLOCK)            
  INNER JOIN tblOrder O WITH(NOLOCK) ON PO.PrevOrderId = O.ID   
  INNER JOIN tblOrderStatus WITH (NOLOCK) ON tblOrderStatus.OrderID = O.ID AND tblOrderStatus.iIsCurrent = 1             
   AND tblOrderStatus.dCreatedDate >= dateadd(day,datediff(day,0,getdate())-365,0)            
   AND tblOrderStatus.iStatus > = 40 AND tblOrderStatus.iStatus NOT IN(150,100,50)            
  WHERE PO.OrderID = @OrderID)            
      
  -- Copy Segments              
 DECLARE @NumberRecordsSegments int, @RowCountSegments int              
               
 DECLARE @tblTempSegmentSelect Table (              
 RowID int IDENTITY(1, 1),               
 SegmentID int NOT NULL)              
               
 INSERT INTO @tblTempSegmentSelect(SegmentID)              
 (SELECT ID  FROM tblSegment WHERE OrderID = @OrderID)              
               
 -- Get the number of records in the temporary table              
 SET @NumberRecordsSegments = @@ROWCOUNT              
 SET @RowCountSegments = 1              
 WHILE @RowCountSegments <= @NumberRecordsSegments              
 BEGIN              
                
  DECLARE @OldSegmentID int              
  Set @OldSegmentID  = (SELECT SegmentID FROM @tblTempSegmentSelect WHERE RowID = @RowCountSegments)              
                
   -- Create Segment            
  INSERT INTO tblSegment(OrderID,cDescription,iUseAutosuppress,cKeyCode1,cKeyCode2,iRequiredQty,iProvidedQty,iAvailableQty,iDedupeOrderSpecified,              
  iDedupeOrderUsed,cMaxPerGroup,cTitleSuppression,cFixedTitle,iDoubleMultiBuyerCount,iIsOrderLevel,dCreatedDate,cCreatedBy, iGroup,iIsRandomRadiusNth)              
  (SELECT @OrderIdentity,cDescription,iUseAutosuppress,cKeyCode1,cKeyCode2,iRequiredQty,0,0,iDedupeOrderSpecified,              
   0,cMaxPerGroup,cTitleSuppression,cFixedTitle,iDoubleMultiBuyerCount,iIsOrderLevel, GETDATE(), @InitiatedBy,iGroup,iIsRandomRadiusNth              
   FROM tblSegment WHERE ID = @OldSegmentID)              
                 
  DECLARE @SegmentIdentity as Int               
  -- New ID created when copying Segment               
  SET @SegmentIdentity  =  @@IDENTITY              
                
                
                
  -- Copy Lists from the current segment to the copy               
 INSERT INTO tblSegmentList(SegmentID,MasterLOLID,iIsHouseList,dCreatedDate,cCreatedBy)              
  (SELECT @SegmentIdentity,tblMasterLoL.ID, (Select case LK_PermissionType when 'H' then 1 else 0 end), GETDATE(), @InitiatedBy FROM               
  tblSegmentList  INNER JOIN tblOrderCASApproval               
  ON tblSegmentList.MasterLOLID = tblOrderCASApproval.MasterLOLID AND tblOrderCASApproval.OrderID = @OrderIdentity              
  INNER JOIN tblMasterLoL on tblMasterLoL.ID = tblOrderCASApproval.MasterLOLID              
  WHERE tblSegmentList.SegmentID = @OldSegmentID)              
                
  IF @BuildID = @OldBuild              
  BEGIN              
   -- Copy Segment selection              
   INSERT INTO tblSegmentSelection
	(SegmentID,cQuestionFieldName,cQuestionDescription,cJoinOperator,iGroupNumber,iGroupOrder,cGrouping,cValues,cValueMode,cDescriptions,cValueOperator,cFileName,cSystemFileName,dCreatedDate,cCreatedBy,cTableName)              
   SELECT @SegmentIdentity,
		  CASE WHEN @DatabaseID = 82 AND @BuildID>= 14509 AND LEFT(cTableName,10) = 'tblChild6_' THEN 'nDeciles' ELSE cQuestionFieldName END,
		  cQuestionDescription,cJoinOperator,iGroupNumber,iGroupOrder,cGrouping,cValues,cValueMode,              
		  cDescriptions,cValueOperator,
		  CASE WHEN (cValueMode = 'F') THEN cFileName ELSE '' END ,              
		  CASE WHEN (cValueMode = 'F') THEN cSystemFileName ELSE ''END,GETDATE(),@InitiatedBy,
		  CASE WHEN @DatabaseID = 82 AND @BuildID >= 14509 AND LEFT(cTableName,10) = 'tblChild6_' THEN (SELECT TOP 1 'tblChild' + CAST(nChildTableNumber AS VARCHAR) + '_' +  CAST(@BuildID AS VARCHAR) +  '_' + @BuildYYYYMM FROM tblModel WHERE cModelName = REPLACE(cQuestionFieldName,'_Decile','')) ELSE cTableName END
    FROM tblSegmentSelection 
   WHERE SegmentID = @OldSegmentID 
   ORDER BY iGroupNumber, tblSegmentSelection.ID              
  END              
  ELSE              
  BEGIN              
   -- Copy Segment selection              
   INSERT INTO tblSegmentSelection(SegmentID,cQuestionFieldName,cQuestionDescription,cJoinOperator,iGroupNumber,iGroupOrder,cGrouping,cValues,cValueMode, cDescriptions,cValueOperator,cFileName,cSystemFileName,dCreatedDate,cCreatedBy,cTableName)    
   SELECT @SegmentIdentity,
		  CASE WHEN @DatabaseID = 82 AND @BuildID >= 14509 AND LEFT(cTableName,10) = 'tblChild6_' THEN 'nDeciles' ELSE cQuestionFieldName END,
		  cQuestionDescription,cJoinOperator,iGroupNumber,iGroupOrder,cGrouping,cValues,cValueMode,              
		  cDescriptions,cValueOperator,CASE  WHEN (cValueMode = 'F')THEN cFileName ELSE ''END ,              
		  CASE WHEN (cValueMode = 'F') THEN cSystemFileName ELSE ''END,GETDATE(),@InitiatedBy,              
		  CASE WHEN (cTableName LIKE '%External%') THEN cTableName 
			   WHEN @DatabaseID = 82 AND @BuildID >= 14509 AND LEFT(cTableName,10) = 'tblChild6_' THEN (SELECT TOP 1 'tblChild' + CAST(nChildTableNumber AS VARCHAR) + '_' +  CAST(@BuildID AS VARCHAR) +  '_' + @BuildYYYYMM FROM tblModel WHERE cModelName = REPLACE(cQuestionFieldName,'_Decile','')) 
			   ELSE (SUBSTRING(cTableName,1, CHARINDEX('_',cTableName,1)-1)+ '_' +  CAST(@BuildID AS VARCHAR) +  '_' + @BuildYYYYMM) END as cTableName              
    FROM tblSegmentSelection 
   WHERE SegmentID = @OldSegmentID 
   ORDER BY iGroupNumber, tblSegmentSelection.ID              
  END              
                
  -- Copy Previous Orders from the current segment to the copy               
  INSERT INTO tblSegmentPrevOrders(SegmentID,PrevOrderID,cIncludeExclude,cCompareFieldName,cPrevSegmentID,cPrevSegmentNumber,cMatchFieldName,dCreatedDate,cCreatedBy)              
  (SELECT @SegmentIdentity,PrevOrderID,cIncludeExclude,cCompareFieldName,cPrevSegmentID,cPrevSegmentNumber,cMatchFieldName,GETDATE(),@InitiatedBy FROM               
  tblSegmentPrevOrders               
  --Adding Line -NJ on Aug-11-10              
  inner join tblOrder on tblOrder.ID=tblSegmentPrevOrders.PrevOrderID              
  --End of adding line              
  --Adding Line -AB on Nov 26 so that only Prev Orders from Last 365 days are copied.              
  INNER JOIN tblOrderStatus WITH (NOLOCK) ON tblOrderStatus.OrderID = tblOrder.ID and tblOrderStatus.iIsCurrent = 1               
  AND tblOrderStatus.dCreatedDate >= dateadd(day,datediff(day,0,getdate())-365,0)               
  AND tblOrderStatus.iStatus > = 40 AND tblOrderStatus.iStatus <> 150 AND tblOrderStatus.iStatus <> 100 AND tblOrderStatus.iStatus <> 50              
  --End of adding line              
  WHERE SegmentID = @OldSegmentID)              
               
              
  -- Copy Split Export Parts if the option is split into N              
  IF (@OrderSplitType = 4)              
  BEGIN              
    INSERT INTO tblOrderExportPart(OrderID,cPartNo,SegmentID,iQuantity,dCreatedDate,cCreatedBy)              
       (SELECT @OrderIdentity,cPartNo, @SegmentIdentity,iQuantity,GETDATE(),@InitiatedBy FROM               
    tblOrderExportPart WHERE SegmentID = @OldSegmentID and OrderID=@OrderID)              
  END              
                 
  DECLARE @NumberRecordsSubSelects int, @RowCountSubSelects int              
         
  DECLARE @SubSelID INT              
  DECLARE tblTempSubSelect_Cursor CURSOR FOR               
        SELECT ID FROM tblSubSelect WHERE SegmentID = @OldSegmentID              
                      
        OPEN tblTempSubSelect_Cursor              
        FETCH NEXT FROM tblTempSubSelect_Cursor               
    INTO @SubSelID              
        WHILE @@FETCH_STATUS = 0               
            BEGIN              
                          
            INSERT INTO tblSubSelect(SegmentID,cIncludeExclude,cCompanyIndividual,dCreatedDate,cCreatedBy)              
   (SELECT @SegmentIdentity,cIncludeExclude,cCompanyIndividual,GETDATE(),@InitiatedBy FROM               
   tblSubSelect WHERE ID = @SubSelID)              
                
   DECLARE @SubSetIdentity as Int               
   -- New ID created when copying Segment               
   SET @SubSetIdentity  =  @@IDENTITY              
                 
   -- Copy Lists from the current segment to the copy               
   INSERT INTO tblSubSelectList(SubSelectID,MasterLOLID,iIsHouseList,dCreatedDate,cCreatedBy)              
   (SELECT @SubSetIdentity,tblSubSelectList.MasterLOLID,iIsHouseList,GETDATE(),@InitiatedBy FROM               
   tblSubSelectList INNER JOIN tblOrderCASApproval               
   ON tblSubSelectList.MasterLOLID = tblOrderCASApproval.MasterLOLID AND tblOrderCASApproval.OrderID = @OrderIdentity              
   WHERE SubSelectID = @SubSelID)              
                 
                 
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
    GETDATE(),@InitiatedBy               
    FROM tblSubSelectSelection WHERE SubSelectID = @SubSelID Order BY iGroupNumber, tblSubSelectSelection.ID              
                  
   END              
   FETCH NEXT FROM tblTempSubSelect_Cursor               
    INTO @SubSelID              
                                                              
            END              
        CLOSE tblTempSubSelect_Cursor              
        DEALLOCATE tblTempSubSelect_Cursor              
   

  --Increment Loop Counter              
  SET @RowCountSegments = @RowCountSegments + 1               
 END              
END 