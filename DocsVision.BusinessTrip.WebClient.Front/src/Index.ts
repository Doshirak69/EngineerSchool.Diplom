import { ExtensionManager, extensionManager } from "@docsvision/webclient/System/ExtensionManager";
import { Service } from "@docsvision/webclient/System/Service";
import * as $RequestManager from "@docsvision/webclient/System/$RequestManager";
import { $BusinessTripFrontendService, BusinessTripFrontendService } from "./Services/$BusinessTripFrontendService";
//import { IEmployeeChangeRequest } from "./Models/IEmployeeChangeRequest";
import { FlightTicketRequestControl } from "./Control/RequestTicketsPriceControl/RequestTicketsPriceControl";
import { $CardInfo } from "@docsvision/webclient/System/LayoutServices";

extensionManager.registerExtension({
    name: "BusinessTripExtension",
    version: "1.0.0",
    globalEventHandlers: [ ],
    layoutServices: [
        Service.fromFactory(
            $BusinessTripFrontendService,
            (services: $RequestManager.$RequestManager) => new BusinessTripFrontendService(services)
        )
    ],
    controls: [
        { controlTypeName: "GetRequestTicketPriceControl", constructor: FlightTicketRequestControl }
    ]

})

