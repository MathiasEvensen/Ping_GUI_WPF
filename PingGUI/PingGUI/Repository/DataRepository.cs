using System;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using PingGUI.OutputAndInput;
using SpeedTest.Net;
using SpeedTest.Net.Models;

namespace PingGUI.Repository
{
    public class DataRepository
    {
        public static Task<DownloadSpeed> RetrieveSpeedAsync()
        {
            return Task.Run(RetrieveSpeed);
        }

        private static async Task<DownloadSpeed> RetrieveSpeed()
        {
            var speed = await FastClient.GetDownloadSpeed().ConfigureAwait(false);
            return speed;
        }
    }
}