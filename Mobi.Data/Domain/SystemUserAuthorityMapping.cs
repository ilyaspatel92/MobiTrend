
namespace Mobi.Data.Domain
{
    public class SystemUserAuthorityMapping : BaseEntity
    {
        public int SystemUserID { get; set; }
         public string ScreenAuthority { get; set; }
         public string ScreenAuthoritySystemName { get; set; }
    }
}
