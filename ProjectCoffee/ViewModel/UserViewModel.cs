using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectCoffee.Models;

namespace ProjectCoffee.ViewModel
{
    public class UserViewModel
    {
        public Item Item { get; set; }

        public List<Item> Items { get; set; }

        public Table Table { get; set; }

        public List<Table> Tables { get; set; }
    }
}