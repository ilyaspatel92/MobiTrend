using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mobi.Data.Domain
{
    public class Language : BaseEntity
    {
        public int Id { get; set; }
        public string LanguageName {  get; set; }
        public int DisplayOrder { get; set; }
        public string UniqueSeoCode { get; set; }
        public bool Published { get; set; }
    }
}
