using System;
using System.Collections.Generic;
using System.Text;

namespace TrackerLibrary.DataAccess
{
    public class ConnectionStringData : IConnectionStringData
    {
        public string SqlConnectionName { get; set; } = "Default";
    }
}
