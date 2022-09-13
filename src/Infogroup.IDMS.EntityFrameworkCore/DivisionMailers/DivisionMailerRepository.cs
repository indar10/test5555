using Abp.Application.Services.Dto;
using Abp.Data;
using Abp.EntityFrameworkCore;
using Infogroup.IDMS.EntityFrameworkCore;
using Infogroup.IDMS.EntityFrameworkCore.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Infogroup.IDMS.DivisionMailers.Dtos;
using Abp.UI;

namespace Infogroup.IDMS.DivisionMailers
{
    public class DivisionMailerRepository : IDMSRepositoryBase<DivisionMailer, int>, IDivisionMailerRepository
    {
        private readonly IActiveTransactionProvider _transactionProvider;
        private readonly DatabaseHelper.DatabaseHelper _databaseHelper;
        public const string sNoLock = " WITH (NOLOCK) ";

        public DivisionMailerRepository(IDbContextProvider<IDMSDbContext> dbContextProvider, IActiveTransactionProvider transactionProvider, DatabaseHelper.DatabaseHelper databaseHelper)
            : base(dbContextProvider)
        {
            _transactionProvider = transactionProvider;
            _databaseHelper = databaseHelper;
        }
        public PagedResultDto<GetDivisionMailerForViewDto> GetAllDivisionMailerList(Tuple<string, string, List<SqlParameter>> query)
        {
            _databaseHelper.EnsureConnectionOpen();
            try
            {               
                var result = new PagedResultDto<GetDivisionMailerForViewDto>();

                using (var command = _databaseHelper.CreateCommand(query.Item2, CommandType.Text, query.Item3.ToArray()))
                {
                    result.TotalCount = Convert.ToInt32(command.ExecuteScalar());
                    command.Parameters.Clear();
                }
                var divisionMailerData = new List<GetDivisionMailerForViewDto>();
                using (var command = _databaseHelper.CreateCommand(query.Item1, CommandType.Text, query.Item3.ToArray()))
                {
                    using (var dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            divisionMailerData.Add(new GetDivisionMailerForViewDto
                            {
                                Id = Convert.ToInt32(dataReader["ID"]),
                                Company = Convert.ToString(dataReader["CCOMPANY"]),
                                Code = Convert.ToString(dataReader["CCODE"]),
                                FirstName = Convert.ToString(dataReader["CFIRSTNAME"]),
                                LastName = Convert.ToString(dataReader["CLASTNAME"]),
                                DivisionName = Convert.ToString(dataReader["CDIVISIONNAME"]),
                                cAddr1 = Convert.ToString(dataReader["CADDR1"]),
                                cAddr2 = Convert.ToString(dataReader["cADDR2"]),
                                cCity = Convert.ToString(dataReader["CCITY"]),
                                cState = Convert.ToString(dataReader["CSTATE"]),
                                cZip = Convert.ToString(dataReader["CZIP"]),
                                cPhone = Convert.ToString(dataReader["CPHONE"]),
                                cFax = Convert.ToString(dataReader["CFAX"]),
                                Email = Convert.ToString(dataReader["CEMAIL"]),
                                Notes = Convert.ToString(dataReader["MNOTES"]),
                                IsActive = Convert.ToBoolean(dataReader["IISACTIVE"])
                            });
                        }
                    }
                    result.Items = divisionMailerData;
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }       
    }
}
