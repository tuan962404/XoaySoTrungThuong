using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XoaySoTrungThuong.Models
{
    public partial class PrizeRule
    {
        public DotQuay_Result DotQuay { get; set; }
        public int LanQuay { get; set; }
        public string GiaiThuong { get; set; }
        public int SoLuongGiai { get; set; }
        public PrizeRule prizerule { get; set; }
    }
}