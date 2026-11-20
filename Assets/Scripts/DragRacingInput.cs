using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Wires the frozen Input System map to RaceManager: Accelerate (hold),
/// ShiftUp and Restart. This is the piece the shipped skeleton was missing —
/// the DragRacing.inputactions asset existed, but nothing ever read it, so
/// no key or button could drive the car.
///
/// Assign the frozen asset (Assets/InputActions/DragRacing.inputactions) in
/// the Inspector; that asset is the canonical map and its bindings must not
/// change without updating the README. If no asset is assigned, the
/// identical frozen bindings are built in code so a fresh checkout is
/// drivable with no editor setup — the fallback only ever builds the same
/// bindings, it never invents different ones.
/// </summary>
public class DragRacingInput : MonoBehaviour
{
    [Header("Frozen input map")]
    [Tooltip("Frozen map: Assets/InputActions/DragRacing.inputactions. Assign it here; if left empty the identical bindings are built in code as a fallback.")]
    public InputActionAsset actions;

    public RaceManager raceManager;

    private InputActionMap _map;
    private InputAction _accelerate;
    private InputAction _shiftUp;
    private InputAction _restart;

    private void Awake()
    {
        if (raceManager == null) raceManager = GetComponent<RaceManager>();
        if (actions == null)
        {
            actions = BuildFallbackMap();
            Debug.LogWarning("[DragRacingInput] No InputActionAsset assigned — using the built-in copy of the frozen Driving map. Assign DragRacing.inputactions in the Inspector to use the canonical asset.");
        }
    }

    private void OnEnable()
    {
        if (actions == null || raceManager == null) return;
        _map = actions.FindActionMap("Driving");
        if (_map == null)
        {
            Debug.LogError("[DragRacingInput] Driving map not found in the assigned InputActionAsset.");
            return;
        }
        _accelerate = _map.FindAction("Accelerate");
        _shiftUp = _map.FindAction("ShiftUp");
        _restart = _map.FindAction("Restart");
        if (_accelerate == null || _shiftUp == null || _restart == null)
        {
            Debug.LogError("[DragRacingInput] Driving map is missing Accelerate, ShiftUp or Restart.");
            return;
        }
        _accelerate.performed += OnAcceleratePerformed;
        _accelerate.canceled += OnAccelerateCanceled;
        _shiftUp.performed += OnShiftUpPerformed;
        _restart.performed += OnRestartPerformed;
        _map.Enable();
    }

    private void OnDisable()
    {
        if (_accelerate != null)
        {
            _accelerate.performed -= OnAcceleratePerformed;
            _accelerate.canceled -= OnAccelerateCanceled;
        }
        if (_shiftUp != null) _shiftUp.performed -= OnShiftUpPerformed;
        if (_restart != null) _restart.performed -= OnRestartPerformed;
        if (_map != null) _map.Disable();
    }

    private void OnAcceleratePerformed(InputAction.CallbackContext ctx) => raceManager.OnAccelerate(ctx.ReadValue<float>());
    private void OnAccelerateCanceled(InputAction.CallbackContext ctx) => raceManager.OnAccelerate(0f);
    private void OnShiftUpPerformed(InputAction.CallbackContext ctx) => raceManager.OnShiftUp();
    private void OnRestartPerformed(InputAction.CallbackContext ctx) => raceManager.OnRestart();

    /// <summary>Builds the frozen Driving map in code: Space / Right Trigger to accelerate, E / Right Bumper to shift up, R / Y-Triangle to restart.</summary>
    private static InputActionAsset BuildFallbackMap()
    {
        var asset = ScriptableObject.CreateInstance<InputActionAsset>();
        var map = asset.AddActionMap("Driving");

        var accelerate = map.AddAction("Accelerate", InputActionType.Button);
        accelerate.AddBinding("<Keyboard>/space");
        accelerate.AddBinding("<Gamepad>/rightTrigger");

        var shiftUp = map.AddAction("ShiftUp", InputActionType.Button);
        shiftUp.AddBinding("<Keyboard>/e");
        shiftUp.AddBinding("<Gamepad>/rightShoulder");

        var restart = map.AddAction("Restart", InputActionType.Button);
        restart.AddBinding("<Keyboard>/r");
        restart.AddBinding("<Gamepad>/buttonNorth");

        return asset;
    }
}
