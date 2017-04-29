using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SyncFood.Models
{
    public class TopUserInfo
    {
        public static string SessionName = "UserInfo";
        public string Name { get; set; }
        public int Age { get; set; }
        public double Height { get; set; }
        public double Weight { get; set; }
        public double OptimalCalloriesPerDay { get; set; }
        public string ImagePath { get; set; }

        public ProductModel Product { get; set; }

        public List<RecipeModel> lstRecipes { get; set; }
    }
}