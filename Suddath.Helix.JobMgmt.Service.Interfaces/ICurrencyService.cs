using System.Threading.Tasks;

namespace Suddath.Helix.JobMgmt.Services.Interfaces
{
    public interface ICurrencyService
    {
        Task<decimal> GetCurrencyExchangeRateAsync(string baseCurrency, string exchangeCurrency);
    }
}
