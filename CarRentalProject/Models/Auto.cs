using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CarRentalProject.Models
{
    public class Auto
    {
        public int ID { get; set; }
        [Required]
        public EBrand Brand { get; set; }
        [Required]
        public string Model { get; set; }
        [Required]
        [Range(1,13)]
        public int PassengerCount { get; set; }
        [Required]
        [Range(10,2000)]
        public int PricePerDay { get; set; }
        [Required]
        public int Year { get; set; }
        [Required]
        public EColor Color { get; set; }
        public double Volume { get; set; }
        public byte[] ImageData { get; set; }
        [NotMapped]
        public IFormFile ImageMimeType { get; set; }
        
    }

    public enum EColor
    {
        Black,
        Blue,
        Red,
        Green,
        White,
        Yellow,
        Gray,
        Brown
    };

    public enum EBrand
    {
        BMW,
        Opel,
        Ford,
        Renault,
        Peugeot,
        Volvo,
        Tayota,
        Hyndai,
        Honda
    }
}
