using System.Dynamic;
using Microsoft.AspNetCore.Authorization;
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
        [AllowAnonymous]
        public async Task<IActionResult> SavePicture([FromForm] PictureAPIModel pictureToAdd)
        {
            var response = new ResponseModel<ExpandoObject>();

            try
            {
                if (pictureToAdd.ImageFile?.Length > 1 * 1024 * 1024)
                {
                    response.Success = false;
                    response.Message = "File size should not exceed 1 MB";
                    return BadRequest(response);
                }

                // Check the allowed extenstions
                string[] allowedFileExtentions = [".jpg", ".jpeg", ".png"];
                var ext = Path.GetExtension(pictureToAdd.ImageFile.FileName);
                if (!allowedFileExtentions.Contains(ext))
                {
                    response.Success = false;
                    response.Message = $"Only {string.Join(",", allowedFileExtentions)} are allowed.";
                    return BadRequest(response);
                }
                
               var picture = await _pictureService.SaveFileAsync(pictureToAdd.ImageFile);

                var url= $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/";

                dynamic pictureObject = new ExpandoObject();
                pictureObject.Id = picture.Id;
                pictureObject.Name = picture.Name;
                pictureObject.Path = url+ picture.Path;

                response.Success = true;
                response.Message = "Item retrieved successfully.";
                response.Data = pictureObject;
                return Ok(response);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response.Success = false;
                response.Message = ex.Message;
                response.Exception = ex;
                return BadRequest(response);
            }
        }

        [HttpDelete("{id}")]
        [AllowAnonymous]
        public IActionResult DeletePicture(int id)
        {
            var response = new ResponseModel<ExpandoObject>();

            try
            {
                var existingPicture = _pictureService.GetPictureById(id);
                if (existingPicture == null)
                {
                    response.Success = false;
                    response.Message = $"Picture with id: {id} does not found";
                    return BadRequest(response);
                }

                _pictureService.DeletePicture(existingPicture);
                // After deleting Picture from database,remove file from directory.
                _pictureService.DeleteFile(existingPicture.Name);
                response.Success = true;
                response.Message = "picture deleted successfully";
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response.Success = false;
                response.Message = ex.Message;
                response.Exception = ex;
                return BadRequest(response);
            }
        }

        #endregion
    }
}
