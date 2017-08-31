using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Compare.Models
{
    public class CompareResult
    {
        public int Coincidences { get; set; }
        public int NewPrice { get; set; }
        public string Time { get; set; }
    }
}
