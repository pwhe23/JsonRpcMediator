using System;
using System.Web.Http;
using AutoMapper;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class Commander : ICommandProcessor
{
    // Copying fakeCommand to command allows commands to be "newed" up with a default constructor
    // in client code, while allowing a second dependency-paramaterized constructor to be
    // populated by the IOC here before being executed
    public T Execute<T>(ICommand<T> fakeCommand)
    {
        var type = fakeCommand.GetType();
        var command = (ICommand<T>)TinyIoC.TinyIoCContainer.Current.Resolve(type);
        Mapper.DynamicMap(fakeCommand, command);
        return command.Execute();
    }

    public Object Execute(string name, string json)
    {
        var command = LoadCommand(name, json);
        return Execute(command);
    }

    private static Object Execute(ICommand command)
    {
        return command.Execute();
    }

    private static ICommand LoadCommand(string name, string json)
    {
        //HACK: GetTypes some other way, like finding instances of ICommand on app load
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

public interface ICommand<out T> : ICommand
{
    new T Execute();
};

public abstract class Command<T> : ICommand<T>
{
    protected Command()
    {
        //commands should all allow default constructors to ease passing instances to Execute()
        //which would create IOC instances with dependency-filled constructors and then map
        //default constructor object to them
    }

    public abstract T Execute();

    object ICommand.Execute()
    {
        return Execute();
    }
};
