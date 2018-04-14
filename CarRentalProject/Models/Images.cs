using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarRentalProject.Models
{
    public class Images
    {
        public int ID { get; set; }
        public int CarID { get; set; }
        public byte[] Image { get; set; }
    }
}
