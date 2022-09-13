using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Infogroup.IDMS.Authorization.Users;
using Infogroup.IDMS.Authorization.Users.Profile;
using Infogroup.IDMS.Authorization.Users.Profile.Dto;
using Infogroup.IDMS.Test.Base;
using Shouldly;
using Xunit;

namespace Infogroup.IDMS.Tests.Authorization.Users
{
    // ReSharper disable once InconsistentNaming
    public class ProfileAppService_Tests : AppTestBase
    {
        private readonly IProfileAppService _profileAppService;

        public ProfileAppService_Tests()
        {
            _profileAppService = Resolve<IProfileAppService>();
        }

        [Fact]
        public async Task ChangePassword_Test()
        {
            //Act
            await _profileAppService.ChangePassword(
                new ChangePasswordInput
                {
                    CurrentPassword = "123qwe",
                    NewPassword = "2mF9d8Ac!5"
                });

            //Assert
            var currentUser = await GetCurrentUserAsync();

            LocalIocManager
                .Resolve<IPasswordHasher<User>>()
                .VerifyHashedPassword(currentUser, currentUser.Password, "2mF9d8Ac!5")
                .ShouldBe(PasswordVerificationResult.Success);
        } 
    }
}
