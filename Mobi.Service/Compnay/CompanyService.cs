using Microsoft.EntityFrameworkCore;
using Mobi.Data.Domain;
using Mobi.Repository;

namespace Mobi.Service.Compnay
{
    /// <summary>
    /// Service to manage operations related to companies.
    /// </summary>
    public class CompanyService : ICompanyService
    {
        #region Fields

        private readonly IRepository<Company> _companyRepository;

        #endregion

        #region Ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="CompanyService"/> class.
        /// </summary>
        /// <param name="companyRepository">Repository for company operations.</param>
        public CompanyService(IRepository<Company> companyRepository)
        {
            _companyRepository = companyRepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Retrieves a list of companies matching the search text.
        /// </summary>
        /// <param name="searchText">The text to search for.</param>
        /// <returns>A list of matching companies.</returns>
        public IEnumerable<Company> GetCompanies(string searchText)
        {
            var query = _companyRepository.GetAll().AsQueryable();

            if (!string.IsNullOrEmpty(searchText))
            {
                query = query.Where(x => x.CompanyName.Contains(searchText) || x.CompanyId.ToString().Contains(searchText));
            }

            query = query.OrderBy(x => x.CompanyName)
                         .ThenBy(x => x.CompanyId)
                         .AsNoTracking();

            return query.ToList();
        }

        /// <summary>
        /// Inserts a new company into the system.
        /// </summary>
        /// <param name="company">The company to insert.</param>
        public void InsertCompany(Company company)
        {
            if (company == null)
                throw new ArgumentNullException(nameof(company));

            _companyRepository.Insert(company);
        }

        /// <summary>
        /// Retrieves a company by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the company.</param>
        /// <returns>The matching company.</returns>
        public Company GetCompanyById(int id)
        {
            return _companyRepository.GetById(id);
        }

        /// <summary>
        /// Updates an existing company's details.
        /// </summary>
        /// <param name="company">The company to update.</param>
        public void UpdateCompany(Company company)
        {
            if (company == null)
                throw new ArgumentNullException(nameof(company));

            _companyRepository.Update(company);
        }

        /// <summary>
        /// Deletes a company from the system.
        /// </summary>
        /// <param name="company">The company to delete.</param>
        public void DeleteCompany(Company company)
        {
            if (company == null)
                throw new ArgumentNullException(nameof(company));

            _companyRepository.Delete(company);
        }

        #endregion
    }
}

