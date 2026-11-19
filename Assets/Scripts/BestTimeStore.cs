using UnityEngine;

/// <summary>
/// Persists the best (fastest) elapsed time on-device and shows it on the
/// strip and the results screen. Stored in PlayerPrefs, so it survives
/// restarts of the game on the same PC.
/// </summary>
public class BestTimeStore : MonoBehaviour
{
    private const string BestTimeKey = "DragRacing.BestElapsedTime";
    private const string BestTuningKey = "DragRacing.BestTuning";

    /// <summary>Best elapsed time in seconds, or -1 if no run recorded yet.</summary>
    public float BestElapsedTime { get; private set; } = -1f;
    public string BestTuning { get; private set; } = "";
    public bool HasBestTime => BestElapsedTime > 0f;

    private void Awake()
    {
        BestElapsedTime = PlayerPrefs.GetFloat(BestTimeKey, -1f);
        BestTuning = PlayerPrefs.GetString(BestTuningKey, "");
    }

    /// <summary>Returns true when the run set a new best time.</summary>
    public bool RecordIfBest(float elapsedSeconds, string tuningUsed = "")
    {
        if (elapsedSeconds <= 0f) return false;
        if (HasBestTime && elapsedSeconds >= BestElapsedTime) return false;
        BestElapsedTime = elapsedSeconds;
        BestTuning = tuningUsed;
        PlayerPrefs.SetFloat(BestTimeKey, BestElapsedTime);
        PlayerPrefs.SetString(BestTuningKey, BestTuning);
        PlayerPrefs.Save();
        return true;
    }

    public string DisplayText => HasBestTime
        ? $"Best: {BestElapsedTime:0.000}s" + (string.IsNullOrEmpty(BestTuning) ? "" : $" ({BestTuning})")
        : "Best: --.--";

    public void ClearBestTime()
    {
        BestElapsedTime = -1f;
        BestTuning = "";
        PlayerPrefs.DeleteKey(BestTimeKey);
        PlayerPrefs.DeleteKey(BestTuningKey);
    }
}
