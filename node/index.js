    var sensorLib = require('node-dht-sensor');

    // Uncomment one of these transports and then change it in fromConnectionString to test other transports
    var Protocol = require('azure-iot-device-http').Http;
    // var Protocol = require('azure-iot-device-mqtt').Mqtt;
    // var Protocol = require('azure-iot-device-amqp').Amqp;
    // var Protocol = require('azure-iot-device-amqp-ws').AmqpWs;
    var Client = require('azure-iot-device').Client;
    var Message = require('azure-iot-device').Message;

     // String containing Hostname, Device Id & Device Key in the following formats:
    //  "HostName=<iothub_host_name>;DeviceId=<device_id>;SharedAccessKey=<device_key>"       
    
    // var connectionString = '<device connection string>';
    //var client = Client.fromConnectionString(connectionString, Protocol);

    var sensor = {
    initialize: function () {
        return sensorLib.initialize(11, 4);
    },
    read: function () {
        var readout = sensorLib.read();
        console.log('Temperature: ' + readout.temperature.toFixed(2) + 'C, ' +
            'humidity: ' + readout.humidity.toFixed(2) + '%');

         var data = JSON.stringify({
             DeviceId:1,
             temperature:readout.temperature.toFixed(2),
             unitOfTemp:"C",
             humidity:readout.humidity.toFixed(2),
             unitOfHumidity:"%",
             TimeFlag:new Date()
          });   

  //  var message = new Message(data);
  //  console.log('Sending message back to the IoT Hub: ' + message.getData());
  //  client.sendEvent(message, printResultFor('send'));
    }
};

// Helper function to print results in the console
function printResultFor(op) {
  return function printResult(err, res) {
    if (err) console.log(op + ' error: ' + err.toString() + "---------------------------------------------");
    if (res) console.log(op + ' status: ' + res.constructor.name + "---------------------------------------------");
  };
}

var connectCallback = function (err) {
  if (err) {
    console.error('Could not connect: ' + err.message);
  } else {
    console.log('Client connected');
    setInterval(function () {
            sensor.read();
        }, 2000);
  }
};

if (sensor.initialize()) {
  //  client.open(connectCallback);
  connectCallback();
} else {
    console.warn('Failed to initialize sensor');
}