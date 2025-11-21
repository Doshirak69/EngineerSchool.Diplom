using System;
using System.Collections.Generic;
using DocsVision.BackOffice.ObjectModel;
using DocsVision.BusinessTrip.WebClient.Services.Interfaces;
using DocsVision.Platform.WebClient;
using DocsVision.WebClientLibrary.ObjectModel.Services.EntityLifeCycle;
using DocsVision.WebClientLibrary.ObjectModel.Services.EntityLifeCycle.Options;

namespace DocsVision.BusinessTrip.WebClient.CardLifeCycle
{
    public class BusinessTripCardLifeCycleDecorator: ICardLifeCycleEx

    {
        private readonly ICardLifeCycleEx _baseLifeCycle;
        private readonly ICurrentObjectContextProvider _contextProvider;
        private readonly IBusinessTripBackendService _backendService;

        public BusinessTripCardLifeCycleDecorator (
            ICardLifeCycleEx baseLifeCycle,
            ICurrentObjectContextProvider contextProvider,
            IBusinessTripBackendService backendService)
        {
            _baseLifeCycle = baseLifeCycle ?? throw new ArgumentNullException(nameof(baseLifeCycle));
            _contextProvider = contextProvider ?? throw new ArgumentNullException(nameof(contextProvider));
            _backendService = backendService ?? throw new ArgumentNullException(nameof(backendService));
        }

        public Guid CardTypeId => _baseLifeCycle.CardTypeId;

        public Guid Create(SessionContext sessionContext, CardCreateLifeCycleOptions options)
        {
            var cardId = _baseLifeCycle.Create(sessionContext, options);

            var objectContext = sessionContext.ObjectContext;
            var card = objectContext.GetObject<Document>(cardId);

            _backendService.FillFieldsOnCreate(card, sessionContext);

            return cardId;
        }

        public bool CanDelete(SessionContext sessionContext, CardDeleteLifeCycleOptions options, out string message)
        {
            return _baseLifeCycle.CanDelete(sessionContext, options, out message);
        }

        public void OnDelete(SessionContext sessionContext, CardDeleteLifeCycleOptions options)
            =>_baseLifeCycle.OnDelete(sessionContext, options);

        public void OnSave(SessionContext sessionContext, CardSaveLifeCycleOptions options)
            => _baseLifeCycle.OnSave(sessionContext, options);

        public bool Validate(SessionContext sessionContext, CardValidateLifeCycleOptions options, out List<ValidationResult> validationResults)
            => _baseLifeCycle.Validate(sessionContext, options,out validationResults);
        
        public string GetDigest(SessionContext sessionContext, CardDigestLifeCycleOptions options)
            => _baseLifeCycle.GetDigest(sessionContext, options);
        
    }
}
