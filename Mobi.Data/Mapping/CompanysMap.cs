using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mobi.Data.Domain;

namespace Mobi.Data.Mapping
{
    public class CompanysMap
    {
        public CompanysMap(EntityTypeBuilder<Company> entityBuilder)
        {
            entityBuilder.HasKey(c => c.Id);
        }
    }
}
