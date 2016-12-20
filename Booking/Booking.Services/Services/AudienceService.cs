using System;
using System.Collections.Generic;
using System.Linq;
using Booking.Models.EfModels;
using Booking.Repositories.Interfaces;
using Booking.Services.Interfaces;

namespace Booking.Services.Services
{
    public class AudienceService : IAudienceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBookingScheduleRuleService _bookingScheduleRuleService;
        private readonly IEventService _eventService;

        public AudienceService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _bookingScheduleRuleService = new BookingScheduleRuleService(_unitOfWork);
            _eventService = new EventService(unitOfWork, new UsersService(), new EmailNotificationService(), this);
        }

        public Audience GetAudience(Guid audienceId)
        {
            return _unitOfWork.AudienceRepository.GetAudienceById(audienceId);
        }

        public void CreateAudience(Audience audience)
        {
            _unitOfWork.AudienceRepository.CreateAudience(audience);
            _unitOfWork.Save();
        }

        public void UpdateAudience(Audience audience)
        {
            _unitOfWork.AudienceRepository.UpdateAudience(audience);
            _unitOfWork.Save();
        }

        public void DeleteAudienceById(Guid id)
        {
            _unitOfWork.AudienceRepository.DeleteAudienceById(id);
            _unitOfWork.Save();
        }

        public void CloseAudience(Guid audienceId)
        {
            var currentAudience = _unitOfWork.AudienceRepository.GetAudienceById(audienceId);
            if (currentAudience != null)
            {
                _eventService.CancelEventsByAudience(audienceId);
                currentAudience.IsBookingAvailable = false;
                _unitOfWork.AudienceRepository.UpdateAudience(currentAudience);
                _unitOfWork.Save();
            }
        }

        public void OpenAudience(Guid audienceId)
        {
            var currentAudience = _unitOfWork.AudienceRepository.GetAudienceById(audienceId);
            if (currentAudience != null)
            {
                currentAudience.IsBookingAvailable = true;
                _unitOfWork.AudienceRepository.UpdateAudience(currentAudience);
                _unitOfWork.Save();
            }
        }

        public bool IsFree(Guid audienceId, DateTime eventStart, DateTime eventEnd, Guid? currentEventId)
        {
            if (!_bookingScheduleRuleService.CanBook(eventStart)) return false;

            var events = _unitOfWork.EventRepository.GetAllEvents()
                .Where(x => x.AudienceId == audienceId && x.Id != currentEventId.Value &&
                            ((x.StartTime < eventStart && x.EndTime > eventStart) ||
                             (x.StartTime < eventEnd) && (x.EndTime > eventEnd) ||
                            (eventStart < x.StartTime) && (eventEnd > x.StartTime) ||
                            (x.StartTime == eventStart) && (x.EndTime == eventEnd)));

            return !events.Any();
        }

        public IEnumerable<Audience> GetAllAudiences()
        {
            return _unitOfWork.AudienceRepository.GetAllAudiences();
        }

        public string GetStyleString(Audience audience)
        {
            return string.Format("left:{0}px; top: {1}px; width: {2}px; height: {3}px",
                audience.Left, audience.Top, audience.Width, audience.Height);
        }
    }
}