
using System;
using Abp.Application.Services.Dto;

namespace Infogroup.IDMS.UserDatabaseMailers.Dtos
{
    public class UserDatabaseMailerDto : EntityDto
    {

        public virtual int Count { get; set; }
        public virtual GetUserDatabaseMailerForViewDto input { get; set; }
        public int DatabaseId { get; set; }
        public bool isDivisionalMailerBroker { get; set; }

    }
}