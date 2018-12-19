namespace Crafty.Data.Migrations
{
  using Microsoft.AspNet.Identity;
  using Microsoft.AspNet.Identity.EntityFramework;
  using Models;
  using System;
  using System.Collections.Generic;
  using System.Data.Entity.Migrations;
  using System.Linq;

  public sealed class Configuration : DbMigrationsConfiguration<Crafty.Data.CraftyDbContext>
  {
    public Configuration()
    {
      AutomaticMigrationsEnabled = true;
      AutomaticMigrationDataLossAllowed = true;
    }

    protected override void Seed(Crafty.Data.CraftyDbContext context)
    {
      //return;
      SeedRolesAndUsers(context);

      if (!context.Categories.Any())
        this.InsertCategoriesToDatabase(context);

      if (!context.Items.Any())
        this.InsertItemsToDatabase(context);
    }

    private static void SeedRolesAndUsers(CraftyDbContext context)
    {
      if (!context.Roles.Any(r => r.Name == "Admin"))
      {
        var store = new RoleStore<IdentityRole>(context);
        var manager = new RoleManager<IdentityRole>(store);
        var role = new IdentityRole("Admin");


        var sellerRole = new IdentityRole("Seller");
        manager.Create(sellerRole);
        manager.Create(role);
      }


      if (!context.Users.Any(u => u.UserName == "admin"))
      {
        var store = new UserStore<User>(context);
        var manager = new UserManager<User>(store);
        var user = new User()
        {
          UserName = "admin",
          FullName = "admin",
          Email = "admin@crafty.com",
          City = "crafty",
          Status = UserStatusType.User
        };

        manager.Create(user, "adminPass123");
        manager.AddToRole(user.Id, "Admin");

        user = new User
        {
          UserName = "Franklin",
          Email = "franklin@gmail",
          City = "Varna",
          FullName = "Franklin frankov",
          Description = "Здравейте, \n аз съм Франк и се занимавам с ръчно производство на неща от всякакъв характер.",
          Website = "http://neshtata.com/",
          Status = UserStatusType.User
        };

        manager.Create(user, "edtklan12");

        user = new User
        {
          UserName = "Monica",
          Email = "moni@sweden.com",
          City = "Sweden",
          FullName = "Moni Moneva",
          Description = "Привет, \n аз съм моника и обичам да правя плетива, маниста и едиси какво...",
          Website = "http://pletko.org/",
          Status = UserStatusType.User
        };

        manager.Create(user, "edtklan12");

        user = new User
        {
          UserName = "George",
          FullName = "Joro Iliev",
          Email = "george@washusa.com",
          City = "United.States.of.America",
          Description = "Дайте ми пари...",
          Website = "http://usa.us/",
          Status = UserStatusType.User
        };

        manager.Create(user, "edtklan12");
      }
    }

    private void InsertCategoriesToDatabase(CraftyDbContext context)
    {
      var section1 = new Section() { Name = "Празници" };
      var section2 = new Section() { Name = "Декорация" };
      var section3 = new Section() { Name = "Бижута и накити" };
      var section4 = new Section() { Name = "Хартиени изделия" };
      var section5 = new Section() { Name = "Изкуство" };
      var section6 = new Section() { Name = "Мода и текстил" };
      var section7 = new Section() { Name = "Животински свят" };
      var section8 = new Section() { Name = "Сватби" };
      var section9 = new Section() { Name = "Изделия от кожа" };
      var section10 = new Section() { Name = "Ковано желязо" };
      var section11 = new Section() { Name = "Оръжия" };
            
      context.Sections.AddOrUpdate(section1);
      context.Sections.AddOrUpdate(section2);
      context.Sections.AddOrUpdate(section3);
      context.Sections.AddOrUpdate(section4);
      context.Sections.AddOrUpdate(section5);
      context.Sections.AddOrUpdate(section6);
      context.Sections.AddOrUpdate(section7);
      context.Sections.AddOrUpdate(section8);
      context.Sections.AddOrUpdate(section9);
      context.Sections.AddOrUpdate(section10);
      context.SaveChanges();

      //var category1 = new Category() { Name = "Св.Валентин", Section = section1, Thumbnail = "https://cdn.pixabay.com/photo/2016/12/11/22/41/lasagna-1900529_960_720.jpg" };
      //var category2 = new Category() { Name = "8 Март", Section = section1, Thumbnail = "https://cdn.pixabay.com/photo/2016/12/11/22/41/lasagna-1900529_960_720.jpg" };
      //var category3 = new Category() { Name = "Великден", Section = section1, Thumbnail = "https://cdn.pixabay.com/photo/2016/12/11/22/41/lasagna-1900529_960_720.jpg" };
      //var category4 = new Category() { Name = "Новогодишни празници", Section = section1, Thumbnail = "https://cdn.pixabay.com/photo/2016/12/11/22/41/lasagna-1900529_960_720.jpg" };
      //var category5 = new Category() { Name = "Хелуин", Section = section1, Thumbnail = "https://cdn.pixabay.com/photo/2016/12/11/22/41/lasagna-1900529_960_720.jpg" };
      //var category6 = new Category() { Name = "Баба Марта", Section = section1, Thumbnail = "https://cdn.pixabay.com/photo/2016/12/11/22/41/lasagna-1900529_960_720.jpg" };
      //var category7 = new Category() { Name = "Таванна", Section = section2, Thumbnail = "https://cdn.pixabay.com/photo/2016/12/11/22/41/lasagna-1900529_960_720.jpg" };
      //var category8 = new Category() { Name = "Статуетки", Section = section2, Thumbnail = "https://cdn.pixabay.com/photo/2016/12/11/22/41/lasagna-1900529_960_720.jpg" };
      //var category9 = new Category() { Name = "Стенна", Section = section2, Thumbnail = "https://cdn.pixabay.com/photo/2016/12/11/22/41/lasagna-1900529_960_720.jpg" };
      //var category10 = new Category() { Name = "Стъкло", Section = section2, Thumbnail = "https://cdn.pixabay.com/photo/2016/12/11/22/41/lasagna-1900529_960_720.jpg" };
      //var category11 = new Category() { Name = "Други", Section = section2, Thumbnail = "https://cdn.pixabay.com/photo/2016/12/11/22/41/lasagna-1900529_960_720.jpg" };
      //var category12 = new Category() { Name = "Гердани", Section = section3, Thumbnail = "https://cdn.pixabay.com/photo/2016/12/11/22/41/lasagna-1900529_960_720.jpg" };
      //var category13 = new Category() { Name = "Гривни", Section = section3, Thumbnail = "https://cdn.pixabay.com/photo/2016/12/11/22/41/lasagna-1900529_960_720.jpg" };
      //var category14 = new Category() { Name = "Брошки", Section = section3, Thumbnail = "https://cdn.pixabay.com/photo/2016/12/11/22/41/lasagna-1900529_960_720.jpg" };
      //var category15 = new Category() { Name = "От кожа", Section = section3, Thumbnail = "https://cdn.pixabay.com/photo/2016/12/11/22/41/lasagna-1900529_960_720.jpg" };
      //var category16 = new Category() { Name = "Други", Section = section3, Thumbnail = "https://cdn.pixabay.com/photo/2016/12/11/22/41/lasagna-1900529_960_720.jpg" };
      //var category17 = new Category() { Name = "Картички", Section = section4, Thumbnail = "https://cdn.pixabay.com/photo/2016/12/11/22/41/lasagna-1900529_960_720.jpg" };
      //var category18 = new Category() { Name = "Оригами", Section = section4, Thumbnail = "https://cdn.pixabay.com/photo/2016/12/11/22/41/lasagna-1900529_960_720.jpg" };
      //var category19 = new Category() { Name = "Апликации", Section = section4, Thumbnail = "https://cdn.pixabay.com/photo/2016/12/11/22/41/lasagna-1900529_960_720.jpg" };
      //var category20 = new Category() { Name = "Върху стъкло", Section = section5, Thumbnail = "https://cdn.pixabay.com/photo/2016/12/11/22/41/lasagna-1900529_960_720.jpg" };
      //var category21 = new Category() { Name = "Картини", Section = section5, Thumbnail = "https://cdn.pixabay.com/photo/2016/12/11/22/41/lasagna-1900529_960_720.jpg" };
      //var category22 = new Category() { Name = "Гипсови модели", Section = section5, Thumbnail = "https://cdn.pixabay.com/photo/2016/12/11/22/41/lasagna-1900529_960_720.jpg" };
      //var category23 = new Category() { Name = "Плетива", Section = section6, Thumbnail = "https://cdn.pixabay.com/photo/2016/12/11/22/41/lasagna-1900529_960_720.jpg" };
      //var category24 = new Category() { Name = "Шапки и шалове", Section = section6, Thumbnail = "https://cdn.pixabay.com/photo/2016/12/11/22/41/lasagna-1900529_960_720.jpg" };
      //var category25 = new Category() { Name = "Ръкавици", Section = section6, Thumbnail = "https://cdn.pixabay.com/photo/2016/12/11/22/41/lasagna-1900529_960_720.jpg" };
      //var category26 = new Category() { Name = "Горници", Section = section6, Thumbnail = "https://cdn.pixabay.com/photo/2016/12/11/22/41/lasagna-1900529_960_720.jpg" };
      //var category27 = new Category() { Name = "Долници", Section = section6, Thumbnail = "https://cdn.pixabay.com/photo/2016/12/11/22/41/lasagna-1900529_960_720.jpg" };
      //var category28 = new Category() { Name = "Къщички", Section = section7, Thumbnail = "https://cdn.pixabay.com/photo/2016/12/11/22/41/lasagna-1900529_960_720.jpg" };
      //var category29 = new Category() { Name = "Дрешки", Section = section7, Thumbnail = "https://cdn.pixabay.com/photo/2016/12/11/22/41/lasagna-1900529_960_720.jpg" };
      //var category30 = new Category() { Name = "Аксесоари", Section = section7, Thumbnail = "https://cdn.pixabay.com/photo/2016/12/11/22/41/lasagna-1900529_960_720.jpg" };
      //var category31 = new Category() { Name = "Катерушки", Section = section7, Thumbnail = "https://cdn.pixabay.com/photo/2016/12/11/22/41/lasagna-1900529_960_720.jpg" };
      //var category32 = new Category() { Name = "Букети", Section = section8, Thumbnail = "https://cdn.pixabay.com/photo/2016/12/11/22/41/lasagna-1900529_960_720.jpg" };
      //var category33 = new Category() { Name = "Късметчета", Section = section8, Thumbnail = "https://cdn.pixabay.com/photo/2016/12/11/22/41/lasagna-1900529_960_720.jpg" };
      //var category34 = new Category() { Name = "Други", Section = section8, Thumbnail = "https://cdn.pixabay.com/photo/2016/12/11/22/41/lasagna-1900529_960_720.jpg" };
      //var category35 = new Category() { Name = "Колани", Section = section9, Thumbnail = "https://cdn.pixabay.com/photo/2016/12/11/22/41/lasagna-1900529_960_720.jpg" };
      //var category36 = new Category() { Name = "Обувки", Section = section9, Thumbnail = "https://cdn.pixabay.com/photo/2016/12/11/22/41/lasagna-1900529_960_720.jpg" };
      //var category37 = new Category() { Name = "Чанти", Section = section9, Thumbnail = "https://cdn.pixabay.com/photo/2016/12/11/22/41/lasagna-1900529_960_720.jpg" };
      //var category38 = new Category() { Name = "Уреди за камина", Section = section10, Thumbnail = "https://cdn.pixabay.com/photo/2016/12/11/22/41/lasagna-1900529_960_720.jpg" };
      //var category39 = new Category() { Name = "Огради", Section = section10, Thumbnail = "https://cdn.pixabay.com/photo/2016/12/11/22/41/lasagna-1900529_960_720.jpg" };
      //var category40 = new Category() { Name = "Креативна декорация", Section = section10, Thumbnail = "https://cdn.pixabay.com/photo/2016/12/11/22/41/lasagna-1900529_960_720.jpg" };
      //var category41 = new Category() { Name = "Ножове и брадви", Section = section11, Thumbnail = "https://cdn.pixabay.com/photo/2016/12/11/22/41/lasagna-1900529_960_720.jpg" };
      //var category42 = new Category() { Name = "Огнестрелни", Section = section11, Thumbnail = "https://cdn.pixabay.com/photo/2016/12/11/22/41/lasagna-1900529_960_720.jpg" };

      var category1 = new Category() { Name = "Св.Валентин", Section = section1 };
      var category2 = new Category() { Name = "8 Март", Section = section1 };
      var category3 = new Category() { Name = "Великден", Section = section1 };
      var category4 = new Category() { Name = "Новогодишни празници", Section = section1 };
      var category5 = new Category() { Name = "Хелуин", Section = section1 };
      var category6 = new Category() { Name = "Баба Марта", Section = section1 };
      var category7 = new Category() { Name = "Таванна", Section = section2 };
      var category8 = new Category() { Name = "Статуетки", Section = section2 };
      var category9 = new Category() { Name = "Стенна", Section = section2 };
      var category10 = new Category() { Name = "Стъкло", Section = section2 };
      var category11 = new Category() { Name = "Други", Section = section2 };
      var category12 = new Category() { Name = "Гердани", Section = section3 };
      var category13 = new Category() { Name = "Гривни", Section = section3 };
      var category14 = new Category() { Name = "Брошки", Section = section3 };
      var category15 = new Category() { Name = "От кожа", Section = section3 };
      var category16 = new Category() { Name = "Други", Section = section3 };
      var category17 = new Category() { Name = "Картички", Section = section4 };
      var category18 = new Category() { Name = "Оригами", Section = section4 };
      var category19 = new Category() { Name = "Апликации", Section = section4 };
      var category20 = new Category() { Name = "Върху стъкло", Section = section5 };
      var category21 = new Category() { Name = "Картини", Section = section5 };
      var category22 = new Category() { Name = "Гипсови модели", Section = section5 };
      var category23 = new Category() { Name = "Плетива", Section = section6 };
      var category24 = new Category() { Name = "Шапки и шалове", Section = section6 };
      var category25 = new Category() { Name = "Ръкавици", Section = section6 };
      var category26 = new Category() { Name = "Горници", Section = section6 };
      var category27 = new Category() { Name = "Долници", Section = section6 };
      var category28 = new Category() { Name = "Къщички", Section = section7 };
      var category29 = new Category() { Name = "Дрешки", Section = section7 };
      var category30 = new Category() { Name = "Аксесоари", Section = section7 };
      var category31 = new Category() { Name = "Катерушки", Section = section7 };
      var category32 = new Category() { Name = "Букети", Section = section8 };
      var category33 = new Category() { Name = "Късметчета", Section = section8 };
      var category34 = new Category() { Name = "Други", Section = section8 };
      var category35 = new Category() { Name = "Колани", Section = section9 };
      var category36 = new Category() { Name = "Обувки", Section = section9 };
      var category37 = new Category() { Name = "Чанти", Section = section9 };
      var category38 = new Category() { Name = "Уреди за камина", Section = section10 };
      var category39 = new Category() { Name = "Огради", Section = section10 };
      var category40 = new Category() { Name = "Креативна декорация", Section = section10 };
      var category41 = new Category() { Name = "Ножове", Section = section11 };

      context.Categories.Add(category1);
      context.Categories.Add(category2);
      context.Categories.Add(category3);
      context.Categories.Add(category4);
      context.Categories.Add(category5);
      context.Categories.Add(category6);
      context.Categories.Add(category7);
      context.Categories.Add(category8);
      context.Categories.Add(category9);
      context.Categories.Add(category10);
      context.Categories.Add(category11); 
      context.Categories.Add(category12);
      context.Categories.Add(category13);
      context.Categories.Add(category14);
      context.Categories.Add(category15);
      context.Categories.Add(category16);
      context.Categories.Add(category17);
      context.Categories.Add(category18);
      context.Categories.Add(category19);
      context.Categories.Add(category20);
      context.Categories.Add(category21);
      context.Categories.Add(category22);
      context.Categories.Add(category23);
      context.Categories.Add(category24);
      context.Categories.Add(category25);
      context.Categories.Add(category26);
      context.Categories.Add(category27);
      context.Categories.Add(category28);
      context.Categories.Add(category29);
      context.Categories.Add(category30);
      context.Categories.Add(category31);
      context.Categories.Add(category32);
      context.Categories.Add(category33);
      context.Categories.Add(category34);
      context.Categories.Add(category35);
      context.Categories.Add(category36);
      context.Categories.Add(category37);
      context.Categories.Add(category38);
      context.Categories.Add(category39);
      context.Categories.Add(category40);
      context.Categories.Add(category41);
      context.SaveChanges();
    }

    private void InsertItemsToDatabase(CraftyDbContext context)
    {
      var userGeorge = context.Users.FirstOrDefault(u => u.UserName == "George");
      var userMonica = context.Users.FirstOrDefault(u => u.UserName == "Monica");
      var userFranklin = context.Users.FirstOrDefault(u => u.UserName == "Franklin");

      var item1 = new Item()
      {
        Title = "Брошка",
        Description = "Ръчно изработена брошка, от сребро.",
        PostedOn = DateTime.Now,
        Price = decimal.Parse("20.50"),
        Seller = userGeorge,
        Thumbnail = "http://www.lorempixel.com/300/200/",
        Category = context.Categories.FirstOrDefault(c => c.Name == "Брошки"),
        Quantity = 10
      };

      var item2 = new Item()
      {
        Title = "Статуетка",
        Description = "Дървена статуетка.",
        PostedOn = DateTime.Now,
        Price = decimal.Parse("240"),
        Seller = userGeorge,
        Thumbnail = "http://www.lorempixel.com/300/400/",
        Category = context.Categories.FirstOrDefault(c => c.Name == "Декорация"),
        Quantity = 100
      };

      var item3 = new Item()
      {
        Title = "Бъчва",
        Description = "Качествена дървена бъчва за винарски дейности.",
        PostedOn = DateTime.Now,
        Price = decimal.Parse("100"),
        Seller = userFranklin,
        Thumbnail = "http://www.lorempixel.com/300/500/",
        Category = context.Categories.FirstOrDefault(c => c.Name == "Брадви"),
        Quantity = 99
      };

      var item4 = new Item()
      {
        Title = "Брадва",
        Description = "Качествена брадва.",
        PostedOn = DateTime.Now,
        Price = decimal.Parse("40"),
        Seller = userGeorge,
        Thumbnail = "http://www.lorempixel.com/300/200/",
        Category = context.Categories.FirstOrDefault(c => c.Name == "Брадви"),
        Quantity = 10
      };

      var item5 = new Item()
      {
        Title = "Терлици",
        Description = "Качествени вълнени ръчно плетени терлици.",
        PostedOn = DateTime.Now,
        Price = decimal.Parse("20.50"),
        Seller = userMonica,
        Thumbnail = "http://www.lorempixel.com/300/300/",
        Category = context.Categories.FirstOrDefault(c => c.Name == "Терлици"),
        Quantity = 1245
      };

      context.Items.AddOrUpdate(item1);
      context.Items.AddOrUpdate(item2);
      context.Items.AddOrUpdate(item3);
      context.Items.AddOrUpdate(item4);
      context.Items.AddOrUpdate(item5);
      context.SaveChanges();
    }
  }
}
