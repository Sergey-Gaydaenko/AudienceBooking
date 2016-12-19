﻿using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Booking.Models.EfModels;
using Booking.Web.AutoMapper.CustomResolvers;
using Booking.Web.ViewModels.Event;
using Booking.Web.ViewModels.Schedule;

namespace Booking.Web.AutoMapper.AutoMapperProfiles
{
    public class EventProfile : Profile
    {
        public EventProfile()
        {
            CreateMap<Event, DisplayEventPopupViewModel>()
                .ForMember(dest => dest.AuthorName,
                    opt => opt.ResolveUsing<EventAuthorNameResolver<DisplayEventPopupViewModel>>())
                .ForMember(dest => dest.EventDate, opt => opt.ResolveUsing<EventDateResolver>())
                .ForMember(dest => dest.AudienceName, opt => opt.MapFrom(source => source.Audience.Name))
                .ForMember(dest => dest.ParticipantsCount, opt => opt.MapFrom(source => source.EventParticipants.Count))
                .ForMember(dest => dest.Duration,
                    opt => opt.MapFrom(source => source.EndTime.Subtract(source.StartTime).TotalMinutes));

            CreateMap<Event, DisplayEventViewModel>()
                .ForMember(dest => dest.Duration, opt => opt.MapFrom(source => source.EndTime.Subtract(source.StartTime).TotalMinutes))
                .ForMember(dest => dest.AuthorName,
                    opt => opt.ResolveUsing<EventAuthorNameResolver<DisplayEventViewModel>>())
                .ForMember(dest => dest.AudienceName, opt => opt.MapFrom(source => source.Audience.Name))
                .ForMember(dest => dest.ParticipantsEmails,
                    opt =>
                        opt.MapFrom(source => source.EventParticipants.ToDictionary(p => p.Id, p => p.ParticipantEmail)))
                .ForMember(dest => dest.AudienceMap, opt => opt.MapFrom(source => source.Audience.AudienceMap));

            CreateMap<Event, DayScheduleItemViewModel>()
                .ForMember(dest => dest.Duration, opt => opt.MapFrom(source => source.EndTime.Subtract(source.StartTime).TotalMinutes));

            CreateMap<IEnumerable<Event>, DayScheduleViewModel>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(source => source));

            CreateMap<Event, CreateEditEventViewModel>()
                .ForMember(dest => dest.AuthorName,
                    opt => opt.ResolveUsing<EventAuthorNameResolver<CreateEditEventViewModel>>())
                    .ForMember(dest => dest.AvailableAudiences, opt => opt.ResolveUsing<EventAvailableAudiencesResolver>());

            CreateMap<Event, EventEditViewModel>()
                .ForMember(dest => dest.AuthorName,
                    opt => opt.ResolveUsing<EventAuthorNameResolver<EventEditViewModel>>())
                .ForMember(dest => dest.ParticipantsEmails,
                    opt =>
                        opt.MapFrom(source => source.EventParticipants.ToDictionary(p => p.Id, p => p.ParticipantEmail)))
                .ForMember(dest => dest.AudienceMap, opt => opt.MapFrom(source => source.Audience.AudienceMap));

            CreateMap<CreateEditEventViewModel, Event>();
        }
    }
}