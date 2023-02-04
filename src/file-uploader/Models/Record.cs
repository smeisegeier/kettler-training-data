using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileUploader.Models
{
    public class Record
    {
        public int Id { get; set; }
        public int Pulse { get; set; }
        public int Power { get; set; }
        public int RPM { get; set; }

        public double TimePassed_minutes { get; set; }

        public double TimePassed_percent { get; set; }

        public double Score_10sec { get; set; }

        public Record() { }
    }
}
