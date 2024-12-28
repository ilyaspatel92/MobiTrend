namespace Mobi.Web.Models.EmployeeLocations
{
    public class EmployeeLocationViewModel
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public List<LocationViewModel> Locations { get; set; }
    }

    public class LocationViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsSelected { get; set; }
    }
}
