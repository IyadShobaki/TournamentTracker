using System.Threading.Tasks;
using TrackerLibrary.Models;

namespace TrackerLibrary.Data
{
    public interface IMatchupData
    {
        Task UpdateMacthup(MatchupModel matchup);
    }
}