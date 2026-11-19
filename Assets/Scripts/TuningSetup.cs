using UnityEngine;

/// <summary>
/// The single tuning variable for the next run: launch RPM.
/// Adjust it on the pre-run setup screen, then launch and compare the
/// elapsed time against the target and the persisted best time.
/// </summary>
public class TuningSetup : MonoBehaviour
{
    [Header("Launch RPM tuning")]
    [Range(1500f, 7500f)]
    public float launchRpm = 3500f;

    public float minLaunchRpm = 1500f;
    public float maxLaunchRpm = 7500f;
    public float stepRpm = 250f;

    public float LaunchRpm => Mathf.Clamp(launchRpm, minLaunchRpm, maxLaunchRpm);

    public void IncreaseLaunchRpm() => launchRpm = Mathf.Min(maxLaunchRpm, launchRpm + stepRpm);
    public void DecreaseLaunchRpm() => launchRpm = Mathf.Max(minLaunchRpm, launchRpm - stepRpm);

    /// <summary>Note the tuning used for a run, for the results screen.</summary>
    public string Describe() => $"Launch RPM {LaunchRpm:0} (range {minLaunchRpm:0}-{maxLaunchRpm:0})";
}
