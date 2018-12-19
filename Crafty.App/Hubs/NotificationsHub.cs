namespace Crafty.App.Hubs
{
  using Microsoft.AspNet.SignalR;
  using Microsoft.AspNet.SignalR.Hubs;
  using Models.ViewModels;

  [HubName("notifications")]
  [Authorize]
  public class NotificationsHub : Hub
  {
    public void SentOrderNotification(string receiver, ConciseNotificationViewModel notification)
    {
      var hub = GlobalHost.ConnectionManager.GetHubContext<NotificationsHub>();
      hub.Clients.User(receiver).sentOrderNotification(notification);
    }
  }
}