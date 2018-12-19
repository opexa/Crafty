
namespace Crafty.App.Hubs
{
  using AutoMapper;
  using Crafty.Models;
  using Microsoft.AspNet.SignalR;
  using Microsoft.AspNet.SignalR.Hubs;
  using Models.ViewModels;
  using System.Linq;

  [HubName("comments")]
  public class CommentsHub : Hub
  {
    public static void RefreshBlogComments(BlogComment comment)
    {
      var hub = GlobalHost.ConnectionManager.GetHubContext<CommentsHub>();
      hub.Clients.All.updateBlogComments(Mapper.Map<BlogCommentViewModel>(comment));
    }

    public static void UpdateBlogComment(string content, int id)
    {
      var hub = GlobalHost.ConnectionManager.GetHubContext<CommentsHub>();
      hub.Clients.All.updateCommentEdit(content, id);
    }
  }
}