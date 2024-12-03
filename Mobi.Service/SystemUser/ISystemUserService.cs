using Mobi.Data.Domain;

namespace Mobi.Service.SystemUser
{
    /// <summary>
    /// Interface to manage operations related to system users.
    /// </summary>
    public interface ISystemUserService
    {
        /// <summary>
        /// Retrieves a list of system users based on the search text.
        /// The search text can be the user's name or code.
        /// </summary>
        /// <param name="searchText">The text to search for (name or code).</param>
        /// <returns>A collection of matching system users.</returns>
        IEnumerable<SystemUsers> GetSystemUsers(string searchText);

        /// <summary>
        /// Retrieves the details of a system user by their unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the system user.</param>
        /// <returns>The details of the specified system user.</returns>
        SystemUsers GetSystemUserById(int id);

        /// <summary>
        /// Inserts a new system user into the system.
        /// </summary>
        /// <param name="systemUser">The details of the system user to be added.</param>
        void InsertSystemUser(SystemUsers systemUser);

        /// <summary>
        /// Updates the details of an existing system user.
        /// </summary>
        /// <param name="systemUser">The updated details of the system user.</param>
        void UpdateSystemUser(SystemUsers systemUser);

        /// <summary>
        /// Deletes a system user from the system.
        /// </summary>
        /// <param name="systemUser">The system user to be deleted.</param>
        void DeleteSystemUser(SystemUsers systemUser);

        /// <summary>
        /// Retrieves the details of a system user by their unique identifier.
        /// </summary>
        /// <param name="userName">System user.</param>
        /// <returns>The details of the specified system user.</returns>
        SystemUsers GetSystemUserByUserName(string userName);


        SystemUsers Authenticate(string username, string password);
        void ChangePassword(int userId, string newPassword);
        void ResetPassword(string username, string newPassword);
    }

}
