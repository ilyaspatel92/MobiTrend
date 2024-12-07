using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mobi.Data.Domain.Employees;

namespace Mobi.Data.Mapping
{
    public class EmployeeMap
    {
        public EmployeeMap(EntityTypeBuilder<Employee> entityBuilder)
        {
            entityBuilder.HasKey(c => c.Id);
        }
    }
   
}
