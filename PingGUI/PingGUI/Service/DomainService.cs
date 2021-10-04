using System.Net.NetworkInformation;
using System.Threading.Tasks;
using PingGUI.Repository;
using SpeedTest.Net.Models;

namespace PingGUI.Service
{
    public class DomainService
    {
        private readonly DataRepository _dataRepository;
        
        public DomainService()
        {
            _dataRepository = new DataRepository();
        }

        public static async Task<DownloadSpeed> GetSpeedAsync()
        {
            var data = await DataRepository.RetrieveSpeedAsync().ConfigureAwait(false);
            data = Compute(data);
            return data;
        }

        private static DownloadSpeed Compute(DownloadSpeed data)
        {
            return data;
        }
        
    }
}