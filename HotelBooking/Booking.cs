using System;

namespace HotelBooking
{
    public class Booking
    {
        public Booking(string guest, DateTime date)
        {
            Guest = guest;
            Date = date;
        }
        public string Guest { get; private set; }
        public DateTime Date { get; private set; }
    }
}