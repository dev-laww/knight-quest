using Game.Data;
using Godot;


namespace Game;

public partial class RunConfig : RefCounted
{
    public enum GradeLevel { }
    public enum SubjectArea { }

    public GradeLevel Grade;
    public SubjectArea Subject;
    public CombatSequence CombatSequence;
}