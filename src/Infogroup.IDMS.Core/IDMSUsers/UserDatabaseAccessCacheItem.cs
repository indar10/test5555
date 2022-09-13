namespace Infogroup.IDMS.IDMSUsers
{
	public class UserDatabaseAccessObjectCacheItem 
    {
		public bool ListAccess { get; set; }		
		public bool AddEditAccess { get; set; }				
		public int AccessObjectId { get; set; }			
		public int DatabaseId { get; set; }				
    }
}