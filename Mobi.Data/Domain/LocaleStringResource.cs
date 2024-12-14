
namespace Mobi.Data.Domain
{
    public class LocaleStringResource: BaseEntity
    {
        public string ResourceName { get; set; }
        public string ResourceValue { get; set; }
        public int LanguageId { get; set; }
    }
}
