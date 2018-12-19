namespace Crafty.Data
{
  using Crafty.Models;
  using Microsoft.AspNet.Identity.EntityFramework;
  using Migrations;
  using System.Data.Entity;
  using System.Data.Entity.ModelConfiguration.Conventions;

  public class CraftyDbContext : IdentityDbContext<User>
  {
    public CraftyDbContext()
        : base("DefaultConnection", throwIfV1Schema: false)
    {
      Database.SetInitializer(new MigrateDatabaseToLatestVersion<CraftyDbContext, Configuration>());
    }

    public static CraftyDbContext Create()
    {
      return new CraftyDbContext();
    }

    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
      modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

      modelBuilder.Entity<Item>().Property(i => i.RowVersion).IsConcurrencyToken();

      modelBuilder.Entity<Item>()
        .HasMany<Like>(i => i.Likes)
        .WithRequired(l => l.Item)
        .WillCascadeOnDelete(true);

      modelBuilder.Entity<User>()
        .HasMany<Item>(i => i.Favourites)
        .WithMany()
        .Map(m =>
        {
          m.MapLeftKey("UserId");
          m.MapRightKey("ItemId");
          m.ToTable("UserFavourites");
        });

      modelBuilder.Entity<User>()
        .HasMany<Order>(i => i.AwaitingOrders)
        .WithMany()
        .Map(m =>
        {
          m.MapLeftKey("SellerId");
          m.MapRightKey("OrderId");
          m.ToTable("AwaitngOrders");
        });

      modelBuilder.Entity<User>()
        .HasMany<Order>(i => i.SentOrders)
        .WithMany()
        .Map(m =>
        {
          m.MapLeftKey("SellerId");
          m.MapRightKey("OrderId");
          m.ToTable("SentOrders");
        });

      modelBuilder.Entity<User>()
        .HasMany<Order>(i => i.FinishedOrders)
        .WithMany()
        .Map(m =>
        {
          m.MapLeftKey("SellerId");
          m.MapRightKey("OrderId");
          m.ToTable("FinishedOrders");
        });

      modelBuilder.Entity<User>()
        .HasMany<Order>(u => u.PurchaseHistory)
        .WithMany()
        .Map(m =>
        {
          m.MapLeftKey("UserId");
          m.MapRightKey("OrderId");
          m.ToTable("UserOrders");
        });

      modelBuilder.Entity<User>()
        .HasMany<Feedback>(u => u.GivenFeedbacks)
        .WithMany()
        .Map(m =>
        {
          m.MapLeftKey("UserId");
          m.MapRightKey("FeedbackId");
          m.ToTable("UserGivenFeedbacks");
        });

      modelBuilder.Entity<User>()
        .HasMany<Feedback>(u => u.ReceivedFeedbacks)
        .WithMany()
        .Map(m =>
        {
          m.MapRightKey("UserId");
          m.MapLeftKey("FeedbackId");
          m.ToTable("UserReceivedFeedbacks");
        });

      modelBuilder.Entity<Blog>()
        .HasMany<Item>(b => b.RelatedItems)
        .WithMany()
        .Map(m =>
        {
          m.MapLeftKey("BlogId");
          m.MapRightKey("ItemId");
          m.ToTable("BlogRelatedItems");
        });

      modelBuilder.Entity<Blog>()
        .HasMany<BlogComment>(b => b.Comments)
        .WithRequired(c => c.Blog)
        .WillCascadeOnDelete(true);

      //modelBuilder.Entity<Blog>()
      //  .HasMany<BlogComment>(b => b.Comments)
      //  .WithMany()
      //  .Map(m =>
      //  {
      //    m.MapLeftKey()
      //  })

      base.OnModelCreating(modelBuilder);
    }

    public virtual IDbSet<Item> Items { get; set; }

    public virtual IDbSet<Section> Sections { get; set; }

    public virtual IDbSet<Category> Categories { get; set; }

    public virtual IDbSet<Like> Likes { get; set; }

    public virtual IDbSet<Order> Orders { get; set; }

    public virtual IDbSet<Feedback> Feedbacks { get; set; }

    public virtual IDbSet<Notification> Notifications { get; set; }

    public virtual IDbSet<Blog> Blogs { get; set; }

    public virtual IDbSet<BlogComment> BlogComments { get; set; }

    public IDbSet<IdentityUserLogin> UserLogins { get; set; }
  }
}
