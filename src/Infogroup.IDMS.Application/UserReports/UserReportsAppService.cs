using Infogroup.IDMS.IDMSUsers;
using Infogroup.IDMS.Reports;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Infogroup.IDMS.UserReports.Dtos;
using Infogroup.IDMS.Dto;
using Abp.Application.Services.Dto;
using Infogroup.IDMS.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;
using Abp.UI;

namespace Infogroup.IDMS.UserReports
{
	[AbpAuthorize]
    public class UserReportsAppService : IDMSAppServiceBase, IUserReportsAppService
    {
		 private readonly IRepository<UserReport> _userReportRepository;
		 private readonly IRepository<IDMSUser,int> _userRepository;
		 private readonly IRepository<Report,int> _lookup_reportRepository;
		 

		  public UserReportsAppService(IRepository<UserReport> userReportRepository , IRepository<IDMSUser, int> userRepository, IRepository<Report, int> lookup_reportRepository) 
		  {
			_userReportRepository = userReportRepository;
            _userRepository = userRepository;
		    _lookup_reportRepository = lookup_reportRepository;
		
		  }

		 public async Task<PagedResultDto<GetUserReportForViewDto>> GetAll(GetAllUserReportsInput input)
         {

            try
            { 
                var filteredUserReports = _userReportRepository.GetAll()
						.Include( e => e.IDMSUserFk)
						.Include( e => e.ReportFk)
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false )
						.WhereIf(input.MinReportIDFilter != null, e => e.ReportID >= input.MinReportIDFilter)
						.WhereIf(input.MaxReportIDFilter != null, e => e.ReportID <= input.MaxReportIDFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.TblUsercFirstNameFilter), e => e.IDMSUserFk != null && e.IDMSUserFk.cFirstName == input.TblUsercFirstNameFilter)
                        .WhereIf(input.TBlUserIDFIlter != null, e => e.IDMSUserFk.cUserID == input.TBlUserIDFIlter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ReportcReportNameFilter), e => e.ReportFk != null && e.ReportFk.cReportName == input.ReportcReportNameFilter);

			var pagedAndFilteredUserReports = filteredUserReports
                .OrderBy(input.Sorting ?? "ID asc")
                .PageBy(input);

            var userReports = from o in pagedAndFilteredUserReports
                              join o1 in _userRepository.GetAll() on o.UserID equals o1.Id into j1
                              from s1 in j1.DefaultIfEmpty()

                              join o2 in _lookup_reportRepository.GetAll() on o.ReportID equals o2.Id into j2
                              from s2 in j2.DefaultIfEmpty()

                              select new GetUserReportForViewDto() {
                                  UserReport = new UserReportDto
                                  {
                                      ReportID = o.ReportID,
                                      Id = o.Id,
                            },
                         	TblUsercFirstName = s1 == null ? "" : s1.cFirstName.ToString(),
                         	ReportcReportName = s2 == null ? "" : s2.cReportName.ToString()
						};

            var totalCount = await filteredUserReports.CountAsync();

            return new PagedResultDto<GetUserReportForViewDto>(
                totalCount,
                await userReports.ToListAsync()
            );
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
		 
		 public async Task<GetUserReportForEditOutput> GetUserReportForEdit(EntityDto input)
         {
            var userReport = await _userReportRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetUserReportForEditOutput {UserReport = ObjectMapper.Map<CreateOrEditUserReportDto>(userReport)};

		    if (output.UserReport.UserID != null)
            {
                var _lookupTblUser = await _userRepository.FirstOrDefaultAsync((int)output.UserReport.UserID);
                output.TblUsercFirstName = _lookupTblUser.cFirstName.ToString();
            }

		    if (output.UserReport.ReportID != null)
            {
                var _lookupReport = await _lookup_reportRepository.FirstOrDefaultAsync((int)output.UserReport.ReportID);
                output.ReportcReportName = _lookupReport.cReportName.ToString();
            }
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditUserReportDto input)
         {

            if(input.Id == null){
                try
                { 
				    await Create(input);
                }
                catch (Exception ex)
                {
                    throw new UserFriendlyException(ex.Message);
                }

            }
			else{
				await Update(input);
			}
         }

		 protected virtual async Task Create(CreateOrEditUserReportDto input)
         {
            try { 
            var userReport = ObjectMapper.Map<UserReport>(input);
            await _userReportRepository.InsertAsync(userReport);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

		 protected virtual async Task Update(CreateOrEditUserReportDto input)
         {
            var userReport = await _userReportRepository.FirstOrDefaultAsync((int)input.Id);
             ObjectMapper.Map(input, userReport);
         }

         public async Task Delete(EntityDto input)
         {
            await _userReportRepository.DeleteAsync(input.Id);
         } 

         public async Task<PagedResultDto<UserReportTblUserLookupTableDto>> GetAllTblUserForLookupTable(GetAllForLookupTableInput input)
         {
             var query = _userRepository.GetAll().WhereIf(
                    !string.IsNullOrWhiteSpace(input.Filter),
                   e=> e.cFirstName.ToString().Contains(input.Filter)
                );

            var totalCount = await query.CountAsync();

            var tblUserList = await query
                .PageBy(input)
                .ToListAsync();

			var lookupTableDtoList = new List<UserReportTblUserLookupTableDto>();
			foreach(var tblUser in tblUserList){
				lookupTableDtoList.Add(new UserReportTblUserLookupTableDto
				{
					Id = tblUser.Id,
					DisplayName = tblUser.cFirstName?.ToString()
				});
			}

            return new PagedResultDto<UserReportTblUserLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
         }

         public async Task<PagedResultDto<UserReportReportLookupTableDto>> GetAllReportForLookupTable(GetAllForLookupTableInput input)
         {
             var query = _lookup_reportRepository.GetAll().WhereIf(
                    !string.IsNullOrWhiteSpace(input.Filter),
                   e=> e.cReportName.ToString().Contains(input.Filter)
                );

            var totalCount = await query.CountAsync();

            var reportList = await query
                .PageBy(input)
                .ToListAsync();

			var lookupTableDtoList = new List<UserReportReportLookupTableDto>();
			foreach(var report in reportList){
				lookupTableDtoList.Add(new UserReportReportLookupTableDto
				{
					Id = report.Id,
					DisplayName = report.cReportName?.ToString()
				});
			}

            return new PagedResultDto<UserReportReportLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
         }
    }
}