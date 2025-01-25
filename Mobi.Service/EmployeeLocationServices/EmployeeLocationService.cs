using Mobi.Data.Domain;
using Mobi.Repository;

namespace Mobi.Service.EmployeeLocationServices
{
    public class EmployeeLocationService : IEmployeeLocationService
    {
        private readonly IRepository<EmployeeLocation> _employeeLocationRepository;

        public EmployeeLocationService(IRepository<EmployeeLocation> employeeLocationRepository)
        {
            _employeeLocationRepository = employeeLocationRepository;
        }

        public bool SaveLocationsForEmployee(int empId, List<int> locationIds,bool isFreeLocation)
        {
            ClearEmpLocations(empId);
            if (isFreeLocation)
            {
                _employeeLocationRepository.Insert(new EmployeeLocation() { EmployeeId = empId, LocationId = 0, IsFreeLocation = isFreeLocation });
                return true;
            }
            else if (empId > 0 && locationIds.Any())
            {
                foreach (var locationId in locationIds)
                    _employeeLocationRepository.Insert(new EmployeeLocation() { EmployeeId = empId, LocationId = locationId, IsFreeLocation = isFreeLocation });
                return true;
            }

            return false;
        }

        public IList<int> GetSelectedLocationsByEmployeeId(int empId)
        {
            if (empId > 0)
            {
                return _employeeLocationRepository.GetAllList().Where(x => x.EmployeeId == empId).Select(x => x.LocationId).ToList();
            }

            return null;
        }

        public bool IsFreeLocationSelected(int empId)
        {
            if (empId > 0)
            {
                return _employeeLocationRepository.GetAllList().Where(x => x.IsFreeLocation == true && x.EmployeeId == empId).Any();
            }

            return false;
        }

        public IEnumerable<EmployeeLocation> GetAllEmployeeLocations()
        {
            
            return _employeeLocationRepository.GetAll();           
        }

        public void ClearEmpLocations(int empId)
        {
            if (empId > 0)
            {
                var employeeLocations = _employeeLocationRepository.GetAllList().Where(x => x.EmployeeId == empId).ToList();

                foreach (var employeeLocation in employeeLocations)
                    _employeeLocationRepository.Delete(employeeLocation);
            }
        }
    }
}
