using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KintaiAuto.ViewModel
{
    public class Kintai
    {
        public bool inputflg { get; set; }
        public DateTime Date { get; set; }

        public string StrTime { get; set; }
        public string strID { get; set; }
        
        public string EndTime { get; set; }
        public string endID { get; set; }

        public string KyuStrTime { get; set; }
        public string KyuEndTime { get; set; }
        public string RakuPtn { get; set; }

        public bool? Rakutrue { get; set; }

    }
    public class LoginsInfo
    {
        public String  Kigyo { get; set; }

        public string ID { get; set; }

        public string PASS { get; set; }


    }

    public class KintaiView
    {
        public List<Kintai> Kintais { get; set; }
    }

    public class RakuPtn
    {
        public string Id { get; set; }

        public string PtnName { get; set; }
    }
}
