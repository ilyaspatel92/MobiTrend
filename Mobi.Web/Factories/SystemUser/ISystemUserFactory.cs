using Mobi.Data.Domain;
using Mobi.Web.Models.SystemUser;

namespace Mobi.Service.Factories
{
    /// <summary>
    /// Defines methods to prepare and map SystemUsers entities.
    /// </summary>
    public interface ISystemUserFactory
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
        SystemUsers PrepareSystemUser(RegisterModel model);
    }
}
