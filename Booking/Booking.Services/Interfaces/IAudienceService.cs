﻿using System;
using System.Collections.Generic;
using Booking.Enums;
using Booking.Models;
using Booking.Models.EfModels;

namespace Booking.Services.Interfaces
{
    public interface IAudienceService
    {
        Audience GetAudience(Guid audienceId);

        void CreateAudience(Audience audience);

        void UpdateAudience(Audience audience);

        void DeleteAudienceById(Guid id);

        void CloseAudience(Guid audienceId);

        void OpenAudience(Guid audienceId);

        bool IsFree(Guid audienceId, DateTime dateTime, int duration, Guid? eventId);

        IEnumerable<Audience> GetAllAudiences();

        IEnumerable<Audience> GetAvailableAudiences();

        string GetStyleString(Audience audience);
    }
}