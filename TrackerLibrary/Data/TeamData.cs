using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.DataAccess;
using TrackerLibrary.Models;

namespace TrackerLibrary.Data
{
    public class TeamData : ITeamData
    {
        private readonly IDataAccess _dataAccess;
        private readonly IConnectionStringData _connectionString;

        public TeamData(IDataAccess dataAccess, IConnectionStringData connectionString)
        {
            _dataAccess = dataAccess;
            _connectionString = connectionString;
        }

        public async Task CreateTeam(TeamModel team)
        {
            var p = new DynamicParameters();
            p.Add("@TeamName", team.TeamName);
            p.Add("@id", DbType.Int32, direction: ParameterDirection.Output);

            await _dataAccess.SaveData("dbo.spTeams_Insert",
                p, _connectionString.SqlConnectionName);

            team.Id = p.Get<int>("@id");

            foreach (PersonModel tm in team.TeamMembers)
            {
                p = new DynamicParameters();
                p.Add("@TeamId", team.Id);
                p.Add("@PersonId", tm.Id);

                await _dataAccess.SaveData("dbo.spTeamMembers_Insert",
                    p, _connectionString.SqlConnectionName);
            }
        }


        public async Task<List<TeamModel>> GetTeam_All()
        {
     
            var output = await _dataAccess.LoadData<TeamModel, dynamic>("dbo.spTeam_GetAll",
                new { }, _connectionString.SqlConnectionName);

            foreach (TeamModel team in output)
            {
                var p = new DynamicParameters();
                p.Add("@TeamId", team.Id);

                team.TeamMembers = await _dataAccess.LoadData<PersonModel, dynamic>("dbo.spTeamMembers_GetByTeam",
                    p, _connectionString.SqlConnectionName);
            }

            return output;

        }
    }
}
