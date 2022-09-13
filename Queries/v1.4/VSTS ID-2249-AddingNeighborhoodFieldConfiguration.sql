
--Example for cValue for database other than 0 : tblMain.Neighborhood 


INSERT INTO [DW_Admin].[dbo].[tblConfiguration] 
           ([DivisionID]   
           ,[DatabaseID] 
           ,[cItem]  
           ,[cDescription] 
           ,[cValue] 
           ,[iValue] 
           ,[dValue]  
           ,[mValue]  
           ,[iIsActive]
           ,[cCreatedBy]  
           ,[cModifiedBy] 
           ,[dCreatedDate] 
           ,[iIsEncrypted]) 
           VALUES
           (0  
           ,0  
           ,'NeighborhoodField'  
		   ,'WebProcessing' 
		   ,'0' 
           ,0 
           ,null 
           ,''  
           ,1  
           ,'SYSTEM' 
           ,''
           ,GETDATE()  
           ,0) 




		  
