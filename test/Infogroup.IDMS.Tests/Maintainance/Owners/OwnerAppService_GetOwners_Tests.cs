using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Authorization.Users;
using Infogroup.IDMS.Authorization.Users.Dto;
using Infogroup.IDMS.Owners.Dtos;
using Shouldly;
using Xunit;

namespace Infogroup.IDMS.Tests.Maintainance.Owners
{
    // ReSharper disable once InconsistentNaming
    public class OwnerAppService_GetUsers_Tests : OwnerAppServiceTestBase
    {
        [Fact]
        public void Should_Get_Initial_Owners()
        {
            //Act
            var output = OwnerAppService.GetAllOwners(new IDMS.Shared.Dtos.GetAllSetupInput { SelectedDatabase = 65 });

            //Assert
            output.TotalCount.ShouldBe(1);
            output.Items.Count.ShouldBe(3);
            
        }

        [Fact]
        public void Should_Get_Owners_Paged_And_Sorted_And_Filtered()
        {



            var output = OwnerAppService.GetAllOwners(
                new IDMS.Shared.Dtos.GetAllSetupInput
                {
                    SelectedDatabase = 65,
                    MaxResultCount=10,
                    Sorting="cCompany"

                });

            
            output.TotalCount.ShouldBe(3);
            output.Items.Count.ShouldBe(3);
            output.Items[0].cCompany.ShouldBe("za");
            
        }

        [Fact]
        public void Should_Get_Owners_Filtered()
        {
            

           
            var output =  OwnerAppService.GetAllOwners(
                new IDMS.Shared.Dtos.GetAllSetupInput
                {
                    SelectedDatabase = 65,
                    Filter = "za"
                });

            
            output.TotalCount.ShouldBe(1);
            output.Items.Count.ShouldBe(1);
            output.Items[0].cCompany.ShouldBe("za");

          
        }
    }
}
