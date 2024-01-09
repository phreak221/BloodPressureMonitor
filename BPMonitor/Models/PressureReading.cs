using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMonitor.Models
{
    [Table("PressureReading")]
    public class PressureReading
    {
        [Column("BPID")]
        [PrimaryKey, Unique, AutoIncrement]
        public int BPID { get; set; }

        [Column("BPReadingDate")]
        [NotNull]
        public string BPReadingDate { get; set; }

        [Column("BPReadingTime")]
        [NotNull]
        public string BPReadingTime { get; set; }

        [Column("BPSystolic")]
        [NotNull]
        public int BPSystolic { get; set; }

        [Column("BPDiastolic")]
        [NotNull]
        public int BPDiastolic { get; set; }

        [Column("BPPulseRate")]
        [NotNull]
        public int BPPulseRate { get; set; }

        [Column("BPStatus")]
        [NotNull]
        public string BPStatus { get; set; }

        [Column("BPOmitted")]
        [NotNull]
        public bool BPOmitted { get; set; }
    }
}
