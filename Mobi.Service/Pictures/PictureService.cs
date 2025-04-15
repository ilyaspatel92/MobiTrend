using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Mobi.Data.Domain;
using Mobi.Repository;
using System;

namespace Mobi.Service.Pictures
{
    /// <summary>
    /// Picture Service
    /// </summary>
    public class PictureService : IPictureService
    {
        #region Fields

        private readonly IRepository<Picture> _pictureRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;

        #endregion

        #region Ctor

        public PictureService(IRepository<Picture> pictureRepository,
            IWebHostEnvironment webHostEnvironment)
        {
            _pictureRepository = pictureRepository;
            _webHostEnvironment = webHostEnvironment;
        }

        #endregion

        #region Methods

        public void DeletePicture(Picture picture)
        {
            _pictureRepository.Delete(picture);
        }

        public Picture GetPictureById(int id)
        {
            return _pictureRepository.GetById(id);
        }

        public Picture GetPictureByName(string name)
        {
            return _pictureRepository.GetAll().FirstOrDefault(x => x.Name == name);
        }

        public IEnumerable<Picture> GetPictures()
        {
            return _pictureRepository.GetAll();
        }

        public void InsertPicture(Picture picture)
        {
            _pictureRepository.Insert(picture);
        }

        public void UpdatePicture(Picture picture)
        {
            _pictureRepository.Update(picture);
        }

        public async Task<Picture> SaveFileAsync(IFormFile imageFile)
        {
            if (imageFile == null)
            {
                throw new ArgumentNullException(nameof(imageFile));
            }

            var webPath = _webHostEnvironment.WebRootPath;
            var path = Path.Combine(webPath, "Uploads");
            // path = 

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            // Check the allowed extenstions
            var ext = Path.GetExtension(imageFile.FileName);
            
            // generate a unique filename
            var fileName = $"{Guid.NewGuid()}{ext}";
            var fileNameWithPath = Path.Combine(path, fileName);
            using var stream = new FileStream(fileNameWithPath, FileMode.Create);
            await imageFile.CopyToAsync(stream);

            var picture = new Picture
            {
                Name = fileName,
                Path = "Uploads/" + fileName,
                CreatedOn = DateTime.UtcNow
            };
            InsertPicture(picture);

            return picture;
        }

        public void DeleteFile(string fileNameWithExtension)
        {
            if (string.IsNullOrEmpty(fileNameWithExtension))
            {
                throw new ArgumentNullException(nameof(fileNameWithExtension));
            }
            var webPath = _webHostEnvironment.WebRootPath;
            var path = Path.Combine(webPath, $"Uploads", fileNameWithExtension);

            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"Invalid file path");
            }
            File.Delete(path);

            var picture = GetPictureByName(fileNameWithExtension);
            if (picture != null) { DeletePicture(picture); }
        }

        #endregion
    }
}
