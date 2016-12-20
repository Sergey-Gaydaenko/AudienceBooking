using System;
using System.Collections.Generic;
using System.Linq;
using Booking.Models;
using Booking.Models.EfModels;
using Booking.Repositories.Interfaces;
using Booking.Services.Interfaces;

namespace Booking.Services.Services
{
    public class ScheduleService : IScheduleService
    {
        private readonly IEventRepository _eventRepository;

        public ScheduleService(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }

        public IEnumerable<Event> GetEventsByDay(DateTime day, Guid audienceMapId)
        {
            return _eventRepository.GetEventsByAudienceMapId(audienceMapId)
                .Where(x => x.StartTime.Day == day.Day &&
                            x.StartTime.Month == day.Month &&
                            x.StartTime.Year == day.Year);
        }

        public IEnumerable<Event> GetEventsByAudience(Guid audienceId, DateTime from, DateTime to)
        {
            return _eventRepository.GetAllEvents().Where(x => x.AudienceId == audienceId && x.StartTime <= from &&
                                                              x.EndTime < to);
        }

        public IEnumerable<Event> GetEventsByAuthor(string userId, DateTime day)
        {
            return _eventRepository.GetAllEvents().Where(x => x.AuthorId == userId &&
                                                              x.StartTime.Day == day.Day &&
                                                              x.StartTime.Month == day.Month &&
                                                              x.StartTime.Year == day.Year);
        }

        public IEnumerable<Event> GetEventsByAuthor(ApplicationUser author)
        {
            return _eventRepository.GetAllEvents().Where(x => x.AuthorId == author.Id);
        }
    }
}