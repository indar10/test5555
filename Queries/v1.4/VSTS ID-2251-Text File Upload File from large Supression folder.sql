---VSTS ID 2251 -Text File: Upload File from large Suppression folder


INSERT INTO tblConfiguration 
( 
DivisionID,  
DatabaseID, 
cItem, 
cDescription, 
cValue, 
iValue, 
dValue, 
mValue, 
iIsActive, 
cCreatedBy, 
dCreatedDate, 
iIsEncrypted  
) 
VALUES   
( 
0,  
0,  
'LargeSuppressionPath', 
'Web Processing', 
'\\stcsanisln01-idms\idmsdevtest\DDI_Upload\IDMSLargeSuppressionFiles\',  
0, 
NULL,
'', 
1, 
'SYSTEM', 
GETDATE(), 
0 
) 


