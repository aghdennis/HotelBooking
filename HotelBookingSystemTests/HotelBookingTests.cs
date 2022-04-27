using FluentAssertions;
using HotelBooking;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HotelBookingSystemTests
{
    public class HotelBookingTests
    {
        private IBookingManager bookingManager;
        private readonly IHotel hotel;      

        public HotelBookingTests()
        {
            hotel = new Hotel("My Test Hotel", new int[] { 101, 102, 201, 203 });            
        }

        [SetUp]
        public void Initialise()
        {
            bookingManager = new BookingManager(hotel);
        }

        [Test]
        public void WhenRoomBookedDoesNotExistInHotelThenThrowException()
        {
            Assert.Throws<RoomDoesNotExistInThisHotelException>(() 
                => bookingManager.AddBooking("Patel", 500, DateTime.Today));
        }

        [Test]
        public void WhenRoomBookedCorrectlyThenRoomIsUnAvailable()
        {
            bookingManager.AddBooking("test guest", 101, DateTime.Today);
            bookingManager.IsRoomAvailable(101, DateTime.Today).Should().BeFalse();
        }

        [Test]
        public void WhenRoom101IsBookedCorrectlyThenRoom102IsAvailable()
        {
            bookingManager.AddBooking("Dennis", 101, DateTime.Today);
            bookingManager.IsRoomAvailable(102, DateTime.Today).Should().BeTrue();
        }

        [Test]
        public void WhenRoomIsUnAvailableButBookedThenExceptionIsThrown()
        {
            bookingManager.AddBooking("Dennis", 101, DateTime.Today);
            Assert.Throws<RoomNotAvailableException>(() => bookingManager.AddBooking("Patel", 101, DateTime.Today));
        }

        [Test]
        public void WhenTheSameRoomIsBookedOnDifferentNightsThenBookingIsSuccessful()
        {
            bookingManager.Store.Add(new KeyValuePair<int, Booking>(101, new Booking("Dennis",DateTime.Today.AddDays(-1))));
            Assert.DoesNotThrow(() => bookingManager.AddBooking("Larry", 101, DateTime.Today));
        }

        [Test]
        public void WhenRoom102And203AreBookedThenAvailableRoomsAre101And201()
        {
            bookingManager.AddBooking("Dennis", 102, DateTime.Today);
            bookingManager.AddBooking("Patel", 203, DateTime.Today);

            var availableRooms = bookingManager.GetAvailableRooms(DateTime.Today);
           
            availableRooms.Should().HaveCount(2);
            availableRooms.Should().Contain(101).And.Contain(201);
        }

        [Test]
        public void WhenMultipleBookingsArePlacedSimultaneouslyThenBookingIsNotDuplicated()
        {
            var tasksList = new List<Task>();

            tasksList.Add(Task.Run(() => bookingManager.AddBooking("Dennis", 102, DateTime.Today)));
            tasksList.Add(Task.Run(() => bookingManager.AddBooking("Mike", 102, DateTime.Today)));
            tasksList.Add(Task.Run(() => bookingManager.AddBooking("Patel", 102, DateTime.Today)));
            tasksList.Add(Task.Run(() => bookingManager.AddBooking("Larry", 102, DateTime.Today)));
            tasksList.Add(Task.Run(() => bookingManager.AddBooking("Emma", 201, DateTime.Today)));
            tasksList.Add(Task.Run(() => bookingManager.AddBooking("Stacey", 203, DateTime.Today)));

           
            Assert.Throws<AggregateException>(() => Task.WaitAll(tasksList.ToArray()));
           
            bookingManager.Store.Count.Should().Be(3);

            foreach(var tsk in tasksList)            
                if(tsk.Status == TaskStatus.Faulted)                
                    tsk.Exception.InnerException.Should().BeOfType(typeof(RoomNotAvailableException));
                
            
        }
    }
}