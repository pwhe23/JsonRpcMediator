using System;
using Omu.ValueInjecter;

public interface IMediator
{
    T Execute<T>(ICommand<T> command);
    Object Execute(ICommand command);
};

public class Mediator : IMediator
{
    // Copying fakeCommand to command allows commands to be "newed" up with a default constructor
    // in client code, while allowing a second dependency-paramaterized constructor to be
    // populated by the IOC here before being executed
    public T Execute<T>(ICommand<T> fakeCommand)
    {
        return (T) Execute((ICommand) fakeCommand);
    }

    public Object Execute(ICommand fakeCommand)
    {
        var type = fakeCommand.GetType();
        var command = (ICommand)TinyIoC.TinyIoCContainer.Current.Resolve(type);
        new ValueInjecter().Inject(command, fakeCommand);
        return command.Execute();
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
