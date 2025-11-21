import { $RequestManager } from "@docsvision/webclient/System/$RequestManager";
import { serviceName } from "@docsvision/web/core/services";
import { IEmployeeChangeRequest } from "../Models/IEmployeeChangeRequest";
import { IEmployeeAccountResponse } from "../Models/IEmployeeAccountResponse";
import { IBusinessTripFrontendService } from "./Interfaces/$BusinessTripFrontend";
import { IFlightOption } from "../Models/IFlightOption";
import { IBusinessTripRequest } from "../Models/IBusinessTripRequest";

export type $BusinessTripServiceKey = {businessTripFrontendService: IBusinessTripFrontendService;};

export const $BusinessTripFrontendService =
  serviceName((x:$BusinessTripServiceKey) => x.businessTripFrontendService);

export class BusinessTripFrontendService implements IBusinessTripFrontendService {

    constructor(private services: $RequestManager) { }

    // POST /api/BusinessTripApi/GetTicketPrices
    async getTicketPrices(request: IBusinessTripRequest): Promise<IFlightOption[]> {
        console.log("POST GetTicketPrices", request);
        try {
            const resp = await this.services.requestManager.post<IFlightOption[]>(
                "/api/BusinessTripApi/GetTicketPrices",
                JSON.stringify(request) 
            );
            console.log("Response GetTicketPrices:", resp);
            return resp;
         } catch (e) {
            console.error("getTicketPrices error:", e);
            let errorMessage = "Произошла неизвестная ошибка при запросе билетов.";
            if (e && e.Message) {
                errorMessage = e.Message;
            } else if (typeof e === 'string') {
                errorMessage = e;
            }

            throw new Error(errorMessage);
         }
    }

    // POST /api/BusinessTripApi/UpdateOnEmployeeChange
    async updateFieldsOnEmployeeChange(request: IEmployeeChangeRequest): Promise<void> {
       console.log("POST UpdateOnEmployeeChange", request);
       await this.services.requestManager.post("/api/BusinessTripApi/UpdateOnEmployeeChange", request);
       try {
           await this.services.requestManager.post(
               "/api/BusinessTripApi/UpdateOnEmployeeChange", 
               request
           );
           console.log("POST UpdateOnEmployeeChange completed successfully");
        } catch (e) {
           console.error("UpdateOnEmployeeChange request failed:", e);
           
           let userMessage = "Не удалось обновить поля руководителя и телефона.";
           
           if (e && e.Message) {
               userMessage = e.Message;
           } else if (typeof e === 'string') {
               userMessage = e;
           } else if (e instanceof Error) {
               userMessage = e.message;
           }
           
           throw new Error(userMessage);
        }
    }

    // POST /api/BusinessTripApi/UpdateOnCityChange
    async updateFieldsOnCityChange(request: IBusinessTripRequest): Promise<void> {
       console.log("POST UpdateOnCityChange", request);
       try {
           await this.services.requestManager.post(
               "/api/BusinessTripApi/UpdateOnCityChange", 
               request
           );
           console.log("POST UpdateOnCityChange completed successfully");
        } catch (e) {
           console.error("UpdateOnCityChange request failed:", e);
           
           let userMessage = "Не удалось обновить сумму командировочных.";
           
           if (e && e.Message) {
               userMessage = e.Message;
           } else if (typeof e === 'string') {
               userMessage = e;
           } else if (e instanceof Error) {
               userMessage = e.message;
           }
           
           throw new Error(userMessage);
        } 
    }

    // GET /api/BusinessTripApi/GetEmployeeAccount/{employeeId}
    async getEmployeeAccountById(employeeId: string): Promise<string | null> {
       console.log("GET GetEmployeeAccount", employeeId);
       try {
           const resp = await this.services.requestManager
               .get<IEmployeeAccountResponse>(`/api/BusinessTripApi/GetEmployeeAccount/${employeeId}`);
           
           console.log("GET GetEmployeeAccount response:", resp);
           return resp.accountName;
        } catch (e) {
           console.error("GetEmployeeAccount request failed:", e);
           
           let userMessage = "Не удалось получить данные сотрудника.";
           
           if (e && e.Message) {
               userMessage = e.Message;
           } else if (typeof e === 'string') {
               userMessage = e;
           } else if (e instanceof Error) {
               userMessage = e.message;
           }
           
           throw new Error(userMessage);
        }
    }
}