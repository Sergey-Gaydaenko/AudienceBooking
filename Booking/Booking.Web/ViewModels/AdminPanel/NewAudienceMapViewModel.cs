﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Booking.Web.ViewModels.AdminPanel
{
    public class NewAudienceMapViewModel
    {
        public string Name { get; set; }

        public HttpPostedFileBase Image { get; set; }
    }
}