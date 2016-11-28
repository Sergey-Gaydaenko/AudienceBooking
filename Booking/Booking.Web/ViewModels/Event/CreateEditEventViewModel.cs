﻿using System;
using Booking.Enums;
using Booking.Web.ViewModels.Audience;

namespace Booking.Web.ViewModels.Event
{
    public class CreateEditEventViewModel
    {
        public Guid Id { get; set; }

        public int EventDay { get; set; }

        public int EventMonth { get; set; }

        public int StartHour { get; set; }

        public int StartMinute { get; set; }

        public int EndHour { get; set; }

        public int EndMinute { get; set; }

        public string Title { get; set; }

        public string AdditionalInfo { get; set; }

        public bool IsAuthorShown { get; set; }

        public string AuthorName { get; set; }

        public bool IsJoinAvailable { get; set; }

        public bool IsPublic { get; set; }

        public AudiencesNamesViewModel AvailableAudiences { get; set; }

        public AudiencesEnum ChosenAudience { get; set; }

        public int LowerHourBound { get; set; }

        public int HigherHourBound { get; set; }
    }
}