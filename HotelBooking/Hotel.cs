using System;

namespace HotelBooking
{
    public class Hotel : IHotel
    {
        private readonly string name;
        private readonly int[] rooms;
        public Hotel(string name, int[] rooms)
        {
            this.name = name;
            this.rooms = rooms;
        }
        public string Name => name;
        public int[] Rooms => rooms;
    }
}
