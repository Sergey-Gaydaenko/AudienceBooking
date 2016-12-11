﻿using System;
using System.Security.Principal;
using Booking.Models;

namespace Booking.Services.Interfaces
{
    public interface IEventService
    {
        void CreateEvent(Event eventEntity);

        Event GetEvent(Guid id);

        void CancelEvent(IPrincipal editor, Guid eventId);

        void UpdateEvent(IPrincipal editor, Event eventEntity);

        void AddParticipant(string email, Guid eventId);

        void RemoveParticipant(IPrincipal editor, Guid participantId);

        bool CanEdit(IPrincipal user, Event eventEntity);
    }
}