USE [DW_Admin]
GO
/****** Object:  StoredProcedure [dbo].[usp_CopyOrderDiv]    Script Date: 5/27/2020 12:51:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
    
    
-- =========================================================================================================    
-- Author:  Amruta Bhogale    
-- Create date: Oct-16-2009    
-- Description: Used make a copy of Order    
-- =========================================================================================================    
/*    
* * *********************UPDATION HISTORY******************************    
 * Updated By  Updated Date   Comments    
 * ------------------------------------------------------------    
 * Tanmay Patel  23-Jan-2014      TFS # 42824: Copy tblOrderPrevOrders content on copy Order division level    
 * Tanmay Patel  25-Feb-2014   TFS # 44051: Copy maxper for divisional order.    
 * Jayesh Patel     31-Mar-2014      Jayesh added validation of copy divisional mailer/broker to make sure they belong to same    
          division, otherwise assign zero    
 * Surendra Sharma 08-Apr-2014      TFS # 44657: Copy cChannelType on copy order    
 * Jayesh Patel     30-Apr-2014      Jayesh added temporary fix for IG Consumer Database. New database design has no child tables so when order    
          gets copied from older build with childtable, this change will replace tblSelection.cTableName with 'tblMain'     
 * Jayesh Patel     22-May-2014      Jayesh added temporary fix for mGen Database. New database design has no child tables so when order    
          gets copied from older build with childtable, this change will replace tblSelection.cTableName with 'tblMain'     
 * Jayesh Patel     29-May-2014      Jayesh added temporary fix for Sapphire Database. New database design has no child tables so when order    
          gets copied from older build with childtable, this change will replace tblSelection.cTableName with 'tblMain'     
 * Surendra Sharma   2-Sept-2014     TFS # 51553 - Add Output File Label.        
 * Tanmay patel      11-Oct-2014  TFS# 52719 - Copy tblOrderXtabReport.ctype    
 * Tanmay Patel      16-Dec-2014     TFS#54172 - Not to copy over ftp info when copying order     
 * Jayesh Patel      27-Feb-2015     SharePoint#264 Blank out ship to info for copied ftp orders     
* Tanmay Patel    13-May-2015    TFS#55533 - Copy SAN number     
* Ketaki Bhale      9-Oct-2015     TFS#57149 - Export Only Data File - Order Level   
* Tanmay Patel	 18-Mar-2016		TFS#57814 - Do not copy export layout if source layout is deleted  
* Pratik Saraf	 19-May-2017		TFS#59780 - Added iIsUnCompressed column
* Pratik Saraf	 31-May-2017		TFS#59780 - Added LK_PGPKeyFile column 
* Jayesh Patel	 21-Sep-2017		Removed Custom Logic for mGen - 847 
* Jayesh Patel	 16-March-2018		VSTS#416 - Added iIsRandomRadiusNth column  
* Saarthak Pande 19-Oct-2018        VSTS#578 - Uppercase & Lowercase Output - Order Level
* Jayesh Patel   02-March-2019	    VSTS#756 - Blank Out OESS Broker and Mailer while copying
* Jayesh Patel   04-May-2019	    Added New Field Mapping for GUI Migration of 1295/992
* Abhishek Kalekar 20-May-2020		VSTS#2594 -  Adding newly added column for previous order tblSegmentPrevOrders.cMatchFieldName
 =============================================================================================================      
*/    
    
ALTER PROCEDURE [dbo].[usp_CopyOrderDiv]    
 -- Add the parameters for the stored procedure here    
 @OrderID as INT,    
 @MailerID as INT,    
 @OfferID as INT,    
 @BuildID as INT,    
 @DivisionMailerID as INT,    
 @DivisionBrokerID as INT,    
 @OfferName  VARCHAR(50),    
 @UserID as INT,    
 @Description  VARCHAR(50),    
 @InitiatedBy as VARCHAR(25)    
AS    
BEGIN    
 -- SET NOCOUNT ON added to prevent extra result sets from    
 -- interfering with SELECT statements.    
 SET NOCOUNT ON;    
 DECLARE @OrderIdentity as Int     
 DECLARE @OrderSplitType as Int     
 DECLARE @BuildYYYYMM char(6)    
 DECLARE @DivisionID int, @NewDivisionalMailerID int, @NewDivisionalBrokerID int, @DatabaseID int    
 Declare @SourceExportLayout varchar(50)      
 -- Check if the mailer/broker belongs to the same division.    
 -- This is necessary when database is moved to a different division and mailer/broker have to merge    
 SELECT @DivisionID = a.DivisionID, @DatabaseID = a.ID     
   FROM tblDatabase a with (nolock)     
     INNER JOIN tblBuild b with (nolock)     
     ON a.ID = b.DatabaseID    
     WHERE b.ID = @BuildID    
     
 SELECT @NewDivisionalMailerID = ID     
   FROM tblDivisionMailer with (nolock)     
     WHERE ID = @DivisionMailerID
	   AND iIsActive = 1
    AND DivisionID = @DivisionID    
    
 SELECT @NewDivisionalBrokerID = ID     
   FROM tblDivisionBroker with (nolock)     
     WHERE ID = @DivisionBrokerID    
    AND DivisionID = @DivisionID    
      
 SELECT @SourceExportLayout = cExportLayout   
FROM tblOrder with(nolock) where ID = @OrderID  
     
 IF @NewDivisionalMailerID IS NULL    
  SET @NewDivisionalMailerID = 0    
    
 IF @NewDivisionalBrokerID IS NULL    
  SET @NewDivisionalBrokerID = 0    
 -- End of the check added by Jayesh on 03/31/2014.     
    
    -- Jayesh added this on 02/27/2015    
 -- Remove ShipToEmail details if FTP Order    
 DECLARE @OrderFTPID int    
    
 SELECT @OrderFTPID = ID    
   FROM tblOrderFTP with (nolock)    
  WHERE OrderID=@OrderID      
 SELECT @BuildYYYYMM = cBuild from tblBuild where ID= @BuildID    
    
 SET @OrderSplitType = 0    
     
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
 cOfferName,DivisionMailerID,DivisionBrokerID,    
 iMinQuantityOrderLevelMaxPer , iMaxQuantityOrderLevelMaxPer , cMaxPerFieldOrderLevel, cChannelType, cFileLabel,cSANNumber,iIsExportDataFileOnly, iIsUnCompressed, LK_PGPKeyFile
 ,LK_AccountCode,LK_Media,iExportLayoutID)    
 (SELECT @BuildID,@MailerID,@OfferID,cOrderType,@UserID,0,@Description,0,0,iIsRandomExecution,iHouseFilePriority,iIsNetUse,NULL,    
     cSortFields, '','','',cBrokerPONo,cNotes,iDecoyQty,iDecoyKeyMethod,CASE WHEN (iDecoyKeyMethod = 0) THEN cDecoyKey ELSE '' END ,cDecoyKey1,cDecoysByKeycode,
	 IIF(CHARINDEX('##',cSpecialProcess) > 1, LEFT(cSpecialProcess, CHARINDEX('##',cSpecialProcess) - 1),  cSpecialProcess), 
     cShiptoType,    
  CASE WHEN @OrderFTPID is null THEN cShipTOEmail ELSE '' END,    
  CASE WHEN @OrderFTPID is null THEN cShipCCEmail ELSE '' END,    
  cShipSUBJECT,NULL,LK_ExportFileFormatID,GETDATE(),@InitiatedBy,    
     iSplitType,iSplitIntoNParts,@SourceExportLayout,iIsAddHeader,'',0,cOutputCase,
	 --iIsOutputToUpper, replaced by cOutputCase
	 @OfferName, 
	 CASE WHEN @NewDivisionalMailerID IN (16048,16047,15130,15049,14806,8958,8957,8956,8951,8950,5649,4290,4154,4098,3929,3537,1210,975,735,19460) THEN 0 ELSE @NewDivisionalMailerID END, 
     CASE WHEN @NewDivisionalBrokerID IN (1683) THEN 0 ELSE @NewDivisionalBrokerID END,
	 iMinQuantityOrderLevelMaxPer , iMaxQuantityOrderLevelMaxPer , cMaxPerFieldOrderLevel, 
     cChannelType, cFileLabel,cSANNumber,iIsExportDataFileOnly, iIsUnCompressed, LK_PGPKeyFile
	 ,LK_AccountCode,LK_Media,iExportLayoutID    
     FROM  tblOrder WHERE ID = @OrderID)    
    
 -- New ID created when copying Order     
 SET @OrderIdentity  =  @@IDENTITY    
     
 --INSERT INTO FTP TABLE -Neeraj Jain Jan-19-12    
 /*    
 INSERT INTO tblOrderFTP(OrderID,cFTPServer,cUserID,cPassword,cCreatedBy,dCreatedDate)    
 SELECT @OrderIdentity,cFTPServer,cUserID,cPassword,cCreatedBy,getdate() from tblOrderFTP    
  Where OrderID=@OrderID    
 */     
  -- Copy Export Layout Update: Copy it no matter what dont check for fields etc..Order will fail if field does not exists    
 -- Update on Mar 19 2012    

-- Update 18-Mar-2016 - Do not copy export order if source order name is deleted.
 IF (ISNULL(@SourceExportLayout,'')<>'')  
BEGIN    
 INSERT INTO tblOrderExportLayout(OrderID,cFieldName,cOutputFieldName, iExportOrder,iWidth, cCalculation, cTableNamePrefix ,dCreatedDate,cCreatedBy)    
 (SELECT @OrderIdentity,cFieldName,cOutputFieldName, iExportOrder, iWidth, cCalculation, cTableNamePrefix ,GETDATE(),@InitiatedBy 
 FROM tblOrderExportLayout WHERE OrderID = @OrderID)    
END  

 SELECT @OrderSplitType=iSplitType FROM tblOrder WHERE ID = @OrderIdentity     
     
 --Get Decoys By Mailer     
 DECLARE @OldMailer as Int     
 DECLARE @OldBuild as Int     
 DECLARE @DivisionForBuild as Int     
     
 SELECT @OldMailer = MailerID, @OldBuild = BuildID from tblOrder where ID = @OrderID    
     
 --SELECT @DivisionForBuild = DivisionID FROM tblBuild WITH(NOLOCK) INNER JOIN tblDatabase WITH(NOLOCK) ON tblDatabase.ID =  tblBuild.DatabaseID    
 --WHERE tblBuild.ID = @BuildID    
     
 --Modified by AB.. according to Jayesh we should never copy decoys from previous order when using "Copy Order" function, regardless of mailer    
 --Always bring in global decoys.    
 --Get Decoys By Mailer     
 /*IF @MailerID = @OldMailer    
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
  (cDecoyType = 'G' and D.DatabaseID in (SELECT ID FROM tblDatabase WITH(NOLOCK) WHERE tblDatabase.DivisionID = @DivisionForBuild))    
  OR (MailerID = @MailerID and cDecoyGroup = 'A')    
 --END*/    
 INSERT INTO tblOrderDecoy (OrderID,cFirstName,cLastName,cName,cAddress1,cAddress2,cCity,cState,cZip,cCompany,cTitle,cEmail,cPhone,    
  cFax,dCreatedDate,cCreatedBy,cDecoyType,cZip4,cKeyCode1, cDecoyGroup)    
  SELECT @OrderIdentity,D.cFirstName,D.cLastName, D.cName, D.cAddress1, D.cAddress2,     
  D.cCity, D.cState, D.cZip,D.cCompany, D.cTitle, D.cEmail, D.cPhone, D.cFax,GETDATE(),@InitiatedBy, D.cDecoyType,    
  D.cZip4, D.cKeyCode1,D.cDecoyGroup FROM   tblDecoy D WHERE     
  (cDecoyType = 'G' and D.DatabaseID in (Select DatabaseID from tblMailer M where M.ID = @MailerID))    
  OR (MailerID = @MailerID and cDecoyGroup = 'A')    
    
     
    
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
     
 INSERT INTO tblOrderMultiColumnReport(OrderID,cFields,cFieldsDescription,iMultiColBySegment,dCreatedDate,cCreatedBy, cSegmentNumbers)    
 SELECT @OrderIdentity, cFields,cFieldsDescription,iMultiColBySegment, GETDATE(),@InitiatedBy, cSegmentNumbers    
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
   INSERT INTO tblSegmentSelection(SegmentID,cQuestionFieldName,cQuestionDescription,cJoinOperator,iGroupNumber,iGroupOrder,cGrouping,cValues,cValueMode,    
   cDescriptions,cValueOperator,cFileName,cSystemFileName,dCreatedDate,cCreatedBy,cTableName)    
   SELECT @SegmentIdentity,cQuestionFieldName,cQuestionDescription,cJoinOperator,iGroupNumber,iGroupOrder,cGrouping,cValues,cValueMode,    
   cDescriptions,cValueOperator,CASE  WHEN (cValueMode = 'F')THEN cFileName ELSE ''END ,    
   CASE  WHEN (cValueMode = 'F') THEN cSystemFileName ELSE ''END,GETDATE(),@InitiatedBy,cTableName    
   FROM tblSegmentSelection WHERE SegmentID = @OldSegmentID Order BY iGroupNumber, tblSegmentSelection.ID    
  END    
  ELSE    
  BEGIN    
   -- Copy Segment selection    
   INSERT INTO tblSegmentSelection(SegmentID,cQuestionFieldName,cQuestionDescription,cJoinOperator,iGroupNumber,iGroupOrder,cGrouping,cValues,cValueMode,    
     cDescriptions,cValueOperator,cFileName,cSystemFileName,dCreatedDate,cCreatedBy,cTableName)    
   SELECT  @SegmentIdentity,

	 -- Changed By Jayesh on 05/04/2019 for GUI Migration
	 CASE WHEN @DatabaseID = 992 AND @BuildID >= 17274 AND @OldBuild < 17274
	 THEN dbo.udf_GetGUIFieldName_992(cQuestionFieldName, 'F', '')
	 ELSE cQuestionFieldName END,

	 CASE WHEN @DatabaseID = 992 AND @BuildID >= 17274 AND @OldBuild < 17274
	 THEN dbo.udf_GetGUIFieldName_992(cQuestionFieldName, 'D', cQuestionDescription)
	 ELSE cQuestionDescription END,
	 -- End of change by Jayesh on 05/04/2019 for GUI Migration

	 cJoinOperator,iGroupNumber,iGroupOrder,cGrouping,cValues,cValueMode,    
     cDescriptions,cValueOperator,CASE  WHEN (cValueMode = 'F') THEN cFileName ELSE '' END ,    
     CASE WHEN (cValueMode = 'F') THEN cSystemFileName ELSE ''END,GETDATE(),@InitiatedBy,    
     CASE WHEN @DatabaseID = 71 AND @BuildID >= 3942 AND cTableName IN ('tblExternal33_191_201206', 'tblExternal34_191_201206', 'tblExternal35_191_201206') THEN 'tblMain' + '_' +  CAST(@BuildID AS VARCHAR) +  '_' + @BuildYYYYMM    
       WHEN (cTableName LIKE '%External%') THEN cTableName     
       -- Added by Jayesh on 04/30/2014    
       WHEN @DatabaseID IN (860,920) AND @BuildID >= 4321 AND cTableName LIKE '%tblChild%' THEN 'tblMain' + '_' +  CAST(@BuildID AS VARCHAR) +  '_' + @BuildYYYYMM    
       -- Added by Jayesh on 05/22/2014    
       --WHEN @DatabaseID IN (847) AND @BuildID >= 4476 AND LEFT(cTableName, 11) = 'tblChild23_'  THEN 'tblChild31' + '_' +  CAST(@BuildID AS VARCHAR) +  '_' + @BuildYYYYMM    
       --WHEN @DatabaseID IN (847) AND @BuildID >= 4476 AND LEFT(cTableName, 11) = 'tblChild24_'  THEN 'tblChild32' + '_' +  CAST(@BuildID AS VARCHAR) +  '_' + @BuildYYYYMM    
       --WHEN @DatabaseID IN (847) AND @BuildID >= 4476 AND LEFT(cTableName, 11) = 'tblChild25_'  THEN 'tblChild33' + '_' +  CAST(@BuildID AS VARCHAR) +  '_' + @BuildYYYYMM    
       --WHEN @DatabaseID IN (847) AND @BuildID >= 4476 AND cTableName LIKE '%tblChild%' AND LEFT(cTableName, 10) NOT IN ('tblChild1_','tblChild2_','tblChild3_', 'tblChild4_', 'tblChild5_')  THEN 'tblMain' + '_' +  CAST(@BuildID AS VARCHAR) +  '_' + @BuildYYYYMM    
  
       -- Added by Jayesh on 05/29/2014    
       WHEN @DatabaseID = 71 AND @BuildID >= 3942 AND cTableName LIKE '%tblChild%' THEN 'tblMain' + '_' +  CAST(@BuildID AS VARCHAR) +  '_' + @BuildYYYYMM    
       ELSE (SUBSTRING(cTableName,1, CHARINDEX('_',cTableName,1)-1)+ '_' +  CAST(@BuildID AS VARCHAR) +  '_' + @BuildYYYYMM) END AS cTableName    
     FROM tblSegmentSelection     
    WHERE SegmentID = @OldSegmentID Order BY iGroupNumber, tblSegmentSelection.ID    
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
 --SELECT @OrderIdentity
END    