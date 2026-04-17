using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyProgression : MonoBehaviour
{
    public enum DifficultyLevel
    {
        Easy,
        Normal,
        Difficult,
    }

    public DifficultyLevel currentDifficulty = DifficultyLevel.Difficult;
    public float normalThreshold = 0.66f;
    public float difficultThreshold = 0.33f;

    static readonly Dictionary<DifficultyLevel, int> spawnCounts = new Dictionary<DifficultyLevel, int>()
    {
        { DifficultyLevel.Easy, 1 },
        { DifficultyLevel.Normal, 1 },
        { DifficultyLevel.Difficult, 2 },
    };

    static readonly Dictionary<DifficultyLevel, bool> ghostAllowance = new Dictionary<DifficultyLevel, bool>()
    {
        { DifficultyLevel.Easy, false },
        { DifficultyLevel.Normal, true },
        { DifficultyLevel.Difficult, true },
    };

    public void UpdateDifficulty(float netWorthRatio)
    {
        DifficultyLevel newDifficulty;
        if (netWorthRatio < difficultThreshold) newDifficulty = DifficultyLevel.Difficult;
        else if (netWorthRatio < normalThreshold) newDifficulty = DifficultyLevel.Normal;
        else newDifficulty = DifficultyLevel.Easy;

        if (newDifficulty != currentDifficulty)
        {
            currentDifficulty = newDifficulty;
            EventManager.Broadcast(new DifficultyChangedEvent(currentDifficulty));
            StartCoroutine(PlayDifficultyLine());
        }
    }

    IEnumerator PlayDifficultyLine()
    {
        switch (currentDifficulty)
        {
            case DifficultyLevel.Easy:
                GameManager.GetManager().PlayVoiceLine("GHOST_LINE_EASY");
                yield return new WaitForSeconds(3.0f);
                GameManager.GetManager().PlayVoiceLine("PLAYER_LINE_EASY");
                break;
            case DifficultyLevel.Normal:
                GameManager.GetManager().SetGracePeriod(10.0f);
                GameManager.GetManager().PlayVoiceLine("GHOST_LINE_NORMAL");
                yield return new WaitForSeconds(4.5f);
                GameManager.GetManager().PlayVoiceLine("PLAYER_LINE_NORMAL");
                break;
            case DifficultyLevel.Difficult:
                GameManager.GetManager().SetGracePeriod(10.0f);
                GameManager.GetManager().PlayVoiceLine("GHOST_LINE_DIFFICULT");
                yield return new WaitForSeconds(3.5f);
                GameManager.GetManager().PlayVoiceLine("PLAYER_LINE_DIFFICULT");
                break;
        }
    }

    public int GetSpawnCount() { return spawnCounts[currentDifficulty]; }
    public bool IsGhostAllowed() { return ghostAllowance[currentDifficulty]; }
}