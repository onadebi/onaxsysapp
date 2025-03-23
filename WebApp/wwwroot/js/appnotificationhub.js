//import * as signalR from '/lib/microsoft/signalr/dist/browser/signalr.js';

"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/notificationshub").build();

connection.start().then(function () {
    console.log("connected to the hub!");
    const userIdentifier = document.getElementById('userIdentifier').value;
    connection.invoke("GetConnectionDetails", userIdentifier).catch(function (err) {
        console.error(`Error getting connection ID: ${err}`);
    });
}).catch(function (err) {
    console.error(`Notifications hub error: ${err}`);
});



connection.on("ReceiveMessage", function (userdetail) {
});

connection.on("ReceiveConnectionDetails", function (userdetail) {
    toastr.info(JSON.stringify(userdetail));

});
