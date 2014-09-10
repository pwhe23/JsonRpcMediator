
public class Home : Command<Home.Output>
{
    public Input Body { get; set; }

    public override Output Execute()
    {
        if (Body == null) return null;
        return new Output
        {
            Title = "Hello2 " + Body.Title,
        };
    }

    public class Input
    {
        public string Title { get; set; }
    }

    public class Output
    {
        public string Title { get; set; }
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
