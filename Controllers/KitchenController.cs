using FoodSync.Core.Model;
using FoodSync.Core.Model.Domain;
using FoodSync.Core.Persistence;
using FoodSync.Core.Persistence.Repositories;
using SyncFood.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SyncFood.Controllers
{
    public class KitchenController : Controller
    {
        SyncFoodContext objSyncFoodContext;
        // GET: Kitchen
        public ActionResult Index()
        {
            KitchenModel objKitchenModel = new KitchenModel();
            objKitchenModel.ProductList = new List<ProductItem>();

            objSyncFoodContext = new SyncFoodContext();
            using (IUnitOfWork unitOfWork = new UnitOfWork(objSyncFoodContext))
            {
                Kitchen objKitchen = unitOfWork.Kitchens.GetKitchenByName(DateTime.Now.ToShortDateString());
                int kitchenId = objKitchen.Id;

                FoodSync.Core.Model.Domain.User objUser = unitOfWork.Users.SingleOrDefault(u => u.UserName == User.Identity.Name);

                double totalCalloriesInKitchen = 0.0;
                List<KitchenProduct> lstProductsInKitchen = unitOfWork.Kitchens.GetProductsFromKitchen(kitchenId, objKitchen.Name,objUser.Id);
                foreach (KitchenProduct product in lstProductsInKitchen)
                {
                    ProductItem objPI = new ProductItem();
                    objPI.Id = product.UserKitchenId;
                    objPI.ProductId = product.ProductId;
                    objPI.Name = product.ProductName;
                    objKitchenModel.ProductList.Add(objPI);

                    totalCalloriesInKitchen += product.ProductCallories;
                }
                objKitchenModel.TotalCallories = totalCalloriesInKitchen;
                objKitchenModel.OptimalCalloriesPerDay = objUser.OptimalCalloriesPerDay;
            }
            return View(objKitchenModel);
        }

        public ActionResult Recycle(int userKitchenId)
        {
            objSyncFoodContext = new SyncFoodContext();
            using (IUnitOfWork unitOfWork = new UnitOfWork(objSyncFoodContext))
            {
                unitOfWork.Kitchens.Recycle(userKitchenId);
            }
            return RedirectToAction("Index");
        }
    }
}