using Game.Autoloads;
using Godot;
using GodotUtilities;
using Logger = Game.Utils.Logger;

namespace Game;

[Scene]
public partial class MainMenu : CanvasLayer
{
    [Node] private Button startButton;
    [Node] private Button shopButton;


    public override void _Notification(int what)
    {
        if (what != NotificationSceneInstantiated) return;

        WireNodes();
    }

    public override void _Ready()
    {
        if (SaveManager.Data != null)
        {
            SaveManager.LoadInventory();
            ShopManager.Stars = SaveManager.Data.Shop.Stars;
            Logger.Info($"Loaded inventory and stars for {SaveManager.Data.Account.Username}");
            foreach (var item in SaveManager.Data.Inventory)
            {
                Logger.Info($"Loaded item: {item.Id} x{item.Quantity}");
            }
        }
        else
        {
            InventoryManager.Instance.ClearInventory();
            ShopManager.Stars = 0;
            Logger.Error("No account logged in, inventory cleared and stars set to 0.");
        }

        startButton.Pressed += OnStartButtonPressed;
        shopButton.Pressed += OnShopButtonPressed;
    }

    private void OnStartButtonPressed()
    {
        AudioManager.Instance.PlayClick();
        Navigator.Push("res://scenes/ui/screens/subject_select.tscn");
    }

    private void OnShopButtonPressed()
    {
        AudioManager.Instance.PlayClick();
        Navigator.Push("res://scenes/ui/screens/shop.tscn");
    }
}