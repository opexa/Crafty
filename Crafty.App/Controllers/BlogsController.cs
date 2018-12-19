namespace Crafty.App.Controllers
{
  using AutoMapper;
  using Crafty.Models;
  using Data.UnitOfWork;
  using Microsoft.AspNet.Identity;
  using Models.BindingModels;
  using Models.ViewModels;
  using System;
  using System.Collections.Generic;
  using System.Drawing;
  using System.Drawing.Imaging;
  using System.Linq;
  using System.Web;
  using System.Web.Mvc;

  [Authorize(Roles = "Admin")]
  public class BlogsController : BaseController
  {
    public BlogsController(ICraftyData data) : base(data) { }
    
    [AllowAnonymous]
    [HttpGet]
    public ActionResult Index()
    {
      return View();
    }

    [HttpGet]
    [AllowAnonymous]
    public ActionResult Details(int id)
    {
      Blog blog = this.Data.Blogs.Find(id);
      if (blog != null)
      {
        if (blog.Visible == false && !this.User.IsInRole("Admin"))
          return RedirectToAction("", "home", null);

        BlogDetailsViewModelBag model = new BlogDetailsViewModelBag()
        {
          BlogModel = Mapper.Map<BlogDetailsViewModel>(blog),
          OtherBlogs = this.GetPopluarBlogs(id),
          MoreCommentsButton = blog.Comments.Count > 15 ? true : false
        };

        try
        {
          blog.Views += 1;
          this.Data.SaveChanges();
          return View(model);
        }
        catch (Exception ex)
        {
          return View(model);
        }
      }
      return HttpNotFound();
    }
    
    [HttpGet]
    public ActionResult Create()
    {
      return View(new AddBlogBindingModel());
    }

    [HttpPost]
    [ValidateInput(false)]
    public ActionResult Create(AddBlogBindingModel model)
    {
      if (this.ModelState.IsValid)
      {
        Blog blog = new Blog()
        {
          Title = model.Title,
          Content = model.Content,
          PostedOn = DateTime.Now,
          Author = this.UserProfile,
          Visible = true,
          AuthorId = this.User.Identity.GetUserId(),
          BlogContentIdentifier = model.BlogContentIdentifier
        };

        if(model.Images != null && model.Images.Any())
        {
          List<string> blogPictures = new List<string>();
          int picId = 1;
          foreach (HttpPostedFileBase picture in model.Images)
          {
            string imgExt = System.IO.Path.GetExtension(picture.FileName);
            string finalPath = "/Images/blogs/" + model.BlogContentIdentifier + "-" + picId + imgExt;
            string physicalPath = Server.MapPath(finalPath);
            picture.SaveAs(physicalPath);

            this.ResizeContentImage(physicalPath, 1200, 450);

            blogPictures.Add(finalPath);
            picId++;
          }
          blog.Thumbnail = blogPictures.First();
          blog.LastImageId = picId;
        }

        if(model.RelatedItems != null && model.RelatedItems.Length > 0)
        {
          foreach (string id in model.RelatedItems.Split(new[] { ',' }))
          {
            Item item = this.Data.Items.Find(int.Parse(id));
            if (item != null)
              blog.RelatedItems.Add(item);
          }
        }

        try
        {
          this.Data.Blogs.Add(blog);
          this.Data.SaveChanges();
          return RedirectToAction("", "home", null);
        }
        catch(Exception ex)
        {
          this.ModelState.AddModelError("", "Възникна грешка при записването на статията в базата.");
          return View();
        }
      }
      return View();
    }

    [HttpGet]
    public ActionResult Edit(int id)
    {
      Blog blog = this.Data.Blogs.Find(id);
      if (blog == null)
        return RedirectToAction("", "home", null);

      EditBlogBindingModel model = Mapper.Map<EditBlogBindingModel>(blog);

      return View(model);
    }

    [HttpPost]
    [ValidateInput(false)]
    public ActionResult Edit(EditBlogBindingModel model)
    {
      if (!this.ModelState.IsValid)
        return View(model);

      Blog blog = this.Data.Blogs.Find(int.Parse(model.Id));

      if (blog == null)
        return RedirectToAction("", "home", null);

      try
      {
        blog.Title = model.Title;
        blog.Content = model.Content;

        if (model.NewImages != null && model.NewImages.Any())
        {
          int c = 0;
          foreach (HttpPostedFileBase picture in model.NewImages)
          {
            blog.LastImageId++;
            string imgExt = System.IO.Path.GetExtension(picture.FileName);
            string finalPath = "/Images/blogs/" + model.BlogContentIdentifier + "-" + blog.LastImageId + imgExt;
            string physicalPath = Server.MapPath(finalPath);
            picture.SaveAs(physicalPath);

            this.ResizeContentImage(physicalPath, 1200, 450);

            if (c == 0 && blog.Thumbnail == null)
              blog.Thumbnail = physicalPath;

            c++;
          }
        }

        if (model.UpdatedRelatedItems != null && model.UpdatedRelatedItems.Length > 0)
        {
          blog.RelatedItems.Clear();
          foreach (string id in model.UpdatedRelatedItems.Split(new[] { ',' }))
          {
            Item item = this.Data.Items.Find(int.Parse(id));

            if (item != null)
              blog.RelatedItems.Add(item);
          }
        }
        this.Data.SaveChanges();
        return RedirectToAction("details", new { Id = int.Parse(model.Id) });
      }
      catch(Exception ex)
      {
        return RedirectToAction("edit", new { Id = int.Parse(model.Id) });
      }

      return Content("error");
    }

    [HttpGet]
    public ActionResult Archive(int id)
    {
      Blog blog = this.Data.Blogs.Find(id);
      if (blog == null)
        return Content("not found");
      
      try
      {
        blog.Visible = false;
        this.Data.SaveChanges();
        return Content("success");
      }
      catch(Exception ex)
      {
        return Content("error");
      }
    }

    [HttpGet]
    public ActionResult Show(int id)
    {
      Blog blog = this.Data.Blogs.Find(id);
      if (blog == null)
        return Content("not found");

      try
      {
        blog.Visible = true;
        this.Data.SaveChanges();
        return Content("success");
      }
      catch (Exception ex)
      {
        return Content("error");
      }
    }

    [HttpPost]
    public ActionResult Delete(int id)
    {
      Blog blog = this.Data.Blogs.Find(id);
      if (blog == null)
        return Content("not found");

      try
      {
        this.Data.Blogs.Remove(blog);
        this.Data.SaveChanges();
        return Content("success");
      }
      catch(Exception ex)
      {
        return Content("error");
      }
    }

    private IEnumerable<ConciseBlogViewModel> GetPopluarBlogs(int blogId)
    {
      IQueryable<Blog> dbBlogs = this.Data.Blogs.All().Where(b => b.Id != blogId && b.Visible == true).OrderByDescending(b => b.Views).ThenBy(b => b.PostedOn).Take(4);

      return Mapper.Map<IEnumerable<ConciseBlogViewModel>>(dbBlogs);
    }

    private void ResizeContentImage(string lcFilename, int lnWidth, int lnHeight)
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
  }
}