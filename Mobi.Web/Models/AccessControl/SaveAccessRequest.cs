namespace Mobi.Web.Models.AccessControl
{
    public class SaveAccessRequest
    {
        public int UserId { get; set; }
        public List<string> Authorities { get; set; }
    }
}
