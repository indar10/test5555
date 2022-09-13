using Abp.Application.Services.Dto;
using Abp.Data;
using Abp.EntityFrameworkCore;
using Abp.UI;
using Infogroup.IDMS.EntityFrameworkCore;
using Infogroup.IDMS.EntityFrameworkCore.Repositories;
using Infogroup.IDMS.Segments.Dtos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading.Tasks;


namespace Infogroup.IDMS.Segments
{
    public class SegmentRepository : IDMSRepositoryBase<Segment, int>, ISegmentRepository
    {
        private readonly IActiveTransactionProvider _transactionProvider;
        private readonly DatabaseHelper.DatabaseHelper _databaseHelper;
        public const string sNoLock = " WITH (NOLOCK) ";
        public SegmentRepository(IDbContextProvider<IDMSDbContext> dbContextProvider, IActiveTransactionProvider transactionProvider, DatabaseHelper.DatabaseHelper databaseHelper)
            : base(dbContextProvider)

        {
            _transactionProvider = transactionProvider;
            _databaseHelper = databaseHelper;
        }

        public async Task<PagedResultDto<GetSegmentListForView>> GetAllSegmentsList(GetSegmentListInput input, string selectQuery, string countQuery, List<SqlParameter> sqlParams)
        {
            try
            {
                _databaseHelper.EnsureConnectionOpen();
                var result = new PagedResultDto<GetSegmentListForView>();
                using (var command = _databaseHelper.CreateCommand(countQuery, CommandType.Text, sqlParams.ToArray()))
                {
                    result.TotalCount = Convert.ToInt32(command.ExecuteScalar());
                    command.Parameters.Clear();
                }
                using (var command = _databaseHelper.CreateCommand(selectQuery, CommandType.Text, sqlParams.ToArray()))
                {
                    var items = new List<GetSegmentListForView>();
                    using (var dataReader = await command.ExecuteReaderAsync())
                    {
                        var iGroupOrdinal = dataReader.GetOrdinal("iGroup");
                        var iOutputQtyOrdinal = dataReader.GetOrdinal("iOutputQty");
                        var iAvailableQtyOrdinal = dataReader.GetOrdinal("iAvailableQty");
                        while (dataReader.Read())
                        {
                            var segmentDto = new GetSegmentListForView
                            {
                                Id = Convert.ToInt32(dataReader["ID"]),
                                OrderId = Convert.ToInt32(dataReader["OrderID"]),
                                Description = dataReader["cDescription"].ToString(),
                                cMaxPerGroup = dataReader["cMaxPerGroup"].ToString().Equals("00") ? string.Empty : dataReader["cMaxPerGroup"].ToString(),
                                iProvidedQty = Convert.ToInt32(dataReader["iProvidedQty"]),
                                iRequiredQty = Convert.ToInt32(dataReader["iRequiredQty"]),
                                cKeyCode1 = dataReader["cKeyCode1"].ToString(),
                                cKeyCode2 = dataReader["cKeyCode2"].ToString(),
                                iDedupeOrderSpecified = Convert.ToInt32(dataReader["iDedupeOrderSpecified"]),
                                iGroup = dataReader.IsDBNull(iGroupOrdinal) ? 1 : dataReader.GetInt32(iGroupOrdinal),
                                iOutputQty = dataReader.IsDBNull(iOutputQtyOrdinal) ? -1 : dataReader.GetInt32(iOutputQtyOrdinal),
                                iAvailableQty = dataReader.IsDBNull(iAvailableQtyOrdinal) ? default : dataReader.GetInt32(iAvailableQtyOrdinal)
                            };
                            if (segmentDto.iOutputQty == -1)
                                segmentDto.iOutputQty = segmentDto.iProvidedQty;
                            items.Add(segmentDto);
                        }
                    }
                    result.Items = items;
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int GetSegmentListCount(int segmentId)
        {
            _databaseHelper.EnsureConnectionOpen();
            int result = 0;
            using (var command = _databaseHelper.CreateCommand(@"Select count(*) from tblSegmentList WITH (NOLOCK) where SegmentID = " + segmentId, CommandType.Text))
            {
                command.CommandTimeout = 3 * 60;
                result = Convert.ToInt32(command.ExecuteScalar());
            }
            return result;
        }
        public bool IsOrderInStatusToUpdate(int iSegmentID)
        {
            var result = 0;
            var SQL = @" SELECT CASE WHEN (tblOrderStatus.iStatus <40) THEN 1 ELSE 0 END   FROM tblSegment WITH (NOLOCK)
                            INNER JOIN tblOrderStatus WITH (NOLOCK) ON tblOrderStatus.OrderID = tblSegment.OrderID
                            AND tblOrderStatus.iIsCurrent = 1 AND tblSegment.ID = " + iSegmentID;

            using (var command = _databaseHelper.CreateCommand(SQL, CommandType.Text))
            {
                command.CommandTimeout = 3 * 60;
                result = Convert.ToInt32(command.ExecuteScalar());
            }
            return int.Parse(result.ToString()) > 0 ? true : false;
        }
        public async Task DeleteSegmentAsync(int iSegmentId)
        {
            _databaseHelper.EnsureConnectionOpen();

            var sqlParameters = new List<SqlParameter>();

            sqlParameters.Add(new SqlParameter("@SegmentID", iSegmentId));

            using (var command = _databaseHelper.CreateCommand("usp_DeleteSegment", CommandType.StoredProcedure, sqlParameters.ToArray()))
            {
                await command.ExecuteNonQueryAsync();
            }

        }

        public async Task<int> CopySegmentAsync(CopySegmentDto copySegmentInfo)
        {
            _databaseHelper.EnsureConnectionOpen();
            var sqlParameters = new List<SqlParameter>();
            var prmCopyToID = new SqlParameter("@ToOrderID", copySegmentInfo.iCopyToOrderID);
            var prmCopyFromID = new SqlParameter("@FromOrderID", copySegmentInfo.iCopyFromOrderID);
            var prmInitiatedBy = new SqlParameter("@InitiatedBy", copySegmentInfo.sInitiatedBy);
            var prmSegmentHashList = new SqlParameter("@SegmentHashList", copySegmentInfo.sCommaSeparatedSegments);
            var prmCount = new SqlParameter("@NoOfSegmentsAdded", DbType.Int32)
            {
                Direction = ParameterDirection.Output,
                Value = 0
            };
            var prmSegmentDesc = new SqlParameter("@SegmentDesc", DBNull.Value);
            var prmKeyCode1 = new SqlParameter("@KeyCode1", DBNull.Value);
            var prmKeyCode2 = new SqlParameter("@KeyCode2", DBNull.Value);
            var prmMaxPerGroup = new SqlParameter("@MaxPerGroup", DBNull.Value);
            var prmiGroup = new SqlParameter("@iGroup", DBNull.Value);
            var prmiRequiredQty = new SqlParameter("@iRequiredQty", DBNull.Value);

            if (!string.IsNullOrEmpty(copySegmentInfo.cSegmentDescription))
            {
                prmSegmentDesc.Value = copySegmentInfo.cSegmentDescription;
            }
            if (!string.IsNullOrEmpty(copySegmentInfo.cKeyCode1))
            {
                prmKeyCode1.Value = copySegmentInfo.cKeyCode1;
            }
            if (!string.IsNullOrEmpty(copySegmentInfo.cKeyCode2))
            {
                prmKeyCode2.Value = copySegmentInfo.cKeyCode2;
            }
            if (!string.IsNullOrEmpty(copySegmentInfo.cmaxPer))
            {
                prmMaxPerGroup.Value = copySegmentInfo.cmaxPer;
            }
            if (copySegmentInfo.iGroup.HasValue)
            {
                prmiGroup.Value = copySegmentInfo.iGroup;
            }
            if (copySegmentInfo.iRequiredQty.HasValue)
            {
                prmiRequiredQty.Value = copySegmentInfo.iRequiredQty;
            }
            sqlParameters.Add(prmCopyToID);
            sqlParameters.Add(prmCopyFromID);
            sqlParameters.Add(prmInitiatedBy);
            sqlParameters.Add(prmSegmentHashList);
            sqlParameters.Add(prmCount);
            sqlParameters.Add(prmSegmentDesc);
            sqlParameters.Add(prmKeyCode1);
            sqlParameters.Add(prmKeyCode2);
            sqlParameters.Add(prmMaxPerGroup);
            sqlParameters.Add(prmiRequiredQty);
            sqlParameters.Add(prmiGroup);

            using (var command = _databaseHelper.CreateCommand("usp_CopySegmentsOtherOrder", CommandType.StoredProcedure, sqlParameters.ToArray()))
            {
                var segmentId = await command.ExecuteScalarAsync();
                return Convert.ToInt32(segmentId);
            }
        }

        public async Task<ImportSegmentDTO> ImportSegmentValidationAsync(int iCopyToOrderID, int iCopyFromOrderID, int iUserID)
        {
            _databaseHelper.EnsureConnectionOpen();

            var sqlParameters = new List<SqlParameter>();
            var importSegment = new ImportSegmentDTO();

            var prmCopyToID = new SqlParameter("@ToID", iCopyToOrderID);
            var prmCopyFromID = new SqlParameter("@FromID", iCopyFromOrderID);
            var prmInitiatedBy = new SqlParameter("@UserID", iUserID);
            var prmIsValid = new SqlParameter("@IsValid", DbType.Boolean)
            {
                Direction = ParameterDirection.InputOutput,
                Value = false
            };
            var prmSegmentDesc = new SqlParameter("@Description", SqlDbType.VarChar, 500)
            {
                Direction = ParameterDirection.InputOutput,
                Value = string.Empty
            };
            var prmCount = new SqlParameter("@NoOfSegments", DbType.Int32)
            {
                Direction = ParameterDirection.InputOutput,
                Value = 0
            };

            sqlParameters.Add(prmCopyToID);
            sqlParameters.Add(prmCopyFromID);
            sqlParameters.Add(prmInitiatedBy);
            sqlParameters.Add(prmIsValid);
            sqlParameters.Add(prmSegmentDesc);
            sqlParameters.Add(prmCount);

            using (var command = _databaseHelper.CreateCommand("usp_ValidateCopyOrderID", CommandType.StoredProcedure, sqlParameters.ToArray()))
            {
                await command.ExecuteNonQueryAsync();
                importSegment.NumberOfSegments = Convert.ToInt32(command.Parameters["@NoOfSegments"].Value);
                importSegment.cSegmentDescription = (command.Parameters["@Description"].Value).ToString();
                importSegment.isValid = Convert.ToBoolean(command.Parameters["@IsValid"].Value);
                importSegment.sCommaSeparatedSegments = string.Empty;
            }
            return importSegment;
        }

        public async Task<int> CopySegmentFromCampaign(int iCopyToOrderID, int iCopyFromOrderID, string sCommanSeparatedSegments, string sInitiatedBy)
        {
            _databaseHelper.EnsureConnectionOpen();

            var noOfSegmentAdded = 0;
            var sqlParameters = new List<SqlParameter>();

            var prmCopyToID = new SqlParameter("@ToOrderID", iCopyToOrderID);
            var prmCopyFromID = new SqlParameter("@FromOrderID", iCopyFromOrderID);
            var prmInitiatedBy = new SqlParameter("@InitiatedBy", sInitiatedBy);
            var prmSegmentHashList = new SqlParameter("@SegmentHashList", sCommanSeparatedSegments);
            var prmCount = new SqlParameter("@NoOfSegmentsAdded", DbType.Int32)
            {
                Direction = ParameterDirection.InputOutput,
                Value = 0
            };

            sqlParameters.Add(prmCopyToID);
            sqlParameters.Add(prmCopyFromID);
            sqlParameters.Add(prmInitiatedBy);
            sqlParameters.Add(prmSegmentHashList);
            sqlParameters.Add(prmCount);

            using (var command = _databaseHelper.CreateCommand("usp_CopySegmentsOtherOrder", CommandType.StoredProcedure, sqlParameters.ToArray()))
            {
                await command.ExecuteNonQueryAsync();
                noOfSegmentAdded = Convert.ToInt32(command.Parameters["@NoOfSegmentsAdded"].Value);
            }
            return noOfSegmentAdded;
        }

        public void MoveSegment(int segmentId, int fromSegment, int toSegment, int toLocation, string initiatedBy)
        {
            _databaseHelper.EnsureConnectionOpen();

            var sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(new SqlParameter("@SegmentID", segmentId));
            sqlParameters.Add(new SqlParameter("@RangeFrom", fromSegment));
            sqlParameters.Add(new SqlParameter("@RangeTo", toSegment));
            sqlParameters.Add(new SqlParameter("@MoveTo", toLocation));
            sqlParameters.Add(new SqlParameter("@InitiatedBy", initiatedBy));

            using (var command = _databaseHelper.CreateCommand("usp_MoveSegment1", CommandType.StoredProcedure, sqlParameters.ToArray()))
            {
                command.ExecuteNonQuery();
            }

        }
        public async Task<PagedResultDto<SegmentsGlobalChangesDto>> GetAllSegmentsForGlobalChanges(Tuple<string, string, List<SqlParameter>> query)
        {
            try
            {
                _databaseHelper.EnsureConnectionOpen();
                var result = new PagedResultDto<SegmentsGlobalChangesDto>();
                using (var command = _databaseHelper.CreateCommand(query.Item2, CommandType.Text, query.Item3.ToArray()))
                {
                    result.TotalCount = Convert.ToInt32(command.ExecuteScalar());
                    command.Parameters.Clear();
                }
                using (var command = _databaseHelper.CreateCommand(query.Item1, CommandType.Text, query.Item3.ToArray()))
                {
                    var items = new List<SegmentsGlobalChangesDto>();
                    using (var dataReader = await command.ExecuteReaderAsync())
                    {
                        var iAvailableQtyOrdinal = dataReader.GetOrdinal("iAvailableQty");
                        while (dataReader.Read())
                        {
                            items.Add(new SegmentsGlobalChangesDto
                            {
                                Id = Convert.ToInt32(dataReader["ID"]),
                                cDescription = (dataReader["cDescription"]).ToString(),
                                iProvidedQty = Convert.ToInt32(dataReader["iProvidedQty"]),
                                iRequiredQty = Convert.ToInt32(dataReader["iRequiredQty"]),
                                iAvailableQty = dataReader.IsDBNull(iAvailableQtyOrdinal) ? default : dataReader.GetInt32(iAvailableQtyOrdinal),
                                cKeyCode1 = (dataReader["cKeyCode1"]).ToString(),
                                iDedupeOrderSpecified = Convert.ToInt32(dataReader["iDedupeOrderSpecified"])
                            });
                        }
                    }
                    result.Items = items;
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<BatchEditSegmentDto> GetAllSegmentsForBatchEdit(Tuple<string, string, List<SqlParameter>> query)
        {
            try
            {
                _databaseHelper.EnsureConnectionOpen();
                var result = new List<BatchEditSegmentDto>();
                var index = 0;
                using (var command = _databaseHelper.CreateCommand(query.Item1, CommandType.Text, query.Item3.ToArray()))
                {
                    using (var dataReader = command.ExecuteReader())
                    {
                        var iUseAutosuppressOrdinal = dataReader.GetOrdinal("iUseAutosuppress");
                        var iGroupOrdinal = dataReader.GetOrdinal("iGroup");
                        var iOutputQtyOrdinal = dataReader.GetOrdinal("iOutputQty");
                        var iAvailableQtyOrdinal = dataReader.GetOrdinal("iAvailableQty");
                        var iIsRandomRadiusNthOrdinal = dataReader.GetOrdinal("iIsRandomRadiusNth");
                        var dModifiedDateOrdinal = dataReader.GetOrdinal("dModifiedDate");
                        var cModifiedByOrdinal = dataReader.GetOrdinal("cModifiedBy");
                        while (dataReader.Read())
                        {
                            var batchEditSegmentDto = new BatchEditSegmentDto
                            {
                                // Not Null Columns
                                Id = Convert.ToInt32(dataReader["ID"]),
                                iDedupeOrderSpecified = Convert.ToInt32(dataReader["iDedupeOrderSpecified"]),
                                cDescription = dataReader["cDescription"].ToString(),
                                iRequiredQty = Convert.ToInt32(dataReader["iRequiredQty"]),
                                cKeyCode1 = dataReader["cKeyCode1"].ToString(),
                                cKeyCode2 = dataReader["cKeyCode2"].ToString(),
                                cMaxPerGroup = dataReader["cMaxPerGroup"].ToString(),
                                OrderId = Convert.ToInt32(dataReader["OrderId"]),
                                cFixedTitle = dataReader["cFixedTitle"].ToString(),
                                cTitleSuppression = dataReader["cTitleSuppression"].ToString(),
                                iDedupeOrderUsed = Convert.ToInt32(dataReader["iDedupeOrderUsed"]),
                                iProvidedQty = Convert.ToInt32(dataReader["iProvidedQty"]),
                                iIsOrderLevel = Convert.ToBoolean(dataReader["iIsOrderLevel"]),
                                iDoubleMultiBuyerCount = Convert.ToInt32(dataReader["iDoubleMultiBuyerCount"]),
                                cCreatedBy = dataReader["cCreatedBy"].ToString(),
                                dCreatedDate = Convert.ToDateTime(dataReader["dCreatedDate"]),
                                // Nullable Columns 
                                iUseAutosuppress = !dataReader.IsDBNull(iUseAutosuppressOrdinal) && dataReader.GetBoolean(iUseAutosuppressOrdinal),
                                iGroup = dataReader.IsDBNull(iGroupOrdinal) ? default(int?) : dataReader.GetInt32(iGroupOrdinal),
                                iOutputQty = dataReader.IsDBNull(iOutputQtyOrdinal) ? default(int?) : dataReader.GetInt32(iOutputQtyOrdinal),
                                iAvailableQty = dataReader.IsDBNull(iAvailableQtyOrdinal) ? default(int?) : dataReader.GetInt32(iAvailableQtyOrdinal),
                                iIsRandomRadiusNth = !dataReader.IsDBNull(iIsRandomRadiusNthOrdinal) && dataReader.GetBoolean(iIsRandomRadiusNthOrdinal),
                                cModifiedBy = dataReader.IsDBNull(cModifiedByOrdinal) ? default : dataReader.GetString(cModifiedByOrdinal),
                                dModifiedDate = dataReader.IsDBNull(dModifiedDateOrdinal) ? default(DateTime?) : dataReader.GetDateTime(dModifiedDateOrdinal),
                                // Custom columns
                                Index = index++,
                                Dirty = false,
                                NextStatus = 1000
                            };
                            batchEditSegmentDto.iDisplayOutputQty = batchEditSegmentDto.iOutputQty == -1 && batchEditSegmentDto.iProvidedQty >= 0
                                ? batchEditSegmentDto.iProvidedQty
                                : batchEditSegmentDto.iOutputQty.Value;
                            result.Add(batchEditSegmentDto);
                        }
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<List<int>> GetAllSegmentIDsForGlobalChanges(Tuple<string, string, List<SqlParameter>> query)
        {
            try
            {
                _databaseHelper.EnsureConnectionOpen();
                var result = new List<int>();
                using (var command = _databaseHelper.CreateCommand(query.Item1, CommandType.Text, query.Item3.ToArray()))
                {
                    using (var dataReader = await command.ExecuteReaderAsync())
                    {
                        while (dataReader.Read())
                        {
                            result.Add(Convert.ToInt32(dataReader["ID"]));
                        }
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<GetSegmentListForView> GetSegmentsForViewById(Tuple<string, List<SqlParameter>> query)
        {
            try
            {
                _databaseHelper.EnsureConnectionOpen();
                GetSegmentListForView result = null;
                using (var command = _databaseHelper.CreateCommand(query.Item1, CommandType.Text, query.Item2.ToArray()))
                {
                    var items = new List<GetSegmentListForView>();
                    using (var dataReader = await command.ExecuteReaderAsync())
                    {
                        var iGroupOrdinal = dataReader.GetOrdinal("iGroup");
                        var iOutputQtyOrdinal = dataReader.GetOrdinal("iOutputQty");
                        var iAvailableQtyOrdinal = dataReader.GetOrdinal("iAvailableQty");
                        while (dataReader.Read())
                        {
                            result = new GetSegmentListForView
                            {
                                Id = Convert.ToInt32(dataReader["ID"]),
                                OrderId = Convert.ToInt32(dataReader["OrderID"]),
                                Description = dataReader["cDescription"].ToString(),
                                cMaxPerGroup = dataReader["cMaxPerGroup"].ToString().Equals("00") ? string.Empty : dataReader["cMaxPerGroup"].ToString(),
                                iProvidedQty = Convert.ToInt32(dataReader["iProvidedQty"]),
                                iRequiredQty = Convert.ToInt32(dataReader["iRequiredQty"]),
                                cKeyCode1 = dataReader["cKeyCode1"].ToString(),
                                cKeyCode2 = dataReader["cKeyCode2"].ToString(),
                                iDedupeOrderSpecified = Convert.ToInt32(dataReader["iDedupeOrderSpecified"]),
                                iGroup = dataReader.IsDBNull(iGroupOrdinal) ? 1 : dataReader.GetInt32(iGroupOrdinal),
                                iOutputQty = dataReader.IsDBNull(iOutputQtyOrdinal) ? -1 : dataReader.GetInt32(iOutputQtyOrdinal),
                                iAvailableQty = dataReader.IsDBNull(iAvailableQtyOrdinal) ? default : dataReader.GetInt32(iAvailableQtyOrdinal)
                            };
                            if (result.iOutputQty == -1)
                                result.iOutputQty = result.iProvidedQty;
                        }
                    }
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
