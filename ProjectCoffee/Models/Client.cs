using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ProjectCoffee.Models
{
    public class Client
    {
        [Required]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "name must contain between 1 and 50 letters")]
        public string Name { get; set; }

        [Key]
        [Required]
        [RegularExpression("^[0][5][0-9]{8,}$", ErrorMessage = "phone number must begin with 05 and contain 10 numbres")]
        public string PhoneNum { get; set; }
        public bool VIP { get; set; }
        public int CCounter { get; set; }
    }
}