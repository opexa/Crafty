namespace Crafty.App.Controllers
{
  using AutoMapper;
  using Crafty.Models;
  using Microsoft.AspNet.Identity;
  using Models;
  using Models.BindingModels;
  using Models.ViewModels;
  using Crafty.Data.UnitOfWork;
  using System;
  using System.Collections.Generic;
  using System.Drawing;
  using System.Drawing.Imaging;
  using System.Linq;
  using System.Web;
  using System.Web.Mvc;

  public class ItemsController : BaseController
  {
    private readonly int ItemsPerAjax = 25;
    private static int CurrentPage = 1;
    private readonly int MaxImgWidth = 1024;
    private readonly int MaxImgHeight = 1024;

    public ItemsController(ICraftyData data) : base(data) { }

    // GET: items/section/id
    [HttpGet]
    public ActionResult Section(int id, int? ctg)
    {
      Section sectionCategories = this.Data.Sections.Find(id);
      if (sectionCategories != null)
      {
        IEnumerable<Item> firstItemsFirstCategory;
        if (ctg == null)
        {
          firstItemsFirstCategory = sectionCategories.Categories.First().Items.OrderByDescending(c => c.PostedOn).Take(25);
        }
        else
        {
          firstItemsFirstCategory = sectionCategories.Categories.FirstOrDefault(c => c.Id == ctg).Items.OrderByDescending(c => c.PostedOn).Take(25);
          ViewBag.CategoryName = sectionCategories.Categories.FirstOrDefault(c => c.Id == ctg).Name;
        }
        IEnumerable<ConciseItemViewModel> model = Mapper.Map<IEnumerable<ConciseItemViewModel>>(firstItemsFirstCategory);

        ViewBag.SectionName = sectionCategories.Name;
        //this.PassSectionsToView("concise");
        return View(model);
      }
      return RedirectToAction("", "error", null);
    }

    // GET: items/category/id
    // Filter items by category.Show them in concise view
    // TODO: Implement hover efect, to show author and shop location
    [HttpGet]
    public ActionResult Category(int id, int? skipNum)
    {
      int page = 2;
      int amount = this.ItemsPerAjax;
      string categoryName = this.Data.Categories.Find(id).Name;

      if (skipNum != null)
        page = int.Parse(skipNum.ToString());

      IEnumerable<Item> itemsInCategory = itemsInCategory = this.Data.Categories.Find(id)
                                                              .Items
                                                              .OrderByDescending(i => i.PostedOn)
                                                              .Skip(page * this.ItemsPerAjax - this.ItemsPerAjax)
                                                              .Take(amount);

      IEnumerable<ConciseItemViewModel> model = Mapper.Map<IEnumerable<ConciseItemViewModel>>(itemsInCategory);
      if (this.Request.IsAjaxRequest())
        return PartialView("_AjaxItemsPage", model);

      this.ViewBag.CategoryName = categoryName;
      return View(model);
    }

    // GET: items/add
    // Return the page, where the user can add new item
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public ActionResult Add()
    {
      //if(this.UserProfile.Status == UserStatusType.User)
      //  return this.RedirectToAction("edit", "account", new { sr = "true" });      

      AddItemViewModelBag model = new AddItemViewModelBag()
      {
        Sections = this.GetSections(),
        AddItemBindingModel = new AddItemBindingModel()
      };
      return this.View(model);
    }

    // pOST: items/add
    // Uploading new item logic.
    // Max image size : 1240x1240px. On exceeding - resize.
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public ActionResult Add(AddItemBindingModel model)
    {
      if (ValidateModel(model))
      {
        User user = this.Data.Users.Find(this.User.Identity.GetUserId());
        Category category = this.Data.Categories.Find(model.CategoryId);
        if (category == null)
          return HttpNotFound("Category not found in the system");

        Item newItem = new Item()
        {
          Category = category,
          Description = model.Description,
          PostedOn = DateTime.Now,
          Price = model.Price,
          Title = model.Title,
          Seller = user,
          Quantity = model.Quantity
        };

        IEnumerable<HttpPostedFileBase> pictures = model.Pictures.Where(p => p != null);
        List<string> itemPictures = new List<string>();
        foreach (HttpPostedFileBase picture in pictures)
        {
          string picName = Guid.NewGuid().ToString();
          string imgExt = System.IO.Path.GetExtension(picture.FileName);
          string finalPath = "/Images/items-images/" + picName + imgExt;
          string physicalPath = Server.MapPath(finalPath);
          picture.SaveAs(physicalPath);
          
          this.ResizeUploadedImage(physicalPath, this.MaxImgWidth, this.MaxImgHeight);
          
          itemPictures.Add(finalPath);
        }
        newItem.Pictures = String.Join(";", itemPictures);
        newItem.Thumbnail = itemPictures[0];
        this.Data.Items.Add(newItem);
        this.Data.SaveChanges();
        return this.RedirectToAction("Items", "Account", null);
      }
      AddItemViewModelBag _model = new AddItemViewModelBag()
      {
        Sections = this.GetSections(),
        AddItemBindingModel = model
      };
      return View(_model);
    }

    [HttpGet]
    public ActionResult Details(int id)
    {
      Item item = this.Data.Items.Find(id);
      if(item != null)
      {
        item.Views = item.Views + 1;
        this.Data.SaveChanges();

        ItemDetailsViewModel itemModel = Mapper.Map<ItemDetailsViewModel>(item);

        if (this.User.Identity.IsAuthenticated)
          itemModel.InFavourites = this.UserProfile.Favourites.FirstOrDefault(i => i.Id == id) != null;
        else
          itemModel.InFavourites = false;

        bool userLike = item.Likes.FirstOrDefault(l => l.User.Id == this.User.Identity.GetUserId()) != null;
        this.ViewBag.Liked = userLike;
        this.ViewBag.CategoryId = item.Category.Id;


        ItemDetailsViewModelBag model = new ItemDetailsViewModelBag()
        {
          ItemDetailsViewModel = itemModel,
          SuggestedItems = this.GetSuggestedItems(item.Category.Id, item.Id)
        };

        return View(model);
      }
      return HttpNotFound("Не беше намерена обява с такъв идентификационен номер");
    }

    [HttpGet]
    public ActionResult Like(int id)
    {
      Item item = this.Data.Items.Find(id);

      string userId = this.User.Identity.GetUserId();
      User currentUser = this.Data.Users.Find(userId);
      Like liked = item.Likes.FirstOrDefault(like => like.User.Id == userId);

      if(this.User.Identity.IsAuthenticated)
      {
        if (liked == null)
        {
          item.Likes.Add(new Like()
          {
            User = currentUser,
            Item = item
          });
          this.Data.SaveChanges();
          return Content("liked");
        }
        else
        {
          this.Data.Likes.Remove(item.Likes.FirstOrDefault(l => l.User.Id == userId));
          this.Data.SaveChanges();
          return Content("unliked");
        }
      }
      return PartialView("_LoginDialog", new LoginViewModel());
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public JsonResult Delete(int id)
    {
      string userId = this.User.Identity.GetUserId();
      User user = this.Data.Users.Find(userId);
      Item item = this.Data.Items.Find(id);

      if((this.User.Identity.IsAuthenticated && item.Seller.Id == user.Id) || this.User.IsInRole("Admin") )
      {
        if(item != null)
        {
          this.Data.Items.Remove(item);
          this.Data.SaveChanges();
          return Json(new { Data = "success" });
        }
        return Json(new { Data = "unexisting" });
      }
      return Json(new { Data = "unauthorized" });
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public ActionResult Edit(int id)
    {
      string userId = this.User.Identity.GetUserId();
      User user = this.Data.Users.Find(userId);
      Item item = this.Data.Items.Find(id);

      if(item != null)
      {
        if(this.User.Identity.IsAuthenticated && item.Seller.Id == userId)
        {
          EditItemBindingModel _model = Mapper.Map<EditItemBindingModel>(item);
          this.PassSectionsToView("concise");
          this.ViewBag.CategoryName = item.Category.Name;
          this.ViewBag.SectionId = item.Category.Section.Id;
          return View(_model);
        }
        return JavaScript("alert('Непозволено действие')");
      }
      return JavaScript("alert('Несъществуваща обява')");
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public ActionResult Edit(int id, EditItemBindingModel model)
    {
      string userId = this.User.Identity.GetUserId();
      User user = this.Data.Users.Find(userId);
      Item item = this.Data.Items.Find(id);

      bool validPictures = ValidateEditedPictures(model.NewPictures, model.OldPictures);
      if (item != null)
      {
        if (!validPictures)
        {
          this.ModelState.AddModelError("NewPictures", "Обявата трябва да има поне 1 снимка");
          return View(model);
        }

        item.Category = this.Data.Categories.Find(model.CategoryId);
        item.Description = model.Description;
        item.Price = model.Price;
        item.Title = model.Title;

        List<string> itemPics = model.OldPictures.ToList();
        if(model.NewPictures != null)
        {
          foreach (HttpPostedFileBase picture in model.NewPictures.Where(p => p != null))
          {
            string picName = Guid.NewGuid().ToString();
            string imgExt = System.IO.Path.GetExtension(picture.FileName);
            string finalPath = "/Images/items-images/" + picName + imgExt;
            string physicalPath = Server.MapPath(finalPath);
            picture.SaveAs(physicalPath);

            this.ResizeUploadedImage(physicalPath, this.MaxImgWidth, this.MaxImgHeight);
            itemPics.Add(finalPath);
          }
        }
        item.Pictures = String.Join(";", itemPics);
        item.Thumbnail = itemPics[0];
        this.Data.Items.Update(item);
        this.Data.SaveChanges();
        return this.RedirectToAction("Items", "Account", null);
      }
      this.ModelState.AddModelError("Id", "Не съществува изделие/обява с такъв идентификационен номер");
      return View(model);
    }

    [HttpPost]
    public ActionResult Favourite(int id)
    {
      if(this.User.Identity.IsAuthenticated)
      {
        Item item = this.Data.Items.Find(id);
        if (item != null)
        {
          string userId = this.User.Identity.GetUserId();
          bool inFavs = this.UserProfile.Favourites.FirstOrDefault(i => i.Id == id) != null;
          if (inFavs)
          {
            this.UserProfile.Favourites.Remove(item);
            this.Data.SaveChanges();
            return Content("removed");
          }
          this.UserProfile.Favourites.Add(item);
          this.Data.SaveChanges();
          return Content("added");
        }
        return Content("Не беше намерена обява с такъв идентификационен номер.");
      }
      return PartialView("_LoginDialog", new LoginViewModel());
    }

    [HttpGet]
    public ActionResult Categories(int id)
    {
      IOrderedQueryable<Category> categories = this.Data.Categories
                                                       .All()
                                                       .Where(sub => sub.Section.Id == id)
                                                       .OrderBy(cat => cat.Name);

      var request = this.Request.IsAjaxRequest();
      IEnumerable<ConciseCategoryViewModel> model = Mapper.Map<IEnumerable<ConciseCategoryViewModel>>(categories);
      return this.PartialView("_Categories", model);
    }

    [HttpGet]
    public ActionResult Suggestions(int id)
    {
      IEnumerable<Item> items = this.Data.Items.Find(id)
                                  .Category
                                  .Items
                                  .Where(i => i.Id != id)
                                  .OrderByDescending(i => i.PostedOn)
                                  .Skip(7).Take(7);

      IEnumerable<ConciseItemViewModel> model = Mapper.Map<IEnumerable<ConciseItemViewModel>>(items);
      return PartialView("_Suggestions", model);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public ActionResult HideCateg(int id)
    {
      Category category = this.Data.Categories.Find(id);
      try
      {
        category.Visible = false;
        this.Data.SaveChanges();
        return Content("success");
      }
      catch(Exception ex)
      {
        return Content("error");
      }
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public ActionResult ShowCateg(int id)
    {
      Category category = this.Data.Categories.Find(id);
      try
      {
        category.Visible = true;
        this.Data.SaveChanges();
        return Content("success");
      }
      catch (Exception ex)
      {
        return Content("error");
      }
    }

    private IEnumerable<ConciseItemViewModel> GetSuggestedItems(int categoryId, int itemId)
    {

      IEnumerable<Item> suggestedItems = this.Data.Categories.Find(categoryId)
                                                 .Items
                                                 .Where(i => i.Id != itemId)
                                                 .OrderByDescending(i => i.Views)
                                                 .ThenByDescending(i => i.PostedOn)
                                                 .Take(10);


      ICollection<ConciseItemViewModel> model = Mapper.Map<ICollection<ConciseItemViewModel>>(suggestedItems);

      if(model.Count() < 5)
      {
        IQueryable<Item> moreItems = this.Data.Items.All().OrderByDescending(i => i.Views).ThenByDescending(i => i.PostedOn).Take(5 - model.Count());
        foreach(Item item in moreItems)
        {
          model.Add(Mapper.Map<ConciseItemViewModel>(item));
        }
      }
      return model;
    }

    private bool ValidPicturesCount(ICollection<HttpPostedFileBase> files)
    {
      return files.Count(m => m != null) > 0;
    }

    private bool ValidateEditedPictures(ICollection<HttpPostedFileBase> newPics, List<string> oldPics)
    {
      int newPicsCount;
      if (newPics != null)
        newPicsCount = newPics.Count(m => m != null);
      else
        newPicsCount = 0;

      int oldPicsCount;
      if (oldPics != null)
        oldPicsCount = oldPics.Count(p => p != "");
      else
        oldPicsCount = 0;

      int finalCount = newPicsCount + oldPicsCount;
      if(finalCount > 0)
        return true;
  
      return false;
    }

    private bool ValidateCategory(int id)
    {
      Category category = this.Data.Categories.Find(id);
      if (category != null)
        return true;
      return false;
    }

    private bool ValidateModel(AddItemBindingModel model)
    {
      bool validPics = ValidPicturesCount(model.Pictures);
      bool validCategory = this.ValidateCategory(model.CategoryId);
      bool isValid = true;

      if (!validCategory)
      {
        ModelState.AddModelError("CategoryId", "Моля посочете категория");
        isValid = false;
      }

      if (!validPics)
      {
        ModelState.AddModelError("Pictures", "Моля качете поне една снимка на изделието.");
        isValid = false;
      }

      if (!ModelState.IsValid)
      {
        isValid = false;
      }

      return isValid;
    }

    [OutputCache(Duration = 9999999)]
    private IEnumerable<ConciseSectionViewModel> GetSections()
    {
      IOrderedQueryable<Section> dbSections = this.Data.Sections.All()
                        .OrderBy(sec => sec.Name);

      return Mapper.Map<IEnumerable<ConciseSectionViewModel>>(dbSections);
    }
    
    private void PassSectionsToView(string type)
    {
      var sections = this.Data.Sections.All().OrderBy(sec => sec.Name);
      this.ViewBag.NavigationMenuSections = Mapper.Map<IEnumerable<MenuSectionViewModel>>(sections);
    }

    private void ResizeUploadedImage(string lcFilename, int lnWidth, int lnHeight)
    {
      Bitmap bmpOut = null;

      try
      {
        Bitmap loBMP = new Bitmap(lcFilename);
        ImageFormat loFormat = loBMP.RawFormat;

        decimal lnRatio;
        int lnNewWidth = 0;
        int lnNewHeight = 0;

        if (loBMP.Width < lnWidth && loBMP.Height < lnHeight)
          return;

        if (loBMP.Width > loBMP.Height)
        {
          lnRatio = (decimal)lnWidth / loBMP.Width;
          lnNewWidth = lnWidth;
          decimal lnTemp = loBMP.Height * lnRatio;
          lnNewHeight = (int)lnTemp;
        }
        else
        {
          lnRatio = (decimal)lnHeight / loBMP.Height;
          lnNewHeight = lnHeight;
          decimal lnTemp = loBMP.Width * lnRatio;
          lnNewWidth = (int)lnTemp;
        }


        bmpOut = new Bitmap(lnNewWidth, lnNewHeight);
        Graphics g = Graphics.FromImage(bmpOut);
        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
        g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
        g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
        g.FillRectangle(Brushes.White, 0, 0, lnNewWidth, lnNewHeight);
        g.DrawImage(loBMP, 0, 0, lnNewWidth, lnNewHeight);

        Bitmap wmark = new Bitmap(Server.MapPath("~/Images/crafty-watermark.png"));
        float wmarkX = (float)lnNewWidth / 2 - 150;
        float wmarkY = (float)lnNewHeight / 2 - 50; // Fix this to center the watermark
        g.DrawImage(wmark, 0, 0);
        loBMP.Dispose();
      }
      catch
      {

      }
      System.IO.File.Delete(lcFilename);
      bmpOut.Save(lcFilename);
    }

    public ContentResult Availability(int id, string amount)
    {
      Item item = this.Data.Items.Find(id);
      if (item == null)
        return Content("not-found");

      if (item.Quantity < int.Parse(amount))
        return Content(item.Quantity.ToString());

      return Content("available");
    }
  }
}