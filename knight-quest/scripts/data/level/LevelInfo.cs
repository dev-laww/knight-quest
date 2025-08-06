using Godot;
using System;

namespace Game.Data;

[GlobalClass]
public partial class LevelInfo : Resource
{
    [Export] public string LevelName;
    [Export(PropertyHint.MultilineText)] public string LevelDescription;
    [Export] public CombatSequence CombatSequence;
}