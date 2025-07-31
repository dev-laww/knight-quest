using System.Threading.Tasks;
using Godot;
using System.Collections.Generic;
using GodotUtilities;

namespace Game.Entities;

// TODO: make use of a state machine for the player
[Scene]
public partial class Player : Entity
{
    private const string ATTACK = "attack";
    private const string HURT = "hurt";

    [Node] private AnimatedSprite2D baseSprite;
    [Node] private AnimatedSprite2D faceSprite;
    [Node] private AnimatedSprite2D hairSprite;
    [Node] private AnimatedSprite2D headSprite;
    [Node] private AnimatedSprite2D clothesSprite;
    [Node] private AnimationTree animationTree;

    private AnimationNodeStateMachinePlayback playback;

    private Dictionary<PartItem.PartType, PartItem> equippedParts = new();
    private Dictionary<PartItem.PartType, AnimatedSprite2D> partSprites;
    private Dictionary<PartItem.PartType, SpriteFrames> defaultFrames;

    public override void _Notification(int what)
    {
        if (what != NotificationSceneInstantiated) return;
        WireNodes();
    }

    public override void _Ready()
    {
        this.AddToGroup();

        playback = (AnimationNodeStateMachinePlayback)animationTree.Get("parameters/playback");

        partSprites = new Dictionary<PartItem.PartType, AnimatedSprite2D>
        {
            { PartItem.PartType.Hair, hairSprite },
            { PartItem.PartType.Clothes, clothesSprite },
            { PartItem.PartType.Head, headSprite }
        };

        defaultFrames = new Dictionary<PartItem.PartType, SpriteFrames>();
        foreach (var kvp in partSprites)
        {
            defaultFrames[kvp.Key] = kvp.Value.SpriteFrames.Duplicate() as SpriteFrames;
        }

        RefreshSprites();
    }

    public override async Task TakeTurn(Entity target)
    {
        await base.TakeTurn(target);
        playback.Travel(ATTACK);
        await ToSignal(animationTree, "animation_finished");
    }

    public void ApplyPart(PartItem part)
    {
        UnequipPart(part.Type);

        equippedParts[part.Type] = part;
        part.IsEquipped = true;

        if (partSprites.TryGetValue(part.Type, out var sprite))
        {
            sprite.SpriteFrames = part.AnimationFrames;
        }

        RefreshSprites();
    }

    public void UnequipPart(PartItem part)
    {
        UnequipPart(part.Type);
    }

    public void UnequipPart(PartItem.PartType type)
    {
        if (equippedParts.TryGetValue(type, out var part))
        {
            part.IsEquipped = false;
            equippedParts.Remove(type);

            if (partSprites.TryGetValue(type, out var sprite))
            {
                sprite.SpriteFrames = defaultFrames[type];
            }
        }

        RefreshSprites();
    }

    public bool IsPartEquipped(PartItem part) =>
        equippedParts.TryGetValue(part.Type, out var equipped) && equipped == part;

    private void RefreshSprites()
    {
        foreach (var sprite in partSprites.Values)
        {
            sprite.Stop();
            sprite.Play("default");
        }

        baseSprite.Stop(); baseSprite.Play("default");
        faceSprite.Stop(); faceSprite.Play("default");
    }
}
