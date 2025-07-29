using System.Threading.Tasks;
using Godot;
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

    public override void _Notification(int what)
    {
        if (what != NotificationSceneInstantiated) return;

        WireNodes();
    }

    public override void _Ready()
    {
        this.AddToGroup();
        playback = (AnimationNodeStateMachinePlayback)animationTree.Get("parameters/playback");
    }

    public override async Task TakeTurn(Entity target)
    {
        await base.TakeTurn(target);
        playback.Travel(ATTACK);

        await ToSignal(animationTree, "animation_finished");
    }

    public void ApplyPart(PartItem part)
    {
      switch (part.Type)
      {
          case PartItem.PartType.Hair:
              hairSprite.SpriteFrames = part.AnimationFrames;
              hairSprite.Play("default");
              break;
          case PartItem.PartType.Clothes:
              clothesSprite.SpriteFrames = part.AnimationFrames;
              clothesSprite.Play("default");
              break;
          case PartItem.PartType.Head:
              headSprite.SpriteFrames = part.AnimationFrames;
              headSprite.Play("default");
              break;
          // default:
      }

        
    }
}