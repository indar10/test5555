-------------------------------------------
-- tblOwner
SELECT * -- count(1) 
FROM tblOwner with (nolock) 
WHERE UPPER(cCompany) like '%INFOGROUP%'

UPDATE 
	tblOwner
SET 
	cCompany = Replace(cCompany COLLATE Latin1_General_CI_AS,'infogroup','Data Axle'),
	cModifiedBy = 'SYSTEM',
	dModifiedDate = GETDATE()
WHERE 
	UPPER(cCompany) like '%INFOGROUP%'

--------------------------------------------
-- tblManager
SELECT * -- count(1) 
FROM tblManager  with (nolock) 
WHERE UPPER(cCompany) like '%INFOGROUP%'

UPDATE 
	tblManager
SET 
	cCompany = Replace(cCompany COLLATE Latin1_General_CI_AS,'infogroup','Data Axle'),
	cModifiedBy = 'SYSTEM',
	dModifiedDate = GETDATE()
WHERE 
	UPPER(cCompany) like '%INFOGROUP%'
	
--------------------------------------------
-- tblmailer
SELECT * -- count(1) 
FROM tblmailer  with (nolock) 
WHERE UPPER(cCompany) like '%INFOGROUP%'

UPDATE tblmailer
SET 
	cCompany = Replace(cCompany COLLATE Latin1_General_CI_AS,'infogroup','Data Axle'),
	cModifiedBy = 'SYSTEM',
	dModifiedDate = GETDATE()
WHERE UPPER(cCompany) like '%INFOGROUP%'

--------------------------------------------
-- tblbroker
SELECT * -- count(1) 
FROM tblbroker  with (nolock) 
WHERE UPPER(cCompany) like '%INFOGROUP%'

UPDATE tblbroker
SET 
	cCompany = Replace(cCompany COLLATE Latin1_General_CI_AS,'infogroup','Data Axle'),
	cModifiedBy = 'SYSTEM',
	dModifiedDate = GETDATE()
WHERE UPPER(cCompany) like '%INFOGROUP%' 

--------------------------------------------
-- tblshipto
SELECT * -- count(1) 
FROM tblshipto  with (nolock) 
WHERE UPPER(cCompany) like '%INFOGROUP%'

UPDATE tblshipto
SET 
	cCompany = Replace(cCompany COLLATE Latin1_General_CI_AS,'infogroup','Data Axle'),
	cModifiedBy = 'SYSTEM',
	dModifiedDate = GETDATE()
WHERE UPPER(cCompany) like '%INFOGROUP%' 

--------------------------------------------
-- tblDivisionBroker
SELECT * -- count(1) 
FROM tblDivisionBroker  with (nolock) 
WHERE UPPER(cCompany) like '%INFOGROUP%'

UPDATE tblDivisionBroker
SET 
	cCompany = Replace(cCompany COLLATE Latin1_General_CI_AS,'infogroup','Data Axle'),
	cModifiedBy = 'SYSTEM',
	dModifiedDate = GETDATE()
WHERE UPPER(cCompany) like '%INFOGROUP%' 

--------------------------------------------
-- tblDivisionMailer
SELECT * -- count(1) 
FROM tblDivisionMailer  with (nolock) 
WHERE UPPER(cCompany) like '%INFOGROUP%'

UPDATE tblDivisionMailer
SET 
	cCompany = Replace(cCompany COLLATE Latin1_General_CI_AS,'infogroup','Data Axle'),
	cModifiedBy = 'SYSTEM',
	dModifiedDate = GETDATE()
WHERE UPPER(cCompany) like '%INFOGROUP%' 

--------------------------------------------
-- tblDivisionShipto
SELECT * --count(1) 
FROM tblDivisionShipto  with (nolock) 
WHERE UPPER(cCompany) like '%INFOGROUP%'

UPDATE tblDivisionShipto
SET 
	cCompany = Replace(cCompany COLLATE Latin1_General_CI_AS,'infogroup','Data Axle'),
	cModifiedBy = 'SYSTEM',
	dModifiedDate = GETDATE()
WHERE UPPER(cCompany) like '%INFOGROUP%' 