using System;
using System.Web.Http;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class Commander : ICommandProcessor
{
    public Object Execute(ICommand command)
    {
        return command.Execute();
    }

    public T Execute<T>(ICommand<T> command)
    {
        return command.Execute();
    }

    public Object Execute(string name, string json)
    {
        var command = LoadCommand(name, json);
        return Execute(command);
    }

    private static ICommand LoadCommand(string name, string json)
    {
        //HACK: get types some other way, like finding instances of ICommand on app load
        var type = Type.GetType(name, true, true);
        var command = (ICommand)TinyIoC.TinyIoCContainer.Current.Resolve(type);

        //Bind Json to Command object
        if (!String.IsNullOrWhiteSpace(json))
        {
            JsonConvert.PopulateObject(json, command);
        }
        return command;
    }
};

[RoutePrefix("command")]
public class CommandController : ApiController
{
    private readonly ICommandProcessor _commander;

    public CommandController(ICommandProcessor commander)
    {
        _commander = commander;
    }

    [Route("{method}")]
    public dynamic Execute(string method)
    {
        var json = GetRequestAsString();
        return _commander.Execute(method, json);
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
    private readonly ICommandProcessor _commander;

    public CommandHub(ICommandProcessor commander)
    {
        _commander = commander;
    }

    public dynamic Execute(string name, JObject obj)
    {
        var json = obj == null ? null : obj.ToString();
        return _commander.Execute(name, json);
    }
};

public interface ICommand
{
    Object Execute();
};

public interface ICommand<T> : ICommand
{
    new T Execute();
};

public abstract class Command<T> : ICommand<T>
{
    public abstract T Execute();

    object ICommand.Execute()
    {
        return Execute();
    }
};
