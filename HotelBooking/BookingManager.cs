using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace HotelBooking
{
    public class BookingManager : IBookingManager
    {
        private readonly IHotel hotel;
        private readonly ConcurrentDictionary<int, Booking> BookingStore;
        public IDictionary<int, Booking> Store => BookingStore;

        public BookingManager(IHotel hotel)
        {
            this.hotel = hotel;
            BookingStore = new ConcurrentDictionary<int, Booking>();
        }
        public void AddBooking(string guest, int room, DateTime date)
        {
            if (IsRoomAvailable(room, date) && BookingStore.TryAdd(room, new Booking(guest, date)))
            { 
                return;
            }

            throw new RoomNotAvailableException($"Room {room} is not available to {guest} on {date}");
        }

        public IEnumerable<int> GetAvailableRooms(DateTime date)
        {
            return hotel.Rooms.Where(x => !BookingStore.Any(y => y.Key == x && y.Value.Date == date));
        }

        public bool IsRoomAvailable(int roomNo, DateTime date)
        {
            if (hotel.Rooms.Any(x => x == roomNo))
            {
                var isUnavailable = BookingStore.TryGetValue(roomNo, out Booking booking);  
                
                return (isUnavailable && booking.Date < DateTime.Today) ? BookingStore.TryRemove(roomNo, out Booking removedBooking) : !isUnavailable;                
            }

            throw new RoomDoesNotExistInThisHotelException($"The room does not exist in the hotel { hotel.Name }");
        }
    }
}