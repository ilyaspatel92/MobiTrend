
namespace Mobi.Data.Domain
{
    public class EmployeeLocation : BaseEntity
    {
        public int EmployeeId { get; set; }
        public int LocationId { get; set; }
        public bool IsFreeLocation { get; set; }
    }
}
