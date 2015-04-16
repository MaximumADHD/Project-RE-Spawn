// Max G 2015 <3
// Player.cs

using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    public bool Jumping = false;
    public bool OnGround = true;
    public bool Dead = false;
    public float JumpForce = 250;
    public float MoveSpeed = 2;
    public float DesiredCameraDist = 3;
    public float CameraDistTweenRate = 0.2f;
    public int respawnDelay = 120; // Time (In Frames) to wait before letting the player respawn.
    public int levelTime = 0;
    public GameObject blood;
    public GameObject InitialSpawn;
    public Camera myCamera;

    private float lastY = 0;
    private int deathSequenceState = 0;
    private int lastSequence = 0;
    private float myCameraDist = 3;
    private bool facingRight = true;
    private bool cameraGoalActive = false;
    private Vector3 cameraGoal;
    private SpriteRenderer sprite;

    private void setLocalScale(float x = 1, float y = 1, float z = 1)
    {
        transform.localScale = new Vector3(x, y, z);
    }

    private float glideTorwards(float val, float desired, float increment)
    {
        // Takes a number, and tries to move it to a desired value using +/- increment and Mathf.Max/Min
        if (val > desired)
        {
            return Mathf.Max(val - increment,desired);
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

    public void Respawn(GameObject respawn)
    {
        facingRight = true;
        sprite.color = new Color(1, 1, 1, 1);
        setLocalScale(1, 1, 1);
        rigidbody2D.WakeUp();
        rigidbody2D.transform.rotation = new Quaternion();
        rigidbody2D.transform.localPosition = respawn.transform.localPosition;
        Dead = false;
        deathSequenceState = 0;
        cameraGoalActive = false;
    }

    public void MovementUpdate()
    {
        float translation = Input.GetAxis("Horizontal");
        rigidbody2D.velocity = new Vector2(translation * MoveSpeed, Mathf.Min(JumpForce, rigidbody2D.velocity.y));
        // Update Jump
        float vertical = Mathf.Max(0, Input.GetAxis("Vertical"));
        if (vertical > 0 && !Jumping && OnGround)
        {
            Jumping = true;
            OnGround = false;
            rigidbody2D.AddForce(new Vector2(0, JumpForce));
        }
        else
        {
            if (rigidbody2D.velocity.y < 0)
            {
                OnGround = false;
                Jumping = false;
            }
            else if ((lastY <= 0 && lastY > -0.0001) && !OnGround && !Jumping)
            {
                OnGround = true;
                rigidbody2D.velocity = new Vector2();
            }
        }
        lastY = rigidbody2D.velocity.y;
        // Update Rotation
        if (facingRight)
        {
            if (translation < -0.1)
            {
                facingRight = false;
                setLocalScale(-1);
            }
        }
        else
        {
            if (translation > 0.1)
            {
                facingRight = true;
                setLocalScale(1);
            }
        }
    }

    public void OnDied()
    {
        setLocalScale(0, 0, 0);
        rigidbody2D.Sleep();
        int particles = Random.Range(70, 100);
        for (int i = 0; i < particles; i++)
        {
            // SPLAT!
            Object.Instantiate(blood, transform.localPosition, new Quaternion()); 
        }
    }

    public void DeadUpdate()
    {
        if (deathSequenceState == respawnDelay)
        {
            if (Input.GetMouseButtonDown(0))
            {
                deathSequenceState = respawnDelay+1; // Signals OnGUI to draw the respawn buttons.
            }
        }
        if (lastSequence != deathSequenceState)
        {
            if (deathSequenceState < respawnDelay)
            {
                if (deathSequenceState == 0)
                {
                    // We just died.
                    OnDied();
                }
                // We need to wait a few frames before letting the player respawn.
                deathSequenceState++;
            }
        }
    }

    public void CameraUpdate()
    {
        if (cameraGoalActive)
        {
            Vector3 currentPos = myCamera.transform.localPosition;
            float tweenSpeed = 0.1f;
            float currentX = glideTorwards(currentPos.x, cameraGoal.x, tweenSpeed);
            float currentY = glideTorwards(currentPos.y, cameraGoal.y, tweenSpeed);
            float currentZ = glideTorwards(currentPos.z, cameraGoal.z, tweenSpeed);
            Vector3 newPos = new Vector3(currentX,currentY,currentZ);
            myCamera.transform.localPosition = newPos;
        }
        else if (!Dead)
        {
            Player player = GameObject.FindObjectOfType<Player>();
            Vector3 loc = player.transform.localPosition;
            myCameraDist = glideTorwards(myCameraDist, DesiredCameraDist, CameraDistTweenRate);
            myCamera.transform.localPosition = new Vector3(loc.x, loc.y, loc.z - myCameraDist);
        }
    }

    public void OnGUI()
    {
        if (deathSequenceState > 0)
        {
            Rect location = new Rect(20, 20, Screen.width, Screen.height);
            if (deathSequenceState < respawnDelay)
            {
                GUI.Label(location, "You have died...");
            }
            else
            {
                int count = 0;
                foreach (GameObject spawn in GameObject.FindGameObjectsWithTag("Spawn"))
                {
                    Rect size = new Rect(10, 10 + (count * 30), 100, 20);
                    GUIContent label = new GUIContent(spawn.name, spawn.name);
                    if (GUI.Button(size, label)) // If we are pressing the button.
                    {
                        Respawn(spawn);
                    }
                    else if (GUI.tooltip == spawn.name)
                    {
                        Vector3 loc = spawn.transform.localPosition;
                        setLocalScale(1, 1, 1);
                        transform.localPosition = loc;
                        rigidbody2D.Sleep();
                        sprite.color = new Color(1, 1, 1, 0.5f);
                        cameraGoal = new Vector3(loc.x, loc.y, loc.z - myCameraDist);
                        cameraGoalActive = true;
                    }
                    count++;
                }
            }
        }
    }

    public void Start()
    {
        // Initialize Variables.
        Jumping = false;
        OnGround = true;
        Dead = false;
        sprite = GetComponent<SpriteRenderer>();
        GameObject rootPlayer = GameObject.Find("RootPlayer");
        rootPlayer.transform.localPosition = new Vector3();
        if (InitialSpawn != null)
        {
            transform.localPosition = InitialSpawn.transform.localPosition;
            return;
        }
        Debug.LogWarning("Player.InitialSpawn was not properly set!");
        GameObject availableSpawn = GameObject.FindGameObjectWithTag("Spawn");
        if (availableSpawn != null)
        {
            transform.localPosition = availableSpawn.transform.localPosition;
        }
        else
        {
            Debug.LogError("CRITICAL ERROR: NO SPAWNS FOUND IN WORLD");
        }
    }

    public void Update()
    {
        if (!Dead)
        {
            MovementUpdate();
        }
        else
        {
            DeadUpdate();
        }
        CameraUpdate();
    }
}
