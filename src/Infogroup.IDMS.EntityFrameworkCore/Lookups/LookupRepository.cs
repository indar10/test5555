using Infogroup.IDMS.Lookups.Dtos;
using System;
using System.Collections.Generic;
using System.Data;
using Abp.Application.Services.Dto;
using System.Data.SqlClient;
using Abp.UI;
using Abp.Data;
using Abp.EntityFrameworkCore;
using Infogroup.IDMS.EntityFrameworkCore;
using Infogroup.IDMS.EntityFrameworkCore.Repositories;
using Infogroup.IDMS.Shared.Dtos;

namespace Infogroup.IDMS.Lookups
{
    public class LookupRepository : IDMSRepositoryBase<Lookup, int>, ILookupRepository
    {
        private readonly IActiveTransactionProvider _transactionProvider;
        private readonly DatabaseHelper.DatabaseHelper _databaseHelper;
        public LookupRepository(IDbContextProvider<IDMSDbContext> dbContextProvider, IActiveTransactionProvider transactionProvider, DatabaseHelper.DatabaseHelper databaseHelper)
            : base(dbContextProvider)
        {
            _transactionProvider = transactionProvider;
            _databaseHelper = databaseHelper;
        }

        public PagedResultDto<LookupDto> GetAllLookupList(Tuple<string, string, List<SqlParameter>> query)

        {
            try {
                _databaseHelper.EnsureConnectionOpen();

                var result = new PagedResultDto<LookupDto>();


                using (var command = _databaseHelper.CreateCommand(query.Item2, CommandType.Text, query.Item3.ToArray()))
                {
                    result.TotalCount = Convert.ToInt32(command.ExecuteScalar());
                    command.Parameters.Clear();
                }
                var LookupData = new List<LookupDto>();
                using (var command = _databaseHelper.CreateCommand(query.Item1, CommandType.Text, query.Item3.ToArray()))
                {
                    using (var dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            LookupData.Add(new LookupDto
                            {
                                ID = Convert.ToInt32(dataReader["ID"]),
                                cLookupValue = dataReader["cLookupValue"].ToString().Trim(),
                                iOrderBy = Convert.ToInt32(dataReader["iOrderBy"]),
                                cCode = dataReader["cCode"].ToString().Trim(),
                                cDescription = dataReader["cDescription"].ToString().Trim(),
                                cField = dataReader["cField"].ToString().Trim(),
                                mField = dataReader["mField"].ToString().Trim(),
                                iField = Convert.ToInt32(dataReader["iField"]),
                                iIsActive = Convert.ToBoolean(dataReader["iIsActive"])
                            });
                        }
                    }
                    result.Items = LookupData;
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }

        }

        public CreateOrEditLookupDto GetLookupById(Tuple<string, string, List<SqlParameter>> query)
        {
            _databaseHelper.EnsureConnectionOpen();

            var result = new CreateOrEditLookupDto();

            using (var command = _databaseHelper.CreateCommand(query.Item1, CommandType.Text, query.Item3.ToArray()))
            {
                using (var dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        result = new CreateOrEditLookupDto
                        {
                            cLookupValue = dataReader["cLookupValue"].ToString().Trim(),
                            iOrderBy = Convert.ToInt32(dataReader["iOrderBy"]),
                            cCode = dataReader["cCode"].ToString().Trim(),
                            cDescription = dataReader["cDescription"].ToString().Trim(),
                            cField = dataReader["cField"].ToString().Trim(),
                            mField = dataReader["mField"].ToString().Trim(),
                            iField = Convert.ToInt32(dataReader["iField"]),
                            iIsActive = Convert.ToBoolean(dataReader["iIsActive"]),
                            cCreatedBy = dataReader["cCreatedBy"].ToString().Trim(),
                            dCreatedDate = Convert.ToDateTime(dataReader["dCreatedDate"])
                        }; 
                    }
                }
            }
            return result;
        }


        public List<DropdownOutputDto> GetLookupData(string Query, List<SqlParameter> sqlParameters)
        {
            _databaseHelper.EnsureConnectionOpen();
            var lookupNames = new List<DropdownOutputDto>();
            using (var command = _databaseHelper.CreateCommand(Query, CommandType.Text, sqlParameters.ToArray()))
            {
                using (var dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        lookupNames.Add(new DropdownOutputDto { Label = dataReader["cLookupValue"].ToString(), Value = dataReader["cLookupValue"].ToString() });
                    }
                }
            }

            return lookupNames;
        }
    }

}
