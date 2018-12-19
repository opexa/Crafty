using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Crafty.Data.Repositories;
using Crafty.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crafty.Data;

namespace Crafty.Data.UnitOfWork
{
  public class CraftyData : ICraftyData
  {
    private readonly DbContext dbContext;

    private readonly IDictionary<Type, object> repositories;

    private IUserStore<User> userStore;

    public CraftyData()
        : this(new CraftyDbContext())
    {
    }

    public CraftyData(DbContext dbContext)
    {
      this.dbContext = dbContext;
      this.repositories = new Dictionary<Type, object>();
    }

    public IRepository<User> Users
    {
      get { return this.GetRepository<User>(); }
    }

    public IRepository<Item> Items
    {
      get { return this.GetRepository<Item>(); }
    }

    public IRepository<Section> Sections
    {
      get { return this.GetRepository<Section>(); }
    }

    public IRepository<Category> Categories
    {
      get { return this.GetRepository<Category>(); }
    }

    public IRepository<Like> Likes
    {
      get { return this.GetRepository<Like>(); }
    }

    public IRepository<Order> Orders
    {
      get { return this.GetRepository<Order>(); }
    }

    public IRepository<Feedback> Feedbacks
    {
      get { return this.GetRepository<Feedback>(); }
    }

    public IRepository<Notification> Notifications
    {
      get { return this.GetRepository<Notification>(); }
    }

    public IRepository<Blog> Blogs
    {
      get { return this.GetRepository<Blog>(); }
    }

    public IRepository<BlogComment> BlogComments
    {
      get { return this.GetRepository<BlogComment>(); }
    }

    public IUserStore<User> UserStore
    {
      get
      {
        if (this.userStore == null)
        {
          this.userStore = new UserStore<User>(this.dbContext);
        }

        return this.userStore;
      }
    }

    public void SaveChanges()
    {
      try
      {
        this.dbContext.SaveChanges();
      }
      catch(Exception ex)
      {
      }
    }

    private IRepository<T> GetRepository<T>() where T : class
    {
      if (!this.repositories.ContainsKey(typeof(T)))
      {
        var type = typeof(GenericEfRepository<T>);
        this.repositories.Add(
            typeof(T),
            Activator.CreateInstance(type, this.dbContext));
      }

      return (IRepository<T>)this.repositories[typeof(T)];
    }
  }
}
