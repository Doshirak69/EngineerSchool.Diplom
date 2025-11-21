import { DateFormatter } from "../../../Helpers/DateFormatter";
var FlightTicketRequestLogic = /** @class */ (function () {
    function FlightTicketRequestLogic() {
    }
    FlightTicketRequestLogic.extractRequestData = function (controlStore, cardInfo) {
        var controls = this.getFormControls(controlStore);
        var cityValue = controls.cityControl.params.value;
        var dateFrom = controls.dateFromControl.params.value;
        var dateTo = controls.dateToControl.params.value;
        var cityName = this.extractCityName(cityValue);
        this.validateFormData(cityName, dateFrom, dateTo);
        return {
            cardId: cardInfo.id,
            cityName: cityName,
            dateFrom: DateFormatter.toAPIFormat(dateFrom),
            dateTo: DateFormatter.toAPIFormat(dateTo)
        };
    };
    FlightTicketRequestLogic.getFormControls = function (controlStore) {
        var cityControl = controlStore.tryGet(FLIGHT_REQUEST_FIELD_NAMES.CITY);
        var dateFromControl = controlStore.tryGet(FLIGHT_REQUEST_FIELD_NAMES.DATE_FROM);
        var dateToControl = controlStore.tryGet(FLIGHT_REQUEST_FIELD_NAMES.DATE_TO);
        if (!cityControl || !dateFromControl || !dateToControl) {
            throw new Error("Не найдены необходимые контролы на разметке. Проверьте имена полей.");
        }
        return { cityControl: cityControl, dateFromControl: dateFromControl, dateToControl: dateToControl };
    };
    FlightTicketRequestLogic.extractCityName = function (cityValue) {
        if (!cityValue)
            return null;
        return cityValue.displayName || cityValue.name || null;
    };
    FlightTicketRequestLogic.validateFormData = function (cityName, dateFrom, dateTo) {
        if (!cityName || !dateFrom || !dateTo) {
            throw new Error("Пожалуйста, заполните поля 'Город', 'Дата вылета' и 'Дата прилета'.");
        }
        if (!DateFormatter.isValidDate(dateFrom) || !DateFormatter.isValidDate(dateTo)) {
            throw new Error("Указаны некорректные даты");
        }
        if (dateFrom > dateTo) {
            throw new Error("Дата вылета не может быть позже даты возвращения");
        }
    };
    FlightTicketRequestLogic.validateLayout = function (layout) {
        if (!layout) {
            throw new Error("Не удалось получить доступ к разметке.");
        }
    };
    FlightTicketRequestLogic.validateCardInfo = function (cardInfo) {
        if (!cardInfo) {
            throw new Error("Не удалось получить доступ к данным карточки.");
        }
    };
    return FlightTicketRequestLogic;
}());
export { FlightTicketRequestLogic };
//# sourceMappingURL=FlightTicketRequestLogic.js.map