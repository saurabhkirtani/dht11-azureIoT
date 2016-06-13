using System;
using System.Collections.Generic;
using System.Linq;
using Sensors.Dht;
using Sensors.OneWire.Common;
using Windows.Devices.Gpio;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using System.Diagnostics;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;

namespace Sensors.OneWire
{
	public sealed partial class MainPage : BindablePage
    {
        private DispatcherTimer _timer = new DispatcherTimer();

        GpioPin _pin = null;
        private IDht _dht = null;
        private List<int> _retryCount = new List<int>();
        private DateTimeOffset _startedAt = DateTimeOffset.MinValue;
        DeviceClient deviceClient1;
        public MainPage()
        {
            this.InitializeComponent();
            deviceClient1 = DeviceClient.CreateFromConnectionString("<enter-your-device-connection-string-here>", TransportType.Http1);

            _timer.Interval = TimeSpan.FromSeconds(5);
            _timer.Tick += _timer_Tick;
        }

        private async void _timer_Tick(object sender, object e)
        {
            DhtReading reading = new DhtReading();
            int val = this.TotalAttempts;
            this.TotalAttempts++;

            reading = await _dht.GetReadingAsync().AsTask();

            _retryCount.Add(reading.RetryCount);
            this.OnPropertyChanged(nameof(AverageRetriesDisplay));
            this.OnPropertyChanged(nameof(TotalAttempts));
            this.OnPropertyChanged(nameof(PercentSuccess));

            if (reading.IsValid)
            {
                this.TotalSuccess++;
                this.Temperature = Convert.ToSingle(reading.Temperature);
                this.Humidity = Convert.ToSingle(reading.Humidity);

                //Send to cloud
                var telemetryDataPoint = new
                {
                    DeviceId = 1,
                    temperature = reading.Temperature,
                    unitOfTemp = "C",
                    humidity = reading.Humidity,
                    unitOfHumidity = "%",
                    TimeFlag = Convert.ToString(DateTime.Now)
                };
                var messageString = JsonConvert.SerializeObject(telemetryDataPoint);
                var message = new Message(System.Text.Encoding.ASCII.GetBytes(messageString));
                await deviceClient1.SendEventAsync(message);
                Debug.WriteLine(messageString);

                this.LastUpdated = DateTimeOffset.Now;
                this.OnPropertyChanged(nameof(SuccessRate));
            }

            this.OnPropertyChanged(nameof(LastUpdatedDisplay));
        }


        public string PercentSuccess
        {
            get
            {
                string returnValue = string.Empty;

                int attempts = this.TotalAttempts;

                if (attempts > 0)
                {
                    returnValue = string.Format("{0:0.0}%", 100f * (float)this.TotalSuccess / (float)attempts);
                }
                else
                {
                    returnValue = "0.0%";
                }

                return returnValue;
            }
        }

        private int _totalAttempts = 0;
        public int TotalAttempts
        {
            get
            {
                return _totalAttempts;
            }
            set
            {
                this.SetProperty(ref _totalAttempts, value);
                this.OnPropertyChanged(nameof(PercentSuccess));
            }
        }

        private int _totalSuccess = 0;
        public int TotalSuccess
        {
            get
            {
                return _totalSuccess;
            }
            set
            {
                this.SetProperty(ref _totalSuccess, value);
                this.OnPropertyChanged(nameof(PercentSuccess));
            }
        }

        private float _humidity = 0f;
        public float Humidity
        {
            get
            {
                return _humidity;
            }

            set
            {
                this.SetProperty(ref _humidity, value);
                this.OnPropertyChanged(nameof(HumidityDisplay));
            }
        }

        public string HumidityDisplay
        {
            get
            {
                Debug.WriteLine("Humidity (%) : "+this.Humidity);
                return string.Format("{0:0.0}% RH", this.Humidity);
            }
        }

        private float _temperature = 0f;
        public float Temperature
        {
            get
            {
                return _temperature;
            }
            set
            {
                this.SetProperty(ref _temperature, value);
                this.OnPropertyChanged(nameof(TemperatureDisplay));
            }
        }

        public string TemperatureDisplay
        {
            get
            {
                Debug.WriteLine("Temperature (C) : "+this.Temperature);
                return string.Format("{0:0.0} °C", this.Temperature);
            }
        }

        private DateTimeOffset _lastUpdated = DateTimeOffset.MinValue;
        public DateTimeOffset LastUpdated
        {
            get
            {
                return _lastUpdated;
            }
            set
            {
                this.SetProperty(ref _lastUpdated, value);
                this.OnPropertyChanged(nameof(LastUpdatedDisplay));
            }
        }

        public string LastUpdatedDisplay
        {
            get
            {
                string returnValue = string.Empty;

                TimeSpan elapsed = DateTimeOffset.Now.Subtract(this.LastUpdated);

                if (this.LastUpdated == DateTimeOffset.MinValue)
                {
                    returnValue = "never";
                }
                else if (elapsed.TotalSeconds < 60d)
                {
                    int seconds = (int)elapsed.TotalSeconds;

                    if (seconds < 2)
                    {
                        returnValue = "just now";
                    }
                    else
                    {
                        returnValue = string.Format("{0:0} {1} ago", seconds, seconds == 1 ? "second" : "seconds");
                    }
                }
                else if (elapsed.TotalMinutes < 60d)
                {
                    int minutes = (int)elapsed.TotalMinutes == 0 ? 1 : (int)elapsed.TotalMinutes;
                    returnValue = string.Format("{0:0} {1} ago", minutes, minutes == 1 ? "minute" : "minutes");
                }
                else if (elapsed.TotalHours < 24d)
                {
                    int hours = (int)elapsed.TotalHours == 0 ? 1 : (int)elapsed.TotalHours;
                    returnValue = string.Format("{0:0} {1} ago", hours, hours == 1 ? "hour" : "hours");
                }
                else
                {
                    returnValue = "a long time ago";
                }

                return returnValue;
            }
        }

        public int AverageRetries
        {
            get
            {
                int returnValue = 0;

                if (_retryCount.Count() > 0)
                {
                    returnValue = (int)_retryCount.Average();
                }

                return returnValue;
            }
        }

        public string AverageRetriesDisplay
        {
            get
            {
                return string.Format("{0:0}", this.AverageRetries);
            }
        }

        public string SuccessRate
        {
            get
            {
                string returnValue = string.Empty;

                double totalSeconds = DateTimeOffset.Now.Subtract(_startedAt).TotalSeconds;
                double rate = this.TotalSuccess / totalSeconds;

                if (rate < 1)
                {
                    returnValue = string.Format("{0:0.00} seconds/reading", 1d / rate);
                }
                else
                {
                    returnValue = string.Format("{0:0.00} readings/sec", rate);
                }

                return returnValue;
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            _pin = GpioController.GetDefault().OpenPin(4, GpioSharingMode.Exclusive);
            _dht = new Dht11(_pin, GpioPinDriveMode.Input);

            _timer.Start();

            _startedAt = DateTimeOffset.Now;

            // ***
            // *** Uncomment to simulate heavy CPU usage
            // ***
            //CpuKiller.StartEmulation();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            _timer.Stop();

            _pin.Dispose();
            _pin = null;

            _dht = null;

            CpuKiller.StopEmulation();

            base.OnNavigatedFrom(e);
        }


    }
}
