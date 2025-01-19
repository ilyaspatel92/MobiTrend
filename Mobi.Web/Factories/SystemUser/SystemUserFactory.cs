using Mobi.Data.Domain;
using Mobi.Service.Helpers;
using Mobi.Web.Models.SystemUser;

namespace Mobi.Service.Factories
{
    /// <summary>
    /// Factory implementation for preparing SystemUsers entities.
    /// </summary>
    public class SystemUserFactory : ISystemUserFactory
    {
        /// <summary>
        /// Prepares a SystemUsers entity from the provided input data.
        /// </summary>
        /// <param name="employeeName">The employee's name.</param>
        /// <param name="userName">The user's name.</param>
        /// <param name="password">The password (in plain text).</param>
        /// <param name="companyId">The company ID.</param>
        /// <param name="userStatus">The user's status.</param>
        /// <returns>A prepared SystemUsers entity.</returns>
        public SystemUsers PrepareSystemUser(RegisterModel model)
        {
            return new SystemUsers
            {
                EmployeeName = model.EmployeeName,
                UserName = model.UserName,
                Email = model.Email,
                Password = PasswordHelper.HashPassword(model.Password),
                CompanyID = model.CompanyID,
                UserStatus = model.UserStatus,
                CreatedDate = DateTime.UtcNow,
                Deleted = false
            };
        }
    }
}
