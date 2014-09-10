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
                Title = "Hello " + Body.Title,
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

You can execute it from Angular.js very easily:

    jsonrpc.http("home", { Body: { Title: 'Test' } }).then(function (data) {
        $scope.home = data;
    }, function (err) {
        alert(JSON.stringify(err));
    });

Or even execute it using SignalR just as simply:

    jsonrpc.signalr("home", { Body: { Title: 'Test' } }).then(function (data) {
        $scope.home = data;
    }, function (err) {
        alert(JSON.stringify(err));
    });
