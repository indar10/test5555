using Abp.Data;
using Abp.EntityFrameworkCore;
using Infogroup.IDMS.CampaignCASApprovals.Dtos;
using Infogroup.IDMS.EntityFrameworkCore;
using Infogroup.IDMS.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infogroup.IDMS.CampaignCASApprovals
{
    public class CampaignCASApprovalRepository : IDMSRepositoryBase<CampaignCASApproval, int>, ICampaignCASApprovalRepository
    {
        private readonly IActiveTransactionProvider _transactionProvider;
        private readonly DatabaseHelper.DatabaseHelper _databaseHelper;
        public const string sNoLock = " WITH (NOLOCK) ";
        public CampaignCASApprovalRepository(IDbContextProvider<IDMSDbContext> dbContextProvider, IActiveTransactionProvider transactionProvider, DatabaseHelper.DatabaseHelper databaseHelper)
            : base(dbContextProvider)
        {
            _transactionProvider = transactionProvider;
            _databaseHelper = databaseHelper;
        }

        public async Task<List<CampaignCASApprovalDto>> getCASApprovalList(int? iOfferID, int BuildID)
        {
            var result = new List<CampaignCASApprovalDto>();
            _databaseHelper.EnsureConnectionOpen();
            using (var command = _databaseHelper.CreateCommand($@"
                                                Select nBasePrice,tblCASApproval.MasterLOLID from tblCASApproval
                                                inner join tblBuildLol on tblCASApproval.MasterLOLID=tblBuildLol.MasterLoLID 
                                                and tblBuildLol.BuildID={BuildID}
                                                and tblBuildLol.LK_Action in ('N','R','O','A') 
                                                WHERE cStatus = 'A' and OfferID = {iOfferID}
                                                UNION
                                                Select case when tblOffer.LK_OfferType='P' then LIST.nBasePrice_Postal 
                                                when tblOffer.LK_OfferType='T' then LIST.nBasePrice_Telemarketing end as nBasePrice,
                                                tblBuildLol.MasterLolID From dbo.tblMasterLol as LIST 
                                                INNER JOIN tblBuildLol ON tblBuildLol.MasterLoLID=LIST.ID
                                                AND tblBuildLol.BuildID = { BuildID } and tblBuildLol.LK_Action in ('N', 'R', 'O', 'A')
                                                inner join tblListMailer ON tblListMailer.ListID = tblBuildLol.MasterLoLID
                                                INNER JOIN tblOffer on tblOffer.MailerID = tblLISTMailer.MailerID AND tblOffer.ID ={iOfferID}",
                                            CommandType.Text))

            {
                command.CommandTimeout = 3 * 60;
                using (var dataReader = await command.ExecuteReaderAsync())
                {
                    while (dataReader.Read())
                    {
                        result.Add(new CampaignCASApprovalDto
                        {
                            MasterLOLID = Convert.ToInt32(dataReader["MasterLOLID"]),
                            nBasePrice = Convert.ToDecimal(dataReader["nBasePrice"])
                        });
                    }
                }
            }
            return new List<CampaignCASApprovalDto>(result.ToList());
        }

    }
}
