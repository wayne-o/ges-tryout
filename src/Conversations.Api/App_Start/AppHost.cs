using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Conversations.Api.App_Start;
using Funq;
using ServiceStack;
using ServiceStack.Text;
using ServiceStack.Web;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(AppHost), "Start")]
namespace Conversations.Api.App_Start
{
    using Conversations.Installers;

    using Raven.Client;
    using Raven.Client.Document;

    using AppSettings = Conversations.Configuration.AppSettings;

    public class AppHost : AppHostBase
    {
        public AppHost()
            : base("Sonatribe Conversations Api wikiwkikiwah", typeof(ServiceInterface.ConversationsService).Assembly)
        {
        }

        public override void Configure(Container container)
        {
            JsConfig.EmitCamelCaseNames = true;
            JsConfig.DateHandler = DateHandler.ISO8601;
            container.Adapter = new WindsorContainerAdapter();
        }

        public override void OnUncaughtException(IRequest httpReq, IResponse httpRes, string operationName, Exception ex)
        {
            base.OnUncaughtException(httpReq, httpRes, operationName, ex);
        }

        public override object OnServiceException(IRequest httpReq, object request, Exception ex)
        {
            return base.OnServiceException(httpReq, request, ex);
        }

        public static void Start()
        {
            var appHost = new AppHost();
            appHost.Init();
        }
    }
}