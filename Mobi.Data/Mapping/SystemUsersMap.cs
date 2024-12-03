using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mobi.Data.Domain;

namespace Mobi.Data.Mapping
{
    public class SystemUsersMap
    {
        public SystemUsersMap(EntityTypeBuilder<SystemUsers> entityBuilder)
        {
            entityBuilder.HasKey(c => c.Id);
        }
    }
}
