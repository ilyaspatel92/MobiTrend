using Mobi.Web.Models.APIModels;

namespace Mobi.Web.Areas.Admin.Factories
{
    public interface IBeaconLocationFactory
    {
        IEnumerable<LocationModel> PrepareLocationBeaconViewModel(int employeeId);
    }
}
