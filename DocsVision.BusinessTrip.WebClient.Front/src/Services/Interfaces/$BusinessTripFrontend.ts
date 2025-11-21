import { IEmployeeChangeRequest } from "../../Models/IEmployeeChangeRequest";
import { IFlightOption } from "../../Models/IFlightOption";
import { IBusinessTripRequest } from "../../Models/IBusinessTripRequest";

export interface IBusinessTripFrontendService {
    updateFieldsOnEmployeeChange(request: IEmployeeChangeRequest): Promise<void>;
    updateFieldsOnCityChange(request: IBusinessTripRequest): Promise<void>;
    getEmployeeAccountById(employeeId: string): Promise<string | null>;
    getTicketPrices(request: IBusinessTripRequest): Promise<IFlightOption[]>;
}
