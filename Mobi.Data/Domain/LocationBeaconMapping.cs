using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mobi.Data.Domain
{
    public class LocationBeaconMapping : BaseEntity
    {
        public int Id { get; set; }
        public int LocationId { get; set; }
        public string BeaconName { get; set; }
        public Guid UUID { get; set; }
        public bool Status  { get; set; }
    }
}
