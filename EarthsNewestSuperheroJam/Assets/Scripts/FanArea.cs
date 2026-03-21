using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Diagnostics;

public class FanArea : MonoBehaviour
{
    private Fan fan;
    private bool cycleState = false;
    private float cycleTime = 0f;
    private bool doCycle = false;
    
    private float timer;

    private AnimationCurve falloffCurve = AnimationCurve.Linear(0, 1, 1, 0);

    private BoxCollider2D col;
    
    private HashSet<Rigidbody2D> bodiesInZone = new HashSet<Rigidbody2D>();
    
    private void Awake()
    {
        col = GetComponent<BoxCollider2D>();
        fan = GetComponentInParent<Fan>();
        falloffCurve = fan.falloffCurve;

        if(fan.cycleOnOff) {
            doCycle = true;
            cycleState = fan.isOn;
            cycleTime = fan.cycleTimeSeconds;

            timer = cycleTime;

        }
    }
    
    float getMaxDistance() {
        Vector2 dir = fan.transform.up.normalized;

        // Get world-space center of collider
        Vector2 center = (Vector2)transform.TransformPoint(col.offset);

        // Get half-size in world space
        Vector2 size = Vector2.Scale(col.size, transform.lossyScale);
        Vector2 extents = size * 0.5f;

        // Project extents onto direction
        float projectedExtent =
            Mathf.Abs(dir.x) * extents.x +
            Mathf.Abs(dir.y) * extents.y;

        // Distance from fan origin to center
        float centerDistance = Vector2.Dot(center - (Vector2)transform.position, dir);

        return centerDistance + projectedExtent;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Rigidbody2D rb = other.attachedRigidbody;
        if (rb != null)
        {
            bodiesInZone.Add(rb);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Rigidbody2D rb = other.attachedRigidbody;
        if (rb != null)
        {
            bodiesInZone.Remove(rb);
        }
    }


    void Update()
    {
        if(doCycle) {
            timer -= Time.deltaTime;
            if(timer <= 0f) {
                cycleState = !cycleState;
                timer = cycleTime;
            }
        }
    }

    private void FixedUpdate() {
        if(!fan.isOn) return; 
        if (doCycle && !cycleState) return; // off during cycle 

        foreach(var rb in bodiesInZone) {
            if(rb == null) continue; // in case something got destroyed while in the zone

            Vector2 fanPosition = fan.transform.position;
            Vector2 toObject = rb.position - fanPosition;
            Vector2 dir = fan.transform.up.normalized;

            float distance = Vector2.Dot(toObject, dir);
            if(distance < 0) continue; // Behind the fan, no wind

            float strength = falloffCurve.Evaluate(distance / getMaxDistance());
            Vector2 force = fan.transform.up * fan.force * strength;

            rb.AddForce(force); 
        }
        
        
    }
}
