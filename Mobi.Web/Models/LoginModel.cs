﻿using Mobi.Data.Domain.Employees;
using System.ComponentModel.DataAnnotations;

namespace Mobi.Web.Models
{
    public class LoginModel
    {
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string DeviceId { get; set; }
        public string Username { get; set; }

        public string RequestType { get; set; }

        public int CompanyId { get; set; }

    }
}
