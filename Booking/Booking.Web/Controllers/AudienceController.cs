using System;
using System.Web.Mvc;
using AutoMapper;
using Booking.Models.EfModels;
using Booking.Repositories;
using Booking.Services.Interfaces;
using Booking.Services.Services;
using Booking.Web.Helpers;
using Booking.Web.ViewModels.Audience;

namespace Booking.Web.Controllers
{
    [HandleException]
    public class AudienceController : Controller
    {
        private readonly IAudienceService _audienceService;

        public AudienceController()
        {
            var uof = new UnitOfWork();
            _audienceService = new AudienceService(uof);
        }

        [HttpGet]
        public ActionResult Index(Guid audienceId)
        {
            var model = _audienceService.GetAudience(audienceId);
            var vm = Mapper.Map<AudienceIndexViewModel>(model);
            return View(vm);
        }

        [HttpGet]
        public ActionResult GetAudienceInfo(Guid audienceId)
        {
            var audience = _audienceService.GetAudience(audienceId);
            var vm = Mapper.Map<AudienceInfoViewModel>(audience);

            return PartialView("_AudienceInfoPartial", vm);
        }

        [HttpGet]
        public ActionResult GetSchedule(Guid audienceId, DateTime startDate, DateTime endDate)
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        public ActionResult IsFree(Guid audienceId, DateTime startEvent, DateTime endEvent, Guid? eventId)
        {
            var isFree = _audienceService.IsFree(audienceId, startEvent, endEvent, eventId);
            return Content(isFree.ToString());
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Open(Guid audienceId)
        {
            _audienceService.OpenAudience(audienceId);
            return RedirectToAction("Index", new {audienceId = audienceId});
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult GetClosePopup(Guid audienceId)
        {
            return PartialView("_AudienceCancelPopup", audienceId);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Close(Guid audienceId)
        {
            _audienceService.CloseAudience(audienceId);
            return RedirectToAction("Index", new {audienceId= audienceId});
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(Guid audienceId)
        {
            var model = _audienceService.GetAudience(audienceId);
            var vm = Mapper.Map<AudienceIndexViewModel>(model);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Save(AudienceInfoViewModel audienceInfoViewModel)
        {
            var audience = _audienceService.GetAudience(audienceInfoViewModel.Id);
            audience.IsBookingAvailable = audienceInfoViewModel.IsBookingAvailable;
            audience.BoardsCount = audienceInfoViewModel.BoardsCount;
            audience.LaptopsCount = audienceInfoViewModel.LaptopsCount;
            audience.PrintersCount = audienceInfoViewModel.PrintersCount;
            audience.ProjectorsCount = audienceInfoViewModel.ProjectorsCount;
            audience.SeatsCount = audienceInfoViewModel.SeatsCount;
            audience.Name = audienceInfoViewModel.Name;
            
            _audienceService.UpdateAudience(audience);

            return View("Index");
        }
    }
}