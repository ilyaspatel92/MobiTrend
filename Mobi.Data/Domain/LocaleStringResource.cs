using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mobi.Data.Domain
{
    public class LocaleStringResource: BaseEntity
    {
        public int Id { get; set; }
        public string ResourceName { get; set; }
        public string ResourceValue { get; set; }
        public int LanguageId { get; set; }
    }
}
