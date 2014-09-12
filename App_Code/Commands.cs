
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
    public int? Id { get; set; }

    public override bool Execute()
    {
        return Id > 0;
    }
};
