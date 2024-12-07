namespace Mobi.Web.Models.APIModels
{
    public abstract class DtoWithId 
    {
        /// <summary>
        /// Gets or sets the dto object identifier
        /// </summary>
        public int Id { get; set; }

        public int LanguageId { get; set; }
    }
}
