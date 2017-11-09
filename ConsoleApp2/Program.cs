using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConsoleApp2
{

    class Program
    {
        static void Main(string[] args)
        {
            using (var db = new ProdContext())
            {
                CategoryForm cf = new CategoryForm();

                var query = from c in db.Categories
                            orderby c.Name descending
                            select c;
                cf.Load();
                cf.ShowDialog();
            }
        }
    }

    public class Category
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual List<Product> Products { get; set; }
    }

    public class Product
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public int UnitsInStock { get; set; }
        public int CategoryID { get; set; }
        [Column(TypeName = "money")]
        public decimal Unitprice { get; set; }

        public virtual List<Order> Orders { get; set; }

    }

    public class Customer
    {
        [Key]
        public string CompanyName { get; set; }
        public string Description { get; set; }
        public virtual List<Order> Orders { get; set; }
    }

    public class Order
    {
        public int OrderID { set; get; }
        public string CustomerID { set; get; }
        public int ProductID { set; get; }
        public int Quantity { set; get; }
    }

    public class ProdContext : DbContext
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
    }
}
