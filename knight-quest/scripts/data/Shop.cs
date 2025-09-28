using System.Collections.Generic;

namespace Game.Data;

public class Shop
{
    public int Stars { get; set; }
    public List<PurchaseHistory> PurchaseHistory { get; set; } = new();
}