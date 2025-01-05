using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mobi.Data.Domain;

namespace Mobi.Service.SystemUserAuthoritys
{
    public interface ISystemUserAuthorityService
    {
        public IEnumerable<SystemUserAuthorityMapping> GetAuthoritiesByUserId(int userId);

        public void DeleteByUserId(int userId);

        public void Insert(SystemUserAuthorityMapping mapping);

    }
}
