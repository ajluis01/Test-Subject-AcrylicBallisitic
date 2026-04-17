using UnityEngine;

// The Game Events used across the Game.
// Anytime there is a need for a new event, it should be added here.

public class GameEvent
{
}

public class DifficultyChangedEvent : GameEvent
{
    public DifficultyProgression.DifficultyLevel newDifficulty;
    public DifficultyChangedEvent(DifficultyProgression.DifficultyLevel difficulty)
    {
        newDifficulty = difficulty;
    }
}

public class LowerMusicVolumeEvent : GameEvent
{
    public bool shouldLower = true;
    public LowerMusicVolumeEvent(bool shouldLower)
    {
        this.shouldLower = shouldLower;
    }
}