using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ProjectCoffee.Models
{
    public class Item
    {
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Information must contain between 1 and 100 letters")]
        public string Info { get; set; }
        [RegularExpression("^[1-9][0-9]*$", ErrorMessage = "Price Must Be Whole Number between 1 - infinity")]
        public int Price { get; set; }
        [Required(ErrorMessage = "Must Have Picture")]
        public Byte[] file { get; set; }
        public bool Availability { get; set; }
        [Key]
        public int idItem { get; set; }

    }
}