using UnityEngine;
using System.Collections;

public class DoorScript : Invokable
{
    private Vector3 initialPos;
    private Vector3 goalPos;
    private float lerp = 0;
    private bool initialized = false;

    public GameObject yAxisGoal;
    public float lerpSpeed = 0.1f;
    private float glideTorwards(float val, float desired, float increment)
    {
        if (val > desired)
        {
            return Mathf.Max(val - increment, desired);
        }
        else if (val < desired)
        {
            return Mathf.Min(val + increment, desired);
        }
        else
        {
            return desired;
        }
    }

    public void Start()
    {
        if (yAxisGoal != null)
        {
            initialized = true;
            initialPos = transform.localPosition;
            goalPos = initialPos - new Vector3(0, yAxisGoal.transform.localPosition.y - transform.localPosition.y, 0);
        }
        else
        {
            Debug.LogError("Failed to initialize door: No 'Y Axis Goal' was defined.");
        }
    }

    public override void OnActiveChanged(bool newState)
    {
        // TODO: Make it play a sound here
    }

    public void Update()
    {
        if (initialized)
        {
            if (isActive)
            {
                lerp = glideTorwards(lerp, 1, lerpSpeed);
            }
            else
            {
                lerp = glideTorwards(lerp, 0, lerpSpeed);
            }
            float x = initialPos.x + ((goalPos.x - initialPos.x) * lerp);
            float y = initialPos.y + ((goalPos.y - initialPos.y) * lerp);
            this.transform.localPosition = new Vector3(x, y, 0);
        }
    }
}
