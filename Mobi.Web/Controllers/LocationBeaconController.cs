using Microsoft.AspNetCore.Mvc;
using Mobi.Data.Domain;
using Mobi.Service.LocationBeacons;
using Mobi.Web.Factories.LocationBeacons;
using Mobi.Web.Models.LocationBeacons;

namespace Mobi.Web.Controllers.LocationBeacons
{
    public class LocationBeaconController : BasePublicController
    {
        private readonly ILocationBeaconService _locationBeaconService;
        private readonly ILocationBeaconFactory _locationBeaconFactory;

        public LocationBeaconController(ILocationBeaconService locationBeaconService, ILocationBeaconFactory locationBeaconFactory)
        {
            _locationBeaconService = locationBeaconService;
            _locationBeaconFactory = locationBeaconFactory;
        }

        [HttpGet]
        public IActionResult List(int? locationId)
        {
            var beacons = _locationBeaconService.GetAllLocationBeaconMappings();

            if (locationId.HasValue)
            {
                beacons = beacons.Where(b => b.LocationId == locationId.Value).ToList();
            }

            var beaconViewModels = _locationBeaconFactory.PrepareLocationBeaconViewModels(beacons);

            return View(beaconViewModels);
        }

        [HttpGet]
        public IActionResult GetBeacons(int? locationId)
        {
            var beacons = _locationBeaconService.GetAllLocationBeaconMappings();

            if (locationId.HasValue)
            {
                beacons = beacons.Where(b => b.LocationId == locationId.Value).ToList();
            }

            var beaconViewModels = _locationBeaconFactory.PrepareLocationBeaconViewModels(beacons);

            return Json(beaconViewModels);
        }

        [HttpPost]
        public IActionResult Create([FromBody] LocationBeaconModel model)
        {
            if (ModelState.IsValid)
            {
                var beacon = new LocationBeaconMapping
                {
                    BeaconName = model.BeaconName,
                    UUID = model.UUID,
                    Status = model.Status,
                    LocationId = model.LocationId
                };

                _locationBeaconService.AddLocationBeaconMapping(beacon);

                return Json(new
                {
                    id = beacon.Id,
                    BeaconName = beacon.BeaconName,
                    UUID = beacon.UUID,
                    Status = beacon.Status,
                    LocationId = beacon.LocationId
                });
            }

            return BadRequest(ModelState);
        }

        [HttpPut]
        public IActionResult Edit([FromBody] LocationBeaconModel model)
        {
            if (ModelState.IsValid)
            {
                var existingBeacon = _locationBeaconService.GetLocationBeaconMappingById(model.Id);
                if (existingBeacon == null)
                {
                    return RedirectToAction("List", "LocationBeacon");
                }

                existingBeacon.BeaconName = model.BeaconName;
                existingBeacon.UUID = model.UUID;
                existingBeacon.Status = model.Status;
                existingBeacon.LocationId = model.LocationId;

                _locationBeaconService.UpdateLocationBeaconMapping(existingBeacon);

                return Json(new
                {
                    id = existingBeacon.Id,
                    BeaconName = existingBeacon.BeaconName,
                    UUID = existingBeacon.UUID,
                    Status = existingBeacon.Status,
                    LocationId = existingBeacon.LocationId
                });
            }

            return BadRequest(ModelState);
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var beacon = _locationBeaconService.GetLocationBeaconMappingById(id);
            if (beacon == null)
            {
                return Ok(new { message = "Beacon deleted Failed not found." });
            }

            _locationBeaconService.RemoveLocationBeaconMapping(beacon);

            return Ok(new { message = "Beacon deleted successfully." });
        }
    }
}
