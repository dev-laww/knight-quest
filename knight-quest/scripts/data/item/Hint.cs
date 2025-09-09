using Game;
using Game.Data;
using Game.Utils;

public partial class Hint : Consumable
{
    public override void Use(Entity target)
    {
        var hud = target.GetHud();
        Logger.Debug($"[Hint::Use] HUD is {(hud == null ? "null" : "not null")}");
        hud?.RevealCorrectAnswer();
    }
}