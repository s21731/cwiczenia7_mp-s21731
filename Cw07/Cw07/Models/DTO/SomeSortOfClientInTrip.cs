using System;
using System.Diagnostics.CodeAnalysis;

namespace Cw07.Models.DTO
{
    public class SomeSortOfClientInTrip
    {
        [NotNull]
        public string FirstName { get; set; }
        [NotNull]
        public string LastName { get; set; }
        [NotNull]
        public string Email { get; set; }
        [NotNull]
        public string Telephone { get; set; }
        [NotNull]
        public string Pesel { get; set; }
        [NotNull]
        public int IdTrip { get; set; }
        [NotNull]
        public string TripName { get; set; }
        public DateTime PaymentDate { get; set; }
    }
}
