
app.controller('HomeCtrl', function ($scope, $http, jsonrpc) {

    jsonrpc.send("Home", { Name: 'Test' }).then(function (data) {
        $scope.home = data;
    }, function (err) {
        alert(JSON.stringify(err));
    });

    //jsonrpc.http("SaveOrder").then(function (data) {
    //    $scope.save = data;
    //}, function (err) {
    //    alert(JSON.stringify(err));
    //});

    jsonrpc.send("SaveOrder", { Id: 1 }).then(function (data) {
        $scope.save = data;
    }, function (err) {
        alert(JSON.stringify(err));
    });

    //jsonrpc.batch({ method:'SaveOrder',params:{Id:1} }, { method: 'SaveOrder' }).then(function (data) {
    //    alert(JSON.stringify(data));
    //}, function (err) {
    //    alert(JSON.stringify(err));
    //});
});
