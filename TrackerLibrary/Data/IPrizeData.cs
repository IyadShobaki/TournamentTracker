using System.Threading.Tasks;
using TrackerLibrary.Models;

namespace TrackerLibrary.Data
{
    public interface IPrizeData
    {
        Task<int> CreatePrize(PrizeModel prize);
    }
}