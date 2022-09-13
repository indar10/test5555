using System.Linq;
using System.Threading.Tasks;
using Abp.Collections.Extensions;
using Abp.MultiTenancy;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using Infogroup.IDMS.Authorization.Roles;
using Infogroup.IDMS.Authorization.Users.Dto;
using Shouldly;
using Xunit;
using System;
using Infogroup.IDMS.Owners.Dtos;

namespace Infogroup.IDMS.Tests.Maintainance.Owners
{
    // ReSharper disable once InconsistentNaming
    public class OwnerAppService_Create_Tests : OwnerAppServiceTestBase
    {
        
        [Fact]
        public async Task Should_Create_Owner()
        {
            
            await CreateOwnerAndTestAsync(955, "Onwer1Address1", "Owner1Address2", "Owner1City", "AB", "123123123", "OWNER1CODE2225", "OWNER1Company2225", "Owner1Fax", "123123123", "Owner1Notes");
            
        }        

        private async Task CreateOwnerAndTestAsync(int databaseId, string cAddress1, string cAddress2, string cCity, string cState, string cZip, string cCode, string cCompany, string cFax, string cPhone, string cNotes)
        {
           
            await OwnerAppService.CreateOrEdit(
                new CreateOrEditOwnerDto
                {
                    DatabaseId = databaseId,
                    cAddress1 = cAddress1,
                    cAddress2 = cAddress2,
                    cCity = cCity,
                    cState = cState,
                    cZip = cZip,
                    cCode = cCode.Trim(),
                    cCompany = cCompany,
                    cFax = cFax,
                    cPhone = cPhone,
                    cNotes = cNotes,
                    cCreatedBy ="sumits",
                    dCreatedDate=DateTime.Now
                   
                }
                );

           
            await UsingDbContextAsync(async context =>
            {
                
                var createdOwner = await context.Owners.FirstOrDefaultAsync(o => o.cCode == cCode);
                createdOwner.ShouldNotBe(null);
                createdOwner.DatabaseId.ShouldBe(databaseId);
                createdOwner.cAddress1.ShouldBe(cAddress1);
                createdOwner.cAddress2.ShouldBe(cAddress2);
                createdOwner.cCity.ShouldBe(cCity);
                createdOwner.cState.ShouldBe(cState);
                createdOwner.cZip.ShouldBe(cZip);
                createdOwner.cCode.ShouldBe(cCode.Trim());
                createdOwner.cCompany.ShouldBe(cCompany);
                createdOwner.cFax.ShouldBe(cFax);
                createdOwner.cPhone.ShouldBe(cPhone);
                createdOwner.cNotes.ShouldBe(cNotes);



               
               
            });
        }

        [Fact]
        public async Task Should_Not_Create_Owner_With_Duplicate_cCode()
        {
            await ShouldNotCreateOwnerAndTestAsync(955, "Onwer1Address1", "Owner1Address2", "Owner1City", "AB", "123123123", "OWNER1CODE2225", "OWNER1Company2225", "Owner1Fax", "123123123", "Owner1Notes");
        }


        private async Task ShouldNotCreateOwnerAndTestAsync(int databaseId, string cAddress1, string cAddress2, string cCity, string cState, string cZip, string cCode, string cCompany, string cFax, string cPhone, string cNotes)
        {
            await Assert.ThrowsAsync<UserFriendlyException>(
                async () =>
                    await OwnerAppService.CreateOrEdit(
                        new CreateOrEditOwnerDto
                        {
                            DatabaseId = databaseId,
                            cAddress1 = cAddress1,
                            cAddress2 = cAddress2,
                            cCity = cCity,
                            cState = cState,
                            cZip = cZip,
                            cCode = cCode.Trim(),
                            cCompany = cCompany,
                            cFax = cFax,
                            cPhone = cPhone,
                            cNotes = cNotes,
                            cCreatedBy = "sumits",
                            dCreatedDate = DateTime.Now

                        }));
        }
    }
}
