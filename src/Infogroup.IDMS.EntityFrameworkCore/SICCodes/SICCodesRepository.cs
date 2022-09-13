using Abp.EntityFrameworkCore;
using Abp.UI;
using Infogroup.IDMS.EntityFrameworkCore;
using Infogroup.IDMS.EntityFrameworkCore.Repositories;
using Infogroup.IDMS.Shared.Dtos;
using Infogroup.IDMS.SICCodes.Dtos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Infogroup.IDMS.SICCodes
{
    public class SICCodesRepository : IDMSRepositoryBase<SICCode, int>, ISICCodesRepository
    {
        private readonly DatabaseHelper.DatabaseHelper _databaseHelper;
        public SICCodesRepository(IDbContextProvider<IDMSDbContext> dbContextProvider, DatabaseHelper.DatabaseHelper databaseHelper)
            : base(dbContextProvider)
        {
            _databaseHelper = databaseHelper;
        }
        public List<SICCodeDto> GetSICCodes(Tuple<string, List<SqlParameter>> query)
        {
            try
            {
                _databaseHelper.EnsureConnectionOpen();
                var result = new List<SICCodeDto>();
                using (var command = _databaseHelper.CreateCommand(query.Item1, CommandType.Text, query.Item2.ToArray()))
                {
                    using (var dataReader = command.ExecuteReader())
                        while (dataReader.Read())
                            result.Add(new SICCodeDto
                            {
                                SICCode = dataReader["Code"].ToString(),
                                SICDescription = dataReader["Description"].ToString(),
                                Indicator = dataReader["Indicator"].ToString(),
                                RelatedSICCode = dataReader["RelatedCode"].ToString(),
                                RelatedSICDescription = dataReader["RelatedDescription"].ToString(),
                                RelatedIndicator = dataReader["RelatedIndicator"].ToString()
                            });
                }
                return result;
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }

        }
        public (List<SICCode>, List<string>) GetSICCodesForSmartAdd(Tuple<string, List<SqlParameter>> query)
        {
            try
            {
                _databaseHelper.EnsureConnectionOpen();
                var result = new List<SICCode>();
                var validCodes = new List<string>();
                if (query.Item2.Count > 0)
                    using (var command = _databaseHelper.CreateCommand(query.Item1, CommandType.Text, query.Item2.ToArray()))
                    {
                        using (var dataReader = command.ExecuteReader())
                            while (dataReader.Read())
                            {
                                var code = dataReader["Code"].ToString();
                                result.Add(new SICCode
                                {
                                    cSICCode = code,
                                    cSICDescription = dataReader["Description"].ToString(),
                                    cType = dataReader["Type"].ToString(),
                                });
                                validCodes.Add(code);
                            }
                    }

                return (result, validCodes);
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }

        }

        public int GetSICCodesCount(Tuple<string, List<SqlParameter>> query)
        {
            try
            {
                _databaseHelper.EnsureConnectionOpen();
                int count = 0;
                using (var command = _databaseHelper.CreateCommand(query.Item1, CommandType.Text, query.Item2.ToArray()))
                {
                    count = Convert.ToInt32(command.ExecuteScalar());
                    command.Parameters.Clear();
                }
                return count;
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }
        public List<DropdownOutputDto> GetFranchiseIndustryBySIC(Tuple<string, List<SqlParameter>> query)
        {
            try
            {
                _databaseHelper.EnsureConnectionOpen();
                var result = new List<DropdownOutputDto>();
                using (var command = _databaseHelper.CreateCommand(query.Item1, CommandType.Text, query.Item2.ToArray()))
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

