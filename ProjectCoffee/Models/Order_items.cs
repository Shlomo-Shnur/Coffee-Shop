using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ProjectCoffee.Models
{
    public class Order_items
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }        
        [Key]
        [Required]
        public int Id { get; set; }
    }
}