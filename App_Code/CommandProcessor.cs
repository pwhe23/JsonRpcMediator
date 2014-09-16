using System;
using System.Web.Http;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class CommandProcessor : IJsonProcessor
{
    private readonly IMediator _mediator;

    public CommandProcessor(IMediator mediator)
    {
        _mediator = mediator;
    }

    public Object Execute(string name, string json)
    {
        var type = FindCommandTypeByName(name);
        var command = CreateCommandFromType(type);

        //Bind Json to Command object
        if (!String.IsNullOrWhiteSpace(json))
        {
            JsonConvert.PopulateObject(json, command);
        }

        return _mediator.Execute(command);
    }

    private static Type FindCommandTypeByName(string name)
    {
        try
        {
            //HACK: GetTypes some other way, like finding instances of ICommand on app load
            return Type.GetType(name, true, true);
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Unable to find command: " + name, ex);
        }
    }

    private static ICommand CreateCommandFromType(Type type)
    {
        try
        {

            return (ICommand)Activator.CreateInstance(type);
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Unable to find default constructor for command: " + type, ex);
        }
    }
};

[RoutePrefix("command")]
public class CommandController : ApiController
{
    private readonly IJsonProcessor _commander;

    public CommandController(IJsonProcessor commander)
    {
        _commander = commander;
    }

    [Route("{name}")]
    public dynamic Execute(string name)
    {
        var json = GetRequestAsString();
        return _commander.Execute(name, json);
    }

    private string GetRequestAsString()
    {
        var task = Request.Content.ReadAsStringAsync();
        task.Wait();
        return task.Result;
    }
};

public class CommandHub : Hub
{
    private readonly IJsonProcessor _commander;

    public CommandHub(IJsonProcessor commander)
    {
        _commander = commander;
    }

    public dynamic Execute(string name, JObject obj)
    {
        var json = obj == null ? null : obj.ToString();
        return _commander.Execute(name, json);
    }
};
