using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mobi.Data.Domain
{
    public class SystemUserAuthorityMapping : BaseEntity
    {
        public int Id { get; set; }
        public int SystemUserID { get; set; }
         public string ScreenAuthority { get; set; }
         public string ScreenAuthoritySystemName { get; set; }
    }
}
