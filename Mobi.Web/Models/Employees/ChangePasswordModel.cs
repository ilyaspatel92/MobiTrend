using System.ComponentModel.DataAnnotations;
using Mobi.Data;

namespace Mobi.Web.Models.Employees
{
    public partial class ChangePasswordModel : BaseEntity
    {
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        public string ConfirmNewPassword { get; set; }

    }
}