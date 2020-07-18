using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.DataAccess;
using TrackerLibrary.Models;

namespace TrackerLibrary.Data
{
    public class TournamentData
    {
        private readonly IDataAccess _dataAccess;
        private readonly ConnectionStringData _connectionString;

        public TournamentData(IDataAccess dataAccess, ConnectionStringData connectionString)
        {
            _dataAccess = dataAccess;
            _connectionString = connectionString;
        }

        public async Task<int> CreateTournament(TournamentModel tournament)
        {
            await SaveTournament(tournament);

            await SaveTournamentPrizes(tournament);

            await SaveTournamentEntries(tournament);

            await SaveTournamentRounds(tournament);

            //TournamentLogic.UpdateTournamentResult(tournament);

            return tournament.Id;
        }

        private async Task SaveTournament(TournamentModel tournament)
        {
            var p = new DynamicParameters();
            p.Add("@TournamentName", tournament.TournamentName);
            p.Add("@EntryFee", tournament.EntryFee);
            p.Add("@id", DbType.Int32, direction: ParameterDirection.Output);

            await _dataAccess.SaveData("dbo.spTournaments_Insert",
                p, _connectionString.SqlConnectionName);

            tournament.Id = p.Get<int>("id");
        }

        private async Task SaveTournamentPrizes(TournamentModel tournament)
        {
            foreach (PrizeModel prize in tournament.Prizes)
            {
                var p = new DynamicParameters();
                p.Add("@TournamentId", tournament.Id);
                p.Add("@PrizeId", prize.Id);
                p.Add("@id", DbType.Int32, direction: ParameterDirection.Output);

                await _dataAccess.SaveData("dbo.spTournamentPrizes_Insert",
                    p, _connectionString.SqlConnectionName);
            }
        }
        private async Task SaveTournamentEntries(TournamentModel tournament)
        {
            foreach (TeamModel team in tournament.EnteredTeams)
            {
                var p = new DynamicParameters();
                p.Add("@TournamentId", tournament.Id);
                p.Add("@TeamId", team.Id);
                p.Add("@id", DbType.Int32, direction: ParameterDirection.Output);

                await _dataAccess.SaveData("dbo.spTournamentEntries_Insert",
                    p, _connectionString.SqlConnectionName);
            }
        }

        private async Task SaveTournamentRounds(TournamentModel tournament)
        {
            foreach (List<MatchupModel> round in tournament.Rounds)
            {
                foreach (MatchupModel matchup in round)
                {
                    var p = new DynamicParameters();
                    p.Add("@TournamentId", tournament.Id);
                    p.Add("@MatchupRound", matchup.MatchupRound);
                    p.Add("@id", DbType.Int32, direction: ParameterDirection.Output);

                    await _dataAccess.SaveData("dbo.spMatchups_Insert",
                        p, _connectionString.SqlConnectionName);

                    matchup.Id = p.Get<int>("@id");

                    foreach (MatchupEntryModel entry in matchup.Entries)
                    {
                        p = new DynamicParameters();
                        p.Add("@MatchupId", matchup.Id);

                        if (entry.ParentMatchup == null)
                        {
                            p.Add("@ParentMatchupId", null);
                        }
                        else
                        {
                            p.Add("@ParentMatchupId", entry.ParentMatchup.Id);
                        }
                        if (entry.TeamCompeting == null)
                        {
                            p.Add("@TeamCompetingId", null);
                        }
                        else
                        {
                            p.Add("@TeamCompetingId", entry.TeamCompeting.Id);
                        }
                        p.Add("@id", DbType.Int32, direction: ParameterDirection.Output);

                        await _dataAccess.SaveData("dbo.spMatchupEntries_Insert",
                            p, _connectionString.SqlConnectionName);
                    }
                }
            }
        }


        public async Task<List<TournamentModel>> GetTournament_All()
        {
            var output = await _dataAccess.LoadData<TournamentModel, dynamic>("dbo.spTournaments_GetAll",
                new { }, _connectionString.SqlConnectionName);

            var p = new DynamicParameters();

            foreach (TournamentModel t in output)
            {
                // Populate Prizes
                p = new DynamicParameters();
                p.Add("@TournamentId", t.Id);
                t.Prizes = await _dataAccess.LoadData<PrizeModel, dynamic>("dbo.spPrizes_GetByTournament",
                    new { }, _connectionString.SqlConnectionName);

                // Populate Teams
                p = new DynamicParameters();
                p.Add("@TournamentId", t.Id);
                t.EnteredTeams = await _dataAccess.LoadData<TeamModel, dynamic>("dbo.spTeam_GetByTournament",
                   new { }, _connectionString.SqlConnectionName);

                foreach (TeamModel team in t.EnteredTeams)
                {
                    // Populate Team Members
                    p = new DynamicParameters();
                    p.Add("@TeamId", team.Id);
                    team.TeamMembers = await _dataAccess.LoadData<PersonModel, dynamic>("dbo.spTeamMembers_GetByTeam",
                        new { }, _connectionString.SqlConnectionName);
                }

                // Populate Rounds
                p = new DynamicParameters();
                p.Add("@TournamentId", t.Id);
                var matchups = await _dataAccess.LoadData<MatchupModel, dynamic>("dbo.spMatchups_GetByTournament",
                    new { }, _connectionString.SqlConnectionName);

                foreach (MatchupModel m in matchups)
                {
                    p = new DynamicParameters();
                    p.Add("@MatchupId", m.Id);
                    m.Entries = await _dataAccess.LoadData<MatchupEntryModel, dynamic>("dbo.spMatchupEntries_GetByMatchup",
                        new { }, _connectionString.SqlConnectionName);

                    // Populate each entry (2 models)
                    // Populate each matchup (1 model)

                    List<TeamModel> allTeams = await GetTeam_All();

                    if (m.WinnerId > 0)
                    {
                        m.Winner = allTeams.Where(x => x.Id == m.WinnerId).First();
                    }

                    foreach (var me in m.Entries)
                    {
                        if (me.TeamCompetingId > 0)
                        {
                            me.TeamCompeting = allTeams.Where(x => x.Id == me.TeamCompetingId).First();
                        }

                        if (me.ParentMatchupId > 0)
                        {
                            me.ParentMatchup = matchups.Where(x => x.Id == me.ParentMatchupId).First();
                        }
                    }
                }
                // List<List<MatchupModel>>  -> Rounds
                List<MatchupModel> currRow = new List<MatchupModel>();
                int currRound = 1;

                foreach (MatchupModel m in matchups)
                {
                    if (m.MatchupRound > currRound)
                    {
                        t.Rounds.Add(currRow);
                        currRow = new List<MatchupModel>();
                        currRound += 1;
                    }
                    currRow.Add(m);
                }

                t.Rounds.Add(currRow);
            }

            return output;

        }

        private async Task<List<TeamModel>> GetTeam_All()
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

        public async Task CompleteTournament(TournamentModel tournament)
        {
            var p = new DynamicParameters();
            p.Add("@id", tournament.Id);

            await _dataAccess.SaveData("dbo.spTournaments_Complete",
                p, _connectionString.SqlConnectionName);
        }

    }
}
