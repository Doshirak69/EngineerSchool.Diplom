using DocsVision.BackOffice.CardLib.CardDefs;
using DocsVision.BackOffice.ObjectModel.Services;
using DocsVision.BackOffice.ObjectModel;
using DocsVision.Platform.ObjectModel;
using DocsVision.Platform.WebClient;
using System;
using System.Collections.Generic;
using System.Linq;
using DocsVision.BusinessTrip.WebClient.Services.Interfaces;
using System.Net.Http;
using ServerExtension.Models;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using NLog;

namespace DocsVision.BusinessTrip.WebClient.Services
{
    public class BusinessTripBackendService: IBusinessTripBackendService
    {
        private readonly ICurrentObjectContextProvider _contextProvider;
        private readonly HttpClient _httpClient;
        private readonly ILogger<BusinessTripBackendService> _logger;

        private const string API_TOKEN = "b165d8c4be5500d4da61df5067fd34ad";
        private const string ORIGIN_IATA = "LED";

        public BusinessTripBackendService(
            ICurrentObjectContextProvider context,
            ILogger<BusinessTripBackendService> logger)
        {
            _contextProvider = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpClient = new HttpClient();
        }

        private ObjectContext GetObjectContext()
        {
            var sessionContext = _contextProvider.GetOrCreateCurrentSessionContext();
            return sessionContext.ObjectContext;
        }
        public void FillFieldsOnCreate(Document card, SessionContext sessionContext)
        {
            _logger.LogInformation("Filling fields on create for card {CardId}", card.GetObjectId());
            try
            {
                var context = GetObjectContext();
                var currentUser = sessionContext.UserInfo.EmployeeId;

                var travelingEmployee = context.GetObject<StaffEmployee>(currentUser);
                if (travelingEmployee != null)
                {
                    card.MainInfo["TravelingEmployee"] = travelingEmployee.GetObjectId();
                    UpdateFieldsBasedOnEmployee(card, travelingEmployee);
                }
                else
                {
                    _logger.LogWarning("Traveling employee not found for user {UserId}", currentUser);
                }
                var secretary = FindFirstActiveEmployeeInGroup(sessionContext, "секретарь", context);
                if (secretary != null)
                {
                    card.MainInfo["ResponsibleEmployee"] = secretary.GetObjectId();
                }
                else
                {
                    _logger.LogWarning("Secretary not found in group");
                }

                FillApprovers(card, sessionContext, context);
                context.AcceptChanges();
                _logger.LogInformation("Successfully filled fields on create for card {CardId}", card.GetObjectId());

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error filling fields on create for card {CardId}", card.GetObjectId());
                throw;
            }
        }

        public void UpdateFieldsOnEmployeeChange(Document card, string employeeAccount)
        {
            _logger.LogInformation("Updating fields on employee change for card {CardId} with account {Account}",
                card.GetObjectId(), employeeAccount);
            try
            {
                var context = GetObjectContext();
                var travelingEmployee = FindEmployeeByAccount(employeeAccount, context);
                if (travelingEmployee != null)
                {
                    card.MainInfo["TravelingEmployee"] = travelingEmployee.GetObjectId();
                    UpdateFieldsBasedOnEmployee(card, travelingEmployee);
                    context.AcceptChanges();
                }
                else
                {
                    _logger.LogWarning("Employee with account {Account} not found", employeeAccount);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating fields on employee change for card {CardId}", card.GetObjectId());
                throw;
            }
        }

        public void UpdateFieldsOnCityChange(Document card, string cityName, DateTime dateFrom, DateTime dateTo)
        {
            _logger.LogInformation("Updating fields on city change for card {CardId} with city {City}, dates: {DateFrom} - {DateTo}",
                card.GetObjectId(), cityName, dateFrom, dateTo);
            try
            {
                var context = GetObjectContext();
                var cityItem = GetUniversalItemByName("Города", cityName, context);
                if (cityItem != null)
                {
                    card.MainInfo["City"] = cityItem.GetObjectId();

                    var dsa = (decimal)cityItem.ItemCard.MainInfo["DailySubsistenceAllowance"];

                    if (decimal.TryParse(dsa.ToString(), out decimal dailyAllowance))
                    {
                        int numberOfDays = (int)(dateTo - dateFrom).TotalDays + 1;
                        decimal totalAllowance = dailyAllowance * numberOfDays;
                        card.MainInfo["TripAllowanceAmount"] = totalAllowance;
                    }
                    else
                    {
                        _logger.LogWarning("Could not parse DailySubsistenceAllowance for city {City}", cityName);
                    }
                }
                else
                {
                    _logger.LogWarning("City {City} not found in dictionary", cityName);
                }
                context.AcceptChanges();
                _logger.LogInformation("Successfully updated fields for card {CardId}", card.GetObjectId());

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating fields on city change for card {CardId}", card.GetObjectId());
                throw;
            }
        }

        private void UpdateFieldsBasedOnEmployee(Document card, StaffEmployee travelingEmployee)
        {
            var context = GetObjectContext();
            var manager = FindEmployeeManager(travelingEmployee, context);
            if (manager != null)
            {
                card.MainInfo["Manager"] = manager.GetObjectId();
                card.MainInfo["PhoneNumber"] = manager.Phone;
            }
            else
            {
                _logger.LogWarning("Manager not found for employee {EmployeeAccount}", travelingEmployee.AccountName);
            }

            PartnersCompany? organization = FindPartnersCompanyByName(travelingEmployee, context);
            if (organization != null)
            {
                card.MainInfo["OrganizationName"] = organization.GetObjectId();
            }
            else
            {
                _logger.LogWarning("Organization not found for employee {EmployeeAccount}", travelingEmployee.AccountName);
            }
        }

        public async Task<List<FlightOptionModel>> GetTicketPrices(string cityName, DateTime dateFrom, DateTime dateTo)
        {
            _logger.LogInformation("Getting ticket prices for city {City}, dates: {DateFrom} - {DateTo}",
               cityName, dateFrom, dateTo);

            try
            {
                var context = GetObjectContext();

                string destinationIata = GetCityIataCode(cityName, context);
                if (string.IsNullOrEmpty(destinationIata))
                {
                    _logger.LogWarning("IATA code not found for city {City}", cityName);
                    return new List<FlightOptionModel>();
                }

                var url = BuildTravelPayoutsUrl(destinationIata, dateFrom, dateTo);

                var apiResponse = await ExecuteTravelPayoutsRequest(url);

                if (apiResponse.Data == null)
                {
                    _logger.LogInformation("No flight options found for {City}", cityName);
                    return new List<FlightOptionModel>();
                }

                var flightOptions = MapToFlightOptions(apiResponse.Data);

                _logger.LogInformation("Found {Count} flight options for {City}",
                    flightOptions.Count, cityName);
                return flightOptions.OrderBy(f => f.Price).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching ticket prices for city {City}", cityName);
                throw;
            }

        }

        #region Helper Methods
        private string? GetCityIataCode(string cityName, ObjectContext context)
        {
            var cityItem = GetUniversalItemByName("Города", cityName, context);

            if (cityItem != null)
            {
                return cityItem.ItemCard.MainInfo["IATA"]?.ToString();
            }
            return null;
        }

        private string BuildTravelPayoutsUrl(string destinationIata, DateTime dateFrom, DateTime dateTo)
        {
            return $"https://api.travelpayouts.com/aviasales/v3/prices_for_dates?" +
                      $"origin={ORIGIN_IATA}&destination={destinationIata}" +
                      $"&departure_at={dateFrom:yyyy-MM-dd}" +
                      $"&return_at={dateTo:yyyy-MM-dd}" +
                      $"&unique=false&sorting=price&direct=true&currency=rub&limit=10&page=1&one_way=false" +
                      $"&token={API_TOKEN}";
        }

        private List<FlightOptionModel> MapToFlightOptions(List<TravelPayoutsFlightModel> flights)
        {
            return flights
                .Select(flight => new FlightOptionModel
                {
                    Airline = flight.Airline,
                    FlightNumber = flight.FlightNumber,
                    Price = flight.Price,
                    DepartureAt = flight.DepartureAt,
                    ReturnAt = flight.ReturnAt,
                    OriginAirport = flight.OriginAirport,
                    DestinationAirport = flight.DestinationAirport,
                    Transfers = flight.Transfers,
                    ReturnTransfers = flight.ReturnTransfers,
                    DurationTo = flight.DurationTo,
                    DurationBack = flight.DurationBack,
                    Link = "https://www.aviasales.ru" + flight.Link
                })
                .GroupBy(f => new
                {
                    f.FlightNumber,
                    f.DepartureAt,
                    f.ReturnAt,
                    f.Airline
                })
                .Select(group => group.OrderBy(f => f.Price).First())
                .OrderBy(f => f.Price)
                .ToList();
        }

        private async Task<TravelPayoutsResponseModel> ExecuteTravelPayoutsRequest(string url)
        {
            var httpResponse = await _httpClient.GetAsync(url);
            var responseContent = await httpResponse.Content.ReadAsStringAsync();

            if (!httpResponse.IsSuccessStatusCode)
            {
                _logger.LogError("External API returned HTTP {StatusCode}. Response: {Response}",
                    httpResponse.StatusCode, responseContent);
                try
                {
                    var errorResponse = JsonSerializer.Deserialize<TravelPayoutsResponseModel>(responseContent);
                    if (!string.IsNullOrEmpty(errorResponse?.Error))
                    {
                        throw new Exception($"External API error: {errorResponse.Error}");
                    }
                }
                catch (JsonException)
                {
                    throw new Exception($"External API returned HTTP {httpResponse.StatusCode}, but error message could not be parsed.");
                }
                httpResponse.EnsureSuccessStatusCode();
            }
            var apiResponse = JsonSerializer.Deserialize<TravelPayoutsResponseModel>(responseContent);
            if (apiResponse?.Success != true || apiResponse.Data == null)
            {
                var errorMsg = $"Failed to get flight options: {apiResponse?.Error ?? "Unknown error"}";
                _logger.LogError(errorMsg);
                throw new Exception(errorMsg);
            }
            return apiResponse;
        }

        private void FillApprovers(Document card, SessionContext sessionContext, ObjectContext context)
        {
            IStaffService staffService = context.GetService<IStaffService>();
            var travelingEmployeeAccount = card.MainInfo["TravelingEmployee"]?.ToString();
            if (string.IsNullOrEmpty(travelingEmployeeAccount))
            {
                _logger.LogWarning("TravelingEmployee not set, cannot fill approvers");
                return;
            }

            var travelingEmployee = context.GetObject<StaffEmployee>(new Guid(travelingEmployeeAccount));
            if (travelingEmployee == null)
            {
                _logger.LogWarning("TravelingEmployee {EmployeeId} not found", travelingEmployeeAccount);
                return;
            }

            var approversSection = card.GetSection(CardDocument.Approvers.ID) as IList<BaseCardSectionRow>;

            StaffEmployee? approver = null;
            bool isDepartmentHead = IsDepartmentHead(travelingEmployee, context);
            if (isDepartmentHead)
            {
                approver = FindDirector(travelingEmployee);
                _logger.LogDebug("Employee is department head, director will be approver");
            }
            else
            {
                approver = staffService.GetEmployeeManager(travelingEmployee);
                _logger.LogDebug("Manager will be approver");
            }

            if (approver != null)
            {
                var approverRow = new BaseCardSectionRow();
                approverRow[CardDocument.Approvers.Approver] = approver.GetObjectId();
                approversSection.Add(approverRow);            }
            else
            {
                _logger.LogWarning("Approver not found for employee {EmployeeId}", travelingEmployee.GetObjectId());
            }
        }

        private StaffEmployee? FindEmployeeByAccount(string accountName, ObjectContext context)
        {
            IStaffService staffService = context.GetService<IStaffService>();
            return staffService.FindEmpoyeeByAccountName(accountName);
        }

        private StaffEmployee? FindEmployeeManager(StaffEmployee employee, ObjectContext context)
        {
            IStaffService staffService = context.GetService<IStaffService>();
            return staffService.GetEmployeeManager(employee);
        }

        private BaseUniversalItem? GetUniversalItemByName(string dictionaryName, string itemName, ObjectContext context)
        {
            IBaseUniversalService universalService = context.GetService<IBaseUniversalService>();
            BaseUniversalItemType dictionaryType = universalService.FindItemTypeWithSameName(dictionaryName, null);
            if (dictionaryType == null)
            {
                _logger.LogWarning("Dictionary {Dictionary} not found", dictionaryName);
                return null;
            }
            return universalService.FindItemWithSameName(itemName, dictionaryType);
        }

        private StaffEmployee? FindFirstActiveEmployeeInGroup(SessionContext sessionContext, string groupName, ObjectContext context)
        {
            IStaffService staffService = context.GetService<IStaffService>();
            var secretaryGroup = staffService.FindGroupByName(null, "Секретарь");
            if (secretaryGroup == null)
            {
                _logger.LogWarning("Group {GroupName} not found", groupName);
                return null;
            }
            var employeeList = secretaryGroup.Employees;
            return employeeList.FirstOrDefault(e => e.Status == StaffEmployeeStatus.Active);
        }

        private StaffEmployee? FindDirector(StaffEmployee employee)
        {
            var topLevelEmployeeUnit = GetOrganization(employee);

            return topLevelEmployeeUnit.Manager;
        }

        private bool IsDepartmentHead(StaffEmployee employee, ObjectContext context)
        {
            IStaffService staffService = context.GetService<IStaffService>();
            if (staffService.GetEmployeeManager(employee) == employee)
            return true;

            return false;
        }

        public static StaffUnit GetOrganization(StaffEmployee employee)
        {
            StaffUnit organization = employee.Unit;

            while (organization.ParentUnit != null &&
                   organization.ParentUnit.Type != StaffUnitType.Organization)
            {
                organization = organization.ParentUnit;
            }
            return organization;
        }

        private PartnersCompany? FindPartnersCompanyByName(StaffEmployee employee, ObjectContext context)
        {
            IPartnersService partnersService = context.GetService<IPartnersService>();
            var topLevelEmployeeUnit = GetOrganization(employee);

            var targetCompany = partnersService.FindSameCompanyOnServer(null, topLevelEmployeeUnit.Name, "");
            return targetCompany;
        }
        #endregion
    }
}
