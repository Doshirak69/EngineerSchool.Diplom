using DocsVision.BusinessTrip.WebClient.CardLifeCycle;
using DocsVision.BusinessTrip.WebClient.Services;
using DocsVision.BusinessTrip.WebClient.Services.Interfaces;
using DocsVision.Platform.WebClient;
using DocsVision.WebClient.Extensibility;
using DocsVision.WebClientLibrary.ObjectModel.Services.EntityLifeCycle;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Resources;

namespace DocsVision.BusinessTrip.WebClient
{
    public class ServerExtension : WebClientExtension
    {
        public ServerExtension(IServiceProvider serviceProvider) : base()
        {
        }

        public override string ExtensionName => Assembly.GetAssembly(typeof(ServerExtension)).GetName().Name;

        public override Version ExtensionVersion => new Version(FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion);

        public override void InitializeServiceCollection(IServiceCollection services)
        {
            services.AddSingleton<IBusinessTripBackendService, BusinessTripBackendService>();

            services.Decorate<ICardLifeCycleEx>(static (original, serviceProvider) =>
            {
                var contextProvider = serviceProvider.GetRequiredService<ICurrentObjectContextProvider>();
                var backendService = serviceProvider.GetRequiredService<IBusinessTripBackendService>();
                return new BusinessTripCardLifeCycleDecorator(original, contextProvider, backendService);
            });
        }
      
        protected override List<ResourceManager> GetLayoutExtensionResourceManagers()
        {
            return new List<ResourceManager>();
        }
    }
}