using Infogroup.IDMS.Offers;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Infogroup.IDMS.OfferSamples.Dtos;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.Authorization;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;
using Infogroup.IDMS.Databases;
using Infogroup.IDMS.Divisions;
using Abp.UI;
using Infogroup.IDMS.Sessions;
using System.IO;
using Infogroup.IDMS.Dto;
using Abp.AspNetZeroCore.Net;

namespace Infogroup.IDMS.OfferSamples
{
    [AbpAuthorize(AppPermissions.Pages_OfferSamples)]
    public class OfferSamplesAppService : IDMSAppServiceBase, IOfferSamplesAppService
    {
        private readonly IRepository<OfferSample, int> _offerSampleRepository;
        private readonly AppSession _mySession;
        private readonly IRepository<Offer, int> _offerRepository;
        private readonly IDatabaseRepository _customDatabaseRepository;
        private readonly IRepository<Division, int> _divisionRepository;

        public OfferSamplesAppService(
            IRepository<OfferSample, int> offerSampleRepository,
            IRepository<Offer, int> offerRepository,
            IDatabaseRepository customDatabaseRepository,
            IRepository<Division, int> divisionRepository,
            AppSession mySession)
        {
            _offerRepository = offerRepository;
            _offerSampleRepository = offerSampleRepository;
            _customDatabaseRepository = customDatabaseRepository;
            _mySession = mySession;
            _divisionRepository = divisionRepository;
        }

        #region Fetch Offer Samples
        public async Task<PagedResultDto<OfferSampleDto>> GetAllOfferSamples(GetAllOfferSamplesInput input)
        {
            try
            {
                var filteredOfferSamples = _offerSampleRepository.GetAll()
                         .WhereIf(input.OfferId > -1, e => Convert.ToInt32(e.OfferId) == input.OfferId)
                         .Select(o => new OfferSampleDto
                         {
                             cDescription = o.cDescription,
                             Id = o.Id,
                             cFileName = o.cFileName
                         });
                var totalCount = await filteredOfferSamples.CountAsync();

                var pagedAndFilteredOfferSamples = filteredOfferSamples
                    .OrderBy(input.Sorting ?? OfferSampleConsts.IdAsc)
                    .PageBy(input);

                return new PagedResultDto<OfferSampleDto>(
                    totalCount,
                    await pagedAndFilteredOfferSamples.ToListAsync()
                );
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public async Task<CreateOrEditOfferSampleDto> GetOfferSampleForEdit(EntityDto input)
        {
            try
            {
                var offerSample = await _offerSampleRepository.FirstOrDefaultAsync(input.Id);
                var output = ObjectMapper.Map<CreateOrEditOfferSampleDto>(offerSample);
                return output;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        #endregion

        #region Save Offer Samples
        [AbpAuthorize(AppPermissions.Pages_OfferSamples_Create, AppPermissions.Pages_OfferSamples_Edit)]
        public void CreateOrEdit(CreateOrEditOfferSampleDto input, string clientFileName, string path)
        {
            try
            {
                input = CommonHelpers.ConvertNullStringToEmptyAndTrim(input);
                OfferSample offerSample;
                if (input.Id == null)
                {
                    input.cCreatedBy = _mySession.IDMSUserName;
                    input.dCreatedDate = DateTime.Now;
                    offerSample = ObjectMapper.Map<OfferSample>(input);
                    input.Id = _offerSampleRepository.InsertAndGetId(offerSample);
                }
                else
                {
                    offerSample = _offerSampleRepository.Get(input.Id.GetValueOrDefault());
                    input.cModifiedBy = _mySession.IDMSUserName;
                    input.dModifiedDate = DateTime.Now;
                    ObjectMapper.Map(input, offerSample);
                    CurrentUnitOfWork.SaveChanges();
                }
                if (!string.IsNullOrWhiteSpace(path))
                {
                    var mailerId = _offerRepository.GetAll().FirstOrDefault(p => p.Id == input.OfferId).MailerId;
                    offerSample.cFileName = $"{OfferSampleConsts.Sample}_{mailerId.ToString()}_{input.OfferId.ToString()}_{input.Id.ToString()}_{clientFileName}";
                    var desFile = path.Substring(0, path.LastIndexOf(OfferSampleConsts.Separator) + 1) + offerSample.cFileName;
                    if (File.Exists(path))
                    {
                        File.Move(path, desFile);
                    }

                    CurrentUnitOfWork.SaveChanges();
                }

            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        #endregion

        #region Delete Offer Samples
        [AbpAuthorize(AppPermissions.Pages_OfferSamples_Delete)]
        public async Task Delete(EntityDto input)
        {
            try
            {
                await _offerSampleRepository.DeleteAsync(input.Id);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        #endregion

        #region Download Offer Samples
        public FileDto DownloadOfferSample(int databaseId, int offerSampleId)
        {
            try
            {
                var divisionId = _customDatabaseRepository.GetAll().FirstOrDefault(p => p.Id == databaseId).DivisionId;
                var filePath = _divisionRepository.GetAll().FirstOrDefault(p => p.Id == divisionId).cOfferPath;

                var FileName = _offerSampleRepository.FirstOrDefault(offerSampleId).cFileName;
                FileName = Path.Combine(filePath, FileName);
                return new FileDto(FileName, MimeTypeNames.ApplicationOctetStream);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        #endregion
    }
}