using System.ComponentModel.DataAnnotations;

namespace NGK_11.Models
{
    public class Location
    {
        [Key]
        public long LocationID { get; set; }
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

    }
}