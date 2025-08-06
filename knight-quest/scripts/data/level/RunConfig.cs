using Game.Data;
using Godot;


namespace Game.Data;

public partial class RunConfig : RefCounted
{
    public enum GradeLevel { }
    public enum SubjectArea { }

    public GradeLevel Grade;
    public SubjectArea Subject;
    public CombatSequence CombatSequence;
}