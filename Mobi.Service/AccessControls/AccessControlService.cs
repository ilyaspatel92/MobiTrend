using Microsoft.AspNetCore.Http;
using Mobi.Service.SystemUserAuthoritys;
using System.Security.Claims;

namespace Mobi.Service.AccessControls
{
    public class AccessControlService : IAccessControlService
    {

        private readonly ISystemUserAuthorityService _systemUserAuthorityService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccessControlService(ISystemUserAuthorityService systemUserAuthorityService,
                                    IHttpContextAccessor httpContextAccessor)
        {
            _systemUserAuthorityService = systemUserAuthorityService;
            _httpContextAccessor = httpContextAccessor;
        }

        public bool HasAccess(string screenAuthority)
        {

            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Sid);

            var authorities = _systemUserAuthorityService.GetAuthoritiesByUserId(Convert.ToInt32(userId))
                            .Select(a => a.ScreenAuthority)
                            .ToList();

            return authorities.Contains(screenAuthority);
        }
    }
}
