using Microsoft.AspNetCore.Mvc;
using Mobi.Web.Areas.Admin.Factories;

namespace Mobi.Web.Areas.Admin.Controllers
{
    public class LocationAPIController : BaseAPIController
    {
        #region Fields

        private readonly IBeaconLocationFactory _beaconLocationFactory;

        #endregion

        #region Ctor
        public LocationAPIController(IBeaconLocationFactory beaconLocationFactory)
        {
            _beaconLocationFactory = beaconLocationFactory;
        }

        #endregion

        #region Methods

        [HttpGet]
        public IActionResult GetAllLocations()
        {
            return Ok(_beaconLocationFactory.PrepareLocationBeaconViewModel());
        }

        #endregion
    }
}
