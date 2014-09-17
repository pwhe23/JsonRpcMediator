JsonRpcMediator
===============

After reading an article by Jimmy Bogard about the Mediator pattern,
it seemed like a good fit to go along with JsonRpc for use in an
Angular app. This is a simple implementatio of the JsonRpc spec that
makes it easy to invoke IRequest objects on the server from javascript.
It currently uses Web API as the JsonRpc endpoint, but that could easily
be swapped out.

Say you have a C# IRequest object on the server:

    public class SaveOrder : IRequest<bool>
    {
        public int? Id { get; set; }

        public class Handler : IRequestHandler<SaveOrder, bool>
        {
            public bool Handle(SaveOrder request)
            {
                return request.Id > 0;
            }
        }
    };


You can execute it from Angular.js very easily:

    jsonrpc.http("SaveOrder", { Id:0 }).then(function (data) {
        $scope.save = data;
    }, function (err) {
        alert(JSON.stringify(err));
    });

Or even execute it using SignalR just as simply:

    jsonrpc.signalr("SaveOrder", { Id:1 }).then(function (data) {
        $scope.save = data;
    }, function (err) {
        alert(JSON.stringify(err));
    });

Links
* [JsonRpc 2.0 Spec](http://www.jsonrpc.org/specification)
* [Ayende on Commands](http://ayende.com/blog/154241/limit-your-abstractions-the-key-is-in-the-infrastructure)
* [Tackling cross-cutting concerns with a mediator pipeline](http://lostechies.com/jimmybogard/2014/09/09/tackling-cross-cutting-concerns-with-a-mediator-pipeline/)
