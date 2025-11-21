using System;
using System.Threading.Tasks;
using DocsVision.BackOffice.ObjectModel;
using DocsVision.BusinessTrip.WebClient.Models;
using DocsVision.BusinessTrip.WebClient.Services.Interfaces;
using DocsVision.Platform.WebClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ServerExtension.Helpers;


namespace DocsVision.BusinessTrip.WebClient.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BusinessTripApiController : ControllerBase
    {
        private readonly ICurrentObjectContextProvider _contextProvider;
        private readonly IBusinessTripBackendService _backendService;
        private readonly ILogger<BusinessTripApiController> _logger;

        public BusinessTripApiController(
            ICurrentObjectContextProvider contextProvider,
            IBusinessTripBackendService backendService,
            ILogger<BusinessTripApiController> logger)
        {
            _contextProvider = contextProvider ?? throw new ArgumentNullException(nameof(contextProvider));
            _backendService = backendService ?? throw new ArgumentNullException(nameof(backendService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("UpdateOnEmployeeChange")]
        public IActionResult UpdateOnEmployeeChange([FromBody] EmployeeChangeRequestModel request)
        {
            _logger.LogInformation("UpdateOnEmployeeChange called for card {CardId}, employee account: {Account}",
               request.CardId, request.EmployeeAccount);

            var sessionContext = _contextProvider.GetOrCreateCurrentSessionContext();
            var objectContext = sessionContext.ObjectContext;

            try
            {
                var card = objectContext.GetObject<Document>(request.CardId);
                _backendService.UpdateFieldsOnEmployeeChange(card, request.EmployeeAccount);

                _logger.LogInformation("Fields updated successfully for card {CardId}", request.CardId);
                return Ok(new { Message = "Fields updated successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating fields on employee change for card {CardId}", request.CardId);
                return StatusCode(500, new { Message = $"Error updating fields: {ex.Message}" });
            }
        }

        [HttpPost("UpdateOnCityChange")]
        public IActionResult UpdateOnCityChange([FromBody] BusinessTripRequestModel request)
        {
            _logger.LogInformation("UpdateOnCityChange called for card {CardId}, city: {City}, dates: {DateFrom} - {DateTo}",
                request.CardId, request.CityName, request.DateFrom, request.DateTo);
            var sessionContext = _contextProvider.GetOrCreateCurrentSessionContext();
            var objectContext = sessionContext.ObjectContext;

            try
            {
                var card = objectContext.GetObject<Document>(request.CardId);
                if (string.IsNullOrEmpty(request.CityName) || request.DateFrom == default || request.DateTo == default)
                {
                    _logger.LogWarning("Missing required parameters for card {CardId}", request.CardId);
                    return BadRequest(new { Message = "Missing required parameters (City, DateFrom, DateTo)." });
                }

                if (!request.DateFrom.TryParseStandardDate(out DateTime dateFrom))
                {
                    _logger.LogWarning("Invalid DateFrom format: {DateFrom} for card {CardId}", request.DateFrom, request.CardId);
                    return BadRequest(new { Message = "Invalid DateFrom format." });
                }

                if (!request.DateTo.TryParseStandardDate(out DateTime dateTo))
                {
                    _logger.LogWarning("Invalid DateTo format: {DateTo} for card {CardId}", request.DateTo, request.CardId);
                    return BadRequest(new { Message = "Invalid DateTo format." });
                }

                _backendService.UpdateFieldsOnCityChange(card, request.CityName, dateFrom, dateTo);

                _logger.LogInformation("Fields updated successfully for card {CardId}", request.CardId);
                return Ok(new { Message = "Fields updated successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating fields on city change for card {CardId}", request.CardId);
                return StatusCode(500, new { Message = $"Error updating fields: {ex.Message}" });
            }
        }

        [HttpGet("GetEmployeeAccount/{employeeId}")]
        public IActionResult GetEmployeeAccount(string employeeId)
        {
            _logger.LogInformation("GetEmployeeAccount called for employee {EmployeeId}", employeeId);

            if (!Guid.TryParse(employeeId, out var guid))
            {
                _logger.LogWarning("Invalid employee identifier format: {EmployeeId}", employeeId);
                return BadRequest(new { Message = "Invalid employee identifier format." });
            }

            try
            {   var session = _contextProvider.GetOrCreateCurrentSessionContext();
                var oc = session.ObjectContext;
                var employee = oc.GetObject<StaffEmployee>(guid);
                if (employee == null)
                {
                    _logger.LogWarning("Employee {EmployeeId} not found", guid);
                    return NotFound(new { Message = "Employee with Id = {guid} not found." });
                }
                var response = new EmployeeAccountResponseModel
                {
                    AccountName = employee.AccountName
                };

                _logger.LogInformation("Employee account retrieved successfully for {EmployeeId}", guid);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving account information for employee {EmployeeId}", employeeId);
                return StatusCode(500, new { Message = $"Error retrieving account information: {ex.Message}" });
            }
        }

        [HttpPost("GetTicketPrices")]
        public async Task<IActionResult> GetTicketPrices([FromBody] BusinessTripRequestModel request)
        {
            _logger.LogInformation("GetTicketPrices called for city: {City}, dates: {DateFrom} - {DateTo}",
                request.CityName, request.DateFrom, request.DateTo);
            try
            {
                if (string.IsNullOrEmpty(request.CityName) || request.DateFrom == default || request.DateTo == default)
                {
                    _logger.LogWarning("Missing required parameters for GetTicketPrices");
                    return BadRequest(new { Message = "Missing required parameters (City, DateFrom, DateTo)." });
                }

                if(!request.DateFrom.TryParseStandardDate(out DateTime dateFrom))
                {
                    _logger.LogWarning("Invalid DateFrom format: {DateFrom}", request.DateFrom);
                    return BadRequest(new { Message = "Invalid DateFrom format." });
                }

                if(!request.DateTo.TryParseStandardDate(out DateTime dateTo))
                {
                    _logger.LogWarning("Invalid DateTo format: {DateTo}", request.DateTo);
                    return BadRequest(new { Message = "Invalid DateTo format." });
                }

                var flightOptions = await _backendService.GetTicketPrices(
                    request.CityName, dateFrom, dateTo);

                _logger.LogInformation("Ticket prices retrieved successfully, found {Count} options", flightOptions.Count);
                return Ok(flightOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching ticket prices for city {City}", request.CityName);
                return StatusCode(500, new { Message = $"Error fetching ticket prices: {ex.Message}" });
            }
        }

    }

}