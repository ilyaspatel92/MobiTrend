namespace Mobi.Web.Models.AccessControl
{
    public class SaveAccessRequest
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public List<string> Authorities { get; set; }
    }
}
