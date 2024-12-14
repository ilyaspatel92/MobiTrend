namespace Mobi.Web.Models.APIModels
{
    public class EmployeeAPIModel
    {
        public string NameEng { get; set; }
        public string NameArabic { get; set; }
        public bool Status { get; set; }
        public string CompanyId { get; set; }
        public string FileNumber { get; set; }
        public string MobileNumber { get; set; }
        public string Email { get; set; }
        public string PhotoPath { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
        public int MobileType { get; set; }
        public int RegistrationType { get; set; }
        public string DeviceId { get; set; }
        public bool RegisterStatus { get; set; }
        public int CID { get; set; }
        public bool IsQrVerify { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? MobRegistrationDate { get; set; }
    }
}
