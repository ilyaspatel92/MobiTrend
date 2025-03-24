using Mobi.Data.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mobi.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Mobi.Service.Helpers;
namespace Mobi.Service.SystemUser
{
    /// <summary>
    /// Service to manage operations related to SystemUsers.
    /// </summary>
    public class SystemUserService : ISystemUserService
    {
        #region Fields

        private readonly IRepository<SystemUsers> _systemUserRepository;

        #endregion

        #region Ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemUserService"/> class.
        /// </summary>
        /// <param name="systemUserRepository">Repository for system user operations.</param>
        public SystemUserService(IRepository<SystemUsers> systemUserRepository)
        {
            _systemUserRepository = systemUserRepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Retrieves system users matching the search text (name or username).
        /// </summary>
        /// <param name="searchText">The text to search by.</param>
        /// <returns>A list of matching system users.</returns>
        public IEnumerable<SystemUsers> GetSystemUsers(string searchText)
        {
            var query = _systemUserRepository.GetAll().AsQueryable();

            if (!string.IsNullOrEmpty(searchText))
            {
                query = query.Where(x => x.EmployeeName.Contains(searchText) || x.UserName.Contains(searchText));
            }

            query = query.OrderBy(x => x.EmployeeName)
                         .ThenBy(x => x.UserName)
                         .AsNoTracking();

            return query.ToList();
        }
        public IEnumerable<SystemUsers> GetAllUsers()
        {
            return _systemUserRepository.GetAll();

        }

        /// <summary>
        /// Inserts a new system user into the system.
        /// </summary>
        /// <param name="systemUser">The system user to insert.</param>
        public void InsertSystemUser(SystemUsers systemUser)
        {
            if (systemUser == null)
                throw new ArgumentNullException(nameof(systemUser));

            _systemUserRepository.Insert(systemUser);
        }

        /// <summary>
        /// Retrieves a system user by their unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the system user.</param>
        /// <returns>The matching system user.</returns>
        public SystemUsers GetSystemUserById(int id)
        {
            return _systemUserRepository.GetById(id);
        }

        public SystemUsers GetSystemUserByEmployeeId(int id)
        {
            return _systemUserRepository.GetAll().FirstOrDefault(u => u.EmployeeId == id);
        }

        /// <summary>
        /// Retrieves the details of a system user by their unique identifier.
        /// </summary>
        /// <param name="userName">System user.</param>
        /// <returns>The details of the specified system user.</returns>
        public SystemUsers GetSystemUserByUserName(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentException("User name cannot be null or empty.", nameof(userName));

            var users = _systemUserRepository.GetAll();
            if (users == null)
                throw new InvalidOperationException("User repository returned null.");

            return users.FirstOrDefault(x => x.UserName == userName);
        }

        /// <summary>
        /// Retrieves the details of a system user by their unique identifier.
        /// </summary>
        /// <param name="email">System user.</param>
        /// <returns>The details of the specified system user.</returns>
        public SystemUsers GetSystemUserByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be null or empty.", nameof(email));

            var users = _systemUserRepository.GetAll();
            if (users == null)
                throw new InvalidOperationException("User repository returned null.");

            return users.FirstOrDefault(x => x.Email == email);
        }

        /// <summary>
        /// Updates an existing system user's details.
        /// </summary>
        /// <param name="systemUser">The system user to update.</param>
        public void UpdateSystemUser(SystemUsers systemUser)
        {
            if (systemUser == null)
                throw new ArgumentNullException(nameof(systemUser));

            _systemUserRepository.Update(systemUser);
        }

        /// <summary>
        /// Deletes a system user from the system.
        /// </summary>
        /// <param name="systemUser">The system user to delete.</param>
        public void DeleteSystemUser(SystemUsers systemUser)
        {
            if (systemUser == null)
                throw new ArgumentNullException(nameof(systemUser));

            _systemUserRepository.Delete(systemUser);
        }


        public SystemUsers Authenticate(string userName, string password)
        {
            var user = _systemUserRepository.GetAll().FirstOrDefault(u => u.UserName == userName);
            if (user == null || !PasswordHelper.VerifyPassword(password, user.Password))
                return null; // Invalid credentials

            return user;
        }

        public void ChangePassword(int userId, string newPassword)
        {
            var user = GetSystemUserById(userId);
            if (user == null) throw new Exception("User not found");

            user.Password = PasswordHelper.HashPassword(newPassword);
            UpdateSystemUser(user);
        }

        public void ResetPassword(string username, string newPassword)
        {
            var user = _systemUserRepository.GetAll().FirstOrDefault(u => u.UserName == username);
            if (user == null) throw new Exception("User not found");

            user.Password = PasswordHelper.HashPassword(newPassword);
            UpdateSystemUser(user);
        }

        /// <summary>
        /// Saves a password reset token for a user.
        /// </summary>
        /// <param name="username">The username of the user.</param>
        /// <param name="token">The password reset token.</param>
        /// <param name="expiry">The expiry date of the token.</param>
        public void SavePasswordResetToken(string email, string token, DateTime expiry)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Username cannot be null or empty.", nameof(email));
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentException("Token cannot be null or empty.", nameof(token));

            var user = _systemUserRepository.GetAll().FirstOrDefault(u => u.Email == email);
            if (user == null)
                throw new Exception("User not found");

            user.PasswordResetToken = token;
            user.PasswordResetTokenExpiry = expiry;

            UpdateSystemUser(user);
        }

        /// <summary>
        /// Validates a password reset token.
        /// </summary>
        /// <param name="username">The username of the user.</param>
        /// <param name="token">The token to validate.</param>
        /// <returns>True if the token is valid; otherwise, false.</returns>
        //public bool ValidatePasswordResetToken(string username, string token)
        //{
        //    if (string.IsNullOrWhiteSpace(username))
        //        throw new ArgumentException("Username cannot be null or empty.", nameof(username));
        //    if (string.IsNullOrWhiteSpace(token))
        //        throw new ArgumentException("Token cannot be null or empty.", nameof(token));

        //    var user = _systemUserRepository.GetAll().FirstOrDefault(u => u.UserName == username);
        //    if (user == null)
        //        throw new Exception("User not found");

        //    return user.PasswordResetToken == token && user.PasswordResetTokenExpiry > DateTime.UtcNow;
        //}


        public bool ValidatePasswordResetToken(string email, string token)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be null or empty.", nameof(email));
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentException("Token cannot be null or empty.", nameof(token));

            var user = _systemUserRepository.GetAll().FirstOrDefault(u => u.Email == email);
            if (user == null)
                return false; // User not found

            if (string.IsNullOrWhiteSpace(user.PasswordResetToken) || user.PasswordResetTokenExpiry == null)
                return false; // No reset token or expiry set for the user

            if (user.PasswordResetToken != token)
                return false; // Token does not match

            if (DateTime.UtcNow.ToLocalTime() > user.PasswordResetTokenExpiry)
                return false; // Token has expired

            return true; // Token is valid
        }


        #endregion

    }

}
