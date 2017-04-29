using FoodSync.Core.Model;
using FoodSync.Core.Model.Domain;
using FoodSync.Core.Persistence;
using SyncFood.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SyncFood.Controllers
{
    public class EditUserController : Controller
    {
        private SyncFoodContext objSyncFoodContext;

        public EditUserController()
        {
            objSyncFoodContext = new SyncFoodContext();
        }

        // GET: EditUser
        public ActionResult Index()
        {
            if (Request.IsAuthenticated)
            {
                string userName = User.Identity.Name;

                using (IUnitOfWork unitOfWork = new UnitOfWork(objSyncFoodContext))
                {
                    FoodSync.Core.Model.Domain.User objUser = unitOfWork.Users.SingleOrDefault(user => user.UserName == userName);
                    if (objUser!=null)
                    {
                        EditUserModel objUserModel = new EditUserModel();
                        objUserModel.FirstName = objUser.FirstName;
                        objUserModel.LastName = objUser.LastName;
                        objUserModel.Country = objUser.Country;
                        objUserModel.Gender = (int)objUser.Gender;
                        objUserModel.Height = objUser.Height;
                        objUserModel.Weight = objUser.Weight;
                        objUserModel.DateOfBirth = objUser.DateOfBirth.ToShortDateString();
                        objUserModel.SelectedActivity = objUser.CurrentActivity;
                        
                        List<Activity> lstActivities = unitOfWork.Activity.GetAllActivities();
                        List<SelectListItem> lstItems = new List<SelectListItem>();
                        List<string> lstA = new List<string>();
                        //IEnumerable<SelectListItem> lstItems = new IEnumerable<SelectListItem>();
                        foreach (Activity activity in lstActivities)
                        {
                            lstA.Add(activity.Name);
                            SelectListItem objNewItem = new SelectListItem();
                            objNewItem.Text = activity.Name;
                            objNewItem.Value = activity.Name;
                            if (activity.Name == objUser.CurrentActivity)
                                objNewItem.Selected = true;
                            else
                                objNewItem.Selected = false;
                            lstItems.Add(objNewItem);
                        }
                        objUserModel.ActivityList = new SelectList(lstItems.ToArray());
                        objUserModel.ActivityList2 = lstA;
                        return View(objUserModel);
                    }
                }
            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Edit(EditUserModel editUser)
        {
            using (IUnitOfWork unitOfWork = new UnitOfWork(objSyncFoodContext))
            {
                //Ne zaboravaj da presmetas
                double optimalCalloriesPerDay = 0;
                double index = 1;
                string lifeStyleType = String.Empty;

                if (editUser.ActivityList != null)
                {
                    foreach (var item in editUser.ActivityList)
                    {
                        if (item.Selected == true)
                        {
                            lifeStyleType = item.Text;
                            break;
                        }
                    }
                }
                else
                {
                    if (editUser.ActivityList2.Count==1)
                    {
                        lifeStyleType = editUser.ActivityList2[0];
                        index = unitOfWork.Activity.GetActivityIndex(lifeStyleType);
                    }
                }


                string[] split = editUser.DateOfBirth.Split('/');
                int month = Convert.ToInt32(split[0]);
                int day = Convert.ToInt32(split[1]);
                int year = Convert.ToInt32(split[2]);

                DateTime DateOfBirth = DateOfBirth = new DateTime(year, month, day);
                DateTime now = DateTime.Today;
                int age = now.Year - DateOfBirth.Year;

                if (editUser.Gender==0)
                {
                    //Male
                    optimalCalloriesPerDay = 655 + (9.6 * editUser.Weight) + (1.8 * editUser.Height) - (4.7 * age);
                }
                else
                {
                    //Female
                    optimalCalloriesPerDay = 66 + (13.7 * editUser.Weight) + (5 * editUser.Height) - (6.8 * age);
                }

                optimalCalloriesPerDay *= index;

                unitOfWork.Users.UpdateUser(User.Identity.Name, editUser.FirstName, editUser.LastName, editUser.Country,
                    editUser.Gender, editUser.Height, editUser.Weight,optimalCalloriesPerDay,editUser.DateOfBirth
                    , lifeStyleType);
            }
            return RedirectToAction("Index", "Home");
        }
    }
}