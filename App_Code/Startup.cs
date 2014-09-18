using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using MediatR;
using Microsoft.Practices.ServiceLocation;
using StructureMap;
using StructureMap.Graph;

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
        var container = new Container(cfg =>
        {
            cfg.Scan(scanner =>
            {
                scanner.TheCallingAssembly();
                scanner.AssemblyContainingType<IMediator>();
                scanner.AddAllTypesOf(typeof (IRequestHandler<,>));
                scanner.AddAllTypesOf(typeof (INotificationHandler<>));
                scanner.AddAllTypesOf<IHttpController>();
                scanner.WithDefaultConventions();
            });
            cfg.For<IJsonProcessor>().Use<MediatedJsonProcessor>();
            cfg.For<TextWriter>().Use(Console.Out);
        });

        var serviceLocator = new StructureMapServiceLocator(container);
        var serviceLocatorProvider = new ServiceLocatorProvider(() => serviceLocator);
        container.Configure(cfg => cfg.For<ServiceLocatorProvider>().Use(serviceLocatorProvider));

        return container;
    }

    private static void RegisterWebApi(HttpConfiguration config)
    {
        config.Services.Replace(typeof(IHttpControllerActivator), new StructureMapServiceActivator(config, _globalContainer));
        config.MapHttpAttributeRoutes();
        config.EnsureInitialized();
    }

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

        public StructureMapServiceActivator(HttpConfiguration configuration, IContainer container)
        {
            _container = container;
        }

        public IHttpController Create(HttpRequestMessage request
            , HttpControllerDescriptor controllerDescriptor, Type controllerType)
        {
            var controller = (IHttpController)_container.GetInstance(controllerType);
            return controller;
        }
    }
};
