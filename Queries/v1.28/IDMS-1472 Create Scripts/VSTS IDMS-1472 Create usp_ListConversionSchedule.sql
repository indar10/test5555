USE [DW_Admin]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[usp_ListConversionSchedule]
AS

-- DECLARATIONS
DECLARE 
	@Counter	INT, 
	@MaxId		INT,
	@ListId		INT,
	@BuildId	INT,
	@BuildLolId INT,
	@Id INT,
	@cScheduledBy	VARCHAR(100),
	@cSystemFileNameReadyToLoad	VARCHAR(255)



DECLARE db_cursor CURSOR FOR
-- GET Id based on LK_ListConversionFrequency 
SELECT 
	Id
FROM 
	tblListConversionSchedule
WHERE
	iIsActive = 1 AND
	((LK_ListConversionFrequency = 'W' AND iInterval = DATEPART(WEEKDAY, GETDATE())) OR
	(LK_ListConversionFrequency = 'M' AND iInterval = DATEPART(MONTH, GETDATE())) OR
	(LK_ListConversionFrequency = 'Y' AND iInterval = DATEPART(DAYOFYEAR, GETDATE())) OR
	(LK_ListConversionFrequency = 'D'))

OPEN db_cursor  
FETCH NEXT FROM db_cursor INTO @Id

-- LOOP ON AVAILABLE Id
WHILE @@FETCH_STATUS = 0
BEGIN
	-- GET THE LISTID AND OTHER FIELDS 
	SELECT 
		@ListId = ListId,
		@BuildId = BuildId,
		@cSystemFileNameReadyToLoad = cSystemFileNameReadyToLoad,
		@cScheduledBy = cScheduledBy
	FROM 
		tblListConversionSchedule
	WHERE
		Id = @Id

	-- GET THE BUILDID AND BUILDLOLID BASED ON @ListId
	IF @BuildId <> 0
		BEGIN
			SELECT TOP 1
				@BuildLolId=BL.ID
			FROM
				tblBuild B
					INNER JOIN tblBuildLol BL
						ON B.ID = BL.BuildID
			WHERE
				B.iIsOnDisk = 0 AND
				BL.BuildId = @BuildId AND
				BL.MasterLoLID = @ListId
			ORDER BY BL.ID DESC
		END
	ELSE
		BEGIN
			SELECT 
				TOP 1 
					@BuildId = B.ID,
					@BuildLolId=BL.ID 
			FROM 
				tblBuild B
					INNER JOIN tblBuildLol BL
						ON B.ID = BL.BuildID
			WHERE
				B.iIsOnDisk = 0 AND
				BL.MasterLoLID = @ListId
			ORDER BY BL.ID DESC
		END
	-- UPDATE THE cSystemFileNameReadyToLoad PATH FROM THE tblListConversionSchedule TABLE
	UPDATE TOP (1) 
		tblBuildLoL
	SET 
		 cSystemFileNameReadyToLoad = @cSystemFileNameReadyToLoad
	WHERE 
		 MasterLoLID = @ListId AND 
		 BuildID = @BuildId

	-- UPDATE tblListLoadStatus TO SET iIsCurrent = 0 (IF THE LIST CONVERSION HAPPENED PREVIOUSLY)
	UPDATE 
		tblListLoadStatus
	SET 
		iIsCurrent = 0
		--dCreatedDate = GETDATE() -- USER MIGHT HAVE SCHEDULED THE CONVERSION BUT THIS WILL GET OVERWRITTEN HERE
	FROM 
		tblListLoadStatus
	WHERE 
		BuildLoLID = @BuildLolId AND 
		iIsCurrent = 1


	DELETE TOP (2)
	FROM 
		tblLoadProcessStatus
	WHERE 
		 BuildLoLID = @BuildLolId AND 
		 iStatus in (5,1000)

	-- INSERT/ UPDATE AND STATE THE LIST CONVERSION STATUS TO 70
	DECLARE @Count int
	SELECT 
		@Count = count(1) 
	FROM
		tblListLoadStatus
	WHERE
		BuildLolId = @BuildLolId AND 
		LK_LoadStatus = '70'

	IF @Count > 0
		BEGIN
			UPDATE TOP (1) tblListLoadStatus
			SET 
				iIsCurrent = 1, 
				dCreatedDate = GETDATE(), -- USER MIGHT HAVE SCHEDULED THE CONVERSION BUT THIS WILL GET OVERWRITTEN HERE
				iIsAutoSchedule = 1
			FROM tblListLoadStatus
			WHERE BuildLoLID = @BuildLolId AND iIsCurrent = 0 AND LK_LoadStatus = '70'
		 END
	ELSE
		BEGIN
			INSERT INTO tblListLoadStatus
			(BuildLoLID, LK_LoadStatus, iIsCurrent, dCreatedDate, cCreatedBy,iIsAutoSchedule)
			VALUES
			(@BuildLolId, '70', 1, GETDATE(), @cScheduledBy,1)
		 END
		

   FETCH NEXT FROM db_cursor INTO @Id        
END
CLOSE db_cursor  
DEALLOCATE db_cursor 
GO


