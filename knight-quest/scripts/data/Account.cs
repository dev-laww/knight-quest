using System.Collections.Generic;

namespace Game.Data;

public class Account
{
    public string Token { get; set; }
    public string Username { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Role { get; set; }
    public Progression Progression { get; set; } = new();
    public List<SavedItem> Inventory { get; set; } = new();
    public Shop Shop { get; set; } = new();
}