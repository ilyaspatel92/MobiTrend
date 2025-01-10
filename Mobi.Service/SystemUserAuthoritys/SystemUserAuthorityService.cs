using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mobi.Data.Domain;
using Mobi.Repository;

namespace Mobi.Service.SystemUserAuthoritys
{
    public class SystemUserAuthorityService : ISystemUserAuthorityService
    {
        private readonly IRepository<SystemUserAuthorityMapping> _systemUserAuthorityMappingRepository;

        public SystemUserAuthorityService(IRepository<SystemUserAuthorityMapping> systemUserAuthorityMappingRepository)
        {
            _systemUserAuthorityMappingRepository = systemUserAuthorityMappingRepository;
        }

        /// <summary>
        /// Retrieves the list of authorities for a specific user.
        /// </summary>
        /// <param name="userId">The ID of the system user.</param>
        /// <returns>List of SystemUserAuthorityMapping.</returns>
        public IEnumerable<SystemUserAuthorityMapping> GetAuthoritiesByUserId(int userId)
        {
            return _systemUserAuthorityMappingRepository.GetAll().Where(x => x.SystemUserID == userId).ToList();
        }

        /// <summary>
        /// Deletes all authorities for a specific user.
        /// </summary>
        /// <param name="userId">The ID of the system user.</param>
        public void DeleteByUserId(int userId)
        {
            var mappings = _systemUserAuthorityMappingRepository.GetAll().Where(x => x.SystemUserID == userId).ToList();
            foreach (var mapping in mappings)
            {
                _systemUserAuthorityMappingRepository.Delete(mapping);
            }
        }

        /// <summary>
        /// Inserts a new authority mapping for a user.
        /// </summary>
        /// <param name="mapping">The authority mapping to insert.</param>
        public void Insert(SystemUserAuthorityMapping mapping)
        {
            _systemUserAuthorityMappingRepository.Insert(mapping);
        }
    }

}
