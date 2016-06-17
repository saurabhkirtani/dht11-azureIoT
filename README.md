### Overview
This repository contains a simple solution to take readings from a DHT11 sensor and send them to cloud (Microsoft Azure) for real-time analytics.

### Pre-requisites
You should have Visual Studio 2015 with Windows 10 SDK on a Windows 10 Machine. The sample here runs on Windows 10 IoT Core on a Raspberry Pi 2 - hence you should have a RPi2 with Windows 10 IoT Core image loaded on a compatible SD Card.

### Steps
To get this to work, follow the steps below:

1.  Clone this repository to your local system.

2.  Open solution -> DHT Solution.sln

3.  In the Solution Explorer window, open Sensors.OneWire. Right click on References -> select Add Reference. In the popup, select Sensors.DHT in the Projects tab. ![Image Add reference](https://github.com/saurabhkirtani/dht11-azureIoT/blob/master/images/addref.PNG). 

Rebuild the solution and then Clean the solution by going to the **Build** toolbar at the top in your Visual Studio 2015. *(If you're still getting a red sqiggly line for Sensors.Dht namespace in Mainpage.Xaml.cs , rebuild the solution to resolve)*s

4.  Without giving power to the Raspberry Pi 2 yet, make sure you have connected the DHT11 sensor to the Raspberry Pi as per below. Pin mapping of Raspberry Pi 2 is [available here](https://developer.microsoft.com/en-us/windows/iot/win10/samples/pinmappingsrpi2)

    -   VCC of DHT11 to 3.3V (Pin 1) of Raspberry Pi 2
    -   Data of DHT11 to GPIO 4 (Pin 7) of Raspberry Pi 2
    -   GND of DHT11 to GND (Pin 6) of Raspberry Pi 2

5.  Take a **cross-cable** LAN wire, and connect the Pi to your laptop.

6.  Power up the Pi, by connecting it to a power socket, or a USB 3.0 port in your laptop. Wait for a minute once this is done.

7.  You will need to find out the IP address of the Raspberry Pi. For this purpose, you can use the [Windows 10 IoT Core Dashboard](https://developer.microsoft.com/en-us/windows/iot/downloads). Download this tool if you don't have it already.

8.  Go back to Visual Studio and deploy the solution to your Raspberry Pi 2. (Choose *Remote Machine* while deploying and specify the IP address of the Pi).  All the Nuget Packages will be automaticaly restored. Wait for about a minute.

9. Once the solution is deployed, you will start seeing the temperature and humidity readings in the Output window in Visual Studio. The values are taken every 4 seconds.

        *Congratulations! You have completed one part of this workshop. We will now move on to the cloud part.*
        
10. Open your Azure account by going to https://portal.azure.com . If you don't have an Azure account, you can get a free trial here: https://azure.microsoft.com/en-in/free/ (For the attendees of the Iot NCR workshop on 18 June, 2016, you'll be distributed with free Azure passes)

11.  Setup IoT Hub in your Azure account. [Instructions here.](https://github.com/Azure/azure-iot-sdks/blob/master/doc/setup_iothub.md)

12.  Install the Device Explorer utility if you don't have it already, create a new device and note down the **device connection string**. [Instructions here.](https://github.com/Azure/azure-iot-sdks/blob/master/tools/DeviceExplorer/doc/how_to_use_device_explorer.md)

14. Uncomment these lines from the code in [solution/Sensors.OneWire/MainPage.xaml.cs] (https://github.com/saurabhkirtani/dht11-azureIoT/blob/master/solution/Sensors.OneWire/MainPage.xaml.cs)

        Line 27:    deviceClient1 = DeviceClient.CreateFromConnectionString("<enter-your-device-connection-string-here>", TransportType.Http1);
        
        Line 94:    var message = new Message(System.Text.Encoding.ASCII.GetBytes(messageString));

        Line 95:    await deviceClient1.SendEventAsync(message);

15. Paste the device connection string obained from step 12 in [solution/Sensors.OneWire/MainPage.xaml.cs](https://github.com/saurabhkirtani/dht11-azureIoT/blob/master/solution/Sensors.OneWire/MainPage.xaml.cs#L27) (line 27)

16. From the Solution Explorer window in Visual Studio, open the file Package.appxmanifest. Go to the *Packaging* tab. Change the *Package name* to *MagicOfCloud*. Change the *Published display name* to your own name.

17. Now deploy the app again to the Raspberry Pi by clicking on the Remote Machine button in the toolbar. Wait for a minute while the app is deployed.

18. Once the app is deployed, go to the Windows 10 IoT Core Dashboard which you would have downloaded in step 7. Click on the *Open in Device portal* link. The browser will ask you for a username and password. By default, the username is administrator and the password is p@ssw0rd

19. In the portal, go to the Apps menu. From the dropdown, select the app whose name starts with *MagicOfCloud* and click on Set Default. Once the app is set as default, shutdown the Pi by clicking on the Shutdown button at the top of the portal.

20. After a few seconds, take out the cross-cable LAN wire from the Pi and remove the power input given to the Pi.

21. Take a **straight cable LAN wire** and attach it to the Pi. Insert the other end of the cable to a Ethernet port in the room.

22. Power up the Pi, and wait for a minute.

23. The data is now being sent to Azure. You can verify that the data is being sent to Azure by using the same Device Explorer tool above. Go to the Data tab, choose the device, and click on Monitor. The data format being sent is of the form *{"DeviceId":1,"temperature":29.0,"unitOfTemp":"C","humidity":41.0,"unitOfHumidity":"%","TimeFlag":"6/13/2016 3:47:14 PM"}*

24. We'll now analyze this data real-time in the cloud, by using a Stream Analytics job and Power BI dashboards. The instructions for the setup are [given here](https://github.com/saurabhkirtani/dht11-azureIoT/blob/master/powerbi-setup.md)


### Simulation
In case you want to simulate the data, you can work with the C# sample SbRealTimePush. Open the project in Visual Studio, its a normal .net console application. Right click on the project and "Rebuild", this should restore the Nuget packages and rebuild the console app.
Replace the connection in line (program.cs) - DeviceClient.CreateFromConnectionString("HostName=XXXXXXXXXXX.azure-devices.net;DeviceId=XXXXX;SharedAccessKey=XXXXXXXXXXXXXXXXXXXXXX==", TransportType.Http1);
with your device connection string for IOT hub.
                     
### Credits
This code is a modified version of [this Hackster project](https://www.hackster.io/porrey/dht11-dht22-temperature-sensor-077790). The code to send data to Azure and analyze it real-time in the cloud has been added to this project.
