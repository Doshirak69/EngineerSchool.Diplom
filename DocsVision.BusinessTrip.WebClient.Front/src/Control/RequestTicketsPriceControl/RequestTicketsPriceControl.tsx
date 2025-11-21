import { ButtonAlignModes } from "@docsvision/web/components/form/button";
import { BaseControl, BaseControlParams, BaseControlState } from "@docsvision/webclient/System/BaseControl";
import { ControlImpl } from "@docsvision/webclient/System/ControlImpl";
import { Layout } from "@docsvision/webclient/System/Layout";
import { r } from "@docsvision/webclient/System/Readonly";
import { rw } from "@docsvision/webclient/System/Readwrite";
import React from "react";
import { IFlightOption } from "../../Models/IFlightOption";
import { $BusinessTripFrontendService } from "../../Services/$BusinessTripFrontendService";
import { Button } from "@docsvision/webclient/Helpers/Button";
import { DropdownButton, IDropdownButtonListItem } from "@docsvision/webclient/Helpers/DropdownButton";
import { $CardInfo } from "@docsvision/webclient/System/LayoutServices";
import { $ControlStore } from "@docsvision/webclient/System/LayoutServices";
import { DateTimePicker } from "@docsvision/webclient/Platform/DateTimePicker";
import { DirectoryDesignerRow } from "@docsvision/webclient/BackOffice/DirectoryDesignerRow";
import { FlightTicketRequestLogic } from "../../Logics/FlightTicketRequestLogic";
import { FlightFormatter } from "../../Helpers/FlightFormatter";

export class FlightTicketRequestControlParams extends BaseControlParams {
    @r standardCssClass?: string = "flightTicketRequestControl";
    @r layout?: Layout;
}

export interface FlightTicketRequestControlState extends FlightTicketRequestControlParams, BaseControlState {
    isResultsVisible: boolean;
    isLoading: boolean;
    flightOptions: IFlightOption[];
    selectedFlight: IFlightOption | null;
    errorMessage: string | null;
    isListOpen: boolean;
}

export class FlightTicketRequestControl extends BaseControl<
    FlightTicketRequestControlParams,
    FlightTicketRequestControlState
> {
    construct() {
        super.construct();
        this.initializeState();
    }

    private initializeState() {
        this.state.isResultsVisible = false;
        this.state.isLoading = false;
        this.state.flightOptions = [];
        this.state.selectedFlight = null;
        this.state.errorMessage = null;
        this.state.isListOpen = false;
    }

    protected createParams() {
        return new FlightTicketRequestControlParams();
    }

    protected createImpl() {
        return new ControlImpl(
            this.props,
            this.state,
            this.renderControl.bind(this));
    }

    private async handleFetchTicketPrices() {
        console.log("Fetching ticket prices initiated");
        const { layout } = this.layout;

        try {
            FlightTicketRequestLogic.validateLayout(layout);

            const cardInfo = layout.getService($CardInfo);
            const controlStore = layout.getService($ControlStore);

            FlightTicketRequestLogic.validateCardInfo(cardInfo);

            this.resetState();

            const request = FlightTicketRequestLogic.extractRequestData(controlStore, cardInfo);
            const frontend = layout.getService($BusinessTripFrontendService);
            const options = await frontend.getTicketPrices(request);

            this.setState({
                flightOptions: options,
                isResultsVisible: true,
                isLoading: false,
                errorMessage: null
            });

        } catch (error) {
            console.error("Failed to fetch ticket prices:", error);
            this.handleError(error);
        }
    }

    private handleFlightSelect(flight: IFlightOption) {
        console.log("Flight option selected:", flight.flightNumber);
        this.setState({
            selectedFlight: flight,
            isListOpen: false
        });
    }

    private handleError(error: unknown) {
        const message = error instanceof Error ? error.message : String(error);
        console.error("Error fetching ticket prices:", error);

        this.setState({
            isLoading: false,
            isResultsVisible: true,
            errorMessage: message
        });
    }

    private resetState() {
        this.setState({
            isLoading: true,
            isResultsVisible: false,
            flightOptions: [],
            selectedFlight: null,
            errorMessage: null
        });
    }

    private createFlightListItems(): IDropdownButtonListItem[] {
        return this.state.flightOptions.map(opt => ({
            key: FlightFormatter.toDisplayName(opt),
            content: FlightFormatter.toDisplayName(opt),
            onClick: () => this.handleFlightSelect(opt)
        }));
    }

    private renderButton() {
        return (
            <Button
                text="Запросить стоимость билетов"
                align={ButtonAlignModes.Center}
                onClick={() => this.handleFetchTicketPrices()}
                loading={this.state.isLoading}
                className="system-card-button"
            />
        );
    }

    private renderError() {
        if (!this.state.errorMessage) return null;

        return (
            <div style={{
                color: 'red',
                marginBottom: '10px',
                fontWeight: 'bold'
            }}>
                Ошибка: {this.state.errorMessage}
            </div>
        );
    }

    private renderFlightDropdown() {
        const listItems = this.createFlightListItems();
        const buttonText = this.state.selectedFlight
            ? FlightFormatter.toDisplayName(this.state.selectedFlight)
            : "Выберите вариант перелета";

        return (
            <div style={{ position: 'relative', zIndex: 1000 }}>
                <DropdownButton
                    buttonText={buttonText}
                    list={listItems}
                    isOpen={this.state.isListOpen}
                    onCloseList={() => this.setState({ isListOpen: false })}
                    onToggleList={() => this.setState({ isListOpen: !this.state.isListOpen })}
                />
            </div>
        );
    }

    private renderSelectedFlightDetails() {
        if (!this.state.selectedFlight) return null;

        const flightData = FlightFormatter.getFlightCardData(this.state.selectedFlight);

        return (
            <div style={{
                marginTop: '15px',
                padding: '15px',
                border: '1px solid #ddd',
                borderRadius: '5px',
                backgroundColor: '#f9f9f9',
                position: 'relative',
                zIndex: 1
            }}>
                <div style={{ marginBottom: '10px' }}>
                    <strong>Цена:</strong> {flightData.price}
                </div>
                <div style={{ marginBottom: '10px' }}>
                    <strong>Авиакомпания:</strong> {flightData.airline}
                </div>
                <div style={{ marginBottom: '10px' }}>
                    <strong>Маршрут:</strong> {flightData.route}
                </div>
                <div style={{ marginBottom: '10px' }}>
                    <strong>Вылет туда:</strong> {flightData.departureTo}
                </div>
                <div style={{ marginBottom: '10px' }}>
                    <strong>Вылет обратно:</strong> {flightData.departureBack}
                </div>
                <div style={{ marginBottom: '10px', textAlign: 'right' }}>
                    <a
                        href={flightData.link}
                        target="_blank"
                        rel="noopener noreferrer"
                        style={{
                            color: '#0066cc',
                            textDecoration: 'none'
                        }}
                    >
                        <strong>Забронировать</strong>
                    </a>
                </div>
            </div>
        );
    }

    private renderResults() {
        if (!this.state.isResultsVisible) return null;

        const hasError = !!this.state.errorMessage;

        return (
            <div style={{
                border: '1px solid #ccc',
                padding: '10px',
                marginTop: '10px',
                backgroundColor: hasError ? '#ffe6e6' : 'inherit'
            }}>
                {this.renderError()}

                {!hasError && this.state.flightOptions.length > 0 && (
                    <div>
                        {this.renderFlightDropdown()}
                        {this.renderSelectedFlightDetails()}
                    </div>
                )}

                {!hasError && this.state.flightOptions.length === 0 && (
                    <div style={{ textAlign: 'center' }}>
                        Варианты перелета не найдены.
                    </div>
                )}
            </div>
        );
    }

    renderControl() {
        return (
            <div style={{ marginBottom: '15px' }}>
                {this.renderButton()}
                {this.renderResults()}
            </div>
        );
    }
}