using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mobi.Data.Domain;

namespace Mobi.Data.Mapping
{
    public class LanguageMap
    {
        public LanguageMap(EntityTypeBuilder<Language> entityBuilder)
        {
            entityBuilder.HasKey(c => c.Id);
        }
    }
}
