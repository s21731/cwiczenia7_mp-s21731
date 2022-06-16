using Cw07.Models;
using Cw07.Models.DTO;
using Cw07.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cw07.Controllers
{
    public class DbService : IDbService
    {
        private readonly CUSERSZENEKDOCUMENTSDATABASEAPBDMDFContext _dbContext;
        public DbService(CUSERSZENEKDOCUMENTSDATABASEAPBDMDFContext dbcontext)
        {
            _dbContext = dbcontext;
        }

        public async Task<IEnumerable<SomeSortOfTrip>> GetTrips()
        {
            return await _dbContext.Trips
                                    .Include(e => e.CountryTrips)
                                    .Include(e => e.ClientTrips)
                                    .Select(e => new SomeSortOfTrip
                                    {
                                        Name = e.Name,
                                        Description = e.Description,
                                        MaxPeople = e.MaxPeople,
                                        DateFrom = e.DateFrom,
                                        DateTo = e.DateTo,
                                        Countries = e.CountryTrips.Select(e => new SomeSortOfCountry { Name = e.IdCountryNavigation.Name }).ToList(),
                                        Clients = e.ClientTrips.Select(e => new SomeSortOfClient { FirstName = e.IdClientNavigation.FirstName, LastName = e.IdClientNavigation.LastName}).ToList()

                                   })
                                    .OrderByDescending(e => e.DateFrom)
                                    .ToListAsync();

        }

        public async Task DeleteClient(int idClient)
        {
            var IsClientHasTrip = await _dbContext.ClientTrips.AnyAsync(client => client.IdClient == idClient);

            if (IsClientHasTrip)
            {
                throw new Exception("Klient posiada co najmniej jedną przypisaną wycieczkę.");
            }

            var client = new Client { IdClient = idClient };

            //_dbContext.Attach(client);
            _dbContext.Remove(client);

            await _dbContext.SaveChangesAsync();
           
        }

        public async Task AddClientToTrip(int idTrip, SomeSortOfClientInTrip clientInTrip)
        {
            bool IsClientExists = await _dbContext.Clients.AnyAsync(c => c.Pesel == clientInTrip.Pesel);
            Client client;
            if (!IsClientExists)
            {
                client = new Client
                {
                    FirstName = clientInTrip.FirstName,
                    LastName = clientInTrip.LastName,
                    Email = clientInTrip.Email,
                    Telephone = clientInTrip.Telephone,
                    Pesel = clientInTrip.Pesel
                };
                await _dbContext.Clients.AddAsync(client);
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                client = await _dbContext.Clients.FirstOrDefaultAsync(c => c.Pesel == clientInTrip.Pesel);
            }

            bool IsTripExists = await _dbContext.Trips.AnyAsync(trip => trip.IdTrip == idTrip);

            if (!IsTripExists)
            {
                throw new Exception("Wycieczka o podanym id nie istnieje");
            }

            bool IsClientSignedUp = await _dbContext.ClientTrips.AnyAsync(c => c.IdClient == client.IdClient && c.IdTrip == idTrip);
            if (!IsClientSignedUp)
            {
                ClientTrip addClientTrip = new ClientTrip
                {
                    IdClient = client.IdClient,
                    IdTrip = idTrip,
                    RegisteredAt = DateTime.Now,
                    PaymentDate = clientInTrip.PaymentDate
                };

                await _dbContext.AddAsync(addClientTrip);
                await _dbContext.SaveChangesAsync();   
            }
            else
            {
                throw new Exception("Klient jest już zapisany na podaną wycieczkę");
            }
        }


        //public async Task RemoveTrip(int id)
        //{
        //    //================================
        //    //dodawanie
        //    var addTrip = new Trip { IdTrip = id, Name = "nazwaWyczieczki" };
        //    _dbContext.Add(addTrip);

        //    //edycja
        //    var editTrip = await _dbContext.Trips.Where(e => e.IdTrip == id).FirstOrDefaultAsync();
        //    editTrip.Name = "aaaa";
        //    //================================


        //    //var trip = _dbContext.Trips.Where(e => e.IdTrip == id).FirstOrDefaultAsync();

        //    var trip = new Trip { IdTrip = id };

        //    _dbContext.Attach(trip);
        //    _dbContext.Remove(trip);

        //    await _dbContext.SaveChangesAsync();
        //}


    }
}
