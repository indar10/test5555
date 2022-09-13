namespace Infogroup.IDMS.MasterLoLs.Dtos
{
    public class MasterLoLForViewDto
    {
		//public MasterLoLDto MasterLoL { get; set; }
        public int ID { get; set; }

        public string cListName { get; set; }

        public string cCode { get; set; }

        public string cNextMarkID { get; set; }

        public int OwnerID { get; set; }

        //public int ManagerID { get; set; }

        public string ListOwner { get; set; }

        public string ManagerName { get; set; }

        public bool iIsActive { get; set; }

        public bool iIsMultibuyer { get; set; }

        public string LK_PermissionType { get; set; }


        public string LK_ListType { get; set; }


        public string LK_ProductCode { get; set; }


        public string LK_DecisionGroup { get; set; }

        public string cEmailAddress { get; set; }

        public string permissionTypeValue { get; set; }

    }
}