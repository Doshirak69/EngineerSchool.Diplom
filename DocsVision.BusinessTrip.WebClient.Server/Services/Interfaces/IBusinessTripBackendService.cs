using DocsVision.BackOffice.ObjectModel;
using DocsVision.Platform.WebClient;
using ServerExtension.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DocsVision.BusinessTrip.WebClient.Services.Interfaces
{
    public interface IBusinessTripBackendService
    {
        void FillFieldsOnCreate(Document card, SessionContext sessionContext);
        void UpdateFieldsOnEmployeeChange(Document card, string employeeAccount);
        void UpdateFieldsOnCityChange(Document card, string cityName, DateTime dateFrom, DateTime dateTo);
        Task<List<FlightOptionModel>> GetTicketPrices(string cityName, DateTime dateFrom, DateTime dateTo);
    }
}
