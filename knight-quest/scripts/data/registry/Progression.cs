using System.Collections.Generic;

namespace Game.Data;

public class Progression
{
    public int TotalStarsEarned { get; set; }
    public List<FinishedLevel> LevelsFinished { get; set; } = new();
}