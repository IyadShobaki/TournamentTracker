using System.Collections.Generic;
using System.Threading.Tasks;
using TrackerLibrary.Models;

namespace TrackerLibrary.Data
{
    public interface IPeopleData
    {
        Task<int> CreatePerson(PersonModel person);
        Task<List<PersonModel>> GetPeople();
    }
}