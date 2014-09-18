using System.Web.Http;
using MediatR;

[Authorize(Roles = "Admin")]
public class SaveOrder : IRequest<bool>
{
    public int? Id { get; set; }

    public class Handler : IRequestHandler<SaveOrder, bool>
    {
        private readonly IMediator _mediator;

        public Handler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public bool Handle(SaveOrder request)
        {
            //Call another Request
            var result = _mediator.Send(new Home
            {
                Name = "Test " + request.Id
            });
            return result != null && request.Id > 0;
        }
    }
};
