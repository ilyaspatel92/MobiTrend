namespace Mobi.Web.Models.APIModels
{
    public class EmployeeAPIModel
    {
        public int EmpId { get; set; }
        public int CompanyId { get; set; }
        public string QrCode { get; set; }
        public string Password { get; set; }
        public int PictureId { get; set; }
    }
}
