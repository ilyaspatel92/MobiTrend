using Mobi.Data.Enums;
using Mobi.Service.LocationBeacons;
using Mobi.Service.Locations;
using Mobi.Web.Factories.LocationBeacons;
using Mobi.Web.Models.APIModels;

namespace Mobi.Web.Areas.Admin.Factories
{
    public class BeaconLocationFactory : IBeaconLocationFactory
    {
        #region Fields

        private readonly ILocationService _locationService;
        private readonly ILocationBeaconService _locationBeaconService;
        private readonly ILocationBeaconFactory _locationBeaconFactory;
        #endregion

        #region Ctor

        public BeaconLocationFactory(ILocationService locationService, 
            ILocationBeaconService locationBeaconService,
            ILocationBeaconFactory locationBeaconFactory)
        {
            _locationService = locationService;
            _locationBeaconService = locationBeaconService;
            _locationBeaconFactory = locationBeaconFactory;
        }

        #endregion

        #region Methods

        public IEnumerable<LocationModel> PrepareLocationBeaconViewModel(int employeeId)
        {
            var locations = _locationService.GetAllEmployeeLocations(employeeId).ToList();

            return locations.Select(location =>
            {
                var locationBeacons = _locationBeaconService.GetAllLocationBeaconMappingsByLocationId(location.Id);

                return new LocationModel
                {
                    BeaconProof = (ProofType)location.ProofType == ProofType.Beacon,
                    CompanyId = location.CompanyId,
                    CreatedDate = location.CreatedDate.ToLocalTime(),
                    GPSLocationAddress = location.GPSLocationAddress,
                    GPSProof = (ProofType)location.ProofType == ProofType.GPS,
                    Id = location.Id,
                    Latitude = location.Latitude,
                    LocationNameArabic = location.LocationNameArabic,
                    LocationNameEnglish = location.LocationNameEnglish,
                    Longitude = location.Longitude,
                    SetRadius = location.SetRadius,
                    Status = location.Status,
                    BeaconList = _locationBeaconFactory.PrepareLocationBeaconViewModels(locationBeacons).ToList()
                };
            });
        }
        
        #endregion
    }
}
