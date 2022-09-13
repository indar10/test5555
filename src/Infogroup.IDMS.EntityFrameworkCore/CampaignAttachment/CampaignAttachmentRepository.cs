using Abp.Data;
using Abp.EntityFrameworkCore;
using Infogroup.IDMS.EntityFrameworkCore;
using Infogroup.IDMS.EntityFrameworkCore.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using Infogroup.IDMS.CampaignAttachments.Dtos;

namespace Infogroup.IDMS.CampaignAttachments
{
    public class CampaignAttachmentRepository : IDMSRepositoryBase<CampaignAttachment, int>, ICampaignAttachmentRepository
    {
        private readonly IActiveTransactionProvider _transactionProvider;
        private readonly DatabaseHelper.DatabaseHelper _databaseHelper;
        public const string sNoLock = " WITH (NOLOCK) ";
        public CampaignAttachmentRepository(IDbContextProvider<IDMSDbContext> dbContextProvider, IActiveTransactionProvider transactionProvider, DatabaseHelper.DatabaseHelper databaseHelper)
           : base(dbContextProvider)
        {
            _transactionProvider = transactionProvider;
            _databaseHelper = databaseHelper;
        }
        public List<CampaignAttachmentDto> GetAttachmentsByID(int campaignId)
        {
            _databaseHelper.EnsureConnectionOpen();
            try
            {
                var attachments = new List<CampaignAttachmentDto>();
                using (var command = _databaseHelper.CreateCommand($@"
                                Select distinct L.cDescription as FormType,L.cCode as Code ,
                                OA.OrderID, OA.cFileName as cFileName ,
                                OA.cRealFileName as RealFileName,ISNULl(OA.ID,0) AS ID 
                                from tblLookup L WITH(NOLOCK) 
                                inner Join tblOrderAttachment OA WITH(NOLOCK) ON OA.LK_AttachmentType = L.cCode
                                WHERE L.cLookupValue='ORDERATTACHMENTTYPE' AND OA.OrderID= { campaignId }", CommandType.Text))

                {
                    command.CommandTimeout = 3 * 60;
                    using (var dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            attachments.Add(
                                new CampaignAttachmentDto
                                {
                                    FormType = dataReader["FormType"].ToString(),
                                    Code = dataReader["Code"].ToString(),
                                    OrderId = dataReader["OrderID"].ToString(),
                                    ID = Convert.ToInt32(dataReader["ID"]),
                                    cFileName = dataReader["cFileName"].ToString(),
                                    RealFileName = dataReader["RealFileName"].ToString(),
                                    IsDisabled = false
                                });
                        }
                    }
                }

                return attachments;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<CampaignAttachmentDto> getActiveItemByOrderID(int campaignId)
        {
            _databaseHelper.EnsureConnectionOpen();
            try
            {
                var attachments = new List<CampaignAttachmentDto>();
                using (var command = _databaseHelper.CreateCommand(@"Select distinct L.cDescription as FormType,L.cCode as Code ,OA.OrderID, OA.cFileName as cFileName ,OA.cRealFileName as RealFileName,ISNULl(OA.ID,0) AS ID from tblLookup L WITH(NOLOCK) inner Join tblOrderAttachment OA WITH(NOLOCK) ON OA.LK_AttachmentType = L.cCode
                    WHERE L.cLookupValue='ORDERATTACHMENTTYPE'AND (OA.OrderID= " + campaignId + " )", CommandType.Text))

                {
                    command.CommandTimeout = 3 * 60;
                    using (var dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            attachments.Add(
                                new CampaignAttachmentDto
                                {
                                    FormType = dataReader["FormType"].ToString(),
                                    Code = dataReader["Code"].ToString(),
                                    OrderId = dataReader["OrderID"].ToString(),
                                    ID = Convert.ToInt32(dataReader["ID"]),
                                    cFileName = dataReader["cFileName"].ToString(),
                                    RealFileName = dataReader["RealFileName"].ToString()
                                });
                        }
                    }
                }

                return attachments;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}
