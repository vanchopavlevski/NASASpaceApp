using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SyncFood.Models
{
    public class EditUserModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Country { get; set; }
        public int Gender { get; set; }
        public double Height { get; set; }
        public double Weight { get; set; }
        public string ImageFilePath { get; set; }
        public string DateOfBirth { get; set; }
        public SelectList ActivityList { get; set; }
        public List<string> ActivityList2 { get; set; }
        public string SelectedActivity { get; set; }
    }
}