using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ProjectCoffee.Models
{
    public class Table
    {
        [Key]
        [Required]
        public int TableId { get; set; }
        public string Client { get; set; }
        [Required]
        public int Chairs { get; set; }
        [Required]
        public bool Occupied { get; set; }
    }
}