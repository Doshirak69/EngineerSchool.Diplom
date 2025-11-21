export interface IFlightOption {
    airline: string;
    flightNumber: string;
    price: number;
    departureAt: string;
    returnAt: string;
    originAirport: string;
    destinationAirport: string;
    transfers: number;
    returnTransfers: number;
    durationTo: number;
    durationBack: number;
    link: string;
}
