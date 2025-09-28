using System.Collections.Generic;
using Newtonsoft.Json;

namespace Game.Data;

public class Save
{
    [JsonProperty("accounts")] public List<Account> Accounts { get; private set; } = new();

    public void AddAccount(Account account)
    {
        if (account == null || string.IsNullOrWhiteSpace(account.Username))
            return;

        if (!Accounts.Exists(a => a.Username == account.Username))
            Accounts.Add(account);
    }
}