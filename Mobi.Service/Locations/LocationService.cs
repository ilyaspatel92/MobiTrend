using System.Text.Json;
using Mobi.Data.Domain;
using Mobi.Repository;

namespace Mobi.Service.Locations
{
    public class LocationService : ILocationService
    {
        private readonly IRepository<Location> _locationRepository;
        private readonly IRepository<EmployeeLocation> _employeeLocationRepository;


        public LocationService(IRepository<Location> locationRepository, IRepository<EmployeeLocation> employeeLocationRepository)
        {
            _locationRepository = locationRepository;
            _employeeLocationRepository = employeeLocationRepository;
        }

        public IEnumerable<Location> GetAllLocations()
        {
            return _locationRepository.GetAll();
        }

        public IList<Location> GetAllEmployeeLocations(int employeeId)
        {
            var query =
                   from l in _locationRepository.GetAll()
                   join elc in _employeeLocationRepository.GetAll() on l.Id equals elc.LocationId
                   where elc.EmployeeId == employeeId
                   select l;

            return query.ToList();
        }

        
        public Location GetLocationById(int id)
        {
            return _locationRepository.GetById(id);
        }

        public void AddLocation(Location location)
        {
            _locationRepository.Insert(location);
        }

        public void UpdateLocation(Location location)
        {
            _locationRepository.Update(location);
        }

        public void RemoveLocation(Location location)
        {
            _locationRepository.Delete(location);
        }

        public bool IsLocationNameExists(string locationNameEnglish)
        {
            return _locationRepository.GetAll()
                .Any(loc => loc.LocationNameEnglish.Equals(locationNameEnglish, StringComparison.OrdinalIgnoreCase));
        }

        private readonly string _apiKey = "AIzaSyAkN-Q_IuLUmbFX_QHCzEEecAufZmwpZ7k";

        public string GetAddressFromCoordinates(decimal latitude, decimal longitude)
        {
            string url = $"https://maps.googleapis.com/maps/api/geocode/json?latlng={latitude},{longitude}&key={_apiKey}";

            using (var httpClient = new HttpClient())
            {
                var response = httpClient.GetAsync(url).Result; // <== Synchronous call
                if (response.IsSuccessStatusCode)
                {
                    var json = response.Content.ReadAsStringAsync().Result;
                    var obj = JsonDocument.Parse(json);
                    var address = obj.RootElement.GetProperty("results").EnumerateArray().FirstOrDefault().GetProperty("formatted_address").GetString();
                    return address;
                }
            }

            return null;
        }
    }
}
