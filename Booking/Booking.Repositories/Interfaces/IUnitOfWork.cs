﻿using System;
using Booking.Models;

namespace Booking.Repositories.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IAudienceRepository AudienceRepository { get; }

        IEventRepository EventRepository { get; }

        IEventParticipantRepository EventParticipantRepository { get; }

        IAudienceMapRepository AudienceMapRepository { get; }

        IBookingScheduleRuleRepository BookingScheduleRuleRepository { get; }

        BookingDbContext Context { get; }

        void Save();
    }
}