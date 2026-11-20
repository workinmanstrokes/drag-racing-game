using UnityEngine;

/// <summary>
/// Builds the visible strip the shipped scene lacked: ground, a quarter-mile
/// strip, centre line, start/finish lines and a simple box car body, all
/// from primitives, so the smallest drivable proof is visible the first time
/// the scene is played. Only builds what is missing: if the scene already
/// has start/finish markers or a car mesh, those are left alone.
/// The art is placeholder primitives on purpose — real car/strip art comes
/// after the driving loop is proven, not before.
/// </summary>
public class DragStripBuilder : MonoBehaviour
{
    public RaceManager raceManager;
    public CarController car;

    private void Awake()
    {
        if (raceManager == null) raceManager = GetComponent<RaceManager>();
        if (car == null) car = GetComponent<CarController>();
        if (raceManager == null) return;

        float length = raceManager.stripLengthMeters;
        Vector3 start = raceManager.StartPosition;
        float midZ = start.z + length * 0.5f;

        // Ground, then the asphalt strip, then a centre line.
        Box("Ground", new Vector3(220f, 0.1f, length + 120f), new Vector3(start.x, -0.06f, midZ), new Color(0.23f, 0.32f, 0.20f));
        Box("Strip", new Vector3(10f, 0.1f, length), new Vector3(start.x, 0f, midZ), new Color(0.13f, 0.13f, 0.14f));
        Box("CentreLine", new Vector3(0.15f, 0.02f, length), new Vector3(start.x, 0.06f, midZ), Color.white);

        BuildCarBody();
        if (raceManager.startLine == null) raceManager.startLine = Marker("StartLine", new Vector3(start.x, 0f, start.z), Color.white);
        if (raceManager.finishLine == null) raceManager.finishLine = Marker("FinishLine", new Vector3(start.x, 0f, start.z + length), new Color(1f, 0.55f, 0.1f));
    }

    private void BuildCarBody()
    {
        if (car == null) return;
        // A mesh added later in the editor wins: do not stack a box on it.
        if (car.GetComponentInChildren<Renderer>() != null) return;

        var body = GameObject.CreatePrimitive(PrimitiveType.Cube);
        body.name = "CarBody (placeholder)";
        body.transform.SetParent(car.transform, false);
        body.transform.localPosition = new Vector3(0f, 0.5f, 0f);
        body.transform.localScale = new Vector3(2f, 0.6f, 4.5f);
        body.GetComponent<Renderer>().material.color = new Color(0.75f, 0.15f, 0.15f);

        var cabin = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cabin.name = "CarCabin (placeholder)";
        cabin.transform.SetParent(car.transform, false);
        cabin.transform.localPosition = new Vector3(0f, 1f, -0.3f);
        cabin.transform.localScale = new Vector3(1.7f, 0.5f, 2f);
        cabin.GetComponent<Renderer>().material.color = new Color(0.1f, 0.1f, 0.12f);
    }

    private static Transform Marker(string name, Vector3 position, Color color)
    {
        var line = GameObject.CreatePrimitive(PrimitiveType.Cube);
        line.name = name;
        line.transform.position = new Vector3(position.x, 0.06f, position.z);
        line.transform.localScale = new Vector3(10f, 0.02f, 0.5f);
        line.GetComponent<Renderer>().material.color = color;
        return line.transform;
    }

    private static GameObject Box(string name, Vector3 scale, Vector3 position, Color color)
    {
        var box = GameObject.CreatePrimitive(PrimitiveType.Cube);
        box.name = name;
        box.transform.position = position;
        box.transform.localScale = scale;
        box.GetComponent<Renderer>().material.color = color;
        return box;
    }
}
