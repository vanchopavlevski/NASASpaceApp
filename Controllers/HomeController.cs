using FoodSync.Core.Model;
using FoodSync.Core.Model.Domain;
using FoodSync.Core.Persistence;
using FoodSync.Core.Persistence.Repositories;
using SyncFood.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SyncFood.Controllers
{
    public class HomeController : Controller
    {
        private SyncFoodContext objSyncFoodContext;
        public ActionResult Index()
        {
            if (Request.IsAuthenticated)
            {
                objSyncFoodContext = new SyncFoodContext();
                using (IUnitOfWork unitOfWork = new UnitOfWork(objSyncFoodContext))
                {
                    FoodSync.Core.Model.Domain.User objUser = unitOfWork.Users.SingleOrDefault(user => user.UserName == User.Identity.Name);
                    if (objUser!=null)
                    {
                        TopUserInfo objTUI = null;
                        if (Session[TopUserInfo.SessionName] != null)
                            objTUI = (TopUserInfo)Session[TopUserInfo.SessionName];
                        else
                            objTUI = new TopUserInfo();
                        
                        objTUI.Name = String.Format("{0} {1}", objUser.FirstName, objUser.LastName);
                        objTUI.OptimalCalloriesPerDay = objUser.OptimalCalloriesPerDay;
                        
                        DateTime now = DateTime.Today;
                        objTUI.Age = now.Year - objUser.DateOfBirth.Year;
                        objTUI.Height = objUser.Height;
                        objTUI.Weight = objUser.Weight;

                        string[] files = Directory.GetFiles(Server.MapPath("~/Images"));
                        if (files.Length == 1)
                        {
                            objTUI.ImagePath = files[0];
                            Python.Python objPython = new Python.Python();
                            string classifierPath = @"C:\Users\Vancho\Desktop\NASA Space App\Python\Food Classifier";
                            string recognizedProductClass = objPython.ProcessImage(files[0], "SyncFoodClassifier.py",classifierPath);
                            string condition = "fresh";
                            //nemame rasipani od drugite proizvodi
                            if (recognizedProductClass=="banana")
                                condition = objPython.ProcessImage(files[0], "SpoiledClassifier.py", @"C:\Users\Vancho\Desktop\NASA Space App\Python\Spoiled Classifier");
                            Product objProduct = unitOfWork.Products.GetProductByName(recognizedProductClass);
                            ProductModel objProductModel = new ProductModel();
                            objProductModel.Name = String.Empty;

                            if (objProduct != null)
                            {
                                objProductModel.Name = objProduct.Name;
                                objProductModel.ProductInfo = objProduct.Info;
                                objProductModel.Callories = objProduct.Calories.ToString();
                                objProductModel.Season = objProduct.Season.ToString();
                                objProductModel.ShelfLife = objProduct.ShelfLife.ToString() + " days";
                                objProductModel.Condition = condition;
                                objProductModel.CarbonFootPrint = objProduct.CarbonFootPrint.ToString();
                                objProductModel.Allergens = objProduct.Allergens.ToString();
                                objProductModel.Hazards = objProduct.Hazards.ToString();

                                Session[ProductModel.SessionName] = objProduct.Name;
                            }

                            objTUI.Product = objProductModel;

                            objTUI.ImagePath = String.Format("~/Images/Product.jpg?t={0}",DateTime.Now.ToShortTimeString());
                        }

                        //Proveri za recepti
                        objTUI.lstRecipes = new List<RecipeModel>();
                        List<KitchenProduct> lstProducts = unitOfWork.Kitchens.GetProductsFromKitchen(DateTime.Today.ToShortDateString(), objUser.Id);
                        List<int> lstOriginals = new List<int>();
                        foreach (var product in lstProducts)
                        {
                            if (lstOriginals.Contains(product.ProductId)) continue;

                            lstOriginals.Add(product.ProductId);
                            List<Recipe> lstR = unitOfWork.Recipes.GetRecipesForProduct(product.ProductId);
                            foreach (var itemR in lstR)
                            {
                                RecipeModel objNewRecipe = new RecipeModel();
                                objNewRecipe.RecipeName = itemR.RecipeName;
                                objNewRecipe.RecipeDescription = itemR.Instructions;
                                objTUI.lstRecipes.Add(objNewRecipe);
                            }
                        }

                        Session[TopUserInfo.SessionName] = objTUI;

                        return View(objTUI);
                    }
                }
            }
            return View();
        }
        
        public ActionResult About()
        {
            if (Request.IsAuthenticated)
            {
                string temp = User.Identity.Name;
            }
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult UploadFile(HttpPostedFileBase file)
        {
            try
            {
                string[] files = Directory.GetFiles(Server.MapPath("~/Images"));
                foreach (string file2 in files)
                    System.IO.File.Delete(file2);

                if (file.ContentLength > 0)
                {
                    string _FileName = "Product.jpg";
                    string _path = Path.Combine(Server.MapPath("~/Images"), _FileName);
                    file.SaveAs(_path);
                }
                ViewBag.Message = "File Uploaded Successfully!!";
                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.Message = "File upload failed!!";
                return RedirectToAction("Index");
            }
        }
        
        public ActionResult ProposeRecipe()
        {
            string productName = String.Empty;
            if (Session[ProductModel.SessionName] != null)
                productName = Session[ProductModel.SessionName].ToString();

            TopUserInfo objTUI = null;

            objSyncFoodContext = new SyncFoodContext();
            using (IUnitOfWork unitOfWork = new UnitOfWork(objSyncFoodContext))
            {
                FoodSync.Core.Model.Domain.User objUser = unitOfWork.Users.SingleOrDefault(u => u.UserName == User.Identity.Name);

                if (Session[TopUserInfo.SessionName]!=null)
                {
                    objTUI = Session[TopUserInfo.SessionName] as TopUserInfo;
                    objTUI.lstRecipes = new List<RecipeModel>();
                    List<KitchenProduct> lstProducts = unitOfWork.Kitchens.GetProductsFromKitchen(DateTime.Today.ToShortDateString(), objUser.Id);
                    List<int> lstOriginals = new List<int>();
                    foreach (var product in lstProducts)
                    {
                        if (lstOriginals.Contains(product.ProductId)) continue;

                        lstOriginals.Add(product.ProductId);
                        List<Recipe> lstR = unitOfWork.Recipes.GetRecipesForProduct(product.ProductId);
                        foreach (var itemR in lstR)
                        {
                            RecipeModel objNewRecipe = new RecipeModel();
                            objNewRecipe.RecipeName = itemR.RecipeName;
                            objNewRecipe.RecipeDescription = itemR.Instructions;
                            objTUI.lstRecipes.Add(objNewRecipe);
                        }
                    }
                }
            }

            return RedirectToAction("Index");
        }

        public ActionResult AddToKitchen()
        {
            string productName = String.Empty;
            if (Session[ProductModel.SessionName] != null)
                productName = Session[ProductModel.SessionName].ToString();

            KitchenModel objKitchenModel = new KitchenModel();

            objSyncFoodContext = new SyncFoodContext();
            using (IUnitOfWork unitOfWork = new UnitOfWork(objSyncFoodContext))
            {
                Product objProduct = unitOfWork.Products.GetProductByName(productName);
                FoodSync.Core.Model.Domain.User objUser = unitOfWork.Users.SingleOrDefault(u => u.UserName == User.Identity.Name);
                Kitchen objKitchen = unitOfWork.Kitchens.GetKitchenByName(DateTime.Now.ToShortDateString());

                if (objKitchen == null)
                {
                    objKitchen = new Kitchen();
                    objKitchen.Name = DateTime.Today.ToShortDateString();
                    unitOfWork.Kitchens.Add(objKitchen);
                    unitOfWork.Complete();
                }

                if (objProduct == null)
                    return RedirectToAction("Index", "Home");

                int kitchenId = objKitchen.Id;
                int userId = objUser.Id;
                int productId = objProduct.Id;

                bool userKitchenExists = unitOfWork.Kitchens.UserKitchenExists(userId, kitchenId);
                if (userKitchenExists)
                {
                    unitOfWork.Kitchens.AddProductToKitchen(productId, userId, kitchenId, DateTime.Today.ToShortDateString());
                    /*
                    objKitchenModel.ProductList = new List<ProductItem>();
                    List<KitchenProduct> lstProductsInKitchen = unitOfWork.Kitchens.GetProductsFromKitchen(kitchenId, objKitchen.Name);
                    foreach (KitchenProduct product in lstProductsInKitchen)
                    {
                        ProductItem objPI = new ProductItem();
                        objPI.Id = product.UserKitchenId;
                        objPI.ProductId = product.ProductId;
                        objPI.Name = product.ProductName;
                        objKitchenModel.ProductList.Add(objPI);
                    }
                    */
                }
                else
                {   
                    unitOfWork.Kitchens.AddProductToKitchen(productId, userId, objKitchen.Id, DateTime.Today.ToShortDateString());
                    /*
                    objKitchenModel.ProductList = new List<ProductItem>();
                    List<KitchenProduct> lstProductsInKitchen = unitOfWork.Kitchens.GetProductsFromKitchen(kitchenId, objKitchen.Name);
                    foreach (KitchenProduct product in lstProductsInKitchen)
                    {
                        ProductItem objPI = new ProductItem();
                        objPI.Id = product.UserKitchenId;
                        objPI.ProductId = product.ProductId;
                        objPI.Name = product.ProductName;
                        objKitchenModel.ProductList.Add(objPI);
                    }
                    */
                }

                if (unitOfWork.DataSet.Exists(objUser.Id, DateTime.Today.ToShortDateString()))
                {
                    unitOfWork.DataSet.UpdateTrainingSet(objUser.Id, DateTime.Today.ToShortDateString(), 1);
                }
                else
                {
                    unitOfWork.DataSet.AddTrainingSet(objUser.Id, (int)DateTime.Now.DayOfWeek, 0, 1, DateTime.Today.ToShortDateString());
                }

                objKitchenModel.Name = objProduct.Name;
            }
            return RedirectToAction("Index");
        }

        public ActionResult TrainKitchen()
        {
            objSyncFoodContext = new SyncFoodContext();
            using (IUnitOfWork unitOfWork = new UnitOfWork(objSyncFoodContext))
            {
                FoodSync.Core.Model.Domain.User objUser = unitOfWork.Users.SingleOrDefault(u => u.UserName == User.Identity.Name);
                List<UserKitchen> lstUserKitchen = unitOfWork.Kitchens.GetLatestProductFromKitchen(DateTime.Today.ToShortDateString(), objUser.Id);
                List<int> lstUniqueProducts = new List<int>();
                string latestRecord = lstUserKitchen[0].Name;
                if (latestRecord != DateTime.Today.ToShortDateString())
                {
                    foreach(UserKitchen item in lstUserKitchen)
                    {
                        if (lstUniqueProducts.Contains(item.ProductId)) continue;
                        lstUniqueProducts.Add(item.ProductId);
                    }

                    foreach(var productId in lstUniqueProducts)
                    {
                        int quantity  = unitOfWork.Kitchens.GetNumberOfItems(latestRecord, objUser.Id, productId);

                        unitOfWork.DataSet.AddTrainingSet(objUser.Id, (int)DateTime.Today.DayOfWeek,
                            0, quantity, DateTime.Today.ToShortDateString());
                    }
                    //Save the Kitchen
                    //unitOfWork.DataSet.AddTrainingSet(objUser.Id,(int)DateTime.Today.DayOfWeek,0,)
                }
            }
                return Redirect("Index");
        }
    }
}