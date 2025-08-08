using Game.Data;
    using Game.Utils;
    using Godot;
    using GodotUtilities;
    
    namespace Game.UI;
    
    [Scene]
    public partial class Slot : Panel
    {
        [Node] public TextureRect icon;
        [Node] private AnimationPlayer animationPlayer;
    
        [Signal]
        public delegate void PressedEventHandler(Slot slot);
    
        private bool selected;
        private Item item;
    
        public bool Selected
        {
            get => selected;
            set
            {
                selected = value;
                // animationPlayer.Play(selected ? "select" : "RESET");
            }
        }
    
        public Item Item
        {
            get => item;
            set
            {
                item = value;
                icon.Texture = item?.Icon;
            }
        }
    
        public override void _Notification(int what)
        {
            if (what != NotificationSceneInstantiated) return;
            WireNodes();
        }
    
        public override void _Ready()
        {
            GuiInput += OnGuiInput;
        }
    
        private void OnGuiInput(InputEvent @event)
        {
            if (@event is not InputEventMouseButton mouseAction) return;
            if (!mouseAction.Pressed) return;
    
            // animationPlayer.Play(selected ? "select" : "RESET");
            Logger.Info($"[DEBUG] Slot clicked: {item?.Name}");
            EmitSignalPressed(this);
        }
    }