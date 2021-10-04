using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using PingGUI.OutputAndInput;
using PingGUI.Service;

namespace PingGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private string _localIP;
        private TextBoxOutput _output;

        private int _startIP = 1;
        private int _stopIP = 255;
        private string ip;

        private const int _timeout = 200;
        private int _nFound = 0;

        private static readonly object _lockObj = new object();
        private readonly Stopwatch Stopwatch = new Stopwatch();
        private TimeSpan _ts;

        private readonly DomainService _domainService;

        public string _input { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            _domainService = new DomainService();
            _output = new TextBoxOutput(TextBox);
            Console.SetOut(_output);
            DataContext = this;
        }

        private void GetLocalIP_OnClick(object sender, RoutedEventArgs e)
        {
            _output.CLS();
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                if (socket.LocalEndPoint is IPEndPoint endPoint) _localIP = endPoint.Address.ToString();
            }

            Console.WriteLine("Your Local IP: \n" + _localIP);
        }

        private string GetIP()
        {
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                if (socket.LocalEndPoint is IPEndPoint endPoint) _localIP = endPoint.Address.ToString();
            }

            return _localIP;
        }

        private async void DetectAllIP_OnClick(object sender, RoutedEventArgs e)
        {
            _output.CLS();

            _nFound = 0;

            var tasks = new List<Task>();

            Stopwatch.Start();

            var initialIp = GetIP();

            String ipBase = initialIp.Substring(0, initialIp.LastIndexOf(".")) + ".";
            for (int i = _startIP; i <= _stopIP; i++)
            {
                ip = ipBase + i;

                Ping p = new Ping();
                var task = PingAndUpdateAsync(p, ip);
                tasks.Add(task);
            }

            await Task.WhenAll(tasks).ContinueWith(
                t =>
                {
                    Stopwatch.Stop();
                    _ts = Stopwatch.Elapsed;
                    Console.WriteLine(_nFound + " devices found. Elapsed time: " + _ts, "Asynchronous");
                });

            Console.ReadLine();
        }

        private async Task PingAndUpdateAsync(Ping ping, string ipadr)
        {
            var reply = await ping.SendPingAsync(ipadr, _timeout);
            /*
            string regex = new Regex("[-a-zA-Z0-9@:%._\\+~#=]{1,256}\\.[a-zA-Z0-9()]{1,6}\\b([-a-zA-Z0-9()@:%_\\+.~#?&//=]*)").ToString();
            var i = regex.Matches(_input);
            if (i.)
            {
                if (reply != null) reply = ping.Send(reply.Address, _timeout);
            }
            */
            if (reply.Status == IPStatus.Success)
            {
                lock (_lockObj)
                {
                    _nFound++;
                    Console.WriteLine(ipadr);
                }
            }
        }

        private void PingIP_OnClick(object sender, RoutedEventArgs e)
        {
            _output.CLS();
            Ping ping = new Ping();

            var reply = ping.Send(_input, _timeout);

            if (reply != null && reply.Status == IPStatus.Success)
            {
                Console.WriteLine(
                    "Ping to " + _input + " [" + reply.Address + "]" + 
                    " Successful" + " Response delay = " + reply.RoundtripTime + " ms" + "\n");
            }
            else if (reply != null)
                switch (reply.Status)
                {
                    case IPStatus.TimedOut:
                        Console.WriteLine("Timed out");
                        break;
                    case IPStatus.Unknown:
                        Console.WriteLine("Unknown error");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
        }

        private async void SpeedTest_OnClick(object sender, RoutedEventArgs e)
        {
            _output.CLS();
            try
            {
                // output.Speed.ToString().Split(',')[0] delt på 100
                var output = await DomainService.GetSpeedAsync().ConfigureAwait(true);
                lock (_lockObj)
                {
                    Console.WriteLine("\nSpeed: " + output.Speed + " " + output.Unit + "\nSource: " + output.Source);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private async void PublicIP_OnClick(object sender, RoutedEventArgs e)
        {
            _output.CLS();
            var httpClient = new HttpClient();
            var publicIp = await httpClient.GetStringAsync("https://api.ipify.org");
            Console.WriteLine($"Public IP Address: \n{publicIp}");
        }

        private async void PingRangeOfIP_OnClick(object sender, RoutedEventArgs e)
        {
            _output.CLS();
            
            _nFound = 0;

            var tasks = new List<Task>();

            Stopwatch.Start();

            var initialIp = _input;

            String ipBase = initialIp.Substring(0, initialIp.LastIndexOf(".")) + ".";
            for (int i = _startIP; i <= _stopIP; i++)
            {
                ip = ipBase + i;

                Ping p = new Ping();
                var task = PingAndUpdateAsync(p, ip);
                tasks.Add(task);
            }

            await Task.WhenAll(tasks).ContinueWith(
                t =>
                {
                    Stopwatch.Stop();
                    _ts = Stopwatch.Elapsed;
                    Console.WriteLine(_nFound + " devices found. Elapsed time: " + _ts, "Asynchronous");
                });

            Console.ReadLine();
        }

        private void DetectOSFromIP_OnClick(object sender, RoutedEventArgs e)
        {
            System.OperatingSystem os = System.Environment.OSVersion;
            string osName = "Unknown";
            
        }
    }
}