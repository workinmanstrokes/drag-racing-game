using UnityEngine;

/// <summary>
/// Single drag car: hold-to-accelerate, manual shift-up, RPM and speed.
/// The car only moves forward along the strip (+Z). Launch RPM comes from
/// TuningSetup and sets the RPM the car leaves the line with.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviour
{
    [Header("Engine")]
    public float maxRpm = 8000f;
    public float redlineRpm = 7500f;
    public float idleRpm = 1200f;
    public float rpmGainPerSecond = 4200f;
    public float rpmDropOnShift = 2600f;

    [Header("Gears")]
    public int gearCount = 5;
    public float[] gearRatios = { 3.4f, 2.3f, 1.7f, 1.3f, 1.0f };
    public float finalDrive = 3.9f;
    public float wheelRadius = 0.34f;

    [Header("State (read-only in play)")]
    public int gear = 1;
    public float rpm;
    public float speedKmh;
    public bool accelerating;
    public bool canDrive;

    private Rigidbody _rb;
    private float _launchRpm;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.constraints = RigidbodyConstraints.FreezeRotationX
                        | RigidbodyConstraints.FreezeRotationZ
                        | RigidbodyConstraints.FreezePositionX;
        rpm = idleRpm;
    }

    /// <summary>Called by RaceManager at the green light.</summary>
    public void Launch(float launchRpm)
    {
        _launchRpm = Mathf.Clamp(launchRpm, idleRpm, redlineRpm);
        rpm = _launchRpm;
        gear = 1;
        canDrive = true;
    }

    public void ResetCar(Vector3 startPosition)
    {
        transform.position = startPosition;
        transform.rotation = Quaternion.identity;
        _rb.linearVelocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
        rpm = idleRpm;
        gear = 1;
        speedKmh = 0f;
        canDrive = false;
        accelerating = false;
    }

    /// <summary>Called every frame with the held accelerate input (0 or 1).</summary>
    public void SetAccelerate(float value)
    {
        accelerating = value > 0.1f;
    }

    public void ShiftUp()
    {
        if (!canDrive || gear >= gearCount) return;
        gear++;
        rpm = Mathf.Max(idleRpm, rpm - rpmDropOnShift);
    }

    private void FixedUpdate()
    {
        if (!canDrive) return;

        if (accelerating)
        {
            rpm = Mathf.Min(maxRpm, rpm + rpmGainPerSecond * Time.fixedDeltaTime);
            float engineForce = (rpm / maxRpm) * gearRatios[gear - 1] * finalDrive * 1400f;
            _rb.AddForce(transform.forward * engineForce, ForceMode.Force);
        }
        else
        {
            rpm = Mathf.Max(idleRpm, rpm - rpmGainPerSecond * 0.5f * Time.fixedDeltaTime);
        }

        // Simple drag so top speed is bounded per gear.
        float topSpeed = 28f + gear * 14f; // m/s, grows with gear
        if (_rb.linearVelocity.magnitude > topSpeed)
            _rb.linearVelocity = _rb.linearVelocity.normalized * topSpeed;

        speedKmh = _rb.linearVelocity.magnitude * 3.6f;
    }
}
