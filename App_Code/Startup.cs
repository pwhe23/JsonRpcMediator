using System.Web.Http;
using MediatR;
using Microsoft.AspNet.SignalR;
using Microsoft.Practices.ServiceLocation;
using Owin;
using TinyIoC;

public class Startup
{
    //OWIN
    public void Configuration(IAppBuilder app)
    {
        var config = new HubConfiguration();
        config.Resolver = new TinyIocSignalRDependencyResolver(TinyIoCContainer.Current);

        app.MapSignalR(config);
    }

    public static void Initialize()
    {
        RegisterIoc();
        GlobalConfiguration.Configure(RegisterWebApi);
    }

    private static void RegisterIoc()
    {
        var container = TinyIoCContainer.Current;
        container.AutoRegister();
        ServiceLocator.SetLocatorProvider(() => new TinyIocServiceLocator());
        container.Register<ServiceLocatorProvider>(() => ServiceLocator.Current);
        container.Register<JsonRpcController>().AsPerRequestSingleton();
        container.Register<IRequestHandler<Home, Home.Response>, Home.Handler>();
        container.Register<IRequestHandler<SaveOrder, bool>, SaveOrder.Handler>();
    }

    private static void RegisterWebApi(HttpConfiguration config)
    {
        config.DependencyResolver = new TinyIocWebApiDependencyResolver(TinyIoCContainer.Current);
        config.MapHttpAttributeRoutes();
        config.EnsureInitialized();
    }
};
