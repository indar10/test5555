using Abp.Data;
using Abp.EntityFrameworkCore;
using Infogroup.IDMS.EntityFrameworkCore;
using Infogroup.IDMS.EntityFrameworkCore.Repositories;
using Infogroup.IDMS.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Data;
using Abp.Application.Services.Dto;
using System.Data.SqlClient;
using Infogroup.IDMS.Databases;

namespace Infogroup.IDMS.Models
{
    public class ModelsRepository : IDMSRepositoryBase<Model, int>, IModelsRepository
    {

        private readonly IActiveTransactionProvider _transactionProvider;
        private readonly DatabaseHelper.DatabaseHelper _databaseHelper;
        public ModelsRepository(IDbContextProvider<IDMSDbContext> dbContextProvider, IActiveTransactionProvider transactionProvider, DatabaseHelper.DatabaseHelper databaseHelper)
            : base(dbContextProvider)
        {
            _transactionProvider = transactionProvider;
            _databaseHelper = databaseHelper;
        }

        public PagedResultDto<ModelScoringDto> GetAllModelsList(Tuple<string, string, List<SqlParameter>> query)
        {
            _databaseHelper.EnsureConnectionOpen();

            var result = new PagedResultDto<ModelScoringDto>();


            using (var command = _databaseHelper.CreateCommand(query.Item2, CommandType.Text, query.Item3.ToArray()))
            {
                result.TotalCount = Convert.ToInt32(command.ExecuteScalar());
                command.Parameters.Clear();
            }
            var modelData = new List<ModelScoringDto>();
            using (var command = _databaseHelper.CreateCommand(query.Item1, CommandType.Text, query.Item3.ToArray()))
            {
                using (var dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        var DatabaseName = dataReader["DatabaseName"].ToString().Trim();
                        if (!string.IsNullOrEmpty(DatabaseName) && (DatabaseName.ToLower().StartsWith(DatabaseNameConst.Infogroup) || DatabaseName.ToLower().EndsWith(DatabaseNameConst.Database)))
                        {
                            DatabaseName = DatabaseName.Replace(DatabaseNameConst.Database, "", StringComparison.OrdinalIgnoreCase);
                            DatabaseName = DatabaseName.Replace(DatabaseNameConst.Infogroup, "", StringComparison.OrdinalIgnoreCase);
                        }
                        modelData.Add(new ModelScoringDto
                        {
                            Id = Convert.ToInt32(dataReader["ID"]),
                            ModelId = Convert.ToInt32(dataReader["ModelID"]),
                            cModelName = dataReader["cModelName"].ToString().Trim(),
                            cModelNumber = dataReader["cModelNumber"].ToString().Trim(),
                            cDatabaseName = DatabaseName,
                            cBuildDescription = dataReader["Build"].ToString().Trim(),
                            cLookupDescription = dataReader["Status"].ToString().Trim(),
                            dStatusDate = Convert.ToDateTime(dataReader["StatusDate"]).ToString("MM/dd/yyyy"),
                            cStatus = Convert.ToInt32(dataReader["LK_ModelStatus"]),
                            iIsActive = Convert.ToBoolean(dataReader["iIsActive"]),
                            ModelType = dataReader["ModelType"].ToString().Trim(),
                        });
                    }
                }
                result.Items = modelData;
            }
            return result;
        }
        
        public int GetChildTableNumber(Tuple<string, List<SqlParameter>> query)
        {
            _databaseHelper.EnsureConnectionOpen();
            var nChildTableNumber = 0;
            using (var command = _databaseHelper.CreateCommand(query.Item1, CommandType.Text, query.Item2.ToArray()))
            {
                nChildTableNumber = Convert.ToInt32(command.ExecuteScalar());
            }
            return nChildTableNumber;
        }

        public CreateOrEditModelDto GetModelById(Tuple<string, string, List<SqlParameter>> query)
        {
            _databaseHelper.EnsureConnectionOpen();

            var result = new CreateOrEditModelDto();

            using (var command = _databaseHelper.CreateCommand(query.Item1, CommandType.Text, query.Item3.ToArray()))
            {
                using (var dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        result = new CreateOrEditModelDto
                        {
                            Id = Convert.ToInt32(dataReader["ID"]),
                            ModelSummaryData = new ModelSummaryDto
                            {
                                cModelName = dataReader["cModelName"].ToString().Trim(),
                                DatabaseId = Convert.ToInt32(dataReader["DatabaseID"]),
                                cDescription = dataReader["Model Description"].ToString().Trim(),
                                iIntercept = Convert.ToInt32(dataReader["iIntercept"]),
                                cClientName = dataReader["cClientName"].ToString(),
                                nChildTableNumber = Convert.ToInt32(dataReader["nChildTableNumber"]),
                                cModelNumber = dataReader["cModelNumber"].ToString().Trim(),
                                iIsScoredForEveryBuild = Convert.ToBoolean(dataReader["iIsScoredForEveryBuild"]),
                                iIsActive = Convert.ToBoolean(dataReader["iIsActive"]),
                                cCreatedBy = dataReader["ModelSummaryCreatedBy"].ToString(),
                                dCreatedDate = Convert.ToDateTime(dataReader["ModelSummaryCreatedDate"]),
                                LK_GiftWeight = dataReader["LK_GiftWeight"].ToString(),
                                LK_ModelType = dataReader["LK_ModelType"].ToString()
                            },
                            ModelDetailData = new ModelDetails.Dtos.ModelDetailDto
                            {
                                ModelID = Convert.ToInt32(dataReader["ModelID"]),
                                BuildID = Convert.ToInt32(dataReader["BuildID"]),
                                cSQL_Score = dataReader["cSQL_Score"].ToString(),
                                cSQL_Deciles = dataReader["cSQL_Deciles"].ToString(),
                                cSQL_Preselect = dataReader["cSQL_Preselect"].ToString(),
                                cSAS_ScoreFileName = dataReader["cSAS_ScoreFileName"].ToString(),
                                cCreatedBy = dataReader["ModelDetailCreatedBy"].ToString(),
                                dCreatedDate = Convert.ToDateTime(dataReader["ModelDetailCreatedDate"])
                            }
                        };                      
                    }
                }
            }
            return result;
        }

        public List<ModelStatusDto> GetModelStatusForModelDetail(int modelDetailId)
        {
            _databaseHelper.EnsureConnectionOpen();

            using (var command = _databaseHelper.CreateCommand($@" select MQ.ID, MQ.ModelDetailID, L.cDescription ,ISNULL(U.cFirstName + ' ' + U.cLastName, MQ.cCreatedBy)  as cCreatedby, MQ.dCreatedDate
                                                                    from tblModelQueue MQ Inner join tblLookup L on MQ.LK_ModelStatus = L.cCode AND cLookupValue = 'MODELSTATUS'
                                                                    left join tblUser U on U.cUserID = MQ.cCreatedBy
                                                                    where ModelDetailID = {modelDetailId} order by MQ.ID desc", CommandType.Text))

            {
                var Modelstatus = new List<ModelStatusDto>();
                using (var dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        var Status = new ModelStatusDto
                        {
                            Id = Convert.ToInt32(dataReader["ID"]),
                            ModelDetailID = Convert.ToInt32(dataReader["ModelDetailID"]),
                            iStatus = dataReader["cDescription"].ToString(),
                            cCreatedBy = dataReader["cCreatedBy"].ToString(),
                            dCreatedDate = dataReader["dCreatedDate"].ToString()
                        };

                        Modelstatus.Add(Status);
                    }
                }

                return Modelstatus;
            }
        }
    }
}