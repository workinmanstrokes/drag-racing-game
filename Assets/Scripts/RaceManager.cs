using UnityEngine;

/// <summary>
/// Owns the run: Christmas-tree sequence, launch, timing, false start,
/// finish, win/lose vs the target time, and instant restart.
/// Attach to the DragStrip root object next to CarController.
/// </summary>
public class RaceManager : MonoBehaviour
{
    public enum RaceState { Waiting, Staging, Tree, Running, FalseStart, Finished }

    [Header("References")]
    public CarController car;
    public Transform startLine;
    public Transform finishLine;

    [Header("Run setup")]
    public float stripLengthMeters = 402.336f; // quarter mile
    public float targetTimeSeconds = 12.5f;    // target time to beat
    public float treeStepSeconds = 0.5f;       // time between tree lights

    [Header("State (read-only in play)")]
    public RaceState state = RaceState.Waiting;
    public float elapsedTime;
    public float reactionTime;
    public bool won;
    public int treeLightsLit;

    private float _treeTimer;
    private float _greenTime;
    private float _launchInputTime;
    private BestTimeStore _bestTime;
    private TuningSetup _tuning;

    private void Awake()
    {
        _bestTime = GetComponent<BestTimeStore>() ?? gameObject.AddComponent<BestTimeStore>();
        _tuning = GetComponent<TuningSetup>() ?? gameObject.AddComponent<TuningSetup>();
    }

    private void Start()
    {
        Restart();
    }

    /// <summary>Instant restart: back to staging with no menu reload.</summary>
    public void Restart()
    {
        car.ResetCar(startLine.position);
        elapsedTime = 0f;
        reactionTime = 0f;
        won = false;
        treeLightsLit = 0;
        _treeTimer = 0f;
        state = RaceState.Staging;
    }

    public void OnAccelerate(float value)
    {
        if (value > 0.1f)
        {
            if (state == RaceState.Staging || state == RaceState.Tree)
            {
                // Launched before green: red light, false start.
                state = RaceState.FalseStart;
                car.canDrive = false;
                return;
            }
            if (state == RaceState.Running && _launchInputTime <= 0f)
                _launchInputTime = Time.time;
        }
        car.SetAccelerate(value);
    }

    public void OnShiftUp() => car.ShiftUp();

    public void OnRestart() => Restart();

    private void Update()
    {
        switch (state)
        {
            case RaceState.Staging:
                // Tree starts as soon as the car is staged.
                state = RaceState.Tree;
                _treeTimer = 0f;
                break;

            case RaceState.Tree:
                _treeTimer += Time.deltaTime;
                treeLightsLit = Mathf.Min(3, 1 + (int)(_treeTimer / treeStepSeconds));
                if (_treeTimer >= treeStepSeconds * 3f)
                {
                    state = RaceState.Running;
                    _greenTime = Time.time;
                    _launchInputTime = 0f;
                    car.Launch(_tuning.LaunchRpm);
                }
                break;

            case RaceState.Running:
                elapsedTime = Time.time - _greenTime;
                if (_launchInputTime > 0f && reactionTime <= 0f)
                    reactionTime = _launchInputTime - _greenTime;
                if (car.transform.position.z >= finishLine.position.z)
                    FinishRun();
                break;
        }
    }

    private void FinishRun()
    {
        state = RaceState.Finished;
        car.canDrive = false;
        won = elapsedTime <= targetTimeSeconds;
        _bestTime.RecordIfBest(elapsedTime);
    }
}
