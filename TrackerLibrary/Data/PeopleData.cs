using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.DataAccess;
using TrackerLibrary.Models;

namespace TrackerLibrary.Data
{
    public class PeopleData : IPeopleData
    {
        private readonly IDataAccess _dataAccess;
        private readonly ConnectionStringData _connectionString;

        public PeopleData(IDataAccess dataAccess, ConnectionStringData connectionString)
        {
            _dataAccess = dataAccess;
            _connectionString = connectionString;
        }

        public async Task<int> CreatePerson(PersonModel person)
        {
            var p = new DynamicParameters();
            p.Add("@FirstName", person.FirstName);
            p.Add("@LastName", person.LastName);
            p.Add("@EmailAddress", person.EmailAddress);
            p.Add("@CellphoneNumber", person.CellphoneNumber);
            p.Add("Id", DbType.Int32, direction: ParameterDirection.Output);

            await _dataAccess.SaveData("dbo.spPeople_Insert", p, _connectionString.SqlConnectionName);

            return p.Get<int>("Id");
        }

        public Task<List<PersonModel>> GetPeople()
        {
            return _dataAccess.LoadData<PersonModel, dynamic>("dbo.spPeople_GetAll",
                new { }, _connectionString.SqlConnectionName);
        }
    }
}
