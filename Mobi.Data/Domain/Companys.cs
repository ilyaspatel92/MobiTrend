﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mobi.Data.Domain
{
    public class Companys : BaseEntity
    {
        [Required]
        public string? CompanyName { get; set; }

        [Required]
        public string CompanyId { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }
    }
}
