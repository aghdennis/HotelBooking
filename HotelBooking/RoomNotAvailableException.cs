using System;

namespace HotelBooking
{
    public class RoomNotAvailableException : Exception
    {
        public RoomNotAvailableException(string exception)
            :base(exception)
        {
        }

        public RoomNotAvailableException()
        {
        }
    }
}
