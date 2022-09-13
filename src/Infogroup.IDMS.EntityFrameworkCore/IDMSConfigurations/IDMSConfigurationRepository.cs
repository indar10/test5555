using Abp.Application.Services.Dto;
using Abp.Data;
using Abp.EntityFrameworkCore;
using Abp.UI;
using Infogroup.IDMS.EntityFrameworkCore;
using Infogroup.IDMS.EntityFrameworkCore.Repositories;
using Infogroup.IDMS.IDMSConfigurations.Dtos;
using Infogroup.IDMS.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Infogroup.IDMS.IDMSConfigurations
{
    public class IDMSConfigurationRepository : IDMSRepositoryBase<IDMSConfiguration, int>, IIDMSConfigurationRepository
    {
        private readonly IActiveTransactionProvider _transactionProvider;
        private readonly DatabaseHelper.DatabaseHelper _databaseHelper;
        public IDMSConfigurationRepository(IDbContextProvider<IDMSDbContext> dbContextProvider, IActiveTransactionProvider transactionProvider, DatabaseHelper.DatabaseHelper databaseHelper)
            : base(dbContextProvider)
        {
            _transactionProvider = transactionProvider;
            _databaseHelper = databaseHelper;
        }
        public PagedResultDto<GetAllConfigurationsForViewDto> GetAllConfigurationListResult(Tuple<string, string, List<SqlParameter>> query)
        {
            try { 
            _databaseHelper.EnsureConnectionOpen();

            var result = new PagedResultDto<GetAllConfigurationsForViewDto>();


            using (var command = _databaseHelper.CreateCommand(query.Item2, CommandType.Text, query.Item3.ToArray()))
            {
                result.TotalCount = Convert.ToInt32(command.ExecuteScalar());
                command.Parameters.Clear();
            }
            var configurationList = new List<GetAllConfigurationsForViewDto>();
            using (var command = _databaseHelper.CreateCommand(query.Item1, CommandType.Text, query.Item3.ToArray()))
            {
                using (var dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        configurationList.Add(new GetAllConfigurationsForViewDto
                        {

                            ID = Convert.ToInt32(dataReader["ID"]),
                            DivisionID = Convert.ToInt32(dataReader["DivisionID"]),
                            DatabaseID = Convert.ToInt32(dataReader["DatabaseID"]),
                            cDatabaseName = dataReader["cDatabaseName"].ToString(),
                            cItem = dataReader["cItem"].ToString(),
                            cDescription = dataReader["cDescription"].ToString(),
                            cValue = dataReader["cValue"].ToString(),
                            iIsActive = Convert.ToBoolean(dataReader["iIsActive"])

                        }); 
                    }
                }
                result.Items = configurationList;
            }
            return result;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

       public List<DropdownOutputDto> GetItemsForDropdown()
        {
            _databaseHelper.EnsureConnectionOpen();
            var result =new List<DropdownOutputDto>();



            using (var command = _databaseHelper.CreateCommand($@"
                    SELECT DISTINCT(cItem) FROM tblConfiguration ORDER BY cItem", CommandType.Text))
            {
                command.CommandTimeout = 3 * 60;
                using (var dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        result.Add(new DropdownOutputDto
                        {
                            Value = dataReader["cItem"].ToString(),
                            Label = dataReader["cItem"].ToString(),
                        });
                    }
                }
            }
            return result;
        }

    }
}
