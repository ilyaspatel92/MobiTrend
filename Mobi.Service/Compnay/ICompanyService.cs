using Mobi.Data.Domain;

namespace Mobi.Service.Compnay
{
    /// <summary>
    /// Interface for managing operations related to companies.
    /// </summary>
    public interface ICompanyService
    {
        /// <summary>
        /// Retrieves a list of companies based on the search text.
        /// The search text can be the company name or ID.
        /// </summary>
        /// <param name="searchText">The text to search for.</param>
        /// <returns>A collection of matching companies.</returns>
        IEnumerable<Company> GetCompanies(string searchText);

        /// <summary>
        /// Inserts a new company into the system.
        /// </summary>
        /// <param name="company">The company to insert.</param>
        void InsertCompany(Company company);

        /// <summary>
        /// Retrieves a company by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the company.</param>
        /// <returns>The matching company.</returns>
        Company GetCompanyById(int id);

        /// <summary>
        /// Updates the details of an existing company.
        /// </summary>
        /// <param name="company">The company to update.</param>
        void UpdateCompany(Company company);

        /// <summary>
        /// Deletes a company from the system.
        /// </summary>
        /// <param name="company">The company to delete.</param>
        void DeleteCompany(Company company);
    }

}
