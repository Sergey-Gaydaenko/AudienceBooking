using System;
using System.Linq;
using System.Security.Principal;
using Booking.Models.EfModels;
using Booking.Repositories.Interfaces;
using Booking.Services.Interfaces;
using Microsoft.AspNet.Identity;

namespace Booking.Services.Services
{
    public class EventService : IEventService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUsersService _usersService;
        private readonly IEmailNotificationService _emailNotificationService;
        private readonly IAudienceService _audienceService;

        public EventService(IUnitOfWork unitOfWork, IUsersService usersService,
            IEmailNotificationService emailNotificationService, IAudienceService audienceService)
        {
            _unitOfWork = unitOfWork;
            _usersService = usersService;
            _emailNotificationService = emailNotificationService;
            _audienceService = audienceService;
        }

        public void CreateEvent(Event eventEntity)
        {
            using (var transaction = _unitOfWork.Context.Database.BeginTransaction())
            {
                try
                {
                    if (!_audienceService.IsFree(eventEntity.AudienceId, eventEntity.StartTime,
                        eventEntity.EndTime, eventEntity.Id))
                    {
                        throw new InvalidOperationException("Sorry, you cannot book this audience for this period.");
                    }

                    _unitOfWork.EventRepository.CreateEvent(eventEntity);
                    _unitOfWork.Save();

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public Event GetEvent(Guid id)
        {
            return _unitOfWork.EventRepository.GetEventById(id);
        }

        public void CancelEvent(IPrincipal editor, Guid eventId)
        {
            var currentEvent = _unitOfWork.EventRepository.GetEventById(eventId);

            if (CanEdit(editor, currentEvent))
            {
                _unitOfWork.EventRepository.DeleteEvent(currentEvent);
                _emailNotificationService.EventCancelledAuthorNotification(currentEvent);
                _emailNotificationService.EventCancelledNotification(currentEvent);

                _unitOfWork.Save();
            }
            else
            {
                throw new UnauthorizedAccessException("You do not have access rights to cancel this event.");
            }
        }

        public void CancelEventsByAudience(Guid audienceId)
        {
            var eventsQuery = _unitOfWork.EventRepository.GetAllEvents()
                .Where(e => e.AudienceId == audienceId);

            foreach (var eventEntity in eventsQuery)
            {
                _unitOfWork.EventRepository.DeleteEvent(eventEntity);
                _emailNotificationService.EventCancelledAuthorNotification(eventEntity);
                _emailNotificationService.EventCancelledNotification(eventEntity);
            }
            _unitOfWork.Save();
        }
        public void CancelEventsByAuthor(string userId)
        {
            var eventsQuery = _unitOfWork.EventRepository.GetAllEvents().
                Where(e => e.AuthorId == userId);

            foreach (var eventEntity in eventsQuery)
            {
                _unitOfWork.EventRepository.DeleteEvent(eventEntity);
                _emailNotificationService.EventCancelledAuthorNotification(eventEntity);
                _emailNotificationService.EventCancelledNotification(eventEntity);
            }

            _unitOfWork.Save();
        }

        public void UpdateEvent(IPrincipal editor, Event eventEntity)
        {
            using (var transaction = _unitOfWork.Context.Database.BeginTransaction())
            {
                try
                {
                    var oldEvent = _unitOfWork.EventRepository.GetEventById(eventEntity.Id);
                    if (CanEdit(editor, oldEvent))
                    {
                        if (!_audienceService.IsFree(eventEntity.AudienceId, eventEntity.StartTime, eventEntity.EndTime, eventEntity.Id))
                        {
                            throw new InvalidOperationException("Sorry, but someone already booked this audience for the specified period.");
                        }

                        _unitOfWork.EventRepository.UpdateEvent(eventEntity);
                        _emailNotificationService.EventEditedAuthorNotification(eventEntity, oldEvent);
                        _emailNotificationService.EventEditedNotification(eventEntity, oldEvent);

                        _unitOfWork.Save();
                    }
                    else
                    {
                        throw new UnauthorizedAccessException(Resources.Resources.YouHaveNotRightsToEditEvent);
                    }

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public void AddParticipant(string email, Guid eventId)
        {
            var eventEntity = _unitOfWork.EventRepository.GetEventById(eventId);

            if (eventEntity.IsJoinAvailable)
            {
                var newEventParticipant = new EventParticipant
                {
                    EventId = eventEntity.Id,
                    ParticipantEmail = email,
                    Event = eventEntity
                };

                eventEntity.EventParticipants.Add(newEventParticipant);
                _emailNotificationService.EventJoinedNotification(email, eventEntity);
                _unitOfWork.Save();
            }
            else
            {
                throw new InvalidOperationException(Resources.Resources.YouCantJoinEvent);
            }
        }

        public void RemoveParticipant(IPrincipal editor, Guid participantId)
        {
            var participant = _unitOfWork.EventParticipantRepository.GetEventParticipantById(participantId);
            var eventEntity = _unitOfWork.EventRepository.GetEventById(participant.EventId);

            if (CanEdit(editor, eventEntity))
            {
                _unitOfWork.EventParticipantRepository.DeleteEventParticipant(participant);
                _unitOfWork.Save();

                _emailNotificationService.RemovedFromParticipantsListNotification(participant.ParticipantEmail,
                    eventEntity);
            }
            else
            {
                throw new InvalidOperationException(Resources.Resources.YouHaveNotRightsToRemoveParticipant);
            }
        }

        public bool CanEdit(IPrincipal userPrincipal, Event eventEntity)
        {
            if (!userPrincipal.Identity.IsAuthenticated) return false;

            var userId = userPrincipal.Identity.GetUserId();

            return (userId == eventEntity.AuthorId) || _usersService.IsAdmin(userPrincipal);
        }

        public bool CanEdit(IPrincipal user, Guid eventId)
        {
            var eventEntity = _unitOfWork.EventRepository.GetEventById(eventId);
            return CanEdit(user, eventEntity);
        }
    }
}