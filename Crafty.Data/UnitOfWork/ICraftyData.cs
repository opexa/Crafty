namespace Crafty.Data.UnitOfWork
{
  using Crafty.Models;
  using Crafty.Data.Repositories;
  using System.Threading.Tasks;

  public interface ICraftyData
  {
    IRepository<User> Users { get; }

    IRepository<Item> Items { get; }

    IRepository<Section> Sections { get; }

    IRepository<Category> Categories { get; }

    IRepository<Like> Likes { get; }

    IRepository<Order> Orders { get; }

    IRepository<Feedback> Feedbacks { get; }

    IRepository<Notification> Notifications { get; }

    IRepository<Blog> Blogs { get; }

    IRepository<BlogComment> BlogComments { get; }

    void SaveChanges();
  }
}
