using Abp.Domain.Repositories;
using Abp.UI;
using Infogroup.IDMS.AutoSuppresses;
using Infogroup.IDMS.Builds;
using Infogroup.IDMS.BuildTableLayouts.Dtos;
using Infogroup.IDMS.BuildTables;
using Infogroup.IDMS.IDMSConfigurations;
using System;
using System.Threading.Tasks;
using System.Linq;


namespace Infogroup.IDMS.BuildTableLayouts
{
    public class BuildTableLayoutManager : IDMSDomainServiceBase, IBuildTableLayoutManager
    {

        private readonly IBuildTableLayoutRepository _buildTableLayoutRepository;
        private readonly IRedisIDMSConfigurationCache _idmsConfigurationCache;
        private readonly IRepository<AutoSuppress, Guid> _autoSuppressRepository;
        private readonly IRepository<Build, int> _buildRepository;
        private readonly IRepository<BuildTable, int> _buildTableRepository;


        public BuildTableLayoutManager(IBuildTableLayoutRepository buildTableLayoutRepository,
                                      IRepository<AutoSuppress, Guid> autoSuppressRepository,
                                      IRepository<Build, int> buildRepository,
                                      IRepository<BuildTable, int> buildTableRepository,
                                      IRedisIDMSConfigurationCache idmsConfigurationCache)
        {
            _buildTableLayoutRepository = buildTableLayoutRepository;
            _idmsConfigurationCache = idmsConfigurationCache;
            _autoSuppressRepository = autoSuppressRepository;
            _buildTableRepository = buildTableRepository;
            _buildRepository = buildRepository;
        }

        public async Task<AdvanceSelectionFields> GetAdvanceSelectionFieldDetails(GetAdvanceFieldDetailsInputDto input)
        {
            string confValue;
            switch (input.Screen)
            {
                case AdvanceSelectionScreen.CountyCity:
                    confValue = _idmsConfigurationCache.GetConfigurationValue("StateCitySelection", input.DatabaseId)?.cValue ?? string.Empty;
                    var neighborHoodConfigValue = _idmsConfigurationCache.GetConfigurationValue("NeighborhoodField", input.DatabaseId)?.cValue;
                    if (string.IsNullOrEmpty(neighborHoodConfigValue) || neighborHoodConfigValue == "0")
                    {
                        neighborHoodConfigValue = string.Empty;
                    }
                    else
                    {
                        neighborHoodConfigValue = $"NeighborhoodSelect:{neighborHoodConfigValue}";
                    }
                    confValue = $"{confValue};{neighborHoodConfigValue}";
                    break;
                case AdvanceSelectionScreen.Industry:
                    confValue = _idmsConfigurationCache.GetConfigurationValue("SearchSICIndustry", input.DatabaseId)?.cValue ?? string.Empty;
                    break;
                case AdvanceSelectionScreen.GeoRadius:
                case AdvanceSelectionScreen.GeoMapping:
                    confValue = _idmsConfigurationCache.GetConfigurationValue("SearchGeoRadius", input.DatabaseId)?.cValue ?? string.Empty;
                    break;
                case AdvanceSelectionScreen.Occupation:
                    confValue = _idmsConfigurationCache.GetConfigurationValue("OccupationSelection", input.DatabaseId)?.cValue ?? string.Empty;
                    break;
                default:
                    throw new UserFriendlyException(this.L("InvalidcItemError"));
            }
            var result = new AdvanceSelectionFields();
            try
            {
                var confValues = confValue.Split(';');
                foreach (string value in confValues)
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        var confpair = value.Split(':');
                        var fielddetails = confpair[1].Split('.');
                        if (fielddetails.Length != 2)
                            continue;
                        var field = await _buildTableLayoutRepository.GetFieldDetailByName(input.BuildId.ToString(), fielddetails[0], fielddetails[1], input.DatabaseId);
                        if (!string.IsNullOrEmpty(field.cQuestionDescription))
                        {
                            switch (confpair[0].Trim().ToUpper())
                            {
                                case "SICCODE":
                                    result.SICCode = field;
                                    result.SICCode.cConfigFieldName = "SICCODE";
                                    break;
                                case "MINORINDUSTRYGROUP":
                                    result.MinorIndustryGroup = field;
                                    result.MinorIndustryGroup.cConfigFieldName = "MINORINDUSTRYGROUP";
                                    break;
                                case "MAJORINDUSTRYGROUP":
                                    result.MajorIndustryGroup = field;
                                    result.MajorIndustryGroup.cConfigFieldName = "MAJORINDUSTRYGROUP";
                                    break;
                                case "FRANCHISEBYSIC":
                                    result.FranchiseBySIC = field;
                                    result.FranchiseBySIC.cConfigFieldName = "FRANCHISEBYSIC";
                                    break;
                                case "INDUSTRYSPECIFICBYSIC":
                                    result.IndustrySpecificBySIC = field;
                                    result.IndustrySpecificBySIC.cConfigFieldName = "INDUSTRYSPECIFICBYSIC";
                                    break;
                                case "PRIMARYSICFLAG":
                                    result.PrimarySICFlag = field;
                                    result.PrimarySICFlag.cConfigFieldName = "PRIMARYSICFLAG";
                                    if (result.PrimarySICFlag.cValueMode.Equals("G"))
                                        result.PrimarySICFlag.Values = await _buildTableLayoutRepository.GetValues(field.ID);
                                    break;
                                case "STATESELECT":
                                    result.StateSelect = field;
                                    result.StateSelect.cConfigFieldName = "STATESELECT";
                                    break;
                                case "STATECOUNTYSELECT":
                                    result.StateCountySelect = field;
                                    result.StateCountySelect.cConfigFieldName = "STATECOUNTYSELECT";
                                    break;
                                case "STATECITYSELECT":
                                    result.StateCitySelect = field;
                                    result.StateCitySelect.cConfigFieldName = "STATECITYSELECT";
                                    break;
                                case "GEORADIUS":
                                    result.GeoRadius = field;
                                    break;
                                case "ZIPRADIUS":
                                    result.ZipRadius = field;
                                    break;
                                case "NEIGHBORHOODSELECT":
                                    result.NeighborhoodSelect = field;
                                    result.NeighborhoodSelect.cConfigFieldName = "NEIGHBORHOODSELECT";
                                    break;
                                case "INDUSTRYSELECTION":
                                    result.IndustrySelection = field;
                                    break;
                                case "OCCUPATIONSELECTION":
                                    result.OccupationSelection = field;
                                    break;
                                case "SPECIALTYSELECTION":
                                    result.SpecialtySelection = field;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
                if (confValues.Length > 0 && input.Screen == AdvanceSelectionScreen.GeoMapping)
                {
                    var field = await _buildTableLayoutRepository.GetFieldDetailByName(input.BuildId.ToString(), "tblMain", "GeoMapping", input.DatabaseId);
                    if (!string.IsNullOrEmpty(field?.cQuestionDescription))
                    {
                        result.GeoMapping = field;
                    }
                }
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
            return result;
        }

        public bool ContainsAutoSupress(int buildId, int databaseId)
        {
            try
            {
                return (from btl in _buildTableLayoutRepository.GetAll()
                        join bt in _buildTableRepository.GetAll()
                        on btl.BuildTableId equals bt.Id
                        join b in _buildRepository.GetAll()
                        on bt.BuildId equals b.Id
                        join aus in _autoSuppressRepository.GetAll()
                        on btl.cFieldName equals aus.cQuestionFieldName
                        where b.Id == buildId && aus.iIsActive && aus.DatabaseID == databaseId
                        select aus.Id).Any();
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
    }
}