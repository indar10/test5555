using System;
using Abp.Notifications;
using Infogroup.IDMS.Dto;

namespace Infogroup.IDMS.Notifications.Dto
{
    public class GetUserNotificationsInput : PagedInputDto
    {
        public UserNotificationState? State { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}