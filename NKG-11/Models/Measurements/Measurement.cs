using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace NGK_11.Models
{
    public class Measurement
    {
        public long MeasurementID {get;set;}
        public DateTime TimeOfMeasurement { get; set; }
        [ForeignKey("LocationFK")]
        public Location LocationOfMeasurement { get; set; }
        [Column(TypeName = "decimal(1, 1)")]
        public double Temperature { get; set; }
        public int Humidity { get; set; }
        [Column(TypeName = "decimal(1, 1)")]
        public double AtmosphericPressure { get; set; }
    }
}
