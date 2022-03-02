using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ProjectCoffee.Models
{
    public class Order
    {
        public string Client { get; set; }

        [Key]
        [Required]
        public int OrderId { get; set; }
        public bool Status { get; set; }
    }
}