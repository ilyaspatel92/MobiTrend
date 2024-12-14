namespace Mobi.Data.Domain
{
    public class Language : BaseEntity
    {
        public string LanguageName {  get; set; }
        public int DisplayOrder { get; set; }
        public string UniqueSeoCode { get; set; }
        public bool Published { get; set; }
    }
}
