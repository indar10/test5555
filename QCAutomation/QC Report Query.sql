use common
DROP TABLE IF EXISTS common.dbo.tbl_QCBuildReport
CREATE TABLE common.dbo.tbl_QCBuildReport
(
ID int identity(1,1),
DatabaseID int not null,
BuildID int not null,
ListID int not null,
[Field Name] varchar(100),
[Field Value] varchar(100),
Description varchar(255),
PrevCount Bigint,
CurrCount Bigint,
ChangeDifference float,
ChangePercentage float,
QC_Date Date
)
CREATE INDEX IX_BuildID_tbl_QCBuildReport ON common.dbo.tbl_QCBuildReport (BuildID, ListID, [Field Name], [Field Value])
CREATE UNIQUE CLUSTERED INDEX UIX_tbl_QCBuildReport ON common.dbo.tbl_QCBuildReport (BuildID, ListID, [Field Name], [Field Value])

Go

CREATE OR ALTER PROCEDURE usp_QCBuildReport @DatabaseID Int, @ChangePercentage Float = 5
AS
BEGIN
	DROP TABLE IF EXISTS #TempBuildCount
	DROP TABLE IF EXISTS #TempBuildCount1

	-----Process for each DB -----------------------
	DECLARE @BuildID AS Int, @ListID AS Int, @WHEREClause AS varchar(max), @PreviousBuildID int

	SELECT @ListID =MLOL.ID,@BuildID =B.ID
	FROM DW_Admin.dbo.tblDatabase D
	INNER JOIN 
		(SELECT ID,cBuild,cDescription,DatabaseID,iIsReadyToUse,ROW_NUMBER() OVER(PARTITION BY DatabaseID ORDER BY ID DESC) AS RN 
		   FROM DW_Admin.dbo.tblBuild WHERE iIsReadyToUse = 1) B
	ON D.ID = B.DatabaseID
	INNER JOIN DW_Admin.dbo.tblMasterLoL MLOL 
	ON MLOL.DatabaseID = D.ID
	WHERE iIsReadyToUse = 1 AND B.RN = 1
	AND D.id = @DatabaseID
	AND LK_PermissionType IN('R') 


	-- Get Previous Build ID
	SELECT @PreviousBuildID = MAX(BuildID) FROM Common.dbo.tbl_QCBuildReport WHERE  BuildID < @BuildID AND DatabaseID = @DatabaseID

	--To Extract the List Profile Report and load into Temp table
	SELECT
	@DatabaseID AS DatabaseID,
	@BuildID AS BuildID,
	@ListID AS ListID,
	CASE
		WHEN
			BRF.BuildTableLayoutID IS NOT NULL 
		THEN
			BRF.cRAWFieldName 
		ELSE
			BTL.cFieldDescription 
	END
	AS [Field Name], bar.cValue AS [Field Value], bdd.cDescription AS Description, bar.iCount AS CurrCount , CAST(GETDATE() AS DATE) AS QC_Date
	INTO #TempBuildCount 
	FROM DW_Admin.dbo.tblBuildAuditReport bar 
	INNER JOIN DW_Admin.dbo.tblBuildTableLayout btl 
	   ON btl.ID = bar.BuildTableLayoutID 
	INNER JOIN DW_Admin.dbo.tblBuildLol Lol 
	   ON Lol.MasterLoLID = bar.MasterLOLID 
	  AND Lol.BuildID = bar.BuildID 
	 LEFT JOIN DW_Admin.dbo.tblBuildRAWFieldName BRF 
	   ON BRF.BuildTableLayoutID = BTL.ID 
	  AND BRF.BuildLOLID = lol.ID 
	 LEFT OUTER JOIN DW_Admin.dbo.tblBuildDD bdd 
	   ON btl.ID = bdd.BuildTableLayoutID 
	  AND bar.cValue = bdd.cValue 
	  AND bdd.BuildLoLID = 
		CASE
			WHEN
				btl.iIsListSpecific = 1 
			THEN
				Lol.ID 
			ELSE
				0 
		END
	WHERE
		bar.MasterLoLID = @ListID 
		AND Lol.BuildID = @BuildID 
	ORDER BY
		4, 5
	

	--Load the Change % data into Temp TAble
	SELECT
		t1.*,
		t2.CurrCount AS PrevCount,
		Round(100.0*(t1.CurrCount - t2.CurrCount) / t1.CurrCount ,2) AS ChangePercentage,
		t1.CurrCount - t2.CurrCount as ChangeDifference 
	INTO #TempBuildCount1 
	FROM
		#TempBuildCount t1 
		LEFT JOIN
			Common.dbo.tbl_QCBuildReport t2 
			ON t1.DatabaseID = t2.DatabaseID 
			AND t1.ListID = t2.ListID 
			AND t1.[Field Name] = t2.[Field Name] 
			AND t1.[Field Value] = t2.[Field VALUE] 
	WHERE
		@PreviousBuildID IS NULL 
		OR
		t2.BuildID = @PreviousBuildID 
	
	
	---Check if Change % is more than 5 then error msg else insert into tbl_QCBuildReport
	IF NOT EXISTS(SELECT * FROM #TempBuildCount1 WHERE ChangePercentage>@ChangePercentage OR ChangePercentage<-@ChangePercentage)
		BEGIN
			-- Delete Before Insert
			DELETE FROM common.dbo.tbl_QCBuildReport WHERE BuildID = @BuildID

			INSERT INTO common.dbo.tbl_QCBuildReport (DatabaseID ,BuildID ,ListID ,[Field Name] ,[Field Value] ,Description, PrevCount ,CurrCount ,ChangePercentage,ChangeDifference ,QC_Date)
			SELECT DatabaseID ,BuildID, ListID ,[Field Name] ,[Field Value] ,Description ,PrevCount,CurrCount ,ChangePercentage,ChangeDifference ,QC_Date
			FROM #TempBuildCount1
		END
	ELSE
		BEGIN
			DECLARE @CreateErrorTable as nvarchar(max), @DropErrorTable as nvarchar(255), @TableName varchar(100), @ErrorMsg varchar(1000)
			
			SET @TableName = 'Tempdata.dbo.tbl_QCBuildReport_' + cast(@DatabaseID as varchar(10)) + '_Error'
			SET @DropErrorTable = 'Drop Table IF Exists ' + @TableName
			EXECUTE sp_executesql @DropErrorTable
			
			SET @CreateErrorTable = 'SELECT DatabaseID ,BuildID ,ListID ,[Field Name] ,[Field Value] ,Description ,PrevCount, CurrCount,ChangeDifference ,ChangePercentage ,QC_Date INTO ' + @TableName + ' FROM #TempBuildCount1'
			EXECUTE sp_executesql  @CreateErrorTable

			SET @ErrorMsg = 'Change Threshold exceeded. Please review data in error table ' + @TableName
			RAISERROR (@ErrorMsg,16,1);
		END

	DROP TABLE IF EXISTS #TempBuildCount
	DROP TABLE IF EXISTS #TempBuildCount1
END
	