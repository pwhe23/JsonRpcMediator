JsonRpc
=======

Simple JsonRpc implementation for Asp.net WebApi

Say you have a C# command on the server:

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

You can execute it from Angular.js very easily:

    jsonrpc.http("home", { Body: { Name: 'Test' } }).then(function (data) {
        $scope.home = data;
    }, function (err) {
        alert(JSON.stringify(err));
    });

Or even execute it using SignalR just as simply:

    jsonrpc.signalr("home", { Body: { Name: 'Test' } }).then(function (data) {
        $scope.home = data;
    }, function (err) {
        alert(JSON.stringify(err));
    });

Links
* [JsonRpc 2.0 Spec](http://www.jsonrpc.org/specification)
* [Ayende on Commands](http://ayende.com/blog/154241/limit-your-abstractions-the-key-is-in-the-infrastructure)
