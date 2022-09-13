using Abp.EntityFrameworkCore;
using Abp.UI;
using Infogroup.IDMS.EntityFrameworkCore;
using Infogroup.IDMS.EntityFrameworkCore.Repositories;
using Infogroup.IDMS.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Infogroup.IDMS.Occupations
{
    public class OccupationsRepository : IDMSRepositoryBase<Occupation, int>, IOccupationsRepository
    {
        private readonly DatabaseHelper.DatabaseHelper _databaseHelper;
        public OccupationsRepository(IDbContextProvider<IDMSDbContext> dbContextProvider, DatabaseHelper.DatabaseHelper databaseHelper)
            : base(dbContextProvider)
        {
            _databaseHelper = databaseHelper;
        }
        public List<DropdownOutputDto> GetOccupationValues(string query, List<SqlParameter> parameters)
        {
            try
            {
                _databaseHelper.EnsureConnectionOpen();
                var result = new List<DropdownOutputDto>();
                using (var command = _databaseHelper.CreateCommand(query, CommandType.Text, parameters.ToArray()))
                {
                    using (var dataReader = command.ExecuteReader())
                        while (dataReader.Read())
                            result.Add(new DropdownOutputDto
                            {
                                Value = dataReader["Code"].ToString(),
                                Label = dataReader["Description"].ToString(),
                            });
                }
                return result;
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }

        }
    }
}

