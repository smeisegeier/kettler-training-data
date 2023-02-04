using FileUploader.Controllers;
using System;
using DextersLabor;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FileUploader.Models
{
    public class Training
    {
        public int Id { get; set; }

        [StringLength(50)]
        public string Device { get; set; }

        [StringLength(50)]
        public string Calibration { get; set; }

        [StringLength(50)]
        public string Software { get; set; }

        [StringLength(50)]
        public string Date { get; set; }

        [StringLength(50)]
        public string Time { get; set; }

        [StringLength(50)]
        public string RecordIntervall { get; set; }

        [StringLength(50)]
        public string Transmission { get; set; }

        [StringLength(50)]
        public string Energy { get; set; }

        public List<Record> Records { get; set; }


        /* MetaData Area*/
        [XmlIgnore]
        [StringLength(50)]
        public string TrainingDateTime { get; set; }

        [XmlIgnore]
        public string FileName { get; set; }

        [XmlIgnore]
        public DateTime CreatedAt { get; set; }

        [XmlIgnore]
        public double Duration_minutes { get; set; }

        [XmlIgnore]
        public int Streak_days { get; set; } = 0;

        public Training() {}

        public static Training GetTrainingExample()
        {
            return new Training()
            {
                Calibration = "07655-350-2007",
                Date = "30.06.2018",
                Device = "SJ10X SKYLON 5",
                Energy = "6.0",
                RecordIntervall = "10",
                Software = "3367",
                Time = "15:48:52",
                Transmission = "9.5",
                Records = new List<Record>()
                {
                   new Record(){Power=105, Pulse=120, RPM=42},
                   new Record(){Power=110, Pulse=100, RPM=45},
                   new Record(){Power=125, Pulse=100, RPM=51},
                   new Record(){Power=105, Pulse=90, RPM=47},
                   new Record(){Power=115, Pulse=105, RPM=53}
                }
            };
        }

        /// <summary>
        /// Processes a Training object for saving
        /// </summary>
        /// <param name="processTime">Time of processing. Multiple files are bundled in one timeframe</param>
        /// <param name="fileName">filename</param>
        public void Process(DateTime processTime, string fileName)
        {
            FileName = fileName;
            CreatedAt = processTime;
            TrainingDateTime = DateTimeHelper.GetDateTimeStringFromPattern(Date + " " + Time, "dd.MM.yyyy HH:mm:ss");
            int recordCount = Records.Count();
            Duration_minutes = recordCount * 10d / 60d;
            int i = 1;
            foreach (var item in Records)
            {
                item.TimePassed_minutes = i * 10d / 60d;
                item.TimePassed_percent = item.TimePassed_minutes / Duration_minutes;
                // Standard Training: 130 W, 50 RPM, 180 intervals (30 minutes)
                item.Score_10sec = (item.Power / 130d) * (item.RPM / 50d) / 180d;
                i++;
            }
        }
    }
}
