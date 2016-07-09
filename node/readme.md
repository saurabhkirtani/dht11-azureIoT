### Overview
This repository contains a simple solution to take readings from a DHT11 sensor and send them to cloud (Microsoft Azure) for real-time analytics, using Node.JS on a Raspberry Pi running Raspbian OS.

### Steps
To get this to work, follow the steps below:

1.  Clone this repository to your local system.

2.  Without giving power to the Raspberry Pi 2 yet, make sure you have connected the DHT11 sensor to the Raspberry Pi as per below. Pin mapping of Raspberry Pi  is [available here.](https://developer.microsoft.com/en-us/windows/iot/win10/samples/pinmappingsrpi2). Pin diagram of DHT11 to Raspberry Pi can also be [found here.](https://developer.microsoft.com/en-us/windows/iot/win10/samples/gpioonewire)

    -   VCC of DHT11 to 3.3V (Pin 1) of Raspberry Pi 2
    -   Data of DHT11 to GPIO 4 (Pin 7) of Raspberry Pi 2
    -   GND of DHT11 to GND (Pin 6) of Raspberry Pi 2

3.  Power up the Pi, by connecting it to a power socket, or a USB 3.0 port in your laptop. Wait for a minute once this is done. Put the Raspberry Pi and your development machine on the same network. Get the IP address of the Pi (either by viewing the IP on a display connected to the Pi, or by using another utility which can show the IPs of the Pi on the network).

4.  Ensure you are able to ping the Pi using the Pi - open cmd and ping the IP. You should be getting a successful ping.
  
5.  Open a SSH client (use Putty if you are on a Windows machine), and SSH into the Pi using its IP.

6.  Open a FTP client, like Filezilla, and access the Pi using the IP.

7.  If you've cloned the repository to your development machine, upload the **node** folder to a location on Raspberry Pi, using Filezilla, or a similar FTP client.

8.  SSH into the Pi (use Putty if you're on a Windows dev machine), run *npm install* to install all the required dependencies automatically. This process can take about a minute or so to complete.

9. Once they are installed, run the index.js file with sudo - *sudo node index.js*. If successful, you will start seeing the temperature and the humiity of the room every 2 seconds.

        *Congratulations! You have completed one part of this workshop. We will now move on to the cloud part.*
        
10. Open your Azure account by going to https://portal.azure.com . If you don't have an Azure account, you can get a free trial here: https://azure.microsoft.com/en-in/free/ 

11.  Setup IoT Hub in your Azure account. [Instructions here.](https://github.com/Azure/azure-iot-sdks/blob/master/doc/setup_iothub.md)

12.  Install the Device Explorer utility (if on a Windows dev machine) if you don't have it already, create a new device and note down the **device connection string**. [Instructions here.](https://github.com/Azure/azure-iot-sdks/blob/master/tools/DeviceExplorer/doc/how_to_use_device_explorer.md). If you are on a non-Windows dev machine, use the iothub-explorer CLI tool - [instructions are here.](https://github.com/Azure/azure-iot-sdks/tree/master/tools/iothub-explorer)

13. Uncomment these lines from the code in index.js 

        var connectionString = '<device connection string>';

        var client = Client.fromConnectionString(connectionString, Protocol);
        
        
        var message = new Message(data);

        console.log('Sending message back to the IoT Hub: ' + message.getData());

        client.sendEvent(message, printResultFor('send'));


        client.open(connectCallback);

14. Paste the device connection string obained from step 12 in connectionString variable . Also, add the comment to where connectCallback() is called (line 62)

16. Upload index.js to the Pi using a FTP client. Overwrite the existing file on the Pi.

17. Run this file - *sudo node index.js*

18. The data is now being sent to Azure. You can verify that the data is being sent to Azure by using the same Device Explorer tool above. Go to the Data tab, choose the device, and click on Monitor. The data format being sent is of the form *{"DeviceId":1,"temperature":"32.00","unitOfTemp":"C","humidity":"33.00","unitOfHumidity":"%","TimeFlag":"2016-07-09T15:03:02.320Z"}*

24. We'll now analyze this data real-time in the cloud, by using a Stream Analytics job and Power BI dashboards. The instructions for the setup are [given here](https://github.com/saurabhkirtani/dht11-azureIoT/blob/master/streamanalytics-PowerBISetup.md)
                     
### Credits
The code uses [this NPM module](https://www.npmjs.com/package/node-dht-sensor) to take the readings from DHT11 sensor.

Contributors to this repo include:

-       [Saurabh Kirtani](https://twitter.com/saurabhkirtani)
-       [Jaswant Singh](https://twitter.com/jaswantsandhu)
-       [Brij Raj Singh](https://twitter.com/brijrajsingh)
