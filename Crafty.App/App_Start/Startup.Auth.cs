namespace Crafty.App
{
  using System;
  using Microsoft.AspNet.Identity;
  using Microsoft.AspNet.Identity.Owin;
  using Microsoft.Owin;
  using Microsoft.Owin.Security.Cookies;
  using Owin;
  using Crafty.Models;
  using Crafty.Data;
  using Microsoft.Owin.Security.Facebook;
  using System.Threading.Tasks;
  using Microsoft.Owin.Security.Google;
  using System.Security.Claims;

  public partial class Startup
  {
    // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
    public void ConfigureAuth(IAppBuilder app)
    {
      // Configure the db context, user manager and signin manager to use a single instance per request
      app.CreatePerOwinContext(CraftyDbContext.Create);
      app.CreatePerOwinContext<UserManager>(UserManager.Create);
      app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

      // Enable the application to use a cookie to store information for the signed in user
      // and to use a cookie to temporarily store information about a user logging in with a third party login provider
      // Configure the sign in cookie
      app.UseCookieAuthentication(new CookieAuthenticationOptions
      {
        AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
        LoginPath = new PathString("/Account/Login"),
        Provider = new CookieAuthenticationProvider
        {
          // Enables the application to validate the security stamp when the user logs in.
          // This is a security feature which is used when you change a password or add an external login to your account.  
          OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<UserManager, User>(
                  validateInterval: TimeSpan.FromMinutes(30),
                  regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
        }
      });
      app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

      // Enables the application to temporarily store user information when they are verifying the second factor in the two-factor authentication process.
      app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));

      // Enables the application to remember the second login verification factor such as phone or email.
      // Once you check this option, your second step of verification during the login process will be remembered on the device where you logged in from.
      // This is similar to the RememberMe option when you log in.
      app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);

      var fbX = new FacebookAuthenticationOptions();
      fbX.AppId = "195375857608618";
      fbX.AppSecret = "3121616ed6e2d415f3da2b3747ba5421";
      fbX.Scope.Add("email");
      fbX.Scope.Add("public_profile");
      fbX.Provider = new FacebookAuthenticationProvider()
      {
        OnAuthenticated = context =>
        {
          context.Identity.AddClaim(new System.Security.Claims.Claim("FacebookAccessToken", context.AccessToken));
          return Task.FromResult(true);
        }
      };
      fbX.SignInAsAuthenticationType = DefaultAuthenticationTypes.ExternalCookie;
      app.UseFacebookAuthentication(fbX);

      var gx = new GoogleOAuth2AuthenticationOptions()
      {
        ClientId = "720341385771-5iadds9fi4iaob3bp3sl1aqjq2c4j547.apps.googleusercontent.com",
        ClientSecret = "E0wPtYU3zOtk8gJUV7yyNtlE",
        Provider = new GoogleOAuth2AuthenticationProvider()
        {
          OnAuthenticated = context =>
          {
            var userDetail = context.User;
            var picture = userDetail["image"]["url"].ToString();
            var email = userDetail["emails"][0]["value"].ToString();
            var firstName = userDetail["name"]["givenName"].ToString();
            var lastName = userDetail["name"]["familyName"].ToString();

            context.Identity.AddClaim(new Claim("picture", picture));
            context.Identity.AddClaim(new Claim("email", email));
            context.Identity.AddClaim(new Claim("firstname", firstName));
            context.Identity.AddClaim(new Claim("lastname", lastName));
            
            return Task.FromResult(0);
          }
        }
      };
      gx.Scope.Add("https://www.googleapis.com/auth/plus.login");
      gx.Scope.Add("https://www.googleapis.com/auth/userinfo.email");
      app.UseGoogleAuthentication(gx);

      //app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
      //{
      //  ClientId = "720341385771-5iadds9fi4iaob3bp3sl1aqjq2c4j547.apps.googleusercontent.com",
      //  ClientSecret = "E0wPtYU3zOtk8gJUV7yyNtlE"
      //});
    }
  }
}