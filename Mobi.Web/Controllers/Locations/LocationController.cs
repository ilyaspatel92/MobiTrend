using Microsoft.AspNetCore.Mvc;
using Mobi.Data.Domain;
using Mobi.Data.Enums;
using Mobi.Service.AccessControls;
using Mobi.Service.Locations;
using Mobi.Web.Factories.Locations;
using Mobi.Web.Models.Locations;

namespace Mobi.Web.Controllers.Locations
{
    public class LocationController : BasePublicController
    {
        private readonly ILocationService _locationService;
        private readonly ILocationFactory _locationFactory;
        private readonly IAccessControlService _accessControlService;
        public LocationController(ILocationService locationService,
                                  ILocationFactory locationFactory,
                                  IAccessControlService accessControlService)
        {
            _locationService = locationService;
            _locationFactory = locationFactory;
            _accessControlService = accessControlService;
        }

        [HttpGet]
        public IActionResult List(string name, bool? status)
        {
            bool hasAccess = _accessControlService.HasAccess(nameof(ScreenAuthorityEnum.Locations));

            if (!hasAccess)
                return RedirectToAction("AccessDenied", "AccessControl");

            var locations = _locationService.GetAllLocations();

            if (!string.IsNullOrEmpty(name))
            {
                locations = locations.Where(l => l.LocationNameEnglish.Contains(name, StringComparison.OrdinalIgnoreCase) ||
                                                 l.LocationNameArabic.Contains(name, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (status.HasValue)
            {
                locations = locations.Where(l => l.Status == status.Value).ToList();
            }

            var locationViewModels = _locationFactory.PrepareLocationViewModels(locations);

            return View(locationViewModels);
        }

        [HttpGet]
        public IActionResult GetLocations(string name, bool? status)
        {
            var locations = _locationService.GetAllLocations();

            if (!string.IsNullOrEmpty(name))
            {
                locations = locations.Where(l => l.LocationNameEnglish.Contains(name, StringComparison.OrdinalIgnoreCase) ||
                                                 l.LocationNameArabic.Contains(name, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (status.HasValue)
            {
                locations = locations.Where(l => l.Status == status.Value).ToList();
            }

            var locationViewModels = _locationFactory.PrepareLocationViewModels(locations);

            return Json(new
            {
                draw = Request.Query["draw"],
                recordsTotal = locations.Count(),
                recordsFiltered = locations.Count(),
                data = locationViewModels
            });
        }
        public IActionResult Create()
        {
            bool hasAccess = _accessControlService.HasAccess(nameof(ScreenAuthorityEnum.Locations));

            if (!hasAccess)
                return RedirectToAction("AccessDenied", "AccessControl");

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
                    ProofType = model.ProofType,
                    Latitude = model.Latitude,
                    Longitude = model.Longitude,
                    SetRadius = model.SetRadius,
                    GPSLocationAddress = model.GPSLocationAddress,
                    CompanyId = 1,
                    CreatedDate = DateTime.Now
                };

                _locationService.AddLocation(location);
                return RedirectToAction(nameof(List));
            }

            return View(model);
        }

        public IActionResult Edit(int id)
        {
            bool hasAccess = _accessControlService.HasAccess(nameof(ScreenAuthorityEnum.Locations));

            if (!hasAccess)
                return RedirectToAction("AccessDenied", "AccessControl");

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
                existingLocation.ProofType = model.ProofType;
                existingLocation.Latitude = model.Latitude;
                existingLocation.Longitude = model.Longitude;
                existingLocation.SetRadius = model.SetRadius;
                existingLocation.SetPolygon = model.SetPolygon;
                existingLocation.GPSLocationAddress = model.GPSLocationAddress;

                _locationService.UpdateLocation(existingLocation);
                return RedirectToAction(nameof(List));
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
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
            bool hasAccess = _accessControlService.HasAccess(nameof(ScreenAuthorityEnum.Locations));

            if (!hasAccess)
                return RedirectToAction("AccessDenied", "AccessControl");

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
