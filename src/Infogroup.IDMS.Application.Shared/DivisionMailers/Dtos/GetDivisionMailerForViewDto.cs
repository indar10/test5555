namespace Infogroup.IDMS.DivisionMailers.Dtos
{
    public class GetDivisionMailerForViewDto
    {
        public string DivisionName { get; set; }
        
        public int Id { get; set; }
        public string Code { get; set; }
        public string Company { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string cAddr1 { get; set; }
        public string cAddr2 { get; set; }
        public string cCity { get; set; }
        public string cState { get; set; }
        public string cPhone { get; set; }
        public string cZip { get; set; }
        public string cFax { get; set; }
        public string Email { get; set; }
        public string Notes { get; set; }
        public bool IsActive { get; set; }

    }
}