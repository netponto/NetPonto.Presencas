using System.Web.Mvc;
using MvcApplication1.Helpers;
using MvcApplication1.Services;
using ServiceStack.CacheAccess;
using ServiceStack.CacheAccess.Providers;
using ServiceStack.Common.Web;
using ServiceStack.Mvc;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.Auth;
using ServiceStack.WebHost.Endpoints;

[assembly: WebActivator.PreApplicationStartMethod(typeof(MvcApplication1.App_Start.AppHost), "Start")]

//IMPORTANT: Add the line below to MvcApplication.RegisterRoutes(RouteCollection) in the Global.asax:
//routes.IgnoreRoute("api/{*pathInfo}"); 

/**
 * Entire ServiceStack Starter Template configured with a 'Hello' Web Service and a 'Todo' Rest Service.
 *
 * Auto-Generated Metadata API page at: /metadata
 * See other complete web service examples at: https://github.com/ServiceStack/ServiceStack.Examples
 */

namespace MvcApplication1.App_Start
{
	//A customizeable typed UserSession that can be extended with your own properties
	//To access ServiceStack's Session, Cache, etc from MVC Controllers inherit from ControllerBase<CustomUserSession>
	public class CustomUserSession : AuthUserSession
	{
		public string CustomProperty { get; set; }
	}

	public class AppHost
		: AppHostBase
	{		
		public AppHost() //Tell ServiceStack the name and where to find your web services
			: base("StarterTemplate ASP.NET Host", typeof(EventAttendeesService).Assembly) { }

		public override void Configure(Funq.Container container)
		{
			//Set JSON web services to return idiomatic JSON camelCase properties
			ServiceStack.Text.JsConfig.EmitCamelCaseNames = true;

		    //Configure User Defined REST Paths
		    Routes
		        .Add<EventAttendees>("/eventattendees")
                .Add<ExportEventEvaluations>("/eventevaluations/{EventId}/export")
                .Add<GetEventEvaluations>("/eventevaluations/{EventId}")
                .Add<ListEventEvaluations>("/listeventevaluations")
		        .Add<EventEvaluations>("/eventevaluations")
                .Add<ExportEventAttendees>("/exporteventattendees/{EventId}")
                .Add<LoadSyncEvents>("/loadsyncevents//{EventId}")
		        ;


		    ContentTypeFilters.Register(ExcelContentTypeHandler.ContentType, ExcelContentTypeHandler.VeryHackishExcelStreamSerializer,
		                                ExcelContentTypeHandler.NonWorkingStreamDeserializer);

		    ContentTypeFilters.Register(PlainTextContentTypeHandler.ContentType, PlainTextContentTypeHandler.ReallySimpleTextStreamSerializer,
		                                PlainTextContentTypeHandler.NonWorkingStreamDeserializer);
            
            ContentTypeFilters.Register(PlainTextContentTypeHandler.ApplicationContentType, PlainTextContentTypeHandler.ReallySimpleTextStreamSerializer,
		                                PlainTextContentTypeHandler.NonWorkingStreamDeserializer);

		    ResponseFilters.Add(AddFilenameToResponses);

		    //Change the default ServiceStack configuration
            SetConfig(new EndpointHostConfig
            {
                DebugMode = true, //Show StackTraces in responses in development
            });

			//Enable Authentication
			//ConfigureAuth(container);

			//Register all your dependencies
		
			//Register In-Memory Cache provider. 
			//For Distributed Cache Providers Use: PooledRedisClientManager, BasicRedisClientManager or see: https://github.com/ServiceStack/ServiceStack/wiki/Caching
			container.Register<ICacheClient>(new MemoryCacheClient());
			container.Register<ISessionFactory>(c => 
				new SessionFactory(c.Resolve<ICacheClient>()));

			//Set MVC to use the same Funq IOC as ServiceStack
			ControllerBuilder.Current.SetControllerFactory(new FunqControllerFactory(container));
		}

	    private void AddFilenameToResponses(IHttpRequest req, IHttpResponse res, object dto)
	    {
	        string filenameExtension = null;
	        switch (req.ResponseContentType)
	        {
	            case ExcelContentTypeHandler.ContentType:
	                filenameExtension = "xlsx";
	                break;
	            case PlainTextContentTypeHandler.ApplicationContentType:
	            case PlainTextContentTypeHandler.ContentType:
	                filenameExtension = "txt";
	                break;
	        }
	        if (filenameExtension != null)
	        {
	            res.AddHeader(HttpHeaders.ContentDisposition, string.Format("attachment;filename={0}.{1}", req.OperationName, filenameExtension));
	        }
	    }

	    public static void Start()
		{
			new AppHost().Init();
		}
	}
}