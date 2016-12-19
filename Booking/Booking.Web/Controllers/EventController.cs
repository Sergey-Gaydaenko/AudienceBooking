using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using AutoMapper;
using Booking.Models.EfModels;
using Booking.Repositories;
using Booking.Services.Interfaces;
using Booking.Services.Services;
using Booking.Web.Helpers;
using Booking.Web.ViewModels.AudienceMap;
using Booking.Web.ViewModels.Event;
using Microsoft.AspNet.Identity;

namespace Booking.Web.Controllers
{
    [HandleException]
    public class EventController : Controller
    {
        private readonly IEventService _eventService;
        private readonly IAudienceService _audienceService;
        private readonly IAudienceMapService _audienceMapService;

        public EventController()
        {
            var uof = new UnitOfWork();
            _audienceService = new AudienceService(uof);
            var usersService = new UsersService();
            var emailNotificationService = new EmailNotificationService();
            _eventService = new EventService(uof, usersService, emailNotificationService, _audienceService);
            _audienceMapService = new AudienceMapService(uof);
        }

        [HttpGet]
        public ActionResult DisplayEventPopup(Guid eventId)
        {
            var eventEntity = _eventService.GetEvent(eventId);
            var vm = Mapper.Map<Event, DisplayEventPopupViewModel>(eventEntity);
            vm.CanEdit = _eventService.CanEdit(User, eventEntity);

            return PartialView("_DisplayEventPopup", vm);
        }

        [HttpGet]
        public ActionResult Index(Guid eventId)
        {
            var eventEntity = _eventService.GetEvent(eventId);
            var vm = Mapper.Map<DisplayEventViewModel>(eventEntity);
            vm.CanEdit = _eventService.CanEdit(User, eventEntity);

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Participate(JoinEventViewModel joinEventVm)
        {
            _eventService.AddParticipant(joinEventVm.Email, joinEventVm.EventId);
            return RedirectToAction("Index", new {eventId = joinEventVm.EventId});
        }

        [HttpGet]
        [Authorize]
        public ActionResult GetNewEventPopup()
        {
            var audiences = _audienceMapService.GetAudienceMap(AudienceMapSelector.AudienceMapId).Audiences;
            var availableAudiences = audiences.Where(a => a.IsBookingAvailable)
                .ToDictionary(a => a.Id, a => a.Name);

            var date = DateTime.Now.AddHours(2);
            var newMinute = (date.Minute/10)*10;
            date = date.AddMinutes(newMinute - date.Minute);

            var viewModel = new CreateEditEventViewModel
            {
                AvailableAudiences = availableAudiences,
                EndTime = date.AddMinutes(30),
                StartTime = date,
                IsPublic = true
            };

            return PartialView("_NewEventPartial", viewModel);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveParticipant(Guid participantId)
        {
            _eventService.RemoveParticipant(User, participantId);
            return Redirect(Request.UrlReferrer.ToString());
        }

        [HttpGet]
        [Authorize]
        public ActionResult Edit(Guid eventId)
        {
            var eventEntity = _eventService.GetEvent(eventId);
            var vm = Mapper.Map<EventEditViewModel>(eventEntity);

            return View(vm);
        }

        [HttpGet]
        [Authorize]
        public ActionResult Create(Guid audienceId, DateTime eventDateTime, int duration)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateEditEventViewModel vm)
        {
            var isFree = _audienceService.IsFree(vm.AudienceId, vm.StartTime, vm.EndTime, vm.Id);

            var duration = vm.EndTime.Subtract(vm.StartTime).TotalMinutes;

            if (duration < 20 || !isFree)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (vm.IsPublic && string.IsNullOrWhiteSpace(vm.Title))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Event newEvent = Mapper.Map<Event>(vm);
            newEvent.AuthorId = User.Identity.GetUserId();

            _eventService.CreateEvent(newEvent);

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Cancel(Guid eventId)
        {
            _eventService.CancelEvent(User, eventId);
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        [HttpGet]
        [Authorize]
        public ActionResult GetCancellationForm(Guid eventId)
        {
            return PartialView("_CancelEventPartial", eventId);
        }

        [HttpGet]
        [Authorize]
        public ActionResult EditPopup(Guid eventId)
        {
            var eventEntity = _eventService.GetEvent(eventId);

            var viewModel = Mapper.Map<CreateEditEventViewModel>(eventEntity);

            return PartialView("_EditEventPopupPartial", viewModel);
        }

        [HttpPost]
        [Authorize]
        public ActionResult Save(EventEditViewModel vm)
        {
            TimeSpan span = vm.EndTime.Subtract(vm.StartTime);
            var duration = (int) span.TotalMinutes;

            var eventEntity = _eventService.GetEvent(vm.Id);

            if (!_audienceService.IsFree(vm.AudienceId, vm.StartTime, vm.EndTime, vm.Id) || duration < 20)
            {
                var model = _audienceMapService.GetAudienceMap(AudienceMapSelector.AudienceMapId);
                vm.AudienceMap = Mapper.Map<AudienceMapViewModel>(model);
                vm.ParticipantsEmails = eventEntity.EventParticipants.ToVmDictionary();
                return View("Edit", vm);
            }

            eventEntity.Title = vm.Title;
            eventEntity.AdditionalInfo = vm.AdditionalInfo;
            eventEntity.AudienceId = vm.AudienceId;
            eventEntity.AuthorName = vm.AuthorName;
            eventEntity.IsAuthorShown = vm.IsAuthorShown;
            eventEntity.IsJoinAvailable = vm.IsJoinAvailable;
            eventEntity.StartTime = vm.StartTime;
            eventEntity.EndTime = vm.EndTime;

            _eventService.UpdateEvent(User, eventEntity);

            return RedirectToAction("Index", new {eventId = eventEntity.Id});
        }

        [HttpPost]
        [Authorize]
        public ActionResult SaveFromPopup(CreateEditEventViewModel vm)
        {
            TimeSpan span = vm.EndTime.Subtract(vm.StartTime);
            var duration = (int) span.TotalMinutes;

            var eventEntity = _eventService.GetEvent(vm.Id);
            if (!_audienceService.IsFree(vm.AudienceId, vm.StartTime, vm.EndTime, vm.Id) || duration < 20)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            eventEntity.Title = vm.Title;
            eventEntity.AdditionalInfo = vm.AdditionalInfo;
            eventEntity.AudienceId = vm.AudienceId;
            eventEntity.AuthorName = vm.AuthorName;
            eventEntity.IsAuthorShown = vm.IsAuthorShown;
            eventEntity.IsJoinAvailable = vm.IsJoinAvailable;
            eventEntity.StartTime = vm.StartTime;
            eventEntity.EndTime = vm.EndTime;

            _eventService.UpdateEvent(User, eventEntity);

            return RedirectToAction("DisplayEventPopup", new {eventId = eventEntity.Id});
        }
    }
}