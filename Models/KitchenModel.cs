using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SyncFood.Models
{
    public class KitchenModel
    {
        public string Name { get; set; }
        public List<ProductItem> ProductList { get; set; }
        public double TotalCallories { get; set; }
        public double OptimalCalloriesPerDay { get; set; }
    }

    public class ProductItem
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string Name { get; set; }
    }
}