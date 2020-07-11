using System.Collections.Generic;
using System.Threading.Tasks;
using TrackerLibrary.Models;

namespace TrackerLibrary.Data
{
    public interface ITeamData
    {
        Task CreateTeam(TeamModel team);
        Task<List<TeamModel>> GetTeam_All();
    }
}