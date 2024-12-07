namespace Mobi.Data.Domain.Employees
{
    public class Employee : BaseEntity
    {
        public int Id { get; set; }
        public string NameEng { get; set; }
        public string? NameArabic { get; set; }
        public bool Status { get; set; }
        public int CompanyId { get; set; }
        public string? FileNumber { get; set; }
        public string? MobileNumber { get; set; }
        public string? Email { get; set; }
        public string? PhotoPath { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
        public int? MobileType { get; set; } 
        public string? RegistrationVia { get; set; }
        public string? DeviceId { get; set; }
        public string? RegisterStatus { get; set; }
        public DateTime CreatedDate { get; set; }

    }
}
