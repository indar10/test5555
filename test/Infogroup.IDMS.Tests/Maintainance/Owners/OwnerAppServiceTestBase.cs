using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Authorization.Users;
using Infogroup.IDMS.Authorization.Users;
using Infogroup.IDMS.Owners;
using Infogroup.IDMS.Test.Base;

namespace Infogroup.IDMS.Tests.Maintainance.Owners
{
    public abstract class OwnerAppServiceTestBase : AppTestBase
    {
        protected readonly IOwnersAppService OwnerAppService;

        protected OwnerAppServiceTestBase()
        {
            OwnerAppService = Resolve<IOwnersAppService>();
        }

        protected void CreateTestOwners()
        {
            //Note: There is a default "admin" user also

              UsingDbContext(
                context =>
                {
                    context.Owners.Add(CreateOwnerEntity(955, "Onwer1Address1", "Owner1Address2", "Owner1City","AB","123123123","OWNER1CODE","OWNER1Company","Owner1Fax","123123123","Owner1Notes"));
                    context.Owners.Add(CreateOwnerEntity(955, "Onwer2Address1", "Owner2Address2", "Owner2City", "AB", "123123123", "OWNER2CODE", "OWNER2Company", "Owner2Fax", "123123123", "Owner2Notes"));
                    context.Owners.Add(CreateOwnerEntity(955, "Onwer3Address1", "Owner3Address2", "Owner3City", "AB", "123123123", "OWNER3CODE", "OWNER3Company", "Owner3Fax", "123123123", "Owner3Notes"));
                });
        }

        protected  Owner CreateOwnerEntity(int databaseId , string cAddress1, string cAddress2, string cCity, string cState, string cZip, string cCode, string cCompany,string cFax,string cPhone,string cNotes)
        {
            var owner = new Owner
            {
                DatabaseId = databaseId,
                cAddress1 = cAddress1,
                cAddress2 = cAddress2,
                cCity = cCity,
                cState = cState,
                cZip = cZip, 
                cCode = cCode,
                cCompany=cCompany,
                cFax=cFax,
                cPhone=cPhone,
                cNotes=cNotes,
                //dCreatedDate=DateTime.Now,
                cCreatedBy="Test"

                
            };

           
            return owner;
        }
    }
}