using System;
using System.Collections.Generic;
using Manage_CLB_HTSV.Models;

namespace Manage_CLB_HTSV.Models.ViewModel
{
    public class HomeViewModel
    {
        public List<TinTuc> LatestNews { get; set; }
        public List<HoatDong> UpcomingActivities { get; set; }
    }
}
