using Infogroup.IDMS.Shared.Dtos;
using System.Collections.Generic;

namespace Infogroup.IDMS.Models.Dtos
{
    public class GetModelTypeAndWeightDto
    {
        public List<DropdownOutputDto> ModelType { get; set; }
        public List<DropdownOutputDto> ModelGiftWeight { get; set; }
    }
}
