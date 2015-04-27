// Max G 2015 <3
// Player.cs

using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    public bool Jumping = false;
    public bool OnGround = true;
    public bool Dead = false;
    public bool InPortal = false;
    public float JumpForce = 250;
    public float MoveSpeed = 2;
    public float DesiredCameraDist = 3;
    public float CameraDistTweenRate = 0.2f;
    public float CharacterScale = 1.5f;
    public int respawnDelay = 120;
    public GameObject blood;
    public GameObject InitialSpawn;
    public Camera myCamera;
    public AudioClip walkSound;
    public AudioClip deathSound;
    public AudioClip jumpSound;
    public AudioClip landSound;
    public AudioClip respawnSound;
    public GuiStylePreset DeathScreenUI;
    public SpriteSheet idleAnim;
    public SpriteSheet walkAnim;
    public SpriteSheet jumpAnim;

    private int deathSequenceState = 0;
    private int lastSequence = -1;
    private float myCameraDist = 3;
    private bool facingRight = true;
    private bool cameraGoalActive = false;
    private int jumpCoolDown = 0;
    private SpawnLocation currentSpawn;
    private Vector3 cameraGoal;
    private SpriteRenderer sprite;
    private AudioSource walkClip;
    private string deathCause;
    private SpriteSheet currentSprite;

    private void setLocalScale(float x = 1, float y = 1, float z = 1)
    {
        transform.localScale = new Vector3(x * CharacterScale, y * CharacterScale, z * CharacterScale);
    }

    private bool isEditorMode()
    {
        GameObject editorObj = GameObject.FindGameObjectWithTag("LevelEdit");
        return editorObj != null;
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

    private void updateClip(AudioSource clip, bool active)
    {
        if (active && !Dead)
        {
            if (!clip.isPlaying)
            {
                clip.volume = 1;
                clip.Play();
            }
        }
        else
        {
            clip.Stop();
        }
    }

    public void Kill(string cause)
    {
        if (!Dead && !InPortal)
        {
            deathCause = cause;
            Dead = true;
        }
    }

    public void Respawn(SpawnLocation respawn)
    {
        AudioSource.PlayClipAtPoint(respawnSound, respawn.transform.localPosition, 1);
        collider2D.enabled = true;
        facingRight = true;
        sprite.color = new Color(1, 1, 1, 1);
        rigidbody2D.WakeUp();
        rigidbody2D.transform.rotation = new Quaternion();
        rigidbody2D.transform.localPosition = respawn.transform.localPosition;
        setLocalScale();
        Dead = false;
        deathSequenceState = 0;
        cameraGoalActive = false;
    }

    public void setCurrentSprite(SpriteSheet newSprite)
    {
        if (currentSprite == null || !currentSprite.Equals(newSprite))
        {
            newSprite.Reset();
            currentSprite = newSprite;
        }
    }

    public void MovementUpdate()
    {
        if (!rigidbody2D.IsSleeping())
        {
            float translation = Input.GetAxis("Horizontal");
            rigidbody2D.velocity = new Vector2(translation * MoveSpeed, Mathf.Min(JumpForce, rigidbody2D.velocity.y));
            // Update Jump
            float vertical = Mathf.Max(0, Input.GetAxis("Vertical"));
            if (vertical > 0 && !Jumping && OnGround && jumpCoolDown == 10)
            {
                setCurrentSprite(jumpAnim);
                Jumping = true;
                OnGround = false;
                rigidbody2D.velocity = new Vector2();
                rigidbody2D.AddForce(new Vector2(0, JumpForce));
                if (!InPortal)
                {
                    AudioSource.PlayClipAtPoint(jumpSound, transform.localPosition,1);
                }
            }
            else
            {
                if (rigidbody2D.velocity.y < 0)
                {
                    Jumping = false;
                }
                if (jumpCoolDown != 10)
                {
                    jumpCoolDown++;
                }
                Vector2 myPos = new Vector2(transform.localPosition.x,transform.localPosition.y);
                Vector2 castFrom = myPos - new Vector2(0,transform.localScale.y/2);
                RaycastHit2D ray = Physics2D.Raycast(castFrom, castFrom + new Vector2(0, -100));
                if (ray.point.y - castFrom.y == 0)
                {
                    if (!OnGround && !Jumping)
                    {
                        OnGround = true;
                        AudioSource.PlayClipAtPoint(landSound, transform.localPosition,1);
                        jumpCoolDown = 0;
                    }
                }
                else if (!Jumping && OnGround)
                {
                    OnGround = false;
                }
            }
            // Update Audio & Animation
            bool walkClipState = (OnGround && !Jumping && translation.ToString() != "0" && !Dead);
            if (OnGround)
            {
                if (walkClipState)
                {
                    setCurrentSprite(walkAnim);
                }
                else
                {
                    setCurrentSprite(idleAnim);
                }
            }
            try
            {
                Sprite next = currentSprite.NextFrame();
                if (next != null)
                {
                    sprite.sprite = next;
                }
            }
            catch
            {
                // *awkward whistle*
            }
            updateClip(walkClip,walkClipState);
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
        else
        {
            walkClip.Stop();
        }
    }

    public void OnDied()
    {
        AudioSource.PlayClipAtPoint(deathSound, transform.localPosition,1);
        sprite.color = new Color(1, 1, 1, 0);
        collider2D.enabled = false;
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
        if (walkClip.isPlaying)
        {
            walkClip.Stop();
        }
        if (deathSequenceState == respawnDelay)
        {
            if (Input.GetMouseButtonDown(0))
            {
                deathSequenceState = respawnDelay+1; // Signals OnGUI to respawn the player.
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
            float tweenSpeed = 0.3f;
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
            int midScreenWidth = (int)(Screen.width / 2);
            int midScreenHeight = (int)(Screen.height / 2);
            if (deathSequenceState < respawnDelay)
            {
                if (currentSpawn)
                {
                    currentSpawn = null;
                }
                string deathMessage = "You died!";
                if (deathSequenceState > 60)
                {
                    deathMessage = deathMessage + "\n" + deathCause;
                }
                GUI.Label(new Rect(0, 0, Screen.width, Screen.height), deathMessage, DeathScreenUI.Style);
            }
            else
            {
                int count = 0;
                if (currentSpawn != null)
                {
                    Vector3 loc = currentSpawn.transform.localPosition;
                    transform.localPosition = loc;
                    rigidbody2D.Sleep();
                    sprite.color = new Color(1, 1, 1, 0.5f);
                    cameraGoal = new Vector3(loc.x, loc.y, loc.z - myCameraDist);
                    cameraGoalActive = true;
                }
                foreach (SpawnLocation spawn in GameObject.FindObjectsOfType<SpawnLocation>())
                {
                    if (spawn.isActive)
                    {

                        GUI.Label(new Rect(midScreenWidth - 150, midScreenHeight - 30,300,30), "RESPAWN AT:");
                        Rect size = new Rect(midScreenWidth-100, midScreenHeight + (count * 30), 200, 20);
                        GUIContent label = new GUIContent(spawn.name, spawn.name);
                        if (GUI.Button(size, label))
                        {
                            Respawn(spawn);
                        }
                        else if (GUI.tooltip == spawn.name)
                        {
                            currentSpawn = spawn;
                        }
                        count++;
                    }
                }
            }
        }
    }

    public void Start()
    {
        if (!isEditorMode())
        {
            // Initialize Variables.
            Jumping = false;
            OnGround = true;
            Dead = false;
            setLocalScale();
            sprite = GetComponent<SpriteRenderer>();
            GameObject rootPlayer = GameObject.Find("RootPlayer");
            rootPlayer.transform.localPosition = new Vector3();
            // Initialize Looping Sound Clips
            walkClip = gameObject.AddComponent<AudioSource>();
            walkClip.clip = walkSound;
            walkClip.volume = 0;
            // Spawn the Player.
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
    }

    public void Update()
    {
        if (isEditorMode())
        {
            transform.localPosition = new Vector3(99999,99999,99999);
            renderer.enabled = false;
        }
        else
        {
            renderer.enabled = true;
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
}
