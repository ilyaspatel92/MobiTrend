namespace Mobi.Web.Models.EmployeeLocations
{
    public class SaveEmployeeLocationsModel
    {
        public int EmployeeId { get; set; }
        public List<int>? LocationIds { get; set; }
        public bool isFreeLocation { get; set; }

    }
}
