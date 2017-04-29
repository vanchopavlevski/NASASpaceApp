using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SyncFood.Models
{
    public class ProductModel
    {
        public static string SessionName = "Product";
        public string Name { get; set; }
        public string ImageControl { get; set; }
        public string ProductInfo { get; set; }
        public string Callories { get; set; }
        public string Season { get; set; }
        public string ShelfLife { get; set; }
        public string Condition { get; set; }
        public string CarbonFootPrint { get; set; }
        public string Allergens { get; set; }
        public string Hazards { get; set; }
    }
}