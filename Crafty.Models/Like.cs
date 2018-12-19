﻿namespace Crafty.Models
{
  using Crafty.Models;
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using System.Threading.Tasks;

  public class Like
  {
    public int Id { get; set; }

    public virtual Item Item { get; set; }

    public virtual User User { get; set; }
  }
}