using System.ComponentModel.DataAnnotations;

namespace Mobi.Web.Models.APIModels
{
    public class PictureAPIModel
    {
        [Required]
        public IFormFile ImageFile { get; set; }
    }
}
