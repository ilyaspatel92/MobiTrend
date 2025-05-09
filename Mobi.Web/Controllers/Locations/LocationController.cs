﻿using Microsoft.AspNetCore.Mvc;
using Mobi.Data.Domain;
using Mobi.Data.Enums;
using Mobi.Service.AccessControls;
using Mobi.Service.LocationBeacons;
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
        private readonly ILocationBeaconService _locationBeaconService;
        public LocationController(ILocationService locationService,
                                  ILocationFactory locationFactory,
                                  IAccessControlService accessControlService,
                                  ILocationBeaconService locationBeaconService)
        {
            _locationService = locationService;
            _locationFactory = locationFactory;
            _accessControlService = accessControlService;
            _locationBeaconService = locationBeaconService;
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
                    CreatedDate = DateTime.UtcNow
                };

                _locationService.AddLocation(location);

                TempData["SuccessMessage"] = "Location added successfully.";

                return RedirectToAction(nameof(Edit), new { id = location.Id });
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
                return RedirectToAction("List", "Location");
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
                try
                {
                    var existingLocation = _locationService.GetLocationById(id);
                    if (existingLocation == null)
                    {
                        return RedirectToAction("List", "Location");
                    }

                    existingLocation.LocationNameEnglish = model.LocationNameEnglish;
                    existingLocation.LocationNameArabic = model.LocationNameArabic;
                    existingLocation.Status = model.Status;
                    existingLocation.ProofType = model.ProofType;
                    existingLocation.Latitude = model.Latitude;
                    existingLocation.Longitude = model.Longitude;
                    existingLocation.SetRadius = Math.Round(model.SetRadius, 2);
                    existingLocation.SetPolygon = model.SetPolygon;
                    existingLocation.GPSLocationAddress = model.GPSLocationAddress;

                    _locationService.UpdateLocation(existingLocation);
                    TempData["SuccessMessage"] = "Location updated successfully.";
                    return RedirectToAction(nameof(List));

                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
                    return RedirectToAction(nameof(List));
                }
               
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var location = _locationService.GetLocationById(id);

            if (location == null)
                return Json(false);

            var beacons = _locationBeaconService.GetAllLocationBeaconMappingsByLocationId(location.Id);

            foreach (var beacon in beacons)
                _locationBeaconService.RemoveLocationBeaconMapping(beacon);

            _locationService.RemoveLocation(location);

            return Json(true);
        }

        public IActionResult Details(int id)
        {
            bool hasAccess = _accessControlService.HasAccess(nameof(ScreenAuthorityEnum.Locations));

            if (!hasAccess)
                return RedirectToAction("AccessDenied", "AccessControl");

            var location = _locationService.GetLocationById(id);
            if (location == null)
            {
                return RedirectToAction("List", "Location");
            }

            var locationModel = _locationFactory.PrepareLocationViewModel(location);

            return View(locationModel);
        }
    }
}
