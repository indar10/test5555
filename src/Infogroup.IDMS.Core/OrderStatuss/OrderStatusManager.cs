using System;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.UI;

namespace Infogroup.IDMS.OrderStatuss
{
    public class OrderStatusManager : IDMSDomainServiceBase, IOrderStatusManager
    {
        private readonly IRepository<OrderStatus> _orderStatusRepository;
        private readonly IOrderStatusRepository _customOrderStatusRepository;
        public OrderStatusManager(IRepository<OrderStatus> orderStatusRepository, IOrderStatusRepository customOrderStatusRepository)
        {
            _orderStatusRepository = orderStatusRepository;
            _customOrderStatusRepository = customOrderStatusRepository;
        }

        public async Task UpdateOrderStatus(int campaignID, CampaignStatus newStatus, string modifiedBy, string cNotes = "")
        {
            var currentStatus = _customOrderStatusRepository.GetOrderStatus(campaignID);
            if ((CampaignStatus)currentStatus != newStatus)
            {                
                var orderStatus = new OrderStatus
                {
                    OrderID = campaignID,
                    iStatus = Convert.ToInt32(newStatus),
                    iIsCurrent = true,
                    cNotes = string.IsNullOrEmpty(cNotes) ? "": cNotes,
                    cCreatedBy = modifiedBy,
                    dCreatedDate = DateTime.Now,
                    iStopRequested = false
                };
                await _orderStatusRepository.InsertAsync(orderStatus);
                CurrentUnitOfWork.SaveChanges();
            }
        }

        public void UpdateScheduleStatus(int campaignID, int newStatus, string modifiedBy, string time, string date1)
        {
            try
            {
                DateTime date = DateTime.Now.Date;
                if (DateTime.TryParse(date1, out date))
                {
                    TimeSpan tm = DateTime.Now.TimeOfDay;
                    if (TimeSpan.TryParse(time, out tm))
                    {
                        DateTime dt = date.Date;

                        dt = dt.Add(tm);
                        //Check to see if the date has changed..
                        if (!dt.CompareTo(DateTime.Now).Equals(0))
                        {
                            //If yes, then compare it to Current Date Time
                            if (dt.CompareTo(DateTime.Now).Equals(0) || dt.CompareTo(DateTime.Now).Equals(-1))
                            {
                                throw new UserFriendlyException(L("ValidScheduleDateTimeValidation"));


                            }

                            var currentStatus = _customOrderStatusRepository.GetOrderStatus(campaignID);
                            if (currentStatus != newStatus)
                            {
                                var orderStatus = new OrderStatus
                                {
                                    OrderID = campaignID,
                                    iStatus = newStatus,
                                    iIsCurrent = true,
                                    cNotes = string.Empty,
                                    cCreatedBy = modifiedBy,
                                    dCreatedDate = dt,
                                    dModifiedDate = dt,

                                    iStopRequested = false
                                };
                                _orderStatusRepository.Insert(orderStatus);
                                CurrentUnitOfWork.SaveChanges();
                            }
                        }

                    }
                    else
                    {
                        throw new UserFriendlyException(L("ValidDateTimeFormatValidation"));


                    }
                }
                else
                {
                    throw new UserFriendlyException(L("dateError"));
                }
                
            }
            catch(Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }



        }
    }
}