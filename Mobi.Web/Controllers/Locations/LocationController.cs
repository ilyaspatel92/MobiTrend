using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mobi.Data.Domain;
using Mobi.Service.Locations;
using Mobi.Web.Factories.Locations;
using Mobi.Web.Models.Locations;

namespace Mobi.Web.Controllers.Locations
{
    [Authorize]
    public class LocationController : Controller
    {
        private readonly ILocationService _locationService;
        private readonly ILocationFactory _locationFactory;

        public LocationController(ILocationService locationService, ILocationFactory locationFactory)
        {
            _locationService = locationService;
            _locationFactory = locationFactory;
        }

        [HttpGet]
        public IActionResult List(string name, int? id)
        {
            var locations = _locationService.GetAllLocations();

            if (!string.IsNullOrEmpty(name))
            {
                locations = locations.Where(l => l.LocationNameEnglish.Contains(name, StringComparison.OrdinalIgnoreCase) ||
                                                 l.LocationNameArabic.Contains(name, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (id.HasValue)
            {
                locations = locations.Where(l => l.Id == id.Value).ToList();
            }

            var locationViewModels = _locationFactory.PrepareLocationViewModels(locations);

            return View(locationViewModels);
        }

        [HttpGet]
        public IActionResult GetLocations(string name, int? id)
        {
            var locations = _locationService.GetAllLocations();

            if (!string.IsNullOrEmpty(name))
            {
                locations = locations.Where(l => l.LocationNameEnglish.Contains(name, StringComparison.OrdinalIgnoreCase) ||
                                                 l.LocationNameArabic.Contains(name, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (id.HasValue)
            {
                locations = locations.Where(l => l.Id == id.Value).ToList();
            }

            var locationViewModels = _locationFactory.PrepareLocationViewModels(locations);

            return Json(new
            {
                draw = Request.Query["draw"], // DataTables 'draw' parameter
                recordsTotal = locations.Count(), // Total records before filtering
                recordsFiltered = locations.Count(), // Filtered records after applying search
                data = locationViewModels // The filtered data
            });
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(LocationModel model)
        {
            if (ModelState.IsValid)
            {
                var location = new Location
                {
                    LocationNameEnglish = model.LocationNameEnglish,
                    LocationNameArabic = model.LocationNameArabic,
                    Status = model.Status,
                    BeaconProof = model.BeaconProofGPSProof,
                    Latitude = model.Latitude,
                    Longitude = model.Longitude,
                    SetRadius = model.SetRadius,
                    GPSLocationAddress = model.GPSLocationAddress,
                    CompanyId = model.CompanyId,
                    CreatedDate = DateTime.Now
                };

                _locationService.AddLocation(location);
                return RedirectToAction(nameof(List));
            }

            return View(model);
        }

        public IActionResult Edit(int id)
        {
            var location = _locationService.GetLocationById(id);
            if (location == null)
            {
                return NotFound();
            }

            var locationModel = _locationFactory.PrepareLocationViewModel(location);

            return View(locationModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, LocationModel model)
        {
            if (id != model.Id)
            {
                ModelState.AddModelError("", "Invalid location ID.");
                return View(model);
            }

            if (ModelState.IsValid)
            {
                var existingLocation = _locationService.GetLocationById(id);
                if (existingLocation == null)
                {
                    return NotFound();
                }

                existingLocation.LocationNameEnglish = model.LocationNameEnglish;
                existingLocation.LocationNameArabic = model.LocationNameArabic;
                existingLocation.Status = model.Status;
                existingLocation.BeaconProof = model.BeaconProofGPSProof;
                existingLocation.Latitude = model.Latitude;
                existingLocation.Longitude = model.Longitude;
                existingLocation.SetRadius = model.SetRadius;
                existingLocation.GPSLocationAddress = model.GPSLocationAddress;
                existingLocation.CompanyId = model.CompanyId;

                _locationService.UpdateLocation(existingLocation);
                return RedirectToAction(nameof(List));
            }

            return View(model);
        }

        public IActionResult Delete(int id)
        {
            var location = _locationService.GetLocationById(id);
            if (location == null)
            {
                return NotFound();
            }

            var locationModel = _locationFactory.PrepareLocationViewModel(location);

            return View(locationModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var location = _locationService.GetLocationById(id);
            if (location == null)
            {
                return NotFound();
            }

            _locationService.RemoveLocation(location);
            return RedirectToAction(nameof(List));
        }

        public IActionResult Details(int id)
        {
            var location = _locationService.GetLocationById(id);
            if (location == null)
            {
                return NotFound();
            }

            var locationModel = _locationFactory.PrepareLocationViewModel(location);

            return View(locationModel);
        }
    }
}
