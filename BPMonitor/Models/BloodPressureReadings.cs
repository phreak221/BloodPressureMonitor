using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMonitor.Models
{
    public class BloodPressureReadings
    {
        public BPReadings BloodPressure { get; set; }
        public DateTime LogDateTime { get; set; }
        public string PressureStatus { get; set; }
    }
}
