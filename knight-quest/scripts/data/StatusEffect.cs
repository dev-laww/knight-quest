using Godot;

namespace Game.Data;

public abstract partial class StatusEffect : Resource
{
    public class Info
    {
        public string Id;
        public bool IsGuaranteed;
        public float Chance;
        public int Turns = 1;
    }


    [Export] public string Id;
    [Export] public string Name;
    [Export] private int TurnDuration = 1;

    public int RemainingDuration;
    protected Entity Target;

    public void Update()
    {
        RemainingDuration -= TurnDuration;
        Tick();
    }

    public void ApplyStatusEffect(Entity target)
    {
        Target = target;
        RemainingDuration += TurnDuration;
        Apply();
    }

    protected virtual void Tick() { }

    public virtual void Apply() { }

    public virtual void Remove() { }
}