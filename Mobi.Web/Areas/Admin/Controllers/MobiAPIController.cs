using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mobi.Data.Domain.Employees;
using Mobi.Service.Employees;
using Mobi.Web.Factories.Employees;
using Mobi.Web.Models.APIModels;

namespace Mobi.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [ApiController]
    [Route("Admin/[controller]/[action]")]
    [Authorize] // Ensure this controller requires authentication
    public class MobiAPIController : Controller
    {

        private readonly IEmployeeService _employeeService;
        private readonly IEmployeeFactory _employeeFactory;

        public MobiAPIController(IEmployeeService employeeService, IEmployeeFactory employeeFactory)
        {
            _employeeService = employeeService;
            _employeeFactory = employeeFactory;
        }

        [HttpGet]
        public IActionResult Index()
        {
            ResponseModel<string> response = new ResponseModel<string>();
            try
            {

                response.Success = true;
                response.Message = "success";
                response.Data = "User created successfully!!";
                return Ok(response);              //OR return response
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Failure";
                response.Data = null;
                response.Exception = ex;
                return StatusCode(500, response);  //OR return response
            }
        }

        [HttpGet]
        public IActionResult GetUsers()
        {

            var employees = _employeeService.GetAllEmployees().ToList();
            return Ok(employees);

        }

        [HttpGet]
        public IActionResult GetEmployees()
        {

            ResponseModel<List<Employee>> response = new ResponseModel<List<Employee>>();

            try
            {
                //////item with datatype of customModel

                var employees = _employeeService.GetAllEmployees().ToList();
                if (employees == null)
                {
                    response.Success = false;
                    response.Message = "Item not found.";
                    return NotFound(response);  //OR return response
                }

                response.Success = true;
                response.Message = "Item retrieved successfully.";
                response.Data = employees;
                return Ok(response);           //OR return response
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Failure";
                response.Exception = ex;
                return StatusCode(500, response);  //OR return response
            }

        }


        [HttpGet]
        public IActionResult VerifyQrCode(int langId, string qrCode)
        {
            ResponseModel<List<Employee>> response = new ResponseModel<List<Employee>>();

            try
            {
                var employees = _employeeService.GetAllEmployees().ToList();
                if (employees == null)
                {
                    response.Success = false;
                    response.Message = "Item not found.";
                    return NotFound(response);  //OR return response
                }

                response.Success = true;
                response.Message = "Item retrieved successfully.";
                response.Data = employees;
                return Ok(response);           //OR return response
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Failure";
                response.Exception = ex;
                return StatusCode(500, response);  //OR return response
            }

        }

        #region Common 

        #endregion

        #region MyRegion

        #endregion
    }

}
