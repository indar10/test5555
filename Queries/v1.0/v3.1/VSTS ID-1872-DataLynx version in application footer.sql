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
'DataLynxBuildVersion',
'WebProcess',
'1.0',
0,
NULL,
'',
1,
'SYSTEM',
GETDATE(),
0
)