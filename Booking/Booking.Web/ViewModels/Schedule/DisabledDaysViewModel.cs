﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Booking.Web.ViewModels.Schedule
{
    public class DisabledDaysViewModel
    {
        public IEnumerable<int> Days { get; set; }

        public int Month { get; set; }

        public int Year { get; set; }
    }
}