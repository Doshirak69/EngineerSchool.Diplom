import { DateTimePicker } from "@docsvision/webclient/Platform/DateTimePicker";
import { DirectoryDesignerRow } from "@docsvision/webclient/BackOffice/DirectoryDesignerRow";
import { DateFormatter } from "../Helpers/DateFormatter";
import { FIELD_NAMES } from "../Helpers/Constants/FieldNames";

export interface IFlightRequestData {
    cardId: string;
    cityName: string;
    dateFrom: string;
    dateTo: string;    
}

export class FlightTicketRequestLogic {

    static extractRequestData(controlStore: any, cardInfo: any): IFlightRequestData {
        console.log("Extracting flight request data");

        const controls = this.getFormControls(controlStore);

        const cityValue = controls.cityControl.params.value as any;
        const dateFrom = controls.dateFromControl.params.value as Date | null;
        const dateTo = controls.dateToControl.params.value as Date | null;

        const cityName = this.extractCityName(cityValue);

        const requestData = {
            cardId: cardInfo.id,
            cityName: cityName!,
            dateFrom: DateFormatter.toAPIFormat(dateFrom!),
            dateTo: DateFormatter.toAPIFormat(dateTo!)
        };

        console.log("Flight request data extracted successfully:", requestData);

        return requestData;
    }

    private static getFormControls(controlStore: any) {
        const cityControl = controlStore.tryGet(
            FIELD_NAMES.TRIP_CITY_CTRL
        ) as DirectoryDesignerRow;

        const dateFromControl = controlStore.tryGet(
            FIELD_NAMES.DATE_FROM_CTRL
        )  as DateTimePicker;

        const dateToControl = controlStore.tryGet(
            FIELD_NAMES.DATE_TO_CTRL
        )  as DateTimePicker;

        if (!cityControl || !dateFromControl || !dateToControl) {
            console.error("Required controls not found on layout");
            throw new Error("Не найдены необходимые контролы на разметке. Проверьте имена полей.");
        }

        return { cityControl, dateFromControl, dateToControl };
    }

    private static extractCityName(cityValue: any): string | null {
        if (!cityValue) {
            console.log("City value is empty");
            return null;
        }
        return cityValue.displayName || cityValue.name || null;
    }

    private static validateFormData(
        cityName: string | null,
        dateFrom: Date | null,
        dateTo: Date | null
    ): void {
        if (!cityName || !dateFrom || !dateTo) {
            console.warn("Required fields are missing", { cityName, dateFrom, dateTo });
            throw new Error("Пожалуйста, заполните поля 'Город', 'Дата вылета' и 'Дата прилета'.");
        }

        if (!DateFormatter.isValidDate(dateFrom) || !DateFormatter.isValidDate(dateTo)) {
            console.warn("Invalid dates provided");
            throw new Error("Указаны некорректные даты");
        }

        if (dateFrom > dateTo) {
            console.warn("DateFrom is after DateTo");
            throw new Error("Дата вылета не может быть позже даты возвращения");
        }
    }

    static validateLayout(layout: any): void {
        if (!layout) {
            console.error("Layout is not available");
            throw new Error("Не удалось получить доступ к разметке.");
        }
    }

    static validateCardInfo(cardInfo: any): void {
        if (!cardInfo) {
            console.error("CardInfo is not available");
            throw new Error("Не удалось получить доступ к данным карточки.");
        }
    }
}