using System.Web.Http;
using Microsoft.AspNet.SignalR;
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
        container.Register<JsonRpcController>().AsPerRequestSingleton();
    }

    private static void RegisterWebApi(HttpConfiguration config)
    {
        config.DependencyResolver = new TinyIocWebApiDependencyResolver(TinyIoCContainer.Current);
        config.MapHttpAttributeRoutes();
        config.EnsureInitialized();
    }
};
