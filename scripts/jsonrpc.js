
app.service('jsonrpc', function ($http, $q) {

    return {
        send: send,
        batch: batch
    };

    function send(method, params) {
        var defer = $q.defer();
        $http.post('jsonrpc', wrap(method, params)).then(function (resp) {
            unwrap(resp.data, defer);
        }, function (err) {
            defer.reject(err);
        });
        return defer.promise;
    }

    function batch() {
        var defer = $q.defer();
        var requests = [];
        angular.forEach(arguments, function (arg) {
            requests.push(wrap(arg.method, arg.params));
        });
        $http.post('jsonrpc', requests).then(function (resp) {
            defer.resolve(resp.data.map(function (res) { return res.result; }));
        }, function (err) {
            defer.reject(err);
        });
        return defer.promise;
    }

    function wrap(method, params) {
        return {
            jsonrpc: '2.0',
            method: method,
            params: params,
            id: guid()
        };
    }

    function unwrap(resp, defer) {
        if (resp.error) {
            defer.reject(resp.error.message);
        } else {
            defer.resolve(resp.result);
        }
    }

    //REF: http://stackoverflow.com/a/2117523
    function guid() {
        return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
            var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
            return v.toString(16);
        });
    }
});
