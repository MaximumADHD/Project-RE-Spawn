using UnityEngine;
using System.Collections;

public class DoorScript : Invokable
{
    private Vector3 initialPos;
    private Vector3 goalPos;
    private float lerp = 0;
    private bool initialized = false;

    public AudioClip openingSound;
    public AudioClip closingSound;
    public AudioClip doorHaulted;

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
            Debug.LogError("Failed to initialize door '" + name + "': No 'Y Axis Goal' was defined.");
        }
    }

    public override void OnActiveChanged(bool newState)
    {
        if (newState)
        {
            AudioSource.PlayClipAtPoint(openingSound, transform.localPosition);
        }
        else
        {
            AudioSource.PlayClipAtPoint(closingSound, transform.localPosition);
        }
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
            transform.localPosition = new Vector3(x, y, 0);
        }
    }

    public void OnCollisionEnter2D(Collision2D hit)
    {
        // See if we can crush the player with the door.
        if (hit.gameObject.name == "Player")
        {
            Player player = GameObject.FindObjectOfType<Player>();
            if (!player.Dead)
            {
                Vector3 pos = transform.localPosition;
                Vector3 size = transform.localScale;
                float minX = pos.x - (size.x / 2);
                float maxX = pos.x + (size.x / 2);
                float bottomY = pos.y - (size.y / 2);
                foreach (ContactPoint2D contact in hit.contacts)
                {
                    if (contact.point.x >= minX && contact.point.x <= maxX)
                    {
                        if (contact.point.y <= bottomY)
                        {
                            Debug.Log("SQUASH");
                            player.Kill("Watch out for doors!\nThey can and will crush you.");
                            player.Dead = true;
                        }
                    }
                }
            }
        }
    }
}
