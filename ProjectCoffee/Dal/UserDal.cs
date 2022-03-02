using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using ProjectCoffee.Models;

namespace ProjectCoffee.Dal
{
    public class UserDal : DbContext
    {
        public DbSet<Client> Clients { get; set; }
        public DbSet<Staff> Staffs { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Order_items> orderedItems { get; set; }
        public DbSet<Table> Tables { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Client>().ToTable("ClientTbl");
            modelBuilder.Entity<Staff>().ToTable("StaffTbl");
            modelBuilder.Entity<Item>().ToTable("ItemTbl");
            modelBuilder.Entity<Order>().ToTable("Order_ClientTbl");
            modelBuilder.Entity<Order_items>().ToTable("Items_OrderTbl");
            modelBuilder.Entity<Table>().ToTable("SittingTbl");

        }

    }
}