using System;
using System.Collections.Generic;
using System.Linq;
using Booking.Models.EfModels;
using Booking.Repositories.Interfaces;
using Booking.Services.Interfaces;

namespace Booking.Services.Services
{
    public class BookingScheduleRuleService : IBookingScheduleRuleService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BookingScheduleRuleService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public bool CanBook(DateTime dateTime)
        {
            var dateOnly = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 23, 59, 59);
            var rule = _unitOfWork.BookingScheduleRuleRepository.RuleForDate(dateOnly);
            return dateTime.Hour >= rule.StartHour &&
                   (dateTime.Hour < rule.EndHour || (dateTime.Minute == 0 && dateTime.Hour == rule.EndHour));
        }

        public BookingScheduleRule GetRule(DateTime day)
        {
            var dateOnly = new DateTime(day.Year, day.Month, day.Day, 23, 59, 59);
            return _unitOfWork.BookingScheduleRuleRepository.RuleForDate(dateOnly);
        }

        public IEnumerable<BookingScheduleRule> GetAllBookingScheduleRules()
        {
            return _unitOfWork.BookingScheduleRuleRepository.GetAllRules();
        }

        public IList<BookingScheduleRule> GetRulesForMonth(int month, int year)
        {
            var query = _unitOfWork.BookingScheduleRuleRepository.GetAppliedRulesByMonth(month, year);

            var list = query.ToList();
            for (int day = 1; day <= 7; ++day)
            {
                list.Add(_unitOfWork.BookingScheduleRuleRepository.RuleForDate(new DateTime(year, month, day)));
            }

            return list;
        }

        public IEnumerable<int> GetDisabledDaysForMonth(int month, int year)
        {
            var rules = GetRulesForMonth(month, year);
            var days = new List<int>();

            for (int day = 1; day <= DateTime.DaysInMonth(year, month); ++day)
            {
                var date = new DateTime(year, month, day);
                var rule = rules.Where(r => r.DayOfWeek == date.DayOfWeek && r.AppliedDate < date)
                    .OrderByDescending(r => r.AppliedDate)
                    .FirstOrDefault();
                if (!BookingAvailable(rule))
                {
                    days.Add(day);
                }
            }

            return days;
        }

        public void CreateRule(BookingScheduleRule rule)
        {
            using (var transaction = _unitOfWork.Context.Database.BeginTransaction())
            {
                try
                {
                    var nextRule = _unitOfWork.BookingScheduleRuleRepository.NextRuleForDayOfWeek(rule.DayOfWeek,
                        rule.AppliedDate);


                    var allEvents = _unitOfWork.EventRepository.GetAllEvents();
                    var eventsBetween = nextRule != null
                        ? allEvents.Where(x => x.StartTime > rule.AppliedDate &&
                                               x.StartTime < nextRule.AppliedDate)
                        : allEvents.Where(x => x.StartTime > rule.AppliedDate);

                    var notMatchedEvents = eventsBetween.Where(x => x.StartTime.Hour < rule.StartHour ||
                                                                    (x.EndTime.Hour > rule.EndHour ||
                                                                     (x.EndTime.Hour == rule.EndHour &&
                                                                      x.EndTime.Minute != 0)));

                    var filteredByDayOfWeek =
                        notMatchedEvents.AsEnumerable().Where(x => x.StartTime.DayOfWeek == rule.DayOfWeek);

                    if (filteredByDayOfWeek.Any())
                    {
                        throw new InvalidOperationException(
                            "You cannot apply this rule, because there are events, which do not match it.");
                    }

                    var oldRule =
                        _unitOfWork.BookingScheduleRuleRepository.GetRuleByAppliedDateAndDayOfWeek(rule.AppliedDate,
                            rule.DayOfWeek);

                    if (oldRule != null)
                    {
                        _unitOfWork.BookingScheduleRuleRepository.DeleteRule(oldRule);
                    }

                    _unitOfWork.BookingScheduleRuleRepository.CreateRule(rule);
                    _unitOfWork.Save();

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public bool BookingAvailable(BookingScheduleRule rule)
        {
            return rule != null && rule.EndHour - rule.StartHour > 0;
        }

        public DateTime GetNextAvailableDate(DateTime date)
        {
            for (int i = 0; i < 30; i++)
            {
                date = date.AddDays(1);
                var rule = GetRule(date);
                if (BookingAvailable(rule))
                {
                    return new DateTime(date.Year, date.Month, date.Day, rule.StartHour, 0, 0);
                }
            }

            throw new Exception("Sorry, booking temporarily unavailable.");
        }

        public DateTime GetPreviousAvailableDate(DateTime date)
        {
            for (int i = 0; i < 30; i++)
            {
                date = date.AddDays(-1);
                var rule = GetRule(date);
                if (BookingAvailable(rule))
                {
                    return new DateTime(date.Year, date.Month, date.Day, rule.EndHour - 1, 40, 0);
                }
            }

            throw new Exception("No previous dates available.");
        }
    }
}