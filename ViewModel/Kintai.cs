using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KintaiAuto.ViewModel
{
    public class Kintai
    {
        public DateTime Date { get; set; }

        public string StrTime { get; set; }
        
        public string EndTime { get; set; }

        public string KyuStrTime { get; set; }
        public string KyuEndTime { get; set; }
        public string RakuPtn { get; set; }

    }

    public class KintaiView
    {
        public List<Kintai> Kintais { get; set; }
    }
}
