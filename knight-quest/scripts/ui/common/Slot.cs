using Game.Data;
using Game.Utils;
using Godot;
using GodotUtilities;

namespace Game.UI;

[Scene]
public partial class Slot : Panel
{
    [Node] private Label label;
    [Node] public TextureRect icon;
    [Node] private AnimationPlayer animationPlayer;

    [Signal] public delegate void UpdatedEventHandler(ItemGroup item);
    [Signal] public delegate void PressedEventHandler(Slot slot);

    private bool selected;
    private ItemGroup item;

    // --- Cooldown variables ---
    [Export] public float CooldownSeconds = 2f; // set in inspector
    private float cooldownTimer = 0f;
    private bool onCooldown = false;

    public bool Selected
    {
        get => selected;
        set
        {
            selected = value;
            // animationPlayer.Play(selected ? "select" : "RESET");
        }
    }

    public ItemGroup Item
    {
        get => item;
        set
        {
            item = value;
            UpdateSlot();
        }
    }

    public override void _Notification(int what)
    {
        if (what != NotificationSceneInstantiated) return;
        WireNodes();
    }

    public override void _Ready()
    {
        UpdateSlot();
        GuiInput += OnGuiInput;
    }

    public override void _Process(double delta)
    {
        if (onCooldown)
        {
            cooldownTimer -= (float)delta;
            if (cooldownTimer <= 0f)
            {
                onCooldown = false;
                icon.Modulate = Colors.White; // Reset icon color
            }
        }
    }

    private void UpdateSlot()
    {
        label.Visible = item is not null && item.Quantity > 1;
        icon.Texture = item?.Item.Icon;
        label.Text = item?.Quantity > 999 ? "999+" : item?.Quantity.ToString();
        EmitSignalUpdated(item);
    }

    private void OnGuiInput(InputEvent @event)
    {
        if (@event is not InputEventMouseButton mouseAction) return;
        if (!mouseAction.Pressed) return;

        if (onCooldown) 
            return; // Ignore presses during cooldown

        animationPlayer.Play(selected ? "select" : "RESET");

        UseItem(); // consume effect
        StartCooldown(); // begin cooldown
        EmitSignalPressed(this);
    }

    private void UseItem()
    {
        if (item is null) return;

       Logger.Info($"Using {item.Item.Name} from slot {Name}");
        // TODO: call consumable effect logic here
        item.Quantity--;
        UpdateSlot();
    }

    private void StartCooldown()
    {
        onCooldown = true;
        cooldownTimer = CooldownSeconds;
        icon.Modulate = Colors.Gray; 
    }
}
