using System;
using System.Collections.Generic;

namespace Manage_CLB_HTSV.ViewModels
{
    public class ThongKeViewModel
    {
        public decimal TongThu { get; set; }
        public decimal TongChi { get; set; }
        public decimal SoDu => TongThu - TongChi;
        public List<string> Labels { get; set; }
        public List<decimal> Data { get; set; }
    }
}
