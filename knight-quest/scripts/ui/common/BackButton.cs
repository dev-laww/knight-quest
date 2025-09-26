using Godot;
using System;
using Game.Autoloads;

public partial class BackButton : Button
{
    public override void _Ready()
    {
        base._Ready();
        Pressed += () => Navigator.Back();
    }
}
