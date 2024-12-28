namespace Mobi.Data.Domain
{
    public class Picture : BaseEntity
    {
        public string Path { get; set; }
        public string Name { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
