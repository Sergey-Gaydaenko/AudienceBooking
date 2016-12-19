﻿using Booking.Models;
using Booking.Models.EfModels;

namespace Booking.Services.Interfaces
{
    public interface IEmailNotificationService
    {
        void AccountRegisteredNotification(ApplicationUser user);

        void AccountRemovedNotification(ApplicationUser user);

        void EventCancelledNotification(Event eventEntity);

        void EventCancelledAuthorNotification(Event eventEntity);

        void RemovedFromParticipantsListNotification(string email, Event eventEntity);

        void EventJoinedNotification(string email, Event eventEntity);

        void EventEditedNotification(Event newEvent, Event oldEvent);

        void EventEditedAuthorNotification(Event newEvent, Event oldEvent);

        void SendFeedbackToAdmins(string name, string surname, string email, string message);

        void ConfirmEmailAddress(ApplicationUser user, string emailBody);
    }
}