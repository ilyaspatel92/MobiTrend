using Microsoft.AspNetCore.Mvc;
using Mobi.Service.Pictures;
using Mobi.Web.Models.APIModels;

namespace Mobi.Web.Areas.Admin.Controllers
{
    public class PictureAPIController : BaseAPIController
    {
        #region Fields

        private readonly IPictureService _pictureService;
        private readonly ILogger<PictureAPIController> _logger;

        #endregion

        #region Ctor

        public PictureAPIController(IPictureService pictureService,
            ILogger<PictureAPIController> logger)
        {
            _pictureService = pictureService;
            _logger = logger;
        }

        #endregion

        #region Methods

        [HttpPost]
        public async Task<IActionResult> SavePicture([FromForm] PictureAPIModel pictureToAdd)
        {
            try
            {
                if (pictureToAdd.ImageFile?.Length > 1 * 1024 * 1024)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, "File size should not exceed 1 MB");
                }

                // Check the allowed extenstions
                string[] allowedFileExtentions = [".jpg", ".jpeg", ".png"];
                var ext = Path.GetExtension(pictureToAdd.ImageFile.FileName);
                if (!allowedFileExtentions.Contains(ext))
                {
                    return StatusCode(StatusCodes.Status400BadRequest, $"Only {string.Join(",", allowedFileExtentions)} are allowed.");
                }
                
                string createdImagePath = await _pictureService.SaveFileAsync(pictureToAdd.ImageFile);

                return Ok(createdImagePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeletePicture(int id)
        {
            try
            {
                var existingPicture = _pictureService.GetPictureById(id);
                if (existingPicture == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound, $"Picture with id: {id} does not found");
                }

                _pictureService.DeletePicture(existingPicture);
                // After deleting Picture from database,remove file from directory.
                _pictureService.DeleteFile(existingPicture.Name);
                return NoContent();  // return 204
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        #endregion
    }
}
