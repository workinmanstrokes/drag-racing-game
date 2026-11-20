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

    /// <summary>Where the car stages. Falls back to the world origin when no start marker is assigned.</summary>
    public Vector3 StartPosition => startLine != null ? startLine.position : Vector3.zero;

    /// <summary>Finish-line Z. Falls back to start + strip length when no finish marker is assigned.</summary>
    public float FinishZ => finishLine != null ? finishLine.position.z : StartPosition.z + stripLengthMeters;

    private void Awake()
    {
        // Tester fix (VEX 1.2/10): the shipped scene left car / startLine /
        // finishLine unassigned, so Restart() threw a NullReferenceException
        // on Play and the game could never run. Resolve everything here so
        // opening the scene and pressing Play just works.
        if (car == null) car = GetComponent<CarController>();
        _bestTime = GetComponent<BestTimeStore>() ?? gameObject.AddComponent<BestTimeStore>();
        _tuning = GetComponent<TuningSetup>() ?? gameObject.AddComponent<TuningSetup>();
        if (GetComponent<DragStripBuilder>() == null) gameObject.AddComponent<DragStripBuilder>();
        if (GetComponent<DragRacingHud>() == null) gameObject.AddComponent<DragRacingHud>();
        if (GetComponent<DragRacingInput>() == null) gameObject.AddComponent<DragRacingInput>();
    }

    private void Start()
    {
        Restart();
    }

    /// <summary>Instant restart: back to staging with no menu reload.</summary>
    public void Restart()
    {
        if (car == null)
        {
            Debug.LogError("[RaceManager] No CarController on this object — cannot stage a run.");
            state = RaceState.Waiting;
            return;
        }
        car.ResetCar(StartPosition);
        elapsedTime = 0f;
        reactionTime = 0f;
        won = false;
        treeLightsLit = 0;
        _treeTimer = 0f;
        _launchInputTime = 0f;
        state = RaceState.Staging;
    }

    public void OnAccelerate(float value)
    {
        if (car == null) return;
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

    public void OnShiftUp()
    {
        if (car == null) return;
        car.ShiftUp();
    }

    public void OnRestart() => Restart();

    private void Update()
    {
        if (car == null) return;
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
                // Held through green (no fresh input event after the tree):
                // measure the reaction from the green light, not from 0.
                if (_launchInputTime <= 0f && car.accelerating)
                    _launchInputTime = Time.time;
                if (_launchInputTime > 0f && reactionTime <= 0f)
                    reactionTime = _launchInputTime - _greenTime;
                if (car.transform.position.z >= FinishZ)
                    FinishRun();
                break;
        }
    }

    private void FinishRun()
    {
        state = RaceState.Finished;
        car.canDrive = false;
        won = elapsedTime <= targetTimeSeconds;
        // Record the tuning that set the best time, so the results screen
        // can show what the best run was launched with.
        _bestTime.RecordIfBest(elapsedTime, _tuning.Describe());
    }
}
