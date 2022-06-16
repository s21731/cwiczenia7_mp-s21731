using Cw07.Models.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cw07.Services
{
    public interface IDbService
    {

        Task<IEnumerable<SomeSortOfTrip>> GetTrips();
        Task DeleteClient(int idClient);
        Task AddClientToTrip(int idTrip, SomeSortOfClientInTrip clientInTrip);


    }
}
