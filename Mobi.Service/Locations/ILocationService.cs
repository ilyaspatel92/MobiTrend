using Mobi.Data.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mobi.Service.Locations
{
    public interface ILocationService
    {
        IEnumerable<Location> GetAllLocations();
        Location GetLocationById(int id);
        void AddLocation(Location location);
        void UpdateLocation(Location location);
        void RemoveLocation(Location location);
        bool IsLocationNameExists(string locationNameEnglish);
    }
}
