using Infogroup.IDMS.Shared.Dtos;
using System.Collections.Generic;

namespace Infogroup.IDMS.BuildTableLayouts.Dtos
{
    public class Field
    {
        public int ID { get; set; }
        public string cQuestionFieldName { get; set; }
        public string cConfigFieldName { get; set; }
        public string cQuestionDescription { get; set; }
        public string cTableName { get; set; }
        public string cValueMode { get; set; }
        public List<DropdownOutputDto> Values { get; set; }

    }
}