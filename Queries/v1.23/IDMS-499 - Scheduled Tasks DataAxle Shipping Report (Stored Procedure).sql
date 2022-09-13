-- ====================================================================      
-- Author: Saarthak Pande
-- Description: Made changes for Scheduled Batch Task - DataAxle Shipping Report  
-- User Story - IDMS-499 - NUA | App Refactor : Batch Task - All Active Auto Scheduled Tasks (tblSchedule) - Shipping Report
-- =====================================================================

USE [DW_Admin]
GO

/****** Object:  StoredProcedure [dbo].[usp_GetOrderFaxGateShippingData_csv_AWS]    Script Date: 3/16/2021 9:45:10 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO




CREATE PROCEDURE [dbo].[usp_GetOrderFaxGateShippingData_csv_AWS]
	@tdStartDate SmallDateTime,
	@tnDivisionID int,
	@Distribution varchar(255)
AS

/*
-- CHANGES:
-- Modified by SF on 11.29.2018 to use @@servername 
*/


SET NOCOUNT ON
DECLARE @tnID int = 0

IF OBJECT_ID('tempdb..#tblDecoyCount') IS NOT NULL
	DROP TABLE #tblDecoyCount

IF OBJECT_ID('tempdb..#tblOrderStatus') IS NOT NULL
	DROP TABLE #tblOrderStatus

IF OBJECT_ID('tempdb..##tmpFTP_ShippingData') IS NOT NULL
	DROP TABLE ##tmpFTP_ShippingData


DECLARE @DivisionName varchar(255)
SELECT @DivisionName = cDivisionName
FROM tblDivision
WHERE ID = @tnDivisionID

-- Cursor Fields
DECLARE @lnBrokerID int, @lcBrokerEmail varchar(1000), @lcBrokerFirstName char(50), @lcBrokerLastName char(50), 
		  @lcBrokerCompany char(50)

DECLARE @lcStartYY char(4), @lcStartMM char(2), @lcStartDD char(2), @lcStartDate smalldatetime, @lcEndDate smalldatetime, @lcCommandString varchar(4000), @lcFileName varchar(100),
		  @lcSubject varchar(1000)

SET @lcStartYY = DATEPART(yyyy, @tdStartDate)
SET @lcStartMM = DATEPART(mm, @tdStartDate)
SET @lcStartDD = DATEPART(dd, @tdStartDate)

SET @lcStartDate = CAST(@lcStartYY + '-' + @lcStartMM + '-' + @lcStartDD + ' 12:00:00 AM' AS smalldatetime)

SET @lcEndDate   = CAST(@lcStartYY + '-' + @lcStartMM + '-' + @lcStartDD + ' 11:59:01 PM' AS smalldatetime)

 SELECT COUNT(*) nCount, OrderID
  INTO #tblDecoyCount
  FROM tblOrderDecoy
 GROUP BY OrderID

SELECT iStatus, OrderID, dCreatedDate AS dShippedDate
  INTO #tblOrderStatus
  FROM tblOrderStatus
 WHERE tblOrderStatus.iStatus IN (130) AND tblOrderStatus.iIsCurrent = 1



-- Generate Data for Each Broker
-- Broker Must have Email Address
-- Step 2: Loop through each build and process all data
DECLARE cBrokers CURSOR READ_ONLY
    FOR SELECT DISTINCT tblBroker.ID, CASE WHEN ISNULL(tblBroker.cEmail, '') = '' THEN 'idmssystemnotification@infogroup.com' ELSE tblBroker.cEmail END, tblBroker.cFirstName, tblBroker.cLastName, tblBroker.cCompany
          FROM tblDivisionBroker tblBroker
		   INNER JOIN tblOrder
			   ON tblOrder.DivisionBrokerID = tblBroker.ID
		   INNER JOIN #tblOrderStatus
			   ON #tblOrderStatus.OrderID = tblOrder.ID
		   INNER JOIN tblBuild builds
		       ON builds.ID = tblOrder.BuildID
           INNER JOIN tblDatabase tbldatabase
		       ON tbldatabase.ID = builds.DatabaseID AND tbldatabase.DivisionID = @tnDivisionID
--			INNER JOIN tblDivision AS D
--			ON D.id = tblBroker.DivisionID
	 	   WHERE #tblOrderStatus.dShippedDate BETWEEN @lcStartDate AND @lcEndDate
			  --AND tblBroker.DivisionID = @tnDivisionID
			  AND ((tblBroker.cCompany NOT IN ('INFOUSA') AND tblBroker.DivisionId = 184) OR tblBroker.DivisionId <> 184)
			  --AND ISNULL(iNoShipReport, 0) = 0
OPEN cBrokers

FETCH NEXT FROM cBrokers INTO @lnBrokerID, @lcBrokerEmail, @lcBrokerFirstName, @lcBrokerLastName, @lcBrokerCompany

WHILE (@@fetch_status = 0)
BEGIN
	-- For Edith Roman Lists, do not export if broker has no email
	IF  @lcBrokerEmail IS NULL AND @tnDivisionID = 183
		CONTINUE

	-- Drop the temp table
	IF OBJECT_ID('tempdb..##tmpFTP_ShippingData') IS NOT NULL
		DROP TABLE ##tmpFTP_ShippingData
	
	CREATE TABLE ##tmpFTP_ShippingData (
	ID INT IDENTITY (1,1) NOT NULL,
	cAll varchar(8000))

		INSERT INTO ##tmpFTP_ShippingData
		SELECT           '"' + 'List Name' +  '",' +
						 '"' + 'Order Description' + '",' +
						 '"' + 'Create Date' + '",' +
						 '"' + 'Ship Date' + '",' +
						 '"' + 'Mailer Name' + '",' +
						 '"' + 'Broker PO #' + '",' +
						 '"' + 'PO Number' + '",' +
						 CASE WHEN @tnDivisionID = 183 THEN '' ELSE '"' + 'Quantity' + '",' END +
						 CASE WHEN @tnDivisionID = 183 THEN '' ELSE '"' + 'Decoy Quantity' + '",' END + 
						 '"' + 'Decoy Key' + '",' +
						 '"' + 'Total Quantity' + '"' AS cAll
	
							 
		INSERT INTO ##tmpFTP_ShippingData
		SELECT '"' + RTRIM(tblDatabase.cDatabaseName) + '",' +
				 '"' + RTRIM(tblOrder.cDescription) + '",' +
				 '"' + CONVERT(char(10), tblOrder.dCreatedDate, 101) + '",' +
				 '"' + CONVERT(char(10), tblOrderStatus.dShippedDate, 101) + '",' +
				 '"' + RTRIM(tblMailer.cCompany) + '",' +
				 '"' + RTRIM(tblOrder.cBrokerPONo) + '",' +
				 '"' + RTRIM(tblOrder.cLVAOrderNo) + '",' +
				 CASE WHEN @tnDivisionID = 183 THEN '' ELSE '"' + CAST(iProvidedCount - ISNULL(tblDecoyCount.nCount, 0) AS varchar) + '",' END + 
				 CASE WHEN @tnDivisionID = 183 THEN '' ELSE '"' + CAST(ISNULL(tblDecoyCount.nCount, 0) AS varchar) + '",' END + 
				 '"' + RTRIM(tblOrder.cDecoyKey) + '",' +
				 '"' + CAST(iProvidedCount AS varchar) + '"' AS cAll
		  FROM tblOrder
		 INNER JOIN tblDivisionMailer tblMailer
			 ON tblOrder.DivisionMailerID = tblMailer.ID
		 INNER JOIN tblDivisionBroker tblBroker
			 ON tblOrder.DivisionBrokerID = tblBroker.ID
		 INNER JOIN tblBuild
			 ON tblOrder.BuildID = tblBuild.ID
		 INNER JOIN tblDatabase
			 ON tblBuild.DatabaseID = tblDatabase.ID
		 LEFT JOIN #tblDecoyCount tblDecoyCount
			 ON tblOrder.ID = tblDecoyCount.OrderID
		 INNER JOIN #tblOrderStatus tblOrderStatus
			 ON tblOrder.ID = tblOrderStatus.OrderID 
		 WHERE tblOrderStatus.dShippedDate BETWEEN @lcStartDate AND @lcEndDate
			AND tblOrder.DivisionBrokerID = @lnBrokerID
	--AND ISNULL(iNoShipReport, 0) = 0
		
		SELECT @tnID = @@ROWCOUNT

	-- Create FTP command file
	IF @tnID > 0
		BEGIN
			SET @lcFileName = 'Shipping_Report_' +  CAST(@tnDivisionID AS varchar) + '_' + CAST(@lnBrokerID AS varchar) + '_' + LTRIM(RTRIM(@lcStartMM)) + LTRIM(RTRIM(@lcStartDD)) + LTRIM(RTRIM(@lcStartYY)) + '.csv'
			SET @lcSubject  = @DivisionName + ' – Shipping Broker Report (' + CONVERT(varchar, @tdStartDate, 101) + ') - ' + @lcBrokerCompany
			IF ISNULL(@lcBrokerEmail, '') = ''
			   SET @lcBrokerEmail = @Distribution
			
			SELECT @lcBrokerEmail as cBrokerEmail, @lcFileName as cFileName , @lcSubject as cSubject 
			SELECT cAll FROM ##tmpFTP_ShippingData ORDER BY ID
		END
	
	-- Get Next Broker
	FETCH NEXT FROM cBrokers INTO @lnBrokerID, @lcBrokerEmail, @lcBrokerFirstName, @lcBrokerLastName, @lcBrokerCompany
END

-- Clean up
CLOSE cBrokers
DEALLOCATE cBrokers

IF OBJECT_ID('tempdb..##tmpFTP_ShippingData') IS NOT NULL
	DROP TABLE ##tmpFTP_ShippingData

/*
DECLARE @tdStartDate SmallDateTime

SET @tdStartDate = GETDATE() - 1

EXEC usp_GetOrderFaxGateShippingData_csv_AWS @tdStartDate,188,'jayesh.patel@infogroup.com'

*/
GO


