using Abp.Data;
using Abp.EntityFrameworkCore;
using Infogroup.IDMS.CampaignMaxPers;
using Infogroup.IDMS.CampaignMaxPers.Dtos;
using Infogroup.IDMS.EntityFrameworkCore;
using Infogroup.IDMS.EntityFrameworkCore.Repositories;
using Infogroup.IDMS.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Data;


namespace Infogroup.IDMS.CampaignMaxPers
{
    public class CampaignMaxPersRepository: IDMSRepositoryBase<CampaignMaxPer, int>, ICampaignMaxPersRepository
    {
        private readonly IActiveTransactionProvider _transactionProvider;
        private readonly DatabaseHelper.DatabaseHelper _databaseHelper;
        public const string sNoLock = " WITH (NOLOCK) ";
        public CampaignMaxPersRepository(IDbContextProvider<IDMSDbContext> dbContextProvider, IActiveTransactionProvider transactionProvider, DatabaseHelper.DatabaseHelper databaseHelper)
           : base(dbContextProvider)
        {
            _transactionProvider = transactionProvider;
            _databaseHelper = databaseHelper;
        }
        public int GetBuildLolByCampaignId(int iCampaignId)
        {
            _databaseHelper.EnsureConnectionOpen();
            var result = 0;
            try
            {
                var lcSQL = $@"Select top 1  tblBuildLoL.ID as BuildLolID  from tblBuildLoL inner join tblSegmentList on tblSegmentList.MasterLOLID = tblBuildLoL.MasterLOLID
                             inner join tblSegment on tblSegmentList.SegmentID = tblSegment.ID inner join tblOrder on tblOrder.ID = tblSegment.OrderID
                             and tblOrder.BuildID = tblBuildLoL.BuildID where SegmentID = (Select min(id) from tblSegment where OrderID = {iCampaignId})";


                using (var command = _databaseHelper.CreateCommand(lcSQL, CommandType.Text))
                {
                    command.CommandTimeout = 3 * 60;
                    result = Convert.ToInt32(command.ExecuteScalar());
                }
                return result;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        public List<string> GetGroupsWithNotSegments(int iCampaignID)
        {
            _databaseHelper.EnsureConnectionOpen();
            var listOfDescription = new List<string>();
            try
            {
                using (var command = _databaseHelper.CreateCommand($@"Select cDescription from tblOrderMaxPer
                            INNER JOIN tblLookup on  tblOrderMaxPer.cGroup = tblLookup.cCode and cLookupValue='MAXPER' 
                            WHERE NOT EXISTS( Select ID from tblSegment where tblOrderMaxPer.OrderID = tblSegment.OrderID and cMaxPerGroup=cGroup)
                            and tblOrderMaxPer.iMaxPerQuantity > 0 and LEN(RTRIM(LTRIM(tblOrderMaxPer.cMaxPerField))) > 0  and OrderID = {iCampaignID}
                            Order By iOrderBy", CommandType.Text))
                {
                    command.CommandTimeout = 3 * 60;
                    using (var dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            listOfDescription.Add(dataReader["cDescription"].ToString());
                        };
                    }
                }
                return listOfDescription;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public List<string> GetUnDefinedGroupsForSegments(int iCampaignID)
        {
            _databaseHelper.EnsureConnectionOpen();
            var listOfDescription = new List<string>();

            try
            {
                using (var command = _databaseHelper.CreateCommand($@"
                          Select DISTINCT tblLookup.cDescription,iOrderBy  from tblSegment INNER JOIN tblLookup 
                          on  tblSegment.cMaxPerGroup = tblLookup.cCode and cLookupValue='MAXPER' 
                          where NOT EXISTS (Select ID from tblOrderMaxPer where tblOrderMaxPer.OrderID =tblSegment.OrderID
                          and cMaxPerGroup=cGroup and tblOrderMaxPer.iMaxPerQuantity > 0 
                          and LEN(RTRIM(LTRIM(tblOrderMaxPer.cMaxPerField))) > 0)
                          and OrderID = {iCampaignID} and iIsOrderLevel =  0  
                          and tblSegment.cMaxPerGroup <> '00' Order By iOrderBy", CommandType.Text))
                {
                    command.CommandTimeout = 3 * 60;
                    using (var dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            listOfDescription.Add(dataReader["cDescription"].ToString());
                        };
                    }
                }
                return listOfDescription;
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        public List<DropdownOutputDto> GetMaxPerFieldsDropdown(int buildId)
        {
            _databaseHelper.EnsureConnectionOpen();
            try
            {
                var maxPerData = new List<DropdownOutputDto>();
                using (var command = _databaseHelper.CreateCommand
                    ($@"select UPPER(BTL.cFieldName) as cfieldname, BTL.cFieldDescription from tblBuildTableLayout BTL
                 inner join tblBuildTable BT ON BT.ID = BTL.BuildTableID AND BTL.iAllowMaxPer = 1 Where
                 BT.BuildID = { buildId } AND BT.LK_TableType = 'M' Order by
                 BTL.cFieldDescription", CommandType.Text))
                {
                    using (var dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            maxPerData.Add(
                                new DropdownOutputDto
                                {
                                    Value = dataReader["cfieldname"],
                                    Label = dataReader["cFieldDescription"].ToString()
                                });
                        }
                    }
                }
                return maxPerData;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public int GetOrderListCount(int iCampaignId)
        {
            var result = 0;
            var lcSQL = $@" Select count(*) from tblSegmentList where SegmentID = (Select min(id) from tblSegment where OrderID = {iCampaignId})";
            using (var command = _databaseHelper.CreateCommand(lcSQL, CommandType.Text))
            {
                command.CommandTimeout = 3 * 60;
                result = Convert.ToInt32(command.ExecuteScalar());
            }
            return result;
        }
    }
}
