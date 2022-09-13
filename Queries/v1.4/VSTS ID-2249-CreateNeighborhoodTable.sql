


CREATE TABLE dbo.tblNeighborhood 
(
cState char(2) NOT NULL, 
cCity varchar(50) NOT NULL, 
cNeighborhood varchar(100) NOT NULL 
) 
GO 


CREATE CLUSTERED INDEX IX_tblNeighborhood_cState ON dbo.tblNeighborhood (cState)  

CREATE UNIQUE INDEX UIX_tblNeighborhood_cState_cNeighborhood ON dbo.tblNeighborhood (cState,cNeighborhood)  
