﻿using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mobi.Data.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mobi.Data.Mapping
{
    public class SystemUserAuthorityMappingMap
    {
        public SystemUserAuthorityMappingMap(EntityTypeBuilder<SystemUserAuthorityMapping> entityBuilder)
        {
            entityBuilder.HasKey(c => c.Id);
        }
    }
}
