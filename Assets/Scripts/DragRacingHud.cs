using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The strip HUD and results screen the shipped build lacked: Christmas-tree
/// lights, RPM / gear / speed, elapsed + reaction time, target time, best
/// time, launch-RPM tuning and the win/lose + false-start messages. Builds
/// its own Canvas from UnityEngine.UI at runtime, so no scene UI setup is
/// needed; assign the Text fields in the Inspector to use hand-placed UI
/// instead of the auto-built canvas.
/// </summary>
public class DragRacingHud : MonoBehaviour
{
    [Header("References (auto-found when empty)")]
    public RaceManager raceManager;
    public CarController car;
    public BestTimeStore bestTime;
    public TuningSetup tuning;

    [Header("UI (auto-built when empty)")]
    public Text treeText;
    public Text hudText;
    public Text statusText;
    public Text resultText;
    public Text bestText;
    public Text tuningText;

    private void Awake()
    {
        if (raceManager == null) raceManager = GetComponent<RaceManager>();
        if (car == null) car = GetComponent<CarController>();
        if (bestTime == null) bestTime = GetComponent<BestTimeStore>();
        if (tuning == null) tuning = GetComponent<TuningSetup>();
    }

    private void Start()
    {
        if (treeText == null || hudText == null || statusText == null ||
            resultText == null || bestText == null || tuningText == null)
            BuildCanvas();
    }

    private void Update()
    {
        if (raceManager == null || car == null) return;

        // Christmas tree.
        switch (raceManager.state)
        {
            case RaceManager.RaceState.Staging:
                treeText.text = "STAGE...";
                break;
            case RaceManager.RaceState.Tree:
                treeText.text = new string('●', raceManager.treeLightsLit) + new string('○', 3 - raceManager.treeLightsLit) + "  WAIT FOR GREEN";
                break;
            case RaceManager.RaceState.Running:
                treeText.text = "●●●  GREEN — GO!";
                break;
            case RaceManager.RaceState.FalseStart:
                treeText.text = "RED LIGHT";
                break;
            case RaceManager.RaceState.Finished:
                treeText.text = "FINISH";
                break;
            default:
                treeText.text = string.Empty;
                break;
        }

        string reaction = raceManager.reactionTime > 0f ? raceManager.reactionTime.ToString("0.000") + "s" : "--.---";
        hudText.text = $"RPM {car.rpm:0}   Gear {car.gear}   {car.speedKmh:0} km/h\nTime {raceManager.elapsedTime:0.000}s   Reaction {reaction}";

        statusText.text = $"Target {raceManager.targetTimeSeconds:0.000}s — beat it to win\nHold SPACE to accelerate — E to shift — R to restart";
        if (tuning != null) tuningText.text = tuning.Describe();
        if (bestTime != null) bestText.text = bestTime.DisplayText;

        switch (raceManager.state)
        {
            case RaceManager.RaceState.Finished:
            {
                string verdict = raceManager.won
                    ? $"YOU WIN — {raceManager.elapsedTime:0.000}s beats the {raceManager.targetTimeSeconds:0.000}s target"
                    : $"YOU LOSE — {raceManager.elapsedTime:0.000}s vs the {raceManager.targetTimeSeconds:0.000}s target";
                string newBest = bestTime != null && bestTime.HasBestTime &&
                    Mathf.Approximately(bestTime.BestElapsedTime, raceManager.elapsedTime)
                    ? "  NEW BEST!"
                    : string.Empty;
                resultText.text = verdict + newBest + "\nPress R for an instant restart.";
                break;
            }
            case RaceManager.RaceState.FalseStart:
                resultText.text = "FALSE START — you launched before green.\nPress R to restart.";
                break;
            default:
                resultText.text = string.Empty;
                break;
        }
    }

    private void BuildCanvas()
    {
        var canvasGo = new GameObject("DragRacingHudCanvas");
        var canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGo.AddComponent<CanvasScaler>();
        canvasGo.AddComponent<GraphicRaycaster>();

        treeText = MakeText(canvasGo.transform, "TreeText", new Vector2(0.5f, 1f), new Vector2(0f, -50f), new Vector2(900f, 60f), 36, TextAnchor.MiddleCenter);
        hudText = MakeText(canvasGo.transform, "HudText", new Vector2(0f, 1f), new Vector2(20f, -120f), new Vector2(520f, 110f), 26, TextAnchor.UpperLeft);
        statusText = MakeText(canvasGo.transform, "StatusText", new Vector2(1f, 1f), new Vector2(-20f, -60f), new Vector2(700f, 70f), 22, TextAnchor.UpperRight);
        tuningText = MakeText(canvasGo.transform, "TuningText", new Vector2(1f, 1f), new Vector2(-20f, -130f), new Vector2(700f, 40f), 22, TextAnchor.UpperRight);
        bestText = MakeText(canvasGo.transform, "BestText", new Vector2(1f, 1f), new Vector2(-20f, -170f), new Vector2(700f, 40f), 22, TextAnchor.UpperRight);
        resultText = MakeText(canvasGo.transform, "ResultText", new Vector2(0.5f, 0.5f), new Vector2(0f, 90f), new Vector2(900f, 120f), 32, TextAnchor.MiddleCenter);
    }

    private static Text MakeText(Transform parent, string name, Vector2 anchor, Vector2 anchoredPosition, Vector2 size, int fontSize, TextAnchor alignment)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var text = go.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = fontSize;
        text.alignment = alignment;
        text.color = Color.white;
        var outline = go.AddComponent<Outline>();
        outline.effectColor = Color.black;
        outline.effectDistance = new Vector2(2f, -2f);
        var rect = go.GetComponent<RectTransform>();
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = size;
        return text;
    }
}
