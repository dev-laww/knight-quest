using System.Threading.Tasks;
using Godot;
using System.Collections.Generic;
using Game.Data;
using Game.Utils;
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

    private Dictionary<Cosmetic.CosmeticType, Cosmetic> equippedParts = new();
    private Dictionary<Cosmetic.CosmeticType, AnimatedSprite2D> partSprites;
    private Dictionary<Cosmetic.CosmeticType, SpriteFrames> defaultFrames;

    public override void _Notification(int what)
    {
        if (what != NotificationSceneInstantiated) return;
        WireNodes();
    }

    public override void _Ready()
    {
        this.AddToGroup();

        playback = (AnimationNodeStateMachinePlayback)animationTree.Get("parameters/playback");

        partSprites = new Dictionary<Cosmetic.CosmeticType, AnimatedSprite2D>
        {
            { Cosmetic.CosmeticType.Hair, hairSprite },
            { Cosmetic.CosmeticType.Clothes, clothesSprite },
            { Cosmetic.CosmeticType.Head, headSprite }
        };

        defaultFrames = new Dictionary<Cosmetic.CosmeticType, SpriteFrames>();
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

    public void EquipCosmetic(Cosmetic part)
    {
        UnequipCosmetic(part.Type);

        equippedParts[part.Type] = part;
        part.Equipped = true;

        if (partSprites.TryGetValue(part.Type, out var sprite))
        {
            sprite.SpriteFrames = part.AnimationFrames;
        }

        RefreshSprites();
    }

    public void UnequipCosmetic(Cosmetic part) => UnequipCosmetic(part.Type);

    public void UnequipCosmetic(Cosmetic.CosmeticType type)
    {
        if (equippedParts.TryGetValue(type, out var part))
        {
            part.Equipped = false;
            equippedParts.Remove(type);

            if (partSprites.TryGetValue(type, out var sprite))
            {
                sprite.SpriteFrames = defaultFrames[type];
            }
        }

        RefreshSprites();
    }
    public void PreviewCosmetic(Cosmetic part)
    {
        if (part == null) return;

        // Check if that part type exists in the player
        if (partSprites.TryGetValue(part.Type, out var sprite))
        {
            sprite.SpriteFrames = part.AnimationFrames;
            RefreshSprites();
            Logger.Info($"[DEBUG] Previewing cosmetic: {part.Name} ({part.Type})");
        }
    }
    
    public void ClearPreview(Cosmetic.CosmeticType type)
    {
        // Reset to default visuals (not equipped state)
        if (partSprites.TryGetValue(type, out var sprite))
        {
            sprite.SpriteFrames = defaultFrames[type];
            RefreshSprites();
            Logger.Info($"[DEBUG] Cleared preview for {type}");
        }
    }



    public bool IsCosmeticEquipped(Cosmetic part)
    {
        var exists = equippedParts.TryGetValue(part.Type, out var equipped);

        return part.ResourcePath == equipped?.ResourcePath && exists;
    }

    private void RefreshSprites()
    {
        foreach (var sprite in partSprites.Values)
        {
            sprite.Stop();
            sprite.Play("default");
        }

        baseSprite.Stop();
        baseSprite.Play("default");
        faceSprite.Stop();
        faceSprite.Play("default");
    }
}