using Game.Data;
using Godot;


namespace Game.Data;

public partial class RunConfig : RefCounted
{
    public enum GradeLevel
    {
        Second,
        Third,
        Fourth,
        Fifth,
        Sixth,
    }

    public enum SubjectArea
    {
        English,
        Math,
    }

    public GradeLevel Grade;
    public SubjectArea Subject;
    public CombatSequence CombatSequence;
}