namespace Crafty.Models
{
  using Crafty.Models;
  using System;
  using System.ComponentModel;
  using System.ComponentModel.DataAnnotations;

  public class Notification
  {
    [Key]
    public int Id { get; set; }

    public virtual User Sender { get; set; }

    [Required]
    public string SenderId { get; set; }

    public virtual User Receiver { get; set; }

    [Required]
    public string ReceiverId { get; set; }

    public NotificationType Type { get; set; }

    public DateTime PostedOn { get; set; }

    public int ObjectId { get; set; }

    public string ObjectName { get; set; }

    [DefaultValue(false)]
    public bool Seen { get; set; }

    //public NotificationObejctType ObjectType { get; set; }
  }
}
