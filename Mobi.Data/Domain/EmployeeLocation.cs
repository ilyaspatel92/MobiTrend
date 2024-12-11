using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mobi.Data.Domain
{
    public class EmployeeLocation : BaseEntity
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int LocationId { get; set; }
    }
}
