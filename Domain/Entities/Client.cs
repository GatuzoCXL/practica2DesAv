using System;
using System.Collections.Generic;

namespace practica_VargasLeonardo.Domain.Entities;

public partial class Client
{
    public int Clientid { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
