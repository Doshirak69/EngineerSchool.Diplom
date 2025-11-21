import { extensionManager } from "@docsvision/webclient/System/ExtensionManager";
import { Service } from "@docsvision/webclient/System/Service";
import { $BusinessTripFrontendService, BusinessTripFrontendService } from "./Services/$BusinessTripFrontendService";
//import { IEmployeeChangeRequest } from "./Models/IEmployeeChangeRequest";
//import { ICityChangeRequest } from "./Models/ICityChangeRequest";
import { FlightTicketRequestControl } from "./Control/RequestTicketsPriceControl/RequestTicketsPriceControl";
extensionManager.registerExtension({
    name: "BusinessTripExtension",
    version: "1.0.0",
    layoutServices: [
        Service.fromFactory($BusinessTripFrontendService, function (services) { return new BusinessTripFrontendService(services); })
    ],
    controls: [
        { controlTypeName: "GetRequestTicketPriceControl", constructor: FlightTicketRequestControl }
    ]
});
//# sourceMappingURL=Index.js.map