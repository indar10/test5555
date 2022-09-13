using Abp.EntityFrameworkCore;
using Infogroup.IDMS.EntityFrameworkCore;
using Infogroup.IDMS.EntityFrameworkCore.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Infogroup.IDMS.Shared.Dtos;
using Infogroup.IDMS.Lookups;
using Abp.UI;

namespace Infogroup.IDMS.Shared
{
    public class SharedRepository : IDMSRepositoryBase<Lookup, int>, ISharedRepository
    {
        private readonly DatabaseHelper.DatabaseHelper _databaseHelper;
        public const string sNoLock = " WITH (NOLOCK) ";
        public SharedRepository(IDbContextProvider<IDMSDbContext> dbContextProvider, DatabaseHelper.DatabaseHelper databaseHelper)
            : base(dbContextProvider)
        {
            _databaseHelper = databaseHelper;
        }
        public List<DropdownOutputDto> GetDropdownOptionsForNumericValues(Tuple<string, List<SqlParameter>> query)
        {
            try
            {
                _databaseHelper.EnsureConnectionOpen();
                var result = new List<DropdownOutputDto>();
                using (var command = _databaseHelper.CreateCommand(query.Item1, CommandType.Text, query.Item2.ToArray()))
                {
                    using (var dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                            result.Add(new DropdownOutputDto
                            {
                                Value = Convert.ToInt32(dataReader["iValue"]),
                                Label = dataReader["cLabel"].ToString(),
                            });
                        command.Parameters.Clear();
                    }
                }
                return result;
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }

        public List<DropdownOutputDto> GetDropdownOptionsForAlphaNumericValues(Tuple<string, List<SqlParameter>> query)
        {
            try
            {
                _databaseHelper.EnsureConnectionOpen();
                var result = new List<DropdownOutputDto>();
                using (var command = _databaseHelper.CreateCommand(query.Item1, CommandType.Text, query.Item2.ToArray()))
                {
                    using (var dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                            result.Add(new DropdownOutputDto
                            {
                                Value = dataReader["cValue"].ToString(),
                                Label = dataReader["cLabel"].ToString(),
                            });
                        command.Parameters.Clear();
                    }                       
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