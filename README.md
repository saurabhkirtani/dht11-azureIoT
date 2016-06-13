### Overview
This repository contains a simple solution to take readings from a DHT11 sensor, display the temperature and humidity, and send the data in real time to Azure Iot Hub.

### Pre-requisites
You should have Visual Studio 2015 with Windows 10 SDK on a Windows 10 Machine. The sample here runs on Windows 10 IoT Core on a Raspberry Pi 2 - hence you should have a RPi2 with Windows 10 IoT Core image loaded on a compatible SD Card.

### Steps
To get this to work, follow the steps below:
1.  Setup IoT Hub in your Azure account. Instructions here: https://github.com/Azure/azure-iot-sdks/blob/master/doc/setup_iothub.md
2.  Install the Device Explorer utility if you don't have it already, and note down the device connection string. Instructions here: https://github.com/Azure/azure-iot-sdks/blob/master/tools/DeviceExplorer/doc/how_to_use_device_explorer.md
3.  Paste this device connection string in [solution/Sensors.OneWire/MainPage.xaml.cs](https://github.com/saurabhkirtani/dht11-azureIoT/blob/master/solution/Sensors.OneWire/MainPage.xaml.cs#L27) - where it's written *<enter-your-device-connection-string-here>* (line 27)
4.  Clean the solution and Build the solution by going to the *Build* toolbar in your Visual Studio 2015.
5.  Deploy the solution to your Raspberry Pi 2. (Choose *Remote Machine* while deploying and specify the IP address/Machine Name of the Pi). If you want to find out the IP address of the Raspberry Pis in the network, you can use the [Windows 10 IoT Core Dashboard](https://developer.microsoft.com/en-us/windows/iot/downloads). All the Nuget Packages will be automaticaly restored.
6.  The solution should start working properly now. The data format being sent is of the form
    *{"DeviceId":1,"temperature":29.0,"unitOfTemp":"C","humidity":41.0,"unitOfHumidity":"%","TimeFlag":"6/13/2016 3:47:14 PM"}*
    You can verify that the data is being sent to Azure by using the same Device Explorer tool above. Go to the Data tab, choose the device, and click on Monitor.
    
### Credits
This code is a modified version of [this Hackster project](https://www.hackster.io/porrey/dht11-dht22-temperature-sensor-077790). The code to send data to Azure has been added to this project.
