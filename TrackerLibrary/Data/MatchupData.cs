using Dapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.DataAccess;
using TrackerLibrary.Models;

namespace TrackerLibrary.Data
{
    public class MatchupData : IMatchupData
    {
        private readonly IDataAccess _dataAccess;
        private readonly ConnectionStringData _connectionString;

        public MatchupData(IDataAccess dataAccess, ConnectionStringData connectionString)
        {
            _dataAccess = dataAccess;
            _connectionString = connectionString;
        }

        public async Task UpdateMacthup(MatchupModel matchup)
        {
            var p = new DynamicParameters();
            if (matchup.Winner != null)
            {
                p.Add("@id", matchup.Id);
                p.Add("@WinnerId", matchup.Winner.Id);

                await _dataAccess.SaveData("dbo.spMatchups_Update",
                    p, _connectionString.SqlConnectionName);
            }
            foreach (MatchupEntryModel matchupEntry in matchup.Entries)
            {
                if (matchupEntry.TeamCompeting != null)
                {
                    p = new DynamicParameters();
                    p.Add("@id", matchupEntry.Id);
                    p.Add("@TeamCompetingId", matchupEntry.TeamCompeting.Id);
                    p.Add("@Score", matchupEntry.Score);

                    await _dataAccess.SaveData("dbo.spMatchupEntries_Update",
                        p, _connectionString.SqlConnectionName);
                }
            }
        }
    }
}
