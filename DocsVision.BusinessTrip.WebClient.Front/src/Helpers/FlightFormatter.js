import { DateFormatter } from "./DateFormatter";
var FlightFormatter = /** @class */ (function () {
    function FlightFormatter() {
    }
    FlightFormatter.toDisplayName = function (flight) {
        if (!flight)
            return "—";
        var departureShort = DateFormatter.toDisplayDateTime(flight.departureAt);
        var returnShort = DateFormatter.toDisplayDateTime(flight.returnAt);
        var airline = flight.airline || "—";
        var flightNumber = flight.flightNumber || "—";
        return "".concat(airline, " ").concat(flightNumber, " (").concat(departureShort, " \u21C4 ").concat(returnShort, ")");
    };
    FlightFormatter.formatRoute = function (flight) {
        if (!flight)
            return "—";
        var origin = flight.originAirport || "—";
        var destination = flight.destinationAirport || "—";
        return "".concat(origin, " \u21C4 ").concat(destination);
    };
    FlightFormatter.formatPrice = function (price, currency) {
        if (currency === void 0) { currency = "₽"; }
        if (typeof price !== 'number' || isNaN(price)) {
            return "—";
        }
        var formatted = price.toLocaleString('ru-RU');
        return "".concat(formatted, " ").concat(currency);
    };
    FlightFormatter.formatDepartureWithDuration = function (departureTime, durationMinutes) {
        var timeStr = DateFormatter.toDisplayDateTime(departureTime);
        var duration = DateFormatter.formatDuration(durationMinutes);
        return "".concat(timeStr, " (").concat(duration, ")");
    };
    FlightFormatter.getFlightCardData = function (flight) {
        return {
            price: this.formatPrice(flight.price),
            airline: flight.airline || "—",
            route: this.formatRoute(flight),
            departureTo: this.formatDepartureWithDuration(flight.departureAt, flight.durationTo),
            departureBack: this.formatDepartureWithDuration(flight.returnAt, flight.durationBack),
            link: flight.link
        };
    };
    return FlightFormatter;
}());
export { FlightFormatter };
//# sourceMappingURL=FlightFormatter.js.map