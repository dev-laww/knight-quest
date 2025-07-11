using Game.Data;
using Godot;


namespace Game;

public partial class RunDetails : RefCounted
{
    public enum GradeLevel { }
    public enum SubjectArea { }

    public GradeLevel Grade;
    public SubjectArea Subject;
    public RunConfiguration RunConfiguration;
}