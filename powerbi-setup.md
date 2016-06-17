### Overview
This document helps you setup a Real Time Power BI report over the data that your dht11 sensors are sending.

Alternatively there is a C# console that can emulate sending the sensor data to IOT Hub, the code for same could be found in SbRealTimePush folder.

##### Create Stream Analytics jobs
1. Go to manage.windowsazure.com, find "Stream Analytics" option.
2. Click + sign, quick create, give it a name "TemperatureJob", select Same region as that of your IOT Hub.
3. Click on TemperatureJob, click on "Inputs"
4. click + sign to add new Input.
5. Name it "InputTemp", take all these settings from the IOT hub settings, Connection strings

        a. subscription - Provide IOT hub settings manually.
        b. IOT Hub - Use the name of your IOT Hub
        c. IOT Hub shared access policy name
        d. IOT Hub policy Key 
        e. IOT Hub consumer group - Leave empty
        f. Event Serialization Format - JSON
        g. Encoding - UTF8
6. click on Test connection - to check if the connection with the IOT hub is working.
7. Click on "Query". Copy Paste this query in the box given

       <code> SELECT Avg(temperature) as Temperatur, Avg(humidity) as Humidity, Max(timeflag) as timeflag, DeviceId FROM inputtemp TIMESTAMP by timeflag group by tumblingwindow(Second,15), DeviceId</code>
       
8. Click on "Test" to test the query. The portal will ask for a json file through which it could test, Provide the test.json file provided in the SbRealTimePush Folder.
9. Click on Outputs, Click on "+" sign to Add Output.
10. Select "Sink" as "PowerBi", Do authorization with PowerBI if you have an account, else signup and authorize yourself.
        Output name - RealTimeTempHumOutput
        Dataset name - dsRealTimeTempHumOutput
        Table name - tblRealTimeTempHumOutput
11. Click 'Test Connection' to check connection to PowerBI
12. Go back to your Stream Analytics job and click on "Start"; it may take sometime

#### Creating PowerBI report

1. Go to [PowerBI.com] (http://www.powerbi.com)
2. A dataset by the name dsRealTimeTempHumOutput should appear on the left bottom, under the 'Datasets' click on it.
3. From right side toolbox, select humidity, minuteval and temperature.
4. Drag minuteval to 'Axis', and remove it from 'Values'
5. Click on Pin visual, give a name to your Report.
![Pin To Dashboard](https://github.com/saurabhkirtani/dht11-azureIoT/blob/master/images/pbi1.PNG "Pin to Dashboard")
6. Then select the name of Dashboard you want it to be pinned to, or Create a new Dashboard, and it'll get pinned to the same.
7. This is how the report should look like, once its ready, the report will refresh by itself as and when the data comes through stream analytics.
![Final PowerBI Report](https://github.com/saurabhkirtani/dht11-azureIoT/blob/master/images/pbi2.PNG "Final PowerBI Report")

  
### Simulation of data
In case you want to simulate the data, you can work with the C# sample SbRealTimePush. Open the project in Visual Studio, its a normal .net console application. Right click on the project and "Rebuild", this should restore the Nuget packages and rebuild the console app.
Replace the connection in line (program.cs) - DeviceClient.CreateFromConnectionString("HostName=XXXXXXXXXXX.azure-devices.net;DeviceId=XXXXX;SharedAccessKey=XXXXXXXXXXXXXXXXXXXXXX==", TransportType.Http1);
with your device connection string for IOT hub.