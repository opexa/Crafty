using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Crafty.App.Models.ViewModels
{
  public class ConciseNotificationViewModel
  {
    public int Id { get; set; }

    public string Type { get; set; }

    public string SenderName { get; set; }

    //public string SenderId { get; set; }

    public string ObjectName { get; set; }

    public int ObjectId { get; set; }


    public bool Seen { get; set; }

    public DateTime PostedOn { get; set; }
  }
}