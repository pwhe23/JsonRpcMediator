using MediatR;

public class Home : IRequest<Home.Response>
{
    public string Name { get; set; }

    public class Response
    {
        public string Message { get; set; }
    };

    public class Handler : IRequestHandler<Home, Response>
    {
        public Response Handle(Home request)
        {
            return new Response
            {
                Message = "Hello " + request.Name,
            };
        }
    }
};

