using System.Threading.Tasks;
using Infogroup.IDMS.BuildTableLayouts.Dtos;

namespace Infogroup.IDMS.BuildTableLayouts
{
    public interface IBuildTableLayoutManager
    {
        Task<AdvanceSelectionFields> GetAdvanceSelectionFieldDetails(GetAdvanceFieldDetailsInputDto input);
        bool ContainsAutoSupress(int buildId, int databaseId);
    }
}