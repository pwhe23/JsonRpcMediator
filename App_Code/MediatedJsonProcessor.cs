using System;
using MediatR;
using Newtonsoft.Json;

public class MediatedJsonProcessor : IJsonProcessor
{
    private readonly IMediator _mediator;

    public MediatedJsonProcessor(IMediator mediator)
    {
        _mediator = mediator;
    }

    public Object Process(string name, string json)
    {
        var type = FindRequestTypeByName(name);
        var request = CreateRequestFromType(type);

        //Bind Json to Command object
        if (!String.IsNullOrWhiteSpace(json))
        {
            JsonConvert.PopulateObject(json, request);
        }

        //Invoke
        var iface = type.GetInterface("IRequest`1");
        var method = _mediator.GetType().GetMethod("Send").MakeGenericMethod(iface.GetGenericArguments());
        return method.Invoke(_mediator, new[] {request});
    }

    private static Type FindRequestTypeByName(string name)
    {
        try
        {
            //HACK: GetTypes some other way, like finding instances of IRequest on app load
            return Type.GetType(name, true, true);
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Unable to find request: " + name, ex);
        }
    }

    private static Object CreateRequestFromType(Type type)
    {
        try
        {

            return Activator.CreateInstance(type);
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Unable to find default constructor for request: " + type, ex);
        }
    }
};
