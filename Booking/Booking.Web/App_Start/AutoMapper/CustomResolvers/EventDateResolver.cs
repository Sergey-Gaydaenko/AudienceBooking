﻿using System.Globalization;
using AutoMapper;
using Booking.Models;
using Booking.Web.ViewModels.Event;

namespace Booking.Web.AutoMapper.CustomResolvers
{
    public class EventDateResolver : IValueResolver<Event, DisplayEventPopupViewModel, string>
    {
        public string Resolve(Event source, DisplayEventPopupViewModel destination, string destMember,
            ResolutionContext context)
        {
            CultureInfo culture = CultureInfo.CreateSpecificCulture("ru-RU");
            return source.EventDateTime.ToString("ddd, d MMMM", culture);
        }
    }
}