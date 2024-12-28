using Microsoft.AspNetCore.Mvc;
using Mobi.Service.Employees;
using Mobi.Web.Areas.Admin.Factories;
using Mobi.Web.Models.APIModels;

namespace Mobi.Web.Areas.Admin.Controllers
{
    public class LocationAPIController : BaseAPIController
    {
        #region Fields

        private readonly IBeaconLocationFactory _beaconLocationFactory;
        private readonly IEmployeeService _employeeService;

        #endregion

        #region Ctor
        public LocationAPIController(IBeaconLocationFactory beaconLocationFactory, IEmployeeService employeeService)
        {
            _beaconLocationFactory = beaconLocationFactory;
            _employeeService = employeeService;
        }

        #endregion

        #region Methods

        [HttpGet]
        public IActionResult GetAllLocations(int LangId)
        {

            var response = new ResponseModel<IList<LocationModel>>();

            try
            {
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var employee = _employeeService.GetCurrentEmployee(token);

                if (employee == null)
                {
                    response.Success = false;
                    response.Message ="Employee record are not found";
                    return BadRequest(response);
                }

                var locationList = _beaconLocationFactory.PrepareLocationBeaconViewModel().ToList();
                response.Success = true;
                response.Message = "Location List successfully";
                response.Data = locationList;
                return Ok(response);

            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                response.Exception = ex;
                return BadRequest(response);
            }
        }

        #endregion
    }
}
