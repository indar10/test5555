using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Authorization.Users;
using Infogroup.IDMS.Authorization.Roles;
using Infogroup.IDMS.Authorization.Users;
using Infogroup.IDMS.Owners.Dtos;
using Shouldly;
using Xunit;

namespace Infogroup.IDMS.Tests.Maintainance.Owners
{
    // ReSharper disable once InconsistentNaming
    public class OwnerAppService_GetOwnerForEdit_Tests : OwnerAppServiceTestBase
    {
       

        [Fact]
        public async Task Should_Work_For_Existing_Owner()
        {
            
            //Act
            var output =await OwnerAppService.GetOwnerForEdit(new EntityDto{Id= 1967 });
            output.ShouldNotBe(null);
            //Assert
            output.Id.ShouldBe(1967);
            //output.cCode.ShouldBe("TestCode");
            output.cCompany.ShouldBe("TestCompany");
            output.cAddress1.ShouldBe("TestAddress1");
            output.cAddress2.ShouldBe("TestAddress2");
            output.cCity.ShouldBe("TestCity");
            output.cState.ShouldBe("MH");
            output.cZip.ShouldBe("87987");
            output.cPhone.ShouldBe("566-646-4666");
            output.cFax.ShouldBe("566-646-4666");
            output.cNotes.ShouldBe("testnotes");
            output.iIsActive.ShouldBe(true);


        }

        protected Role CreateRole(string roleName)
        {
            return UsingDbContext(context => context.Roles.Add(new Role(AbpSession.TenantId, roleName, roleName)).Entity);
        }
    }
}
