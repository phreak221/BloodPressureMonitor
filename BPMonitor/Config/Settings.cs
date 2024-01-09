using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMonitor.Config
{
    public static class Settings
    {
        public readonly static string DBPATH = $"{AppDomain.CurrentDomain.BaseDirectory}bplogs.sqlite";
    }
}
