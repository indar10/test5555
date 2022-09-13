using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Infogroup.IDMS.Authorization.Users.Dto;
using Infogroup.IDMS.Authorization.Users.Profile.Dto;
using Infogroup.IDMS.Dto;

namespace Infogroup.IDMS.Authorization.Users.Profile
{
    public interface IProfileAppService : IApplicationService
    {

        Task ChangePassword(ChangePasswordInput input);

        Task UpdateProfilePicture(UpdateProfilePictureInput input);

        Task<GetPasswordComplexitySettingOutput> GetPasswordComplexitySetting();

        Task<GetProfilePictureOutput> GetProfilePicture();

        Task<GetProfilePictureOutput> GetProfilePictureById(Guid profilePictureId);

        Task<GetProfilePictureOutput> GetFriendProfilePictureById(GetFriendProfilePictureByIdInput input);

        Task ChangeLanguage(ChangeUserLanguageDto input);

        Task VerifySmsCode(VerifySmsCodeInputDto input);

    }
}
