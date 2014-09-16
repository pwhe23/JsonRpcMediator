
public class Home : Command<Home.Output>
{
    public Input Body { get; set; }

    public override Output Execute()
    {
        if (Body == null) return null;
        return new Output
        {
            Message = "Hello2 " + Body.Name,
        };
    }

    public class Input
    {
        public string Name { get; set; }
    }

    public class Output
    {
        public string Message { get; set; }
    };
};

public class Save : Command<bool>
{
    private readonly IMediator _mediator;

    public Save() { }
    public Save(IMediator mediator)
    {
        _mediator = mediator;
    }

    public int? Id { get; set; }

    public override bool Execute()
    {
        //Commands can call other commands using the Mediator
        var result = _mediator.Execute(new Home {Body = new Home.Input {Name = "Test " + Id}});
        return result != null && Id > 0;
    }
};
