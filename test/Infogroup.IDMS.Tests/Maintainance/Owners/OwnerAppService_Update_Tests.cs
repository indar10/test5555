using System.Linq;
using System.Threading.Tasks;
using Abp.Authorization.Users;
using Abp.UI;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Infogroup.IDMS.Authorization.Roles;
using Infogroup.IDMS.Authorization.Users;
using Infogroup.IDMS.Authorization.Users.Dto;
using Shouldly;
using Xunit;
using Infogroup.IDMS.Owners.Dtos;
using System;
using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.Tests.Maintainance.Owners
{
    // ReSharper disable once InconsistentNaming
    public class OwnerAppService_Update_Tests : OwnerAppServiceTestBase
    {
        [Fact]
        public async Task Should_Update_Owner()
        {
            var output = await OwnerAppService.GetOwnerForEdit(new EntityDto { Id = 1967 });

            await UpdateOwnerAndTestAsync(1967, output.cCompany, output.cAddress1, output.cAddress2, output.cCity, output.cState, output.cZip, output.cCode, output.cFax, output.cPhone, output.cNotes, output.iIsActive, output.cCreatedBy, output.dCreatedDate, output.DatabaseId);

        }

        private async Task UpdateOwnerAndTestAsync(int Id, string cCompany, string cAddress1, string cAddress2, string cCity, string cState, string cZip, string cCode, string cFax, string cPhone, string cNotes, bool iIsActive, string cCreatedBy, DateTime dCreatedDate, int DatabaseId)
        {

            await OwnerAppService.CreateOrEdit(
                new CreateOrEditOwnerDto
                {
                    Id = Id,   
                    DatabaseId = DatabaseId,
                    cCompany = cCompany,
                    cAddress1 = cAddress1,
                    cAddress2 = cAddress2,
                    cCity = cCity,
                    cState = cState,
                    cZip = cZip,
                    cCode = cCode.Trim(),
                    cFax = cFax,
                    cPhone = cPhone,
                    cNotes = cNotes,
                    iIsActive = iIsActive,
                    cCreatedBy = cCreatedBy,
                    dCreatedDate =dCreatedDate,
                    cModifiedBy = "sumits"

                }
                );


            await UsingDbContextAsync(async context =>
            {

                var updatedOwner = await context.Owners.FirstOrDefaultAsync(o => o.cCode == cCode);
                updatedOwner.ShouldNotBe(null);
                updatedOwner.Id.ShouldBe(Id);
                updatedOwner.cAddress1.ShouldBe(cAddress1);
                updatedOwner.cAddress2.ShouldBe(cAddress2);
                updatedOwner.cCity.ShouldBe(cCity);
                updatedOwner.cState.ShouldBe(cState);
                updatedOwner.cZip.ShouldBe(cZip);
                //updatedOwner.cCode.ShouldBe(cCode.Trim());
                updatedOwner.cCompany.ShouldBe(cCompany);
                updatedOwner.cFax.ShouldBe(cFax);
                updatedOwner.cPhone.ShouldBe(cPhone);
                updatedOwner.cNotes.ShouldBe(cNotes);
                updatedOwner.iIsActive.ShouldBe(iIsActive);
                                                          
            });
        }

        [Fact]
        public async Task Should_Not_Update_Owner_With_Duplicate_cCode()
        {
            var output = await OwnerAppService.GetOwnerForEdit(new EntityDto { Id = 1967 });
            await UpdateOwnerAndTestAsync(1967, output.cCompany, output.cAddress1, output.cAddress2, output.cCity, output.cState, output.cZip, "Test", output.cFax, output.cPhone, output.cNotes, output.iIsActive, output.cCreatedBy, output.dCreatedDate, output.DatabaseId);
        }

        [Fact]
        public async Task Should_Not_Update_Owner_With_Duplicate_cCompany()
        {
            var output = await OwnerAppService.GetOwnerForEdit(new EntityDto { Id = 1967 });
            await UpdateOwnerAndTestAsync(1967, "Test", output.cAddress1, output.cAddress2, output.cCity, output.cState, output.cZip, output.cCode, output.cFax, output.cPhone, output.cNotes, output.iIsActive, output.cCreatedBy, output.dCreatedDate, output.DatabaseId);
        }


        private async Task ShouldNotUpdateOwnerAndTestAsync(int Id, string cCompany, string cAddress1, string cAddress2, string cCity, string cState, string cZip, string cCode, string cFax, string cPhone, string cNotes, string cCreatedBy, DateTime dCreatedDate, int DatabaseId)
        {
            await Assert.ThrowsAsync<UserFriendlyException>(
                async () =>
                    await OwnerAppService.CreateOrEdit(
                        new CreateOrEditOwnerDto
                        {
                            Id = Id,
                            DatabaseId = DatabaseId,
                            cCompany = cCompany,
                            cAddress1 = cAddress1,
                            cAddress2 = cAddress2,
                            cCity = cCity,
                            cState = cState,
                            cZip = cZip,
                            cCode = cCode.Trim(),
                            cFax = cFax,
                            cPhone = cPhone,
                            cNotes = cNotes,
                            cCreatedBy = cCreatedBy,
                            dCreatedDate =dCreatedDate,
                            cModifiedBy = "sumits"

                        }));
        }

        


       
    }
}
