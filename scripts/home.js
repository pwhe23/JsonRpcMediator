
app.controller('HomeCtrl', function ($scope, $http, jsonrpc) {

    //jsonrpc.http("home", { Body: { Title: 'Test' } }).then(function (data) {
    //    $scope.home = data;
    //}, function (err) {
    //    alert(JSON.stringify(err));
    //});

    $http.post("command/home", { Body: { Title: 'Test4' } }).then(function (resp) {
        $scope.home = resp.data;
    }, function (err) {
        alert(JSON.stringify(err));
    });

    //jsonrpc.http("save").then(function (data) {
    //    $scope.save = data;
    //}, function (err) {
    //    alert(JSON.stringify(err));
    //});

    //$http.post("jsonrpc/save").then(function (resp) {
    //    $scope.save = resp.data;
    //});

    //jsonrpc.signalr("save", { Id: 1 }).then(function (data) {
    //    $scope.save = data;
    //}, function (err) {
    //    alert(JSON.stringify(err));
    //});

    //jsonrpc.batch({ method:'save',params:{Id:1} }, { method: 'save' }).then(function (data) {
    //    alert(JSON.stringify(data));
    //}, function (err) {
    //    alert(JSON.stringify(err));
    //});

    $.connection.commandHub.server.execute("save", {Id:1}).then(function (data) {
        $scope.save = data;
    }, function (err) {
        alert(JSON.stringify(err));
    });
});
