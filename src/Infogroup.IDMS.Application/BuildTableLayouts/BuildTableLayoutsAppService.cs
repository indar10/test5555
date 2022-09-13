using Infogroup.IDMS.BuildTableLayouts.Dtos;
using Abp.UI;
using System;
using System.Collections.Generic;
using Abp.Authorization;
using System.Threading.Tasks;


namespace Infogroup.IDMS.BuildTableLayouts
{
    [AbpAuthorize]
    public class BuildTableLayoutsAppService : IDMSAppServiceBase, IBuildTableLayoutsAppService
    {
        private readonly IBuildTableLayoutRepository _customBuildTableLayoutRepository;
        private readonly IBuildTableLayoutManager _buildTableLayoutManager;
    

        public BuildTableLayoutsAppService(IBuildTableLayoutRepository customBuildTableLayoutRepository
            , IBuildTableLayoutManager buildTableLayoutManager)
        {
            _customBuildTableLayoutRepository = customBuildTableLayoutRepository;
            _buildTableLayoutManager = buildTableLayoutManager;
        }

        public List<GetBuildTableLayoutForViewDto> GetAllMultiFields(GetAllBuildTableLayoutsInput input)
        {
            try
            {
                return _customBuildTableLayoutRepository.GetAllMultiFieldsData(input);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public async Task<AdvanceSelectionFields> GetFieldDetails(GetAdvanceFieldDetailsInputDto input )
        {
            try
            {
                return await _buildTableLayoutManager.GetAdvanceSelectionFieldDetails(input);
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }
    }
}