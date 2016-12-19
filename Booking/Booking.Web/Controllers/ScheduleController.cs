using System;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using Booking.Repositories;
using Booking.Services.Interfaces;
using Booking.Services.Services;
using Booking.Web.ViewModels.Home;
using Booking.Web.ViewModels.Schedule;

namespace Booking.Web.Controllers
{
    public class ScheduleController : Controller
    {
        private readonly IScheduleService _scheduleService;
        private readonly IAudienceMapService _audienceMapService;
        private readonly IBookingScheduleRuleService _bookingScheduleRuleService;
        private readonly IAudienceService _audienceService;

        public ScheduleController()
        {
            var uof = new UnitOfWork();
            _scheduleService = new ScheduleService(uof.EventRepository);
            _audienceMapService = new AudienceMapService(uof);
            _bookingScheduleRuleService = new BookingScheduleRuleService(uof);
            _audienceService = new AudienceService(uof);
        }

        [HttpGet]
        public ActionResult GetDaySchedule(DateTime date, Guid audienceMapId)
        {
            var rule = _bookingScheduleRuleService.GetRule(date);

            var events = _scheduleService.GetEventsByDay(date, audienceMapId);
            var viewModel = Mapper.Map<DayScheduleViewModel>(events);
            viewModel.BookingHourStart = rule.StartHour;
            viewModel.BookingHourEnd = rule.EndHour;

            var audienceMap = _audienceMapService.GetAudienceMap(audienceMapId);
            var availableAudiences = audienceMap.Audiences.Where(a => a.IsBookingAvailable)
                .Select(a => new ScheduleRowName {Id = a.Id, Name = a.Name});
            viewModel.AvailableAudiences = availableAudiences;

            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetDayScheduleByAuthor(DateTime date, string authorId)
        {
            var rule = _bookingScheduleRuleService.GetRule(date);

            var events = _scheduleService.GetEventsByAuthor(authorId, date);
            var viewModel = Mapper.Map<DayScheduleViewModel>(events);
            viewModel.BookingHourStart = rule.StartHour;
            viewModel.BookingHourEnd = rule.EndHour;

            var availableAudiences = _audienceService.GetAllAudiences().Where(a => a.IsBookingAvailable)
                .Select(a => new ScheduleRowName {Id = a.Id, Name = a.Name});
            viewModel.AvailableAudiences = availableAudiences;

            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult GetScheduleRule(DateTime date)
        {
            var rule = _bookingScheduleRuleService.GetRule(date);
            var ruleVm = new ScheduleRuleVm
            {
                IsBookingAvailable = _bookingScheduleRuleService.BookingAvailable(rule),
                StartHour = rule.StartHour,
                EndHour = rule.EndHour
            };

            return Json(ruleVm, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetNextAvailableDate(DateTime date)
        {
            var next = _bookingScheduleRuleService.GetNextAvailableDate(date);
            var rule = _bookingScheduleRuleService.GetRule(next);
            var vm = new AvailableDayViewModel
            {
                Date = next,
                StartHour = rule.StartHour,
                EndHour = rule.EndHour
            };

            return Json(vm, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetPreviousAvailableDate(DateTime date)
        {
            var next = _bookingScheduleRuleService.GetPreviousAvailableDate(date);
            var rule = _bookingScheduleRuleService.GetRule(next);
            var vm = new AvailableDayViewModel
            {
                Date = next,
                StartHour = rule.StartHour,
                EndHour = rule.EndHour
            };

            return Json(vm, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetDisabledDaysForMonth(int month, int year)
        {
            var days = _bookingScheduleRuleService.GetDisabledDaysForMonth(month, year);
            var vm = new DisabledDaysViewModel
            {
                Days = days,
                Month = month,
                Year = year
            };

            return Json(vm, JsonRequestBehavior.AllowGet);
        }
    }
}