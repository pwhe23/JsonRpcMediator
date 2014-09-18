using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Security.Authentication;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using MediatR;
using Microsoft.Practices.ServiceLocation;
using StructureMap;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;
using StructureMap.Web.Pipeline;

public class Startup
{
    private static IContainer _globalContainer;

    public static void Initialize()
    {
        _globalContainer = RegisterIoc();
        GlobalConfiguration.Configure(RegisterWebApi);
    }

    private static IContainer RegisterIoc()
    {
        var container = new Container(cfg => cfg.Scan(scan =>
        {
            scan.TheCallingAssembly();
            scan.LookForRegistries();
        }));

        var serviceLocator = new StructureMapServiceLocator(container);
        var serviceLocatorProvider = new ServiceLocatorProvider(() => serviceLocator);
        container.Configure(cfg => cfg.For<ServiceLocatorProvider>().Use(serviceLocatorProvider));

        return container;
    }

    private static void RegisterWebApi(HttpConfiguration config)
    {
        config.Services.Replace(typeof(IHttpControllerActivator), new StructureMapServiceActivator(_globalContainer));
        config.MapHttpAttributeRoutes();
        config.EnsureInitialized();
    }

    public class WebRegistry : Registry
    {
        public WebRegistry()
        {
            Scan(scan =>
            {
                scan.TheCallingAssembly();
                scan.AssemblyContainingType<IMediator>();
                scan.AddAllTypesOf(typeof(IRequestHandler<,>));
                scan.AddAllTypesOf(typeof(INotificationHandler<>));
                scan.WithDefaultConventions();
            });

            Scan(scan =>
            {
                scan.TheCallingAssembly();
                scan.AddAllTypesOf<IHttpController>();
                scan.WithDefaultConventions().OnAddedPluginTypes(x => x.LifecycleIs(new HttpContextLifecycle()));
            });

            var handlerType = For(typeof(IRequestHandler<,>));
            handlerType.DecorateAllWith(typeof(AuthorizationHandler<,>));

            For<IJsonProcessor>().Use<MediatedJsonProcessor>();
            For<TextWriter>().Use(Console.Out);
        }
    };

    //REF: https://github.com/jbogard/MediatR/blob/master/src/MediatR.Examples.StructureMap/StructureMapServiceLocator.cs
    public class StructureMapServiceLocator : ServiceLocatorImplBase
    {
        private readonly IContainer _container;

        public StructureMapServiceLocator(IContainer container)
        {
            _container = container;
        }

        protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
        {
            return _container.GetAllInstances(serviceType).Cast<object>();
        }

        protected override object DoGetInstance(Type serviceType, string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return serviceType.IsAbstract || serviceType.IsInterface
                           ? _container.TryGetInstance(serviceType)
                           : _container.GetInstance(serviceType);
            }

            return _container.GetInstance(serviceType, key);
        }
    }

    //REF: http://stackoverflow.com/questions/18896758/webapi-apicontroller-with-structuremap
    public class StructureMapServiceActivator : IHttpControllerActivator
    {
        private readonly IContainer _container;

        public StructureMapServiceActivator(IContainer container)
        {
            _container = container;
        }

        public IHttpController Create(HttpRequestMessage request,
                                      HttpControllerDescriptor controllerDescriptor,
                                      Type controllerType)
        {
            var scopedContainer = _container.GetNestedContainer();
            scopedContainer.Inject(typeof (HttpRequestMessage), request);
            request.RegisterForDispose(scopedContainer);
            var controller = (IHttpController) scopedContainer.GetInstance(controllerType);
            return controller;
        }
    };

    public class AuthorizationHandler<TRequest, TResponse>
        : IRequestHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IRequestHandler<TRequest, TResponse> _inner;
        private readonly IAppContext _appContext;

        public AuthorizationHandler(IRequestHandler<TRequest, TResponse> inner, IAppContext appContext)
        {
            _inner = inner;
            _appContext = appContext;
        }

        public TResponse Handle(TRequest request)
        {
            var authorize = request.GetType().GetCustomAttribute<AuthorizeAttribute>();
            if (authorize == null || string.IsNullOrWhiteSpace(authorize.Roles))
                return _inner.Handle(request);

            var roles = authorize.Roles.Split(',');
            if (!roles.Intersect(_appContext.Roles).Any())
                throw new AuthenticationException("Invalid Role");

            return _inner.Handle(request);
        }
    };

    public interface IAppContext
    {
        string UserName { get; }
        string[] Roles { get; }
    };

    public class AppContext : IAppContext
    {
        public string UserName
        {
            get { return "MyUser"; }
        }

        public string[] Roles
        {
            get { return new[] {"Admin2"}; }
        }
    };
};
