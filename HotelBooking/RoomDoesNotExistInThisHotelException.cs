using System;

namespace HotelBooking
{
    public class RoomDoesNotExistInThisHotelException : Exception
    {
        public RoomDoesNotExistInThisHotelException(string exception)
            :base(exception)
        {

        }

        public RoomDoesNotExistInThisHotelException()
        {

        }
    }
}
