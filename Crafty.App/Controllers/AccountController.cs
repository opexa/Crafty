namespace Crafty.App.Controllers
{
  using System.Linq;
  using System.Threading.Tasks;
  using System.Web;
  using System.Web.Mvc;
  using Microsoft.AspNet.Identity;
  using Microsoft.AspNet.Identity.Owin;
  using Microsoft.Owin.Security;
  using Crafty.Models;
  using System;
  using System.Collections.Generic;
  using Models.ViewModels;
  using Models.BindingModels;
  using AutoMapper;
  using Data.UnitOfWork;
  using Facebook;
  using System.Security.Claims;
  using System.Net;
  using System.IO;
  using Data;

  [Authorize]
  public class AccountController : BaseController
  {
    private ApplicationSignInManager _signInManager;
    private UserManager _userManager;
    private const string defaultBanner = "/Images/default-banner.jpg";
    private const string defaultAvatar = "/Images/default-avatar.jpg";

    public AccountController(ICraftyData data) : base(data)
    {
    }

    public AccountController(UserManager userManager, ApplicationSignInManager signInManager, ICraftyData data) : this(data)
    {
      UserManager = userManager;
      SignInManager = signInManager;
    }

    public ApplicationSignInManager SignInManager
    {
      get
      {
        return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
      }
      private set
      {
        _signInManager = value;
      }
    }

    public UserManager UserManager
    {
      get
      {
        return _userManager ?? HttpContext.GetOwinContext().GetUserManager<UserManager>();
      }
      private set
      {
        _userManager = value;
      }
    }

    // User details home page
    public ActionResult Index()
    {
      string userId = this.User.Identity.GetUserId();
      User userProfile = this.Data.Users.Find(userId);
      ProfileDetailsViewModel model = Mapper.Map<ProfileDetailsViewModel>(userProfile);
      this.ViewBag.aoCount = this.UserProfile.AwaitingOrders.Count;

      return View(model);
    }

    [HttpGet]
    [Authorize]
    public ActionResult Items(int? skip)
    {
      string userId = this.User.Identity.GetUserId();
      User user = this.Data.Users.Find(userId);

      int skipNumber = 1;
      if (skip != null)
        skipNumber = int.Parse(skip.ToString());

      IEnumerable<Item> items = user.ItemsForSale
                                 .OrderByDescending(i => i.PostedOn)
                                 .Skip(skipNumber * 25 - 25)
                                 .Take(25);

      IEnumerable<MyItemViewModel> model = Mapper.Map<IEnumerable<MyItemViewModel>>(items);
      if (this.Request.IsAjaxRequest())
        return PartialView("_AccountItems", model);

      this.ViewBag.aoCount = this.UserProfile.AwaitingOrders.Count;
      return View(model);
    }


    [HttpGet]
    [Authorize]
    public ActionResult Edit(string sr)
    {
      if (sr != null && sr == "true" && this.UserProfile.Status == UserStatusType.User)
        this.ViewBag.ShowAddingAdRequirements = true;

      EditAccountBindingModel model = Mapper.Map<EditAccountBindingModel>(this.UserProfile);
      this.ViewBag.Username = this.UserProfile.UserName;
      this.PassUserStatusTypesToView();
      this.ViewBag.aoCount = this.UserProfile.AwaitingOrders.Count;

      return this.View(model);
    }

    [HttpPost]
    [Authorize]
    public ActionResult Edit(EditAccountBindingModel model)
    {
      if (!this.ModelState.IsValid)
        return this.View(model);
      
      User user = this.UserProfile;

      if (Request.Files["profile_img"].FileName.Length > 0)
      {
        HttpPostedFileBase prImgFile = Request.Files["profile_img"];
        string imgName = Guid.NewGuid().ToString();
        string imgExt = System.IO.Path.GetExtension(prImgFile.FileName);
        string finalPath = imgName + imgExt;

        string physicalPath = Server.MapPath("~/Images/_profile-images/" + finalPath);
        prImgFile.SaveAs(physicalPath);
        user.ProfileImg = "/Images/_profile-images/" + finalPath;
      }
      
      user.Description = model.Description;
      user.FullName = model.FullName;
      user.PhoneNumber = model.PhoneNumber;
      user.City = model.City;
      user.ShippingAddress = model.ShippingAddress;
      user.Website = model.Website;
      user.Email = model.Email;

      if (Request.Files["banner_img"].FileName.Length > 0)
      {
        HttpPostedFileBase banImgFile = Request.Files["banner_img"];
        string imgName = Guid.NewGuid().ToString();
        string imgExt = System.IO.Path.GetExtension(banImgFile.FileName);
        string finalPath = imgName + imgExt;

        string physicalPath = Server.MapPath("~/Images/_banner-images/" + finalPath);
        banImgFile.SaveAs(physicalPath);
        user.ProfileBanner = "/Images/_banner-images/" + finalPath;
      }

      this.Data.Users.Update(user);
      this.Data.SaveChanges();

      this.Response.Cookies["userStatusCookie"].Value = user.Status.ToString();
      return this.RedirectToAction("Index");
    }

    [HttpGet]
    [Authorize]
    public ActionResult Favourites()
    {
      IEnumerable<Item> favourites = this.UserProfile.Favourites;
      IEnumerable<ConciseItemViewModel> model = Mapper.Map<IEnumerable<ConciseItemViewModel>>(favourites);
      
      if(this.User.IsInRole("Admin"))
        this.ViewBag.aoCount = this.UserProfile.AwaitingOrders.Count;

      return View(model);
    }

    [HttpGet]
    [Authorize]
    public ActionResult Likes()
    {
      IEnumerable<Item> likes = this.UserProfile.Likes.Select(l => l.Item);
      IEnumerable<ConciseItemViewModel> model = Mapper.Map<IEnumerable<ConciseItemViewModel>>(likes);
      this.ViewBag.aoCount = this.UserProfile.AwaitingOrders.Count;

      return View(model);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public ActionResult Users(string username)
    {
      User user = this.Data.Users.All().FirstOrDefault(u => u.UserName == username);
      UserProfileDetailsViewModel model = Mapper.Map<UserProfileDetailsViewModel>(user);

      return this.View(model);
    }

    // GET: /Account/Login
    [AllowAnonymous]
    public ActionResult Login(string returnUrl)
    {
      ViewBag.ReturnUrl = returnUrl;
      return View();
    }

    // POST: /Account/Login
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
    {
      if (!ModelState.IsValid)
      {
        return View(model);
      }


      // This doesn't count login failures towards account lockout
      // To enable password failures to trigger account lockout, change to shouldLockout: true
      var result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, shouldLockout: false);
      switch (result)
      {
        case SignInStatus.Success:
          // setting cookie for the user status.
          // Used in: _LoginPartial to know which tab to display.
          HttpCookie profilePictureCookie = new HttpCookie("profilePicture");
          User loggedUser = this.Data.Users.All().FirstOrDefault(u => u.UserName == model.UserName);
          profilePictureCookie.Value = loggedUser.ProfileImg.ToString();
          profilePictureCookie.Expires = DateTime.Now.AddDays(365);
          Response.Cookies.Add(profilePictureCookie);

          if (!this.Request.IsAjaxRequest())
            return RedirectToLocal(returnUrl);

          return JavaScript("location.reload(true)");
        case SignInStatus.LockedOut:
          return View("Lockout");
        case SignInStatus.RequiresVerification:
          return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
        case SignInStatus.Failure:
        default:
          ModelState.AddModelError("", "Неуспешен опит за вход в системата.");
          return View(model);
      }
    }

    // GET: /Account/VerifyCode
    [AllowAnonymous]
    public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
    {
      // Require that the user has already logged in via username/password or external login
      if (!await SignInManager.HasBeenVerifiedAsync())
      {
        return View("Error");
      }
      return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
    }

    // POST: /Account/VerifyCode
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
    {
      if (!ModelState.IsValid)
      {
        return View(model);
      }

      // The following code protects for brute force attacks against the two factor codes. 
      // If a user enters incorrect codes for a specified amount of time then the user account 
      // will be locked out for a specified amount of time. 
      // You can configure the account lockout settings in IdentityConfig
      var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
      switch (result)
      {
        case SignInStatus.Success:
          return RedirectToLocal(model.ReturnUrl);
        case SignInStatus.LockedOut:
          return View("Lockout");
        case SignInStatus.Failure:
        default:
          ModelState.AddModelError("", "Invalid code.");
          return View(model);
      }
    }

    // GET: /Account/Register
    [AllowAnonymous]
    public ActionResult Register(string sender)
    {
      if (this.User.Identity.IsAuthenticated)
      {
        if(sender == "acc_edit")
        {
          return this.View();
        }
        return this.RedirectToAction("Index", "Home");
      }

      return View();
    }

    //User register.Required fields are username, email, city and status which by default is seller.
    //On successfull login, setting cookie about the user status.
    // POST: /Account/Register
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Register(RegisterViewModel model)
    {
      if (ModelState.IsValid)
      {
        if (this.Data.Users.All().FirstOrDefault(u => u.UserName == model.UserName) != null)
        {
          this.ModelState.AddModelError("Username", "Това потребителско име вече е заето, моля изберете друго.");
          return this.View(model);
        }
        if (this.User.Identity.IsAuthenticated)
          this.LogOff("async");

        User user = new User
        {
          UserName = model.UserName,
          Email = model.Email,
          FullName = model.FullName,
          Status = UserStatusType.User,
          ProfileImg = "/Images/default-avatar.jpg",
          ProfileBanner = "/Images/default-banner.jpg"
        };

        var result = await UserManager.CreateAsync(user, model.Password);
        if (result.Succeeded)
        {
          await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

          //HttpCookie userStatusCookie = new HttpCookie("userStatusCookie");
          //userStatusCookie.Value = user.Status.ToString();
          //userStatusCookie.Expires = DateTime.Now.AddDays(365);
          //Response.Cookies.Add(userStatusCookie);

          return RedirectToAction("Index", "Home");
        }
        AddErrors(result);
      }
      return View(model);
    }

    // GET: /Account/ForgotPassword
    [AllowAnonymous]
    public ActionResult ForgotPassword()
    {
      return View();
    }

    // POST: /Account/ForgotPassword
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
      if (ModelState.IsValid)
      {
        var user = await UserManager.FindByNameAsync(model.Email);
        if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
        {
          // Don't reveal that the user does not exist or is not confirmed
          return View("ForgotPasswordConfirmation");
        }

        // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
        // Send an email with this link
        // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
        // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
        // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
        // return RedirectToAction("ForgotPasswordConfirmation", "Account");
      }

      // If we got this far, something failed, redisplay form
      return View(model);
    }

    // GET: /Account/ForgotPasswordConfirmation
    [AllowAnonymous]
    public ActionResult ForgotPasswordConfirmation()
    {
      return View();
    }

    // GET: /Account/ResetPassword
    [AllowAnonymous]
    public ActionResult ResetPassword(string code)
    {
      return code == null ? View("Error") : View();
    }

    // POST: /Account/ResetPassword
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
    {
      if (!ModelState.IsValid)
      {
        return View(model);
      }
      var user = await UserManager.FindByNameAsync(model.Email);
      if (user == null)
      {
        // Don't reveal that the user does not exist
        return RedirectToAction("ResetPasswordConfirmation", "Account");
      }
      var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
      if (result.Succeeded)
      {
        return RedirectToAction("ResetPasswordConfirmation", "Account");
      }
      AddErrors(result);
      return View();
    }

    // GET: /Account/ResetPasswordConfirmation
    [AllowAnonymous]
    public ActionResult ResetPasswordConfirmation()
    {
      return View();
    }

    // POST: /Account/ExternalLogin
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public ActionResult ExternalLogin(string provider, string returnUrl)
    {
      // Request a redirect to the external login provider
      return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
    }

    //
    // GET: /Account/SendCode
    [AllowAnonymous]
    public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
    {
      var userId = await SignInManager.GetVerifiedUserIdAsync();
      if (userId == null)
      {
        return View("Error");
      }
      var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
      var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
      return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
    }

    // POST: /Account/SendCode
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> SendCode(SendCodeViewModel model)
    {
      if (!ModelState.IsValid)
      {
        return View();
      }

      // Generate the token and send it
      if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
      {
        return View("Error");
      }
      return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
    }

    // GET: /Account/ExternalLoginCallback
    [AllowAnonymous]
    public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
    {
      var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
      if (loginInfo == null)
      {
        return RedirectToAction("Login");
      }

      // Sign in the user with this external login provider if the user already has a login
      var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
      switch (result)
      {
        case SignInStatus.Success:
          CraftyDbContext context = new CraftyDbContext();

          ClaimsIdentity externalIdentity = AuthenticationManager.GetExternalIdentity(DefaultAuthenticationTypes.ExternalCookie);
          string identifier = externalIdentity.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value;
          var userId = context.UserLogins.FirstOrDefault(u => u.ProviderKey == identifier).UserId;
          User user = this.Data.Users.Find(userId);

          HttpCookie profilePictureCookie = new HttpCookie("profilePicture");
          profilePictureCookie.Value = user.ProfileImg.ToString();
          profilePictureCookie.Expires = DateTime.Now.AddDays(365);
          Response.Cookies.Add(profilePictureCookie);

          return RedirectToLocal(returnUrl);
        case SignInStatus.LockedOut:
          return View("Lockout");
        case SignInStatus.RequiresVerification:
          return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
        case SignInStatus.Failure:
        default:
          // If the user does not have an account, then prompt the user to create an account
          ViewBag.ReturnUrl = returnUrl;
          ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
          return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel());
      }
    }

    // POST: /Account/ExternalLoginConfirmation
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
    {
      if (User.Identity.IsAuthenticated)
      {
        return RedirectToAction("Index", "Manage");
      }
      
      // Get the information about the user from the external login provider
      var info = await AuthenticationManager.GetExternalLoginInfoAsync();
      if (info == null)
      {
        return View("ExternalLoginFailure");
      }


      ClaimsIdentity identity = AuthenticationManager.GetExternalIdentity(DefaultAuthenticationTypes.ExternalCookie);
      var provider = identity.Claims.First().OriginalIssuer;

      User user = null; 

      switch(provider)
      {
        case "Facebook":
          user = this.BindFacebookProfile(model.Username);
          break;
        case "Google":
          user = this.BindGoogleProfile(model.Username);
          break;
        default: break;
      }

      var result = await UserManager.CreateAsync(user);
      if (result.Succeeded)
      {
        result = await UserManager.AddLoginAsync(user.Id, info.Login);
        if (result.Succeeded)
        {
          await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
          return RedirectToLocal(returnUrl);
        }
      }
      AddErrors(result);

      ViewBag.ReturnUrl = returnUrl;
      return View(model);
    }

    [HttpGet]
    public ActionResult Purchases()
    {
      IEnumerable<Order> myPurchases = this.UserProfile.PurchaseHistory.OrderByDescending(o => o.PostedOn).Take(25);
      IEnumerable<ConcisePurchaseViewModel> model = Mapper.Map<IEnumerable<ConcisePurchaseViewModel>>(myPurchases);
      this.ViewBag.aoCount = this.UserProfile.AwaitingOrders.Count;

      return View(model);
    }

    private User BindFacebookProfile(string username)
    {
      ClaimsIdentity identity = AuthenticationManager.GetExternalIdentity(DefaultAuthenticationTypes.ExternalCookie);
      string access_token = identity.FindFirstValue("FacebookAccessToken");
      FacebookClient fb = new FacebookClient(access_token);
      dynamic myInfo = fb.Get("/me?fields=email,first_name,last_name,picture.width(200).height(200),cover.width(2500).height(400),website,hometown,link");
      string fb_name = myInfo["first_name"] + " " + myInfo["last_name"];
      string fb_pic = myInfo["picture"]["data"]["url"] != null ? myInfo["picture"]["data"]["url"] : "/Images/default-avatar.jpg";
      string fb_cover = myInfo["cover"]["source"] != null ? myInfo["cover"]["source"] : "/Images/default-banner.jpg";
      string fb_email = myInfo["email"];

      string fb_website = null;
      if (((IDictionary<string, object>)myInfo).ContainsKey("website"))
        fb_website = myInfo["website"];

      string fb_town = null;
      if (((IDictionary<string, object>)myInfo).ContainsKey("hometown"))
        fb_town = myInfo["hometown"]["name"];

      User user = new User
      {
        UserName = username,
        Email = fb_email,
        FullName = fb_name,
        ProfileBanner = fb_cover,
        ProfileImg = fb_pic,
        Website = fb_website,
        City = fb_town,
        Status = UserStatusType.User
      };
      return user;
    }

    private User BindGoogleProfile(string username)
    {
      ClaimsIdentity identity = AuthenticationManager.GetExternalIdentity(DefaultAuthenticationTypes.ExternalCookie);
      string g_mail = identity.Claims.FirstOrDefault(c => c.Type == "email").Value;
      string g_firstName = identity.Claims.FirstOrDefault(c => c.Type == "firstname").Value;
      string g_lastName = identity.Claims.FirstOrDefault(c => c.Type == "lastname").Value;
      string g_picture = identity.Claims.FirstOrDefault(c => c.Type == "picture").Value;

      User user = new User()
      {
        UserName = username,
        FullName = g_firstName + " " + g_lastName,
        ProfileImg = g_picture,
        Email = g_mail,
        Status = UserStatusType.User,
        ProfileBanner = "/Images/default-banner.jpg"
      };
      return user;
    }

    // POST: /Account/LogOff
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult LogOff(string type)
    {
      AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
      Request.Cookies.Remove("profilePicture");
      this.HideRegInfo();

      if (type == "async")
        return Content("success");

      return RedirectToAction("Index", "Home");
    }

    // GET: /Account/ExternalLoginFailure
    [AllowAnonymous]
    public ActionResult ExternalLoginFailure()
    {
      return View();
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        if (_userManager != null)
        {
          _userManager.Dispose();
          _userManager = null;
        }

        if (_signInManager != null)
        {
          _signInManager.Dispose();
          _signInManager = null;
        }
      }

      base.Dispose(disposing);
    }

    // Passing user status types to the view.
    // Used in register and edit profile views.
    private void PassUserStatusTypesToView()
    {
      ViewBag.ProfileStatusTypes = new List<SelectListItem>()
      {
        new SelectListItem() { Text = "Продавач", Value = "0" },
        new SelectListItem() { Text = "Потребител", Value = "1" }
      };
    }

    [HttpGet]
    [AllowAnonymous]
    public ActionResult Checkname(string username)
    {
      User user = this.Data.Users.All().FirstOrDefault(u => u.UserName == username);
      if (user == null)
        return Content("free");
      return Content("taken");
    }

    [HttpGet]
    [AllowAnonymous]
    public void HideRegInfo()
    {
      this.Session["ShowRegInfo"] = "false";
    }
    #region Helpers
    // Used for XSRF protection when adding external logins
    private const string XsrfKey = "XsrfId";

    private IAuthenticationManager AuthenticationManager
    {
      get
      {
        return HttpContext.GetOwinContext().Authentication;
      }
    }

    private void AddErrors(IdentityResult result)
    {
      foreach (var error in result.Errors)
      {
        ModelState.AddModelError("", error);
      }
    }

    private ActionResult RedirectToLocal(string returnUrl)
    {
      if (Url.IsLocalUrl(returnUrl))
      {
        return Redirect(returnUrl);
      }
      return RedirectToAction("Index", "Home");
    }

    internal class ChallengeResult : HttpUnauthorizedResult
    {
      public ChallengeResult(string provider, string redirectUri)
          : this(provider, redirectUri, null)
      {
      }

      public ChallengeResult(string provider, string redirectUri, string userId)
      {
        LoginProvider = provider;
        RedirectUri = redirectUri;
        UserId = userId;
      }

      public string LoginProvider { get; set; }
      public string RedirectUri { get; set; }
      public string UserId { get; set; }

      public override void ExecuteResult(ControllerContext context)
      {
        var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
        if (UserId != null)
        {
          properties.Dictionary[XsrfKey] = UserId;
        }
        context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
      }
    }
    #endregion
  }
}