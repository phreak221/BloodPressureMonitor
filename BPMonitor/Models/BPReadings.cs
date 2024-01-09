using SQLite;
using System;

namespace BPMonitor.Models
{
    [Table("BPReadings")]
    public class BPReadings
    {
        [Column("BPID")]
        [PrimaryKey, Unique, AutoIncrement]
        public int BPID { get; set; }

        [Column("BPTime")]
        [NotNull]
        public DateTime BPTime { get; set; }

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
