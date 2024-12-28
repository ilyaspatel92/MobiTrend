using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mobi.Data.Domain;

namespace Mobi.Data.Mapping
{
    public class PictureMap
    {
        public PictureMap(EntityTypeBuilder<Picture> entityBuilder)
        {
            entityBuilder.HasKey(c => c.Id);
        }
    }
}
