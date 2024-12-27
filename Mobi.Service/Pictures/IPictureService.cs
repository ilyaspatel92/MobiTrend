using Microsoft.AspNetCore.Http;
using Mobi.Data.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mobi.Service.Pictures
{
    /// <summary>
    /// Picture Service interface
    /// </summary>
    public interface IPictureService
    {
        /// <summary>
        /// Retrieves a list of Pictures.
        /// </summary>
        /// <returns>A collection of Pictures.</returns>
        IEnumerable<Picture> GetPictures();

        /// <summary>
        /// Inserts a new Picture into the system.
        /// </summary>
        /// <param name="Picture">The Picture to insert.</param>
        void InsertPicture(Picture picture);

        /// <summary>
        /// Retrieves a Picture by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the Picture.</param>
        /// <returns>The matching Picture.</returns>
        Picture GetPictureById(int id);

        /// <summary>
        /// Updates the details of an existing Picture.
        /// </summary>
        /// <param name="Picture">The Picture to update.</param>
        void UpdatePicture(Picture picture);

        /// <summary>
        /// Deletes a Picture from the system.
        /// </summary>
        /// <param name="Picture">The Picture to delete.</param>
        void DeletePicture(Picture picture);

        /// <summary>
        /// Save a Picture to the file system.
        /// </summary>
        /// <param name="Picture">The Picture to save.</param>
        Task<string> SaveFileAsync(IFormFile imageFile);

        /// <summary>
        /// Delete a Picture from the file system.
        /// </summary>
        /// <param name="fileNameWithExtension">Name of file to Delete.</param>
        void DeleteFile(string fileNameWithExtension);
    }
}
