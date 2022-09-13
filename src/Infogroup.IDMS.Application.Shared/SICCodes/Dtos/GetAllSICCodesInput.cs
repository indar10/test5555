namespace Infogroup.IDMS.SICCodes.Dtos
{
    public class GetAllSICCodesInput { 
		public string Filter { get; set; }
        public string cType { get; set; }
        public bool IsSortyBySICCode { get; set; }
    }
    public class ValidateSICCodesInputDto
    {
        public string SearchText { get; set; }
    }
}