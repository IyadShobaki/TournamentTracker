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
    public class PrizeData : IPrizeData
    {
        private readonly IDataAccess _dataAccess;
        private readonly ConnectionStringData _connectionString;

        public PrizeData(IDataAccess dataAccess, ConnectionStringData connectionString)
        {
            _dataAccess = dataAccess;
            _connectionString = connectionString;
        }

        public async Task<int> CreatePrize(PrizeModel prize)
        {
            var p = new DynamicParameters();
            p.Add("@PlaceNumber", prize.PlaceNumber);
            p.Add("@PlaceName", prize.PlaceName);
            p.Add("@PrizeAmount", prize.PrizeAmount);
            p.Add("@PrizePercentage", prize.PrizePercentage);
            p.Add("@id", DbType.Int32, direction: ParameterDirection.Output);

            await _dataAccess.SaveData("dbo.spPrizes_Insert",
                p, _connectionString.SqlConnectionName);

            return p.Get<int>("Id");
        }
    }
}
