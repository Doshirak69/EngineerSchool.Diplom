import { IFlightOption } from "../Models/IFlightOption";
import { DateFormatter } from "./DateFormatter";

export class FlightFormatter {
    static toDisplayName(flight: IFlightOption): string {
        if (!flight) return "—";

        const departureShort = DateFormatter.toDisplayDateTime(flight.departureAt);
        const returnShort = DateFormatter.toDisplayDateTime(flight.returnAt);
        const airline = flight.airline || "—";
        const flightNumber = flight.flightNumber || "—";

        return `${airline} ${flightNumber} (${departureShort} ⇌ ${returnShort})`;
    }

    static formatRoute(flight: IFlightOption): string {
        if (!flight) return "—";

        const origin = flight.originAirport || "—";
        const destination = flight.destinationAirport || "—";

        return `${origin} ⇄ ${destination}`;
    }

    static formatPrice(price: number, currency: string = "₽"): string {
        if (typeof price !== 'number' || isNaN(price)) {
            return "—";
        }

        const formatted = price.toLocaleString('ru-RU');

        return `${formatted} ${currency}`;
    }

    static formatDepartureWithDuration(
        departureTime: string | Date,
        durationMinutes: number
    ): string {
        const timeStr = DateFormatter.toDisplayDateTime(departureTime);
        const duration = DateFormatter.formatDuration(durationMinutes);

        return `${timeStr} (${duration})`;
    }

    static getFlightCardData(flight: IFlightOption) {
        return {
            price: this.formatPrice(flight.price),
            airline: flight.airline || "—",
            route: this.formatRoute(flight),
            departureTo: this.formatDepartureWithDuration(
                flight.departureAt,
                flight.durationTo
            ),
            departureBack: this.formatDepartureWithDuration(
                flight.returnAt,
                flight.durationBack
            ),
            link: flight.link
        };
    }
}