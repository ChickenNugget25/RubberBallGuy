using UnityEngine;

public class Fan : MonoBehaviour
{
    public float force = 20f;
    public bool isOn = true;
    public bool cycleOnOff = false;
    public float cycleTimeSeconds = 5f;
    public Vector2 forceDirection = Vector2.up;

    public AnimationCurve falloffCurve = AnimationCurve.Linear(0, 1, 1, 0); // fan falloff curve
}
